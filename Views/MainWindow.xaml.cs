using System.Windows;
using PSAutomationToolkit.ViewModels;

namespace PSAutomationToolkit.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            var win = App.GetService<SettingsWindow>();
            win.Owner = this;
            win.ShowDialog();
        }

        private void OpenLog_Click(object sender, RoutedEventArgs e)
        {
            var win = new LogViewerWindow { Owner = this };
            win.Show();
        }
    }
}
