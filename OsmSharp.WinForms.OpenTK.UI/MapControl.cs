using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OsmSharp.Math.Geo;
using OsmSharp.UI;
using OsmSharp.UI.Map;
using OsmSharp.UI.Renderer;
using OsmSharp.Units.Angle;
using OsmSharp.WinForms.OpenTK.UI.Renderer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OsmSharp.WinForms.OpenTK.UI
{
    public class MapControl : GLControl, IMapView
    {
        /// <summary>
        /// Event raised when the map is touched.
        /// </summary>
        public event MapViewDelegates.MapTouchedDelegate MapTouched;

        /// <summary>
        /// Holds the target.
        /// </summary>
        private OpenTKTarget2D _target;

        /// <summary>
        /// Creates a new map control.
        /// </summary>
        public MapControl()
        {
            //this.DoubleBuffered = true;
            _target = new OpenTKTarget2D();
            _renderer = new MapRenderer<OpenTKTarget2D>(new OpenTKRenderer2D());

            this.MapAllowPan = true;
            this.MapAllowZoom = true;
        }

        /// <summary>
        /// Keep loaded status to make sure not to access any un-initialized functionality.
        /// </summary>
        private bool _isLoaded = false;

        private void SetupViewport()
        {
            int w = this.Width;
            int h = this.Height;
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, w, 0, h, -1, 1); // Bottom-left corner pixel has coordinate (0, 0)
            GL.Viewport(0, 0, w, h); // Use all of the glControl painting area
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

        private System.Threading.Timer _timer;

        /// <summary>
        /// Raises the OnLoad event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _isLoaded = true;
            GL.ClearColor(Color.SkyBlue);
            this.SetupViewport();

            _timer = new System.Threading.Timer(DeQueueNotifyMapViewChanged, null, 200, 200);
        }

        #region Rendering/Drawing

        /// <summary>
        /// Holds the map renderer.
        /// </summary>
        private MapRenderer<OpenTKTarget2D> _renderer;

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
            // do frame.
            if (_isLoaded)
            {
                long ticksBefore = DateTime.Now.Ticks;

                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                _target.OnDrawFrame();

                this.SwapBuffers();

                long ticksAfter = DateTime.Now.Ticks;

                Console.WriteLine("Rendering took: {0}ms @ zoom level {1}",
                    (new TimeSpan(ticksAfter - ticksBefore).TotalMilliseconds), this.MapZoom);
            }
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

            if (this.MapAllowPan &&
                e.Button == MouseButtons.Left &&
                _draggingCoordinates != null)
            {
                var currentCoordinates = new double[] { e.X, e.Y };
                var delta = new double[] { _draggingCoordinates[0] - currentCoordinates[0],
                        (_draggingCoordinates[1] - currentCoordinates[1])};

                var newCenter = new double[] { this.Width / 2.0f + delta[0], this.Height / 2.0f + delta[1] };

                this.MapCenter = _oldCenter;

                View2D view = _renderer.Create(this.Width, this.Height, this.Map,
                    (float)this.Map.Projection.ToZoomFactor(this.MapZoom), this.MapCenter, false, true);

                double[] sceneCenter = view.FromViewPort(this.Width, this.Height,
                                                       newCenter[0], newCenter[1]);

                // project to new center.
                this.MapCenter = this.Map.Projection.ToGeoCoordinates(sceneCenter[0], sceneCenter[1]);

                // notify the map.
                this.QueueNotifyMapViewChanged();
            }
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
                //this.Invalidate();
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
            View2D view = _renderer.Create(this.Width, this.Height, this.Map,
                (float)this.Map.Projection.ToZoomFactor(this.MapZoom), this.MapCenter, false, true);
            GL.Viewport(0, 0, this.Width, this.Height); // Use all of the glControl painting area
            _target.Width = this.Width;
            _target.Height = this.Height;
            _target.SetOrtho((float)view.LeftTop[0], (float)view.RightTop[0],
                (float)view.LeftTop[1], (float)view.RightBottom[1]);

            this.Invalidate();

            _isQueued = true;
        }

        /// <summary>
        /// Try to dequeu a queued event.
        /// </summary>
        /// <param name="state"></param>
        private void DeQueueNotifyMapViewChanged(object state)
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

            // make sure viewport is correct.
            // notify the map.
            lock (this.Map)
            {
                this.Map.ViewChanged((float)this.Map.Projection.ToZoomFactor(this.MapZoom), this.MapCenter,
                    _renderer.Create(this.Width, this.Height, this.Map,
                    (float)this.Map.Projection.ToZoomFactor(this.MapZoom), this.MapCenter, false, true));

                long ticksAfter = DateTime.Now.Ticks;

                Console.WriteLine("Map view changed notification took: {0}ms @ zoom level {1}",
                    (new TimeSpan(ticksAfter - ticksBefore).TotalMilliseconds), this.MapZoom);

                // execute rendering step.
                View2D view = _renderer.Create(this.Width, this.Height, this.Map,
                    (float)this.Map.Projection.ToZoomFactor(this.MapZoom), this.MapCenter, false, true);
                _renderer.Render(_target, this.Map, view, (float)this.Map.Projection.ToZoomFactor(this.MapZoom));
            }

            this.Invoke(new Empty(this.Invalidate));
        }

        private delegate void Empty();

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
                View2D view = _renderer.Create(this.Width, this.Height, this.Map,
                    (float)this.Map.Projection.ToZoomFactor(this.MapZoom), this.MapCenter, false, true);

                // get scene coordinates.
                double[] scenCoordinates = view.FromViewPort(this.Width, this.Height, e.X, e.Y);
                GeoCoordinate geoCoordinates = this.Map.Projection.ToGeoCoordinates(scenCoordinates[0],
                    scenCoordinates[1]);

                // create map user control event args.
                MapControlEventArgs args
                    = new MapControlEventArgs(e, geoCoordinates);

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
                double[] scenCoordinates = view.FromViewPort(this.Width, this.Height, e.X, e.Y);
                GeoCoordinate geoCoordinates = this.Map.Projection.ToGeoCoordinates(scenCoordinates[0],
                    scenCoordinates[1]);

                // create map user control event args.
                MapControlEventArgs args
                    = new MapControlEventArgs(e, geoCoordinates);

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
                View2D view = _renderer.Create(this.Width, this.Height, this.Map,
                    (float)this.Map.Projection.ToZoomFactor(this.MapZoom), this.MapCenter, false, true);

                // get scene coordinates.
                double[] scenCoordinates = view.FromViewPort(this.Width, this.Height, e.X, e.Y);
                GeoCoordinate geoCoordinates = this.Map.Projection.ToGeoCoordinates(scenCoordinates[0],
                    scenCoordinates[1]);

                // create map user control event args.
                MapControlEventArgs args
                    = new MapControlEventArgs(e, geoCoordinates);

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
                double[] scenCoordinates = view.FromViewPort(this.Width, this.Height, e.X, e.Y);
                GeoCoordinate geoCoordinates = this.Map.Projection.ToGeoCoordinates(scenCoordinates[0],
                    scenCoordinates[1]);

                // create map user control event args.
                MapControlEventArgs args
                    = new MapControlEventArgs(e, geoCoordinates);

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
                double[] scenCoordinates = view.FromViewPort(this.Width, this.Height, e.X, e.Y);
                GeoCoordinate geoCoordinates = this.Map.Projection.ToGeoCoordinates(scenCoordinates[0],
                    scenCoordinates[1]);

                // create map user control event args.
                MapControlEventArgs args
                    = new MapControlEventArgs(e, geoCoordinates);

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
                View2D view = _renderer.Create(this.Width, this.Height, this.Map,
                    (float)this.Map.Projection.ToZoomFactor(this.MapZoom), this.MapCenter, false, true);

                // get scene coordinates.
                double[] scenCoordinates = view.FromViewPort(this.Width, this.Height, e.X, e.Y);
                GeoCoordinate geoCoordinates = this.Map.Projection.ToGeoCoordinates(scenCoordinates[0],
                    scenCoordinates[1]);

                // create map user control event args.
                MapControlEventArgs args
                    = new MapControlEventArgs(e, geoCoordinates);

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

        void IMapView.RegisterAnimator(OsmSharp.UI.Animations.MapViewAnimator mapViewAnimator)
        {

        }

        void IMapView.SetMapView(GeoCoordinate center, Degree mapTilt, float mapZoom)
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
    }
}