using System.Diagnostics;
using System.Windows;
using PSAutomationToolkit.Services;

namespace PSAutomationToolkit.Views
{
    public partial class LogViewerWindow : Window
    {
        private readonly LoggingService _logger;

        public LogViewerWindow()
        {
            InitializeComponent();
            _logger = App.GetService<LoggingService>();
            LoadLogs();
        }

        private void LoadLogs()
        {
            var lines = _logger.GetRecentLogs(200);
            LogTextBox.Text = lines.Length > 0
                ? string.Join("\n", lines)
                : "(No log entries yet)";
            LogPathText.Text = _logger.GetLogPath();

            // Scroll to bottom
            LogScroll.ScrollToEnd();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
            => LoadLogs();

        private void OpenLogFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName        = _logger.GetLogPath(),
                    UseShellExecute = true
                });
            }
            catch { }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
            => Close();
    }
}
