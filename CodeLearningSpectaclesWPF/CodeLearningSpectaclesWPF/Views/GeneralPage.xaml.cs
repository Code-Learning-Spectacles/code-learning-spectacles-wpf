using CodeLearningSpectaclesWPF.Models;
using CodeLearningSpectaclesWPF.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
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

namespace CodeLearningSpectaclesWPF.Views
{
   

    public partial class GeneralPage : Page
    {

        private List<LanguageConstructDTO> languageConstructDTOs;
        private List<Profilelanguageconstruct> profileLanguageconstructs;

        private static HttpClient Client = new HttpClient()
        {
            BaseAddress = new Uri("https://localhost:7107/api/v1/")
        };

        public GeneralPage(List<LanguageConstructDTO> languageconstructDTOs, List<Profilelanguageconstruct> profileLanguageconstructs)
        {
            this.languageConstructDTOs = languageconstructDTOs;
            this.profileLanguageconstructs = profileLanguageconstructs;

            InitializeComponent();

            foreach (var dto in languageConstructDTOs)
            {
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

                Button favouriteButton = new Button();
                favouriteButton.Content = !profileLanguageconstructs.Any(plc => plc.Languageconstructid == dto.Languageconstructid) ? "Add to Favourites" : "Saved";
                favouriteButton.Margin = new Thickness(5);
                favouriteButton.IsEnabled= !profileLanguageconstructs.Any(plc => plc.Languageconstructid == dto.Languageconstructid);
                favouriteButton.Background = Brushes.White;
                favouriteButton.Click += async (sender, e) =>
                {
                    Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("ACCESS_TOKEN"));
                  
                    Profilelanguageconstruct profileLanguageConstruct = new Profilelanguageconstruct
                    {
                        
                        Languageconstructid = dto.Languageconstructid,
                        Profileid = Convert.ToInt32(Helpers.ProfileID),
                        //Notes = inputTextBox.Text
                    };

                    try
                    {
                        var response = await Client.PostAsJsonAsync("Profilelanguageconstructs", profileLanguageConstruct);

                        if (response.IsSuccessStatusCode)
                        {
                            //inputTextBox.IsEnabled = false;
                            favouriteButton.Content = "Saved";
                            favouriteButton.IsEnabled = false;
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("There was an error saving your selection. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                };

                StackPanel buttonsPanel = new StackPanel();
                buttonsPanel.Orientation = Orientation.Horizontal;
                buttonsPanel.Children.Add(favouriteButton);
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
}
