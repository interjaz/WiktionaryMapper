using System.Collections.Generic;

namespace Memoling.Tools.WiktionaryParser.Data
{
    public class DataProcessorResult
    {
        public DataProcessorContext Context;
        public IEnumerable<Section> TransformedSections;
        public IEnumerable<Section> ExtraSections;
    }
}
