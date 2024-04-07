using CodeLearningSpectaclesWPF.Models;
using CodeLearningSpectaclesWPF.ViewModels;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading.Tasks;

namespace CodeLearningSpectaclesWPF
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {

    public static string ACCESS_TOKEN = "";
    public static bool AUTHENTICATED = false;

    private DeviceVerification? deviceVerification;
    private MainViewModel ViewModel {  get; set; }

    public MainWindow()
    {
      InitializeComponent();
      ViewModel = new MainViewModel();
      DataContext = ViewModel;
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
      using var client = new HttpClient();
      string response = await AuthenticateAsync(client);
      deviceVerification = JsonSerializer.Deserialize<DeviceVerification>(response);
      if (deviceVerification != null)
      {
        ViewModel.MessageStart = "Please go to";
        ViewModel.UrlMessage = deviceVerification.verification_uri;
        //hypVerification.NavigateUri = new Uri(viewModel.UrlMessage);
        ViewModel.MessageEnd = "and enter the code: " + deviceVerification.user_code;
        btnCopyCode.Content = "Copy Code";
        bool success = false;
        txbStatus.Text = "\nWaiting for authentication...";
        while (!success)
        {
          success = await GetAccessTokenAsync(client, deviceVerification);
        }
        txbStatus.Text += "\nSuccessfully authenticated! Access token: " + ACCESS_TOKEN;
      }
      else
      {
        txbStatus.Text += "Authentication failed.";
      }
      
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
      Process.Start(new ProcessStartInfo
      {
        FileName = e.Uri.AbsoluteUri,
        UseShellExecute = true
      });
      e.Handled = true;
    }

    static async Task<string> AuthenticateAsync(HttpClient client)
    {
      string url = "https://github.com/login/device/code?client_id=" + Environment.GetEnvironmentVariable("CLIENT_ID") + "&scope=read:user";
      HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, url);
      msg.Headers.Add("Accept", "application/json");
      HttpResponseMessage response = await client.SendAsync(msg);

      return await response.Content.ReadAsStringAsync();
    }

    static async Task<bool> GetAccessTokenAsync(HttpClient client, DeviceVerification deviceVerification)
    {
      await Task.Delay(5000);
      string url = "https://github.com/login/oauth/access_token?client_id=" + Environment.GetEnvironmentVariable("CLIENT_ID") + "&device_code=" + deviceVerification.device_code + "&grant_type=urn:ietf:params:oauth:grant-type:device_code";
      HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, url);
      msg.Headers.Add("Accept", "application/json");
      HttpResponseMessage response = await client.SendAsync(msg);

      string content = await response.Content.ReadAsStringAsync();
      AccessToken? accessToken = JsonSerializer.Deserialize<AccessToken>(content);
      if (accessToken != null && accessToken.access_token != string.Empty)
      {
        ACCESS_TOKEN = accessToken.access_token;
        AUTHENTICATED = true;
        return true;
      }
      return false;
    }

    private void CopyCode_Click(object sender, RoutedEventArgs e)
    {
      if (deviceVerification != null)
      {
        Clipboard.SetText(deviceVerification.user_code);
        btnCopyCode.Content = "Copied!";
      }
    }

    private async void SendRequest_Click(object sender, RoutedEventArgs e)
    {
      using var client = new HttpClient();
      string url = "https://localhost:7107/api/Codeconstructs";
      HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Get, url);
      msg.Headers.Add("Authorization", "Bearer " + ACCESS_TOKEN);
      HttpResponseMessage response = await client.SendAsync(msg);

      string content = await response.Content.ReadAsStringAsync();
      MessageBox.Show(content);
    }
  }
}
