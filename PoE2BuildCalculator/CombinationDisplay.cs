using System.Collections.Immutable;

using Domain.HelperForms;
using Domain.Helpers;
using Domain.Main;
using Domain.Validation;

namespace PoE2BuildCalculator
{
	public partial class CombinationDisplay : BaseForm
	{
		private sealed class CombinationViewModel
		{
			public int Rank { get; init; }
			public double Score { get; init; }
			public List<Item> Items { get; init; }
			public Dictionary<string, double> AggregatedStats { get; init; }
			public string ItemNamesCache { get; init; }
			public List<ImmutableDictionary<string, object>> ItemStatsDictsCache { get; init; }
		}

		private readonly List<Tier> _tiers;
		private readonly List<Group> _validatorGroups;
		private readonly HashSet<string> _tieredStats = [];
		private readonly HashSet<string> _validatorStats = [];
		private readonly Dictionary<string, (double StatWeight, double TierWeight, int TierIndex)> _tieredStatWeights = [];
		private Dictionary<string, ItemStatsHelper.StatDescriptor> _statDescriptorsByName;
		private ImmutableList<CombinationViewModel> _combinationsToDisplay = [];
		private Dictionary<int, CombinationViewModel> _combinationsByRank;
		private HashSet<string> _relevantStatsCache;

		private List<(List<Item> Combination, double Score)> _pendingCombinations;
		private ToolStripProgressBar _progressBar;
		private CancellationTokenSource _cts;
		private ToolStripButton _cancelButton;

		public CombinationDisplay(
			List<(List<Item> Combination, double Score)> scoredCombinations,
			List<Tier> tiers = null,
			List<Group> validatorGroups = null)
		{
			InitializeComponent();
			this.Shown += CombinationDisplay_Shown;

			_tiers = tiers ?? [];
			_validatorGroups = validatorGroups ?? [];
			_statDescriptorsByName = ItemStatsHelper.GetStatDescriptors().ToDictionary(d => d.PropertyName, d => d, StringComparer.OrdinalIgnoreCase);

			ExtractRelevantStats(tiers, validatorGroups);
			BuildTieredStatWeights(tiers);
			_pendingCombinations = scoredCombinations ?? [];
		}

		private void BuildTieredStatWeights(List<Tier> tiers)
		{
			if (tiers == null) return;

			for (int tierIndex = 0; tierIndex < tiers.Count; tierIndex++)
			{
				var tier = tiers[tierIndex];
				foreach (var (statName, statWeight) in tier.StatWeights)
				{
					if (statWeight > 0)
					{
						// Effective weight = (statWeight / totalStatWeight) * (tierWeight / 100)
						double normalizedStatWeight = tier.TotalStatWeight > 0
							? statWeight / tier.TotalStatWeight
							: 0;
						double effectiveWeight = normalizedStatWeight * (tier.TierWeight / 100.0);

						_tieredStatWeights[statName] = (statWeight, effectiveWeight, tierIndex);
					}
				}
			}
		}

		private void ExtractRelevantStats(List<Tier> tiers, List<Group> validatorGroups)
		{
			if (tiers != null)
			{
				foreach (var tier in tiers)
				{
					foreach (var statName in tier.StatWeights.Keys.Where(k => tier.StatWeights[k] > 0))
					{
						_tieredStats.Add(statName);
					}
				}
			}

			if (validatorGroups != null)
			{
				foreach (var group in validatorGroups)
				{
					if (group.Stats != null)
					{
						foreach (var stat in group.Stats)
						{
							_validatorStats.Add(stat.PropertyName);
						}
					}
				}
			}

			_relevantStatsCache = [.. _tieredStats.Union(_validatorStats)];
		}

		private Dictionary<string, double> ComputeAggregatedStats(List<ImmutableDictionary<string, object>> itemStatsDicts)
		{
			var result = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

			foreach (var statName in _relevantStatsCache)
			{
				double sum = 0;
				foreach (var itemStatsDict in itemStatsDicts)
				{
					if (itemStatsDict.TryGetValue(statName, out var value) && value is not string) sum += Convert.ToDouble(value);
				}

				result[statName] = sum;
			}

			return result;
		}

