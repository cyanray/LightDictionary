using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryService.Models
{
    public class SuggestionItem
    {
        public string Word { get; set; }

        public string Chinese { get; set; }

        public long Frequency { get; set; }
    }
}
