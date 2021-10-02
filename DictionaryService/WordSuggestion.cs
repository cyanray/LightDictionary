using DictionaryService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryService
{
    public static class WordSuggestion
    {
        public static Task<List<SuggestionItem>> GetSuggestionsAsync(string text, int limit = 10)
        {
            using (var db = new BingDictContext())
            {
                var result = db.Dict
                    .AsNoTracking()
                    .Where(x => EF.Functions.Like(x.Word, $"{text}%"))
                    .Take(limit)
                    .OrderByDescending(x => x.Freq)
                    .Select(x => new SuggestionItem()
                    {
                        Word = x.Word,
                        Chinese = x.AutoSugg,
                        Frequency = (long)x.Freq
                    })
                    .ToListAsync();
                return result;
            }
        }
    }
}
