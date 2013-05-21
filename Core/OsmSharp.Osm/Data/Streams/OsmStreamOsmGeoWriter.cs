using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Osm.Simple;
using OsmSharp.Osm.Simple.Cache;

namespace OsmSharp.Osm.Data.Streams
{
    /// <summary>
    /// Osm stream writer that converts the stream to complete OsmGeo-objects.
    /// </summary>
    public abstract class OsmStreamOsmGeoWriter : OsmStreamWriter
    {
        /// <summary>
        /// Holds the data cache.
        /// </summary>
        private readonly OsmDataCache _cache;

        /// <summary>
        /// Creates a new osm stream writer that converts the stream to complete OsmGeo-objects.
        /// </summary>
        protected OsmStreamOsmGeoWriter()
        {
            _cache = new OsmDataCacheMemory();
        }

        /// <summary>
        /// Creates a new osm stream writer that converts the stream to complete OsmGeo-objects.
        /// </summary>
        protected OsmStreamOsmGeoWriter(OsmDataCache cache)
        {
            _cache = cache;
        }

        #region OsmStreamWriter-Implementation

        /// <summary>
        /// Initializes this writer.
        /// </summary>
        public override void Initialize()
        {
            
        }

        /// <summary>
        /// Adds a new simple node.
        /// </summary>
        /// <param name="simpleNode"></param>
        public override void AddNode(SimpleNode simpleNode)
        {
            _cache.AddNode(simpleNode);

            this.AddNode(Node.CreateFrom(simpleNode));
        }

        /// <summary>
        /// Adds a new simple way.
        /// </summary>
        /// <param name="simpleWay"></param>
        public override void AddWay(SimpleWay simpleWay)
        {
            _cache.AddWay(simpleWay);

            Way way = Way.CreateFrom(simpleWay, _cache);
            if (way != null)
            {
                this.AddWay(way);
            }
        }

        /// <summary>
        /// Adds a new simple relation.
        /// </summary>
        /// <param name="simpleRelation"></param>
        public override void AddRelation(Simple.SimpleRelation simpleRelation)
        {
            _cache.AddRelation(simpleRelation);

            Relation relation = Relation.CreateFrom(simpleRelation, _cache);
            if (relation != null)
            {
                this.AddRelation(relation);
            }
        }

        #endregion

        /// <summary>
        /// A node was detected in the source-stream.
        /// </summary>
        /// <param name="node"></param>
        protected abstract void AddNode(Node node);

        /// <summary>
        /// A complete was was detected in the source-stream.
        /// </summary>
        /// <param name="way"></param>
        protected abstract void AddWay(Way way);

        /// <summary>
        /// A complete relation was detected in the source-stream.
        /// </summary>
        /// <param name="relation"></param>
        protected abstract void AddRelation(Relation relation);
    }
}