		private async void CombinationDisplay_Shown(object sender, EventArgs e)
		{
			this.SuspendLayout();
			SplitContainerMain.SuspendLayout();

			ConfigureForm();
			SetupMasterGrid();
			SetupDetailGrid();

			this.ResumeLayout(false);
			SplitContainerMain.ResumeLayout(false);

			SplitContainerMain.Visible = false;
			StatusBarLabel.Text = "Loading combinations...";
			this.Update(); // Force UI refresh

			await LoadCombinationsAsync();
			SplitContainerMain.Visible = true;
		}

		private async Task LoadCombinationsAsync()
		{
			if (_pendingCombinations == null || _pendingCombinations.Count == 0)
			{
				StatusBarLabel.Text = "No combinations to display";
				return;
			}

			_cts = new CancellationTokenSource();
			_progressBar = StatusBar.Items.OfType<ToolStripProgressBar>().FirstOrDefault();
			_cancelButton = StatusBar.Items.OfType<ToolStripButton>().FirstOrDefault();

			StatusBar.SuspendLayout();
			if (_progressBar != null)
			{
				_progressBar.Visible = true;
				_progressBar.Value = 0;
				_progressBar.Maximum = 100;
			}

			if (_cancelButton != null)
			{
				_cancelButton.Visible = true;
				_cancelButton.Enabled = true;
			}

			StatusBar.ResumeLayout();
			StatusBar.Refresh();

			try
			{
				await Task.Run(() => PrepareCombinationDataAsync(_cts.Token), _cts.Token);

				StatusBarLabel.Text = "Rendering grid...";
				StatusBar.Refresh();
				await Task.Delay(10);

				DataGridViewMaster.RowCount = _combinationsToDisplay.Count;
				StatusBarLabel.Text = $"Loaded {_combinationsToDisplay.Count:N0} combinations - Select rows to compare";
			}
			catch (OperationCanceledException)
			{
				StatusBarLabel.Text = "Loading cancelled";
				this.Close();
			}
			catch (Exception ex)
			{
				ErrorHelper.ShowError(ex, $"{nameof(CombinationDisplay)} - {nameof(LoadCombinationsAsync)}");
				StatusBarLabel.Text = "Error loading combinations";
			}
			finally
			{
				if (_progressBar != null)
				{
					_progressBar.Value = 0;
					_progressBar.Visible = false;
				}

				if (_cancelButton != null)
				{
					_cancelButton.Visible = false;
					_cancelButton.Enabled = false;
				}

				_cts?.Dispose();
				_cts = null;
			}
		}

		private void PrepareCombinationDataAsync(CancellationToken cancellationToken)
		{
			var totalCount = _pendingCombinations.Count;
			var viewModels = new List<CombinationViewModel>(totalCount);
			const int BATCH_SIZE = 250; // ✅ Reduced update frequency from 50 to 250
			int processedCount = 0;
			var lastUpdateTime = DateTime.UtcNow;

			for (int i = 0; i < totalCount; i++)
			{
				if (cancellationToken.IsCancellationRequested)
					break;

				var (combination, score) = _pendingCombinations[i];

				var itemStatsDicts = new List<ImmutableDictionary<string, object>>(combination.Count);
				foreach (var item in combination)
					itemStatsDicts.Add(ItemStatsHelper.ToDictionary(item.ItemStats));

				var aggregatedStats = ComputeAggregatedStats(itemStatsDicts);
				var itemNames = string.Join(", ", combination.Select(item => item.Name));

				var vm = new CombinationViewModel
				{
					Rank = i + 1,
					Score = score,
					Items = combination,
					AggregatedStats = aggregatedStats,
					ItemNamesCache = itemNames,
					ItemStatsDictsCache = itemStatsDicts
				};

				viewModels.Add(vm);
				processedCount++;

				// ✅ Update every BATCH_SIZE OR at end OR every 200ms minimum
				bool shouldUpdate = (processedCount % BATCH_SIZE == 0) ||
									(processedCount == totalCount) ||
									((DateTime.UtcNow - lastUpdateTime).TotalMilliseconds >= 200);

				if (shouldUpdate && _progressBar?.GetCurrentParent() != null)
				{
					int percentComplete = (int)(processedCount / (double)totalCount * 100);
					bool isFinalUpdate = processedCount == totalCount;
					lastUpdateTime = DateTime.UtcNow;

					void UpdateUI()
					{
						try
						{
							if (_progressBar != null && !_progressBar.IsDisposed)
							{
								_progressBar.Value = Math.Clamp(percentComplete, 0, 100);
							}

							if (StatusBarLabel != null && !StatusBarLabel.IsDisposed)
							{
								StatusBarLabel.Text = $"Loading: {percentComplete}% ({processedCount:N0}/{totalCount:N0})";
							}

							_progressBar?.GetCurrentParent()?.Update(); // ✅ Synchronous paint
						}
						catch { }
					}

					// ✅ Always use synchronous Invoke for reliable rendering
					_progressBar.GetCurrentParent().Invoke(UpdateUI);

					// ✅ Brief pause to let UI breathe (only for non-final updates)
					if (!isFinalUpdate)
					{
						Thread.Sleep(15);
					}
				}
			}

			_combinationsToDisplay = [.. viewModels];
			_combinationsByRank = _combinationsToDisplay.ToDictionary(vm => vm.Rank, vm => vm);
			_pendingCombinations = null;
		}

