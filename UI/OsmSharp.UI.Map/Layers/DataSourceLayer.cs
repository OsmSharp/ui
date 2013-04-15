// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
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
using OsmSharp.Osm.Data;
using OsmSharp.Osm.Map.Styles;
using OsmSharp.Osm.Map.Elements;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Tools.Math.Shapes;
using System.Drawing;
using OsmSharp.Tools.Math.Geo.Factory;

namespace OsmSharp.Osm.Map.Layers
{
    /// <summary>
    /// A layer around a data source.
    /// </summary>
    public class DataSourceLayer : ILayer
    {
        /// <summary>
        /// The event raised when data or elements in this layer changed.
        /// </summary>
        public event Map.LayerChangedDelegate Changed;

        /// <summary>
        /// The datasource for this layer.
        /// </summary>
        private IDataSourceReadOnly _source;

        /// <summary>
        /// The layers in this datasource layer.
        /// </summary>
        private IList<DataSourceSubLayer> _layers;

        /// <summary>
        /// The style to use for this datasource.
        /// </summary>
        private OsmStyle _style;

        /// <summary>
        /// The unique id of this layer.
        /// </summary>
        private Guid _id;
        
        /// <summary>
        /// Holds the valid/invalid flag of this layer.
        /// </summary>
        private bool _valid;

        /// <summary>
        /// The default color of this layer.
        /// </summary>
        private Color _default;

        /// <summary>
        /// Creates a new data source.
        /// </summary>
        /// <param name="source"></param>
        public DataSourceLayer(IDataSourceReadOnly source)
        {
            this.Visible = true;
            this.Name = "Data Source Layer";

            _source = source;
            _id = Guid.NewGuid();

            // TODO: Add modification event.
            // _source.
            _default = Color.Blue;

            this.MinZoom = -1;
            this.MaxZoom = -1;
        }

        /// <summary>
        /// Creates a new data source.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="default_color"></param>
        public DataSourceLayer(
            IDataSourceReadOnly source,
            Color default_color)
            : this(source)
        {
            _default = default_color;
        }

        #region Style

        /// <summary>
        /// Gets/Sets this layer's styles.
        /// </summary>
        public OsmStyle Style
        {
            get
            {
                return _style;
            }
            set
            {
                _style = value;
            }
        }

        #endregion

        #region ILayer Members

        /// <summary>
        /// Returns the unique id of this layer.
        /// </summary>
        public Guid Id
        {
            get 
            {
                return _id;
            }
        }

        /// <summary>
        /// Invalidates this layer.
        /// </summary>
        public void Invalidate()
        {
            _valid = false;
        }

        /// <summary>
        /// Validates this layer.
        /// </summary>
        public void Validate()
        {
            _valid = true;
        }

        /// <summary>
        /// Gets the valid flag.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return _valid;
        }

        /// <summary>
        /// Returns the child layers.
        /// </summary>
        public IList<ILayer> Layers
        {
            get 
            {
                return (IList<ILayer>)_layers;
            }
        }

        /// <summary>
        /// Returns all the elements in this layer.
        /// </summary>
        /// <returns></returns>
        public IList<IElement> GetElements(
            GeoCoordinateBox box,
            double zoom_factor)
        {
            // get objects from source.
            IList<OsmGeo> objects = _source.Get(box, null);

            // convert objects to elements using interpreter.
            List<IElement> elements = new List<IElement>();
            Interpreter.Interpreter interpreter = new OsmSharp.Osm.Interpreter.Interpreter();
            foreach (OsmGeo geo in objects)
            {
                if (geo is OsmGeo)
                {
                    if (geo is Node)
                    {
                        Node n = geo as Node;

                        if (n.Tags.ContainsKey("type"))
                        {
                            //ElementDot dot = new ElementDot(
                            //    Color.Red.ToArgb(),
                            //    0.0002f,
                            //    new OsmSharp.Tools.Math.Shapes.ShapeDotF<GeoCoordinate, GeoCoordinateBox>(
                            //        n.Coordinate));
                            //elements.Add(dot);
                        }
                    }
                    else if (geo is Way)
                    {
                        Way w = geo as Way;
                        ElementLine line;
                        if (w.Tags.ContainsKey("metadata_name"))
                        {
                            Color transparent_color = Color.FromArgb(230,_default);
                            line = new ElementLine(
                                new ShapePolyLineF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine>(
                                    PrimitiveGeoFactory.Instance,
                                    w.GetCoordinates().ToArray<GeoCoordinate>()),
                                transparent_color.ToArgb(),
                                4f,
                                true);
                            elements.Add(line);
                        }
                    }
                }
            }

            // return result.
            return elements;
        }

        /// <summary>
        /// Returns the number of elements.
        /// </summary>
        public int ElementCount
        {
            get 
            {
                return 0;
            }
        }

        /// <summary>
        /// The minimum zoom for this layer.
        /// </summary>
        public int MinZoom
        {
            get;
            set;
        }

        /// <summary>
        /// The maximum zoom for this layer.
        /// </summary>
        public int MaxZoom
        {
            get;
            set;
        }

        /// <summary>
        /// Adds a dot to this layer.
        /// </summary>
        /// <param name="dot"></param>
        /// <returns></returns>
        public ElementDot AddDot(GeoCoordinate dot)
        {
            throw new NotSupportedException();
        }
        /// <summary>
        /// Adds a line to this layer.
        /// </summary>
        /// <param name="dot"></param>
        /// <returns></returns>
        public ElementLine AddLine(ElementDot first, GeoCoordinate dot,bool create_dot)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Adds a line to this layer.
        /// </summary>
        /// <param name="dot"></param>
        /// <returns></returns>
        public ElementLine AddLine(ElementLine line, GeoCoordinate dot, bool create_dot)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Remove this element from the layer.
        /// </summary>
        /// <param name="dot"></param>
        /// <returns></returns>
        public void RemoveElement(IElement element)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Adds an element to the layer.
        /// </summary>
        /// <param name="element"></param>
        public void AddElement(IElement element)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region Remove Elements

        /// <summary>
        /// Removes all elements for the given geo object in all the sub layers.
        /// </summary>
        /// <param name="geo"></param>
        /// <param name="elements"></param>
        internal void RemoveElements(OsmGeo geo)
        {
            foreach (DataSourceSubLayer layer in _layers)
            {
                layer.RemoveElements(geo);
            }
        }

        #endregion

        #region ILayer Members


        public bool Visible
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/Sets the name of this layer.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        #endregion
    }
}
