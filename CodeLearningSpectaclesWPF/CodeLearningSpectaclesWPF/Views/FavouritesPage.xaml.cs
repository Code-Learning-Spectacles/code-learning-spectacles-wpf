using CodeLearningSpectaclesWPF.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
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
  /// <summary>
  /// Interaction logic for FavouritesPage.xaml
  /// </summary>
  public partial class FavouritesPage : Page
  {
    private List<LanguageConstructDTO> languageConstructDTOs;
    public FavouritesPage(List<LanguageConstructDTO> languageconstructDTOs)
    {
      this.languageConstructDTOs = languageconstructDTOs;
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

        Button favouriteButton = new Button();
        favouriteButton.Content = "Remove from favourites";
        favouriteButton.Margin = new Thickness(5);
        favouriteButton.Background = Brushes.White;
        favouriteButton.Click += (sender, e) =>
        {
          //TODO: When click do api call to remove this language construct and note from user faves

        };

        Border horizontalLine = new Border();
        horizontalLine.BorderThickness = new Thickness(0, 1, 0, 0);
        horizontalLine.BorderBrush = Brushes.Black;
        horizontalLine.Margin = new Thickness(15);


        Label noteLabel = new Label();
        noteLabel.Content = "Add Note:";
        noteLabel.FontSize = 14;

        TextBox inputTextBox = new TextBox();
        inputTextBox.Margin = new Thickness(5);

        stackPanel.Children.Add(codeLabel);
        stackPanel.Children.Add(constructTextBlock);
        stackPanel.Children.Add(horizontalLine);
        stackPanel.Children.Add(noteLabel);
        stackPanel.Children.Add(inputTextBox);
        stackPanel.Children.Add(favouriteButton);
        stackPanel.Background = Brushes.White;

        Border border = new Border();
        border.BorderBrush = Brushes.Black;
        border.Margin = new Thickness(5);

        border.BorderThickness = new Thickness(1);
        border.Padding = new Thickness(5);
        border.Child = stackPanel;

        FavHeading.Content = dto.Codinglanguage;
        FavWrapPanel.Children.Add(border);


      }
    }
  }
}
