using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Memoling.Tools.WiktionaryMapper.Data;
using Memoling.Tools.WiktionaryParser.Data;
using Memoling.Tools.WiktionaryParser.Section;
using Memoling.Tools.WiktionaryParser.Tag;
using Memoling.Tools.WiktionaryParser;

namespace Memoling.Tools.WiktionaryMapper
{
    // TODO: Make more generic
    public class DataProcessorConfigBuilder
    {
        public static DataProcessorConfig Build()
        {
            return new DataProcessorConfigBuilder().config;
        }

        private DataProcessorConfig config;
        private IEnumerable<TagTransformation> commonTagTransformations;

        public static string TranslationSection = "Translations";
        public static string[] PartsOfSpeechSections = new string[] { "Noun", "Verb", "Pronoun", "Adverb", "Adjective" };
        public static string SynonymSection = "Synonyms";
        public static string AnotonymSection = "Antonyms";

        private DataProcessorConfigBuilder()
        {
            BuildCommonTagTransformation();

            config = new DataProcessorConfig()
            {
                TopLevelSection = "English",
                Langauge = "en",
                RemoveSections = new List<string> { "English", "Pronunciation", "See also", "Derived terms", "Etymology", "Anagrams" },
                PartOfSpeech = new List<string> { "Noun", "Verb", "Pronoun", "Adverb", "Adjective" },
            };

            config.SectionTransformations = new List<SectionTransformation>()
            {
                new SectionTransformation() 
                {
                    Headers = new string[] { TranslationSection },
                    Transformation = TranslationWithMeaningTransformation
                },
                new SectionTransformation() 
                {
                    Headers =  PartsOfSpeechSections,
                    Transformation = PartsOfSpeechTransformation
                },
                new SectionTransformation() 
                {
                    Headers = new string[] { SynonymSection },
                    Transformation = SynonymsTransformation,
                },
                new SectionTransformation() 
                {
                    Headers = new string[] { AnotonymSection },
                    Transformation = AntonymsTransformation,
                },
            };
        }

        private void BuildCommonTagTransformation()
        {
            commonTagTransformations = new List<TagTransformation>()
            {
                // Remove image tags
                new TagTransformation()
                {
                    TagPattern = TagPatterns.Image,
                },
                // Remove audio tags
                new TagTransformation() 
                {
                    TagPattern = TagPatterns.Audio,
                },
                // Remove cite tags
                new TagTransformation() 
                {
                    TagPattern = TagPatterns.Cite
                },
                // Remove Category
                new TagTransformation() 
                {
                    TagPattern = TagPatterns.Category
                },
                // Remove Quotes
                new TagTransformation() 
                {
                    TagPattern = TagPatterns.Quote
                },
                // Remove explicit references
                new TagTransformation() 
                {
                    TagPattern = TagPatterns.ReferenceExplicit
                },
                // Remove picture dictionaries
                new TagTransformation() 
                {
                    TagPattern = TagPatterns.PictureDictionary
                },
                // Remove orphan lines
                new TagTransformation() 
                {
                    TagPattern = TagPatterns.OrphanLine
                },
                // Shorten tags
                new TagTransformation() 
                {
                    TagPattern = TagPatterns.Tag,
                    Transformation = (Match match) => 
                    {
                        return match.Value.Substring(1, match.Value.Length-2);
                    }
                },

                // --------------------------------------------- Cleaning

                // Remove empty enumerations
                new TagTransformation()
                {
                    TagPattern = TagPatterns.EmptyEnumeration,
                },
                // Remove 3 new lines in a row
                new TagTransformation() 
                {
                    TagPattern = TagPatterns.NL3
                },
                // Remove reference tag (as last)
                new TagTransformation() 
                {
                    TagPattern = TagPatterns.Reference,
                    Transformation = (Match match) => 
                    {
                        return match.Value.Substring(2,match.Value.Length-4);
                    }
                },

            };
        }

