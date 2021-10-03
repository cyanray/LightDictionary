using DictionaryService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryService
{
    public interface IDictionaryService
    {
        Task<DictItem> SearchAsync(string word);
    }
}
