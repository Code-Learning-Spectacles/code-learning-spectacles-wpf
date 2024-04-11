using CodeLearningSpectaclesWPF.Models;
using CodeLearningSpectaclesWPF.Models.DTO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CodeLearningSpectaclesWPF.Views
{
    /// <summary>
    /// Interaction logic for FavouritesPage.xaml
    /// </summary>
    public partial class FavouritesPage : Page
    {
        private List<LanguageConstructDTO> languageConstructDTOs;
        private List<Profilelanguageconstruct> profileLanguageconstructs;

        private static HttpClient Client = new HttpClient()
        {
            BaseAddress = new Uri("http://API-env.eba-8bvi8xmn.eu-west-1.elasticbeanstalk.com/api/v1/")
        };

        public FavouritesPage(List<LanguageConstructDTO> languageconstructDTOs, List<Profilelanguageconstruct> profileLanguageconstructs)
        {
            this.languageConstructDTOs = languageconstructDTOs;
            this.profileLanguageconstructs = profileLanguageconstructs;
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("ACCESS_TOKEN"));
            InitializeComponent();
            LoadData();
        }

        private void LoadData(string search = "")
        {
            foreach (var dto in languageConstructDTOs)
            {
                if (search.Equals("") || dto.Codeconstruct.Contains(search, StringComparison.OrdinalIgnoreCase))
                {
                    var profileLanguageConstruct = profileLanguageconstructs.FirstOrDefault(plc => plc.Languageconstructid == dto.Languageconstructid);
                    StackPanel stackPanel = new StackPanel();

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

                    Label noteLabel = new Label();
                    noteLabel.Content = "Note:";
                    noteLabel.FontSize = 14;

                    Button editButton = new Button();
                    TextBox inputTextBox = new TextBox();
                    inputTextBox.Margin = new Thickness(5);
                    inputTextBox.TextWrapping = TextWrapping.Wrap;
                    inputTextBox.Text = profileLanguageConstruct == null ? "" : profileLanguageConstruct.Notes;
                    inputTextBox.TextChanged += (sender, e) =>
                    {
                        editButton.IsEnabled = true;
                    };


                    editButton.Content = "Edit Note";
                    editButton.IsEnabled = false;
                    editButton.Margin = new Thickness(5);
                    editButton.Padding = new Thickness(3);
                    editButton.Background = Brushes.White;
                    editButton.Click += async (sender, e) =>
                    {
                        //When click do api call to update note
                        if (profileLanguageConstruct != null)
                        {
                            using StringContent jsonContent = new(JsonSerializer.Serialize(new
                            {
                                Profilelanguageconstructid = profileLanguageConstruct.Profilelanguageconstructid,
                                Notes = inputTextBox.Text,
                                Profileid = Helpers.ProfileID,
                                Languageconstructid = dto.Languageconstructid
                            }), Encoding.UTF8, "application/json");
                            var response = await Client.PutAsync("Profilelanguageconstructs/" + profileLanguageConstruct.Profilelanguageconstructid, jsonContent);
                            if (!response.IsSuccessStatusCode)
                            {
                                MessageBox.Show("Failed to edit: " + response.ToString(), "Error editing", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            else
                            {
                                MessageBox.Show("Updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                                editButton.IsEnabled = false;
                            }
                        }
                    };



                    Button removeButton = new Button();
                    removeButton.HorizontalAlignment = HorizontalAlignment.Stretch;
                    removeButton.Content = "Remove from Favourites";
                    removeButton.Margin = new Thickness(5);
                    removeButton.Padding = new Thickness(3);
                    removeButton.BorderBrush = new SolidColorBrush(Colors.Red);
                    removeButton.Background = Brushes.White;
                    removeButton.Click += async (sender, e) =>
                    {
                        //When click do api call to remove this language construct and note from user faves
                        if (profileLanguageConstruct != null)
                        {
                            var response = await Client.DeleteAsync("Profilelanguageconstructs/" + profileLanguageConstruct.Profilelanguageconstructid);
                            if (response.IsSuccessStatusCode)
                            {
                                removeButton.Content = "Removed";
                                inputTextBox.IsEnabled = false;
                                removeButton.IsEnabled = false;
                                editButton.IsEnabled = false;
                            }
                            else
                            {
                                MessageBox.Show("Failed to edit: " + response.ToString(), "Error editing", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    };

                    Button copyButton = new Button();
                    copyButton.HorizontalAlignment = HorizontalAlignment.Stretch;
                    copyButton.Content = "Copy Code";
                    copyButton.Margin = new Thickness(5);
                    copyButton.Padding = new Thickness(3);

                    copyButton.Background = Brushes.White;
                    copyButton.Click += (sender, e) =>
                    {
                        string textToCopy = constructTextBlock.Text;
                        Clipboard.SetText(textToCopy);
                        MessageBox.Show("Copied to clipboard!", "Copied", MessageBoxButton.OK, MessageBoxImage.Information);
                    };



                    stackPanel.Children.Add(codeLabel);
                    stackPanel.Children.Add(constructTextBlock);
                    stackPanel.Children.Add(horizontalLine);
                    stackPanel.Children.Add(copyButton);
                    stackPanel.Children.Add(noteLabel);
                    stackPanel.Children.Add(inputTextBox);
                    stackPanel.Children.Add(editButton);
                    stackPanel.Children.Add(removeButton);


                    stackPanel.Background = Brushes.White;

                    Border border = new Border();
                    border.BorderBrush = Brushes.Black;
                    border.Margin = new Thickness(5);

                    border.BorderThickness = new Thickness(1);
                    border.Padding = new Thickness(5);
                    border.Child = stackPanel;

                    switch (dto.Codinglanguage)
                    {
                        case "Python":
                            PythonHeading.Content = dto.Codinglanguage;
                            PythonWrapPanel.Children.Add(border);
                            break;
                        case "CSharp":
                            CsharpHeading.Content = "C#";
                            CsharpWrapPanel.Children.Add(border);
                            break;
                        case "JavaScript":
                            JavascriptHeading.Content = dto.Codinglanguage;
                            JavascriptWrapPanel.Children.Add(border);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            PythonHeading.Content = "";
            PythonWrapPanel.Children.Clear();
            CsharpHeading.Content = "";
            CsharpWrapPanel.Children.Clear();
            JavascriptHeading.Content = "";
            JavascriptWrapPanel.Children.Clear();
            LoadData(txtSearch.Text);
        }
    }
}
