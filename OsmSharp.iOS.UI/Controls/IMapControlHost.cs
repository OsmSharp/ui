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

using System;
using OsmSharp.UI;
#if __UNIFIED__
using UIKit;
#else
using MonoTouch.UIKit;
#endif

namespace OsmSharp.iOS.UI.Controls
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
		/// <remarks>This allows markers and controls to add subviews.</remarks>
		void AddView(UIView view);

		/// <summary>
		/// Removes a view from this host.
		/// </summary>
		/// <param name="view"></param>
		/// <remarks>This allows markers and controls to remove subviews.</remarks>
		void RemoveView(UIView view);

		/// <summary>
		/// Notifies this host that the given control has changed.
		/// </summary>
		/// <param name="control">Control.</param>
		void NotifyControlChange(MapControl control);

		/// <summary>
		/// Notifies this host that the control was clicked.
		/// </summary>
		/// <param name="control">Control.</param>
		void NotifyControlClicked(MapControl control);
	}
}

