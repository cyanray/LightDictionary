using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryService.Models
{
    public class DualLangWordDefinition
    {
        /// <summary>
        /// 词性
        /// </summary>
        public string PartOfSpeech { get; set; }

        /// <summary>
        /// 词性，用于 UI 展示
        /// </summary>
        public string PartOfSpeechDisplay { get => $"{PartOfSpeech}."; }

        /// <summary>
        /// 中文定义
        /// </summary>
        public string ChineseDefinition { get; set; }

        /// <summary>
        /// 英文定义
        /// </summary>
        public string EnglishDefinition { get; set; }

        /// <summary>
        /// 补充
        /// </summary>
        public string Complement { get; set; }

        /// <summary>
        /// 例子
        /// </summary>
        public List<DualLangExampleSentence> Examples { get; set; } = new List<DualLangExampleSentence>();
    }
}
