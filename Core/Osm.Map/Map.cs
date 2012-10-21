// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// Foobar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Foobar is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Foobar. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Map.Layers;
using Osm.Map.Elements;
using Tools.Math.Geo;

namespace Osm.Map
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
                elements.AddRange(
                    this.Layers[idx].GetElements(box,zoom_factor));
            }

            return new MapQueryResult(box.Center,elements);
        }

        #endregion
    }
}
