using Memoling.Tools.WiktionaryParser.Data;

namespace Memoling.Tools.WiktionaryParser.Output
{
    public interface IJsonOutputResult
    {
        string JsonNewDocument(DataProcessorContext context);
        string JsonNewNode(DataProcessorContext context);
    }
}
