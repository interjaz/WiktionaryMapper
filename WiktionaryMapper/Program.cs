using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Memoling.Tools.WiktionaryMapper.Data;
using Memoling.Tools.WiktionaryParser.Data;
using Memoling.Tools.WiktionaryParser.Input;
using Memoling.Tools.WiktionaryParser.Output;

namespace Memoling.Tools.WiktionaryMapper
{
    class Program
    {
        static StreamCollection streamCollection;
        static StreamReader source;
        static OutputProcessor outputProcessor;
        static Stopwatch stopwatch;
        static int processedEntry;
        static StreamUris uris;

        static void Init()
        {
            streamCollection = new StreamCollection();
            outputProcessor = new OutputProcessor();
            stopwatch = new Stopwatch();
        }

        static void Main(string[] args)
        {
            Init();
            ParseArgs(args);

            IDataProcessor data = CreateDataProcessor();
            InputProcessor inputProcessor = new InputProcessor(data);
            
            PrintImportStarted();

            try
            {
                using (streamCollection)
                {
                    int i = 0;
                    foreach (var result in inputProcessor.Process(source))
                    {
                        // If failed to Parse
                        if (result == null)
                        {
                            continue;
                        }

                        if (i++ > 100) break;
                
                        outputProcessor.Next(result);
                        PrintProgress();
                    }
                }

                PrintOptimizationStarted();

                Optimize(uris.Synonims);
                //Optimize(uris.Antonyms);

                PrintFinished();
            }
            catch (Exception ex)
            {
                PrintException(ex);
            }
        }

        static void ParseArgs(string[] args)
        {
            // Parse data - for now define it manually
            uris.Source.Uri = @"\\BARTOSZ-HP\Share\enwiktionary-20130202-pages-articles-multistream.xml";
            uris.Translations.Uri = @"\\BARTOSZ-HP\Share\trans.sql";
            uris.Definitions.Uri = @"\\BARTOSZ-HP\Share\defs.sql";
            uris.Synonims.Uri = @"\\BARTOSZ-HP\Share\syns.sql";
            uris.Temp.Uri = @"\\BARTOSZ-HP\Share\synsUniq.sql";

            //uris.Source.Uri = @"D:\Share\enwiktionary-20130202-pages-articles-multistream.xml";
            //uris.Translations.Uri = @"D:\Share\trans.sql";
            //uris.Definitions.Uri = @"D:\Share\defs.sql";
            //uris.Synonims.Uri = @"D:\Share\syns.sql";
            //uris.Temp.Uri = @"D:\Share\synsUniq.sql";

            uris.Translations.Format = OutputFormat.Sql;
            uris.Definitions.Format = OutputFormat.Sql;
            uris.Synonims.Format = OutputFormat.Sql;
            uris.Temp.Format = OutputFormat.Sql;

            source = new StreamReader(uris.Source.Uri);
            streamCollection.Add(source);

            AddOutputStream(uris.Translations, DataProcessorConfigBuilder.TranslationSection);
            AddOutputStream(uris.Definitions, DataProcessorConfigBuilder.PartsOfSpeechSections);
            AddOutputStream(uris.Synonims, DataProcessorConfigBuilder.SynonymSection);
        }

        static void AddOutputStream(StreamUris.StreamDetail detail, string header) 
        {
            AddOutputStream(detail, new string[] { header });
        }

        static void AddOutputStream(StreamUris.StreamDetail detail, string[] headers)
        {
            var stream = new StreamWriter(detail.Uri);
            outputProcessor.Outputs.Add(new OutputResult(headers, stream, detail.Format));
            streamCollection.Add(stream);
        }

        static IDataProcessor CreateDataProcessor()
        {
            var config = DataProcessorConfigBuilder.Build();
            return new DataProcessor(config);
        }

        static void Optimize(StreamUris.StreamDetail detail)
        {
            using (StreamReader sr = new StreamReader(detail.Uri))
            using (StreamWriter sw = new StreamWriter(uris.Temp.Uri))
            {
                BinaryExpressionOutputResult.RemoveDuplicates(sr, sw, detail.Format);
            }

            File.Delete(detail.Uri);
            File.Move(uris.Temp.Uri, detail.Uri);
        }

        #region Printing & UI

        static void PrintImportStarted()
        {
            Console.WriteLine("Importing");
            stopwatch.Start();
            processedEntry = 0;
        }

        static void PrintProgress()
        {
            processedEntry++;
            if (processedEntry % 100 == 0)
            {
                Console.Write("\r{0} ", processedEntry);
            }
        }

        static void PrintOptimizationStarted()
        {
            Console.WriteLine(stopwatch.Elapsed.ToString());
            Console.WriteLine();
            Console.WriteLine("Optimizing");
            stopwatch.Reset();
            stopwatch.Start();
        }

        static void PrintFinished()
        {
            Console.WriteLine();
            Console.WriteLine(stopwatch.Elapsed.ToString());
            Console.WriteLine("Done");
            Console.Read();
        }

        static void PrintException(Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine(ex.ToString());
            Console.Read();
        }

        #endregion

        struct StreamUris
        {
            public struct StreamDetail
            {
                public string Uri;
                public OutputFormat Format;
            }

            public StreamDetail Source;
            public StreamDetail Definitions;
            public StreamDetail Translations;
            public StreamDetail Synonims;
            public StreamDetail Antonyms;
            public StreamDetail Temp;
        }

    }
}
