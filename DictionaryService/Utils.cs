using DictionaryService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryService
{
    internal static class Utils
    {
        internal static readonly BingDictContext BingDictDbContext = new BingDictContext();
    }
}
