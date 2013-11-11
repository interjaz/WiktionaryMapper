using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Memoling.Tools.WiktionaryParser.Data;
using Memoling.Tools.WiktionaryParser;

namespace Memoling.Tools.WiktionaryMapper.Data
{
    public class TranslationsWithMeaning : NormalizedOutputResult
    {
        private static long initialId = 0;

        public static long NextId()
        {
            return Interlocked.Increment(ref initialId);
        }

        public IEnumerable<Translation> Translations { get; set; }
        public string Meaning { get; set; }
        public long MeaningId { get; set; }

        public override string SqlCreateStatement(DataProcessorContext context)
        {
            string sql = "";

            sql += "CREATE TABLE wiki_TranslationMeanings (" +
                 "MeaningId INT NOT NULL," +
                 "Meaning VARCHAR(MAX) NOT NULL" +
             ")" + SqlDelimiter + "\n";

            sql += "CREATE TABLE wiki_Translations (" +
                "ExpressionA VARCHAR(MAX) NOT NULL," +
                "ExpressionB VARCHAR(MAX) NOT NULL," +
                "LanguageA VARCHAR(MAX) NOT NULL," +
                "LanguageB VARCHAR(MAX) NOT NULL, " +
                "MeaningId INT NULL" +
            ")" + SqlDelimiter + "\n";

            if (SqlLite)
            {
                sql = sql.Replace("VARCHAR(MAX)", "VARCHAR");
            }

            return sql;
        }

        public override string SqlInsertStatement(DataProcessorContext context)
        {
            bool hasMeaning = !string.IsNullOrWhiteSpace(Meaning);

            StringBuilder sql = new StringBuilder();
            if (hasMeaning)
            {
                sql.Append("INSERT INTO wiki_TranslationMeanings VALUES(" + MeaningId + ", '" + Meaning.EscapeSql() + "')" + SqlDelimiter + "\n");
            }

            foreach (var translation in Translations)
            {
                sql.Append("INSERT INTO wiki_Translations VALUES ('" + translation.ExpressionA.EscapeSql() + "','" + translation.ExpressionB.EscapeSql() + "','" + translation.LanguageA + "','" + translation.LanguageB + "'," + (hasMeaning ? MeaningId.ToString() : "null") + ")" + SqlDelimiter + "\n");
            }

            return sql.ToString();
        }

        public override string PlainTextToString(DataProcessorContext context)
        {
            StringBuilder txt = new StringBuilder();

            foreach (var translation in Translations)
            {

                return string.Format("{0}{5}{1}{5}{2}{5}{3}{4}\n", translation.ExpressionA, translation.ExpressionB, translation.LanguageA, translation.LanguageB, Meaning, PlainTextDelimiter);
            }

            return txt.ToString();
        }
    }
}
