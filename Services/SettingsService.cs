using System;
using System.IO;
using Newtonsoft.Json;
using PSAutomationToolkit.Models;

namespace PSAutomationToolkit.Services
{
    public class SettingsService
    {
        private readonly string _settingsPath;
        private AppSettings _settings;

        public SettingsService()
        {
            var appDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "PSAutomationToolkit");
            Directory.CreateDirectory(appDir);
            _settingsPath = Path.Combine(appDir, "settings.json");
            _settings = Load();
        }

        public AppSettings Settings => _settings;

        private AppSettings Load()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    var json = File.ReadAllText(_settingsPath);
                    return JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();
                }
            }
            catch { }
            return new AppSettings
            {
                DefaultSaveDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };
        }

        public void Save()
        {
            try
            {
                var json = JsonConvert.SerializeObject(_settings, Formatting.Indented);
                File.WriteAllText(_settingsPath, json);
            }
            catch { }
        }

        public void Update(AppSettings updated)
        {
            _settings = updated;
            Save();
        }
    }
}
