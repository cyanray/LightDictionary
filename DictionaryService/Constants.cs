using DictionaryService.DbContexts;
using DictionaryService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryService
{
    internal static class Constants
    {
        internal static readonly BingDictContext BingDictDbContext = new BingDictContext();

        internal static readonly BingEnhancedDictDbContext BingEnhancedDictDbContext = new BingEnhancedDictDbContext();
    }
}
