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

namespace Osm.Core.Simple
{
    /// <summary>
    /// Primive used as a base class for any osm object that has a meaning on the map (Nodes, Ways and Relations).
    /// </summary>
    public class SimpleOsmGeo
    {
        public long? Id { get; set; }

        public SimpleOsmGeoType Type { get; protected set; }

        public IDictionary<string,string> Tags { get; set; }

        public long? ChangeSetId { get; set; }

        public bool? Visible { get; set; }

        public DateTime? TimeStamp { get; set; }

        public ulong? Version { get; set; }

        public long? UserId { get; set; }

        public string UserName { get; set; }
    }
}
