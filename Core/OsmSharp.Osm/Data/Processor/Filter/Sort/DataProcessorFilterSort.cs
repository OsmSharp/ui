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

namespace OsmSharp.Osm.Data.Core.Processor.Filter.Sort
{
    /// <summary>
    /// A data processing filter that sorts the OSM data: Node -> Way -> Relation.
    /// </summary>
    public class DataProcessorFilterSort : DataProcessorFilter
    {
        /// <summary>
        /// The current type.
        /// </summary>
        private SimpleOsmGeoType _current_type = SimpleOsmGeoType.Node;

        /// <summary>
        /// Creates a new data processor filter.
        /// </summary>
        public DataProcessorFilterSort()
            :base()
        {

        }

        /// <summary>
        /// Initializes this filter.
        /// </summary>
        public override void Initialize()
        {
            if (this.Source == null)
            {
                throw new Exception("No source registered!");
            }
            // no intialisation this filter does the same thing every time.
            this.Source.Initialize();
        }

        /// <summary>
        /// Move to the next object.
        /// </summary>
        /// <returns></returns>
        public override bool MoveNext()
        {
            if (this.Source.MoveNext())
            {
                bool finished = false;
                while (this.Current().Type != _current_type)
                {
                    if (!this.Source.MoveNext())
                    {
                        finished = true;
                        break;
                    }
                }

                if (!finished && this.Current().Type == _current_type)
                {
                    return true;
                }
            }

            switch (_current_type)
            {
                case SimpleOsmGeoType.Node:
                    this.Source.Reset();
                    _current_type = SimpleOsmGeoType.Way;
                    return this.MoveNext();
                case SimpleOsmGeoType.Way:
                    this.Source.Reset();
                    _current_type = SimpleOsmGeoType.Relation;
                    return this.MoveNext();
                case SimpleOsmGeoType.Relation:
                    return false;
            }
            throw new InvalidOperationException("Unkown SimpleOsmGeoType");
            
        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        /// <returns></returns>
        public override SimpleOsmGeo Current()
        {
            return this.Source.Current();
        }

        /// <summary>
        /// Resets this filter.
        /// </summary>
        public override void Reset()
        {
            _current_type = SimpleOsmGeoType.Node;
            this.Source.Reset();
        }

        /// <summary>
        /// Returns true if this source can be reset.
        /// </summary>
        public override bool CanReset
        {
            get
            {
                return this.Source.CanReset;
            }
        }
    }
}
