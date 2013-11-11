using System.Collections.Generic;
using System.IO;
using System.Linq;
using Memoling.Tools.WiktionaryParser.Data;

namespace Memoling.Tools.WiktionaryParser.Output
{
    public class OutputResult
    {
        private IEnumerable<string> headers;
        private StreamWriter stream;
        private OutputFormat format;
        private bool firstCall;

        public OutputResult(string header, StreamWriter stream, OutputFormat format)
            : this(new string[] { header }, stream, format)
        {
        }

        public OutputResult(IEnumerable<string> headers, StreamWriter stream, OutputFormat format)
        {
            this.firstCall = true;
            this.headers = headers;
            this.stream = stream;
            this.format = format;
        }

        public void Next(DataProcessorResult result)
        {
            foreach (var header in headers)
            {
                var sections = result.TransformedSections.Where(s => s.Header == header);
                foreach (var section in sections)
                {
                    dynamic obj = section.Content;
                    RecursiveProcess(result, obj);
                }
            }
        }

        private void RecursiveProcess(DataProcessorResult result, dynamic content)
        {
            var enumContents = content as IEnumerable<dynamic>;

            if (enumContents != null)
            {
                foreach (var enumContent in enumContents)
                {
                    RecursiveProcess(result, enumContent);
                }
            }
            else
            {
                Process(result, content);
            }
        }

        private void Process(DataProcessorResult result, dynamic content)
        {
            string data = null;
            switch (format)
            {
                case OutputFormat.Sql:
                    var sqlContent = (ISqlOutputResult)content;

                    if (firstCall)
                    {
                        data = sqlContent.SqlCreateStatement(result.Context);
                        data += sqlContent.SqlInsertStatement(result.Context);
                        firstCall = false;
                    }
                    else
                    {
                        data = sqlContent.SqlInsertStatement(result.Context);
                    }

                    break;
                case OutputFormat.Json:
                    var jsonContent = (IJsonOutputResult)content;

                    if (firstCall)
                    {
                        data = jsonContent.JsonNewDocument(result.Context);
                        data += jsonContent.JsonNewNode(result.Context);
                        firstCall = false;
                    }
                    else
                    {
                        data = jsonContent.JsonNewNode(result.Context);
                    }

                    break;
                case OutputFormat.Xml:
                    var xmlContent = (IXmlOutputResult)content;

                    if (firstCall)
                    {
                        data = xmlContent.XmlNewDocument(result.Context);
                        data += xmlContent.XmlNewNode(result.Context);
                        firstCall = false;
                    }
                    else
                    {
                        data = xmlContent.XmlNewNode(result.Context);
                    }

                    break;
                default:
                case OutputFormat.PlainText:
                    var txtContent = (IPlainTextOutputResult)content;

                    if (txtContent == null)
                    {
                        data = content.ToString();
                    }
                    else
                    {
                        data = txtContent.PlainTextToString(result.Context);
                    }
                    break;
            }

            stream.Write(data);
        }
    }
}
