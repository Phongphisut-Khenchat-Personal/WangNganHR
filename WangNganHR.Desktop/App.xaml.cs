using WangNganHR.Desktop.Localization;
using WangNganHR.Desktop.Services;
using WangNganHR.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace WangNganHR.Desktop;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        DispatcherUnhandledException += (_, args) =>
        {
            MessageBox.Show(args.Exception.Message, "Wang Ngan HR", MessageBoxButton.OK, MessageBoxImage.Error);
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