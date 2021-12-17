using LightDictionary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LightDictionary.Utils
{
    public class NoteItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NoteItemExpandedTemplate { get; set; }

        public DataTemplate NoteItemCollapsedTemplate { get; set; }

        public DataTemplate NoteHeaderTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is NoteItem noteItem)
            {
                return noteItem.IsSelected ? NoteItemExpandedTemplate : NoteItemCollapsedTemplate;
            }
            else
            {
                return NoteHeaderTemplate;
            }
        }

    }
}
