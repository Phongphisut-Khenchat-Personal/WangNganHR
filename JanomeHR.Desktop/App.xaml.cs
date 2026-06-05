using JanomeHR.Desktop.Localization;
using JanomeHR.Desktop.Services;
using JanomeHR.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace JanomeHR.Desktop;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        DispatcherUnhandledException += (_, args) =>
        {
            MessageBox.Show(args.Exception.Message, "Janome HR", MessageBoxButton.OK, MessageBoxImage.Error);
            args.Handled = true;
        };
        LocalizationService.Instance.Initialize("en");

        var collection = new ServiceCollection();
        collection.AddSingleton<ApiService>();
        collection.AddTransient<LoginWindow>();
        collection.AddTransient<MainWindow>();

        Services = collection.BuildServiceProvider();

        var login = Services.GetRequiredService<LoginWindow>();
        login.Show();
    }
}