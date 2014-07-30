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
using MonoTouch.UIKit;
using OsmSharp.UI.Renderer;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Math.Geo;

namespace OsmSharp.iOS.UI.Controls
{
    /// <summary>
    /// A wrapper around a view that can appear on top of the MapView.
    /// </summary>
    public abstract class MapControl : IDisposable
    {
        /// <summary>
        /// Returns the view baseclass.
        /// </summary>
        public abstract UIView BaseView
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
        /// Attaches this control to the given control host.
        /// </summary>
        /// <param name="controlHost">Map view.</param>
        internal abstract void AttachTo(IMapControlHost controlHost);

        /// <summary>
        /// Detaches this control to the given control host.
        /// </summary>
        /// <param name="controlHost">Control host.</param>
        internal abstract void DetachFrom(IMapControlHost controlHost);

        /// <summary>
        /// Sets layout.
        /// </summary>
        /// <param name="pixelsWidth"></param>
        /// <param name="pixelsHeight"></param>
        /// <param name="view"></param>
        /// <param name="projection"></param>
        /// <returns></returns>
        internal abstract bool SetLayout(double pixelsWidth, double pixelsHeight, View2D view, IProjection projection);

        public abstract void Dispose();
    }

    /// <summary>
    /// Map control.
    /// </summary>
    public class MapControl<TView> : MapControl
        where TView : UIView
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
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public MapControl(TView view, GeoCoordinate location, MapControlAlignmentType alignment, int width, int height)
        {
            _view = view;
            _location = location;
            _alignment = alignment;

            _view.Frame = new System.Drawing.RectangleF (new System.Drawing.PointF (0, 0), new System.Drawing.SizeF(width, height));
        }

        /// <summary>
        /// Holds this markers location.
        /// </summary>
        private GeoCoordinate _location;

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        public override GeoCoordinate Location {
            get {
                return _location;
            }
            set {
                _location = value;

                if (_controlHost != null) {
                    _controlHost.NotifyControlChange (this);
                }
            }
        } 

        /// <summary>
        /// Gets or sets the alignment.
        /// </summary>
        public MapControlAlignmentType Alighnment
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
        public override UIView BaseView
        {
            get
            {
                return this.View;
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
        /// <param name="controlHost">The control host.</param>
        internal override void DetachFrom(IMapControlHost controlHost)
        {
            _controlHost = null;
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
            double[] projected = projection.ToPixel (this.Location);
            double[] locationPixel = view.ToViewPort (pixelsWidth, pixelsHeight, projected [0], projected [1]);

            // set the new location depending on the size of the image and the alignment parameter.
            double leftMargin = locationPixel [0];// - this.Bitmap.Size.Width / 2.0;
            double topMargin = locationPixel [1];
            switch (_alignment) {
                case MapControlAlignmentType.CenterTop:
                    topMargin = locationPixel [1] + this.View.Frame.Height / 2.0;
                    break;
                case MapControlAlignmentType.CenterBottom:
                    topMargin = locationPixel [1] - this.View.Frame.Height / 2.0;
                    break;
            }
            this.View.Center = new System.Drawing.PointF ((float)leftMargin, (float)topMargin);

            return true;
        }

        public override void Dispose()
        {

        }
    }
}