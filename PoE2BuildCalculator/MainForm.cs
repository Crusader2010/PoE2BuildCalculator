using Domain.Combinations;
using Domain.Main;
using Domain.Static;
using System.Collections.Immutable;

namespace PoE2BuildCalculator
{
    public partial class MainForm : Form
    {
        // Class references
        private Manager.FileParser _fileParser { get; set; }
        private TierManager _formTierManager { get; set; }
        private CustomValidator _customValidator { get; set; }
        internal Func<List<Item>, bool> _itemValidatorFunction { get; set; } = x => true;
        private ImmutableList<Item> _parsedItems { get; set; } = [];

        private IEnumerable<List<Item>> _combinations { get; set; } = [];

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
        }

        private void ButtonOpenItemListFile_Click(object sender, EventArgs e)
        {
            OpenPoE2ItemList.ShowDialog();
            if (!string.IsNullOrWhiteSpace(OpenPoE2ItemList.FileName))
            {
                _fileParser = new Manager.FileParser(OpenPoE2ItemList.FileName);
                StatusBarLabel.Text = $"Loaded file: {OpenPoE2ItemList.FileName}";
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
            parseButton!.Enabled = false;
            ButtonOpenItemListFile.Enabled = false;
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
                ButtonOpenItemListFile.Enabled = true;

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
                _cts.Cancel(true);
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
            if (_fileParser == null)
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

        private void ButtonComputeCombinations_Click(object sender, EventArgs e)
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

            var itemsForClasses = _parsedItems.Where(x => Constants.ITEM_CLASSES.Contains(x.Class, StringComparer.OrdinalIgnoreCase)).GroupBy(x => x.Class).ToDictionary(x => x.Key, x => x.ToList());
            itemsForClasses.Remove(Constants.ITEM_CLASS_RING, out var rings);

            var itemsWithoutRingsInput = new List<List<Item>>();
            itemsWithoutRingsInput.AddRange(itemsForClasses.Values);

            _combinations = CombinationGenerator.GenerateCombinations(itemsWithoutRingsInput, rings, _itemValidatorFunction);
            TextboxDisplay.Text = $"Total number of combinations: {_combinations.LongCount()}";
        }

        private void ButtonManageCustomValidator_Click(object sender, EventArgs e)
        {
            _customValidator = _customValidator == null || _customValidator.IsDisposed ? new CustomValidator(this) : _customValidator;
            _customValidator.Show(this);
        }
    }
}