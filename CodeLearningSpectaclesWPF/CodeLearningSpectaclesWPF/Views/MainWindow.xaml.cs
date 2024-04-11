using CodeLearningSpectaclesWPF.Models;
using CodeLearningSpectaclesWPF.Models.DTO;
using CodeLearningSpectaclesWPF.Views;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
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
            BaseAddress = new Uri("http://API-env.eba-8bvi8xmn.eu-west-1.elasticbeanstalk.com/api/v1/")
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
            btnFavourites.Foreground = (Brush)bc.ConvertFrom("#000");
            btnFavourites.BorderThickness = new Thickness(1);

            btnPython.BorderBrush = (Brush)bc.ConvertFrom("#FF707070");
            btnPython.Background = (Brush)bc.ConvertFrom("#FFF");
            btnPython.Foreground = (Brush)bc.ConvertFrom("#000");
            btnPython.BorderThickness = new Thickness(1);

            btnCsharp.BorderBrush = (Brush)bc.ConvertFrom("#FF707070");
            btnCsharp.Background = (Brush)bc.ConvertFrom("#FFF");
            btnCsharp.Foreground = (Brush)bc.ConvertFrom("#000");
            btnCsharp.BorderThickness = new Thickness(1);

            btnJavascript.BorderBrush = (Brush)bc.ConvertFrom("#FF707070");
            btnJavascript.Background = (Brush)bc.ConvertFrom("#FFF");
            btnJavascript.Foreground = (Brush)bc.ConvertFrom("#000");
            btnJavascript.BorderThickness = new Thickness(1);
        }

        private async void btnFavourites_Click(object sender, RoutedEventArgs e)
        {
            ResetSelections();
            btnFavourites.BorderBrush = (Brush)bc.ConvertFrom("#FFF");
            btnFavourites.Background = (Brush)bc.ConvertFrom("#FF8697AD");
            btnFavourites.Foreground = (Brush)bc.ConvertFrom("#FFF");
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("ACCESS_TOKEN"));
            var languageconstructs = await Client.GetFromJsonAsync<List<Languageconstruct>>("Languageconstructs");
            var profileLanguageconstructs = await Client.GetFromJsonAsync<List<Profilelanguageconstruct>>("Profilelanguageconstructs/getByProfile/" + Helpers.ProfileID);

            var languageConstructDTOs = languageconstructs
            .Where(languageConstruct => profileLanguageconstructs.Any(profileLanguageConstruct => profileLanguageConstruct.Languageconstructid == languageConstruct.Languageconstructid))
            .Select(languageConstruct => new LanguageConstructDTO
            {
                Languageconstructid = languageConstruct.Languageconstructid,
                Codinglanguage = Enum.GetName(typeof(CodingLanguagesEnum), languageConstruct.Codinglanguageid),
                Codeconstruct = Enum.GetName(typeof(ConstructEnum), languageConstruct.Codeconstructid),
                Construct = languageConstruct.Construct
            })
            .ToList();

            DataFrame.Content = new FavouritesPage(languageConstructDTOs, profileLanguageconstructs);
        }

        private async void btnPython_Click(object sender, RoutedEventArgs e)
        {
            ResetSelections();
            btnPython.BorderBrush = (Brush)bc.ConvertFrom("#FFF");
            btnPython.Background = (Brush)bc.ConvertFrom("#FF8697AD");
            btnPython.Foreground = (Brush)bc.ConvertFrom("#FFF");

            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("ACCESS_TOKEN"));
            var languageId = (int)CodingLanguagesEnum.Python;
            var languageconstructs = await Client.GetFromJsonAsync<List<Languageconstruct>>("Languageconstructs/getByLanguage/" + languageId.ToString());
            var profileLanguageconstructs = await Client.GetFromJsonAsync<List<Profilelanguageconstruct>>("Profilelanguageconstructs/getByProfile/" + Helpers.ProfileID);


            var languageConstructDTOs = new List<LanguageConstructDTO>();

            foreach (var languageConstruct in languageconstructs)
            {
                var dto = new LanguageConstructDTO
                {
                    Languageconstructid = languageConstruct.Languageconstructid,
                    Codinglanguage = Enum.GetName(typeof(CodingLanguagesEnum), languageConstruct.Codinglanguageid),
                    Codeconstruct = Enum.GetName(typeof(ConstructEnum), languageConstruct.Codeconstructid),
                    Construct = languageConstruct.Construct
                };

                languageConstructDTOs.Add(dto);
            }


            DataFrame.Content = new GeneralPage(languageConstructDTOs, profileLanguageconstructs);
        }

        private async void btnCsharp_Click(object sender, RoutedEventArgs e)
        {
            ResetSelections();
            btnCsharp.BorderBrush = (Brush)bc.ConvertFrom("#FFF");
            btnCsharp.Background = (Brush)bc.ConvertFrom("#FF8697AD");
            btnCsharp.Foreground = (Brush)bc.ConvertFrom("#FFF");

            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("ACCESS_TOKEN"));
            var languageId = (int)CodingLanguagesEnum.CSharp;
            var languageconstructs = await Client.GetFromJsonAsync<List<Languageconstruct>>("Languageconstructs/getByLanguage/" + languageId.ToString());
            var profileLanguageconstructs = await Client.GetFromJsonAsync<List<Profilelanguageconstruct>>("Profilelanguageconstructs/getByProfile/" + Helpers.ProfileID);

            var languageConstructDTOs = new List<LanguageConstructDTO>();

            foreach (var languageConstruct in languageconstructs)
            {
                var dto = new LanguageConstructDTO
                {
                    Languageconstructid = languageConstruct.Languageconstructid,
                    Codinglanguage = Enum.GetName(typeof(CodingLanguagesEnum), languageConstruct.Codinglanguageid),
                    Codeconstruct = Enum.GetName(typeof(ConstructEnum), languageConstruct.Codeconstructid),
                    Construct = languageConstruct.Construct
                };

                languageConstructDTOs.Add(dto);
            }


            DataFrame.Content = new GeneralPage(languageConstructDTOs, profileLanguageconstructs);
        }

        private async void btnJavascript_Click(object sender, RoutedEventArgs e)
        {
            ResetSelections();
            btnJavascript.BorderBrush = (Brush)bc.ConvertFrom("#FFF");
            btnJavascript.Background = (Brush)bc.ConvertFrom("#FF8697AD");
            btnJavascript.Foreground = (Brush)bc.ConvertFrom("#FFF");

            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("ACCESS_TOKEN"));
            var languageId = (int)CodingLanguagesEnum.JavaScript;
            var languageconstructs = await Client.GetFromJsonAsync<List<Languageconstruct>>("Languageconstructs/getByLanguage/" + languageId.ToString());
            var profileLanguageconstructs = await Client.GetFromJsonAsync<List<Profilelanguageconstruct>>("Profilelanguageconstructs/getByProfile/" + Helpers.ProfileID);


            var languageConstructDTOs = new List<LanguageConstructDTO>();

            foreach (var languageConstruct in languageconstructs)
            {
                var dto = new LanguageConstructDTO
                {
                    Languageconstructid = languageConstruct.Languageconstructid,
                    Codinglanguage = Enum.GetName(typeof(CodingLanguagesEnum), languageConstruct.Codinglanguageid),
                    Codeconstruct = Enum.GetName(typeof(ConstructEnum), languageConstruct.Codeconstructid),
                    Construct = languageConstruct.Construct
                };

                languageConstructDTOs.Add(dto);
            }


            DataFrame.Content = new GeneralPage(languageConstructDTOs, profileLanguageconstructs);
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
