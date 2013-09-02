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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Osm;

namespace OsmSharp.Data.Redis.Osm.Primitives
{
    /// <summary>
    /// A serializable type to keep in the redis db.
    /// </summary>
    public class RedisNode
    {
        /// <summary>
        /// The id.
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// The latitude.
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// The longitude.
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// The changeset id.
        /// </summary>
        public long? ChangeSetId { get; set; }

        /// <summary>
        /// The visible flag.
        /// </summary>
        public bool? Visible { get; set; }

        /// <summary>
        /// The timestamp.
        /// </summary>
        public DateTime? TimeStamp { get; set; }

        /// <summary>
        /// The version.
        /// </summary>
        public ulong? Version { get; set; }

        /// <summary>
        /// The userid.
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// The username.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Holds the tags.
        /// </summary>
        public List<RedisTag> Tags { get; set; }
    }
}
