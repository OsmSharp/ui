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

using OsmSharp.UI.Animations.Invalidation;
using OsmSharp.UI.Animations.Invalidation.Triggers;
using OsmSharp.UI.Renderer;

namespace OsmSharp.UI.Animations
{
    /// <summary>
    /// Abstract representation of a non-realtime rendering map surface.
    /// </summary>
    public interface IInvalidatableMapSurface
    {
		/// <summary>
        /// Returns true if the surface knows for sure that it will continue moving.
		/// </summary>
		bool StillMoving();

        /// <summary>
        /// Triggers the rendering.
        /// </summary>
        void Render();

        /// <summary>
        /// Cancels the rendering.
        /// </summary>
        void CancelRender();

        /// <summary>
        /// Registers the given invalidation listener.
        /// </summary>
        /// <param name="listener"></param>
        void RegisterListener(TriggerBase listener);

        /// <summary>
        /// Resets the current listener and disables automatic invalidation.
        /// </summary>
        void ResetListener();
    }
}