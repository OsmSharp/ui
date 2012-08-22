using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Geo;
using Osm.Map.Elements;

namespace Osm.Map
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
