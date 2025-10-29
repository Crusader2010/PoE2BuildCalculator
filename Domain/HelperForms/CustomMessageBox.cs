using Font = System.Drawing.Font;

namespace Domain.HelperForms
{
	/*
	═══════════════════════════════════════════════════════════════════
	HELPER METHODS REFERENCE
	═══════════════════════════════════════════════════════════════════

	AppendColored(text, color, bold, newLine)
	  - Appends colored text
	  - Optional: bold formatting
	  - Optional: automatic newline

	AppendBold(text, newLine)
	  - Appends bold text

	AppendStyled(text, FontStyle, newLine)
	  - Appends text with any FontStyle
	  - FontStyle: Bold, Italic, Underline, Strikeout, or combinations

	AppendFormatted(text, color, style, newLine)
	  - Most flexible - combines color and style
	  - All parameters optional

	AppendHeader(text, newLine)
	  - Large, bold text for headers
	  - Automatically increases font size by 2pt

	AppendSeparator(character, length)
	  - Draws a line separator
	  - Default: 50 '─' characters
	  - Useful for visual separation 
	
	────────────────────────────────────────────────────────────────────
	EXAMPLE: AppendFormatted with Multiple Options
	────────────────────────────────────────────────────────────────────

	CustomMessageBox.ShowFormatted(rtb =>
	{
		// Regular text
		rtb.AppendFormatted("Normal text\n");

		// Bold red text
		rtb.AppendFormatted(
			"Bold red text\n",
			color: Color.Red,
			style: FontStyle.Bold
		);

		// Italic blue text
		rtb.AppendFormatted(
			"Italic blue text\n",
			color: Color.Blue,
			style: FontStyle.Italic
		);

		// Bold italic green text
		rtb.AppendFormatted(
			"Bold italic green\n",
			color: Color.Green,
			style: FontStyle.Bold | FontStyle.Italic
		);

		// Underlined text
		rtb.AppendFormatted(
			"Underlined text",
			style: FontStyle.Underline
		);

	}, "Formatting Examples", MessageBoxButtons.OK, MessageBoxIcon.Information);

	────────────────────────────────────────────────────────────────────
	If you already have RTF content:
	────────────────────────────────────────────────────────────────────
	
	string rtfContent = @"{\rtf1\ansi {\colortbl;\red255\green0\blue0;\red0\green128\blue0;} \b Error:\b0  \cf1 File not found! \cf0\line\line Please check the path and try again.}";

	CustomMessageBox.ShowRtf(
		rtfContent,
		"Error",
		MessageBoxButtons.OK,
		MessageBoxIcon.Error
	);
	*/

	public partial class CustomMessageBox : BaseForm
	{
		private string _message;
		private string _rtfContent;
		private string _title;
		private bool _wrapText;
		private bool _isRtf;
		private MessageBoxButtons _buttons;
		private MessageBoxIcon _icon;

		private DialogResult _dialogResult1;
		private DialogResult _dialogResult2;

		private CustomMessageBox(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon, bool wrapText, bool isRtf = false, string rtfContent = null)
		{
			InitializeComponent();

			_wrapText = wrapText;
			_message = message;
			_rtfContent = rtfContent;
			_title = title;
			_buttons = buttons;
			_icon = icon;
			_isRtf = isRtf;
		}

		/// <summary>
		/// Shows a CustomMessageBox with plain text.
		/// </summary>
		public static DialogResult Show(string message, string title, MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None, IWin32Window owner = null, bool wrapText = true)
		{
			using var dialog = new CustomMessageBox(message, title, buttons, icon, wrapText);
			return owner != null ? dialog.ShowDialog(owner) : dialog.ShowDialog();
		}

		/// <summary>
		/// Shows a CustomMessageBox with RTF formatted content.
		/// </summary>
		public static DialogResult ShowRtf(string rtfContent, string title, MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None, IWin32Window owner = null, bool wrapText = true)
		{
			using var dialog = new CustomMessageBox(string.Empty, title, buttons, icon, wrapText, isRtf: true, rtfContent: rtfContent);
			return owner != null ? dialog.ShowDialog(owner) : dialog.ShowDialog();
		}

