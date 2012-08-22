using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Data.Core.Processor;
using Osm.Routing.CH.PreProcessing;
using Osm.Core.Simple;

namespace Osm.Routing.CH.Processor
{
    /// <summary>
    /// Data processor target for osm data that will be converted into a Contracted Graph.
    /// </summary>
    public class CHDataProcessorTarget : DataProcessorTarget
    {
        /// <summary>
        /// The sparse pre-processor.
        /// </summary>
        private CHPreProcessor _pre_processor;

        /// <summary>
        /// The highway nodes.
        /// </summary>
        private HashSet<long> _highway_nodes;

        /// <summary>
        /// Creates a CH target with a CH preprocessor.
        /// </summary>
        /// <param name="pre_processor"></param>
        public CHDataProcessorTarget(CHPreProcessor pre_processor)
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

        private int _levels = 0;

        public int Levels
        {
            get
            {
                return _levels;
            }
        }

        public override void AddWay(SimpleWay way)
        {
            _pre_processor.Process(way, SimpleChangeType.Create);

            if (way.Tags != null && 
                way.Tags.ContainsKey("highway"))
            {
                foreach (long id in way.Nodes)
                {
                    _highway_nodes.Add(id);
                    _levels = _highway_nodes.Count;
                }
            }
        }

        public override void AddRelation(SimpleRelation relation)
        {

        }

        public override void Close()
        {
            _pre_processor.Start(_highway_nodes.GetEnumerator());
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
