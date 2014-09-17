using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.IO.MemoryMappedFiles.Generic
{
    public class GenericMemoryMappedFile<T> : IMemoryMappedFile<T>
        where T : struct
    {
        public IMemoryMappedViewAccessor<T> CreateViewAccessor(long offset, long size)
        {
            throw new NotImplementedException();
        }

        public long GetSizeOf()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}