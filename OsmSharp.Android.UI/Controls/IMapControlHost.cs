// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2014 Abelshausen Ben
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

using Android.Views;
using OsmSharp.UI;
using OsmSharp.UI.Map;
using OsmSharp.UI.Renderer;

namespace OsmSharp.Android.UI.Controls
{
    /// <summary>
    /// Abstract definition of a map marker host.
    /// </summary>
    public interface IMapControlHost : IMapView
    {
        /// <summary>
        /// Adds a view to this host.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="layout"></param>
        /// <remarks>This allows markers and controls to add subviews.</remarks>
        void AddView(View view, ViewGroup.LayoutParams layout);

        /// <summary>
        /// Removes a view from this host.
        /// </summary>
        /// <param name="view"></param>
        /// <remarks>This allows markers and controls to remove subviews.</remarks>
        void RemoveView(View view);

        /// <summary>
        /// Notifies this host that the given marker has changed.
        /// </summary>
        /// <param name="marker">Marker.</param>
        void NotifyControlChange(MapControl marker);

        /// <summary>
        /// Notifies this host that the control was clicked.
        /// </summary>
        /// <param name="control">Control.</param>
        void NotifyControlClicked(MapControl control);
    }
}