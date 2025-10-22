using System.Collections.Immutable;
using System.Numerics;

using Domain.Combinations;
using Domain.Enums;
using Domain.Helpers;
using Domain.Main;

using Manager;

using PoE2BuildCalculator.Helpers;

namespace PoE2BuildCalculator
{
	public partial class MainForm : BaseForm, IDisposable
	{
		// Field for thread synchronization
		private readonly object _lockObject = new();
		private readonly long _maxCombinationsToStore = 10000000;
		private int _bestCombinationCount = 100;

		internal Func<List<Item>, bool> _itemValidatorFunction { get; set; } = x => true;
		internal ImmutableList<Item> _parsedItems { get; private set; } = [];

		// Class references
		private FileParser _fileParser { get; set; }
		private TierManager _tierManager { get; set; }
		private CustomValidator _customValidator { get; set; }
		private ProgressReportingHelper _progressHelper;

		private List<List<Item>> _combinations { get; set; } = [];
		private ToolStripProgressBar _statusProgressBar;
		private ToolStripStatusLabel StatusBarEstimate;
		private ToolStripButton _cancelButton;

		private CombinationFilterStrategy _filterStrategy = CombinationFilterStrategy.Balanced;
		private Dictionary<string, (double Min, double Max)> _statNormalization;

		// Speed estimation fields for combinations
		private double _smoothedSpeed = 0;
		private long _lastProcessedCount = 0;
		private DateTime _lastProgressTime = DateTime.MinValue;
		private int _progressReportCount = 0;
		private const int MIN_REPORTS_FOR_ESTIMATE = 3;  // Wait for 3 reports before showing estimate
		private const double SMOOTHING_FACTOR = 0.3;  // Weight for new speed (0.7 for old speed)

		private struct ScoredCombination
		{
			public List<Item> Combination;
			public double Score;

			public ScoredCombination(List<Item> combination, double score)
			{
				Combination = combination;
				Score = score;
			}
		}

		public MainForm()
		{
			InitializeComponent();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			ConfigureOpenFileDialog();
			ConfigureParserControls();

			// ✅ Validate controls were created
			if (_statusProgressBar == null || _cancelButton == null)
			{
				MessageBox.Show(
					"Failed to initialize progress controls. Some features may not work correctly.",
					"Initialization Warning",
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning);
				// Continue anyway - operations will work, just without progress UI
			}
			else
			{
				_progressHelper = new ProgressReportingHelper(
					_statusProgressBar,
					_cancelButton,
					StatusBarLabel,
					[PanelButtons, PanelConfig]);
			}

			this.FormClosing += MainForm_FormClosing;
			NumericBestCombinationsCount.Value = _bestCombinationCount;

			// Enable double buffering for smoother rendering
			SetStyle(ControlStyles.OptimizedDoubleBuffer |
					 ControlStyles.AllPaintingInWmPaint |
					 ControlStyles.UserPaint, true);
			UpdateStyles();
		}

		private void ButtonOpenItemListFile_Click(object sender, EventArgs e)
		{
			var dialogResult = OpenPoE2ItemList.ShowDialog(this);
			if (dialogResult == DialogResult.OK && !string.IsNullOrWhiteSpace(OpenPoE2ItemList.FileName) && File.Exists(OpenPoE2ItemList.FileName))
			{
				_fileParser = new FileParser(OpenPoE2ItemList.FileName);
				StatusBarLabel.Text = $"Loaded file: {OpenPoE2ItemList.FileName}";
			}
			else
			{
				StatusBarLabel.Text = "No file selected or file does not exist.";
			}
		}

		private async void ButtonParseItemListFile_Click(object sender, EventArgs e)
		{
			if (_fileParser is null)
			{
				StatusBarLabel.Text = "No file loaded. Please choose a file first.";
				return;
			}

			TextboxDisplay.Text = string.Empty;
			_progressHelper.Start();  // This disables PanelButtons
			StatusBarLabel.Text = "Parsing... 0%";

			var progress = new Progress<int>(p =>
			{
				_progressHelper.UpdateProgress(p, $"Parsing file... {p}%");
			});

			try
			{
				await _fileParser.ParseFileAsync(progress, _progressHelper.Token).ConfigureAwait(true);
				StatusBarLabel.Text = "Parsing completed.";
			}
			catch (OperationCanceledException)
			{
				StatusBarLabel.Text = "Parsing cancelled.";
			}
			catch (Exception ex)
			{
				StatusBarLabel.Text = $"Error: {ex.Message}";
				ErrorHelper.ShowError(ex, $"{nameof(MainForm)} - {nameof(ButtonParseItemListFile_Click)}");
			}
			finally
			{
				_progressHelper.Stop();  // ✅ This re-enables PanelButtons (which includes parse button)
				_parsedItems = _fileParser?.GetParsedItems() ?? [];
			}
		}

