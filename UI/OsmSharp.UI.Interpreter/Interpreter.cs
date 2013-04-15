// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Osm.Interpreter.Features;

namespace OsmSharp.Osm.Interpreter
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
