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
        public async Task<DictItem> SearchAsync(string word, bool useEnhancedDb)
        {
            DictItem result = new DictItem()
            {
                Word = word
            };
            Dict dictResult;
            if (useEnhancedDb)
            {
                dictResult = await Constants.BingEnhancedDictDbContext.Dict
                .AsNoTracking()
                .Where(x => x.Word == word)
                .FirstOrDefaultAsync();
            }
            else
            {
                dictResult = await Constants.BingDictDbContext.Dict
                .AsNoTracking()
                .Where(x => x.Word == word)
                .FirstOrDefaultAsync();
            }

            if (dictResult == null) return null;

            XDocument doc = XDocument.Parse(dictResult.Defi);

            var prons = doc.Root.Descendants("PRON").Descendants("PRON").ToList();
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

            var cdefs = doc.Root.Descendants("C_DEF").Descendants("SENS").ToList();
            foreach (var item in cdefs)
            {
                var partOfSpeech = item.Element("POS").Value;
                var definition = item.Element("SEN").Element("SEN").Element("D").Value;
                result.ChineseDefinitions.Add(new WordDefinition()
                {
                    PartOfSpeech = partOfSpeech,
                    Definition = definition
                });
            }

            var hdefs = doc.Root.Descendants("H_DEF").Descendants("SENS").ToList();
            foreach (var item in hdefs)
            {
                var partOfSpeech = item.Element("POS").Value;
                var definition = item.Element("SEN").Element("SEN").Element("D").Value;
                result.EnglishDefinitions.Add(new WordDefinition()
                {
                    PartOfSpeech = partOfSpeech,
                    Definition = definition
                });
            }

            var sents = doc.Root.Descendants("SENT_P").ToList();
            foreach (var item in sents)
            {
                var English = item.Element("T").Element("D").Value;
                var Chinses = item.Element("S").Element("D").Value;
                result.ExampleSentences.Add(new DualLangExampleSentence()
                {
                    Chinese = Chinses,
                    English = English
                });
            }

            result.EnglishDefinitions.Sort((a,b) => b.PartOfSpeech.CompareTo(a.PartOfSpeech));
            result.ChineseDefinitions.Sort((a, b) => b.PartOfSpeech.CompareTo(a.PartOfSpeech));

            return result;
        }

        public Task<DictItem> SearchAsync(string word)
        {
            return SearchAsync(word, false);
        }
    }
}
