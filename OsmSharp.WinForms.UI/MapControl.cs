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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using OsmSharp.Math.Geo;
using OsmSharp.UI;
using OsmSharp.UI.Map;
using OsmSharp.UI.Renderer;
using OsmSharp.Units.Angle;
using OsmSharp.WinForms.UI.Renderer;

namespace OsmSharp.WinForms.UI
{
    /// <summary>
    /// A map control.
    /// </summary>
    public partial class MapControl : UserControl, IMapView
    {
        /// <summary>
        /// Event raised when the map was first touched.
        /// </summary>
        public event MapViewDelegates.MapTouchedDelegate MapTouchedDown;

        /// <summary>
        /// Event raised when the map is touched.
        /// </summary>
        public event MapViewDelegates.MapTouchedDelegate MapTouched;

        /// <summary>
        /// Event raised after the map was touched.
        /// </summary>
        public event MapViewDelegates.MapTouchedDelegate MapTouchedUp;

        /// <summary>
        /// Raised when the map is moved.
        /// </summary>
        public event MapViewDelegates.MapMoveDelegate MapMove;

        /// <summary>
        /// Raised when the map was first initialized, meaning it has a size and it was rendered for the first time.
        /// </summary>
        public event MapViewDelegates.MapInitialized MapInitialized;

        /// <summary>
        /// Creates a new map control.
        /// </summary>
        public MapControl()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            _renderer = new MapRenderer<Graphics>(new GraphicsRenderer2D());

            this.MapAllowPan = true;
            this.MapAllowZoom = true;
        }

        /// <summary>
        /// The center coordinates.
        /// </summary>
        public GeoCoordinate MapCenter { get; set; }

        /// <summary>
        /// The zoom factor.
        /// </summary>
        public float MapZoom { get; set; }

        /// <summary>
        /// Gets or sets the MapTilt.
        /// </summary>
        public Degree MapTilt { get; set; }

        /// <summary>
        /// Gets the current view.
        /// </summary>
        public View2D CurrentView
        {
            get
            {
                return _renderer.Create(this.Width, this.Height, this.Map,
                    (float)this.Map.Projection.ToZoomFactor(this.MapZoom), this.MapCenter, false, true);
            }
        }

        /// <summary>
        /// Returns the current height.
        /// </summary>
        public int CurrentHeight
        {
            get { return this.Width; }
        }

        /// <summary>
        /// Returns the current width.
        /// </summary>
        public int CurrentWidth
        {
            get { return this.Height; }
        }

        /// <summary>
        /// Holds the map.
        /// </summary>
        private OsmSharp.UI.Map.Map _map;

        /// <summary>
        /// Gets/sets the map.
        /// </summary>
        public OsmSharp.UI.Map.Map Map
        {
            get { return _map; }
            set
            {
                if (_map != null)
                {
                    _map.MapChanged -= new OsmSharp.UI.Map.Map.MapChangedDelegate(_map_MapChanged);
                }
                _map = value;
                if (_map != null)
                {
                    _map.MapChanged += new OsmSharp.UI.Map.Map.MapChangedDelegate(_map_MapChanged);
                }
            }
        }

        /// <summary>
        /// Called when the map has changed.
        /// </summary>
        void _map_MapChanged()
        {
            // invalidate.
            this.Invalidate();
        }

