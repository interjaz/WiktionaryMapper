using Memoling.Tools.WiktionaryParser.Data;

namespace Memoling.Tools.WiktionaryParser.Output
{
    public interface ISqlOutputResult
    {
        string SqlCreateStatement(DataProcessorContext context);
        string SqlInsertStatement(DataProcessorContext context);
    }
}
