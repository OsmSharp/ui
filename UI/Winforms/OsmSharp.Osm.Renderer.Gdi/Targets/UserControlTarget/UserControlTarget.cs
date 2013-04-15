// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
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
using System.Linq;
using System.Text;
using OsmSharp.Tools.Math.Geo;
using System.Windows.Forms;
using System.Drawing;
using OsmSharp.Osm.Renderer.Gdi.Layers;
using OsmSharp.Osm.Map.Layers;

namespace OsmSharp.Osm.Renderer.Gdi.Targets.UserControlTarget
{
    /// <summary>
    /// Control displaying a map and implementing the IGdiTarget interface.
    /// </summary>
    public class UserControlTarget : UserControl, IGdiTarget
    {
        /// <summary>
        /// The current graphics object.
        /// </summary>
        private Graphics _current_graphics;

        /// <summary>
        /// The current view.
        /// </summary>
        private View _current_view;

        /// <summary>
        /// Creates a new user control target.
        /// </summary>
        public UserControlTarget()
        {
            this.DoubleBuffered = true;

            this.InitializeComponent();

            _positioned_custom_layers = new Dictionary<ILayer, List<GdiCustomLayer>>();
            _custom_layers = new List<GdiCustomLayer>();
        }
        

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // UserControlTarget
            // 
            this.Name = "UserControlTarget";
            this.Size = new System.Drawing.Size(649, 323);
            this.DisplayAttributions = true;
            this.DisplayStatus = true;
            this.DisplayCardinalDirections = true;
            this.ResumeLayout(false);
        }

        #region Properties

        /// <summary>
        /// The map this target is displaying.
        /// </summary>
        public OsmSharp.Osm.Map.Map Map 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// The center of the map displayed.
        /// </summary>
        private GeoCoordinate _center;

        /// <summary>
        /// The center of the map displayed.
        /// </summary>
        public GeoCoordinate Center 
        {
            get
            {
                return _center;
            }
            set
            {
                _center = value;
            }
        }

        /// <summary>
        /// The current zoomfactor.
        /// </summary>
        private float _zoom_factor;

        /// <summary>
        /// The current zoomfactor.
        /// </summary>
        public float ZoomFactor
        {
            get
            {
                return _zoom_factor;
            }
            set
            {
                _zoom_factor = value;
            }
        }

        /// <summary>
        /// Returns the current view.
        /// </summary>
        public View View
        {
            get
            {
                return _current_view;
            }
        }

        #endregion

        #region Custom Gdi Layers

        private List<GdiCustomLayer> _custom_layers;

        private Dictionary<ILayer, List<GdiCustomLayer>> _positioned_custom_layers;

        public void AddCustomLayer(GdiCustomLayer layer)
        {
            _custom_layers.Add(layer);
        }

        public void AddCustomLayerAbove(ILayer layer, GdiCustomLayer custom_layer)
        {
            if (!_positioned_custom_layers.ContainsKey(layer))
            {
                _positioned_custom_layers.Add(layer, new List<GdiCustomLayer>());
            }
            _positioned_custom_layers[layer].Add(custom_layer);
        }

        #endregion

        #region Overrides
        
        /// <summary>
        /// OnPaint event; used to paint the map on this target when request so by the UI.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.Center != null && this.Map != null)
            {
                this.BackgroundImage = null;

                // construct a view.
                _current_view = View.CreateFrom(this, this.ZoomFactor, this.Center);

                // render the map.
                _current_graphics = e.Graphics;
                _current_graphics.Clear(Color.White);
                _current_graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                _current_graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
                _current_graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                //_current_graphics.
                GdiRenderer renderer = new GdiRenderer(this.Map, this);
                foreach (GdiCustomLayer layer in _custom_layers)
                {
                    renderer.AddCustomLayer(layer);
                }
                renderer.Render(_current_view);
                renderer.Change += new Renderer<IGdiTarget>.ChangeDelegate(renderer_Change);
            }

