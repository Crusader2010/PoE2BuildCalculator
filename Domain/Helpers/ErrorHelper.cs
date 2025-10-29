using Domain.HelperForms;

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
			CustomMessageBox.ShowFormatted(x =>
			{
				x.AppendColored(@"Application error:", Color.DarkRed, true, true);
				x.AppendSeparator(Color.DarkRed, FontStyle.Bold, '-', 20);
				x.AppendNewLine();
				x.AppendFormatted(message, Color.DarkBlue, FontStyle.Regular, false);
			}
			, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}
