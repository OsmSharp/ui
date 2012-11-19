using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Routing.Core.Interpreter;
using OsmSharp.Routing.Core.Roads.Tags;
using OsmSharp.Routing.Core.Constraints;
using OsmSharp.Routing.Core.Interpreter.Roads;
using OsmSharp.Routing.Core.Constraints.Highways;

namespace OsmSharp.Osm.Routing.Interpreter
{
    /// <summary>
    /// A routing interpreter for OSM data.
    /// </summary>
    public class OsmRoutingInterpreter : IRoutingInterpreter
    {
        /// <summary>
        /// Holds the edge interpreter.
        /// </summary>
        private IEdgeInterpreter _edge_interpreter;

        /// <summary>
        /// Holds the routing constraints.
        /// </summary>
        private IRoutingConstraints _constraints;

        /// <summary>
        /// Creates a new routing intepreter with default settings.
        /// </summary>
        public OsmRoutingInterpreter()
        {
            _edge_interpreter = new Edge.EdgeInterpreter();
            _constraints = null;
            //_constraints = new DefaultHighwayConstraints(_edge_interpreter);            
        }

        /// <summary>
        /// Creates a new routing interpreter with given constraints.
        /// </summary>
        /// <param name="constraints"></param>
        public OsmRoutingInterpreter(IRoutingConstraints constraints)
        {
            _edge_interpreter = new Edge.EdgeInterpreter();
            _constraints = constraints;   
        }

        /// <summary>
        /// Returns true if the given vertices can be traversed in the given order.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="along"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public bool CanBeTraversed(uint from, uint along, uint to)
        {
            return true;
        }

        /// <summary>
        /// Returns and edge interpreter.
        /// </summary>
        public IEdgeInterpreter EdgeInterpreter
        {
            get 
            {
                return _edge_interpreter; 
            }
        }

        /// <summary>
        /// Returns the constraints.
        /// </summary>
        public IRoutingConstraints Constraints
        {
            get
            {
                return _constraints;
            }
        }
    }
}