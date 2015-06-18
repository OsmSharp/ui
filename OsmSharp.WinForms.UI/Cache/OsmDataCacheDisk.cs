// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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

using OsmSharp.Osm;
using OsmSharp.Osm.Cache;
using OsmSharp.Osm.Streams;
using OsmSharp.Osm.Xml.Streams;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.WinForms.UI.Cache
{
    /// <summary>
    /// An osm data cache for simple OSM objects kept in memory.
    /// </summary>
    public class OsmDataCacheDisk : OsmDataCache, IDisposable
    {
        /// <summary>
        /// The disk cache folder.
        /// </summary>
        private DirectoryInfo _cacheDirectory;

        /// <summary>
        /// Creates a new osm data cache for simple OSM objects.
        /// </summary>
        public OsmDataCacheDisk()
        {
            _cacheDirectory = new DirectoryInfo(Path.Combine(Path.GetTempPath() + Guid.NewGuid().ToString()));
            _cacheDirectory.Create();
        }

        /// <summary>
        /// Creates a new osm data cache for simple OSM objects.
        /// </summary>
        public OsmDataCacheDisk(DirectoryInfo cacheDirectory)
        {
            _cacheDirectory = cacheDirectory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        public override void AddNode(Node node)
        {
            if (node == null) throw new ArgumentNullException("node");
            if (node.Id == null) throw new Exception("node.Id is null");

            this.Store(node);
        }

        /// <summary>
        /// Removes the node with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool RemoveNode(long id)
        {
            if (this.Exist(id, OsmGeoType.Node))
            {
                this.Delete(id, OsmGeoType.Node);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public override bool TryGetNode(long id, out Node node)
        {
            if (this.Exist(id, OsmGeoType.Node))
            {
                node = this.Read(id, OsmGeoType.Node) as Node;
                return true;
            }
            node = null;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="way"></param>
        public override void AddWay(Way way)
        {
            if (way == null) throw new ArgumentNullException("way");
            if (way.Id == null) throw new Exception("way.Id is null");

            this.Store(way);
        }

        /// <summary>
        /// Removes the way with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool RemoveWay(long id)
        {
            if (this.Exist(id, OsmGeoType.Way))
            {
                this.Delete(id, OsmGeoType.Way);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="way"></param>
        /// <returns></returns>
        public override bool TryGetWay(long id, out Way way)
        {
            if (this.Exist(id, OsmGeoType.Way))
            {
                way = this.Read(id, OsmGeoType.Way) as Way;
                return true;
            }
            way = null;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relation"></param>
        public override void AddRelation(Relation relation)
        {
            if (relation == null) throw new ArgumentNullException("relation");
            if (relation.Id == null) throw new Exception("relation.Id is null");

            this.Store(relation);
        }

        /// <summary>
        /// Removes the relation with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool RemoveRelation(long id)
        {
            if (this.Exist(id, OsmGeoType.Relation))
            {
                this.Delete(id, OsmGeoType.Relation);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="relation"></param>
        /// <returns></returns>
        public override bool TryGetRelation(long id, out Relation relation)
        {
            if (this.Exist(id, OsmGeoType.Relation))
            { // exists.
                relation = this.Read(id, OsmGeoType.Relation) as Relation;
                return true;
            }
            relation = null;
            return false;
        }

        /// <summary>
        /// Returns the storage file name for this given object type and id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private string StoreFileName(long id, OsmGeoType type)
        {
            switch (type)
            {
                case OsmGeoType.Node:
                    return Path.Combine(_cacheDirectory.FullName, string.Format("{0}.node", id.ToString()));
                case OsmGeoType.Way:
                    return Path.Combine(_cacheDirectory.FullName, string.Format("{0}.way", id.ToString()));
                case OsmGeoType.Relation:
                    return Path.Combine(_cacheDirectory.FullName, string.Format("{0}.relation", id.ToString()));
            }
            throw new ArgumentOutOfRangeException();
        }

        /// <summary>
        /// Returns the storage file name for this given object.
        /// </summary>
        /// <param name="osmGeo"></param>
        /// <returns></returns>
        private string StoreFileName(OsmGeo osmGeo)
        {
            return this.StoreFileName(osmGeo.Id.Value, osmGeo.Type);
        }

        /// <summary>
        /// Stores an osmGeo object to disk.
        /// </summary>
        /// <param name="osmGeo"></param>
        private void Store(OsmGeo osmGeo)
        {
            using (var targetFile = new FileInfo(this.StoreFileName(osmGeo)).OpenWrite())
            {
                var target = new XmlOsmStreamTarget(targetFile);
                target.RegisterSource(new OsmGeo[] { osmGeo }.ToOsmStreamSource());
                target.Pull();
                target.Flush();
            }
        }

        /// <summary>
        /// Reads an osmGeo object from disk.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        private OsmGeo Read(long id, OsmGeoType type)
        {
            using (var sourceFile = new FileInfo(this.StoreFileName(id, type)).OpenRead())
            {
                var source = new XmlOsmStreamSource(sourceFile);
                var readObjects = new List<OsmGeo>(source);
                source.Dispose();

                if (readObjects != null && readObjects.Count == 1)
                {
                    return readObjects[0];
                }
                throw new InvalidDataException("Invalid cached file read, make sure not to modify the cached while in use or to synchonize access.");
            }
        }

        /// <summary>
        /// Deletes an osmGeo object from disk.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        private void Delete(long id, OsmGeoType type)
        {
            File.Delete(this.StoreFileName(id, type));
        }

        /// <summary>
        /// Returns true if the given object exists.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool Exist(long id, OsmGeoType type)
        {
            return File.Exists(this.StoreFileName(id, type));
        }

        /// <summary>
        /// Makes sure the cache directory is deleted after using it.
        /// </summary>
        public void Dispose()
        {
            _cacheDirectory.Delete(true);
        }

        /// <summary>
        /// Clears all data from this cache.
        /// </summary>
        public override void Clear()
        {
            _cacheDirectory.Delete(true);

            _cacheDirectory = new DirectoryInfo(Path.Combine(Path.GetTempPath() + Guid.NewGuid().ToString()));
            _cacheDirectory.Create();
        }
    }
}