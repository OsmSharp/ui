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

using OsmSharp.UI.Renderer;

namespace OsmSharp.UI.Animations.Invalidation.Triggers
{
    /// <summary>
    /// Base class for an invalidation triggering mechanism.
    /// </summary>
    public abstract class TriggerBase
    {
        /// <summary>
        /// Holds the invalidatable map surface.
        /// </summary>
        private IInvalidatableMapSurface _surface;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="surface"></param>
        public TriggerBase(IInvalidatableMapSurface surface)
        {
            _surface = surface;
        }

        /// <summary>
        /// Registers this trigger.
        /// </summary>
        public void Register()
        {
            _surface.RegisterListener(this);
        }

        /// <summary>
        /// Unregisters this trigger.
        /// </summary>
        public void Unregister()
        {
            _surface.ResetListener();
        }

        /// <summary>
        /// A way for the surface to complain that it is invalid.
        /// </summary>
        public virtual void Invalidate()
        {

        }

		/// <summary>
		/// Gets the surface.
		/// </summary>
		/// <value>The surface.</value>
		public IInvalidatableMapSurface Surface
		{
			get
			{
				return _surface;
			}
		}

        /// <summary>
        /// Triggers rendering.
        /// </summary>
        public virtual void Render()
        {
            _surface.CancelRender();
            _surface.Render();
		}

        /// <summary>
        /// Notifies the current view has changed.
        /// </summary>
        /// <param name="view">The view that was used for the latest rendering.</param>
        /// <param name="zoom">The zoom that was used for the latest rendering.</param>
        public abstract void NotifyChange(View2D view, double zoom);

        /// <summary>
        /// Notifies the rendering was a success.
        /// </summary>
        /// <param name="view">The view that was used for the latest rendering.</param>
        /// <param name="zoom">The zoom that was used for the latest rendering.</param>
        /// <param name="millis">The milliseconds that the previous rendering took.</param>
        public abstract void NotifyRenderSuccess(View2D view, double zoom, int millis);

        /// <summary>
        /// Stops this invalidation trigger.
        /// </summary>
        public abstract void Stop();
    }
}