		/// <summary>
		/// Shows a CustomMessageBox with a builder action for formatting.
		/// </summary>
		public static DialogResult ShowFormatted(Action<RichTextBox> formatAction, string title, MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None, IWin32Window owner = null, bool wrapText = true)
		{
			using var dialog = new CustomMessageBox(string.Empty, title, buttons, icon, wrapText, isRtf: true);
			formatAction?.Invoke(dialog.RichTextBoxMessage);
			return owner != null ? dialog.ShowDialog(owner) : dialog.ShowDialog();
		}

		private void CustomMessageBox_Load(object sender, EventArgs e)
		{
			SetControls();
		}

		private void SetControls()
		{
			this.SuspendLayout();

			this.Text = _title;

			// Set content based on format
			if (_isRtf && !string.IsNullOrEmpty(_rtfContent))
			{
				RichTextBoxMessage.Rtf = _rtfContent;
			}
			else if (!_isRtf)
			{
				RichTextBoxMessage.Text = _message;
			}

			RichTextBoxMessage.Select(0, 0);

			// Add smooth scrolling appearance
			RichTextBoxMessage.SelectionLength = 0;
			RichTextBoxMessage.DeselectAll();
			RichTextBoxMessage.SelectionStart = 0;
			RichTextBoxMessage.ScrollToCaret();

			CheckboxWrapText.Checked = _wrapText;
			RichTextBoxMessage.WordWrap = _wrapText;

			// Auto-size width based on content
			using (var g = RichTextBoxMessage.CreateGraphics())
			{
				int minTextWidth = (int)RichTextBoxMessage.Text.Split('\n')
					.Max(line => g.MeasureString(line, RichTextBoxMessage.Font).Width) + 50;

				if (minTextWidth > RichTextBoxMessage.Width)
					this.Width = Math.Min(minTextWidth, Screen.PrimaryScreen.WorkingArea.Width - 100);
			}

			this.MaximumSize = new Size(850, 700);
			this.MinimumSize = new Size(250, 200);

			Button1.Visible = true;
			Button2.Visible = true;
			Button1.Text = "";
			Button2.Text = "";
			this.AcceptButton = Button1;
			this.CancelButton = Button2;
			Button1.Focus();

			Icon systemIcon = _icon switch
			{
				MessageBoxIcon.Error => SystemIcons.Error,
				MessageBoxIcon.Warning => SystemIcons.Warning,
				MessageBoxIcon.Information => SystemIcons.Information,
				MessageBoxIcon.Question => SystemIcons.Question,
				MessageBoxIcon.None => null,
				_ => null
			};

			if (systemIcon != null)
			{
				this.Icon = systemIcon;
				PictureBoxIcon.Image = systemIcon.ToBitmap();
				PictureBoxIcon.Visible = true;
			}
			else
			{
				this.Icon = null;
				PictureBoxIcon.Visible = false;
			}

			switch (_buttons)
			{
				case MessageBoxButtons.OK:
				case MessageBoxButtons.AbortRetryIgnore:
				case MessageBoxButtons.CancelTryContinue:
					Button1.Text = "OK";
					Button2.Visible = false;
					_dialogResult1 = _dialogResult2 = DialogResult.OK;
					break;
				case MessageBoxButtons.OKCancel:
					Button1.Text = "OK";
					Button2.Text = "Cancel";
					Button2.Visible = true;
					_dialogResult1 = DialogResult.OK;
					_dialogResult2 = DialogResult.Cancel;
					break;
				case MessageBoxButtons.YesNoCancel:
				case MessageBoxButtons.YesNo:
				case MessageBoxButtons.RetryCancel:
					Button1.Text = "Yes";
					Button2.Text = "No";
					Button2.Visible = true;
					_dialogResult1 = DialogResult.Yes;
					_dialogResult2 = DialogResult.No;
					break;
				default:
					Button1.Text = "OK";
					Button2.Visible = false;
					_dialogResult1 = _dialogResult2 = DialogResult.OK;
					break;
			}

			this.ResumeLayout();
		}

