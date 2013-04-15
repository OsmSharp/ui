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
using OsmSharp.Osm.Map.Elements;
using OsmSharp.Tools.Math.Geo;

namespace OsmSharp.Osm.Map.Layers
{
    /// <summary>
    /// Represents a map layer.
    /// </summary>
    public interface ILayer
    {
        /// <summary>
        /// The event raised when data within the layer has changed.
        /// </summary>
        event OsmSharp.Osm.Map.Map.LayerChangedDelegate Changed;

        /// <summary>
        /// Returns the unique id for this layer.
        /// </summary>
        Guid Id
        {
            get;
        }

        /// <summary>
        /// Invalidates this layer.
        /// </summary>
        void Invalidate();

        /// <summary>
        /// Validates this layer.
        /// </summary>
        void Validate();

        /// <summary>
        /// Returns true if this layer is valid.
        /// </summary>
        bool IsValid();

        /// <summary>
        /// Returns the child layers in this layer.
        /// </summary>
        IList<ILayer> Layers
        {
            get;
        }

        /// <summary>
        /// Gets/sets the visible flag of a layer.
        /// </summary>
        bool Visible 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets/Sets the name of this layer.
        /// </summary>
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the elements in this layer.
        /// </summary>
        /// <returns></returns>
        IList<IElement> GetElements(GeoCoordinateBox box, double zoom_factor);

        /// <summary>
        /// Returns the number of elements in this layer.
        /// </summary>
        int ElementCount
        {
            get;
        }

        /// <summary>
        /// The minimum zoom a layer is visible at. (-1=unset)
        /// </summary>
        int MinZoom
        {
            get;
        }

        /// <summary>
        /// The maximum zoom al layer is visible at. (-1=unset)
        /// </summary>
        int MaxZoom
        {
            get;
        }

        /// <summary>
        /// Adds a new dot to the layer.
        /// </summary>
        /// <param name="dot"></param>
        ElementDot AddDot(GeoCoordinate dot);

        /// <summary>
        /// Adds a new line between the last dot of the line and a new dot at the given coordinates.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="geoCoordinate"></param>
        /// <returns></returns>
        ElementLine AddLine(ElementLine line, GeoCoordinate dot, bool create_dot);

        /// <summary>
        /// Adds a new line between the first dot and a new dot at the given coordinates.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="geoCoordinate"></param>
        /// <returns></returns>
        ElementLine AddLine(ElementDot first, GeoCoordinate dot, bool create_dot);

        /// <summary>
        /// Removes an element from this layer.
        /// </summary>
        /// <param name="element"></param>
        void RemoveElement(IElement element);

        /// <summary>
        /// Adds an element from this layer.
        /// </summary>
        /// <param name="element"></param>
        void AddElement(IElement element);
    }
}