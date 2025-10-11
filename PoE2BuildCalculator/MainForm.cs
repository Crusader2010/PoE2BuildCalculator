using Domain.Combinations;
using Domain.Helpers;
using Domain.Main;
using Domain.Static;
using System.Collections.Immutable;

namespace PoE2BuildCalculator
{
    public partial class MainForm : Form, IDisposable
    {
        // Field for thread synchronization
        private readonly object _validatorLock = new();
        private readonly long _maxCombinationsToStore = 10000000;
        private readonly long _safeBenchmarkSampleSize = 500000;

        // Class references
        private Manager.FileParser _fileParser { get; set; }
        private TierManager _formTierManager { get; set; }
        private CustomValidator _customValidator { get; set; }
        internal Func<List<Item>, bool> _itemValidatorFunction { get; set; } = x => true;
        private ImmutableList<Item> _parsedItems { get; set; } = [];

        private List<List<Item>> _combinations { get; set; } = [];
        private List<List<Item>> _benchmarkSampledItems;
        private List<Item> _benchmarkSampledRings;
        private readonly object _benchmarkSampleLock = new();

        // Progress UI
        private ToolStripProgressBar _statusProgressBar;

        // Cancellation support
        private CancellationTokenSource _cts;
        private ToolStripButton _cancelButton;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ConfigureOpenFileDialog();

            ConfigureParserControls();

            // Wire up FormClosing for cleanup
            this.FormClosing += MainForm_FormClosing;
        }

