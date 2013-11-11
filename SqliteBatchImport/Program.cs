using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Memoling.Tools.SqliteBatchImport
{
    class Program
    {
        private static FileInfo importSql;
        private static FileInfo exportDb;
        private static bool force = false;
        private static bool quite = false;
        private static string delimiter = ";\n";

        static void Main(string[] args)
        {
            ParseArgs(args);
            Regex pattern = new Regex(delimiter);

            using (SqlWrapper wrapper = new SqlWrapper())
            {
                wrapper.Connect(exportDb.FullName);
                using (StreamReader sr = new StreamReader(importSql.FullName))
                using (SQLiteTransaction trans = wrapper.connection.BeginTransaction())
                {

                    StringBuilder sb = new StringBuilder();
                    while (!sr.EndOfStream)
                    {
                        string sql = "";
                        string line = sr.ReadLine() + "\n";

                        if (pattern.IsMatch(line))
                        {
                            foreach (Match match in pattern.Matches(line))
                            {
                                sb.Append(line.Substring(0, match.Index));
                                bool insideQuotes = sb.ToString().Count(c => c == '\'') % 2 != 0;

                                if (insideQuotes)
                                {
                                    sb.Append(line.Substring(match.Index, line.Length - match.Index));
                                }
                                else
                                {
                                    try
                                    {
                                        sql = sb.ToString();
                                        wrapper.ExecuteNonQuery(sql, null);
                                        sb.Clear();
                                    }
                                    catch (Exception ex)
                                    {
                                        PrintException(ex, sql);

                                        if (!force)
                                        {
                                            trans.Rollback();
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            sb.Append(line);
                        }
                    }

                    trans.Commit();
                }
            }
        }

        static void ParseArgs(string[] args)
        {
            if (args.Count() == 0)
            {
                Console.WriteLine("SqlBatchImport to_import.sql database.sqlite [flag1 flag2 ...]");
                Console.WriteLine("flag:");
                Console.WriteLine("-f\tForce execution - skip errors (prints out exception and sql)");
                Console.WriteLine("-q\tQuite - do not print anything");
                Console.WriteLine("Mind this tool has relatively limited capabilities. Delimiter: ;\n");
                
                Environment.Exit(0);
            }


            importSql = new FileInfo(args[0]);
            exportDb = new FileInfo(args[1]);
            force = false;
            quite = false;

            for (int i = 2; i < args.Count(); i++)
            {
                string val = args[i];
                if (val.Contains("f"))
                {
                    force = true;
                }
                if (val.Contains("q"))
                {
                    quite = true;
                }
            }
        }

        static void PrintException(Exception ex, string sql)
        {
            if (quite)
            {
                return;
            }

            Console.WriteLine("---------- Exception -------------");
            Console.WriteLine(ex);
            Console.WriteLine("------------- SQL ----------------");
            Console.WriteLine(sql);
        }

    }
}
