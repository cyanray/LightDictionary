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

        public List<WordDefinition> WordDefinitions { get; set; } = new List<WordDefinition>();
    }
}