		private void ButtonCancelParse_Click(object sender, EventArgs e)
		{
			_progressHelper.Cancel();
		}

		#region Prepare controls

		private void ConfigureOpenFileDialog()
		{
			OpenPoE2ItemList.Multiselect = false;
			OpenPoE2ItemList.Title = "Select the PoE2 Item List File";
			OpenPoE2ItemList.AddToRecent = true;
			OpenPoE2ItemList.Filter = "Text Files|*.txt|All Files|*.*";
			OpenPoE2ItemList.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
		}

		private void ConfigureParserControls()
		{
			var parent = StatusBarLabel?.GetCurrentParent() ?? throw new InvalidOperationException("StatusBar not properly initialized. Cannot create progress controls.");

			_statusProgressBar = new ToolStripProgressBar
			{
				Name = "StatusBarProgressBar",
				Minimum = 0,
				Maximum = 100,
				Value = 0,
				Visible = false,
				Size = new Size(150, 16)
			};
			parent.Items.Add(_statusProgressBar);

			_cancelButton = new ToolStripButton
			{
				Name = "ButtonCancelParse",
				Text = "Cancel",
				BackColor = Color.LightPink,
				Enabled = false,
				Visible = false,
				Alignment = ToolStripItemAlignment.Right
			};
			_cancelButton.Click += ButtonCancelParse_Click;
			parent.Items.Add(_cancelButton);
		}

		#endregion

		#region Scoring

		private static HashSet<int> BuildTieredItemIds(ImmutableList<Item> items, List<Tier> tiers)
		{
			if (tiers == null || tiers.Count == 0)
				return null;

			var tieredStatNames = tiers
				.SelectMany(t => t.StatWeights.Keys)
				.ToHashSet(StringComparer.OrdinalIgnoreCase);

			var tieredIds = new HashSet<int>();

			foreach (var item in items)
			{
				if (ItemHasAnyTieredStat(item, tieredStatNames))
				{
					tieredIds.Add(item.Id);
				}
			}

			return tieredIds;
		}

		private static bool ItemHasAnyTieredStat(Item item, HashSet<string> tieredStatNames)
		{
			var itemStatsDict = ItemStatsHelper.ToDictionary(item.ItemStats);

			foreach (var statName in tieredStatNames)
			{
				if (itemStatsDict.TryGetValue(statName, out var value))
				{
					double numValue = Convert.ToDouble(value);
					if (numValue != 0) // Has non-zero value for this tiered stat
						return true;
				}
			}

			return false;
		}

		private static Dictionary<string, (double Min, double Max)> ComputeStatNormalization(ImmutableList<Item> items, List<Tier> tiers)
		{
			var result = new Dictionary<string, (double, double)>(StringComparer.OrdinalIgnoreCase);

			if (tiers == null || tiers.Count == 0)
				return result;

			var tieredStatNames = tiers
				.SelectMany(t => t.StatWeights.Keys)
				.ToHashSet(StringComparer.OrdinalIgnoreCase);

			foreach (var statName in tieredStatNames)
			{
				var values = new List<double>();

				foreach (var item in items)
				{
					var itemStatsDict = ItemStatsHelper.ToDictionary(item.ItemStats);
					if (itemStatsDict.TryGetValue(statName, out var value) && value is not string)
					{
						values.Add(Convert.ToDouble(value));
					}
				}

				if (values.Count == 0)
				{
					result[statName] = (0, 0);
					continue;
				}

				double min = values.Min();
				double max = values.Max();
				result[statName] = (min, max);
			}

			return result;
		}

		private static double NormalizeStat(double value, double min, double max)
		{
			if (Math.Abs(max - min) < 0.000001) // Essentially equal
				return 0.5; // All values same, use middle

			return (value - min) / (max - min); // Maps to [0, 1]
		}

