using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Osm;
using OsmSharp.Geo.Geometries;
using OsmSharp.Geo.Features;

namespace OsmSharp.UI.Map.Styles.MapCSS
{
    /// <summary>
    /// Represents a MapCSS object that can be interpreted by the MapCSS interpreter.
    /// </summary>
    public class MapCSSObject : ITagsSource
    {
        /// <summary>
        /// Creates a new MapCSS object with a compete osmgeo object.
        /// </summary>
        /// <param name="osmGeoComplete"></param>
        public MapCSSObject(CompleteOsmGeo osmGeoComplete)
        {
            if (osmGeoComplete == null) throw new ArgumentNullException();

            this.OsmGeoComplete = osmGeoComplete;
        }

        /// <summary>
        /// Creates a new MapCSS object with an osmgeo object.
        /// </summary>
        /// <param name="osmGeo"></param>
        public MapCSSObject(OsmGeo osmGeo)
        {
            if (osmGeo == null) throw new ArgumentNullException();

            this.OsmGeo = osmGeo;
        }

        /// <summary>
        /// Creates a new MapCSS object with a geometry object.
        /// </summary>
        /// <param name="feature"></param>
        public MapCSSObject(Feature feature)
        {
            if (feature == null) throw new ArgumentNullException();

            this.Feature = feature;

            if (!(this.Feature.Geometry is LineairRing ||
                this.Feature.Geometry is Polygon ||
                this.Feature.Geometry is MultiPolygon ||
                this.Feature.Geometry is LineString))
            {
                throw new Exception("Invalid MapCSS type.");
            }
        }

        /// <summary>
        /// Gets the complete osm geo object.
        /// </summary>
        public CompleteOsmGeo OsmGeoComplete { get; private set; }

        /// <summary>
        /// Gets the osm geo object.
        /// </summary>
        public OsmGeo OsmGeo { get; private set; }

        /// <summary>
        /// Gets the feature.
        /// </summary>
        public Feature Feature { get; set; }

        /// <summary>
        /// Returns true if this object contains an osmgeo object.
        /// </summary>
        public bool IsOsm
        {
            get
            {
                return this.OsmGeoComplete != null ||
                    this.OsmGeo != null;
            }
        }

        /// <summary>
        /// Returns true if this object contains a geometry object.
        /// </summary>
        public bool IsGeo
        {
            get
            {
                return this.Feature != null;
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
                    if (this.OsmGeoComplete != null)
                    {
                        switch (this.OsmGeoComplete.Type)
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
                        switch (this.OsmGeo.Type)
                        {
                            case OsmGeoType.Node:
                                return MapCSS.MapCSSType.Node;
                            case OsmGeoType.Way:
                                return MapCSS.MapCSSType.Way;
                            case OsmGeoType.Relation:
                                return MapCSS.MapCSSType.Relation;
                        }
                    }
                }
                else
                {
                    if (this.Feature.Geometry is LineairRing ||
                        this.Feature.Geometry is Polygon ||
                        this.Feature.Geometry is MultiPolygon)
                    {
                        return MapCSS.MapCSSType.Area;
                    }
                    else if (this.Feature.Geometry is LineString)
                    {
                        return MapCSS.MapCSSType.Line;
                    }
                }
                throw new Exception("Invalid MapCSS type.");
            }
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
                return this.Feature.Attributes != null &&
                    this.Feature.Attributes.ContainsKey(key);
            }
            if (this.OsmGeoComplete != null)
            {
                return this.OsmGeoComplete.Tags != null &&
                    this.OsmGeoComplete.Tags.ContainsKey(key);
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
                if (this.Feature.Attributes != null &&
                    this.Feature.Attributes.TryGetValue(key, out value))
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
            if (this.OsmGeoComplete != null)
            {
                return this.OsmGeoComplete.Tags != null &&
                    this.OsmGeoComplete.Tags.TryGetValue(key, out tagValue);
            }
            return this.OsmGeo.Tags != null &&
                this.OsmGeo.Tags.TryGetValue(key, out tagValue);
        }
    }
}
