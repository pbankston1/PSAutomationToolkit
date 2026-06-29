# PS Automation Toolkit Pro  v2.0

**Professional PowerShell Script Generator for IT Personnel**

A complete WPF desktop application (.NET 8) that lets IT administrators instantly generate
PowerShell automation scripts across 18 domains — from Active Directory and Azure to AI Systems,
Security, and DevOps automation.

---

## What's New in v2.0

| Feature | Detail |
|---|---|
| **84+ scripts** (was 43) | Nearly doubled the library |
| **18 categories** (was 9) | 9 brand-new domains added |
| **Tag filter** | Filter scripts by tag (Azure, SQL, Security, etc.) |
| **Expanded search** | Searches across all 84 scripts instantly |
| **Log folder shortcut** | One-click access to log files from status bar |

---

## Script Categories (18 Domains)

| # | Category | Scripts | Highlights |
|---|---|---|---|
| 1 | **Identity & Access Management** | 7 | Privileged audit, RBAC, MFA report, Conditional Access, stale cleanup |
| 2 | **Active Directory – User Management** | 12 | Create, bulk, copy, disable, groups, export |
| 3 | **Active Directory – Admin & Reporting** | 7 | Inactive, lockouts, password expiry, health check, nested groups |
| 4 | **Endpoint Management** | 4 | WU compliance, software deploy, BitLocker, hardware inventory |
| 5 | **Intune / Endpoint Management** | 2 | Device inventory, compliance status |
| 6 | **Windows System Administration** | 6 | Local users, services, event logs, software, reboot, remote info |
| 7 | **Networking** | 5 | IP config, static IP, DNS records, ping sweep, port test |
| 8 | **Microsoft 365 / Azure AD** | 3 | Create user, assign licenses, sign-in logs |
| 9 | **Exchange Online** | 2 | Shared mailbox, mailbox statistics |
| 10 | **Productivity & Collaboration** | 4 | Teams report, SharePoint sites, OneDrive usage, bulk Teams add |
| 11 | **Cloud Infrastructure** | 5 | Azure inventory, VM power, cost analysis, storage audit, policy compliance |
| 12 | **Server & Infrastructure** | 4 | AD sites, IIS inventory, SQL inventory, Hyper-V health |
| 13 | **Security & Compliance** | 4 | MDE risk, SIEM export, DLP report, Zero Trust assessment |
| 14 | **Security & Monitoring** | 3 | Defender scan, perf report, scheduled health check |
| 15 | **Automation & Administration** | 4 | Scheduled task audit, PS Remoting setup, GPO audit, cert expiry |
| 16 | **Automation Workflows** | 3 | Full onboarding, full offboarding, documentation generator |
| 17 | **Data & Analytics** | 3 | SQL backup status, Power BI usage, file server analytics |
| 18 | **AI Systems** | 6 | Azure OpenAI inventory, Copilot adoption, AI quota/cost, AzureML, AI governance, Content Safety |

---

## Prerequisites

| Requirement | Version |
|---|---|
| Windows OS | 10 or 11 (64-bit) |
| .NET SDK | 8.0 or later |
| Visual Studio | 2022 Community or higher |

Install .NET 8: https://dotnet.microsoft.com/download/dotnet/8

---

## Build Instructions

### Option A — One-Click (Recommended)
```
1. Unzip the project
2. Double-click build.cmd
3. Find the EXE in: Build\Publish\PSAutomationToolkit.exe
```

### Option B — Visual Studio 2022
```
1. Open PSAutomationToolkit\PSAutomationToolkit.csproj
2. Wait for NuGet restore (automatic)
3. Press F5 to run  |  Ctrl+F5 for release
4. Publish: right-click project → Publish → Folder
```

### Option C — dotnet CLI
```cmd
cd PSAutomationToolkit
dotnet run                                                  # dev run
dotnet build -c Release                                     # release build
dotnet publish -c Release -r win-x64 --self-contained true ^
  -p:PublishSingleFile=true -o ..\dist                      # single EXE
```

---

## Project Structure

```
PSAutomationToolkit/
├── PSAutomationToolkit.csproj        .NET 8 WPF project
├── App.xaml / App.xaml.cs           DI container + global styles
├── build.cmd                         One-click build
├── README.md
│
├── Models/
│   └── Models.cs                    ScriptEntry, AppSettings, LogEntry, enums
│
├── Services/
│   ├── ScriptEngineService.cs       Loads & queries the 84-script library
│   ├── LoggingService.cs            Daily rotating log (LocalAppData)
│   └── SettingsService.cs           JSON-persisted settings
│
├── ViewModels/
│   ├── BaseViewModel.cs             INotifyPropertyChanged base
│   ├── RelayCommand.cs              ICommand implementation
│   ├── Converters.cs                5 IValueConverter implementations
│   ├── MainViewModel.cs             Core logic: search, filter, generate, export
│   └── SettingsViewModel.cs         Settings dialog binding
│
├── Views/
│   ├── MainWindow.xaml              3-panel UI: sidebar / script list / code
│   ├── MainWindow.xaml.cs
│   ├── SettingsWindow.xaml          Theme, directory, advanced toggle
│   ├── SettingsWindow.xaml.cs
│   ├── LogViewerWindow.xaml         Live activity log
│   └── LogViewerWindow.xaml.cs
│
└── Resources/
    └── Scripts.json                 84 categorized PowerShell scripts
```

---

## NuGet Packages

| Package | Purpose |
|---|---|
| MaterialDesignThemes 4.9.0 | Modern dark/light UI |
| MaterialDesignColors 2.1.4 | Color palette |
| CommunityToolkit.Mvvm 8.2.2 | MVVM helpers |
| Newtonsoft.Json 13.0.3 | JSON serialization |
| AvalonEdit 6.3.0.90 | Code editor component |
| Microsoft.Extensions.DependencyInjection 8.0.0 | DI container |

---

## Adding Your Own Scripts

Edit `Resources/Scripts.json` — no code changes required:

```json
{
  "Category": "AI Systems",
  "TaskName": "My Custom AI Script",
  "Description": "Brief plain-English description.",
  "Tags": ["AI", "Azure", "Custom"],
  "ComplexityLevel": "Mid",
  "PowerShellCode": "# Your PowerShell code here\nWrite-Host 'Hello AI!'"
}
```

**ComplexityLevel**: `Entry` · `Mid` · `Senior`

---

## Data Storage

```
%LOCALAPPDATA%\PSAutomationToolkit\
├── settings.json
└── Logs\
    └── PSToolkit_YYYYMMDD.log
```

---

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    WPF Views (XAML)                          │
│   MainWindow │ SettingsWindow │ LogViewerWindow              │
└──────────────────────┬──────────────────────────────────────┘
                       │  DataContext binding (MVVM)
┌──────────────────────▼──────────────────────────────────────┐
│                    ViewModels                                 │
│   MainViewModel │ SettingsViewModel │ Converters             │
└────────┬────────────────────┬────────────────────┬──────────┘
         │                    │                    │
┌────────▼──────┐  ┌──────────▼──────┐  ┌─────────▼──────────┐
│ ScriptEngine  │  │ SettingsService │  │  LoggingService     │
│ Service       │  │ (JSON persist)  │  │  (daily log file)   │
└────────┬──────┘  └─────────────────┘  └────────────────────┘
         │
┌────────▼─────────────────────────────────────────┐
│              Resources/Scripts.json               │
│    84 scripts · 18 categories · fully editable   │
└──────────────────────────────────────────────────┘
```

---

## License

MIT — free for personal and commercial IT use.
All scripts should be tested in a non-production environment before deployment.
