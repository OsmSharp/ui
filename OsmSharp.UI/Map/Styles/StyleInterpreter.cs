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
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Osm;
using OsmSharp.UI.Renderer;
using OsmSharp;
using OsmSharp.Osm.Data;
using OsmSharp.UI.Renderer.Scene;
using OsmSharp.Collections.Tags;

namespace OsmSharp.UI.Map.Styles
{
    /// <summary>
    /// Represents a style interpreter.
    /// </summary>
    public abstract class StyleInterpreter
    {
        /// <summary>
        /// Returns the canvas color if any.
        /// </summary>
        /// <returns></returns>
        public abstract SimpleColor GetCanvasColor();

        /// <summary>
        /// Translates the given OSM objects into corresponding geometries.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="projection"></param>
        /// <param name="source"></param>
        /// <param name="osmGeo"></param>
        public virtual void Translate(Scene2D scene, IProjection projection, IDataSourceReadOnly source, OsmGeo osmGeo)
        {
            switch (osmGeo.Type)
            {
                case OsmGeoType.Node:
                    var node = osmGeo as Node;
                    if(node.Tags == null)
                    { // make sure that a node has a tag collection by default.
                        node.Tags= new TagsCollection();
                    }
                    this.Translate(scene, projection, node);
                    break;
                case OsmGeoType.Way:
                    this.Translate(scene, projection, CompleteWay.CreateFrom(osmGeo as Way, source));
                    break;
                case OsmGeoType.Relation:
                    this.Translate(scene, projection, CompleteRelation.CreateFrom(osmGeo as Relation, source));
                    break;
            }
        }

        /// <summary>
        /// Translates the given OSM objects into corresponding geometries.
        /// </summary>
        /// <param name="projection">The projection to use.</param>
        /// <param name="osmGeo">The osm object.</param>
        /// <param name="scene">The scene to fill with the resulting geometries.</param>
        /// <returns></returns>
        public abstract void Translate(Scene2D scene, IProjection projection, ICompleteOsmGeo osmGeo);

        /// <summary>
        /// Returns true if this style applies to the given object.
        /// </summary>
        /// <param name="osmGeo"></param>
        /// <returns></returns>
        public abstract bool AppliesTo(OsmGeo osmGeo);
    }
}