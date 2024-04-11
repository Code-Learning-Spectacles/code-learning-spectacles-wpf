using CodeLearningSpectaclesWPF.Models;
using CodeLearningSpectaclesWPF.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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
            BaseAddress = new Uri("https://localhost:7107/api/v1/")
        };

        public FavouritesPage(List<LanguageConstructDTO> languageconstructDTOs, List<Profilelanguageconstruct> profileLanguageconstructs)
        {
            this.languageConstructDTOs = languageconstructDTOs;
            this.profileLanguageconstructs = profileLanguageconstructs;
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("ACCESS_TOKEN"));
            InitializeComponent();

            foreach (var dto in languageConstructDTOs)
            {
                var profileLanguageConstruct = profileLanguageconstructs.FirstOrDefault(plc => plc.Languageconstructid == dto.Languageconstructid);
                StackPanel stackPanel = new StackPanel();

                Label codeLabel = new Label();
                codeLabel.Content = dto.Codeconstruct;
                codeLabel.FontSize = 20;
                codeLabel.FontWeight = FontWeights.Bold;

                TextBlock constructTextBlock = new TextBlock();
                constructTextBlock.Text = dto.Construct;
                constructTextBlock.TextWrapping = TextWrapping.Wrap;

                Border horizontalLine = new Border();
                horizontalLine.BorderThickness = new Thickness(0, 1, 0, 0);
                horizontalLine.BorderBrush = Brushes.Black;
                horizontalLine.Margin = new Thickness(15);

                Label noteLabel = new Label();
                noteLabel.Content = "Note:";
                noteLabel.FontSize = 14;

                TextBox inputTextBox = new TextBox();
                inputTextBox.Margin = new Thickness(5);
                inputTextBox.TextWrapping = TextWrapping.Wrap;
                inputTextBox.Text = profileLanguageConstruct == null ? "" : profileLanguageConstruct.Notes;

                Button editButton = new Button();
                editButton.Content = "Edit";
                editButton.Margin = new Thickness(5);
                editButton.Background = Brushes.White;
                editButton.Click += async (sender, e) =>
                {
                    //When click do api call to update note
                    if (profileLanguageConstruct != null)
                    {
                        using StringContent jsonContent = new(JsonSerializer.Serialize(new { 
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
                            MessageBox.Show("Updated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                };

                Button favouriteButton = new Button();
                favouriteButton.Content = "Remove";
                favouriteButton.Margin = new Thickness(5);
                favouriteButton.Background = Brushes.White;
                favouriteButton.Click += async (sender, e) =>
                {
                    //When click do api call to remove this language construct and note from user faves
                    if (profileLanguageConstruct != null)
                    {
                        var response = await Client.DeleteAsync("Profilelanguageconstructs/" + profileLanguageConstruct.Profilelanguageconstructid);
                        if (response.IsSuccessStatusCode)
                        {
                            favouriteButton.Content = "Removed";
                            inputTextBox.IsEnabled = false;
                            favouriteButton.IsEnabled = false;
                            editButton.IsEnabled = false;
                        }
                        else
                        {
                            MessageBox.Show("Failed to edit: " + response.ToString(), "Error editing", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                };

                stackPanel.Children.Add(codeLabel);
                stackPanel.Children.Add(constructTextBlock);
                stackPanel.Children.Add(horizontalLine);
                stackPanel.Children.Add(noteLabel);
                stackPanel.Children.Add(inputTextBox);
                stackPanel.Children.Add(editButton);
                stackPanel.Children.Add(favouriteButton);
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
                    case "C#":
                        CsharpHeading.Content = dto.Codinglanguage;
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
}
