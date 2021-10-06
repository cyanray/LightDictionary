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
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace LightDictionary.UserControls
{
    [ContentProperty(Name = "MainContent")]
    public sealed partial class CardView : UserControl
    {
        public CardView()
        {
            this.InitializeComponent();
        }

        public static DependencyProperty MainContentProperty = 
            DependencyProperty.Register("MainContent", typeof(object), typeof(CardView), null);

        public object MainContent
        {
            get => GetValue(MainContentProperty);
            set => SetValue(MainContentProperty, value);
        }

    }
}
