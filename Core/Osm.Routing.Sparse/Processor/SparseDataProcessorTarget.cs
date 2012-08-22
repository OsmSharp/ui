using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Data.Core.Processor;
using Osm.Data.Core.Sparse;
using Osm.Core.Simple;
using Osm.Data.Core.Sparse.Primitives;
using Tools.Math.Geo;
using Osm.Routing.Core.Roads.Tags;
using Osm.Routing.Sparse.PreProcessor;

namespace Osm.Routing.Sparse.Processor
{
    /// <summary>
    /// Data processor target for osm data that will be converted into a sparse routable graph.
    /// </summary>
    public class SparseDataProcessorTarget : DataProcessorTarget
    {
        /// <summary>
        /// The sparse pre-processor.
        /// </summary>
        private SparsePreProcessor _pre_processor;

        /// <summary>
        /// Creates a redis sparse target with a redis sparse data source.
        /// </summary>
        /// <param name="pre_processor"></param>
        public SparseDataProcessorTarget(SparsePreProcessor pre_processor)
        {
            _pre_processor = pre_processor;
            _highway_nodes = new HashSet<long>();
        }

        public override void Initialize()
        {

        }

        public override void ApplyChange(SimpleChangeSet change)
        {
            throw new NotImplementedException();
        }

        public override void AddNode(SimpleNode node)
        {
            _pre_processor.Process(node, SimpleChangeType.Create);
        }

        /// <summary>
        /// The highway nodes.
        /// </summary>
        private HashSet<long> _highway_nodes;


        public override void AddWay(SimpleWay way)
        {
            _pre_processor.Process(way, SimpleChangeType.Create);

            if (way.Tags != null &&
                way.Tags.ContainsKey("highway"))
            {
                foreach (long id in way.Nodes)
                {
                    _highway_nodes.Add(id);
                }
            }
        }

        public override void AddRelation(SimpleRelation relation)
        {

        }

        public HashSet<long> ProcessedNodes
        {
            get
            {
                return _highway_nodes;
            }
        }
    }
}
