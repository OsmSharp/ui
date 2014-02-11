// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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

using OsmSharp.Routing.Graph;

namespace OsmSharp.Routing.Osm.Graphs
{
    /// <summary>
    /// A simple edge containing the orignal OSM-tags and a flag indicating the direction of this edge relative to the 
    /// OSM-direction.
    /// </summary>
    public struct LiveEdge : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Contains a value that represents tagsId and forward flag [forwardFlag (true when zero)][tagsIdx].
        /// </summary>
        private uint _value;

        /// <summary>
        /// Flag indicating if this is a forward or backward edge relative to the tag descriptions.
        /// </summary>
        public bool Forward 
        {
            get
            { // true when first bit is 0.
                return _value % 2 == 0;
            }
            set
            {
                if (_value % 2 == 0)
                { // true already.
                    if (!value) { _value = _value + 1; }
                }
                else
                { // false already.
                    if (value) { _value = _value - 1; }
                }
            }
        }

        /// <summary>
        /// The properties of this edge.
        /// </summary>
        public uint Tags
        {
            get
            {
                return _value / 2;
            }
            set
            {
                if (_value % 2 == 0)
                { // true already.
                    _value = value * 2;
                }
                else
                { // false already.
                    _value = (value * 2) + 1;
                }
            }
        }

        /// <summary>
        /// Returns true if this edge represents a neighbour-relation.
        /// </summary>
        public bool RepresentsNeighbourRelations
        {
            get { return true; }
        }
    }
}