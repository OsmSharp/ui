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

using Android.Views;
using Android.Widget;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.UI.Renderer;
using System;

namespace OsmSharp.Android.UI.Controls
{
    /// <summary>
    /// A wrapper around a view that can appear on top of the MapView.
    /// </summary>
    public abstract class MapControl : IDisposable
    {
        /// <summary>
        /// Returns the view baseclass.
        /// </summary>
        public abstract View BaseView
        {
            get;
        }

        /// <summary>
        /// Returns the handle.
        /// </summary>
        public abstract IntPtr Handle
        {
            get;
        }


        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        public abstract GeoCoordinate Location
        {
            get;
            set;
        }        
        
        /// <summary>
        /// Gets or sets the move with map flag.
        /// </summary>
        /// <value><c>true</c> if move with map; otherwise, <c>false</c>.</value>
        /// <remarks>When false, this control will not move along with the map.</remarks>
        public bool MoveWithMap
        {
            get;
            set;
        }

        /// <summary>
        /// holds pointer to user-object
        /// </summary>
        public virtual object Tag { get; set; }

        /// <summary>
        /// Attaches this control to the given control host.
        /// </summary>
        /// <param name="controlHost">Map view.</param>
        internal abstract void AttachTo(IMapControlHost controlHost);

        /// <summary>
        /// Detaches this control from the given control host.
        /// </summary>
        /// <param name="controlHost">Map view.</param>
        internal abstract void DetachFrom(IMapControlHost controlHost);

        /// <summary>
        /// Called when any map control has changed and is about to be reposition if needed.
        /// </summary>
        internal virtual void OnBeforeSetLayout()
        {

        }

        /// <summary>
        /// Sets layout.
        /// </summary>
        /// <param name="pixelsWidth"></param>
        /// <param name="pixelsHeight"></param>
        /// <param name="view"></param>
        /// <param name="projection"></param>
        /// <param name="afterLayout"></param>
        /// <returns></returns>
        internal abstract bool SetLayout(double pixelsWidth, double pixelsHeight, View2D view, IProjection projection);

        /// <summary>
        /// Called when any map control has changed and was repositioned if needed.
        /// </summary>
        internal virtual void OnAfterSetLayout()
        {

        }

        /// <summary>
        /// Notifies this control there was a map tap.
        /// </summary>
        protected internal abstract void NotifyMapTap();

        /// <summary>
        /// Notifies this control another control was clicked.
        /// </summary>
        protected internal abstract void NotifyOtherControlClicked();

        /// <summary>
        /// Releases all resource used by the <see cref="OsmSharp.Android.UI.Controls.MapControl"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the
        /// <see cref="OsmSharp.Android.UI.Controls.MapControl"/>. The <see cref="Dispose"/> method leaves the
        /// <see cref="OsmSharp.Android.UI.Controls.MapControl"/> in an unusable state. After calling
        /// <see cref="Dispose"/>, you must release all references to the
        /// <see cref="OsmSharp.Android.UI.Controls.MapControl"/> so the garbage collector can reclaim the memory that
        /// the <see cref="OsmSharp.Android.UI.Controls.MapControl"/> was occupying.</remarks>
        public abstract void Dispose();
    }

