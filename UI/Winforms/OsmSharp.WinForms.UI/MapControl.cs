using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using OsmSharp.Tools;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.UI.Map;
using OsmSharp.UI.Map.Layers;
using OsmSharp.UI.Renderer;
using OsmSharp.WinForms.UI.Renderer;

namespace OsmSharp.WinForms.UI
{
    /// <summary>
    /// A map control.
    /// </summary>
    public partial class MapControl : UserControl
    {
        /// <summary>
        /// Creates a new map control.
        /// </summary>
        public MapControl()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            _renderer = new MapRenderer<Graphics>(new GraphicsRenderer2D());
        }

        /// <summary>
        /// The center coordinates.
        /// </summary>
        public GeoCoordinate Center { get; set; }

        /// <summary>
        /// The zoom factor.
        /// </summary>
        public float ZoomFactor { get; set; }

        /// <summary>
        /// Gets/sets the map.
        /// </summary>
        public Map Map { get; set; }

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

            Graphics g = e.Graphics;

            if (!_quickMode)
            {
                // set the nice graphics properties.
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            }
            else
            {
                // set the quickmode graphics properties.
                g.SmoothingMode = SmoothingMode.HighSpeed;
                g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                g.CompositingQuality = CompositingQuality.HighSpeed;
                g.InterpolationMode = InterpolationMode.Default;
            }

            // render the map.
            if (_quickMode)
            { // only render the cached scene.
                _renderer.RenderCache(g, this.Map, this.ZoomFactor, this.Center);
            }
            else
            { // render the entire scene.
                _renderer.Render(g, this.Map, this.ZoomFactor, this.Center);
            }

            long ticksAfter = DateTime.Now.Ticks;

            Console.WriteLine("Rendering took: {0}ms", 
                (new TimeSpan(ticksAfter - ticksBefore).TotalMilliseconds));
        }

        #endregion

        /// <summary>
        /// Raises the OnResize event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (this.Map != null && this.Center != null)
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
        /// Raises the onmousedown event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                _draggingCoordinates = new float[] { e.X, e.Y };
                _oldCenter = this.Center;
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

            if (e.Button == MouseButtons.Left &&
                _draggingCoordinates != null)
            {
                float[] currentCoordinates = new float[] { e.X, e.Y };
                float[] delta = new float[] { _draggingCoordinates[0] - currentCoordinates[0],
                        _draggingCoordinates[1] - currentCoordinates[1]};

                float[] newCenter = new float[] { (float)this.Width / 2.0f + delta[0], (float)this.Height / 2.0f + delta[1] };

                this.Center = _oldCenter;

                View2D view = _renderer.Create(this.Width, this.Height, this.Map, this.ZoomFactor, this.Center);

                float[] sceneCenter = view.ToViewPort(this.Width, this.Height,
                                                       newCenter[0], newCenter[1]);
                
                // project to new center.
                this.Center = this.Map.Projection.ToGeoCoordinates(sceneCenter[0], sceneCenter[1]);

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
            this.Invalidate();
        }

        /// <summary>
        /// Raises the mousewheel event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            this.ZoomFactor += (float)(e.Delta / 120.0);

            this.QueueNotifyMapViewChanged();
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

            // notify the map.
            this.Map.ViewChanged(this.ZoomFactor, this.Center, _renderer.Create(this.Width, this.Height, this.Map,
                                                                                  this.ZoomFactor, this.Center));
            this.Invalidate();
        }

        #endregion
    }
}