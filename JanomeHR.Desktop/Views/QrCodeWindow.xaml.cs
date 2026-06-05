using JanomeHR.Desktop.Localization;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace JanomeHR.Desktop.Views;

public partial class QrCodeWindow : Window
{
    private readonly string _qrBase64;

    public QrCodeWindow(string qrBase64)
    {
        InitializeComponent();
        _qrBase64 = qrBase64;
        LoadQrImage();
    }

    private void LoadQrImage()
    {
        try
        {
            var base64 = _qrBase64.Replace("data:image/png;base64,", "");
            var bytes  = Convert.FromBase64String(base64);
            using var ms = new MemoryStream(bytes);
            var img = new BitmapImage();
            img.BeginInit();
            img.CacheOption  = BitmapCacheOption.OnLoad;
            img.StreamSource = ms;
            img.EndInit();
            ImgQr.Source = img;
        }
        catch (Exception ex)
        {
            MessageBox.Show(Loc.F("Msg_QrLoadFailed", ex.Message));
        }
    }

    private void BtnPrint_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var base64   = _qrBase64.Replace("data:image/png;base64,", "");
            var bytes    = Convert.FromBase64String(base64);
            var savePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                $"JanomeHR_QRCode_{DateTime.Now:yyyyMMdd_HHmmss}.png");

            File.WriteAllBytes(savePath, bytes);
            MessageBox.Show(
                Loc.F("Msg_QrSaved", Environment.NewLine, savePath),
                Loc.T("Msg_SuccessTitle"),
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(Loc.F("Msg_QrSaveFailed", ex.Message));
        }
    }
}
