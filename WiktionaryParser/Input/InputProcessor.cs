using System.Collections.Generic;
using System.IO;
using System.Xml;
using Memoling.Tools.WiktionaryParser.Data;

namespace Memoling.Tools.WiktionaryParser.Input
{
    public class InputProcessor
    {
        const string XmlNsGeneral = "4";
        const string XmlNsContent = "0";
        const string PageElementName = "page";
        const string TitleElementName = "title";
        const string NamespaceElementName = "ns";
        const string TextElementName = "text";
        
        XmlReaderSettings settings;
        XmlReader reader;
        IDataProcessor processor;

        public InputProcessor(IDataProcessor processor) 
        {
            settings = new XmlReaderSettings() { IgnoreWhitespace = true, IgnoreProcessingInstructions = true, IgnoreComments = true };
            this.processor = processor;
        }
       
        public IEnumerable<DataProcessorResult> Process(StreamReader stream)
        {
            reader = XmlReader.Create(stream, settings);
            reader.MoveToContent();

            string text = "";
            string title = "";

            while (reader.ReadToElement(PageElementName))
            {
                // Useful for debugging
                //var elem = XElement.ReadFrom(reader) as XElement;

                reader.ReadToElement(TitleElementName);
                title = reader.ReadElementContentAsString();

                reader.ReadToElement(NamespaceElementName);
                if (string.CompareOrdinal(reader.ReadElementContentAsString(), XmlNsContent) == 0)
                {
                    reader.ReadToElement(TextElementName);
                    text = reader.ReadElementContentAsString();

                    yield return processor.Next(title, text);
                }
            }
        }
    }
}