		private void ConfigureForm()
		{
			this.AutoSize = false;
			this.AutoSizeMode = AutoSizeMode.GrowOnly;
			this.StartPosition = FormStartPosition.CenterScreen;

			SetStyle(ControlStyles.OptimizedDoubleBuffer |
					 ControlStyles.AllPaintingInWmPaint |
					 ControlStyles.UserPaint, true);
			UpdateStyles();
		}

		private void ButtonClose_Click(object sender, EventArgs e)
		{
			this.Close();
			this.Dispose();
		}

		private void ButtonExport_Click(object sender, EventArgs e)
		{
			CustomMessageBox.Show("Export functionality not yet implemented.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_cts?.Cancel();
				_cts?.Dispose();
				_cts = null;
				components?.Dispose();
			}
			base.Dispose(disposing);
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			// Cancel loading if form is closed while loading
			if (_cts != null && !_cts.IsCancellationRequested)
			{
				_cts.Cancel();
				StatusBarLabel.Text = "Cancelling...";

				// Wait briefly for cancellation to complete
				Thread.Sleep(100);
			}

			base.OnFormClosing(e);
		}

		private void StatusBarCancelButton_Click(object sender, EventArgs e)
		{
			if (_cancelButton != null)
			{
				_cancelButton.Enabled = false;
				StatusBarLabel.Text = "Cancelling...";
				_cts?.Cancel();
			}
		}



		private void SetupMasterGrid()
		{
			DataGridViewMaster.SuspendLayout();

			DataGridViewMaster.VirtualMode = true;
			DataGridViewMaster.ReadOnly = true;
			DataGridViewMaster.AllowUserToAddRows = false;
			DataGridViewMaster.AllowUserToDeleteRows = false;
			DataGridViewMaster.AllowUserToResizeRows = false;
			DataGridViewMaster.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			DataGridViewMaster.MultiSelect = true;
			DataGridViewMaster.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
			DataGridViewMaster.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
			DataGridViewMaster.RowHeadersVisible = false;
			DataGridViewMaster.Dock = DockStyle.Fill;

			DataGridViewMaster.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255);
			DataGridViewMaster.RowsDefaultCellStyle.BackColor = Color.White;

			DataGridViewMaster.CellValueNeeded += MasterGrid_CellValueNeeded;
			DataGridViewMaster.SelectionChanged += MasterGrid_SelectionChanged;

			BuildMasterColumns();

			DataGridViewMaster.ResumeLayout();
		}

