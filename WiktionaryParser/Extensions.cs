using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Memoling.Tools.WiktionaryParser
{
    public static class Extension
    {
        public static bool ReadToElement(this XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ReadToElement(this XmlReader reader, string name)
        {
            if (string.CompareOrdinal(reader.Name, name) == 0)
            {
                return true;
            }

            while (reader.ReadToElement())
            {
                if (string.CompareOrdinal(reader.Name, name) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        public static string RemoveRegexMatches(this string str, Regex pattern)
        {
            MatchCollection matches = pattern.Matches(str);
            StringBuilder sb = new StringBuilder(str);

            int removed = 0;
            foreach (Match match in matches)
            {
                sb.Remove(match.Index - removed, match.Length);
                removed += match.Length;
            }

            return sb.ToString();
        }

        public static string ReplaceRegexMatches(this string str, Regex pattern, MatchEvaluator replace) 
        {
            return pattern.Replace(str, replace);        
        }

        public static string EscapeSql(this string str) 
        {
            return str.Replace("'", "''");
        }
    }
}
