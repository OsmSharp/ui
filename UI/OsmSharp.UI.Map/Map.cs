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
using OsmSharp.Osm.Map.Layers;
using OsmSharp.Osm.Map.Elements;
using OsmSharp.Tools.Math.Geo;

namespace OsmSharp.Osm.Map
{
    /// <summary>
    /// Represents an osm map data structure.
    /// </summary>
    public class Map
    {
        public delegate void LayerChangedDelegate(ILayer layer);

        /// <summary>
        /// The layers in this map.
        /// </summary>
        private IList<ILayer> _layers;

        /// <summary>
        /// Creates a new map.
        /// </summary>
        public Map()
        {
            _layers = new List<ILayer>();
        }

        /// <summary>
        /// Event thrown when the map was modified.
        /// </summary>
        public event EventHandler Modified;

        /// <summary>
        /// Returns the child layers in this layer.
        /// </summary>
        public IList<ILayer> Layers
        {
            get
            {
                return _layers;
            }
        }

        #region Resolve Objects

        /// <summary>
        /// Returns all the elements in the given box.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public MapQueryResult GetElementsAt(GeoCoordinateBox box, double zoom_factor)
        {
            List<IElement> elements = new List<IElement>();
            
            for(int idx = this.Layers.Count-1;idx>=0;idx--)
            {
                if (this.Layers[idx].Visible)
                { // only add elements from visible layers.
                    elements.AddRange(
                        this.Layers[idx].GetElements(box, zoom_factor));
                }
            }

            return new MapQueryResult(box.Center,elements);
        }

        #endregion
    }
}
