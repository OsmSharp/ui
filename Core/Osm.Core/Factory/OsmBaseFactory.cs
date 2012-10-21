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
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Core;

namespace Osm.Core.Factory
{
    public static class OsmBaseFactory
    {
        public static Node CreateNode()
        {
            return CreateNode(OsmBaseIdGenerator.NewId());
        }

        public static Node CreateNode(long id)
        {
            return new Node(id);
        }

        public static Way CreateWay()
        {
            return CreateWay(OsmBaseIdGenerator.NewId());
        }

        public static Way CreateWay(long id)
        {
            return new Way(id);
        }

        public static Relation CreateRelation()
        {
            return CreateRelation(OsmBaseIdGenerator.NewId());
        }

        public static Relation CreateRelation(long id)
        {
            return new Relation(id);
        }

        public static ChangeSet CreateChangeSet()
        {
            return CreateChangeSet(OsmBaseIdGenerator.NewId());
        }

        public static ChangeSet CreateChangeSet(long id)
        {
            return new ChangeSet(id);
        }
    }
}
