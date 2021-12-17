using LightDictionary.Models;
using LightDictionary.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NotePage : Page
    {

        ObservableCollection<NoteHeader> NoteHeaders = new ObservableCollection<NoteHeader>();

        List<NoteItem> NoteItems = new List<NoteItem>();

        ObservableValue<NoteItem> SelectedNoteItem = new ObservableValue<NoteItem>();

        public NotePage()
        {
            this.InitializeComponent();


            NoteItems.Add(new NoteItem()
            {
                Content = "hello",
                CreateTime = DateTime.Parse("2021-11-08")
            });
            NoteItems.Add(new NoteItem()
            {
                Content = "hello2",
                CreateTime = DateTime.Parse("2021-11-08")
            });
            NoteItems.Add(new NoteItem()
            {
                Content = "hello3",
                CreateTime = DateTime.Parse("2021-11-08")
            });
            NoteItems.Add(new NoteItem()
            {
                Content = "will",
                CreateTime = DateTime.Parse("2021-12-12")
            });
            NoteItems.Add(new NoteItem()
            {
                Content = "light",
                CreateTime = DateTime.Parse("2021-12-12")
            });
            NoteItems.Add(new NoteItem()
            {
                Content = "light",
                CreateTime = DateTime.Parse("2021-12-12")
            });
            NoteItems.Add(new NoteItem()
            {
                Content = "light",
                CreateTime = DateTime.Parse("2021-12-12")
            });
            NoteItems.Add(new NoteItem()
            {
                Content = "light",
                CreateTime = DateTime.Parse("2021-12-12")
            });
            NoteItems.Add(new NoteItem()
            {
                Content = "light",
                CreateTime = DateTime.Parse("2021-12-12")
            });

        }

        public Task<List<NoteHeader>> GetContactsGroupedAsync()
        {
            var query = from item in NoteItems
                        group item by item.CreateTime into g
                        orderby g.Key
                        select new NoteHeader() 
                        { 
                            Items = new ObservableCollection<NoteItem>(g),
                            Title = g.Key.ToString("yyyy-MM-dd")
                        };

            return Task.FromResult(new List<NoteHeader>(query));
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var result = await GetContactsGroupedAsync();
            foreach (var item in result)
            {
                NoteHeaders.Add(item);
            }
        }

        private void NoteTreeView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if(NoteTreeView.SelectedItem is NoteItem item)
            {
                SelectedNoteItem.Value = item;
            }
        }
    }
}
