using Memoling.Tools.WiktionaryParser.Data;

namespace Memoling.Tools.WiktionaryParser.Output
{
    public interface IPlainTextOutputResult
    {
        string PlainTextToString(DataProcessorContext context);
    }
}
