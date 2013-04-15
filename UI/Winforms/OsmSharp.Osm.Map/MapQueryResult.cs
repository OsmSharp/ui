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
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Osm.Map.Elements;

namespace OsmSharp.Osm.Map
{
    public class MapQueryResult
    {
        private SortedSet<IElement> _items;

        public MapQueryResult(GeoCoordinate center, IList<IElement> elements)
        {
            _items = new SortedSet<IElement>(new MapQueryResultItemComparer(center));
            foreach (IElement element in elements)
            {
                _items.Add(element);
            }
        }

        public SortedSet<IElement> Items
        {
            get
            {
                return _items;
            }
        }
    }

    internal class MapQueryResultItemComparer : IComparer<IElement>
    {
        private GeoCoordinate _center;
        public MapQueryResultItemComparer(GeoCoordinate center)
        {
            _center = center;
        }

        #region IComparer<IElement> Members

        public int Compare(IElement x, IElement y)
        {
            double x_distance = x.ShortestDistanceTo(_center);
            double y_distance = y.ShortestDistanceTo(_center);
            return x_distance.CompareTo(y_distance);
        }

        #endregion
    }
}
