using DictionaryService;
using DictionaryService.Models;
using LightDictionary.Models;
using LightDictionary.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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

        public DictItem LocalResult { get; set; }

        public DictionaryPage()
        {
            this.InitializeComponent();

            var changed =
                Observable.FromEventPattern<
                    TypedEventHandler<AutoSuggestBox, AutoSuggestBoxTextChangedEventArgs>,
                    AutoSuggestBox,
                    AutoSuggestBoxTextChangedEventArgs>(
                    handler => DictionarySearchBox.TextChanged += handler,
                    handler => DictionarySearchBox.TextChanged -= handler);

            var input = changed
                .DistinctUntilChanged(x => x.Sender.Text)
                .Throttle(TimeSpan.FromMilliseconds(300));

            var notUserInput = input
                .ObserveOnDispatcher(Windows.UI.Core.CoreDispatcherPriority.Normal)
                .Where(x => x.EventArgs.Reason != AutoSuggestionBoxTextChangeReason.UserInput)
                .Select(x => Task.FromResult<List<SuggestionItem>>(null));

            var userInput = input
                .ObserveOnDispatcher(Windows.UI.Core.CoreDispatcherPriority.Normal)
                .Where(x => x.EventArgs.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
                .Where(x => !string.IsNullOrEmpty(x.Sender.Text))
                .Select(x => WordSuggestion.GetSuggestionsAsync(x.Sender.Text, 8));

            var merge = Observable
                .Merge(notUserInput, userInput)
                .Switch();

            merge
                .ObserveOnDispatcher(Windows.UI.Core.CoreDispatcherPriority.Normal)
                .Subscribe(suggestions =>
                {
                    if (suggestions == null) return;
                    if (suggestions.Count == 0)
                    {
                        suggestions.Add(new SuggestionItem() { Word = "No results found" });
                    }
                    DictionarySearchBox.ItemsSource = suggestions;
                });
        }

        // Handle user selecting an item, in our case just output the selected item.
        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var item = args.SelectedItem as SuggestionItem;
            sender.Text = item.Word;
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (string.IsNullOrEmpty(SearchText.Trim())) return;
            var param = new DictionaryNavParam()
            {
                SearchText = SearchText
            };
            _ = MainPage.MainPageFrame.Navigate(typeof(DictionaryPage), param);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is DictionaryNavParam param)
            {
                SearchText = param.SearchText;
                HasSearchResult = true;
                VisualStateManager.GoToState(this, HasSearchResultState.Name, false);
                var result = await Constants.BingLocalDictionaryService.SearchAsync(SearchText);
                LocalResult = result;
            }
            base.OnNavigatedTo(e);
        }

    }
}
