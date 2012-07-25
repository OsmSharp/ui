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