        private IEnumerable<TranslationsWithMeaning> TranslationWithMeaningTransformation(DataProcessorContext context, string header, string content)
        {
            var matches = TagPatterns.TranslationMeaning.Matches(content).Cast<Match>().ToList();

            if (matches.Count > 0)
            {

                for (int i = 0; i < matches.Count;i++ )
                {
                    Match match = matches[i];

                    // It should have following format {{...|meaning}}
                    string[] parts = match.Value.Split(new char[] { '|', '}' }, StringSplitOptions.RemoveEmptyEntries);

                    string matchContent = content.Substring(match.Index);
                    if (i + 1 < matches.Count)
                    {
                        // Not the last element
                        matchContent = content.Substring(0, matches[i + 1].Index - match.Index);
                    }

                    if (parts.Count() > 1)
                    {
                        string meaning = parts[1];
                        yield return new TranslationsWithMeaning()
                        {
                            MeaningId = TranslationsWithMeaning.NextId(),
                            Meaning = TagTransformation.Transform(meaning, commonTagTransformations).Trim(),
                            Translations = TranslationTransformation(context, header, matchContent)
                        };
                    }
                    else
                    {
                        // Unfortunately this happens too
                        yield return new TranslationsWithMeaning()
                        {
                            Translations = TranslationTransformation(context, header, matchContent)
                        };
                    }
                }
            }
            else
            {
                yield return new TranslationsWithMeaning()
                {
                    Translations = TranslationTransformation(context, header, content)
                };
            }
        }

        private IEnumerable<Translation> TranslationTransformation(DataProcessorContext context, string header, string content)
        {
            var matches = TagPatterns.Translation.Matches(content);
            foreach (Match match in matches)
            {
                string value = match.Value;
                string[] parts = value.Split('|');
                string language = parts[1];
                string translation = parts[2].Split('}')[0];
                translation = TagTransformation.Transform(translation, commonTagTransformations).Trim();

                yield return new Translation()
                {
                    ExpressionA = context.Title,
                    LanguageA = context.Config.Langauge,
                    ExpressionB = translation,
                    LanguageB = language
                };
            }
        }

        private IEnumerable<PartOfSpeech> PartsOfSpeechTransformation(DataProcessorContext context, string header, string content)
        {
            yield return new PartOfSpeech()
            {
                Expression = context.Title,
                Definition = TagTransformation.Transform(content, commonTagTransformations).Trim(),
                Language = context.Config.Langauge,
                Part = header,
            };
        }

        private IEnumerable<dynamic> SynonymsTransformation(DataProcessorContext context, string header, string content)
        {
            return EnumerationTransformation(context, header, content, (string value) =>
            {
                return new Synonym()
                {
                    ExpressionA = context.Title,
                    ExpressionB = value,
                    Language = context.Config.Langauge
                };
            });
        }

        private IEnumerable<dynamic> AntonymsTransformation(DataProcessorContext context, string header, string content)
        {
            return EnumerationTransformation(context, header, content, (string value) =>
            {
                return new Antonym()
                {
                    ExpressionA = context.Title,
                    ExpressionB = value,
                    Language = context.Config.Langauge
                };
            });
        }

        private IEnumerable<dynamic> EnumerationTransformation(DataProcessorContext context, string header, string content, Func<string, dynamic> instanceBuilder)
        {
            var items = content.Split(new char[] { '*', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in items)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    string modifiable = item;

                    // Sense not supported 
                    modifiable = modifiable.RemoveRegexMatches(TagPatterns.Sense);
                    // Wikisaurus not supported
                    modifiable = modifiable.RemoveRegexMatches(TagPatterns.Wikisaurus);

                    // Some values are separated by ',' and ';'
                    foreach (var val in modifiable.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        var match = TagPatterns.Garbage.Match(val);
                        if (match.Success && match.Length == val.Length)
                        {
                            continue;
                        }

                        yield return instanceBuilder(TagTransformation.Transform(val, commonTagTransformations).Trim());
                    }
                }
            }
        }
    }
}