		/// <summary>
		/// ✅ Cache tier weights to avoid repeated Sum() calls
		/// </summary>
		private static double ScoreCombination(List<Item> combination, List<Tier> tiers, Dictionary<string, (double Min, double Max)> normalization)
		{
			if (tiers == null || tiers.Count == 0)
				return 0;

			// ✅ Pre-compute ONCE per combination
			var itemStatsDicts = combination
				.Select(item => ItemStatsHelper.ToDictionary(item.ItemStats))
				.ToList();

			double totalScore = 0;
			double totalTierWeight = tiers.Sum(t => t.TierWeight);

			if (totalTierWeight == 0)
				return 0;

			foreach (var tier in tiers)
			{
				double tierScore = 0;
				double totalStatWeight = tier.TotalStatWeight;

				if (totalStatWeight == 0)
					continue;

				foreach (var (statName, statWeight) in tier.StatWeights)
				{
					if (statWeight == 0)
						continue;

					// Sum stat across all items in combination
					double rawSum = 0;
					foreach (var itemStatsDict in itemStatsDicts) // ✅ Use pre-computed dicts
					{
						if (itemStatsDict.TryGetValue(statName, out var value) && value is not string)
						{
							rawSum += Convert.ToDouble(value);
						}
					}

					// Normalize the sum
					double normalizedSum = 0;
					if (normalization.TryGetValue(statName, out var range))
					{
						normalizedSum = NormalizeStat(rawSum, range.Min, range.Max);
					}

					// Weight the stat within the tier
					tierScore += normalizedSum * (statWeight / totalStatWeight);
				}

				// Weight the tier
				totalScore += tierScore * (tier.TierWeight / totalTierWeight);
			}

			return totalScore;
		}

		#endregion

		#region Combination computation estimate

		// Add this method to MainForm class:
		private string CalculateEstimatedTime(long processedCombinations, BigInteger totalCombinations, DateTime now)
		{
			// Too early - not enough data
			if (_progressReportCount < MIN_REPORTS_FOR_ESTIMATE)
			{
				return "Calculating...";
			}

			// Calculate time since last report
			if (_lastProgressTime == DateTime.MinValue)
			{
				_lastProgressTime = now;
				_lastProcessedCount = processedCombinations;
				return "Calculating...";
			}

			double elapsedSeconds = (now - _lastProgressTime).TotalSeconds;

			// Avoid division by zero
			if (elapsedSeconds < 0.001)
			{
				return _smoothedSpeed > 0 ? FormatEstimate(processedCombinations, totalCombinations) : "Calculating...";
			}

			// Calculate current speed (combinations per second)
			long combinationsProcessed = processedCombinations - _lastProcessedCount;
			double currentSpeed = combinationsProcessed / elapsedSeconds;
			double adaptiveFactor = _progressReportCount < 10 ? 0.5 : SMOOTHING_FACTOR;

			// Update smoothed speed (exponential moving average)
			if (_smoothedSpeed == 0)
			{
				_smoothedSpeed = currentSpeed;  // First time - just use current
			}
			else
			{
				_smoothedSpeed = (1 - adaptiveFactor) * _smoothedSpeed + adaptiveFactor * currentSpeed;
			}

			// Update tracking variables
			_lastProgressTime = now;
			_lastProcessedCount = processedCombinations;

			// Calculate estimate
			return FormatEstimate(processedCombinations, totalCombinations);
		}

		private string FormatEstimate(long processedCombinations, BigInteger totalCombinations)
		{
			if (_smoothedSpeed <= 0)
			{
				return "Calculating...";
			}

			// Calculate remaining combinations
			BigInteger remaining = totalCombinations - processedCombinations;

			if (remaining <= 0)
			{
				return "Finishing...";
			}

			// Calculate estimated seconds remaining
			double remainingSeconds = CombinationGenerator.GetBigNumberRatio(remaining, new BigInteger((long)_smoothedSpeed));

			// Handle edge cases
			if (double.IsInfinity(remainingSeconds) || double.IsNaN(remainingSeconds) || remainingSeconds < 0)
			{
				return "Calculating...";
			}

			// Cap at reasonable maximum (10 years)
			if (remainingSeconds > TimeSpan.FromDays(3650).TotalSeconds)
			{
				return "Estimate time: >10 years";
			}

			// Convert to TimeSpan and format
			TimeSpan estimatedTime = TimeSpan.FromSeconds(remainingSeconds);

			// Use existing FormatTimeSpan method from ExecutionEstimate
			string formattedTime = FormatTimeSpanCompact(estimatedTime);

			return $"Est: {formattedTime}";
		}

