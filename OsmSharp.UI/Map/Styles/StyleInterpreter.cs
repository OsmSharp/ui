// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2016 Abelshausen Ben
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

using OsmSharp.Math.Geo.Projections;
using OsmSharp.Osm;
using OsmSharp.Osm.Data;
using OsmSharp.Osm.Complete;
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
        public virtual void Translate(Scene2D scene, IProjection projection, IOsmGeoSource source, OsmGeo osmGeo)
        {
            if (osmGeo.Tags == null)
            { // make sure that an object has a tag collection by default.
                osmGeo.Tags = new TagsCollection();
            }

            switch (osmGeo.Type)
            {
                case OsmGeoType.Node:
                    this.Translate(scene, projection, (osmGeo as Node));
                    break;
                case OsmGeoType.Way:
                    this.Translate(scene, projection, (osmGeo as Way).CreateComplete(source));
                    break;
                case OsmGeoType.Relation:
                    this.Translate(scene, projection, (osmGeo as Relation).CreateComplete(source));
                    break;
            }
        }

        /// <summary>
        /// Translates the given OSM objects into corresponding geometries.
        /// </summary>
        public abstract void Translate(Scene2D scene, IProjection projection, ICompleteOsmGeo osmGeo);

        /// <summary>
        /// Returns true if this style applies to the given object.
        /// </summary>
        public abstract bool AppliesTo(OsmGeo osmGeo);
    }
}