using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Memoling.Tools.WiktionaryParser.Tag
{
    public class TagTransformation
    {
        public Regex TagPattern;
        public MatchEvaluator Transformation;

        public static string Transform(string data, IEnumerable<TagTransformation> transformations)
        {
            foreach (var transformation in transformations)
            {
                if (transformation.Transformation == null)
                {
                    data = data.RemoveRegexMatches(transformation.TagPattern);
                }
                else
                {
                    data = data.ReplaceRegexMatches(transformation.TagPattern, transformation.Transformation);
                }
            }

            return data;
        }
    }
}
