using CodeLearningSpectaclesWPF.Models;
using CodeLearningSpectaclesWPF.Models.DTO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Text.RegularExpressions;

namespace CodeLearningSpectaclesWPF.Views
{


    public partial class GeneralPage : Page
    {

        private List<LanguageConstructDTO> languageConstructDTOs;
        private List<Profilelanguageconstruct> profileLanguageconstructs;
        private readonly Dictionary<string, Button> buttons = new Dictionary<string, Button>();

        private static HttpClient Client = new HttpClient()
        {
            BaseAddress = new Uri("http://API-env.eba-8bvi8xmn.eu-west-1.elasticbeanstalk.com/api/v1/")
        };

        public GeneralPage(List<LanguageConstructDTO> languageconstructDTOs, List<Profilelanguageconstruct> profileLanguageconstructs)
        {
            this.languageConstructDTOs = languageconstructDTOs;
            this.profileLanguageconstructs = profileLanguageconstructs;

            InitializeComponent();
            LoadData();
        }

        private void LoadData(string search = "")
        {
            foreach (var dto in languageConstructDTOs)
            {
                if (search.Equals("") || dto.Codeconstruct.Contains(search, StringComparison.OrdinalIgnoreCase))
                {
                    StackPanel stackPanel = new StackPanel();
                    StackPanel buttonsPanel = new StackPanel();

                    Label codeLabel = new Label();
                    codeLabel.Content = Regex.Replace(dto.Codeconstruct, "(?<=.)([A-Z])", " $1");
                    codeLabel.FontSize = 20;
                    codeLabel.FontWeight = FontWeights.Bold;

                    TextBlock constructTextBlock = new TextBlock();
                    constructTextBlock.Text = dto.Construct;
                    constructTextBlock.FontSize = 16;
                    constructTextBlock.Padding = new Thickness(5, 3, 5, 3);
                    constructTextBlock.TextWrapping = TextWrapping.Wrap;

                    Border horizontalLine = new Border();
                    horizontalLine.BorderThickness = new Thickness(0, 1, 0, 0);
                    horizontalLine.BorderBrush = Brushes.Black;
                    horizontalLine.Margin = new Thickness(15);

                    Button copyButton = new Button();
                    copyButton.Content = "Copy Code";
                    copyButton.Margin = new Thickness(5);
                    copyButton.Background = Brushes.White;
                    copyButton.Click += (sender, e) =>
                    {
                        string textToCopy = constructTextBlock.Text;
                        Clipboard.SetText(textToCopy);
                        MessageBox.Show("Copied to clipboard!", "Copied", MessageBoxButton.OK, MessageBoxImage.Information);
                    };

                    var alreadySaved = profileLanguageconstructs.Any(plc => plc.Languageconstructid == dto.Languageconstructid);

                    Button favouriteButton = new Button();
                    var favButtonId = "favouriteButton" + dto.Languageconstructid.ToString();

                    favouriteButton.Content = !alreadySaved ? "Add to Favourites" : "Saved";
                    favouriteButton.Margin = new Thickness(5);
                    favouriteButton.Padding = new Thickness(3);
                    favouriteButton.IsEnabled = !alreadySaved;
                    favouriteButton.Background = Brushes.White;
                    favouriteButton.Click += async (sender, e) =>
                    {
                        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("ACCESS_TOKEN"));

                        Profilelanguageconstruct profileLanguageConstruct = new Profilelanguageconstruct
                        {

                            Languageconstructid = dto.Languageconstructid,
                            Profileid = Convert.ToInt32(Helpers.ProfileID),
                        };

                        try
                        {
                            var response = await Client.PostAsJsonAsync("Profilelanguageconstructs", profileLanguageConstruct);
                            if (response.IsSuccessStatusCode)
                            {
                                MessageBox.Show("Construct successfully saved!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                                favouriteButton.Content = "Saved";
                                favouriteButton.IsEnabled = false;

                                var buttonId = "favouriteNoteButton" + dto.Languageconstructid.ToString();
                                Button favNoteButton = buttons[buttonId];
                                favNoteButton.IsEnabled = false;
                                favNoteButton.Content = "Saved";
                                favNoteButton.Visibility = Visibility.Hidden;

                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("There was an error saving your selection. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                    };
                    buttons.Add(favButtonId, favouriteButton);

                    Button favouriteNoteButton = new Button();
                    var favNoteButtonId = "favouriteNoteButton" + dto.Languageconstructid.ToString();
                    favouriteNoteButton.Name = favNoteButtonId;
                    favouriteNoteButton.Content = !alreadySaved ? "Save with Note" : "Saved";
                    favouriteNoteButton.Margin = new Thickness(5);
                    favouriteNoteButton.Padding = new Thickness(3);
                    favouriteNoteButton.Visibility = !alreadySaved ? Visibility.Visible : Visibility.Hidden;
                    favouriteNoteButton.IsEnabled = !alreadySaved;
                    favouriteNoteButton.Background = Brushes.White;
                    favouriteNoteButton.Click += async (sender, e) =>
                    {
                        inputLabel.Content = dto.Languageconstructid;
                        popup.IsOpen = true;

                    };
                    buttons.Add(favNoteButtonId, favouriteNoteButton);


                    buttonsPanel.Orientation = Orientation.Horizontal;
                    buttonsPanel.Children.Add(favouriteButton);
                    buttonsPanel.Children.Add(favouriteNoteButton);
                    buttonsPanel.Children.Add(copyButton);

                    stackPanel.Children.Add(codeLabel);
                    stackPanel.Children.Add(constructTextBlock);
                    stackPanel.Children.Add(horizontalLine);
                    stackPanel.Children.Add(buttonsPanel);
                    stackPanel.Background = Brushes.White;

                    Border border = new Border();
                    border.BorderBrush = Brushes.Black;
                    border.Margin = new Thickness(5);

                    border.BorderThickness = new Thickness(1);
                    border.Padding = new Thickness(5);
                    border.Child = stackPanel;

                    Heading.Content = dto.Codinglanguage == "CSharp" ? "C#" : dto.Codinglanguage;
                    MainWrapPanel.Children.Add(border);
                }
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Heading.Content = "";
            MainWrapPanel.Children.Clear();
            buttons.Clear();
            LoadData(txtSearch.Text);
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string userInput;
            userInput = txtBox.Text;

            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("ACCESS_TOKEN"));

            Profilelanguageconstruct profileLanguageConstruct = new Profilelanguageconstruct
            {

                Languageconstructid = (int)inputLabel.Content,
                Profileid = Convert.ToInt32(Helpers.ProfileID),
                Notes = userInput
            };

            try
            {
                var response = await Client.PostAsJsonAsync("Profilelanguageconstructs", profileLanguageConstruct);

                if (response.IsSuccessStatusCode)
                {
                    popup.IsOpen = false;
                    MessageBox.Show("Construct successfully saved!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    var favButtonId = "favouriteButton" + inputLabel.Content;
                    var favNoteButtonId = "favouriteNoteButton" + inputLabel.Content;

                    Button favButton = buttons[favButtonId];
                    favButton.IsEnabled = false;
                    favButton.Content = "Saved";

                    Button favNoteButton = buttons[favNoteButtonId];
                    favNoteButton.IsEnabled = false;
                    favNoteButton.Visibility = Visibility.Hidden;
                    favNoteButton.Content = "Saved";
                    favNoteButton.Visibility = Visibility.Hidden;

                    txtBox.Text = "";

                }

            }
            catch (Exception ex)
            {
                popup.IsOpen = false;
                MessageBox.Show("There was an error saving your selection. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            popup.IsOpen = false;
            txtBox.Text = "";
        }
    }
}

