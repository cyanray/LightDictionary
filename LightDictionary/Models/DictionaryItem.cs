using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace LightDictionary.Models
{
    public class DictionaryItem
    {
        public string Name { get; set; }

        public Visibility RefreshButtonVisibility { get; set; }

        public int MyProperty { get; set; }
    }
}
