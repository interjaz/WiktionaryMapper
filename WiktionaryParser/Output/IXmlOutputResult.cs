using Memoling.Tools.WiktionaryParser.Data;

namespace Memoling.Tools.WiktionaryParser.Output
{
    public interface IXmlOutputResult
    {
        string XmlNewDocument(DataProcessorContext context);
        string XmlNewNode(DataProcessorContext context);
    }
}