            if (_selection_rectangle != null)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(125,Color.Blue)), _selection_rectangle.Value);
                e.Graphics.DrawRectangle(new Pen(Color.DarkBlue), _selection_rectangle.Value);
            }
        }

        void renderer_Change()
        {
            this.Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            this.Invalidate();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            this.RaiseOnMapMouseWheel(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            this.RaiseOnMapMouseDown(e);
        }


        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            
            this.RaiseOnMapMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            this.RaiseOnMapMouseUp(e);
        }

        #endregion

        #region Events

        /// <summary>
        /// Delegate used to define events from this map control.
        /// </summary>
        /// <param name="e"></param>
        public delegate void MapMouseEventDelegate(UserControlTargetEventArgs e);

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
            if (_current_view != null)
            {
                // create map user control event args.
                UserControlTargetEventArgs args
                    = new UserControlTargetEventArgs(e,
                        _current_view.ConvertFromTargetCoordinates(this, e.X, e.Y));

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
        protected void OnMapMouseUp(UserControlTargetEventArgs e)
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
            if (_current_view != null)
            {
                // create map user control event args.
                UserControlTargetEventArgs args
                    = new UserControlTargetEventArgs(e,
                        _current_view.ConvertFromTargetCoordinates(this, e.X, e.Y));

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
        protected void OnMapMouseDown(UserControlTargetEventArgs e)
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
            if (_current_view != null)
            {
                // create map user control event args.
                UserControlTargetEventArgs args
                    = new UserControlTargetEventArgs(e,
                        _current_view.ConvertFromTargetCoordinates(this, e.X, e.Y));

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
        protected void OnMapMouseMove(UserControlTargetEventArgs e)
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
            if (_current_view != null)
            {
                // create map user control event args.
                UserControlTargetEventArgs args
                    = new UserControlTargetEventArgs(e,
                        _current_view.ConvertFromTargetCoordinates(this, e.X, e.Y));

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
        protected void OnMapMouseWheel(UserControlTargetEventArgs e)
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
            if (_current_view != null)
            {
                // create map user control event args.
                UserControlTargetEventArgs args
                    = new UserControlTargetEventArgs(e,
                        _current_view.ConvertFromTargetCoordinates(this, e.X, e.Y));

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
        protected void OnMapMouseDoubleClick(UserControlTargetEventArgs e)
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
            if (_current_view != null)
            {
                // create map user control event args.
                UserControlTargetEventArgs args
                    = new UserControlTargetEventArgs(e,
                        _current_view.ConvertFromTargetCoordinates(this, e.X, e.Y));

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
        protected void OnMapMouseClick(UserControlTargetEventArgs e)
        {

        }

        #endregion

        #endregion

        #region IGdiTarget Members

        public Graphics Graphics
        {
            get 
            { 
                return _current_graphics; 
            }
        }

        private Pen _pen;
        public Pen Pen
        {
            get 
            {
                if (_pen == null)
                {
                    _pen = new Pen(Brushes.Black);
                    _pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
                }
                return _pen;
            }
        }

        #endregion

        #region ITarget Members

        public int XRes
        {
            get 
            { 
                return this.Width; 
            }
        }

        public int YRes
        {
            get 
            { 
                return this.Height; 
            }
        }

        #endregion

        private Rectangle? _selection_rectangle;

        public void DrawSelectionBox(Point p1, Point p2)
        {
            int x = Math.Min(p1.X, p2.X);
            int y = Math.Min(p1.Y, p2.Y);

            int width = Math.Abs(p1.X - p2.X);
            int height = Math.Abs(p1.Y - p2.Y);
            _selection_rectangle = new Rectangle(new Point(x,y), new Size(width,height));
        }

        public void ResetSelectiongBox()
        {
            _selection_rectangle = null;
        }

        #region ITarget Members


        public bool DisplayStatus
        {
            get;
            set;
        }

        public bool DisplayAttributions
        {
            get;
            set;
        }

        #endregion

        #region ITarget Members


        public bool DisplayCardinalDirections
        {
            get;
            set;
        }

        #endregion
    }
}
