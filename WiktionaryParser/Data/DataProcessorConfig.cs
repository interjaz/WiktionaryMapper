using System.Collections.Generic;
using Memoling.Tools.WiktionaryParser.Section;

namespace Memoling.Tools.WiktionaryParser.Data
{
    public class DataProcessorConfig
    {
        public IEnumerable<string> RemoveSections { get; set; }
        public IEnumerable<string> PartOfSpeech { get; set; }

        public string Langauge { get; set; }
        public string TopLevelSection { get; set; }

        public IEnumerable<SectionTransformation> SectionTransformations { get; set; }
    }
}
