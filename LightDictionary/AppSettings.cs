using DictionaryService.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using LightDictionary.Utils;
using LightDictionary.Models;

namespace LightDictionary
{
    public class AppSettings : INotifyPropertyChanged
    {
        public string PrimaryLanguageOverride
        {
            get => ReadSettings(nameof(PrimaryLanguageOverride), "en-US");
            set
            {
                SaveSettings(nameof(PrimaryLanguageOverride), value);
                NotifyPropertyChanged();
            }
        }

        public string CustomTheme
        {
            get => ReadSettings(nameof(CustomTheme), "Default");
            set
            {
                SaveSettings(nameof(CustomTheme), value);
                NotifyPropertyChanged();
            }
        }

        public bool EnableEnhancedDictionary
        {
            get => ReadSettings(nameof(EnableEnhancedDictionary), false);
            set
            {
                SaveSettings(nameof(EnableEnhancedDictionary), value);
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<HistoryItem> _searchHistoryItems;
        public ObservableCollection<HistoryItem> SearchHistoryItems
        {
            get
            {
                if (_searchHistoryItems == null)
                {
                    var text = ReadSettings(nameof(SearchHistoryItems), "[]");
                    _searchHistoryItems = JsonConvert.DeserializeObject<ObservableCollection<HistoryItem>>(text);
                }
                return _searchHistoryItems;
            }
        }

        public void AddSearchHistoryItem(HistoryItem item)
        {
            SearchHistoryItems.Remove(x => x.Word == item.Word);
            SearchHistoryItems.Insert(0, item);
            if (SearchHistoryItems.Count > 15)
            {
                for (int i = 15; i < _searchHistoryItems.Count; i++)
                {
                    SearchHistoryItems.RemoveAt(i);
                }
            }
            SaveSearchHistoryItems();
        }

        public void RemoveSearchHistoryItem(HistoryItem item)
        {
            SearchHistoryItems.Remove(item);
            SaveSearchHistoryItems();
        }

        private void SaveSearchHistoryItems()
        {
            var text = JsonConvert.SerializeObject(SearchHistoryItems);
            SaveSettings(nameof(SearchHistoryItems), text);
            NotifyPropertyChanged(nameof(SearchHistoryItems));
        }

        public void ClearSearchHistoryItems()
        {
            SearchHistoryItems.Clear();
            SaveSearchHistoryItems();
        }

        public ApplicationDataContainer LocalSettings { get; set; }

        public AppSettings()
        {
            LocalSettings = ApplicationData.Current.LocalSettings;
        }

        private void SaveSettings(string key, object value)
        {
            LocalSettings.Values[key] = value;
        }

        private T ReadSettings<T>(string key, T defaultValue)
        {
            if (LocalSettings.Values.ContainsKey(key))
            {
                return (T)LocalSettings.Values[key];
            }
            if (null != defaultValue)
            {
                return defaultValue;
            }
            return default(T);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
