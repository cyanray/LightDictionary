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
            var result = Constants.BingDictDbContext.Dict
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

        /// <summary>
        /// 确保 BingDictDbContext 已经初始化，避免第一次查询时卡顿。
        /// </summary>
        /// <returns></returns>
        public static bool Init()
        {
            Constants.BingDictDbContext.Database.IsSqlite();
            return true;
        }
    }
}
