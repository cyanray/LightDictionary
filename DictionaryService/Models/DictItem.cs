using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryService.Models
{
    public class DictItem
    {
        public string Word { get; set; }

        public string UsPronunciation { get; set; }

        public string UkPronunciation { get; set; }

        public SuggestionItem Suggestion {  get; set; }

        public List<WordDefinition> ChineseDefinitions { get; set; } = new List<WordDefinition>();

        public List<WordDefinition> EnglishDefinitions { get; set; } = new List<WordDefinition>();

        public List<ExampleSentence> ExampleSentences { get; set; } = new List<ExampleSentence>();
    }
}