		private void Button1_Click(object sender, EventArgs e)
		{
			this.DialogResult = _dialogResult1;
		}

		private void Button2_Click(object sender, EventArgs e)
		{
			this.DialogResult = _dialogResult2;
		}

		private void CheckboxWrapText_CheckedChanged(object sender, EventArgs e)
		{
			RichTextBoxMessage.WordWrap = CheckboxWrapText.Checked;
		}
	}

	/// <summary>
	/// Helper class for building formatted messages for CustomMessageBox.
	/// </summary>
	public static class MessageBoxFormatter
	{
		/// <summary>
		/// Appends text with specified color.
		/// </summary>
		public static void AppendColored(this RichTextBox rtb, string text, Color color, bool bold = false, bool newLine = false)
		{
			int start = rtb.TextLength;
			rtb.AppendText(text + (newLine ? Environment.NewLine : ""));
			rtb.Select(start, text.Length);
			rtb.SelectionColor = color;
			rtb.SelectionFont = bold ? new Font(rtb.Font, FontStyle.Bold) : new Font(rtb.Font, rtb.Font.Style);

			rtb.Select(rtb.TextLength, 0);
		}

		/// <summary>
		/// Appends bold text.
		/// </summary>
		public static void AppendNewLine(this RichTextBox rtb)
		{
			rtb.AppendText(Environment.NewLine);
		}

		/// <summary>
		/// Appends bold text.
		/// </summary>
		public static void AppendBold(this RichTextBox rtb, string text, bool newLine = false)
		{
			int start = rtb.TextLength;
			rtb.AppendText(text + (newLine ? Environment.NewLine : ""));
			rtb.Select(start, text.Length);
			rtb.SelectionFont = new Font(rtb.Font, FontStyle.Bold);
			rtb.Select(rtb.TextLength, 0);
		}

		/// <summary>
		/// Appends text with custom font style.
		/// </summary>
		public static void AppendStyled(this RichTextBox rtb, string text, FontStyle style, bool newLine = false)
		{
			int start = rtb.TextLength;
			rtb.AppendText(text + (newLine ? Environment.NewLine : ""));
			rtb.Select(start, text.Length);
			rtb.SelectionFont = new Font(rtb.Font, style);
			rtb.Select(rtb.TextLength, 0);
		}

		/// <summary>
		/// Appends text with custom color and font style.
		/// </summary>
		public static void AppendFormatted(this RichTextBox rtb, string text, Color? color = null, FontStyle? style = null, bool newLine = false)
		{
			int start = rtb.TextLength;
			rtb.AppendText(text + (newLine ? Environment.NewLine : ""));
			rtb.Select(start, text.Length);
			if (color.HasValue) rtb.SelectionColor = color.Value;
			if (style.HasValue) rtb.SelectionFont = new Font(rtb.Font, style.Value);
			rtb.Select(rtb.TextLength, 0);
		}

		/// <summary>
		/// Appends a header (large, bold text).
		/// </summary>
		public static void AppendHeader(this RichTextBox rtb, string text, bool newLine = true)
		{
			int start = rtb.TextLength;
			rtb.AppendText(text + (newLine ? Environment.NewLine : ""));
			rtb.Select(start, text.Length);
			rtb.SelectionFont = new Font(rtb.Font.FontFamily, rtb.Font.Size + 2, FontStyle.Bold);
			rtb.Select(rtb.TextLength, 0);
		}

		/// <summary>
		/// Appends a section separator.
		/// </summary>
		public static void AppendSeparator(this RichTextBox rtb, Color? color, FontStyle? style, char character = '─', int length = 50)
		{
			string text = new string(character, length) + Environment.NewLine;

			int start = rtb.TextLength;
			rtb.AppendText(text);
			rtb.Select(start, text.Length);
			if (color.HasValue) rtb.SelectionColor = color.Value;
			if (style.HasValue) rtb.SelectionFont = new Font(rtb.Font, style.Value);
			rtb.Select(rtb.TextLength, 0);
		}
	}
}
