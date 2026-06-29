using System.Collections.Generic;

namespace PSAutomationToolkit.Models
{
    public class ScriptEntry
    {
        public string Category { get; set; } = string.Empty;
        public string TaskName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PowerShellCode { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
        public ComplexityLevel ComplexityLevel { get; set; } = ComplexityLevel.Entry;
    }

    public enum ComplexityLevel
    {
        Entry,
        Mid,
        Senior
    }

    public class CategoryGroup
    {
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public List<ScriptEntry> Scripts { get; set; } = new();
    }

    public class ScriptLibrary
    {
        public List<ScriptEntry> Scripts { get; set; } = new();
    }

    public class AppSettings
    {
        public bool IsDarkTheme { get; set; } = true;
        public string DefaultSaveDirectory { get; set; } = string.Empty;
        public bool ShowAdvancedScripts { get; set; } = true;
        public string PrimaryColor { get; set; } = "Blue";
        public string AccentColor { get; set; } = "LightBlue";
    }

    public class LogEntry
    {
        public System.DateTime Timestamp { get; set; } = System.DateTime.Now;
        public string Action { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string Level { get; set; } = "INFO";
    }
}
