using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace PoE2BuildCalculator
{
    public partial class MainForm : Form
    {
        private Manager.FileParser _fileParser { get; set; }

        // Background worker to run parsing off the UI thread.
        private BackgroundWorker _parserWorker;
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

            ConfigureBackgroundParserAndControls();
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

        private void ButtonParseItemListFile_Click(object sender, EventArgs e)
        {
            if (_fileParser is null)
            {
                StatusBarLabel.Text = "No file loaded. Please choose a file first.";
                return;
            }

            if (_parserWorker.IsBusy)
            {
                StatusBarLabel.Text = "Parsing is already in progress.";
                return;
            }

            // Create a new CancellationTokenSource for this parse.
            _cts = new CancellationTokenSource();

            // Reset UI and start background parsing.
            if (_statusProgressBar != null)
            {
                _statusProgressBar.Value = 0;
                _statusProgressBar.Visible = true;
            }

            if (_cancelButton != null)
            {
                _cancelButton.Enabled = true;
            }

            StatusBarLabel.Text = $"Parsing... {_statusProgressBar.Value}%";

            // Pass both parser and CTS to the background worker.
            _parserWorker.RunWorkerAsync(new object[] { _fileParser, _cts });
        }

        private void ButtonCancelParse_Click(object sender, EventArgs e)
        {
            if (!_parserWorker.IsBusy)
                return;

            // Disable to prevent multiple clicks.
            if (_cancelButton != null)
                _cancelButton.Enabled = false;

            StatusBarLabel.Text = "Cancelling...";
            try
            {
                // Request cancellation.
                _cts.Cancel();
                // Also notify BackgroundWorker (it may be used by worker code if it checks CancellationPending).
                _parserWorker.CancelAsync();
            }
            catch
            {
                // ignore exceptions from cancel attempt
            }
        }

        private void ParserWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (sender is not BackgroundWorker worker)
                return;

            if (e.Argument is not object[] args || args.Length != 2)
                return;

            if (args[0] is not Manager.FileParser parser)
                return;

            if (args[1] is not CancellationTokenSource cts)
                return;

            // Provide a progress callback that routes to the BackgroundWorker.
            var progress = new Progress<int>(p =>
            {
                // worker.ReportProgress is safe from any thread.
                worker.ReportProgress(p);
            });

            // Call the async parser synchronously on the background thread and pass the cancellation token.
            try
            {
                parser.ParseFileAsync(progress, cts.Token).GetAwaiter().GetResult();
            }
            catch (OperationCanceledException)
            {
                // Mark the worker as cancelled so RunWorkerCompleted can react accordingly.
                e.Cancel = true;
                return;
            }
            catch (Exception)
            {
                // Let the background worker capture the exception (RunWorkerCompleted will see e.Error).
                throw;
            }
        }

        private void ParserWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (_statusProgressBar != null)
            {
                int value = e.ProgressPercentage; // Math.Max(Math.Min(e.ProgressPercentage, 100), 0);
                _statusProgressBar.Value = value;
            }

            StatusBarLabel.Text = $"Parsing file... {e.ProgressPercentage}%";
        }

        private void ParserWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                StatusBarLabel.Text = $"Error: {e.Error}";
            }
            else if (e.Cancelled)
            {
                StatusBarLabel.Text = "Parsing cancelled.";
            }
            else
            {
                StatusBarLabel.Text = "Parsing completed.";
            }

            if (_statusProgressBar != null)
            {
                _statusProgressBar.Visible = false;
                _statusProgressBar.Value = 0;
            }

            if (_cancelButton != null)
            {
                _cancelButton.Enabled = false;
            }

            // Dispose and clear the CTS used for this parse.
            try
            {
                _cts?.Dispose();
            }
            catch { }
            _cts = null;
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

        private void ConfigureBackgroundParserAndControls()
        {
            // Create and add a ToolStripProgressBar to the StatusStrip (next to existing StatusBarLabel).
            // StatusBarLabel is assumed to be a ToolStripStatusLabel placed on a StatusStrip in the designer.
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
                    Size = new System.Drawing.Size(150, 16)
                };
                parent.Items.Add(_statusProgressBar);

                // Add a cancel button to the StatusStrip so the user can cancel parsing.
                _cancelButton = new ToolStripButton
                {
                    Name = "ButtonCancelParse",
                    Text = "Cancel",
                    Enabled = false,
                    Visible = true
                };

                _cancelButton.Click += ButtonCancelParse_Click;
                parent.Items.Add(_cancelButton);
            }

            // Prepare the BackgroundWorker with cancellation enabled.
            _parserWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            _parserWorker.DoWork += ParserWorker_DoWork;
            _parserWorker.ProgressChanged += ParserWorker_ProgressChanged;
            _parserWorker.RunWorkerCompleted += ParserWorker_RunWorkerCompleted;
        }

        #endregion
    }
}
