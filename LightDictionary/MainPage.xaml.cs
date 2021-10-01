using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Microsoft.Toolkit.Uwp.Helpers;
using Color = Windows.UI.Color;
using LightDictionary.Utils;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LightDictionary
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Utils.ThemeHelper.ThemeChangedCallbackToken m_rootFrameRequestedThemeCallbackToken;

        public MainPage()
        {
            this.InitializeComponent();
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;

            titleBar.ButtonBackgroundColor = Windows.UI.Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Windows.UI.Colors.Transparent;

            // Hide default title bar.
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            UpdateTitleBarLayout(coreTitleBar);

            // Register a handler for when the size of the overlaid caption control changes.
            // For example, when the app moves to a screen with a different DPI.
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

            // Register a handler for when the title bar visibility changes.
            // For example, when the title bar is invoked in full screen mode.
            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;

            //Register a handler for when the window changes focus
            Window.Current.Activated += Current_Activated;

            // Set XAML element as a draggable region.
            Window.Current.SetTitleBar(AppTitleBar);

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;

            _ = ContentFrame.Navigate(typeof(DictionaryPage));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Register RequestedTheme changed callback to update title bar system button colors.
            m_rootFrameRequestedThemeCallbackToken =
                Utils.ThemeHelper.RegisterAppThemeChangedCallback(RootFrame_RequestedThemeChanged);

            SetTitleBarControlColors();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Utils.ThemeHelper.
                UnregisterAppThemeChangedCallback(m_rootFrameRequestedThemeCallbackToken);
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBarLayout(sender);
        }

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            // Ensure the custom title bar does not overlap window caption controls
            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, 0, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
        }

        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            if (sender.IsVisible)
            {
                AppTitleBar.Visibility = Visibility.Visible;
            }
            else
            {
                AppTitleBar.Visibility = Visibility.Collapsed;
            }
        }

        // Update the TitleBar based on the inactive/active state of the app
        private void Current_Activated(object sender, WindowActivatedEventArgs e)
        {
            VisualStateManager.GoToState(
                this, e.WindowActivationState == CoreWindowActivationState.Deactivated ? WindowNotFocused.Name : WindowFocused.Name, false);
        }

        private void RootFrame_RequestedThemeChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (Frame.RequestedThemeProperty == dp)
            {
                _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() => { SetTitleBarControlColors(); }));
            }
        }

        private void SetTitleBarControlColors()
        {
            var applicationView = ApplicationView.GetForCurrentView();
            if (applicationView == null)
            {
                return;
            }

            var applicationTitleBar = applicationView.TitleBar;
            if (applicationTitleBar == null)
            {
                return;
            }

            if (ThemeHelper.RootTheme == ElementTheme.Light)
            {
                applicationTitleBar.ButtonBackgroundColor = Windows.UI.Colors.Transparent;
                applicationTitleBar.ButtonForegroundColor = Windows.UI.Colors.Black;
                applicationTitleBar.ButtonInactiveBackgroundColor = Windows.UI.Colors.Transparent;
                applicationTitleBar.ButtonInactiveForegroundColor = ColorHelper.ToColor("#FF7A7A7A");
                applicationTitleBar.ButtonHoverBackgroundColor = ColorHelper.ToColor("#19000000");
                applicationTitleBar.ButtonHoverForegroundColor = Windows.UI.Colors.Black;
                applicationTitleBar.ButtonPressedBackgroundColor = ColorHelper.ToColor("#33000000");
                applicationTitleBar.ButtonPressedForegroundColor = Windows.UI.Colors.Black;
            }
            else if (ThemeHelper.RootTheme == ElementTheme.Dark)
            {
                applicationTitleBar.ButtonBackgroundColor = Windows.UI.Colors.Transparent;
                applicationTitleBar.ButtonForegroundColor = Windows.UI.Colors.White;
                applicationTitleBar.ButtonInactiveBackgroundColor = Windows.UI.Colors.Transparent;
                applicationTitleBar.ButtonInactiveForegroundColor = ColorHelper.ToColor("#FF858585");
                applicationTitleBar.ButtonHoverBackgroundColor = ColorHelper.ToColor("#19FFFFFF");
                applicationTitleBar.ButtonHoverForegroundColor = Windows.UI.Colors.White;
                applicationTitleBar.ButtonPressedBackgroundColor = ColorHelper.ToColor("#33FFFFFF");
                applicationTitleBar.ButtonPressedForegroundColor = Windows.UI.Colors.White;
            }

        }

        // Update the TitleBar content layout depending on NavigationView DisplayMode
        private void MainNav_DisplayModeChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewDisplayModeChangedEventArgs args)
        {
            const int topIndent = 16;
            const int expandedIndent = 48;
            int minimalIndent = 104;

            // If the back button is not visible, reduce the TitleBar content indent.
            if (MainNav.IsBackButtonVisible.Equals(Microsoft.UI.Xaml.Controls.NavigationViewBackButtonVisible.Collapsed))
            {
                minimalIndent = 48;
            }

            Thickness currMargin = AppTitleBar.Margin;

            // Set the TitleBar margin dependent on NavigationView display mode
            if (sender.PaneDisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Top)
            {
                AppTitleBar.Margin = new Thickness(topIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
            else if (sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal)
            {
                AppTitleBar.Margin = new Thickness(minimalIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
            else
            {
                AppTitleBar.Margin = new Thickness(expandedIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
        }

        private void MainNav_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender,
            Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            var item = args.InvokedItemContainer;
            if (item != null)
            {
                switch (item.Tag)
                {
                    case "Dictionary":
                        _ = ContentFrame.Navigate(typeof(DictionaryPage));
                        break;

                    case "Translation":
                        _ = ContentFrame.Navigate(typeof(TranslationPage));
                        break;
                    case "Note":
                        _ = ContentFrame.Navigate(typeof(NotePage));
                        break;
                }
            }
            if (args.IsSettingsInvoked)
            {
                _ = ContentFrame.Navigate(typeof(SettingsPage));
                return;
            }
        }

        private void MainNav_BackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
        {
            TryGoBack();
        }

        private bool TryGoBack()
        {
            if (!ContentFrame.CanGoBack) return false;

            // Don't go back if the nav pane is overlayed.
            if (MainNav.IsPaneOpen &&
                (MainNav.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Compact ||
                 MainNav.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal))
                return false;

            ContentFrame.GoBack();
            return true;
        }

        private void ContentFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            MainNav.IsBackEnabled = ContentFrame.CanGoBack;
        }
    }
}
