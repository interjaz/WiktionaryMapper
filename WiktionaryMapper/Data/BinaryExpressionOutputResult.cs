using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Memoling.Tools.WiktionaryParser;
using Memoling.Tools.WiktionaryParser.Data;
using Memoling.Tools.WiktionaryParser.Output;

namespace Memoling.Tools.WiktionaryMapper.Data
{
    public class BinaryExpressionOutputResult : NormalizedOutputResult
    {
        public string Language { get; set; }
        public string ExpressionA { get; set; }
        public string ExpressionB { get; set; }

        protected string Name;

        public override string SqlCreateStatement(DataProcessorContext context)
        {
            string sql = "CREATE TABLE wiki_" + Name + " (" +
                "ExpressionA VARCHAR(MAX) NOT NULL," +
                "ExpressionB VARCHAR(MAX) NOT NULL," +
                "Language VARCHAR(MAX) NOT NULL" +
            ");\n";

            if (SqlLite)
            {
                sql = sql.Replace("VARCHAR(MAX)", "VARCHAR");
            }

            return sql;
        }

        public override string SqlInsertStatement(DataProcessorContext context)
        {
            return "INSERT INTO wiki_" + Name + " VALUES ('" + ExpressionA.EscapeSql() + "','" + ExpressionB.EscapeSql() + "','" + Language + "');\n";
        }

        public override string PlainTextToString(DataProcessorContext context)
        {
            return string.Format("{0}{3}{1}{3}{2}\n", ExpressionA, ExpressionB, Language, PlainTextDelimiter);
        }

        public static void RemoveDuplicates(StreamReader input, StreamWriter output, OutputFormat format)
        {
            if (format == OutputFormat.Sql || format == OutputFormat.PlainText)
            {

                List<string> data = input.ReadToEnd().Split('\n').ToList();

                List<string> unique = new List<string>();
                Regex pattern = new Regex(@"[(,]");

                while (data.Count() > 0)
                {
                    string line = data[0];
                    data.RemoveAll(p => p == line);

                    if (line.Trim() == string.Empty)
                    {
                        continue;
                    }

                    unique.Add(line);

                    string[] parts = pattern.Split(line);
                    string duplicateLine = parts[0] + "(" + parts[2] + "," + parts[1] + "," + parts[3];


                    data.RemoveAll(p => p == duplicateLine);
                }

                foreach (var line in unique)
                {
                    output.Write(line + "\n");
                }
            }
        }
    }
}
