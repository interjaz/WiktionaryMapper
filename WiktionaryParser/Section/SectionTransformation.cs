using System;
using System.Collections.Generic;
using Memoling.Tools.WiktionaryParser.Data;

namespace Memoling.Tools.WiktionaryParser.Section
{
    public class SectionTransformation
    {
        public IEnumerable<string> Headers { get; set; }
        public Func<DataProcessorContext, string, string, dynamic> Transformation { set; get; }

        public override string ToString()
        {
            return string.Join("|", Headers);
        }
    }
}
