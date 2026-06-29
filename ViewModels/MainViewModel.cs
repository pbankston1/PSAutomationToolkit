using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using PSAutomationToolkit.Models;
using PSAutomationToolkit.Services;

namespace PSAutomationToolkit.ViewModels
{
    public class CategoryItem
    {
        public string Name  { get; set; } = "";
        public string Icon  { get; set; } = "";
        public int    Count { get; set; }
    }

    public class MainViewModel : BaseViewModel
    {
        private readonly ScriptEngineService _scriptEngine;
        private readonly LoggingService      _logger;
        private readonly SettingsService     _settings;

        // ── Backing fields ──────────────────────────────────────────────
        private CategoryItem?  _selectedCategory;
        private ScriptEntry?   _selectedScript;
        private string         _generatedCode      = string.Empty;
        private string         _searchQuery        = string.Empty;
        private bool           _isSearchActive;
        private string         _statusMessage      = "Ready";
        private bool           _isCodeGenerated;
        private string         _selectedComplexity = "All";
        private string         _selectedTag        = "All Tags";
        private int            _totalCategories;

        // ── Collections ─────────────────────────────────────────────────
        public ObservableCollection<CategoryItem>  Categories      { get; } = new();
        public ObservableCollection<ScriptEntry>   FilteredScripts { get; } = new();
        public ObservableCollection<string>        AvailableTags   { get; } = new();

        public MainViewModel(ScriptEngineService scriptEngine,
                             LoggingService      logger,
                             SettingsService     settings)
        {
            _scriptEngine = scriptEngine;
            _logger       = logger;
            _settings     = settings;

            GenerateCodeCommand    = new RelayCommand(GenerateCode,    () => _selectedScript != null);
            CopyToClipboardCommand = new RelayCommand(CopyToClipboard, () => _isCodeGenerated);
            SaveAsPs1Command       = new RelayCommand(SaveAsPs1,       () => _isCodeGenerated);
            SaveAsTxtCommand       = new RelayCommand(SaveAsTxt,       () => _isCodeGenerated);
            ClearSearchCommand     = new RelayCommand(ClearSearch);
            OpenLogFolderCommand   = new RelayCommand(OpenLogFolder);

            LoadData();
        }

        // ── Properties ──────────────────────────────────────────────────

