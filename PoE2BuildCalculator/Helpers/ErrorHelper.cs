namespace PoE2BuildCalculator.Helpers
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
			CustomMessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}
