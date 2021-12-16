using DictionaryService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace LightDictionary.Utils
{
    internal static class AppShared
    {
        public static BingLocalDictionaryService BingLocalDictionaryService = new BingLocalDictionaryService();

        public static BingOnlineDictionaryService BingOnlineDictionaryService = new BingOnlineDictionaryService();

        public static AppSettings AppSettings= new AppSettings();

        public static string GetVersion()
        {
            PackageVersion version = Package.Current.Id.Version;
            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }
}
