﻿using DictionaryService;
using DictionaryService.Models;
using LightDictionary.Models;
using LightDictionary.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace LightDictionary
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DictionaryPage : Page
    {
        public bool HasSearchResult { get; private set; }

        public string SearchText { get; set; }

        public DictItem LocalResult { get; set; }

        public DictItem BingResult { get; set; }

        private AppSettings AppSettings = Constants.AppSettings;

        public DictionaryPage()
        {
            this.InitializeComponent();

            SearchHistoryList.ItemsSource = AppSettings.SearchHistoryItems;

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


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DictionarySearchBox.Focus(FocusState.Programmatic);
        }

        // Handle user selecting an item, in our case just output the selected item.
        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var item = args.SelectedItem as SuggestionItem;
            sender.Text = item.Word;
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (string.IsNullOrEmpty(SearchText?.Trim())) return;
            var param = new DictionaryNavParam()
            {
                SearchText = SearchText
            };
            SearchAction(param);
        }

        private static void SearchAction(DictionaryNavParam param)
        {
            _ = MainPage.MainPageFrame.Navigate(typeof(DictionaryPage), param);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is DictionaryNavParam param)
            {
                SearchText = param.SearchText;
                HasSearchResult = true;
                VisualStateManager.GoToState(this, DisplaySearchResultState.Name, false);

                var bingResult = Constants.BingOnlineDictionaryService.SearchAsync(SearchText);
                var localResult = Constants.BingLocalDictionaryService.SearchAsync(SearchText, AppSettings.EnableEnhancedDictionary);

                try
                {
                    LocalResult = await localResult;
                    VisualStateManager.GoToState(this, HasLocalSearchResultState.Name, false);
                }
                catch (Exception)
                {
                    // TODO: 捕获异常
                    throw;
                }

                AppSettings.AddSearchHistoryItem(new HistoryItem()
                {
                    Word = SearchText,
                    Chinese = string.Join("\n", LocalResult?.ChineseDefinitions.Select(x => x.Definition))
                });

                try
                {
                    BingResult = await bingResult;
                    VisualStateManager.GoToState(this, HasBingSearchResultState.Name, false);
                }
                catch (Exception)
                {
                    // TODO: 捕获异常
                    throw;
                }
                this.Bindings.Update();
            }
            base.OnNavigatedTo(e);
        }

        private void SearchHistoryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Assign DataTemplate for selected items
            foreach (var item in e.AddedItems)
            {
                ListViewItem lvi = (sender as ListView).ContainerFromItem(item) as ListViewItem;
                lvi.ContentTemplate = (DataTemplate)this.Resources["HistoryItemExpanded"];
            }
            //Remove DataTemplate for unselected items
            foreach (var item in e.RemovedItems)
            {
                ListViewItem lvi = (sender as ListView).ContainerFromItem(item) as ListViewItem;
                lvi.ContentTemplate = (DataTemplate)this.Resources["HistoryItemCollapsed"];
            }
        }

        private void SearchHistoryList_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            var item = (HistoryItem)((FrameworkElement)e.OriginalSource).DataContext;
            listView.SelectedItem = item;
            SearhHistoryMenuFlyout.ShowAt(listView, e.GetPosition(listView));
        }

        private void SearchHistoryList_DoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            var item = (HistoryItem)((FrameworkElement)e.OriginalSource).DataContext;
            var param = new DictionaryNavParam()
            {
                SearchText = item.Word
            };
            SearchAction(param);
        }

        private void ClearSearchHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            AppSettings.ClearSearchHistoryItems();
        }

        private void SearchHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var item = (HistoryItem)SearchHistoryList.SelectedItem;
            var param = new DictionaryNavParam()
            {
                SearchText = item.Word
            };
            SearchAction(param);
        }

        private void DeleteHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var item = (HistoryItem)SearchHistoryList.SelectedItem;
            AppSettings.RemoveSearchHistoryItem(item);
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    System.Uri manifestUri = new Uri("https://dictionary.blob.core.chinacloudapi.cn/media/audio/tom/8f/c8/8FC8ED9CD7522AD312AAA417DDCFBEB8.mp3");
        //    var mediaPlayer = new MediaPlayer
        //    {
        //        Source = MediaSource.CreateFromUri(manifestUri)
        //    };
        //    mediaPlayer.Play();
        //}
    }
}
