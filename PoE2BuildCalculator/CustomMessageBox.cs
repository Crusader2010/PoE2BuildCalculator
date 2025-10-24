using PoE2BuildCalculator.Helpers;

namespace PoE2BuildCalculator
{
	public partial class CustomMessageBox : BaseForm
	{
		private string _message;
		private string _title;
		private MessageBoxButtons _buttons;
		private MessageBoxIcon _icon;

		private DialogResult _dialogResult1;
		private DialogResult _dialogResult2;

		public CustomMessageBox(string message, string title, MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
		{
			InitializeComponent();

			_message = message;
			_title = title;
			_buttons = buttons;
			_icon = icon;
		}

		/// <summary>
		/// Shows a CustomMessageBox as a modal dialog and returns the result.
		/// Automatically handles initialization and disposal.
		/// </summary>
		public static DialogResult Show(string message, string title, MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None, IWin32Window owner = null)
		{
			using var dialog = new CustomMessageBox(message, title, buttons, icon);
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
			TextBoxMessage.Text = _message;
			TextBoxMessage.Select(0, 0);
			CheckboxWrapText.Checked = false;
			TextBoxMessage.WordWrap = CheckboxWrapText.Checked;

			using (var g = TextBoxMessage.CreateGraphics())
			{
				int minTextWidth = (int)TextBoxMessage.Text.Split('\n')
					.Min(line => g.MeasureString(line, TextBoxMessage.Font).Width) + 50;

				if (minTextWidth > TextBoxMessage.Width) this.Width = minTextWidth;
			}

			Button1.Visible = true;
			Button2.Visible = true;
			Button1.Text = "";
			Button2.Text = "";

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
			TextBoxMessage.WordWrap = CheckboxWrapText.Checked;
		}
	}
}