		private static string FormatTimeSpanCompact(TimeSpan ts)
		{
			if (ts.TotalSeconds < 1)
				return "<1s";
			if (ts.TotalSeconds < 60)
				return $"{ts.TotalSeconds:F0} seconds";
			if (ts.TotalMinutes < 60)
				return $"{ts.TotalMinutes:F1} minutes";
			if (ts.TotalHours < 24)
				return $"{ts.TotalHours:F1} hours";
			if (ts.TotalDays < 365)
				return $"{ts.TotalDays:F1} days";
			return $"{ts.TotalDays / 365:F1} years";
		}

		private void ResetEstimationTracking()
		{
			_smoothedSpeed = 0;
			_lastProcessedCount = 0;
			_lastProgressTime = DateTime.MinValue;
			_progressReportCount = 0;

			if (StatusBarEstimate != null)
			{
				StatusBarEstimate.Visible = false;
				StatusBarEstimate.Text = "";
			}
		}

		#endregion

		private void TierManagerButton_Click(object sender, EventArgs e)
		{
			var tierManager = new TierManager();
			_tierManager = tierManager;

			tierManager.Show(this);
		}

		private void ShowItemsDataButton_Click(object sender, EventArgs e)
		{
			bool haveComputedCombinations = _combinations != null && _combinations.Count > 0;
			if (!haveComputedCombinations && (_fileParser == null || _fileParser.GetParsedItems().Count == 0))
			{
				StatusBarLabel.Text = "No parsed data available. Please load and parse a file first.";
				return;
			}

			var computedItems = haveComputedCombinations
									? [.. _combinations.SelectMany(list => list)]
									: _fileParser?.GetParsedItems() ?? [];

			var display = new DataDisplay(computedItems);

			// show modal so caller waits for the user to close it
			display.Show(this);
		}

