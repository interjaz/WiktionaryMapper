using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;

namespace Memoling.Tools.SqliteBatchImport
{
    class SqlWrapper : IDisposable
    {
        public SQLiteConnection connection;

        public void Connect(string path)
        {
            var connStrBuilder = new SQLiteConnectionStringBuilder();
            connStrBuilder.FullUri  = path;

            connection = new SQLiteConnection(connStrBuilder.ConnectionString);
            connection.Open();
        }

        public void ExecuteNonQuery(string query, SQLiteTransaction trans, params Tuple<string,string>[] parameters)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(query, connection,trans))
            {
                foreach (var p in parameters)
                {
                    cmd.Parameters.Add(p.Item1, DbType.String).Value = p.Item2;
                }

                cmd.ExecuteNonQuery();
            }
        }

        public void Dispose()
        {
            Close();
        }

        public void Close()
        {
            connection.Clone();
            connection.Dispose();
        }

    }
}
