using DictionaryService;
using LightDictionary.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
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
    public sealed partial class SettingsPage : Page
    {
        private readonly IReadOnlyList<string> UserLanguages = ApplicationLanguages.ManifestLanguages;

        public AppSettings AppSettings { get; set; } = new AppSettings();

        private bool UserChange = false;
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private void LanguagePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!UserChange) return;
            InfoNeedRestart.IsOpen = true;
        }

        private async void InfoNeedRestart_RestartAction(object sender, RoutedEventArgs e)
        {
            AppRestartFailureReason result = await CoreApplication.RequestRestartAsync("");

            if (result == AppRestartFailureReason.NotInForeground || result == AppRestartFailureReason.Other)
            {
                //TODO: 自动重启失败，提示需要手动重启
            }
        }

        private void LanguagePicker_DropDownOpened(object sender, object e)
        {
            UserChange = true;
        }

        private void OnThemeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is RadioButton selectItem)
            {
                string themeName = selectItem.Tag.ToString();
                ThemeHelper.RootTheme = ThemeHelper.GetEnum<ElementTheme>(themeName);
                AppShared.AppSettings.CustomTheme = themeName;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var currentTheme = ThemeHelper.RootTheme.ToString();
            ThemeRadioButtons.Items.Cast<RadioButton>().FirstOrDefault(c => c?.Tag?.ToString() == currentTheme).IsChecked = true;
        }

        private async void EnhancedDictSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!EnhancedDictHelper.IsExtracted)
            {
                try
                {
                    await Task.Run(() => { EnhancedDictHelper.ExtractDatabase(); });
                    SuccessInfo.Message = "成功解压缩增强词典数据库";
                    SuccessInfo.IsOpen = true;
                }
                catch (Exception ex)
                {
                    ErrorInfo.Message = ex.Message;
                    ErrorInfo.IsOpen = true;
                    AppSettings.EnableEnhancedDictionary = false;
                }
            }
        }
    }
}
