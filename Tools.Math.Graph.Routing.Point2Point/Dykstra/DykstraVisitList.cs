//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Tools.Math.Graph.Routing.Dykstra
//{
//    /// <summary>
//    /// Helper class for the Dykstra algorithm keeping a list of visited vertices.
//    /// </summary>
//    public class DykstraVisitList<EdgeType, VertexType>
//    {
//        private Dictionary<VertexType, VertexReference<EdgeType, VertexType>> _node_ref_idx;

//        private SortedList<float, List<VertexReference<EdgeType, VertexType>>> _list;

//        public DykstraVisitList()
//        {
//            _node_ref_idx = new Dictionary<VertexType, VertexReference<EdgeType, VertexType>>();
//            _list = new SortedList<float, List<VertexReference<EdgeType, VertexType>>>();
//        }

//        #region Add

//        /// <summary>
//        /// Adds a node to the sorted list.
//        /// </summary>
//        /// <param name="list"></param>
//        /// <param name="weight"></param>
//        /// <param name="node"></param>
//        public void AddNode(
//            float weight,
//            VertexReference<EdgeType, VertexType> node)
//        {
//            List<VertexReference<EdgeType, VertexType>> value;
//            if (_list.TryGetValue(weight, out value))
//            {

//            }
//            else
//            {
//                value = new List<VertexReference<EdgeType, VertexType>>();
//                _list.Add(weight, value);
//            }
//            if (_node_ref_idx.ContainsKey(node.Vertex))
//            {
//                _node_ref_idx[node.Vertex] = node;
//            }
//            else
//            {
//                _node_ref_idx.Add(node.Vertex, node);
//            }
//            value.Add(node);
//        }

//        /// <summary>
//        /// Adds nodes to the sorted list.
//        /// </summary>
//        /// <param name="list"></param>
//        /// <param name="weight"></param>
//        /// <param name="node"></param>
//        public void AddNodes(
//            float weight,
//            List<VertexReference<EdgeType, VertexType>> nodes)
//        {
//            List<VertexReference<EdgeType, VertexType>> value;
//            if (_list.TryGetValue(weight, out value))
//            {

//            }
//            else
//            {
//                value = new List<VertexReference<EdgeType, VertexType>>();
//                _list.Add(weight, value);
//            }
//            value.AddRange(nodes);
//            foreach (VertexReference<EdgeType, VertexType> node in nodes)
//            {
//                if (_node_ref_idx.ContainsKey(node.Vertex))
//                {
//                    _node_ref_idx[node.Vertex] = node;
//                }
//                else
//                {
//                    _node_ref_idx.Add(node.Vertex, node);
//                }
//            }
//        }

//        /// <summary>
//        /// Adds nodes to the sorted list.
//        /// </summary>
//        /// <param name="list"></param>
//        /// <param name="weight"></param>
//        /// <param name="node"></param>
//        public void AddAll(
//            SortedList<float, List<VertexReference<EdgeType, VertexType>>> list_to_add)
//        {
//            foreach (KeyValuePair<float, List<VertexReference<EdgeType, VertexType>>> pair in list_to_add)
//            {
//                this.AddNodes(pair.Key, pair.Value);
//            }
//        }

//        #endregion

//        #region Remove

//        /// <summary>
//        /// Removes a node from the visit list.
//        /// </summary>
//        /// <param name="weight"></param>
//        /// <param name="node"></param>
//        public void RemoveNode(
//            float weight,
//            VertexReference<EdgeType, VertexType> node)
//        {
//            List<VertexReference<EdgeType, VertexType>> value;
//            if (_list.TryGetValue(weight, out value))
//            {
//                value.Remove(node);
//                _node_ref_idx.Remove(node.Vertex);
//                if (value.Count == 0)
//                {
//                    _list.Remove(weight);
//                }
//            }
//            else
//            {
//                throw new ArgumentOutOfRangeException("Could not remove node!");
//            }
//        }

//        #endregion

//        #region Get

//        #region Get References

//        /// <summary>
//        /// Returns the reference for a given node.
//        /// </summary>
//        /// <param name="node"></param>
//        /// <returns></returns>
//        public VertexReference<EdgeType, VertexType> GetRef(VertexType node)
//        {
//            return _node_ref_idx[node];
//        }

//        /// <summary>
//        /// Returns true if the ref_val was found.
//        /// </summary>
//        /// <param name="node"></param>
//        /// <param name="ref_val"></param>
//        /// <returns></returns>
//        public bool TryGetRef(VertexType node, out VertexReference<EdgeType, VertexType> ref_val)
//        {
//            return _node_ref_idx.TryGetValue(node, out ref_val);
//        }

//        #endregion

//        #region Get First

//        /// <summary>
//        /// Returns the very first reference entry in this list.
//        /// </summary>
//        /// <returns></returns>
//        public VertexReference<EdgeType, VertexType> GetFirst()
//        {
//            List<VertexReference<EdgeType, VertexType>> refs = _list[_list.Keys[0]];
//            return refs[0];
//        }

//        #endregion

//        /// <summary>
//        /// Returns all vertexes in this list.
//        /// </summary>
//        /// <returns></returns>
//        public List<VertexReference<EdgeType, VertexType>> GetAllReferences()
//        {
//            List<VertexReference<EdgeType, VertexType>> list = new List<VertexReference<EdgeType,VertexType>>();

//            foreach(KeyValuePair<float, List<VertexReference<EdgeType, VertexType>>> pair in _list)
//            {
//                list.AddRange(pair.Value);
//            }

//            return list;
//        }

//        /// <summary>
//        /// Returns the element count in this list.
//        /// </summary>
//        public int Count
//        {
//            get
//            {
//                return _list.Count;
//            }
//        }

//        #endregion

//        /// <summary>
//        /// Intersects this list with the other list and returns the common elements.
//        /// </summary>
//        /// <param name="backward_visit_list"></param>
//        /// <returns></returns>
//        public IList<KeyValuePair<VertexReference<EdgeType, VertexType>, VertexReference<EdgeType, VertexType>>> Intersect(
//            DykstraVisitList<EdgeType, VertexType> visit_list)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
