using System;
using System.Collections.Generic;


namespace DictionaryService.Models
{
    public partial class Dict
    {
        public string Word { get; set; }
        public string AutoSugg { get; set; }
        public string Defi { get; set; }
        public long? Freq { get; set; }
    }
}