		private async void ButtonComputeCombinations_Click(object sender, EventArgs e)
		{
			if (_fileParser == null)
			{
				StatusBarLabel.Text = "No parsed data available. Please load and parse a file first.";
				return;
			}

			var prepared = ItemPreparationHelper.PrepareItemsForCombinations(_parsedItems);
			if (!prepared.HasItems && !prepared.HasRings)
			{
				MessageBox.Show("No items found in any category. Cannot generate combinations.",
					"Missing Items", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			var totalCount = CombinationGenerator.ComputeTotalCombinations(prepared.ItemsWithoutRings, prepared.Rings);
			if (totalCount == 0)
			{
				MessageBox.Show("No combinations possible. All item class lists are empty.", "Unavailable combinations", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			// Confirm with user
			string countMessage = $"Total combinations to process: {totalCount}\n\n" +
				$"This is approximately {CommonHelper.GetBigIntegerApproximation(totalCount)} combinations.\n\n" +
				"Do you want to proceed?";

			if (MessageBox.Show(countMessage, "Combination Count", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
			{
				StatusBarLabel.Text = "Operation cancelled by user.";
				return;
			}

			HashSet<int> tieredItemIds = null;
			List<Tier> tiers = null;

			// ✅ Build tiered item IDs if strategy requires it
			if (_filterStrategy != CombinationFilterStrategy.Comprehensive)
			{
				// Get tiers from TierManager
				if (_tierManager != null && !_tierManager.IsDisposed)
				{
					tiers = _tierManager.GetTiers();
					if (tiers != null && tiers.Count > 0)
					{
						tieredItemIds = BuildTieredItemIds(_parsedItems, tiers);
					}
				}
			}

			// ✅ Compute normalization if tiers exist
			if (tiers != null && tiers.Count > 0)
			{
				_statNormalization = ComputeStatNormalization(_parsedItems, tiers);
			}

			TextboxDisplay.Text = "Processing combinations...\r\n\r\n";
			_progressHelper.Start();

			ResetEstimationTracking();
			StatusBarEstimate.Visible = false;
			StatusBarEstimate.Text = "";

			// ✅ CLIENT-SIDE THROTTLING: Process every 5th progress report (500k combinations)
			long progressReportCounter = 0;
			var lastProcessedForUI = 0L;
			const long UI_UPDATE_INTERVAL = 3; // Update UI every 5 reports = 500k combinations

			var progress = new Progress<CombinationProgress>(p =>
			{
				// ✅ Increment counter and check if we should update UI
				long currentCounter = Interlocked.Increment(ref progressReportCounter);

				// ✅ Always update on first report and every Nth report
				bool shouldUpdateUI = (currentCounter == 1) || (currentCounter % UI_UPDATE_INTERVAL == 0);

				// ✅ Always update on final report (100% complete)
				if (p.PercentComplete >= 99.99) shouldUpdateUI = true;

				if (!shouldUpdateUI) return;

				// ✅ Track that we processed this update
				Interlocked.Exchange(ref lastProcessedForUI, p.ProcessedCombinations);

				_progressReportCount++;

				// Calculate estimated time remaining
				string estimate = CalculateEstimatedTime(p.ProcessedCombinations, p.TotalCombinations, DateTime.Now);

				// Update StatusBar estimate (show after enough data collected)
				if (_progressReportCount >= MIN_REPORTS_FOR_ESTIMATE)
				{
					StatusBarEstimate.Visible = true;
					StatusBarEstimate.Text = estimate;
				}

				string statusText = $"Processing: {p.PercentComplete:F2}% | " +
									  $"Processed: {p.ProcessedCombinations} | " +
									  $"Valid: {p.ValidCombinations} | " +
									  $"Elapsed: {p.ElapsedTime:hh\\:mm\\:ss}";

				_progressHelper.UpdateProgress((int)p.PercentComplete, statusText);

				TextboxDisplay.Text = $"Progress: {p.PercentComplete:F2}%\r\n" +
									 $"Processed: {p.ProcessedCombinations}\r\n" +
									 $"Valid: {p.ValidCombinations}\r\n" +
									 $"Elapsed: {p.ElapsedTime:hh\\:mm\\:ss}\r\n" +
									 $"{estimate}";
			});

			try
			{
				var result = await Task.Run(() =>
					CombinationGenerator.GenerateCombinationsParallel(
						prepared.ItemsWithoutRings,
						prepared.Rings,
						_itemValidatorFunction,
						progress,
						maxValidToStore: _maxCombinationsToStore,
						filterStrategy: _filterStrategy,
						tieredItemIds: tieredItemIds,
						_progressHelper.Token),
					_progressHelper.Token);

				if (tiers != null && tiers.Count > 0 && result.ValidCombinationsCollection.Count > 0)
				{
					StatusBarLabel.Text = "Scoring combinations...";

					// ✅ Use min-heap for top-N selection (O(n log k) instead of O(n log n))
					var globalTopN = new PriorityQueue<ScoredCombination, double>();
					var lockObj = new object();

					Parallel.ForEach(
						result.ValidCombinationsCollection,
									new ParallelOptions
									{
										MaxDegreeOfParallelism = Environment.ProcessorCount
									},
									() => new PriorityQueue<ScoredCombination, double>(), // Thread-local min-heap
									(combo, state, localQueue) =>
									{
										double score = ScoreCombination(combo, tiers, _statNormalization);
										var scored = new ScoredCombination(combo, score);

										if (localQueue.Count < _bestCombinationCount)
										{
											localQueue.Enqueue(scored, score); // Min-heap
										}
										else
										{
											var minInQueue = localQueue.Peek();
											if (score > minInQueue.Score)
											{
												localQueue.Dequeue();
												localQueue.Enqueue(scored, score);
											}
										}

										return localQueue;
									},
									localQueue =>
									{
										lock (lockObj)
										{
											while (localQueue.Count > 0)
											{
												var item = localQueue.Dequeue();

												if (globalTopN.Count < _bestCombinationCount)
												{
													globalTopN.Enqueue(item, item.Score);
												}
												else
												{
													var minInGlobal = globalTopN.Peek();
													if (item.Score > minInGlobal.Score)
													{
														globalTopN.Dequeue();
														globalTopN.Enqueue(item, item.Score);
													}
												}
											}
										}
									});

					// Extract and sort results (only sorting top N items)
					var scoredCombinations = new List<ScoredCombination>(_bestCombinationCount);
					while (globalTopN.Count > 0)
					{
						scoredCombinations.Add(globalTopN.Dequeue());
					}
					scoredCombinations.Reverse(); // Dequeued in ascending order, reverse for descending

					// Display scored results
					var scoredSummary = new System.Text.StringBuilder();
					scoredSummary.AppendLine($"=== TOP {scoredCombinations.Count} COMBINATIONS (by score) ===\n");

					for (int i = 0; i < scoredCombinations.Count; i++)
					{
						scoredSummary.AppendLine($"#{i + 1} - Score: {scoredCombinations[i].Score:F2}");
						scoredSummary.AppendLine($"  Items: {string.Join(", ", scoredCombinations[i].Combination.Select(item => item.Name))}");
						scoredSummary.AppendLine();
					}

					TextboxDisplay.Text = scoredSummary.ToString();
				}
				else
				{
					DisplayCombinationResults(result);
				}

				_combinations = result.ValidCombinationsCollection;
			}
			catch (OperationCanceledException)
			{
				TextboxDisplay.Text = "Operation cancelled by user.";
				StatusBarLabel.Text = "Cancelled.";
			}
			catch (Exception ex)
			{
				TextboxDisplay.Text = $"Error: {ex.Message}";
				StatusBarLabel.Text = "Error during processing.";
				MessageBox.Show($"An error occurred:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				_progressHelper.Stop();
				StatusBarEstimate.Visible = false;
				StatusBarEstimate.Text = "";
			}
		}

		private void ButtonManageCustomValidator_Click(object sender, EventArgs e)
		{
			lock (_lockObject)
			{
				if (_customValidator == null || _customValidator.IsDisposed)
				{
					_customValidator = new CustomValidator(this);
				}

				_customValidator.Show(this);
				_customValidator.Activate();
			}
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			_progressHelper?.Dispose();

			lock (_lockObject)
			{
				try
				{
					if (_customValidator != null && !_customValidator.IsDisposed) _customValidator.Dispose();
					if (_tierManager != null && !_tierManager.IsDisposed) _tierManager.Dispose();

					_customValidator = null;
					_tierManager = null;
				}
				catch { }
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_progressHelper?.Dispose();
				components?.Dispose();
			}
			base.Dispose(disposing);
		}

		private void DisplayCombinationResults(CombinationResult<Item> result)
		{
			this.SuspendLayout();
			StatusBarEstimate.Visible = false;
			StatusBarEstimate.Text = "";

			double speed = result.ElapsedTime.TotalSeconds > 0
				? result.ProcessedCombinations / result.ElapsedTime.TotalSeconds
				: 0;

			var summary = new System.Text.StringBuilder();
			summary.AppendLine("=== COMBINATION GENERATION COMPLETE ===");
			summary.AppendLine();
			summary.AppendLine($"Total Combinations: {result.TotalCombinations}");
			summary.AppendLine($"Processed: {result.ProcessedCombinations}");
			summary.AppendLine($"Valid Combinations: {result.ValidCombinations}");
			summary.AppendLine($"Rejection Rate: {(1 - (double)result.ValidCombinations / result.ProcessedCombinations) * 100:F2}%");
			summary.AppendLine($"Elapsed Time: {result.ElapsedTime:hh\\:mm\\:ss\\.fff}");
			summary.AppendLine($"Speed: {speed:F2} combinations/second");

			if (result.ValidCombinations > _maxCombinationsToStore)
			{
				summary.AppendLine();
				summary.AppendLine($"NOTE: Only the first {_maxCombinationsToStore:N0} valid combinations were stored.");
			}

			// ✅ RESTORE THESE
			TextboxDisplay.Text = summary.ToString();
			StatusBarLabel.Text = $"Complete! {result.ValidCombinations} valid combinations found.";
			this.ResumeLayout();

			MessageBox.Show(summary.ToString(), "Generation Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void RadioComprehensive_CheckedChanged(object sender, EventArgs e)
		{
			if (RadioComprehensive.Checked) _filterStrategy = CombinationFilterStrategy.Comprehensive;
		}

		private void RadioBalanced_CheckedChanged(object sender, EventArgs e)
		{
			if (RadioBalanced.Checked) _filterStrategy = CombinationFilterStrategy.Balanced;
		}

		private void RadioStrict_CheckedChanged(object sender, EventArgs e)
		{
			if (RadioStrict.Checked) _filterStrategy = CombinationFilterStrategy.Strict;
		}

		private void NumericBestCombinationsCount_ValueChanged(object sender, EventArgs e)
		{
			_bestCombinationCount = (int)NumericBestCombinationsCount.Value;
		}
	}
}
