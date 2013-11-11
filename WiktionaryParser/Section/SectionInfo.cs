
namespace Memoling.Tools.WiktionaryParser.Data
{
    public class SectionInfo
    {
        public string Header { get; set; }
        public int Level { get; set; }
        public int ContentIndex { get; set; }
        public int ContentLength { get; set; }

        public override string ToString()
        {
            return Header;
        }
    }
}
