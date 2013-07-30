using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Osm;
using OsmSharp.UI.Renderer;
using OsmSharp;
using OsmSharp.Osm.Data;

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
                    this.Translate(scene, projection, CompleteNode.CreateFrom(osmGeo as Node));
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
        public abstract void Translate(Scene2D scene, IProjection projection, CompleteOsmGeo osmGeo);
    }
}