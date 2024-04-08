using CodeLearningSpectaclesWPF.Views;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using System.Windows.Media;

namespace CodeLearningSpectaclesWPF
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {

    private static HttpClient Client = new HttpClient()
    {
      BaseAddress = new Uri("https://localhost:7107/api/v1")
    };

    public static BrushConverter bc = new();

    public MainWindow()
    {
      InitializeComponent();
      lblWelcome.Text = "Hi " + Helpers.Profile.login;
    }

    public void ResetSelections()
    {
      //Ensure only the selected page is highlighted when switching to a new page
      btnFavourites.BorderBrush = (Brush)bc.ConvertFrom("#FF707070");
      btnFavourites.Background = (Brush)bc.ConvertFrom("#FFF");
      btnFavourites.BorderThickness = new Thickness(1);

      btnPython.BorderBrush = (Brush)bc.ConvertFrom("#FF707070");
      btnPython.Background = (Brush)bc.ConvertFrom("#FFF");
      btnPython.BorderThickness = new Thickness(1);

      btnTemp1.BorderBrush = (Brush)bc.ConvertFrom("#FF707070");
      btnTemp1.Background = (Brush)bc.ConvertFrom("#FFF");
      btnTemp1.BorderThickness = new Thickness(1);

      btnTemp2.BorderBrush = (Brush)bc.ConvertFrom("#FF707070");
      btnTemp2.Background = (Brush)bc.ConvertFrom("#FFF");
      btnTemp2.BorderThickness = new Thickness(1);

      btnTemp3.BorderBrush = (Brush)bc.ConvertFrom("#FF707070");
      btnTemp3.Background = (Brush)bc.ConvertFrom("#FFF");
      btnTemp3.BorderThickness = new Thickness(1);
    }

    private void btnFavourites_Click(object sender, RoutedEventArgs e)
    {
      ResetSelections();
      btnFavourites.BorderBrush = (Brush)bc.ConvertFrom("#FF84A8FF");
      btnFavourites.Background = (Brush)bc.ConvertFrom("#FF84A8FF");
      btnFavourites.BorderThickness = new Thickness(2);
      DataFrame.Content = new FavouritesPage();
    }

    private void btnPython_Click(object sender, RoutedEventArgs e)
    {
      ResetSelections();
      btnPython.BorderBrush = (Brush)bc.ConvertFrom("#FF84A8FF");
      btnPython.Background = (Brush)bc.ConvertFrom("#FF84A8FF");
      btnPython.BorderThickness = new Thickness(2);
      DataFrame.Content = new GeneralPage();
    }

    private async void btnTemp_Click(object sender, RoutedEventArgs e)
    {
      ResetSelections();
      btnTemp1.BorderBrush = (Brush)bc.ConvertFrom("#FF84A8FF");
      btnTemp1.Background = (Brush)bc.ConvertFrom("#FF84A8FF");
      btnTemp1.BorderThickness = new Thickness(2);
      DataFrame.Content = new GeneralPage();
      try
      {
        Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Environment.GetEnvironmentVariable("ACCESS_TOKEN"));
        using HttpResponseMessage response = await Client.GetAsync("Codeconstructs");

        string content = await response.Content.ReadAsStringAsync();
        MessageBox.Show(content);
      }
      catch (HttpRequestException ex)
      {
        MessageBox.Show(ex.Message, "Error getting data", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    private void btnExit_Click(object sender, RoutedEventArgs e)
    {
      ResetSelections();
      DataFrame.Content = null;
      LoginWindow window = new LoginWindow();
      window.Show();
      this.Close();
    }
  }
}
