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
using OsmSharp.Osm.Simple;

namespace OsmSharp.Osm.Data.Core.Processor
{
    /// <summary>
    /// A data processor filter.
    /// </summary>
    public abstract class DataProcessorFilter : DataProcessorSource
    {
        /// <summary>
        /// Holds the source.
        /// </summary>
        private DataProcessorSource _source;

        /// <summary>
        /// Creates a new data processor filter.
        /// </summary>
        protected DataProcessorFilter()
        {

        }

        /// <summary>
        /// Registers a source as the source to filter.
        /// </summary>
        /// <param name="source"></param>
        public virtual void RegisterSource(DataProcessorSource source)
        {
            _source = source;
        }

        /// <summary>
        /// Returns the source being filtered.
        /// </summary>
        protected DataProcessorSource Source
        {
            get
            {
                return _source;
            }
        }

        /// <summary>
        /// Initializes this filter.
        /// </summary>
        public abstract override void Initialize();

        /// <summary>
        /// Moves to the next object.
        /// </summary>
        /// <returns></returns>
        public abstract override bool MoveNext();

        /// <summary>
        /// Returns the current object.
        /// </summary>
        /// <returns></returns>
        public abstract override SimpleOsmGeo Current();
    }
}