		private void BuildMasterColumns()
		{
			DataGridViewMaster.Columns.Clear();

			DataGridViewMaster.Columns.Add(new DataGridViewTextBoxColumn
			{
				Name = "Rank",
				HeaderText = "#",
				AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
				ValueType = typeof(int),
				DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
			});

			DataGridViewMaster.Columns.Add(new DataGridViewTextBoxColumn
			{
				Name = "Score",
				HeaderText = "Score",
				AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
				ValueType = typeof(double),
				DefaultCellStyle = new DataGridViewCellStyle
				{
					Alignment = DataGridViewContentAlignment.MiddleCenter,
					Format = "F2"
				}
			});

			DataGridViewMaster.Columns.Add(new DataGridViewTextBoxColumn
			{
				Name = "Items",
				HeaderText = "Item Names",
				Width = 600,
				ValueType = typeof(string),
				AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
				DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleLeft }
			});
		}

		private void MasterGrid_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
		{
			if (e.RowIndex < 0 || e.RowIndex >= _combinationsToDisplay.Count) return;

			var viewModel = _combinationsToDisplay[e.RowIndex];

			e.Value = DataGridViewMaster.Columns[e.ColumnIndex].Name switch
			{
				"Rank" => viewModel.Rank,
				"Score" => viewModel.Score,
				"Items" => viewModel.ItemNamesCache,
				_ => null
			};
		}

		private void MasterGrid_SelectionChanged(object sender, EventArgs e)
		{
			DataGridViewDetail.SuspendLayout();
			RebuildDetailGrid();
			DataGridViewDetail.ResumeLayout();
		}

		private void RebuildDetailGrid()
		{
			DataGridViewDetail.SuspendLayout();

			var selectedRows = DataGridViewMaster.SelectedRows;
			if (selectedRows.Count == 0)
			{
				DataGridViewDetail.Rows.Clear();
				DataGridViewDetail.Columns.Clear();
				StatusBarLabel.Text = "Select combinations to compare";
				DataGridViewDetail.ResumeLayout();
				return;
			}

			var selectedIndices = new List<int>(selectedRows.Count);
			foreach (DataGridViewRow row in selectedRows)
			{
				if (row.Index >= 0 && row.Index < _combinationsToDisplay.Count) selectedIndices.Add(row.Index);
			}

			if (selectedIndices.Count == 0)
			{
				DataGridViewDetail.Rows.Clear();
				DataGridViewDetail.Columns.Clear();
				StatusBarLabel.Text = "Select combinations to compare";
				DataGridViewDetail.ResumeLayout();
				return;
			}

			selectedIndices.Sort();

			// Collect all stats (UNION)
			var allStats = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (var index in selectedIndices)
			{
				allStats.UnionWith(_combinationsToDisplay[index].AggregatedStats.Keys);
			}

			var orderedStats = OrderStatsByImportance(allStats);

			BuildDetailColumns(orderedStats);
			PopulateDetailRows(selectedIndices, orderedStats);

			StatusBarLabel.Text = $"Comparing {selectedIndices.Count} combination(s)";
			DataGridViewDetail.ResumeLayout();
		}

		private List<string> OrderStatsByImportance(HashSet<string> allStats)
		{
			var result = new List<string>(allStats.Count);
			var tieredStats = new List<(string StatName, double EffectiveWeight, int TierIndex)>();
			var validatorStats = new List<string>();
			var otherStats = new List<string>();

			// Single pass categorization
			foreach (var statName in allStats)
			{
				if (_tieredStatWeights.TryGetValue(statName, out var tierInfo))
					tieredStats.Add((statName, tierInfo.StatWeight * tierInfo.TierWeight, tierInfo.TierIndex));
				else if (_validatorStats.Contains(statName))
					validatorStats.Add(statName);
				else
					otherStats.Add(statName);
			}

			// Sort and add tiered stats
			if (tieredStats.Count > 0)
			{
				tieredStats.Sort((a, b) =>
				{
					int tierCompare = a.TierIndex.CompareTo(b.TierIndex);
					return tierCompare != 0 ? tierCompare : b.EffectiveWeight.CompareTo(a.EffectiveWeight);
				});

				result.Capacity = tieredStats.Count + validatorStats.Count + otherStats.Count;
				foreach (var (statName, _, _) in tieredStats)
				{
					result.Add(statName);
				}
			}

			// Sort and add validator stats
			if (validatorStats.Count > 0)
			{
				validatorStats.Sort(StringComparer.OrdinalIgnoreCase);
				result.AddRange(validatorStats);
			}

			// Sort and add other stats
			if (otherStats.Count > 0)
			{
				otherStats.Sort(StringComparer.OrdinalIgnoreCase);
				result.AddRange(otherStats);
			}

			return result;
		}

		private void BuildDetailColumns(List<string> orderedStats)
		{
			DataGridViewDetail.Columns.Clear();

			// Rank column
			DataGridViewDetail.Columns.Add(new DataGridViewTextBoxColumn
			{
				Name = "Rank",
				HeaderText = "#",
				AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
				ValueType = typeof(int),
				DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
				Frozen = true  // Keep visible while scrolling
			});

			// Score column (sortable, default sort DESC)
			DataGridViewDetail.Columns.Add(new DataGridViewTextBoxColumn
			{
				Name = "Score",
				HeaderText = "Score",
				AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
				ValueType = typeof(double),
				DefaultCellStyle = new DataGridViewCellStyle
				{
					Alignment = DataGridViewContentAlignment.MiddleCenter,
					Format = "F2"
				},
				Frozen = true
			});

			// Stat columns (ordered by importance)
			foreach (var statName in orderedStats)
			{
				var descriptor = _statDescriptorsByName.TryGetValue(statName, out var dictDescriptor) ? dictDescriptor : null;
				string headerText = descriptor?.Header ?? statName;

				// Add visual indicator for tiered stats
				if (_tieredStatWeights.ContainsKey(statName))
				{
					headerText = $"★ {headerText}";  // Star for tiered stats
				}

				DataGridViewDetail.Columns.Add(new DataGridViewTextBoxColumn
				{
					Name = $"Stat_{statName}",
					HeaderText = headerText,
					AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
					ValueType = typeof(double),
					DefaultCellStyle = new DataGridViewCellStyle
					{
						Alignment = DataGridViewContentAlignment.MiddleCenter,
						Format = "F2"
					},
					Tag = statName  // Store original stat name for click event
				});
			}

			// Enable sorting
			DataGridViewDetail.AllowUserToOrderColumns = false;
			DataGridViewDetail.AutoGenerateColumns = false;
			DataGridViewDetail.ReadOnly = true;
			DataGridViewDetail.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			DataGridViewDetail.MultiSelect = false;
			DataGridViewDetail.Dock = DockStyle.Fill;
		}

		private void PopulateDetailRows(List<int> selectedIndices, List<string> orderedStats)
		{
			DataGridViewDetail.Rows.Clear();

			var columnCount = 2 + orderedStats.Count;

			foreach (var index in selectedIndices)
			{
				var vm = _combinationsToDisplay[index];
				var rowValues = new object[columnCount];

				rowValues[0] = vm.Rank;
				rowValues[1] = vm.Score;

				for (int i = 0; i < orderedStats.Count; i++)
				{
					rowValues[2 + i] = vm.AggregatedStats.GetValueOrDefault(orderedStats[i], 0.0);
				}

				DataGridViewDetail.Rows.Add(rowValues);
			}

			// Default sort by Score DESC
			if (DataGridViewDetail.Rows.Count > 0)
			{
				DataGridViewDetail.Sort(DataGridViewDetail.Columns["Score"], System.ComponentModel.ListSortDirection.Descending);
			}
		}

		private void SetupDetailGrid()
		{
			DataGridViewDetail.CellDoubleClick += DetailGrid_CellDoubleClick;

			DataGridViewDetail.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255);
			DataGridViewDetail.RowsDefaultCellStyle.BackColor = Color.White;
		}

		private void DetailGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			// Ignore header and non-stat columns
			if (e.RowIndex < 0 || e.ColumnIndex < 2) return;

			var column = DataGridViewDetail.Columns[e.ColumnIndex];
			if (!column.Name.StartsWith("Stat_", StringComparison.OrdinalIgnoreCase)) return;

			var statName = column.Tag as string;
			var rank = (int)DataGridViewDetail.Rows[e.RowIndex].Cells["Rank"].Value;

			ShowStatBreakdown(rank, statName);
		}

		private void ShowStatBreakdown(int rank, string statName)
		{
			if (!_combinationsByRank.TryGetValue(rank, out var vm)) return;

			var descriptor = _statDescriptorsByName.TryGetValue(statName, out var dictDescriptor) ? dictDescriptor : null;
			string displayName = descriptor?.Header ?? statName;

			if (!vm.AggregatedStats.TryGetValue(statName, out double totalValue)) return;

			var breakdown = new System.Text.StringBuilder();
			breakdown.AppendLine($"Combination #{rank} - {displayName} Breakdown");
			breakdown.AppendLine($"Total: {totalValue:F2}");
			breakdown.AppendLine();
			breakdown.AppendLine("Item Contributions:");

			double sum = 0;
			for (int i = 0; i < vm.Items.Count; i++)
			{
				var itemStatsDict = vm.ItemStatsDictsCache[i];
				if (itemStatsDict.TryGetValue(statName, out var value) && value is not string)
				{
					double val = Convert.ToDouble(value);
					if (val != 0)
					{
						breakdown.AppendLine($"  {vm.Items[i].Name}: {val:F2}");
						sum += val;
					}
				}
			}

			breakdown.AppendLine();
			breakdown.AppendLine($"Computed Total: {sum:F2}");

			CustomMessageBox.Show(breakdown.ToString(), "Stat Breakdown", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
	}
}