        public CategoryItem? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetField(ref _selectedCategory, value))
                {
                    _isSearchActive = false;
                    SearchQuery     = string.Empty;
                    SelectedTag     = "All Tags";
                    LoadScriptsForCategory();
                    SelectedScript  = null;
                    GeneratedCode   = string.Empty;
                    IsCodeGenerated = false;
                }
            }
        }

        public ScriptEntry? SelectedScript
        {
            get => _selectedScript;
            set
            {
                if (SetField(ref _selectedScript, value))
                {
                    IsCodeGenerated = false;
                    GeneratedCode   = string.Empty;
                    if (value != null)
                        StatusMessage = $"Selected: {value.TaskName}  [{value.ComplexityLevel}]";
                }
            }
        }

        public string GeneratedCode
        {
            get => _generatedCode;
            set => SetField(ref _generatedCode, value);
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (SetField(ref _searchQuery, value))
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        _isSearchActive = true;
                        PerformSearch();
                    }
                    else
                    {
                        _isSearchActive = false;
                        LoadScriptsForCategory();
                    }
                }
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetField(ref _statusMessage, value);
        }

        public bool IsCodeGenerated
        {
            get => _isCodeGenerated;
            set => SetField(ref _isCodeGenerated, value);
        }

        public string SelectedComplexity
        {
            get => _selectedComplexity;
            set { if (SetField(ref _selectedComplexity, value)) LoadScriptsForCategory(); }
        }

        public string SelectedTag
        {
            get => _selectedTag;
            set { if (SetField(ref _selectedTag, value)) LoadScriptsForCategory(); }
        }

        public List<string> ComplexityFilters { get; } = new() { "All", "Entry", "Mid", "Senior" };

        public int TotalScripts     => _scriptEngine.TotalScriptCount;
        public int TotalCategories  { get => _totalCategories; set => SetField(ref _totalCategories, value); }

        // ── Commands ────────────────────────────────────────────────────
        public ICommand GenerateCodeCommand    { get; }
        public ICommand CopyToClipboardCommand { get; }
        public ICommand SaveAsPs1Command       { get; }
        public ICommand SaveAsTxtCommand       { get; }
        public ICommand ClearSearchCommand     { get; }
        public ICommand OpenLogFolderCommand   { get; }

        // ── Private Methods ─────────────────────────────────────────────

        private void LoadData()
        {
            try
            {
                _scriptEngine.LoadScripts();
                var counts = _scriptEngine.GetCategoryCounts();

                Categories.Clear();
                foreach (var cat in _scriptEngine.GetCategories())
                {
                    Categories.Add(new CategoryItem
                    {
                        Name  = cat,
                        Icon  = _scriptEngine.GetCategoryIcon(cat),
                        Count = counts.TryGetValue(cat, out var c) ? c : 0
                    });
                }

                // Populate global tag list
                AvailableTags.Clear();
                AvailableTags.Add("All Tags");
                foreach (var tag in _scriptEngine.GetAllTags())
                    AvailableTags.Add(tag);

                TotalCategories = Categories.Count;
                StatusMessage   = $"Loaded {_scriptEngine.TotalScriptCount} scripts across {Categories.Count} categories";
                _logger.Log("Application Started", $"Loaded {_scriptEngine.TotalScriptCount} scripts in {Categories.Count} categories");

                if (Categories.Count > 0)
                    SelectedCategory = Categories[0];
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading scripts: {ex.Message}";
                _logger.Log("Load Error", ex.Message, "ERROR");
                MessageBox.Show($"Failed to load script library:\n{ex.Message}",
                    "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadScriptsForCategory()
        {
            FilteredScripts.Clear();
            if (_selectedCategory == null) return;

            var showAdvanced = _settings.Settings.ShowAdvancedScripts;
            var scripts      = _scriptEngine.GetScriptsByCategory(_selectedCategory.Name, showAdvanced).AsEnumerable();

            // Complexity filter
            if (_selectedComplexity != "All" &&
                Enum.TryParse<ComplexityLevel>(_selectedComplexity, out var lvl))
                scripts = scripts.Where(s => s.ComplexityLevel == lvl);

            // Tag filter
            if (_selectedTag != "All Tags" && !string.IsNullOrEmpty(_selectedTag))
                scripts = scripts.Where(s => s.Tags.Contains(_selectedTag, StringComparer.OrdinalIgnoreCase));

            foreach (var s in scripts)
                FilteredScripts.Add(s);

            StatusMessage = _isSearchActive
                ? StatusMessage
                : $"{_selectedCategory.Name} — {FilteredScripts.Count} scripts";
        }

        private void PerformSearch()
        {
            FilteredScripts.Clear();
            var results = _scriptEngine.Search(_searchQuery);

            // Apply complexity filter to search results too
            if (_selectedComplexity != "All" &&
                Enum.TryParse<ComplexityLevel>(_selectedComplexity, out var lvl))
                results = results.Where(s => s.ComplexityLevel == lvl);

            foreach (var s in results)
                FilteredScripts.Add(s);

            StatusMessage = $"Search \"{_searchQuery}\": {FilteredScripts.Count} results";
        }

        private void GenerateCode()
        {
            if (_selectedScript == null) return;

            GeneratedCode   = _selectedScript.PowerShellCode;
            IsCodeGenerated = true;
            StatusMessage   = $"✓ Generated: {_selectedScript.TaskName}";

            _logger.Log("Script Generated",
                $"Category={_selectedScript.Category} | Task={_selectedScript.TaskName} | Level={_selectedScript.ComplexityLevel}");
        }

        private void CopyToClipboard()
        {
            if (string.IsNullOrEmpty(_generatedCode)) return;
            try
            {
                Clipboard.SetText(_generatedCode);
                StatusMessage = "✓ Code copied to clipboard";
                _logger.Log("Clipboard Copy", _selectedScript?.TaskName ?? "");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Copy failed: {ex.Message}";
            }
        }

        private void SaveAsPs1() => SaveFile(".ps1", "PowerShell Script|*.ps1");
        private void SaveAsTxt() => SaveFile(".txt", "Text File|*.txt");

        private void SaveFile(string ext, string filter)
        {
            if (string.IsNullOrEmpty(_generatedCode)) return;

            var defaultName = _selectedScript != null
                ? SanitizeFileName(_selectedScript.TaskName) + ext
                : "script" + ext;

            var dlg = new SaveFileDialog
            {
                Title            = "Save Script",
                FileName         = defaultName,
                DefaultExt       = ext,
                Filter           = filter + "|All Files|*.*",
                InitialDirectory = _settings.Settings.DefaultSaveDirectory
            };

            if (dlg.ShowDialog() != true) return;

            try
            {
                File.WriteAllText(dlg.FileName, _generatedCode, System.Text.Encoding.UTF8);
                StatusMessage = $"✓ Saved: {Path.GetFileName(dlg.FileName)}";
                _logger.Log("Script Saved",
                    $"File={dlg.FileName} | Task={_selectedScript?.TaskName}");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Save failed: {ex.Message}";
                _logger.Log("Save Error", ex.Message, "ERROR");
                MessageBox.Show($"Failed to save file:\n{ex.Message}", "Save Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearSearch()
        {
            SearchQuery     = string.Empty;
            SelectedTag     = "All Tags";
            _isSearchActive = false;
            LoadScriptsForCategory();
        }

        private void OpenLogFolder()
        {
            try
            {
                var logPath = Path.GetDirectoryName(_logger.GetLogPath());
                if (logPath != null && Directory.Exists(logPath))
                    System.Diagnostics.Process.Start("explorer.exe", logPath);
            }
            catch { }
        }

        private static string SanitizeFileName(string name)
            => string.Concat(name.Select(c =>
                    Path.GetInvalidFileNameChars().Contains(c) ? '_' : c))
                .Replace(' ', '_');
    }
}
