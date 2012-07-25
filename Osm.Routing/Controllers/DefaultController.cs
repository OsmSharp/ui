//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Osm.Core;
//using Tools.Math.Graph.Routing.Point2Point;
//using Tools.Math.Geo;
//using Tools.Math.Graph.Routing.Point2Point.Dykstra;
//using Tools.Math.Graph;
//using Osm.Routing.Raw.Graphs;

//namespace Osm.Routing.Raw.Contollers
//{
//    public class DefaultController : IPoint2PointRouterController<Way, GraphVertex>
//    {
//        private float _factor;

//        private GeoCoordinateBox _routing_box;

//        private GeoCoordinateBox _selected_nodes_box;

//        public DefaultController(float factor)
//        {
//            _factor = factor;
//        }

//        #region IPoint2PointRouterController<Way,Node> Members

//        public void NotifyRoutingAlgorithm(RoutingAlgorithmsEnum algorithm)
//        {
//            if (algorithm != RoutingAlgorithmsEnum.Dykstra)
//            {
//                throw new Exception("Invalid controller!");
//            }
//        }

//        public void NotifyVertexSelected(VertexReference<Way, GraphVertex> vertex)
//        {
//            List<GeoCoordinate> coordinates = null;
//            if (_selected_nodes_box == null)
//            {
//                coordinates = new List<GeoCoordinate>();
//            }
//            else
//            {
//                coordinates = _selected_nodes_box.Corners.ToList<GeoCoordinate>();
//            }
//            coordinates.Add(vertex.Vertex.Coordinate);
//            _selected_nodes_box = new GeoCoordinateBox(coordinates.ToArray());
//        }

//        public void NotifyVerticesToRoute(List<GraphVertex> vertices)
//        {
//            List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
//            foreach (GraphVertex node in vertices)
//            {
//                coordinates.Add(node.Coordinate);
//            }
//            _routing_box = new GeoCoordinateBox(coordinates.ToArray());
//            //_selected_nodes_box = _routing_box;
//        }

//        public bool QueryStopConditions()
//        {
//            if (_selected_nodes_box != null && _routing_box != null)
//            {
//                if (_selected_nodes_box.IsInside(_routing_box))
//                {
//                    float new_factor = (float)(_selected_nodes_box.Surface / _routing_box.Surface);
//                    if (new_factor >= _factor)
//                    {
//                        return true;
//                    }
//                }
//            }
//            return false;
//        }

//        public void NotifyRoutesNotFound(GraphVertex from, List<GraphVertex> to)
//        {

//        }

//        #endregion

//        #region IPoint2PointRouterController<Way,GraphVertex> Members

//        public void NotifyFound(Tools.Math.Graph.Routing.GraphRoute<Way, GraphVertex> route)
//        {

//        }

//        public void NotifyGraph(Graph<Way, GraphVertex> graph)
//        {

//        }

//        #endregion
//    }
//}
