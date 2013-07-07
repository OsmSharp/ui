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

namespace OsmSharp.Osm.Data.Streams.Filters
{
    /// <summary>
    /// A filter that raizes events for each object.
    /// </summary>
    public class OsmStreamFilterWithEvents : OsmStreamFilter
    {
        /// <summary>
        /// Holds the parameters object sent with the events.
        /// </summary>
        private readonly object _param;

        /// <summary>
        /// Creates a new filter with events.
        /// </summary>
        public OsmStreamFilterWithEvents()
        {
            _param = null;
        }

        /// <summary>
        /// Creates a new filter with events.
        /// </summary>
        /// <param name="param"></param>
        public OsmStreamFilterWithEvents(object param)
        {
            _param = param;
        }

        /// <summary>
        /// An empty delegate.
        /// </summary>
        public delegate void EmptyDelegate();

        /// <summary>
        /// Event raised when filter is initialized.
        /// </summary>
        public event EmptyDelegate InitializeEvent;

        /// <summary>
        /// Initializes this filter.
        /// </summary>
        public override void Initialize()
        {
            if (this.InitializeEvent != null)
            {
                this.InitializeEvent();
            }

            this.Reader.Initialize();
        }

        /// <summary>
        /// A delegate with a simple osm geo object as parameter.
        /// </summary>
        /// <param name="simpleOsmGeo"></param>
        /// <param name="param"></param>
        public delegate void SimpleOsmGeoDelegate(Simple.OsmGeo simpleOsmGeo, object param);

        /// <summary>
        /// Event raised when the move is made to the next object.
        /// </summary>
        public event SimpleOsmGeoDelegate MovedToNextEvent;

        /// <summary>
        /// Moves this filter to the next object.
        /// </summary>
        /// <returns></returns>
        public override bool MoveNext()
        {
            if (this.Reader.MoveNext())
            {
                if (this.MovedToNextEvent != null)
                {
                    this.MovedToNextEvent(this.Reader.Current(), _param);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        /// <returns></returns>
        public override Simple.OsmGeo Current()
        {
            return this.Reader.Current();
        }

        /// <summary>
        /// Resets this filter.
        /// </summary>
        public override void Reset()
        {
            this.Reader.Reset();
        }

        /// <summary>
        /// Returns true if this filter can be reset.
        /// </summary>
        public override bool CanReset
        {
            get { return this.Reader.CanReset; }
        }
    }
}