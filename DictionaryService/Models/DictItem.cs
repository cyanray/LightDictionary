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

        /// <summary>
        /// 中文释义
        /// </summary>
        public List<WordDefinition> ChineseDefinitions { get; set; } = new List<WordDefinition>();

        /// <summary>
        /// 英文释义
        /// </summary>
        public List<WordDefinition> EnglishDefinitions { get; set; } = new List<WordDefinition>();

        /// <summary>
        /// 权威字典双语释义
        /// </summary>
        public List<DualLangWordDefinition> DualLangDefinitions { get; set; } = new List<DualLangWordDefinition>();

        /// <summary>
        /// 例句
        /// </summary>
        public List<DualLangExampleSentence> ExampleSentences { get; set; } = new List<DualLangExampleSentence>();
    }
}
