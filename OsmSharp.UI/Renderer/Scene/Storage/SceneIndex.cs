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

using OsmSharp.UI.Renderer.Scene.Styles;
using ProtoBuf;
using System.Collections.Generic;

namespace OsmSharp.UI.Renderer.Scene.Storage
{
    /// <summary>
    /// Scene index.
    /// </summary>
    [ProtoContract]
    internal class SceneIndex
    {
        /// <summary>
        /// Holds the point styles.
        /// </summary>
        [ProtoMember(1)]
        public StylePoint[] PointStyles { get; set; }

        /// <summary>
        /// Holds the text styles.
        /// </summary>
        [ProtoMember(2)]
        public StyleText[] TextStyles { get; set; }

        /// <summary>
        /// Holds the line styles.
        /// </summary>
        [ProtoMember(3)]
        public StyleLine[] LineStyles { get; set; }

        /// <summary>
        /// Holds the polygon styles.
        /// </summary>
        [ProtoMember(4)]
        public StylePolygon[] PolygonStyles { get; set; }

        /// <summary>
        /// Holds the zoom ranges.
        /// </summary>
        [ProtoMember(5)]
        public Scene2DZoomRange[] ZoomRanges { get; set; }

        /// <summary>
        /// Holds the zoom factors.
        /// </summary>
        [ProtoMember(6)]
        public float[] ZoomFactors { get; set; }

        /// <summary>
        /// Holds the icon images.
        /// </summary>
        [ProtoMember(7)]
        public List<byte[]> IconImage { get; set; }
    }
}