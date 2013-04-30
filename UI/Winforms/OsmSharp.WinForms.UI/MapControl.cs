using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
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
        /// Raises the OnPaint event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Bitmap bitmap = new Bitmap((int)e.Graphics.VisibleClipBounds.Width,
                           (int)e.Graphics.VisibleClipBounds.Height);
            Graphics g = Graphics.FromImage(bitmap);

            // set graphics properties.
            g.SmoothingMode = SmoothingMode.HighQuality;

            // initialize the renderers.
            var graphicsRenderer2D = new GraphicsRenderer2D(g);
            var mapRenderer = new MapRenderer<Graphics>(graphicsRenderer2D);

            // render the map.
            mapRenderer.Render(this.Map, this.ZoomFactor, this.Center);

            e.Graphics.DrawImageUnscaled(bitmap, 0, 0);
        }

        /// <summary>
        /// Raises the OnResize event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (this.Map != null && this.Center != null)
            {
                // TODO: find out a better way than creating renderers to get the view.
                // initialize the renderers.
                Graphics g = this.CreateGraphics();
                var graphicsRenderer2D = new GraphicsRenderer2D(g);
                var mapRenderer = new MapRenderer<Graphics>(graphicsRenderer2D);

                // notify the map.
                this.Map.ViewChanged(this.ZoomFactor, this.Center, mapRenderer.Create(this.Map,
                                                                                      this.ZoomFactor, this.Center));
                this.Invalidate();
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

                // TODO: find out a better way than creating renderers to get the view.
                // initialize the renderers.
                var graphicsRenderer2D = new GraphicsRenderer2D(this.CreateGraphics());
                var mapRenderer = new MapRenderer<Graphics>(graphicsRenderer2D);
                View2D view = mapRenderer.Create(this.Map, this.ZoomFactor, this.Center);

                float[] sceneCenter = view.ToViewPort(this.Width, this.Height,
                                                       newCenter[0], newCenter[1]);
                
                // project to new center.
                this.Center = this.Map.Projection.ToGeoCoordinates(sceneCenter[0], sceneCenter[1]);

                // notify the map.
                this.Map.ViewChanged(this.ZoomFactor, this.Center, mapRenderer.Create(this.Map,
                                                                                      this.ZoomFactor, this.Center));

                //_draggingCoordinates = currentCoordinates;
                this.Invalidate();
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
        }

        /// <summary>
        /// Raises the mousewheel event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            this.ZoomFactor += (float)(e.Delta / 120.0);

            // TODO: find out a better way than creating renderers to get the view.
            // initialize the renderers.
            Graphics g = this.CreateGraphics();
            var graphicsRenderer2D = new GraphicsRenderer2D(g);
            var mapRenderer = new MapRenderer<Graphics>(graphicsRenderer2D);

            // notify the map.
            this.Map.ViewChanged(this.ZoomFactor, this.Center, mapRenderer.Create(this.Map,
                                                                                  this.ZoomFactor, this.Center));
            this.Invalidate();
        }

        #endregion
    }
}