        private void ButtonOpenItemListFile_Click(object sender, EventArgs e)
        {
            var dialogResult = OpenPoE2ItemList.ShowDialog(this);
            if (dialogResult == DialogResult.OK && !string.IsNullOrWhiteSpace(OpenPoE2ItemList.FileName) && File.Exists(OpenPoE2ItemList.FileName))
            {
                _fileParser = new Manager.FileParser(OpenPoE2ItemList.FileName);
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

            // Prevent reentrancy from the same button.
            var parseButton = sender as Control;
            PanelButtons!.Enabled = false;
            TextboxDisplay.Text = string.Empty;

            // Create a new CancellationTokenSource for this parse.
            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            // Reset UI and start parsing.
            if (_statusProgressBar != null)
            {
                _statusProgressBar.Value = 0;
                _statusProgressBar.Visible = true;
            }

            if (_cancelButton != null)
            {
                _cancelButton.Enabled = true;
                _cancelButton.Visible = true;
            }

            StatusBarLabel.Text = $"Parsing... 0%";

            // Use Progress<int> to marshal updates to the UI thread (captured SynchronizationContext).
            var progress = new Progress<int>(p =>
            {
                if (_statusProgressBar != null) _statusProgressBar.Value = p;
                StatusBarLabel.Text = $"Parsing file... {p}%";
            });

            try
            {
                // Await the parser directly (no blocking GetResult). This keeps the UI responsive.
                await _fileParser.ParseFileAsync(progress, _cts.Token).ConfigureAwait(true);

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
                StatusBarLabel.Text = $"Error: {ex}";
            }
            finally
            {
                // Restore UI state.
                if (_statusProgressBar != null)
                {
                    _statusProgressBar.Visible = false;
                    _statusProgressBar.Value = 0;
                }

                if (_cancelButton != null)
                {
                    _cancelButton.Enabled = false;
                    _cancelButton.Visible = false;
                }

                parseButton.Enabled = true;
                PanelButtons.Enabled = true;

                // Dispose and clear the CTS used for this parse.
                try
                {
                    _cts?.Dispose();
                }
                catch { }
                _cts = null;
            }
        }

        private void ButtonCancelParse_Click(object sender, EventArgs e)
        {
            // If there's no active parse, nothing to do.
            if (_cts == null)
                return;

            // Disable to prevent multiple clicks.
            if (_cancelButton != null) _cancelButton.Enabled = false;

            StatusBarLabel.Text = "Cancelling...";
            try
            {
                // Request cancellation.
                _cts.Cancel();
            }
            catch
            {
                // ignore exceptions from cancel attempt
            }
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
            // Create and add a ToolStripProgressBar to the StatusStrip (next to existing StatusBarLabel).
            var parent = StatusBarLabel.GetCurrentParent();
            if (parent != null)
            {
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

                // Add a cancel button to the StatusStrip so the user can cancel parsing.
                _cancelButton = new ToolStripButton
                {
                    Name = "ButtonCancelParse",
                    Text = "Cancel",
                    BackColor = Color.DeepPink,
                    Enabled = false,
                    Visible = false
                };

                _cancelButton.Click += ButtonCancelParse_Click;
                parent.Items.Add(_cancelButton);
            }
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
            if (_parsedItems.Count == 0)
            {
                StatusBarLabel.Text = "There are no valid items in the chosen file. No combinations can be computed.";
                return;
            }

            var itemsForClasses = _parsedItems
                .Where(x => Constants.ITEM_CLASSES.Contains(x.Class, StringComparer.OrdinalIgnoreCase))
                .GroupBy(x => x.Class)
                .ToDictionary(x => x.Key, x => x.ToList());

            // Extract rings (may be null or empty)
            itemsForClasses.TryGetValue(Constants.ITEM_CLASS_RING, out var rings);
            itemsForClasses.Remove(Constants.ITEM_CLASS_RING);

            // Get other item lists (filter out empties handled in generator)
            var itemsWithoutRingsInput = itemsForClasses.Values.ToList();

            // Check if we have any items at all
            bool hasItems = itemsWithoutRingsInput.Any(list => list.Count > 0);
            bool hasRings = rings != null && rings.Count > 0;

            if (!hasItems && !hasRings)
            {
                MessageBox.Show("No items found in any category. Cannot generate combinations.", "No Items", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Quick count first
            var totalCount = CombinationGenerator.ComputeTotalCombinations(itemsWithoutRingsInput, rings);
            if (totalCount == 0)
            {
                MessageBox.Show("No combinations possible. All item class lists are empty.", "No Combinations", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Show count and ask for confirmation if large
            string countMessage = $"Total combinations to process: {totalCount}";
            countMessage += $"\n\nThis is approximately {CommonHelper.GetBigIntegerApproximation(totalCount)} combinations.";
            countMessage += "\n\nConsider using the Benchmark feature first to estimate how much time this will take.";
            countMessage += "\n\nDo you want to proceed?";

            var dialog = MessageBox.Show(countMessage, "Combination Count", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialog != DialogResult.Yes)
            {
                StatusBarLabel.Text = "Operation cancelled by user.";
                return;
            }

            // Disable button during processing
            TextboxDisplay.Text = "Processing combinations...\r\n\r\n";

            PrepareUIForProgressReporting();

            // Progress reporter
            var progress = new Progress<CombinationProgress>(p =>
            {
                if (_statusProgressBar != null && p?.PercentComplete <= 100)
                {
                    _statusProgressBar.Value = (int)p.PercentComplete;
                }

                StatusBarLabel.Text = $"Processing: {p.PercentComplete:F2}% | " +
                                     $"Processed: {p.ProcessedCombinations} | " +
                                     $"Valid: {p.ValidCombinations} | " +
                                     $"Elapsed: {p.ElapsedTime:hh\\:mm\\:ss}";

                TextboxDisplay.Text = $"Progress: {p.PercentComplete:F2}%\r\n" +
                                     $"Processed: {p.ProcessedCombinations}\r\n" +
                                     $"Valid: {p.ValidCombinations}\r\n" +
                                     $"Elapsed: {p.ElapsedTime:hh\\:mm\\:ss}";
            });

            try
            {
                var result = await Task.Run(() =>
                {
                    try
                    {
                        var inner = CombinationGenerator.GenerateCombinationsParallel(
                                        itemsWithoutRingsInput,
                                        rings,
                                        _itemValidatorFunction,
                                        progress,
                                        maxValidToStore: _maxCombinationsToStore,
                                        _cts.Token);

                        return inner;
                    }
                    catch (Exception ex) when (ex is not OperationCanceledException)
                    {
                        throw;
                    }
                }, _cts.Token);

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

                if (result.ValidCombinations > 1000000)
                {
                    summary.AppendLine();
                    summary.AppendLine($"NOTE: Only the first {_maxCombinationsToStore} valid combinations were stored.");
                }

                TextboxDisplay.Text = summary.ToString();
                StatusBarLabel.Text = $"Complete! {result.ValidCombinations} valid combinations found.";

                MessageBox.Show(summary.ToString(), "Generation Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
                StatusBarLabel.Text = $"Error during processing.";
                MessageBox.Show($"An error occurred:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ResetUIStateAfterCancelling();
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
            // Cancel any ongoing parsing
            try
            {
                _cts?.Cancel();
                _cts?.Dispose();
                _cts = null;
            }
            catch { }

            // Dispose custom validator if it exists
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

        private void ButtonBenchmark_Click(object sender, EventArgs e)
        {
            if (_fileParser == null)
            {
                MessageBox.Show("No parsed data available. Please load and parse a file first.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _parsedItems = _fileParser.GetParsedItems();
            if (_parsedItems.Count == 0)
            {
                MessageBox.Show("There are no valid items in the chosen file.", "No Items", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var itemsForClasses = _parsedItems
                .Where(x => Constants.ITEM_CLASSES.Contains(x.Class, StringComparer.OrdinalIgnoreCase))
                .GroupBy(x => x.Class)
                .ToDictionary(x => x.Key, x => x.ToList());

            itemsForClasses.TryGetValue(Constants.ITEM_CLASS_RING, out var rings);
            itemsForClasses.Remove(Constants.ITEM_CLASS_RING);

            var itemsWithoutRingsInput = itemsForClasses.Values.ToList();

            bool hasItems = itemsWithoutRingsInput.Any(list => list.Count > 0);
            bool hasRings = rings != null && rings.Count > 0;

            if (!hasItems && !hasRings)
            {
                MessageBox.Show("No items found in any category. Cannot benchmark.", "No Items", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            StatusBarLabel.Text = "Running benchmark...";
            TextboxDisplay.Text = "Benchmarking...\r\nPlease wait, sampling combinations to estimate execution time...";
            Application.DoEvents();

            const int totalIterations = 25;
            const int maxListSampleSize = 100;
            try
            {
                PrepareUIForProgressReporting();

                StatusBarLabel.Text = "Preparing benchmark samples...";
                TextboxDisplay.Text = "Preparing benchmark...\r\n";
                Application.DoEvents();

                // Check for cancellation before pre-sampling
                _cts.Token.ThrowIfCancellationRequested();

                // Pre-sample ONCE (lazy initialization pattern)
                lock (_benchmarkSampleLock)
                {
                    if (_benchmarkSampledItems == null || _benchmarkSampledRings == null)
                    {
                        _benchmarkSampledItems = [.. itemsWithoutRingsInput.Select(static list => list.Take(maxListSampleSize).ToList())];
                        _benchmarkSampledRings = rings?.Take(maxListSampleSize).ToList();

                        TextboxDisplay.AppendText("✓ Samples created\r\n");
                    }
                    else
                    {
                        TextboxDisplay.AppendText("✓ Using cached samples\r\n");
                    }
                }

                // Check for cancellation before warmup
                _cts.Token.ThrowIfCancellationRequested();

                StatusBarLabel.Text = "Warming up...";
                TextboxDisplay.AppendText("Warming up and collecting garbage...\r\n");
                Application.DoEvents();

                // Do GC ONCE before all iterations
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
                GC.WaitForPendingFinalizers();
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);

                TextboxDisplay.AppendText($"✓ Ready\r\n\r\nRunning {totalIterations} benchmark iterations...\r\n");
                Application.DoEvents();

                ExecutionEstimate estimate = null;
                for (int i = 0; i < totalIterations; i++)
                {
                    // Check for cancellation at start of each iteration
                    _cts.Token.ThrowIfCancellationRequested();

                    estimate = CombinationGenerator.EstimateExecutionTime(
                        _benchmarkSampledItems,
                        _benchmarkSampledRings,
                        _itemValidatorFunction,
                        safeSampleSize: _safeBenchmarkSampleSize,
                        skipGarbageCollection: true);  // Skip GC since we did it once

                    // Report progress
                    int percentComplete = ((i + 1) * 100) / totalIterations;
                    if (_statusProgressBar != null)
                    {
                        _statusProgressBar.Value = percentComplete;
                    }

                    StatusBarLabel.Text = $"Benchmark: {i + 1}/{totalIterations} iterations ({percentComplete}%)";

                    // Update display every 3 iterations to avoid UI lag
                    if ((i + 1) % 5 == 0 || i == totalIterations - 1)
                    {
                        TextboxDisplay.AppendText($"  Iteration {i + 1}/{totalIterations} complete\r\n");
                        Application.DoEvents();
                    }
                }

                string summary = estimate.GetFormattedSummary();
                MessageBox.Show(summary, "Benchmark Results", MessageBoxButtons.OK, MessageBoxIcon.Information);

                TextboxDisplay.Text = summary;
                StatusBarLabel.Text = "Benchmark complete.";
            }
            catch (OperationCanceledException)
            {
                TextboxDisplay.AppendText("Benchmark cancelled by user.\r\n");
                StatusBarLabel.Text = "Benchmark cancelled.";
            }
            catch (Exception ex)
            {
                string errorMsg = $"Error during benchmark: \n\n{ex.Message}";
                TextboxDisplay.Text = errorMsg;
                StatusBarLabel.Text = "Benchmark failed.";
                MessageBox.Show(errorMsg, "Benchmark Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ResetUIStateAfterCancelling();
            }
        }

        private void ResetUIStateAfterCancelling()
        {
            if (_statusProgressBar != null)
            {
                _statusProgressBar.Visible = false;
                _statusProgressBar.Value = 0;
            }

            if (_cancelButton != null)
            {
                _cancelButton.Enabled = false;
                _cancelButton.Visible = false;
            }

            PanelButtons.Enabled = true;

            _cts?.Dispose();
            _cts = null;
        }

        private void PrepareUIForProgressReporting()
        {
            PanelButtons.Enabled = false;

            // Create cancellation token
            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            // Show progress bar
            if (_statusProgressBar != null)
            {
                _statusProgressBar.Value = 0;
                _statusProgressBar.Visible = true;
            }

            if (_cancelButton != null)
            {
                _cancelButton.Enabled = true;
                _cancelButton.Visible = true;
            }
        }
    }
}
