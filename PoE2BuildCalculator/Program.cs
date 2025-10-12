namespace PoE2BuildCalculator
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // ✅ ADD: Global exception handlers to catch unhandled exceptions
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            // ✅ Silently handle expected cancellation exceptions
            if (e.Exception is OperationCanceledException)
            {
                return;
            }

            MessageBox.Show(
                $"An error occurred:\n\n{e.Exception}",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // ✅ Silently handle expected cancellation exceptions
            if (e.ExceptionObject is OperationCanceledException)
            {
                return;
            }

            var ex = e.ExceptionObject as Exception;
            MessageBox.Show(
                $"A critical error occurred:\n\n{ex?.ToString() ?? "Unknown error"}",
                "Critical Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}