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
using OsmSharp.Osm.Map.Layers;

namespace OsmSharp.Osm.Map.Styles
{
    /// <summary>
    /// Represents a style of a datasource.
    /// 
    /// A style is a way of transforming features (for osm objects) into map elements.
    /// </summary>
    public class OsmStyleInterpreter : IFeatureClient
    {
        /// <summary>
        /// Holds the layer this style if for.
        /// </summary>
        private DataSourceLayer _layer;

        /// <summary>
        /// Holds the style information.
        /// </summary>
        private OsmStyle _style;

        /// <summary>
        /// Creates a new style for a datasoruce layer.
        /// </summary>
        /// <param name="layer"></param>
        public OsmStyleInterpreter(DataSourceLayer layer)
        {
            _layer = layer;
        }


        #region IFeatureClient Members

        /// <summary>
        /// Before interpretation; remove the object.
        /// </summary>
        /// <param name="obj"></param>
        public void OnBeforeInterpretation(OsmGeo obj)
        {
            _layer.RemoveElements(obj);
        }

        /// <summary>
        /// Interpretation of the given object succeeded!
        /// </summary>
        /// <param name="features_path"></param>
        /// <param name="obj"></param>
        /// <param name="meta"></param>
        public void InterpretationSucces(
            IList<Feature> features_path, 
            OsmGeo obj, 
            IDictionary<string, string> meta)
        {

        }

        /// <summary>
        /// After interpretation; if not succesfull report!
        /// </summary>
        /// <param name="succes"></param>
        /// <param name="obj"></param>
        public void OnAfterInterpretation(bool succes, OsmGeo obj)
        {
            // TODO: report unsuccesfull interpretation.
        }

        #endregion
    }
}
