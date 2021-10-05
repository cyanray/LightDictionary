using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

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
