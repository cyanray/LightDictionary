using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryService.Models
{
    public class WordDefinition
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
        /// 定义
        /// </summary>
        public string Definition { get; set; }


    }
}
