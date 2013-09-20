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
using OsmSharp.Osm.Data.Streams;
using OsmSharp.UI.Map.Styles.MapCSS.v0_2.Domain;
using System.IO;
using OsmSharp.UI.Renderer.Scene;
using OsmSharp.Osm.Data.Memory;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Osm;

namespace OsmSharp.UI.Map.Styles.MapCSS.Streams
{
    /// <summary>
    /// Implements a streaming target that converts the given OSM-data into a MapCSS translated scene.
    /// </summary>
    public class MapCSSSceneOsmStreamTarget : OsmStreamTarget
    {
        /// <summary>
        /// Holds the interpreter.
        /// </summary>
        private MapCSSInterpreter _mapCSSInterpreter;

        /// <summary>
        /// Holds the scene output stream.
        /// </summary>
        private Stream _sceneStream;

        /// <summary>
        /// Holds the scene.
        /// </summary>
        private Scene2D _scene;

        /// <summary>
        /// Holds the projection.
        /// </summary>
        private IProjection _projection;

        /// <summary>
        /// Creates a new MapCSS scene target.
        /// </summary>
        /// <param name="mapCSSInterpreter"></param>
        /// <param name="sceneStream"></param>
        /// <param name="scene"></param>
        /// <param name="projection"></param>
        public MapCSSSceneOsmStreamTarget(MapCSSInterpreter mapCSSInterpreter, Stream sceneStream, 
            Scene2D scene, IProjection projection)
        {
            _projection = projection;
            _scene = scene;
            _sceneStream = sceneStream;
            _mapCSSInterpreter = mapCSSInterpreter;
        }

        /// <summary>
        /// Holds the memory data source.
        /// </summary>
        private MemoryDataSource _dataSource;

        /// <summary>
        /// Initializes this target.
        /// </summary>
        public override void Initialize()
        {
            _dataSource = new MemoryDataSource();
            _sceneStream.Seek(0, SeekOrigin.Begin);
        }

        /// <summary>
        /// Adds a new node.
        /// </summary>
        /// <param name="simpleNode"></param>
        public override void AddNode(Osm.Node simpleNode)
        {
            _dataSource.AddNode(simpleNode);
        }

        /// <summary>
        /// Adds a new way.
        /// </summary>
        /// <param name="simpleWay"></param>
        public override void AddWay(Osm.Way simpleWay)
        {
            _dataSource.AddWay(simpleWay);
        }

        /// <summary>
        /// Adds a new relation.
        /// </summary>
        /// <param name="simpleRelation"></param>
        public override void AddRelation(Osm.Relation simpleRelation)
        {
            _dataSource.AddRelation(simpleRelation);
        }

        /// <summary>
        /// Flushes all data.
        /// </summary>
        public override void Flush()
        {
            base.Flush();

            // reset the reader.
            this.Reader.Reset();

            // flush the scene by writing the result.
            foreach (var osmGeo in this.Reader)
            {
                // translate each object into scene object.
                _mapCSSInterpreter.Translate(_scene, _projection, _dataSource, osmGeo as OsmGeo);
            }

            // create the stream.
            _scene.Serialize(_sceneStream, true);
            _sceneStream.Flush();
        }
    }
}
