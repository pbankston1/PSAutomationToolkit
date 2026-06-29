using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using PSAutomationToolkit.Services;
using PSAutomationToolkit.ViewModels;
using PSAutomationToolkit.Views;

namespace PSAutomationToolkit
{
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            // Services (singletons)
            services.AddSingleton<LoggingService>();
            services.AddSingleton<SettingsService>();
            services.AddSingleton<ScriptEngineService>();

            // ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<SettingsViewModel>();

            // Views
            services.AddTransient<MainWindow>();
            services.AddTransient<SettingsWindow>();

            _serviceProvider = services.BuildServiceProvider();

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }

        public static T GetService<T>() where T : class
        {
            var app = (App)Current;
            return app._serviceProvider!.GetRequiredService<T>();
        }
    }
}
