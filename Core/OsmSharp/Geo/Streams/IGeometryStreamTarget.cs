// OsmSharp - OpenStreetMap tools & library.
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

namespace OsmSharp.Geo.Geometries.Streams
{
    /// <summary>
    /// Represents a geometry target accepting geometry objects for processing.
    /// </summary>
    public interface IGeometryStreamTarget
    {
        /// <summary>
        /// Intializes this target.
        /// </summary>
        /// <rremarks>Has to be called before starting to add objects.</rremarks>
        void Initialize();

        /// <summary>
        /// Adds a new geometry.
        /// </summary>
        /// <param name="geometry"></param>
        void AddGeometry(Geometry geometry);

        /// <summary>
        /// Closes this target.
        /// </summary>
        /// <remarks>Closes any open connections, file locks or anything related to this target.</remarks>
        void Close();
    }
}
