using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryService
{
    internal static partial class Utils
    {
        /// <summary>
        /// 分辨句子A和句子B谁是中文谁是英文（根据句子中空格的数量，空格数量多者为英文句子）
        /// </summary>
        /// <param name="a">句子A</param>
        /// <param name="b">句子B</param>
        /// <returns></returns>
        public static (string English, string Chinese) DepartByLanguage(string a, string b)
        {
            return a.Count(x => x == ' ') > b.Count(x => x == ' ') ? (a, b) : (b, a);
        }
    }

}
