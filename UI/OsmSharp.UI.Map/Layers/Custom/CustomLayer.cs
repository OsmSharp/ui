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
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Osm.Map.Elements;
using System.Drawing;
using OsmSharp.Tools.Math.Shapes;
using OsmSharp.Tools.Math.Geo.Factory;

namespace OsmSharp.Osm.Map.Layers.Custom
{
    /// <summary>
    /// Custom layer class.
    /// 
    /// This layer can be used for adding thumbnails, poi's, images and other primitives to the map.
    /// </summary>
    public class CustomLayer : ILayer
    {
        /// <summary>
        /// The event raised when data or elements in this layer changed.
        /// </summary>
        public event Map.LayerChangedDelegate Changed;

        /// <summary>
        /// The unique id for this layer.
        /// </summary>
        private Guid _guid;

        /// <summary>
        /// Boolean containing the valid state.
        /// </summary>
        private bool _valid;

        /// <summary>
        /// The elements in this custom layer.
        /// </summary>
        private IList<IElement> _elements;

        /// <summary>
        /// Creates a new custom layer.
        /// </summary>
        public CustomLayer()
        {
            this.Visible = true;
            this.Name = "Custom Layer";

            _guid = Guid.NewGuid();
            _elements = new List<IElement>();
            _valid = false;

            this.MinZoom = -1;
            this.MaxZoom = -1;
        }

        #region ILayer Members

        public Guid Id
        {
            get 
            {
                return _guid;
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
                return new List<ILayer>();
            }
        }

        /// <summary>
        /// Returns the elements.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="zoom_factor"></param>
        /// <returns></returns>
        public IList<IElement> GetElements(GeoCoordinateBox box, double zoom_factor)
        {
            List<IElement> elements = new List<IElement>();
            List<IElement> local_elements; 
            lock (_elements)
            {
                local_elements = new List<IElement>(_elements);
            }

            foreach (IElement element in local_elements)
            {
                if (element.IsVisibleIn(box))
                {
                    elements.Add(element);
                }
            }

            return elements;
        }

        /// <summary>
        /// Returns the number of elements.
        /// </summary>
        public int ElementCount
        {
            get
            {
                return -1;
            }
        }

        /// <summary>
        /// Gets/sets the visible flag of this layer.
        /// </summary>
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
            ElementDot element = new ElementDot(
                Color.Black.ToArgb(),
                0.0002f,
                new OsmSharp.Tools.Math.Shapes.ShapeDotF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine>(PrimitiveGeoFactory.Instance, dot),
                false);

            lock (_elements)
            {
                _elements.Add(element);
            }
            return element;
        }

        /// <summary>
        /// Adds a line to this layer.
        /// </summary>
        /// <param name="dot"></param>
        /// <returns></returns>
        public ElementLine AddLine(ElementDot first, GeoCoordinate dot, bool create_dot)
        {
            // add new dot.
            if (create_dot)
            {
                this.AddDot(dot);
            }

            // create polyline.
            List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
            coordinates.Add(first.Dot.Point);
            coordinates.Add(dot);
            ShapePolyLineF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> line
                = new ShapePolyLineF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine>(
                    PrimitiveGeoFactory.Instance,
                    coordinates.ToArray());

            // create the line element.
            ElementLine element = new ElementLine(
                line,
                Color.Black.ToArgb(),
                0.0002f,
                true);

            lock (_elements)
            {
                _elements.Add(element);
            }

            return element;
        }

        /// <summary>
        /// Adds a line to this layer.
        /// </summary>
        /// <param name="dot"></param>
        /// <returns></returns>
        public ElementLine AddLine(
            GeoCoordinate dot1,
            GeoCoordinate dot2, 
            bool create_dot,
            double width,
            bool width_fixed,
            int color)
        {
            // add new dot.
            if (create_dot)
            {
                this.AddDot(dot1);
                this.AddDot(dot2);
            }

            // create polyline.
            List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
            coordinates.Add(dot1);
            coordinates.Add(dot2);
            ShapePolyLineF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> line
                = new ShapePolyLineF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine>(
                    PrimitiveGeoFactory.Instance,
                    coordinates.ToArray());

            // create the line element.
            ElementLine element = new ElementLine(
                line,
                color,
                width,
                width_fixed);
            //ElementLine element = new ElementLine(
            //    line,
            //    Color.FromArgb(230,Color.Blue).ToArgb(),
            //    0.0002f,
            //    true);

            lock (_elements)
            {
                _elements.Add(element);
            }

            return element;
        }

        /// <summary>
        /// Adds a line to this layer.
        /// </summary>
        /// <param name="dot"></param>
        /// <returns></returns>
        public ElementLine AddLine(ElementLine line, GeoCoordinate dot, bool create_dot)
        {
            lock (_elements)
            {
                // remove the old line.
                _elements.Remove(line);
            }

            // add new dot.
            if (create_dot)
            {
                this.AddDot(dot);
            }

            // create polyline.
            List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
            coordinates.AddRange(line.Line.Points);
            coordinates.Add(dot);

            ShapePolyLineF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> polyline
                = new ShapePolyLineF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine>(
                    PrimitiveGeoFactory.Instance,
                    coordinates.ToArray());

            // create the line element.
            ElementLine element = new ElementLine(
                polyline,
                Color.Black.ToArgb(),
                0.0002f,
                true);

            lock (_elements)
            {
                _elements.Add(element);
            }

            return element;
        }


        /// <summary>
        /// Remove this element from the layer.
        /// </summary>
        /// <param name="dot"></param>
        /// <returns></returns>
        public void RemoveElement(IElement element)
        {
            lock (_elements)
            {
                _elements.Remove(element);
            }
        }

        /// <summary>
        /// Removes all elements from this layer.
        /// </summary>
        public void Clear()
        {
            lock (_elements)
            {
                _elements.Clear();
            }
        }

        /// <summary>
        /// Adds an element to the layer.
        /// </summary>
        /// <param name="element"></param>
        public void AddElement(IElement element)
        {
            lock (_elements)
            {
                _elements.Add(element);
            }
        }


        #endregion

        public ElementImage AddImage(
            Image image,
            GeoCoordinate coordinate)
        {
            ElementImage image_element =
                new ElementImage(coordinate, image);
            lock (_elements)
            {
                _elements.Add(image_element);
            }

            return image_element;
        }

        public ElementText AddText(
            int color,
            float size,
            string text,
            GeoCoordinate coordinate)
        {
            ElementText element = new ElementText(color, new Font("Verdana", size), coordinate, text);
            lock (_elements)
            {
                _elements.Add(element);
            }

            return element;
        }
    }
}
