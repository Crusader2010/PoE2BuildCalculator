namespace Domain.Helpers
{
    public static class ErrorHelper
    {
        /// <summary>
        /// Shows an error message in a message box with copy-paste support.
        /// </summary>
        /// <param name="error">The error/exception to display.</param>
        /// <param name="caption">Optional caption for the message box.</param>
        public static void ShowError(Exception error, string caption = "Error")
        {
            ShowError(error.ToString(), caption);
        }

        /// <summary>
        /// Shows a custom error message in a message box with copy-paste support.
        /// </summary>
        /// <param name="message">The error message to display.</param>
        /// <param name="caption">Optional caption for the message box.</param>
        public static void ShowError(string message, string caption = "Error")
        {
            using var dialog = new ErrorDialog(message, caption);
            dialog.ShowDialog();
        }

        // Custom dialog for displaying long error messages
        private class ErrorDialog : Form
        {
            public ErrorDialog(string message, string caption)
            {
                Text = caption;
                Size = new Size(600, 400);
                StartPosition = FormStartPosition.CenterParent;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;

                var textBox = new TextBox
                {
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Both,
                    Dock = DockStyle.Fill,
                    Text = message,
                    Font = new Font(FontFamily.GenericMonospace, 10),
                    WordWrap = false
                };

                var okButton = new Button
                {
                    Text = "OK",
                    DialogResult = DialogResult.OK,
                    Height = 40,
                    Width = 80,
                    Anchor = AnchorStyles.Right | AnchorStyles.Bottom
                };

                var copyButton = new Button
                {
                    Text = "Copy",
                    Height = 40,
                    Width = 80,
                    Anchor = AnchorStyles.Left | AnchorStyles.Bottom
                };
                copyButton.Click += (s, e) => Clipboard.SetText(textBox.Text);

                var buttonPanel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Bottom,
                    FlowDirection = FlowDirection.RightToLeft,
                    Height = 50,
                    Padding = new Padding(10),
                };
                buttonPanel.Controls.Add(okButton);
                buttonPanel.Controls.Add(copyButton);

                Controls.Add(textBox);
                Controls.Add(buttonPanel);

                AcceptButton = okButton;
            }
        }
    }
}
