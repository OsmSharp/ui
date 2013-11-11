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

namespace OsmSharp.UI.Map.Styles.Streams
{
    /// <summary>
    /// Implements a streaming target that converts the given OSM-data into a MapCSS translated scene.
    /// </summary>
    public class StyleOsmStreamSceneTarget : OsmCompleteStreamTarget
    {
        /// <summary>
        /// Holds the interpreter.
        /// </summary>
        private StyleInterpreter _mapCSSInterpreter;

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
        /// <param name="scene"></param>
        /// <param name="projection"></param>
        public StyleOsmStreamSceneTarget(StyleInterpreter mapCSSInterpreter, 
            Scene2D scene, IProjection projection)
        {
            _projection = projection;
            _scene = scene;
            _mapCSSInterpreter = mapCSSInterpreter;
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
        public override void AddNode(CompleteNode node)
        {
            _mapCSSInterpreter.Translate(_scene, _projection, node);
        }

        /// <summary>
        /// Adds a new way.
        /// </summary>
        /// <param name="way"></param>
        public override void AddWay(CompleteWay way)
        {
            if (way.Id == 198214128)
            {
                System.Diagnostics.Debug.WriteLine("");
            }
            _mapCSSInterpreter.Translate(_scene, _projection, way);
        }

        /// <summary>
        /// Adds a new relation.
        /// </summary>
        /// <param name="relation"></param>
        public override void AddRelation(CompleteRelation relation)
        {
            _mapCSSInterpreter.Translate(_scene, _projection, relation);
        }

        /// <summary>
        /// Flushes all data.
        /// </summary>
        public override void Flush()
        {
            base.Flush();
        }
    }
}