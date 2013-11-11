
namespace Memoling.Tools.WiktionaryParser.Data
{
    public class DataProcessorContext
    {
        public DataProcessorConfig Config { get; private set; }
        public string Title { get; private set; }
        public string Page { get; private set; }

        public DataProcessorContext(DataProcessorConfig config, string title, string page)
        {
            Config = config;
            Title = title;
            Page = page;
        }
    }
}
