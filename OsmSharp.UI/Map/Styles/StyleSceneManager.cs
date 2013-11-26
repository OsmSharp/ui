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
using OsmSharp.UI.Renderer;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Osm.Data;
using OsmSharp.Math.Geo;
using OsmSharp.Collections.LongIndex.LongIndex;
using OsmSharp.UI.Renderer.Scene;

namespace OsmSharp.UI.Map.Styles
{
    /// <summary>
    /// Manages the objects in a scene for a style interpreter.
    /// </summary>
    public class StyleSceneManager
    {
        /// <summary>
        /// Holds the scene.
        /// </summary>
		private Scene2D _scene;

        /// <summary>
        /// Holds the style interpreter.
        /// </summary>
        private StyleInterpreter _interpreter;

        /// <summary>
        /// Holds the interpreted nodes.
        /// </summary>
        private LongIndex _interpretedNodes;

        /// <summary>
        /// Holds the interpreted relations.
        /// </summary>
        private LongIndex _interpretedRelations;

        /// <summary>
        /// Holds the interpreted way.
        /// </summary>
        private LongIndex _interpretedWays;

        /// <summary>
        /// Creates a new style scene manager.
        /// </summary>
        /// <param name="interpreter">The intepreter converting OSM-objects into scene-objects.</param>
        /// <param name="zoomLevels"></param>
        public StyleSceneManager(StyleInterpreter interpreter, IProjection projection, List<float> zoomLevels)
            : this(new Scene2D(projection, zoomLevels), interpreter) { }

        /// <summary>
        /// Creates a new style scene manager.
        /// </summary>
        /// <param name="scene">The scene to manage.</param>
        /// <param name="interpreter">The intepreter converting OSM-objects into scene-objects.</param>
        public StyleSceneManager(Scene2D scene,
            StyleInterpreter interpreter)
        {
            _scene = scene;
            _interpreter = interpreter;

            _interpretedNodes = new LongIndex();
            _interpretedWays = new LongIndex();
            _interpretedRelations = new LongIndex();

            SimpleColor? color = _interpreter.GetCanvasColor();
            //this.Scene.BackColor = color.HasValue ? color.Value.Value : SimpleColor.FromArgb(0, 255, 255, 255).Value;
        }

        /// <summary>
        /// Returns the scene being managed.
        /// </summary>
        public Scene2D Scene
        {
            get
            {
                return _scene;
            }
        }

        /// <summary>
        /// Fills the scene with objects from the given datasource that existing inside the given boundingbox with the given projection.
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="box"></param>
        /// <param name="projection"></param>
        public void FillScene(IDataSourceReadOnly dataSource, GeoCoordinateBox box, IProjection projection)
        {
            IList<Osm.OsmGeo> osmGeos = dataSource.Get(box, null);
            foreach (var osmGeo in osmGeos)
            { // translate each object into scene object.
                LongIndex index = null;
                switch (osmGeo.Type)
                {
                    case Osm.OsmGeoType.Node:
                        index = _interpretedNodes;
                        break;
                    case Osm.OsmGeoType.Way:
                        index = _interpretedWays;
                        break;
                    case Osm.OsmGeoType.Relation:
                        index = _interpretedRelations;
                        break;
                }
                if (!index.Contains(osmGeo.Id.Value))
                { // object was not yet interpreted.
                    index.Add(osmGeo.Id.Value);

                    _interpreter.Translate(_scene, projection, dataSource, osmGeo);
                }
            }
        }
    }
}
