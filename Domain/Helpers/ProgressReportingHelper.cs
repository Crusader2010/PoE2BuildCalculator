namespace Domain.Helpers
{
    /// <summary>
    /// Manages progress reporting UI elements (progress bar, cancel button, status label).
    /// </summary>
    public sealed class ProgressReportingHelper : IDisposable
    {
        private readonly ToolStripProgressBar _progressBar;
        private readonly ToolStrip _statusStrip;
        private readonly ToolStripButton _cancelButton;
        private readonly ToolStripStatusLabel _statusLabel;
        private readonly Panel _controlPanel;
        private CancellationTokenSource _cts;
        private bool _isActive;

        public ProgressReportingHelper(ToolStripProgressBar progressBar, ToolStripButton cancelButton, ToolStripStatusLabel statusLabel, Panel controlPanel)
        {
            _progressBar = progressBar ?? throw new ArgumentNullException(nameof(progressBar));
            _statusStrip = _progressBar.GetCurrentParent();
            _cancelButton = cancelButton ?? throw new ArgumentNullException(nameof(cancelButton));
            _statusLabel = statusLabel ?? throw new ArgumentNullException(nameof(statusLabel));
            _controlPanel = controlPanel ?? throw new ArgumentNullException(nameof(controlPanel));
        }

        public CancellationToken Token => _cts?.Token ?? CancellationToken.None;


        /// <summary>
        /// Updates progress and ensures progress bar is visible.
        /// </summary>
        public void UpdateProgress(double percentComplete, string statusText)
        {
            if (!_isActive) return;

            // Show progress bar on first update
            if (!_progressBar.Visible)
            {

                _statusStrip?.SuspendLayout();

                _progressBar.Visible = true;
                _progressBar.Value = (int)Math.Clamp(percentComplete, 0.0d, 100.0d);
                _statusLabel.Text = statusText;

                _statusStrip?.ResumeLayout(performLayout: false);
                _statusStrip?.PerformLayout();
            }

            _progressBar.Value = (int)Math.Clamp(percentComplete, 0.0d, 100.0d);
            _statusLabel.Text = statusText;
        }

        /// <summary>
        /// Stops and resets progress reporting UI.
        /// </summary>

        public void Start()
        {
            if (_isActive)
            {
                Stop();
            }

            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            _statusStrip?.SuspendLayout();

            _controlPanel.Enabled = false;
            _cancelButton.Enabled = true;
            _cancelButton.Visible = true;
            _progressBar.Value = 0;
            _progressBar.Visible = true;

            _statusStrip?.ResumeLayout(performLayout: true);
            _statusStrip?.PerformLayout();

            _isActive = true;
        }

        public void Stop()
        {
            if (!_isActive) return;

            _statusStrip?.SuspendLayout();

            _progressBar.Visible = false;
            _progressBar.Value = 0;
            _cancelButton.Enabled = false;
            _cancelButton.Visible = false;
            _controlPanel.Enabled = true;

            _statusStrip?.ResumeLayout(performLayout: true);

            _cts?.Dispose();
            _cts = null;
            _isActive = false;
        }

        /// <summary>
        /// Requests cancellation.
        /// </summary>
        public void Cancel()
        {
            if (_cancelButton.Enabled)
            {
                _cancelButton.Enabled = false;
                _statusLabel.Text = "Cancelling...";
                _cts?.Cancel();
            }
        }

        public void Dispose()
        {
            try
            {
                _cts?.Cancel();  // ✅ Cancel before disposing
            }
            catch { }

            try
            {
                _cts?.Dispose();
            }
            catch { }

            _cts = null;
        }
    }
}