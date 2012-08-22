using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Core.Simple;

namespace Osm.Data.Core.Processor.ChangeSets
{
    public abstract class DataProcessorChangeSetSource
    {
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
        public abstract SimpleChangeSet Current();

        /// <summary>
        /// Resets the source to the beginning.
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Closes this source.
        /// </summary>
        public abstract void Close();

    }
}
