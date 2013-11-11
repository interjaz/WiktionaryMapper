using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Memoling.Tools.WiktionaryParser.Data;
using Memoling.Tools.WiktionaryParser;

namespace Memoling.Tools.WiktionaryMapper.Data
{
    public class PartOfSpeech : NormalizedOutputResult
    {
        public string Expression { get; set; }
        public string Language { get; set; }
        public string Part { get; set; }
        public string Definition { get; set; }

        public override string ToString()
        {
            return (Expression ?? "null") + "|" + (Language ?? "null") + "|" + (Part ?? "null") + "|" + (Definition ?? "null");
        }

        public override string SqlCreateStatement(DataProcessorContext context)
        {
            string sql = "CREATE TABLE wiki_Definitions (" +
                "Expression VARCHAR(MAX) NOT NULL," +
                "Language VARCHAR(MAX) NOT NULL, " +
                "PartOfSpeech VARCHAR(MAX) NOT NULL," +
                "Definition VARCHAR(MAX) NOT NULL" +
            ")" + SqlDelimiter + "\n";


            if (SqlLite)
            {
                sql = sql.Replace("VARCHAR(MAX)", "VARCHAR");
            }

            return sql;
        }


        public override string SqlInsertStatement(DataProcessorContext context)
        {
            return "INSERT INTO wiki_Definitions VALUES ('" + Expression.EscapeSql() + "','" + Language + "','" + Part.EscapeSql() + "','" + Definition.EscapeSql() + "')" + SqlDelimiter + "\n";
        }

        public override string PlainTextToString(DataProcessorContext context)
        {
            return string.Format("{0}{4}{1}{4}{2}{4}{3}\n", Expression, Language, Part, Definition.Replace("\n", "\\n"), PlainTextDelimiter);
        }
    }
}
