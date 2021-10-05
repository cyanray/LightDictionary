using DictionaryService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DictionaryService
{
    public class BingLocalDictionaryService : IDictionaryService
    {
        public async Task<DictItem> SearchAsync(string word)
        {
            DictItem result = new DictItem()
            {
                Word = word
            };
            var dictResult = await Utils.BingDictDbContext.Dict
                .AsNoTracking()
                .Where(x => x.Word == word)
                .FirstOrDefaultAsync();
            if (dictResult == null) return null;

            result.Suggestion = new SuggestionItem
            {
                Word = dictResult.Word,
                Chinese = dictResult.AutoSugg,
                Frequency = (long)dictResult.Freq
            };

            XDocument doc = XDocument.Parse(dictResult.Defi);
            Console.WriteLine(doc);
            var prons = doc.Root.Elements("PRON").Descendants("PRON").ToList();
            foreach (var item in prons)
            {
                var L = item.Element("L").Value;
                var V = item.Element("V").Value;
                if (L.ToUpper() == "US")
                {
                    result.UsPronunciation = V;
                }
                else
                {
                    result.UkPronunciation = V;
                }
            }

            var defs = doc.Root.Descendants("SENS").ToList();
            foreach (var item in defs)
            {
                var partOfSpeech = item.Element("POS").Value;
                var definition = item.Element("SEN").Element("SEN").Element("D").Value;
                result.WordDefinitions.Add(new WordDefinition()
                {
                    PartOfSpeech = partOfSpeech,
                    Definition = definition
                });
            }

            return result;
        }
    }
}
