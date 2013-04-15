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
using OsmSharp.Osm.Map.Elements;
using OsmSharp.Tools.Math.Geo;

namespace OsmSharp.Osm.Map.Layers
{
    /// <summary>
    /// Represents a layer inside a datasource layer.
    /// </summary>
    internal class DataSourceSubLayer : ILayer
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
        private IList<ILayer> _layers;

        /// <summary>
        /// The unique id of this layer.
        /// </summary>
        private Guid _id;

        /// <summary>
        /// Holds the valid/invalid flag of this layer.
        /// </summary>
        private bool _valid;

        /// <summary>
        /// Holds the elements per objects.
        /// </summary>
        IDictionary<long, List<IElement>> _elements_per_object;

        /// <summary>
        /// Holds the elements in this layer.
        /// </summary>
        List<IElement> _elements;

        /// <summary>
        /// Creates a new datasource sub layer.
        /// </summary>
        public DataSourceSubLayer()
        {
            this.Visible = true;
            this.Name = "Data Source Sub Layer";

            _valid = false;
            _id = Guid.NewGuid();

            this.MaxZoom = -1;
            this.MinZoom = -1;
        }

        #region ILayer Members

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
                return _layers;
            }
        }

        /// <summary>
        /// Returns the elements in this layer.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public IList<IElement> GetElements(GeoCoordinateBox box, double zoom_factor)
        {
            // TODO: implement the boundingbox cache.
            return _elements;
        }

        /// <summary>
        /// Returns the number of elements in this sub layer.
        /// </summary>
        public int ElementCount
        {
            get 
            {
                return _elements.Count;
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
        public ElementLine AddLine(ElementDot first, GeoCoordinate dot, bool create_dot)
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

        #region Internal Functions

        /// <summary>
        /// Adds elements for the given geo object.
        /// </summary>
        /// <param name="geo"></param>
        /// <param name="elements"></param>
        internal void AddElements(OsmGeo geo, IList<IElement> elements)
        {
            long id = geo.Id;

            // add list.
            if (_elements_per_object.ContainsKey(id))
            { 

            }
            else
            { // add new list for this object.
                _elements_per_object.Add(id, new List<IElement>());
            }
            
            // add elements.
            if (elements != null && elements.Count > 0)
            {
                _elements.AddRange(elements);
                _elements_per_object[id].AddRange(elements);
            }
        }

        /// <summary>
        /// Removes all elements for the given geo object.
        /// </summary>
        /// <param name="geo"></param>
        /// <param name="elements"></param>
        internal void RemoveElements(OsmGeo geo)
        {
            long id = geo.Id;
            // clear the list for the object.
            foreach (IElement element in _elements_per_object[id])
            {
                _elements.Remove(element);
            }
            _elements_per_object.Remove(id);
        }

        #endregion
    }
}