        /// <summary>
        /// Raises the OnLoad event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var timer = new System.Windows.Forms.Timer();
            timer.Interval = 200;
            timer.Tick += new EventHandler(DeQueueNotifyMapViewChanged);
            timer.Enabled = true;
        }

        #region Rendering/Drawing

        /// <summary>
        /// Holds the map renderer.
        /// </summary>
        private MapRenderer<Graphics> _renderer; 

        /// <summary>
        /// Holds the quickmode boolean.
        /// </summary>
        private bool _quickMode = false;

        /// <summary>
        /// Raises the OnPaint event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            long ticksBefore = DateTime.Now.Ticks;

            var g = e.Graphics;

            if (!_quickMode)
            { // set the nice graphics properties.
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            }
            else
            { // set the quickmode graphics properties.
                g.SmoothingMode = SmoothingMode.HighSpeed;
                g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                g.CompositingQuality = CompositingQuality.HighSpeed;
                g.InterpolationMode = InterpolationMode.Default;
            }

            // render the map.
            var view = _renderer.Create(this.Width, this.Height, this.Map,
                (float)this.Map.Projection.ToZoomFactor(this.MapZoom), this.MapCenter, false, true);
            if (_quickMode)
            { // only render the cached scene.
                _renderer.Render(g, this.Map, view, (float)this.Map.Projection.ToZoomFactor(this.MapZoom));
            }
            else
            { // render the entire scene.
                _renderer.Render(g, this.Map, view, (float)this.Map.Projection.ToZoomFactor(this.MapZoom));
            }

            long ticksAfter = DateTime.Now.Ticks;

            Console.WriteLine("Rendering took: {0}ms @ zoom level {1}",
                (new TimeSpan(ticksAfter - ticksBefore).TotalMilliseconds), this.MapZoom);
        }

        #endregion

        /// <summary>
        /// Raises the OnResize event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (this.Map != null && this.MapCenter != null)
            {
                // notify the map.
                this.QueueNotifyMapViewChanged();
            }
        }

        #region Drag/Zoom

        /// <summary>
        /// Coordinates where dragging started.
        /// </summary>
        private float[] _draggingCoordinates;

        /// <summary>
        /// Coordinates of the old center.
        /// </summary>
        private GeoCoordinate _oldCenter;

        /// <summary>
        /// Gets or sets the map tilt flag.
        /// </summary>
        public bool MapAllowTilt
        {
            get
            {
                return false;
            }
            set
            {
                // no map tilt functionality.
            }
        }

        /// <summary>
        /// Gets or sets the map pan flag.
        /// </summary>
        public bool MapAllowPan
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the map zoom flag.
        /// </summary>
        public bool MapAllowZoom
        {
            get;
            set;
        }

        /// <summary>
        /// Raises the onmousedown event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                _draggingCoordinates = new float[] { e.X, e.Y };
                _oldCenter = this.MapCenter;
                _quickMode = true;
            }
        }

        /// <summary>
        /// Raises the mousemove event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var currentCoordinates = new double[] { e.X, e.Y };
            if (this.MapAllowPan && 
                e.Button == MouseButtons.Left &&
                _draggingCoordinates != null)
            {
                var delta = new double[] { _draggingCoordinates[0] - currentCoordinates[0],
                        (_draggingCoordinates[1] - currentCoordinates[1])};

                var newCenter = new double[] { this.Width / 2.0f + delta[0], this.Height / 2.0f + delta[1] };

                this.MapCenter = _oldCenter;

                View2D view = _renderer.Create(this.Width, this.Height, this.Map, 
                    (float)this.Map.Projection.ToZoomFactor(this.MapZoom), this.MapCenter, false, true);

                double x, y;
                var fromMatrix = view.CreateFromViewPort(this.Width, this.Height);
                fromMatrix.Apply(newCenter[0], newCenter[1], out x, out y);
                
                // project to new center.
                this.MapCenter = this.Map.Projection.ToGeoCoordinates(x, y);

                // notify the map.
                this.QueueNotifyMapViewChanged();
            }
            this.RaiseOnMapMouseMove(e);
        }

        /// <summary>
        /// Raises the mouseup event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            _draggingCoordinates = null;
            _quickMode = false;
            if (this.MapAllowPan)
            {
                this.Invalidate();
            }
        }

        /// <summary>
        /// Raises the mousewheel event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (this.MapAllowZoom)
            {
                this.MapZoom += (float)(e.Delta / 2000.0);

                this.QueueNotifyMapViewChanged();
            }
        }

        /// <summary>
        /// An event is queued already.
        /// </summary>
        private bool _isQueued = false;

        /// <summary>
        /// Queues a notifiy map changed event.
        /// </summary>
        private void QueueNotifyMapViewChanged()
        {
            _isQueued = true;

            this.Invalidate();
        }

        /// <summary>
        /// Try to dequeu a queued event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeQueueNotifyMapViewChanged(object sender, EventArgs e)
        {
            if (_isQueued)
            {
                this.NotifyMapViewChanged();
                _isQueued = false;
            }
        }

        /// <summary>
        /// Notifies that the mapview has changed.
        /// </summary>
        private void NotifyMapViewChanged()
        {
            long ticksBefore = DateTime.Now.Ticks;

            if (this.Height == 0 || this.Width == 0) { return; }

            // notify the map.
            var view = _renderer.Create(this.Width, this.Height, this.Map,
                (float)this.Map.Projection.ToZoomFactor(this.MapZoom), this.MapCenter, false, true);
            this.Map.ViewChanged((float)this.Map.Projection.ToZoomFactor(this.MapZoom), this.MapCenter, 
                view, view);

            long ticksAfter = DateTime.Now.Ticks;

            Console.WriteLine("Map view changed notification took: {0}ms @ zoom level {1}",
                (new TimeSpan(ticksAfter - ticksBefore).TotalMilliseconds), this.MapZoom);

            this.Invalidate();
        }

        #endregion

        #region Events

        /// <summary>
        /// Delegate used to define events from this map control.
        /// </summary>
        /// <param name="e"></param>
        public delegate void MapMouseEventDelegate(MapControlEventArgs e);

        #region MapMouseUp

        /// <summary>
        /// The map mouse up event.
        /// </summary>
        public event MapMouseEventDelegate MapMouseUp;

        /// <summary>
        /// Raises the OnMapMouseUp event.
        /// </summary>
        /// <param name="e"></param>
        private void RaiseOnMapMouseUp(MouseEventArgs e)
        {
            if (this.Map != null)
            {
                var view = _renderer.Create(this.Width, this.Height, this.Map,
                    (float)this.Map.Projection.ToZoomFactor(this.MapZoom), this.MapCenter, false, true);

                // get scene coordinates.
                double x, y;
                var fromMatrix = view.CreateFromViewPort(this.Width, this.Height);
                fromMatrix.Apply(e.X, e.Y, out x, out y);
                var geoCoordinates = this.Map.Projection.ToGeoCoordinates(x, y);

                // create map user control event args.
                var args = new MapControlEventArgs(e, geoCoordinates);
                this.OnMapMouseUp(args);
                if (MapMouseUp != null)
                {
                    MapMouseUp(args);
                }
            }
        }

        /// <summary>
        /// Called on a mouse up event.
        /// </summary>
        /// <param name="e"></param>
        protected void OnMapMouseUp(MapControlEventArgs e)
        {

        }

        #endregion

        #region MapMouseDown

        /// <summary>
        /// The map mouse up event.
        /// </summary>
        public event MapMouseEventDelegate MapMouseDown;

        /// <summary>
        /// Raises the OnMapMouseDown event.
        /// </summary>
        /// <param name="e"></param>
        private void RaiseOnMapMouseDown(MouseEventArgs e)
        {
            if (this.Map != null)
            {
                View2D view = _renderer.Create(this.Width, this.Height, this.Map,
                    (float)this.Map.Projection.ToZoomFactor(this.MapZoom), this.MapCenter, false, true);

                // get scene coordinates.
                double x, y;
                var fromMatrix = view.CreateFromViewPort(this.Width, this.Height);
                fromMatrix.Apply(e.X, e.Y, out x, out y);
                var geoCoordinates = this.Map.Projection.ToGeoCoordinates(x, y);

                // create map user control event args.
                var args = new MapControlEventArgs(e, geoCoordinates);
                this.OnMapMouseDown(args);
                if (MapMouseDown != null)
                {
                    MapMouseDown(args);
                }
            }
        }
        /// <summary>
        /// Called on a mouse up event.
        /// </summary>
        /// <param name="e"></param>
        protected void OnMapMouseDown(MapControlEventArgs e)
        {

        }

        #endregion

        #region MapMouseMove

        /// <summary>
        /// The map mouse up event.
        /// </summary>
        public event MapMouseEventDelegate MapMouseMove;

        /// <summary>
        /// Raises the OnMapMouseMove event.
        /// </summary>
        /// <param name="e"></param>
        private void RaiseOnMapMouseMove(MouseEventArgs e)
        {
            if (this.Map != null)
            {
                var view = _renderer.Create(this.Width, this.Height, this.Map,
                    (float)this.Map.Projection.ToZoomFactor(this.MapZoom), this.MapCenter, false, true);

                // get scene coordinates.
                double x, y;
                var fromMatrix = view.CreateFromViewPort(this.Width, this.Height);
                fromMatrix.Apply(e.X, e.Y, out x, out y);
                var geoCoordinates = this.Map.Projection.ToGeoCoordinates(x, y);

                // create map user control event args.
                var args = new MapControlEventArgs(e, geoCoordinates);
                this.OnMapMouseMove(args);
                if (MapMouseMove != null)
                {
                    MapMouseMove(args);
                }
            }
        }

        /// <summary>
        /// Called on a mouse up event.
        /// </summary>
        /// <param name="e"></param>
        protected void OnMapMouseMove(MapControlEventArgs e)
        {

        }

        #endregion

        #region MapMouseWheel

        /// <summary>
        /// The map mouse up event.
        /// </summary>
        public event MapMouseEventDelegate MapMouseWheel;

        /// <summary>
        /// Raises the OnMapMouseWheel event.
        /// </summary>
        /// <param name="e"></param>
        private void RaiseOnMapMouseWheel(MouseEventArgs e)
        {
            if (this.Map != null)
            {
                View2D view = _renderer.Create(this.Width, this.Height, this.Map,
                    (float)this.Map.Projection.ToZoomFactor(this.MapZoom), this.MapCenter, false, true);

                // get scene coordinates.
                double x, y;
                var fromMatrix = view.CreateFromViewPort(this.Width, this.Height);
                fromMatrix.Apply(e.X, e.Y, out x, out y);
                var geoCoordinates = this.Map.Projection.ToGeoCoordinates(x, y);

                // create map user control event args.
                var args = new MapControlEventArgs(e, geoCoordinates);
                this.OnMapMouseWheel(args);
                if (MapMouseWheel != null)
                {
                    MapMouseWheel(args);
                }
            }
        }
        /// <summary>
        /// Called on a mouse up event.
        /// </summary>
        /// <param name="e"></param>
        protected void OnMapMouseWheel(MapControlEventArgs e)
        {

        }

        #endregion

        #region MapMouseDoubleClick

        /// <summary>
        /// The map mouse up event.
        /// </summary>
        public event MapMouseEventDelegate MapMouseDoubleClick;

        /// <summary>
        /// Raises the OnMapMouseDoubleClick event.
        /// </summary>
        /// <param name="e"></param>
        private void RaiseOnMapMouseDoubleClick(MouseEventArgs e)
        {
            if (this.Map != null)
            {
                View2D view = _renderer.Create(this.Width, this.Height, this.Map,
                    (float)this.Map.Projection.ToZoomFactor(this.MapZoom), this.MapCenter, false, true);

                // get scene coordinates.
                double x, y;
                var fromMatrix = view.CreateFromViewPort(this.Width, this.Height);
                fromMatrix.Apply(e.X, e.Y, out x, out y);
                var geoCoordinates = this.Map.Projection.ToGeoCoordinates(x, y);

                // create map user control event args.
                var args = new MapControlEventArgs(e, geoCoordinates);
                this.OnMapMouseDoubleClick(args);
                if (MapMouseDoubleClick != null)
                {
                    MapMouseDoubleClick(args);
                }
            }
        }

        /// <summary>
        /// Called on mouse double click.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            this.RaiseOnMapMouseDoubleClick(e);
        }

        /// <summary>
        /// Called on a mouse up event.
        /// </summary>
        /// <param name="e"></param>
        protected void OnMapMouseDoubleClick(MapControlEventArgs e)
        {

        }

        #endregion

        #region MapMouseClick

        /// <summary>
        /// The map mouse up event.
        /// </summary>
        public event MapMouseEventDelegate MapMouseClick;

        /// <summary>
        /// Raises the OnMapMouseClick event.
        /// </summary>
        /// <param name="e"></param>
        private void RaiseOnMapMouseClick(MouseEventArgs e)
        {
            if (this.Map != null)
            {
                var view = _renderer.Create(this.Width, this.Height, this.Map,
                    (float)this.Map.Projection.ToZoomFactor(this.MapZoom), this.MapCenter, false, true);

                // get scene coordinates.
                double x, y;
                var fromMatrix = view.CreateFromViewPort(this.Width, this.Height);
                fromMatrix.Apply(e.X, e.Y, out x, out y);
                var geoCoordinates = this.Map.Projection.ToGeoCoordinates(x, y);

                // create map user control event args.
                var args = new MapControlEventArgs(e, geoCoordinates);
                this.OnMapMouseClick(args);
                if (MapMouseClick != null)
                {
                    MapMouseClick(args);
                }
            }
        }

        /// <summary>
        /// Called on mouse click.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            this.RaiseOnMapMouseClick(e);
        }

        /// <summary>
        /// Called on a mouse up event.
        /// </summary>
        /// <param name="e"></param>
        protected void OnMapMouseClick(MapControlEventArgs e)
        {

        }

        #endregion

        #endregion

		#region IMapView implementation

		void IMapView.RegisterAnimator (OsmSharp.UI.Animations.MapViewAnimator mapViewAnimator)
		{

		}

		void IMapView.SetMapView (GeoCoordinate center, Degree mapTilt, float mapZoom)
		{

		}

		#endregion

        /// <summary>
        /// Notifies a map change.
        /// </summary>
        /// <param name="pixelsWidth"></param>
        /// <param name="pixelsHeight"></param>
        /// <param name="view"></param>
        /// <param name="projection"></param>
        public void NotifyMapChange(double pixelsWidth, double pixelsHeight, View2D view, Math.Geo.Projections.IProjection projection)
        {

        }

        /// <summary>
        /// Returns the density.
        /// </summary>
        public float Density
        {
            get { return 1; }
        }

        /// <summary>
        /// Gets or sets the map bounding box.
        /// </summary>
        public GeoCoordinateBox MapBoundingBox
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the minimum zoom level.
        /// </summary>
        public float MapMinZoomLevel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum zoom level.
        /// </summary>
        public float MapMaxZoomLevel
        {
            get;
            set;
        }
    }
}