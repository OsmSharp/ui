// OsmSharp - OpenStreetMap tools & library.
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

using System;
using OsmSharp.Osm;

namespace OsmSharp.Osm.Data.Streams.Filters
{
    /// <summary>
    /// An OSM stream filter sort.
    /// </summary>
    public class OsmStreamFilterSort : OsmStreamFilter
    {
        /// <summary>
        /// The current type.
        /// </summary>
        private OsmGeoType _currentType = OsmGeoType.Node;

        /// <summary>
        /// Initializes this filter.
        /// </summary>
        public override void Initialize()
        {
            if (this.Reader == null)
            {
                throw new Exception("No target registered!");
            }
            // no intialisation this filter does the same thing every time.
            this.Reader.Initialize();
        }

        /// <summary>
        /// Move to the next object.
        /// </summary>
        /// <returns></returns>
        public override bool MoveNext()
        {
            if (this.Reader.MoveNext())
            {
                bool finished = false;
                while (this.Current().Type != _currentType)
                {
                    if (!this.Reader.MoveNext())
                    {
                        finished = true;
                        break;
                    }
                }

                if (!finished && this.Current().Type == _currentType)
                {
                    return true;
                }
            }

            switch (_currentType)
            {
                case OsmGeoType.Node:
                    this.Reader.Reset();
                    _currentType = OsmGeoType.Way;
                    return this.MoveNext();
                case OsmGeoType.Way:
                    this.Reader.Reset();
                    _currentType = OsmGeoType.Relation;
                    return this.MoveNext();
                case OsmGeoType.Relation:
                    return false;
            }
            throw new InvalidOperationException("Unkown SimpleOsmGeoType");
        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        /// <returns></returns>
        public override OsmGeo Current()
        {
            return this.Reader.Current();
        }

        /// <summary>
        /// Resets this filter.
        /// </summary>
        public override void Reset()
        {
            _currentType = OsmGeoType.Node;
            this.Reader.Reset();
        }

        /// <summary>
        /// Returns true if this source can be reset.
        /// </summary>
        public override bool CanReset
        {
            get
            {
                return this.Reader.CanReset;
            }
        }
    }
}