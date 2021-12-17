using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightDictionary.Models
{
    public class NoteHeader
    {
        public string Title { get; set; }

        public ObservableCollection<NoteItem> Items { get; set; }
    }
}
