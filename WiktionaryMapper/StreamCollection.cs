using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memoling.Tools.WiktionaryMapper
{
    /// <summary>
    /// Collection for StreamReaders and StreamWriters
    /// </summary>
    public class StreamCollection : ICollection<IDisposable>, IDisposable
    {
        private List<IDisposable> streams;

        public StreamCollection()
        {
            streams = new List<IDisposable>();
        }

        public void Dispose()
        {
            foreach (var stream in streams)
            {
                stream.Dispose();
            }
        }

        public void Add(IDisposable item)
        {
            streams.Add(item);
        }

        public void Clear()
        {
            streams.Clear();
        }

        public bool Contains(IDisposable item)
        {
            return streams.Contains(item);
        }

        public void CopyTo(IDisposable[] array, int arrayIndex)
        {
            streams.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return streams.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(IDisposable item)
        {
            return streams.Remove(item);
        }

        public IEnumerator<IDisposable> GetEnumerator()
        {
            return streams.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return streams.GetEnumerator();
        }
    }
}
