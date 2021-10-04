using LightDictionary.Utils;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Color = Windows.UI.Color;
using MUXC = Microsoft.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LightDictionary
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ThemeHelper.ThemeChangedCallbackToken m_rootFrameRequestedThemeCallbackToken;

        public static Frame MainPageFrame;

        public MainPage()
        {
            this.InitializeComponent();
            MainPageFrame = ContentFrame;

            var titleBar = ApplicationView.GetForCurrentView().TitleBar;

            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

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
                ThemeHelper.RegisterAppThemeChangedCallback(RootFrame_RequestedThemeChanged);

            SetTitleBarControlColors();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            ThemeHelper.
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

            Color bgColor = Colors.Transparent;
            Color fgColor = ((SolidColorBrush)Resources["ButtonForegroundColor"]).Color;
            Color inactivefgColor = ((SolidColorBrush)Resources["ButtonInactiveForegroundBrush"]).Color;
            Color hoverbgColor = ((SolidColorBrush)Resources["ButtonHoverBackgroundBrush"]).Color;
            Color hoverfgColor = ((SolidColorBrush)Resources["ButtonHoverForegroundBrush"]).Color;
            Color pressedbgColor = ((SolidColorBrush)Resources["ButtonPressedBackgroundBrush"]).Color;
            Color pressedfgColor = ((SolidColorBrush)Resources["ButtonPressedForegroundBrush"]).Color;

            applicationTitleBar.ButtonBackgroundColor = bgColor;
            applicationTitleBar.ButtonForegroundColor = fgColor;
            applicationTitleBar.ButtonInactiveBackgroundColor = bgColor;
            applicationTitleBar.ButtonInactiveForegroundColor = inactivefgColor;
            applicationTitleBar.ButtonHoverBackgroundColor = hoverbgColor;
            applicationTitleBar.ButtonHoverForegroundColor = hoverfgColor;
            applicationTitleBar.ButtonPressedBackgroundColor = pressedbgColor;
            applicationTitleBar.ButtonPressedForegroundColor = pressedfgColor;
        }

        // Update the TitleBar content layout depending on NavigationView DisplayMode
        private void MainNav_DisplayModeChanged(MUXC.NavigationView sender, MUXC.NavigationViewDisplayModeChangedEventArgs args)
        {
            const int topIndent = 16;
            const int expandedIndent = 48;
            int minimalIndent = 104;

            // If the back button is not visible, reduce the TitleBar content indent.
            if (MainNav.IsBackButtonVisible.Equals(MUXC.NavigationViewBackButtonVisible.Collapsed))
            {
                minimalIndent = 48;
            }

            Thickness currMargin = AppTitleBar.Margin;

            // Set the TitleBar margin dependent on NavigationView display mode
            if (sender.PaneDisplayMode == MUXC.NavigationViewPaneDisplayMode.Top)
            {
                AppTitleBar.Margin = new Thickness(topIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
            else if (sender.DisplayMode == MUXC.NavigationViewDisplayMode.Minimal)
            {
                AppTitleBar.Margin = new Thickness(minimalIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
            else
            {
                AppTitleBar.Margin = new Thickness(expandedIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
        }

        private void MainNav_ItemInvoked(MUXC.NavigationView sender, MUXC.NavigationViewItemInvokedEventArgs args)
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
                    default:
                        break;
                }
            }
            if (args.IsSettingsInvoked)
            {
                _ = ContentFrame.Navigate(typeof(SettingsPage));
                return;
            }
        }

        private void MainNav_BackRequested(MUXC.NavigationView sender, MUXC.NavigationViewBackRequestedEventArgs args)
        {
            TryGoBack();
        }

        private bool TryGoBack()
        {
            if (!ContentFrame.CanGoBack) return false;

            // Don't go back if the nav pane is overlayed.
            if (MainNav.IsPaneOpen &&
                (MainNav.DisplayMode == MUXC.NavigationViewDisplayMode.Compact ||
                 MainNav.DisplayMode == MUXC.NavigationViewDisplayMode.Minimal))
            {
                return false;
            }

            ContentFrame.GoBack();
            return true;
        }

        private void ContentFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            MainNav.IsBackEnabled = ContentFrame.CanGoBack;
        }
    }
}
