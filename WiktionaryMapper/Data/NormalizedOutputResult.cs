using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Memoling.Tools.WiktionaryParser.Data;
using Memoling.Tools.WiktionaryParser.Output;

namespace Memoling.Tools.WiktionaryMapper.Data
{
    public abstract class NormalizedOutputResult : ISqlOutputResult, IPlainTextOutputResult
    {
        protected const string PlainTextDelimiter = "\t";
        protected const string SqlDelimiter = ";";

        protected const bool SqlLite = true;

        public abstract string SqlCreateStatement(DataProcessorContext context);
        public abstract string SqlInsertStatement(DataProcessorContext context);
        public abstract string PlainTextToString(DataProcessorContext context);
    }
}
