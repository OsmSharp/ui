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

using System.IO;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Osm;
using OsmSharp.Osm.Streams.Complete;
using OsmSharp.UI.Renderer.Scene;
using OsmSharp.UI.Renderer.Scene.Simplification;
using OsmSharp.Collections.Tags;

namespace OsmSharp.UI.Map.Styles.Streams
{
    /// <summary>
    /// Implements a streaming target that converts the given OSM-data into a MapCSS translated scene.
    /// </summary>
    public class StyleOsmStreamSceneStreamTarget : OsmCompleteStreamTarget
    {
        /// <summary>
        /// Holds the interpreter.
        /// </summary>
        private StyleInterpreter _styleInterpreter;

        /// <summary>
        /// Holds the scene.
        /// </summary>
        private Scene2D _scene;

        /// <summary>
        /// Holds the projection.
        /// </summary>
        private IProjection _projection;

        /// <summary>
        /// Holds the stream.
        /// </summary>
        private Stream _stream;

        /// <summary>
        /// Creates a new MapCSS scene target.
        /// </summary>
        /// <param name="styleInterpreter"></param>
        /// <param name="scene"></param>
        /// <param name="projection"></param>
        /// <param name="stream"></param>
        public StyleOsmStreamSceneStreamTarget(StyleInterpreter styleInterpreter,
            Scene2D scene, IProjection projection, Stream stream)
        {
            _projection = projection;
            _scene = scene;
            _styleInterpreter = styleInterpreter;
            _stream = stream;
        }

        /// <summary>
        /// Initializes this target.
        /// </summary>
        public override void Initialize()
        {

        }

        /// <summary>
        /// Adds a new node.
        /// </summary>
        /// <param name="node"></param>
        public override void AddNode(Node node)
        {
            _styleInterpreter.Translate(_scene, _projection, node);
        }

        /// <summary>
        /// Adds a new way.
        /// </summary>
        /// <param name="way"></param>
        public override void AddWay(CompleteWay way)
        {
            _styleInterpreter.Translate(_scene, _projection, way);
        }

        /// <summary>
        /// Adds a new relation.
        /// </summary>
        /// <param name="relation"></param>
        public override void AddRelation(CompleteRelation relation)
        {
            _styleInterpreter.Translate(_scene, _projection, relation);
        }

        /// <summary>
        /// Flushes all data.
        /// </summary>
        public override void Flush()
        {
            // merge objects.
            var merger = new Scene2DObjectMerger();
            Scene2D mergedScene = merger.BuildMergedScene(_scene);
            _scene = null;

            // serialize scene.
            TagsCollectionBase metaTags = new TagsCollection();
            mergedScene.Serialize(_stream, true, metaTags);

            _stream.Flush();
            base.Flush();
        }
    }
}