using System.Windows.Input;
using Microsoft.Win32;
using PSAutomationToolkit.Models;
using PSAutomationToolkit.Services;

namespace PSAutomationToolkit.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly SettingsService _settingsService;
        private AppSettings _settings;

        public SettingsViewModel(SettingsService settingsService)
        {
            _settingsService = settingsService;
            _settings        = CloneSettings(settingsService.Settings);

            BrowseSaveDirectoryCommand = new RelayCommand(BrowseSaveDirectory);
            SaveCommand                = new RelayCommand(Save);
        }

        public bool IsDarkTheme
        {
            get => _settings.IsDarkTheme;
            set { _settings.IsDarkTheme = value; OnPropertyChanged(); }
        }

        public string DefaultSaveDirectory
        {
            get => _settings.DefaultSaveDirectory;
            set { _settings.DefaultSaveDirectory = value; OnPropertyChanged(); }
        }

        public bool ShowAdvancedScripts
        {
            get => _settings.ShowAdvancedScripts;
            set { _settings.ShowAdvancedScripts = value; OnPropertyChanged(); }
        }

        public ICommand BrowseSaveDirectoryCommand { get; }
        public ICommand SaveCommand { get; }

        private void BrowseSaveDirectory()
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Select Default Save Directory",
                InitialDirectory = DefaultSaveDirectory
            };
            if (dialog.ShowDialog() == true)
                DefaultSaveDirectory = dialog.FolderName;
        }

        private void Save()
        {
            _settingsService.Update(_settings);
        }

        private static AppSettings CloneSettings(AppSettings src) => new()
        {
            IsDarkTheme          = src.IsDarkTheme,
            DefaultSaveDirectory = src.DefaultSaveDirectory,
            ShowAdvancedScripts  = src.ShowAdvancedScripts,
            PrimaryColor         = src.PrimaryColor,
            AccentColor          = src.AccentColor,
        };
    }
}
