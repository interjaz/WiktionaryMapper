using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Memoling.Tools.WiktionaryParser.Tag;

namespace Memoling.Tools.WiktionaryParser.Data
{
    public class DataProcessor : IDataProcessor
    {
        public int TopSectionLevel = 2;

        DataProcessorConfig config;

        public DataProcessor(DataProcessorConfig config)
        {
            this.config = config;
        }

        public DataProcessorResult Next(string title, string page)
        {
            // Prepare page
            RemoveCategoryReferences(ref page, title);
            RemoveXmlComments(ref page);
            DataProcessorContext context = new DataProcessorContext(config, title, page);

            var sectionInfos = GetSectionsInfo(page).ToList();

            RemoveConfigSections(sectionInfos);
            RemoveInvalidTopSections(sectionInfos);

            if (sectionInfos.Count == 0)
            {
                // Failed to parse
                return null;
            }

            // Enumerate and save (allow querying result)
            var transformedSections = TransformSections(context, sectionInfos).ToList();
            var extraSections = ExtraSections(context, sectionInfos).ToList();

            return new DataProcessorResult()
            {
                Context = context,
                TransformedSections = transformedSections,
                ExtraSections = extraSections
            };
        }

        private void RemoveXmlComments(ref string page)
        {
            page = page.RemoveRegexMatches(TagPatterns.XmlComments);
        }

        private void RemoveCategoryReferences(ref string page, string title)
        {
            Regex pattern = new Regex(@"\[\[[a-z-]+:" + Regex.Escape(title) + @"\]\]");
            page = page.RemoveRegexMatches(pattern);
        }

        private void RemoveConfigSections(ICollection<SectionInfo> sections)
        {
            foreach (var toRemove in config.RemoveSections)
            {
                var section = sections.FirstOrDefault(s => s.Header == toRemove);
                if (section != null)
                {
                    sections.Remove(section);
                }
            }
        }

        private void RemoveInvalidTopSections(ICollection<SectionInfo> sections)
        {
            for (int i = 0; i < sections.Count;i++ )
            {
                var section = sections.ElementAt(i);
                if (section.Level == TopSectionLevel && section.Header != config.TopLevelSection)
                {
                    // Remove all section from i

                    for (int j = sections.Count-1; j >= i; j--)
                    {
                        sections.Remove(sections.ElementAt(j));
                    }
                    
                    return;
                }
            }
        }
        
        private IEnumerable<SectionInfo> GetSectionsInfo(string text)
        {
            List<Match> matches = new List<Match>();
            foreach (Match match in TagPatterns.Section.Matches(text))
            {
                matches.Add(match);
            }

            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];
                string value = match.ToString();
                int level = value.Count(c => c == '=') / 2;

                int index = match.Index + match.Length;
                int nextMatchIndex;
                if (i < matches.Count - 1)
                {
                    nextMatchIndex = matches[i + 1].Index;
                }
                else
                {
                    nextMatchIndex = text.Length;
                }

                yield return new SectionInfo()
                {
                    Header = value.Substring(level, value.Length - level * 2),
                    Level = level,
                    ContentIndex = index,
                    ContentLength = nextMatchIndex - index,
                };
            }
        }

        private IEnumerable<Section> TransformSections(DataProcessorContext context, ICollection<SectionInfo> sectionInfos)
        {
            foreach (var sectionTransformation in config.SectionTransformations)
            {
                foreach (var header in sectionTransformation.Headers)
                {
                    // sectionInfos will be modified, so we need a copy of original collection
                    var sectionsToTransform = sectionInfos.Where(s => s.Header == header).ToList();
                    if (sectionsToTransform.Count == 0)
                    {
                        continue;
                    }

                    foreach (var section in sectionsToTransform)
                    {
                        sectionInfos.Remove(section);
                        var content = context.Page.Substring(section.ContentIndex, section.ContentLength).Trim();
                        yield return new Section()
                        {
                            Header = section.Header,
                            Content = sectionTransformation.Transformation(context, section.Header, content)
                        };
                    }
                }
            }
        }

        private IEnumerable<Section> ExtraSections(DataProcessorContext context, IEnumerable<SectionInfo> sectionInfos)
        {
            foreach (var section in sectionInfos)
            {
                yield return new Section()
                {
                    Header = section.Header,
                    Content = context.Page.Substring(section.ContentIndex, section.ContentLength).Trim()
                };
            }
        }
    }
}
