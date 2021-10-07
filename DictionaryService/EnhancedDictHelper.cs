using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace DictionaryService
{
    public static class EnhancedDictHelper
    {
        /// <summary>
        /// 是否已经解压缩
        /// </summary>
        public static bool IsExtracted
        { 
            get
            {
                return File.Exists(DatabasePath);
            }
        }

        public static string DatabaseZipPath
        {
            get => Path.Combine(Package.Current.InstalledLocation.Path, "offline_enhanceV2.zip");
        }

        public static string DatabasePath
        {
            get => Path.Combine(ApplicationData.Current.LocalFolder.Path, "offline_enhanceV2.db");
        }


        public static void ExtractDatabase()
        {
            using (ZipArchive archive = ZipFile.Open(DatabaseZipPath, ZipArchiveMode.Read))
            {
                archive.ExtractToDirectory(ApplicationData.Current.LocalFolder.Path);
            }
        }

    }
}