    /// <summary>
    /// A wrapper around a view that can appear on top of the MapView.
    /// </summary>
    /// <typeparam name="TView"></typeparam>
    public class MapControl<TView> : MapControl
        where TView : View
    {
        /// <summary>
        /// Holds the view being displayed.
        /// </summary>
        private TView _view;

        /// <summary>
        /// Holds the control host where this 
        /// </summary>
        private IMapControlHost _controlHost;

        /// <summary>
        /// Holds the default alignment.
        /// </summary>
        private MapControlAlignmentType _alignment;

        /// <summary>
        /// Creates a MapControl based on the given view.
        /// </summary>
        /// <param name="location">The location the view has to stay at.</param>
        /// <param name="view">The view being wrapped.</param>
        /// <param name="alignment">The alignment.</param>
        protected MapControl(TView view, GeoCoordinate location, MapControlAlignmentType alignment)
        {
            _view = view;
            _location = location;
            _alignment = alignment;

            this.MoveWithMap = true;
        }

        /// <summary>
        /// Creates a MapControl based on the given view.
        /// </summary>
        /// <param name="location">The location the view has to stay at.</param>
        /// <param name="view">The view being wrapped.</param>
        /// <param name="alignment">The alignment.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public MapControl(TView view, GeoCoordinate location, MapControlAlignmentType alignment, int width, int height)
        {
            _view = view;
            _location = location;
            _alignment = alignment;

            this.MoveWithMap = true;
            this.SetSize(width, height);
        }

        /// <summary>
        /// Sets the size of this control.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        protected void SetSize(int width, int height)
        {
            _view.SetMinimumWidth(width);
            _view.SetMinimumHeight(height);

            var layoutParams = new FrameLayout.LayoutParams(width, height + 5);
            layoutParams.LeftMargin = -1;
            layoutParams.TopMargin = -1;
            layoutParams.Gravity = GravityFlags.Top | GravityFlags.Left;
            _view.LayoutParameters = layoutParams;
        }

        /// <summary>
        /// Returns the view.
        /// </summary>
        public TView View
        {
            get
            {
                return _view;
            }
        }

        /// <summary>
        /// Returns the base view.
        /// </summary>
        public override View BaseView
        {
            get
            {
                return this.View;
            }
        }

        /// <summary>
        /// Returns the handle.
        /// </summary>
        public override IntPtr Handle
        {
            get
            {
                return _view.Handle;
            }
        }

        /// <summary>
        /// Holds this markers location.
        /// </summary>
        private GeoCoordinate _location;

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        public override GeoCoordinate Location
        {
            get
            {
                return _location;
            }
            set
            {
                _location = value;

                if (_controlHost != null && _location != null)
                {
                    _controlHost.NotifyControlChange(this);
                }
            }
        }

        /// <summary>
        /// Gets or sets the alignment.
        /// </summary>
        public MapControlAlignmentType Alignment
        {
            get
            {
                return _alignment;
            }
            set
            {
                _alignment = value;

                if (_controlHost != null && _location != null)
                {
                    _controlHost.NotifyControlChange(this);
                }
            }
        }

        /// <summary>
        /// Attaches this control to the given control host.
        /// </summary>
        /// <param name="controlHost">Map view.</param>
        internal override void AttachTo(IMapControlHost controlHost)
        {
            _controlHost = controlHost;
        }

        /// <summary>
        /// Detaches this control from the given control host.
        /// </summary>
        /// <param name="controlHost">Map view.</param>
        internal override void DetachFrom(IMapControlHost controlHost)
        {
            _controlHost = null;
        }

        /// <summary>
        /// Returns the current control host.
        /// </summary>
        protected IMapControlHost Host
        {
            get
            {
                return _controlHost;
            }
        }

        /// <summary>
        /// Sets the layout.
        /// </summary>
        /// <param name="pixelsWidth">Pixels width.</param>
        /// <param name="pixelsHeight">Pixels height.</param>
        /// <param name="view">View.</param>
        /// <param name="projection">Projection.</param>
        internal override bool SetLayout(double pixelsWidth, double pixelsHeight, View2D view, IProjection projection)
        {
            if (this.MoveWithMap)
            { // keep location the same and move with map.
                if (this.Location != null)
                { // only set layout if there is a location set.
                    var projected = projection.ToPixel(this.Location);
                    double leftMargin, topMargin;
                    var fromMatrix = view.CreateToViewPort(pixelsWidth, pixelsHeight);
                    fromMatrix.Apply(projected[0], projected[1], out leftMargin, out topMargin);

                    leftMargin = leftMargin - (this.View.LayoutParameters as FrameLayout.LayoutParams).Width / 2.0;

                    switch (_alignment)
                    {
                        case MapControlAlignmentType.Center:
                            topMargin = topMargin - (this.View.LayoutParameters as FrameLayout.LayoutParams).Height / 2.0;
                            break;
                        case MapControlAlignmentType.CenterTop:
                            break;
                        case MapControlAlignmentType.CenterBottom:
                            topMargin = topMargin - (this.View.LayoutParameters as FrameLayout.LayoutParams).Height;
                            break;
                    }

                    (this.View.LayoutParameters as FrameLayout.LayoutParams).LeftMargin = (int)leftMargin;
                    (this.View.LayoutParameters as FrameLayout.LayoutParams).TopMargin = (int)topMargin;
                }
            }
            else
            { // do not move with map but change the location.
                var locationProjected = view.FromViewPort(pixelsWidth, pixelsHeight, 
                    this.View.Left + this.View.Width / 2.0f, this.View.Top + this.View.Height / 2.0f);
                _location = projection.ToGeoCoordinates(locationProjected[0], locationProjected[1]);
            }
            return true;
        }

        /// <summary>
        /// Notifies this control there was a map tap.
        /// </summary>
        protected internal override void NotifyMapTap()
        {

        }

        /// <summary>
        /// Notifies this control another control was clicked.
        /// </summary>
        protected internal override void NotifyOtherControlClicked()
        {

        }


        /// <summary>
        /// Releases all resource used by the MapControl object.
        /// </summary>
        public override void Dispose()
        {

        }
    }
}