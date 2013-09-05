using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Osm;
using OsmSharp.Geo.Geometries;

namespace OsmSharp.UI.Map.Styles.MapCSS
{
    /// <summary>
    /// Represents a MapCSS object that can be interpreted by the MapCSS interpreter.
    /// </summary>
    public class MapCSSObject : ITagsSource
    {
        /// <summary>
        /// Creates a new MapCSS object with an osmgeo object.
        /// </summary>
        /// <param name="geo"></param>
        public MapCSSObject(CompleteOsmGeo osmGeo)
        {
            if (osmGeo == null) throw new ArgumentNullException();

            this.OsmGeo = osmGeo;
        }

        /// <summary>
        /// Creates a new MapCSS object with a geometry object.
        /// </summary>
        /// <param name="geometry"></param>
        public MapCSSObject(Geometry geometry)
        {
            if (geometry == null) throw new ArgumentNullException();

            this.Geometry = geometry;

            if (!(this.Geometry is LineairRing ||
                this.Geometry is Polygon ||
                this.Geometry is MultiPolygon ||
                this.Geometry is LineString))
            {
                throw new Exception("Invalid MapCSS type.");
            }
        }

        /// <summary>
        /// Gets the osmgeo object.
        /// </summary>
        public CompleteOsmGeo OsmGeo { get; private set; }

        /// <summary>
        /// Gets the geometry object.
        /// </summary>
        public Geometry Geometry { get; set; }

        /// <summary>
        /// Returns true if this object contains an osmgeo object.
        /// </summary>
        public bool IsOsm
        {
            get
            {
                return this.OsmGeo != null;
            }
        }

        /// <summary>
        /// Returns true if this object contains a geometry object.
        /// </summary>
        public bool IsGeo
        {
            get
            {
                return this.Geometry != null;
            }
        }

        /// <summary>
        /// Returns the type of MapCSS object.
        /// </summary>
        public MapCSSType MapCSSType
        {
            get
            {
                if (this.IsOsm)
                {
                    switch (this.OsmGeo.Type)
                    {
                        case CompleteOsmType.Node:
                            return MapCSS.MapCSSType.Node;
                        case CompleteOsmType.Way:
                            return MapCSS.MapCSSType.Way;
                        case CompleteOsmType.Relation:
                            return MapCSS.MapCSSType.Relation;
                    }
                }
                else
                {
                    if (this.Geometry is LineairRing ||
                        this.Geometry is Polygon ||
                        this.Geometry is MultiPolygon)
                    {
                        return MapCSS.MapCSSType.Area;
                    }
                    else if (this.Geometry is LineString)
                    {
                        return MapCSS.MapCSSType.Line;
                    }
                }
                throw new Exception("Invalid MapCSS type.");
            }
        }

        /// <summary>
        /// Returns true if the object set in this mapcss object is of the given type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsType(Type type)
        {
            if (this.IsGeo)
            {
                return type.IsInstanceOfType(this.Geometry);
            }
            return type.IsInstanceOfType(this.OsmGeo);
        }

        /// <summary>
        /// Returns true if the tags- or attributecollection contains the given key.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            if (this.IsGeo)
            {
                return this.Geometry.Attributes != null &&
                    this.Geometry.Attributes.ContainsKey(key);
            }
            return this.OsmGeo.Tags != null &&
                this.OsmGeo.Tags.ContainsKey(key);
        }

        /// <summary>
        /// Returns true if the tags- or attributecollection contains the given key.
        /// Returns the value associated with the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="tagValue"></param>
        /// <returns></returns>
        public bool TryGetValue(string key, out string tagValue)
        {
            if (this.IsGeo)
            {
                object value;
                if (this.Geometry.Attributes != null &&
                    this.Geometry.Attributes.TryGetValue(key, out value))
                {
                    if (value != null)
                    {
                        tagValue = value.ToString();
                        return true;
                    }
                }
                tagValue = string.Empty;
                return false;
            }
            tagValue = string.Empty;
            return this.OsmGeo.Tags != null &&
                this.OsmGeo.Tags.TryGetValue(key, out tagValue);
        }
    }
}
