using CodeLearningSpectaclesWPF.Models;
using CodeLearningSpectaclesWPF.ViewModels;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
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
using System.Xml.Linq;

namespace CodeLearningSpectaclesWPF
{
  /// <summary>
  /// Interaction logic for LoginWindow.xaml
  /// </summary>
  public partial class LoginWindow : Window
  {

    private static HttpClient Client = new HttpClient()
    {
      BaseAddress = new Uri("https://localhost:7107/api/v1/")
    };

    private DeviceVerification? deviceVerification;
    private LoginViewModel ViewModel { get; set; }

    public LoginWindow()
    {
      InitializeComponent();
      ViewModel = new LoginViewModel();
      DataContext = ViewModel;
    }

    private async void Login_Click(object sender, RoutedEventArgs e)
    {
      if (Environment.GetEnvironmentVariable("ACCESS_TOKEN") != null)
      {
        MessageBox.Show("Login Success");
        MainWindow window = new MainWindow();
        window.Show();
        this.Close();
      }
      else
      {
        string response = await AuthenticateAsync();
        deviceVerification = JsonSerializer.Deserialize<DeviceVerification>(response);
        if (deviceVerification != null)
        {
          btnCopyCode.Visibility = Visibility.Visible;
          ViewModel.MessageStart = "Please go to";
          ViewModel.UrlMessage = deviceVerification.verification_uri;
          ViewModel.MessageEnd = "and enter the code: " + deviceVerification.user_code;
          btnCopyCode.Content = "Copy Code";
          bool success = false;
          btnLogin.IsEnabled = false;
          btnLogin.Content = "Waiting for authentication...";
          while (!success)
          {
            success = await Helpers.GetAccessTokenAsync(Client, deviceVerification);
          }
          Helpers.Profile = await GetProfileAsync();
          MessageBox.Show("Login Success");
          MainWindow window = new MainWindow();
          window.Show();
          this.Close();
        }
        else
        {
          MessageBox.Show("Login Success");
        }
      }
    }

    private async void CheckProfile(string name)
    {
      try
      {
        Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Environment.GetEnvironmentVariable("ACCESS_TOKEN"));
        Client.DefaultRequestHeaders.Accept.Clear();
        Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var profiles = await Client.GetFromJsonAsync<List<Profile>>("Profiles");
        if (profiles != null && profiles.Count > 0)
        {
          var profile = profiles.Find(x => x.Name.Equals(name));
          if (profile == null)
          {
            // Create profile for user
            using StringContent jsonContent = new(JsonSerializer.Serialize(new { Name = name }), Encoding.UTF8, "application/json");
            using HttpResponseMessage createProfileResponse = await Client.PostAsync("Profiles", jsonContent);
            Profile? newProfile = await createProfileResponse.Content.ReadFromJsonAsync<Profile>();
            Helpers.ProfileID = ""+newProfile.Profileid;
          }
          else
          {
            //Store id for future use
            Helpers.ProfileID = ""+profile.Profileid;
          }
        }
      }
      catch (HttpRequestException ex)
      {
        MessageBox.Show(ex.Message, "Error Checking Profile", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    private async Task<AuthObject?> GetProfileAsync()
    {
      try
      {
        string url = "https://api.github.com/user";
        HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Get, url);
        msg.Headers.Add("User-Agent", "CodeLearningSpectaclesAPI");
        msg.Headers.Add("Authorization", "Bearer " + Environment.GetEnvironmentVariable("ACCESS_TOKEN"));
        HttpResponseMessage response = await Client.SendAsync(msg);

        if (response.IsSuccessStatusCode)
        {
          string content = await response.Content.ReadAsStringAsync();
          var auth = JsonSerializer.Deserialize<AuthObject>(content);
          CheckProfile(auth.login);
          return auth;
        }
      }
      catch (HttpRequestException ex)
      {
        MessageBox.Show(ex.Message, "Error getting data", MessageBoxButton.OK, MessageBoxImage.Error);
      }
      return null;
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

    static async Task<string> AuthenticateAsync()
    {
      string url = "https://github.com/login/device/code?client_id=" + Environment.GetEnvironmentVariable("CLIENT_ID") + "&scope=read:user";
      HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, url);
      msg.Headers.Add("Accept", "application/json");
      HttpResponseMessage response = await Client.SendAsync(msg);

      return await response.Content.ReadAsStringAsync();
    }

    private void CopyCode_Click(object sender, RoutedEventArgs e)
    {
      if (deviceVerification != null)
      {
        Clipboard.SetText(deviceVerification.user_code);
        btnCopyCode.Content = "Copied!";
      }
    }

    private void Logout_Click(object sender, RoutedEventArgs e)
    {
      Helpers.WriteToFile("");
      Environment.SetEnvironmentVariable("ACCESS_TOKEN", null);
      btnLogin.Visibility = Visibility.Visible;
      btnCopyCode.Visibility = Visibility.Hidden;
      btnLogin.Content = "Login with Github";
      lblLoggedIn.Content = "Not logged in.";
      ViewModel.MessageStart = "";
      ViewModel.UrlMessage = "";
      ViewModel.MessageEnd = "";

    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
      btnLogin.IsEnabled = true;
      // Check login status of user if not coming back from main window
      if (Environment.GetEnvironmentVariable("ACCESS_TOKEN") == null)
      {
        string token = Helpers.ReadFromFile();
        if (token != "")
        {
          Environment.SetEnvironmentVariable("ACCESS_TOKEN", token);
          Helpers.Profile = await GetProfileAsync();
          if (Helpers.Profile != null)
          {
            Environment.SetEnvironmentVariable("USERNAME", Helpers.Profile.login);
            lblLoggedIn.Content = "Logged in as: " + Helpers.Profile.login;
            btnLogin.Content = "Continue";
            btnLogin.Visibility = Visibility.Visible;
            btnLogout.Visibility = Visibility.Visible;
          }
          else
          {
            lblLoggedIn.Content = "Not logged in.";
            btnLogin.Visibility = Visibility.Visible;
          }
        }
        else
        {
          lblLoggedIn.Content = "Not logged in.";
          btnLogin.Visibility = Visibility.Visible;
        }
      }
      else
      {
        lblLoggedIn.Content = "Logged in as: " + Helpers.Profile.login;
        btnLogin.Content = "Continue";
        btnLogin.Visibility = Visibility.Visible;
        btnLogout.Visibility = Visibility.Visible;
      }
    }
  }
}