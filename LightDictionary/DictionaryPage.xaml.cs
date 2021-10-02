using DictionaryService;
using DictionaryService.Models;
using LightDictionary.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace LightDictionary
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DictionaryPage : Page
    {
        private bool HasSearchResult = false;

        public string SearchText { get; set; }

        public DictionaryPage()
        {
            this.InitializeComponent();
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput) return;
            var searchText = sender.Text.ToLower();
            var suitableItems = WordSuggestion.GetSuggestions(searchText)
                                              .ToList();
            if (suitableItems.Count == 0)
            {
                suitableItems.Add(new SuggestionItem() { Word = "No results found" });
            }
            sender.ItemsSource = suitableItems;
        }


        // Handle user selecting an item, in our case just output the selected item.
        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem is SuggestionItem item)
            {
                sender.Text = item.Word;
            }
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var param = new DictionaryNavParam()
            {
                SearchText = sender.Text
            };
            _ = MainPage.MainPageFrame.Navigate(typeof(DictionaryPage), param);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is DictionaryNavParam param)
            {
                if (string.IsNullOrEmpty(param.SearchText?.Trim())) return;
                SearchText = param.SearchText;
                HasSearchResult = true;
                VisualStateManager.GoToState(this, HasSearchResultState.Name, false);
                // TODO: Loading search result...
            }
            base.OnNavigatedTo(e);
        }

    }
}
