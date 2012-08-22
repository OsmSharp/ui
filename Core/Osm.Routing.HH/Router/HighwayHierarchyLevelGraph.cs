//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Osm.Routing.HH.Highways;
//using Osm.Routing.Graphs;

//namespace Osm.Routing.HH.Router
//{
//    public class HighwayHierarchyLevelGraph : Tools.Math.Graph.Graph<HighwayEdge, GraphVertex>
//    {
//        private int _level;

//        private IHighwayHierarchy _highway_hierarchy;

//        public HighwayHierarchyLevelGraph(IHighwayHierarchy highway_hierarchy, int level)
//            :base(new HighwayHierarchyGraphInterpreter())
//        {
//            _level = level;
//            _highway_hierarchy = highway_hierarchy;
//        }

//        public bool ContainsVertex(GraphVertex vertex)
//        {
//            return _highway_hierarchy.ContainsVertex(_level, vertex);
//        }

//        public override IList<HighwayEdge> GetEdgesForVertex(GraphVertex vertex)
//        {
//            return new List<HighwayEdge>(_highway_hierarchy.GetNeigbours(_level, vertex, null));
//        }

//        public override IList<GraphVertex> GetNeighbourVerticesOnEdge(HighwayEdge edge, GraphVertex vertex)
//        {
//            List<GraphVertex> neighbours = new List<GraphVertex>();
//            if (edge.From == vertex)
//            {
//                neighbours.Add(edge.To);
//            }
//            return neighbours;
//        }
//    }
//}
