using WangNganHR.Desktop.Localization;
using WangNganHR.Desktop.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Input;

namespace WangNganHR.Desktop.Views;

public partial class LoginWindow : Window
{
    private readonly ApiService _api;

    public LoginWindow(ApiService api)
    {
        InitializeComponent();
        _api = api;
    }

    private void BtnSettings_Click(object sender, RoutedEventArgs e) =>
        LanguageMenuHelper.Open(BtnSettings, LanguageMenu);

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            _ = LoginAsync();
    }

    private async void BtnLogin_Click(object sender, RoutedEventArgs e) =>
        await LoginAsync();

    private async Task LoginAsync()
    {
        TxtError.Visibility = Visibility.Collapsed;
        var username = TxtUsername.Text.Trim();
        var password = TxtPassword.Password;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ShowError(Loc.T("Login_ErrorEmpty"));
            return;
        }

        BtnLogin.IsEnabled = false;

        try
        {
            var result = await _api.LoginAsync(username, password);
            if (result is null)
            {
                ShowError(Loc.T("Login_ErrorFailed"));
                return;
            }

            _api.SetToken(result.Token);

            var main = App.Services.GetRequiredService<MainWindow>();
            main.Show();
            Close();
        }
        catch
        {
            ShowError(Loc.T("Login_ErrorApi"));
        }
        finally
        {
            BtnLogin.IsEnabled = true;
        }
    }

    private void ShowError(string message)
    {
        TxtError.Text = message;
        TxtError.Visibility = Visibility.Visible;
    }
}
