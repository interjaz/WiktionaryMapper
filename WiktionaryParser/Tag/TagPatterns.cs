using System.Text.RegularExpressions;

namespace Memoling.Tools.WiktionaryParser.Tag
{
    public static class TagPatterns
    {
        public static Regex Image = new Regex(@"\[\[Image:[^\]\]]+\]\]");
        public static Regex Audio = new Regex(@"{{audio[^}}]+}}");
        public static Regex EmptyEnumeration = new Regex(@".*\*[ ]*\n");
        public static Regex NL3 = new Regex(@"\n\n\n");
        public static Regex Cite = new Regex(@"{{seeCites}}");
        public static Regex Section = new Regex(@"=[=]+[^=]+[=]+=");
        public static Regex TranslationMeaning = new Regex(@"{{trans-top\|[^}}]*}}");
        public static Regex Translation = new Regex(@"{{t.\|[a-z]+\|.*\n");
        public static Regex Category = new Regex(@"\[\[Categor[^\]\]]*\]\]");
        public static Regex Tag = new Regex(@"{{[^}}]+}}");
        public static Regex XmlComments = new Regex(@"<!--[^\-\->]*-->");
        public static Regex Sense = new Regex(@"{{sense|[^}}]*}}");
        public static Regex Wikisaurus = new Regex(@".*\[\[Wikisaurus:[^\]\]]*\]\]");

        public static Regex Label = new Regex(@"{{label\|[^}}]+}}");
        public static Regex Reference = new Regex(@"\[\[[^\]\]]+]]");

        public static Regex Quote = new Regex(@"{{quote[^}]*}}");
        public static Regex ReferenceExplicit = new Regex(@"{{reference[^}}]*}}");
        public static Regex PictureDictionary = new Regex(@"{{ picdic[^}}]*}}");

        // This one needs to be used with care, otherwise it may remove important characters.
        // Example when it can be used is when you compare whether match length equals input length (only garbage)
        public static Regex Garbage = new Regex("[~`!@#$%^&*()0-9-_=+{\\[}\\]|\\\\:;\"'<,>.?/ ]*");

        public static Regex OrphanLine = new Regex(@"[-]+$");
    }
}
