using System.Collections.Generic;
using Memoling.Tools.WiktionaryParser.Data;

namespace Memoling.Tools.WiktionaryParser.Output
{
    public class OutputProcessor
    {
        public List<OutputResult> Outputs { get; private set; }

        public OutputProcessor()
        {
            Outputs = new List<OutputResult>();
        }

        public void Next(DataProcessorResult result)
        {
            foreach(var output in Outputs)
            {
                output.Next(result);
            }
        }
    }
}
