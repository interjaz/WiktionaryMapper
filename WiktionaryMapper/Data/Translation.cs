using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memoling.Tools.WiktionaryMapper.Data
{
    public class Translation
    {
        public string ExpressionA { get; set; }
        public string ExpressionB { get; set; }
        public string LanguageA { get; set; }
        public string LanguageB { get; set; }

        public override string ToString()
        {
            return (LanguageA ?? "null") + "|"
                + (ExpressionA ?? "null") + "|"
                + (LanguageB ?? "null") + "|"
                + (ExpressionB ?? "null");
        }

    }
}
