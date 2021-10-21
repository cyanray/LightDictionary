using DictionaryService.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryService
{
    public class BingOnlineDictionaryService : IDictionaryService
    {
        public async Task<DictItem> SearchAsync(string word)
        {
            var client = new RestClient($"https://www.bing.com/api/v6/dictionarywords/search?q={word}&appid=371E7B2AF0F9B84EC491D731DF90A55719C7D209&mkt=zh-cn&pname=bingdict");
            var request = new RestRequest(Method.GET);
            IRestResponse response = await client.ExecuteAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("非 200 状态相应");
            }
            var json = JObject.Parse(response.Content);
            var meaningGroups = json["value"][0]["meaningGroups"].ToObject<List<MeaningGroup>>();

            var result = new DictItem()
            {
                Word = word
            };

            var pronResult = meaningGroups
                .Where(x => x.PartsOfSpeech.Description == "权威英汉双解发音")
                .ToList();

            foreach (var item in pronResult)
            {
                var pron = item.Meaning.RichDefinitions.FirstOrDefault()?.Fragments.FirstOrDefault()?.Text;
                if (item.PartsOfSpeech?.Name == "UK")
                {
                    result.UkPronunciation = pron;
                }
                else
                {
                    result.UsPronunciation = pron;
                }
            }

            var wordResult = meaningGroups
                .Where(x => x.PartsOfSpeech.Description == "英汉")
                .ToList();

            foreach (var item in wordResult)
            {
                var partOfSpeech = item.PartsOfSpeech?.Name;
                var defination = string.Join("; ", item.Meaning.RichDefinitions.Select(x => x.Fragments.FirstOrDefault()?.Text));
                result.ChineseDefinitions.Add(new WordDefinition() { PartOfSpeech = partOfSpeech, Definition = defination });
            }

            var authResult = meaningGroups
                .Where(x => x.PartsOfSpeech.Description == "权威英汉双解")
                .ToList();

            foreach (var item in authResult)
            {
                foreach (var richDefinition in item.Meaning.RichDefinitions)
                {
                    var definition = richDefinition.SubDefinitions
                                                .Where(x => x.Domain == "Definition")
                                                .Select(x => x.Fragments)
                                                .Where(x => x.Count >= 2)
                                                .FirstOrDefault();
                    var (en_def, ch_def) = Utils.DepartByLanguage(definition[0].Text, definition[1].Text);

                    var examples = richDefinition.SubDefinitions
                        .Where(x => x.Domain.StartsWith("Examples"))
                        .Select(x => x.Fragments)
                        .Where(x => x.Count >= 2)
                        .Select(x =>
                        {
                            var (English, Chinese) = Utils.DepartByLanguage(x[0].Text, x[1].Text);
                            return new DualLangExampleSentence()
                            {
                                English = English,
                                Chinese = Chinese
                            };
                        })
                        .ToList();

                    var complement = richDefinition.SubDefinitions
                                        .Where(x => x.Domain == "Complements")
                                        .Select(x => x.Fragments)
                                        .FirstOrDefault()
                                        ?.FirstOrDefault()?.Text;

                    var wdef = new DualLangWordDefinition()
                    {
                        PartOfSpeech = item.PartsOfSpeech?.Name,
                        ChineseDefinition = ch_def,
                        EnglishDefinition = en_def,
                        Complement = complement,
                        Examples = examples
                    };

                    result.DualLangDefinitions.Add(wdef);
                }
            }
            return result;
        }

    }

    internal class Fragment
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }

    internal class SubDefinition
    {
        [JsonProperty("fragments")]
        public List<Fragment> Fragments { get; set; }

        [JsonProperty("domains")]
        public List<string> domains { get; set; }

        public string Domain
        {
            get
            {
                return string.Join(".", domains);
            }
        }
    }

    internal class RichDefinition
    {
        [JsonProperty("fragments")]
        public List<Fragment> Fragments { get; set; }

        [JsonProperty("examples")]
        public List<string> Examples { get; set; }

        [JsonProperty("subDefinitions")]
        public List<SubDefinition> SubDefinitions { get; set; }
    }

    internal class Synonym
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    internal class Meaning
    {
        [JsonProperty("richDefinitions")]
        public List<RichDefinition> RichDefinitions { get; set; }

        [JsonProperty("synonyms")]
        public List<Synonym> Synonyms { get; set; }
    }

    internal class PartsOfSpeech
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    internal class MeaningGroup
    {
        [JsonProperty("meanings")]
        private List<Meaning> meanings { get; set; }

        [JsonProperty("partsOfSpeech")]

        private List<PartsOfSpeech> partsOfSpeech { get; set; }

        public PartsOfSpeech PartsOfSpeech
        {
            get
            {
                return partsOfSpeech.Count == 0 ? null : partsOfSpeech[0];
            }
        }

        public Meaning Meaning
        {
            get
            {
                return meanings.Count == 0 ? null : meanings[0];
            }
        }
    }


}
