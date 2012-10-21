using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Data.Core.Processor;
using Osm.Core.Simple;

namespace Osm.Data.Core.Raw.Memory
{
    /// <summary>
    /// A memory data source processor target.
    /// </summary>
    internal class MemoryDataSourceProcessorTarget : DataProcessorTarget
    {
        /// <summary>
        /// Holds the memory data source.
        /// </summary>
        private MemoryDataSource _source;

        /// <summary>
        /// Creates a memory data processor target.
        /// </summary>
        /// <param name="source"></param>
        public MemoryDataSourceProcessorTarget(MemoryDataSource source)
        {
            _source = source;
        }

        /// <summary>
        /// Initializes this target.
        /// </summary>
        public override void Initialize()
        {

        }

        /// <summary>
        /// Applying changesets is not supported.
        /// </summary>
        /// <param name="change"></param>
        public override void ApplyChange(SimpleChangeSet change)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Adds a given node.
        /// </summary>
        /// <param name="node"></param>
        public override void AddNode(SimpleNode node)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a given way.
        /// </summary>
        /// <param name="way"></param>
        public override void AddWay(SimpleWay way)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a given relation.
        /// </summary>
        /// <param name="relation"></param>
        public override void AddRelation(SimpleRelation relation)
        {
            throw new NotImplementedException();
        }
    }
}
