using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LightDictionary.Utils
{
    /// <summary>
    /// Class providing functionality around switching and restoring theme settings
    /// </summary>
    public static class ThemeHelper
    {
        /// <summary>
        /// Get or set the RequestedTheme of the root element.
        /// </summary>
        public static ElementTheme RootTheme
        {
            get
            {
                if (Window.Current.Content is FrameworkElement rootElement)
                {
                    return rootElement.RequestedTheme;
                }

                return ElementTheme.Default;
            }
            set
            {
                if (Window.Current.Content is FrameworkElement rootElement)
                {
                    rootElement.RequestedTheme = value;
                }
            }
        }

        public static TEnum GetEnum<TEnum>(string text) where TEnum : struct
        {
            if (!typeof(TEnum).GetTypeInfo().IsEnum)
            {
                throw new InvalidOperationException("Generic parameter 'TEnum' must be an enum.");
            }
            return (TEnum)Enum.Parse(typeof(TEnum), text);
        }

        public static void InitializeAppTheme()
        {

        }

        public struct ThemeChangedCallbackToken
        {
            public WeakReference RootFrame;
            public long Token;
        }

        public static ThemeChangedCallbackToken RegisterAppThemeChangedCallback(DependencyPropertyChangedCallback callback)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            long token = rootFrame.RegisterPropertyChangedCallback(Frame.RequestedThemeProperty, callback);
            return new ThemeChangedCallbackToken { RootFrame = new WeakReference(rootFrame), Token = token };
        }

        public static void UnregisterAppThemeChangedCallback(ThemeChangedCallbackToken callbackToken)
        {
            if (callbackToken.RootFrame.IsAlive)
            {
                Frame rootFrame = callbackToken.RootFrame.Target as Frame;
                rootFrame.UnregisterPropertyChangedCallback(Frame.RequestedThemeProperty, callbackToken.Token);
            }
        }
    }

}
