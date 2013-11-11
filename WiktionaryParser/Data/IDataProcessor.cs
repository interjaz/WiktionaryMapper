
namespace Memoling.Tools.WiktionaryParser.Data
{
    public interface IDataProcessor
    {
        DataProcessorResult Next(string title, string text);
    }
}
