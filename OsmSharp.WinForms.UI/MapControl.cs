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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using OsmSharp;
using OsmSharp.Math.Geo;
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
        public float ZoomLevel { get; set; }

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
        }}

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
            View2D view = _renderer.Create(this.Width, this.Height, this.Map,
                (float)this.Map.Projection.ToZoomFactor(this.ZoomLevel), this.Center, false, true);
            if (_quickMode)
            { // only render the cached scene.
                //_renderer.Render(g, this.Map, (float)this.Map.Projection.ToZoomFactor(this.ZoomLevel), this.Center);
                _renderer.RenderCache(g, this.Map, view);
            }
            else
            { // render the entire scene.
                _renderer.Render(g, this.Map, view);
            }

            long ticksAfter = DateTime.Now.Ticks;

            Console.WriteLine("Rendering took: {0}ms @ zoom level {1}",
                (new TimeSpan(ticksAfter - ticksBefore).TotalMilliseconds), this.ZoomLevel);
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
                var currentCoordinates = new double[] { e.X, e.Y };
                var delta = new double[] { _draggingCoordinates[0] - currentCoordinates[0],
                        (_draggingCoordinates[1] - currentCoordinates[1])};

                var newCenter = new double[] { this.Width / 2.0f + delta[0], this.Height / 2.0f + delta[1] };

                this.Center = _oldCenter;

                View2D view = _renderer.Create(this.Width, this.Height, this.Map, 
                    (float)this.Map.Projection.ToZoomFactor(this.ZoomLevel), this.Center, false, true);

                double[] sceneCenter = view.FromViewPort(this.Width, this.Height,
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

            this.ZoomLevel += (float)(e.Delta / 2000.0);

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
            long ticksBefore = DateTime.Now.Ticks;

            if (this.Height == 0 || this.Width == 0) { return; }

            // notify the map.
            this.Map.ViewChanged((float)this.Map.Projection.ToZoomFactor(this.ZoomLevel), this.Center, _renderer.Create(this.Width, this.Height, this.Map,
                                                                                  (float)this.Map.Projection.ToZoomFactor(this.ZoomLevel), this.Center, false, true));

            long ticksAfter = DateTime.Now.Ticks;

            Console.WriteLine("Map view changed notification took: {0}ms @ zoom level {1}",
                (new TimeSpan(ticksAfter - ticksBefore).TotalMilliseconds), this.ZoomLevel);

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
                View2D view = _renderer.Create(this.Width, this.Height, this.Map,
                    (float)this.Map.Projection.ToZoomFactor(this.ZoomLevel), this.Center, false, true);

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
                    (float)this.Map.Projection.ToZoomFactor(this.ZoomLevel), this.Center, false, true);

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
                    (float)this.Map.Projection.ToZoomFactor(this.ZoomLevel), this.Center, false, true);

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
                    (float)this.Map.Projection.ToZoomFactor(this.ZoomLevel), this.Center, false, true);

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
                    (float)this.Map.Projection.ToZoomFactor(this.ZoomLevel), this.Center, false, true);

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
                    (float)this.Map.Projection.ToZoomFactor(this.ZoomLevel), this.Center, false, true);

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
    }
}