using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Core;
using Osm.Interpreter.Features;

namespace Osm.Interpreter
{
    /// <summary>
    /// And osm geo interpreter.
    /// </summary>
    public class Interpreter
    {
        /// <summary>
        /// The root of all features.
        /// </summary>
        private Feature _root_feature;

        /// <summary>
        /// Creates a new interpreter.
        /// </summary>
        public Interpreter()
        {
            // load and instantiate the features.
                        
        }

        /// <summary>
        /// Interprets the object and reports the findings to the client.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="geo"></param>
        public void Interpret(IFeatureClient client, OsmGeo geo)
        {
            // call the onbefore function
            client.OnBeforeInterpretation(geo);

            // interpret the object.
            bool succes = _root_feature.Interpret(geo,client);

            // call the onafter function
            client.OnAfterInterpretation(succes,geo);
        }
    }
}
