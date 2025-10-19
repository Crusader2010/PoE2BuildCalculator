using System.Collections.Immutable;

using Domain.Combinations;
using Domain.Helpers;
using Domain.Main;

using Manager;

namespace PoE2BuildCalculator
{
    public partial class MainForm : Form, IDisposable
    {
        // Field for thread synchronization
        private readonly object _lockObject = new();
        private readonly long _maxCombinationsToStore = 10000000;
        private readonly long _safeBenchmarkSampleSize = 500000;

        internal Func<List<Item>, bool> _itemValidatorFunction { get; set; } = x => true;
        internal bool _benchmarkRanOnCustomValidator { get; set; } = false;
        internal ImmutableList<Item> _parsedItems { get; private set; } = [];


        // Class references
        private FileParser _fileParser { get; set; }
        private TierManager _tierManager { get; set; }
        private CustomValidator _customValidator { get; set; }
        private ProgressReportingHelper _progressHelper;

        private List<List<Item>> _combinations { get; set; } = [];
        private List<List<Item>> _benchmarkSampledItems;
        private List<Item> _benchmarkSampledRings;
        private ToolStripProgressBar _statusProgressBar;
        private ToolStripButton _cancelButton;
        private readonly object _benchmarkSampleLock = new();

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
                    PanelButtons);
            }

            this.FormClosing += MainForm_FormClosing;

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

                lock (_lockObject)
                {
                    _benchmarkSampledItems = null;
                    _benchmarkSampledRings = null;
                }
            }
            catch (OperationCanceledException)
            {
                StatusBarLabel.Text = "Parsing cancelled.";
            }
            catch (Exception ex)
            {
                StatusBarLabel.Text = $"Error: {ex.Message}";
                ErrorHelper.ShowError(ex, "Parsing Error");
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

        private void TierManagerButton_Click(object sender, EventArgs e)
        {
            var tierManager = new TierManager();
            _tierManager = tierManager;

            tierManager.Show(this);
        }

        private void ShowItemsDataButton_Click(object sender, EventArgs e)
        {
            if (_fileParser == null || _fileParser.GetParsedItems().Count == 0)
            {
                StatusBarLabel.Text = "No parsed data available. Please load and parse a file first.";
                return;
            }

            var display = new DataDisplay
            {
                // pass parser to the dialog before showing it
                _fileParser = _fileParser
            };

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

            if (!_benchmarkRanOnCustomValidator)
            {
                if (MessageBox.Show("The validator function for item combinations has changed. Are you sure you do not want to run the benchmark first?",
                    "Run Benchmark first", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }
                else
                {
                    _benchmarkRanOnCustomValidator = true;
                }
            }

            var prepared = ItemPreparationHelper.PrepareItemsForCombinations(_parsedItems);
            if (!prepared.HasItems && !prepared.HasRings)
            {
                MessageBox.Show("No items found in any category. Cannot generate combinations.",
                    "Missing Items", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var totalCount = CombinationGenerator.ComputeTotalCombinations(
                prepared.ItemsWithoutRings, prepared.Rings);

            if (totalCount == 0)
            {
                MessageBox.Show("No combinations possible. All item class lists are empty.",
                    "Unavailable combinations", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Confirm with user
            string countMessage = $"Total combinations to process: {totalCount}\n\n" +
                $"This is approximately {CommonHelper.GetBigIntegerApproximation(totalCount)} combinations.\n\n" +
                "Consider using the Benchmark feature first to estimate how much time this will take.\n\n" +
                "Do you want to proceed?";

            if (MessageBox.Show(countMessage, "Combination Count", MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                StatusBarLabel.Text = "Operation cancelled by user.";
                return;
            }

            TextboxDisplay.Text = "Processing combinations...\r\n\r\n";
            _progressHelper.Start();

            var progress = new Progress<CombinationProgress>(p =>
            {
                string statusText = $"Processing: {p.PercentComplete:F2}% | " +
                                  $"Processed: {p.ProcessedCombinations} | " +
                                  $"Valid: {p.ValidCombinations} | " +
                                  $"Elapsed: {p.ElapsedTime:hh\\:mm\\:ss}";

                _progressHelper.UpdateProgress((int)p.PercentComplete, statusText);

                TextboxDisplay.Text = $"Progress: {p.PercentComplete:F2}%\r\n" +
                                     $"Processed: {p.ProcessedCombinations}\r\n" +
                                     $"Valid: {p.ValidCombinations}\r\n" +
                                     $"Elapsed: {p.ElapsedTime:hh\\:mm\\:ss}";
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
                        _progressHelper.Token),
                    _progressHelper.Token);

                DisplayCombinationResults(result);
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
                MessageBox.Show($"An error occurred:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _progressHelper.Stop();
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

        private async void ButtonBenchmark_Click(object sender, EventArgs e)
        {
            if (_fileParser == null)
            {
                MessageBox.Show("No parsed data available. Please load and parse a file first.",
                    "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var prepared = ItemPreparationHelper.PrepareItemsForCombinations(_parsedItems);
            if (!prepared.HasItems && !prepared.HasRings)
            {
                MessageBox.Show("No items found in any category. Cannot benchmark.",
                    "No Items", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int totalIterations = _customValidator != null && _customValidator._customValidatorCreated ? 2 : 25;
            const int maxListSampleSize = 100;

            StatusBarLabel.Text = "Preparing benchmark...";
            TextboxDisplay.Text = "Preparing benchmark...\r\n";

            try
            {
                _progressHelper.Start();

                // Pre-sample ONCE (lazy initialization)
                List<List<Item>> sampledItems;
                List<Item> sampledRings;

                lock (_benchmarkSampleLock)
                {
                    if (_benchmarkSampledItems == null || _benchmarkSampledRings == null)
                    {
                        (_benchmarkSampledItems, _benchmarkSampledRings) =
                            ItemPreparationHelper.CreateSamples(
                                prepared.ItemsWithoutRings,
                                prepared.Rings,
                                maxListSampleSize);

                        TextboxDisplay.AppendText("✓ Samples created\r\n");
                    }
                    else
                    {
                        TextboxDisplay.AppendText("✓ Using cached samples\r\n");
                    }

                    // Create local copies to avoid locking during async operation
                    sampledItems = _benchmarkSampledItems;
                    sampledRings = _benchmarkSampledRings;
                }

                _progressHelper.Token.ThrowIfCancellationRequested();

                // Warmup and GC (quick, can stay on UI thread)
                StatusBarLabel.Text = "Warming up...";
                TextboxDisplay.AppendText("Warming up and collecting garbage...\r\n");

                PerformWarmupGarbageCollection();

                TextboxDisplay.AppendText($"✓ Ready\r\n\r\nRunning {totalIterations} benchmark iterations...\r\n");

                // Progress callback for UI updates
                var progress = new Progress<BenchmarkProgress>(p =>
                {
                    _progressHelper.UpdateProgress(p.PercentComplete, p.StatusMessage);

                    // Update display every 5 iterations to reduce UI churn
                    if (p.CurrentIteration % 5 == 0 || p.CurrentIteration == totalIterations)
                    {
                        TextboxDisplay.AppendText($"  Iteration {p.CurrentIteration}/{totalIterations} complete\r\n");
                    }
                });

                // Run benchmark on background thread
                ExecutionEstimate estimate = await Task.Run(() =>
                    RunBenchmarkIterations(totalIterations, sampledItems, sampledRings, progress),
                    _progressHelper.Token);

                DisplayBenchmarkResults(estimate);
                _benchmarkRanOnCustomValidator = true;
            }
            catch (OperationCanceledException)
            {
                TextboxDisplay.AppendText("\r\nBenchmark cancelled by user.\r\n");
                StatusBarLabel.Text = "Benchmark cancelled.";
            }
            catch (Exception ex)
            {
                string errorMsg = $"Error during benchmark:\n\n{ex.Message}";
                TextboxDisplay.Text = errorMsg;
                StatusBarLabel.Text = "Benchmark failed.";
                MessageBox.Show(errorMsg, "Benchmark Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _progressHelper.Stop();
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

        private ExecutionEstimate RunBenchmarkIterations(int totalIterations, List<List<Item>> sampledItems, List<Item> sampledRings, IProgress<BenchmarkProgress> progress)
        {
            ExecutionEstimate estimate = null;

            for (int i = 0; i < totalIterations; i++)
            {
                _progressHelper.Token.ThrowIfCancellationRequested();

                estimate = CombinationGenerator.EstimateExecutionTime(
                    sampledItems,
                    sampledRings,
                    _itemValidatorFunction,
                    safeSampleSize: _safeBenchmarkSampleSize,
                    skipGarbageCollection: true);

                int currentIteration = i + 1;
                int percentComplete = currentIteration * 100 / totalIterations;

                progress?.Report(new BenchmarkProgress
                {
                    CurrentIteration = currentIteration,
                    TotalIterations = totalIterations,
                    PercentComplete = percentComplete,
                    StatusMessage = $"Benchmark: {currentIteration}/{totalIterations} iterations ({percentComplete}%)"
                });
            }

            return estimate;
        }

        private static void PerformWarmupGarbageCollection()
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
            GC.WaitForPendingFinalizers();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
        }

        private void DisplayBenchmarkResults(ExecutionEstimate estimate)
        {
            string summary = estimate.GetFormattedSummary();

            // ✅ RESTORE THESE
            TextboxDisplay.Text = summary;
            StatusBarLabel.Text = "Benchmark complete.";

            MessageBox.Show(summary, "Benchmark Results",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DisplayCombinationResults(CombinationResult<Item> result)
        {
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

            MessageBox.Show(summary.ToString(), "Generation Complete",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
