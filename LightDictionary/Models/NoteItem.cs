using LightDictionary.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LightDictionary.Models
{
    public class NoteItem : INotifyPropertyChanged
    {
        private bool isSelected;

        public string Content { get; set; }

        public DateTime CreateTime { get; set; }

        public bool IsSelected 
        { 
            get => isSelected; 
            set
            {
                isSelected = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
