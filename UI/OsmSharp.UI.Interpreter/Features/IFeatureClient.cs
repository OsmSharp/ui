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

namespace OsmSharp.Osm.Interpreter.Features
{
    /// <summary>
    /// Represents a feature client.
    /// </summary>
    public interface IFeatureClient
    {
        /// <summary>
        /// Called when interpretation is about to start.
        /// </summary>
        /// <param name="obj"></param>
        void OnBeforeInterpretation(OsmGeo obj);

        /// <summary>
        /// Called when a feature interprets an object succesfully.
        /// </summary>
        /// <param name="features_path"></param>
        /// <param name="obj"></param>
        /// <param name="meta">Some extra info the feature might want to include.</param>
        void InterpretationSucces(
            IList<Feature> features_path,
            OsmGeo obj,
            IDictionary<string,string> meta);

        /// <summary>
        /// Called when interpretation is finished.
        /// </summary>
        /// <param name="obj"></param>
        void OnAfterInterpretation(bool succes,OsmGeo obj);
    }
}
