using DictionaryService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightDictionary.Utils
{
    internal static class Constants
    {
        public static BingLocalDictionaryService BingLocalDictionaryService = new BingLocalDictionaryService();

        public static BingOnlineDictionaryService BingOnlineDictionaryService = new BingOnlineDictionaryService();

        public static AppSettings AppSettings= new AppSettings();
    }
}
