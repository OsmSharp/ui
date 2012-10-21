using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Data.PBF
{
    /// <summary>
    /// Consumers Osm PBF primitives.
    /// </summary>
    internal interface IPBFOsmPrimitiveConsumer
    {
        /// <summary>
        /// Processes the given node using the properties in the given block.
        /// </summary>
        /// <param name="node"></param>
        void ProcessNode(PrimitiveBlock block, Node node);

        /// <summary>
        /// Processes the given way using the properties in the given block.
        /// </summary>
        /// <param name="way"></param>
        void ProcessWay(PrimitiveBlock block, Way way);

        /// <summary>
        /// Processing the given relation using the properties in the given block.
        /// </summary>
        /// <param name="relation"></param>
        void ProcessRelation(PrimitiveBlock block, Relation relation);
    }
}
