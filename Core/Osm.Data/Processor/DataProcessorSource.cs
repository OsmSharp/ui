using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Core.Simple;

namespace Osm.Data.Core.Processor
{
    /// <summary>
    /// Base class for any (streamable) source of osm data (Nodes, Ways and Relations).
    /// </summary>
    public abstract class DataProcessorSource
    {
        /// <summary>
        /// Creates a new source.
        /// </summary>
        public DataProcessorSource()
        {

        }

        /// <summary>
        /// Initializes this source.
        /// </summary>
        /// <param name="param"></param>
        public abstract void Initialize();
        
        /// <summary>
        /// Move to the next item in the stream.
        /// </summary>
        /// <returns></returns>
        public abstract bool MoveNext();

        /// <summary>
        /// Returns the current item in the stream.
        /// </summary>
        /// <returns></returns>
        public abstract SimpleOsmGeo Current();

        /// <summary>
        /// Resets the source to the beginning.
        /// </summary>
        public abstract void Reset();
    }
}
