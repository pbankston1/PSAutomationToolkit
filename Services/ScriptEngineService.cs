using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using PSAutomationToolkit.Models;

namespace PSAutomationToolkit.Services
{
    public class ScriptEngineService
    {
        private List<ScriptEntry> _allScripts = new();

        private static readonly Dictionary<string, string> CategoryIcons = new()
        {
            // Original categories
            ["Active Directory – User Management"]   = "👤",
            ["Active Directory – Admin & Reporting"] = "📊",
            ["Windows System Administration"]        = "🖥",
            ["Networking"]                           = "🌐",
            ["Microsoft 365 / Azure AD"]             = "☁",
            ["Exchange Online"]                      = "📧",
            ["Intune / Endpoint Management"]         = "📱",
            ["Security & Monitoring"]                = "🔒",
            ["Automation Workflows"]                 = "⚡",
            // New categories
            ["Identity & Access Management"]         = "🔑",
            ["Endpoint Management"]                  = "💻",
            ["Productivity & Collaboration"]         = "🤝",
            ["Cloud Infrastructure"]                 = "🏗",
            ["Security & Compliance"]                = "🛡",
            ["Server & Infrastructure"]              = "🗄",
            ["Automation & Administration"]          = "⚙",
            ["Data & Analytics"]                     = "📈",
            ["AI Systems"]                           = "🤖",
        };

        // Preferred display order
        private static readonly string[] CategoryOrder = new[]
        {
            "Identity & Access Management",
            "Active Directory – User Management",
            "Active Directory – Admin & Reporting",
            "Endpoint Management",
            "Intune / Endpoint Management",
            "Windows System Administration",
            "Networking",
            "Microsoft 365 / Azure AD",
            "Exchange Online",
            "Productivity & Collaboration",
            "Cloud Infrastructure",
            "Server & Infrastructure",
            "Security & Compliance",
            "Security & Monitoring",
            "Automation & Administration",
            "Automation Workflows",
            "Data & Analytics",
            "AI Systems",
        };

        public void LoadScripts()
        {
            try
            {
                var assembly    = Assembly.GetExecutingAssembly();
                var resourceName = assembly.GetManifestResourceNames()
                    .FirstOrDefault(n => n.EndsWith("Scripts.json"));

                string json;

                if (resourceName != null)
                {
                    using var stream = assembly.GetManifestResourceStream(resourceName)!;
                    using var reader = new StreamReader(stream);
                    json = reader.ReadToEnd();
                }
                else
                {
                    var exeDir   = AppDomain.CurrentDomain.BaseDirectory;
                    var filePath = Path.Combine(exeDir, "Resources", "Scripts.json");
                    json = File.ReadAllText(filePath);
                }

                var library = JsonConvert.DeserializeObject<ScriptLibrary>(json);
                _allScripts = library?.Scripts ?? new List<ScriptEntry>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load script library: {ex.Message}", ex);
            }
        }

        public IEnumerable<string> GetCategories()
        {
            var available = _allScripts.Select(s => s.Category).Distinct().ToHashSet();
            foreach (var cat in CategoryOrder)
                if (available.Contains(cat)) yield return cat;
            // Any extra not in ordered list
            foreach (var cat in available.Except(CategoryOrder))
                yield return cat;
        }

        public string GetCategoryIcon(string category)
            => CategoryIcons.TryGetValue(category, out var icon) ? icon : "📄";

        public IEnumerable<ScriptEntry> GetScriptsByCategory(string category, bool includeAdvanced = true)
        {
            var scripts = _allScripts.Where(s => s.Category == category);
            if (!includeAdvanced)
                scripts = scripts.Where(s => s.ComplexityLevel != ComplexityLevel.Senior);
            return scripts.OrderBy(s => s.TaskName);
        }

        public IEnumerable<ScriptEntry> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return _allScripts;
            var q = query.ToLower();
            return _allScripts.Where(s =>
                s.TaskName.ToLower().Contains(q) ||
                s.Description.ToLower().Contains(q) ||
                s.Category.ToLower().Contains(q) ||
                s.Tags.Any(t => t.ToLower().Contains(q)));
        }

        public int TotalScriptCount => _allScripts.Count;

        public Dictionary<string, int> GetCategoryCounts()
            => _allScripts.GroupBy(s => s.Category)
                          .ToDictionary(g => g.Key, g => g.Count());

        public IEnumerable<string> GetAllTags()
            => _allScripts.SelectMany(s => s.Tags).Distinct().OrderBy(t => t);
    }
}
