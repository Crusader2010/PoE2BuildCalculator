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
        private readonly object _validatorLock = new();
        private readonly long _maxCombinationsToStore = 10000000;
        private readonly ulong _safeBenchmarkSampleSize = 500000;

        // Class references
        private FileParser _fileParser { get; set; }
        private TierManager _formTierManager { get; set; }
        private CustomValidator _customValidator { get; set; }
        internal Func<List<Item>, bool> _itemValidatorFunction { get; set; } = x => true;
        private ImmutableList<Item> _parsedItems { get; set; } = [];

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

                lock (_benchmarkSampleLock)
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
            _formTierManager = tierManager;

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

            _parsedItems = _fileParser.GetParsedItems();
            var prepared = ItemPreparationHelper.PrepareItemsForCombinations(_parsedItems);

            if (!prepared.HasItems && !prepared.HasRings)
            {
                MessageBox.Show("No items found in any category. Cannot generate combinations.",
                    "No Items", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var totalCount = CombinationGenerator.ComputeTotalCombinations(
                prepared.ItemsWithoutRings, prepared.Rings);

            if (totalCount == 0)
            {
                MessageBox.Show("No combinations possible. All item class lists are empty.",
                    "No Combinations", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                // ✅ CORRECTED: CancellationToken is LAST parameter
                var result = await Task.Run(() =>
                    CombinationGenerator.GenerateCombinationsParallel(
                        prepared.ItemsWithoutRings,
                        prepared.Rings,
                        _itemValidatorFunction,
                        maxValidToStore: _maxCombinationsToStore,
                        progress: progress,
                        cancellationToken: _progressHelper.Token), // ✅ LAST
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
            lock (_validatorLock)
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
            try
            {
                _progressHelper?.Cancel(); // ✅ Cancel BEFORE disposing
                Thread.Sleep(50); // ✅ Brief delay to let cancellation propagate
                _progressHelper?.Dispose();
            }
            catch (OperationCanceledException)
            {
                // ✅ Expected when cancelling operations
            }
            catch
            {
                // Ignore other disposal errors during shutdown
            }

            lock (_validatorLock)
            {
                try
                {
                    if (_customValidator != null && !_customValidator.IsDisposed)
                    {
                        _customValidator.Dispose();
                    }
                    _customValidator = null;
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

            _parsedItems = _fileParser.GetParsedItems();
            var prepared = ItemPreparationHelper.PrepareItemsForCombinations(_parsedItems);

            if (!prepared.HasItems && !prepared.HasRings)
            {
                MessageBox.Show("No items found in any category. Cannot benchmark.",
                    "No Items", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            const int totalIterations = 25;
            const int maxListSampleSize = 100;

            StatusBarLabel.Text = "Preparing benchmark...";
            TextboxDisplay.Text = "Preparing benchmark...\r\n";

            try
            {
                _progressHelper.Start();

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

                    sampledItems = _benchmarkSampledItems;
                    sampledRings = _benchmarkSampledRings;
                }

                _progressHelper.Token.ThrowIfCancellationRequested();

                StatusBarLabel.Text = "Warming up...";
                TextboxDisplay.AppendText("Warming up and collecting garbage...\r\n");

                PerformWarmupGarbageCollection();

                TextboxDisplay.AppendText($"✓ Ready\r\n\r\nRunning {totalIterations} benchmark iterations...\r\n");

                var progress = new Progress<BenchmarkProgress>(p =>
                {
                    _progressHelper.UpdateProgress(p.PercentComplete, p.StatusMessage);

                    if (p.CurrentIteration % 5 == 0 || p.CurrentIteration == totalIterations)
                    {
                        TextboxDisplay.AppendText($"  Iteration {p.CurrentIteration}/{totalIterations} complete\r\n");
                    }
                });

                ExecutionEstimate estimate = await RunBenchmarkIterationsAsync(
                    totalIterations,
                    sampledItems,
                    sampledRings,
                    progress,
                    _progressHelper.Token); // ✅ CancellationToken LAST

                DisplayBenchmarkResults(estimate);
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

        private async Task<ExecutionEstimate> RunBenchmarkIterationsAsync(
            ulong totalIterations,
            List<List<Item>> sampledItems,
            List<Item> sampledRings,
            IProgress<BenchmarkProgress> progress,
            CancellationToken cancellationToken)
        {
            ExecutionEstimate estimate = null;

            for (ulong i = 0; i < totalIterations; i++)
            {
                ulong currentIteration = i + 1;

                // Create sub-progress that reports to main progress with iteration context
                var iterationProgress = new Progress<BenchmarkProgress>(p =>
                {
                    progress?.Report(new BenchmarkProgress
                    {
                        CurrentIteration = currentIteration,
                        TotalIterations = totalIterations,
                        PercentComplete = ((currentIteration - 1) * 100.0d / totalIterations) + (p.PercentComplete / totalIterations),
                        StatusMessage = $"Iteration {currentIteration}/{totalIterations}: {p.StatusMessage}"
                    });
                });

                estimate = await CombinationGenerator.EstimateExecutionTimeAsync(
                    sampledItems,
                    sampledRings,
                    _itemValidatorFunction,
                    safeSampleSize: _safeBenchmarkSampleSize,
                    skipGarbageCollection: true,
                    progress: iterationProgress,
                    cancellationToken: cancellationToken);
            }

            return estimate;
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
