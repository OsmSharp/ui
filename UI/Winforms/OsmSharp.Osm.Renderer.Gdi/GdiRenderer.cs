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
using OsmSharp.Osm.Map.Layers;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Osm.Map.Elements;
using System.Drawing;
using System.Drawing.Drawing2D;
using OsmSharp.Osm.Renderer.Gdi.Layers;

namespace OsmSharp.Osm.Renderer.Gdi
{
    public class GdiRenderer : Renderer<IGdiTarget>
    {
        
        public GdiRenderer(Osm.Map.Map map, IGdiTarget target)
            : base(map, target)
        {
            _positioned_custom_layers = new Dictionary<ILayer, List<GdiCustomLayer>>();
            _custom_layers = new List<GdiCustomLayer>();
        }

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

        protected override void OnBeforeRender(View view)
        {

        }

        protected override void OnBeforeRender(ILayer layer, View view)
        {

        }

        protected override void OnAfterRender(View view)
        {
            foreach (GdiCustomLayer cust_layer in _custom_layers)
            {
                cust_layer.Render(this.Target, view);
            }
        }

        protected override void OnAfterRender(ILayer layer, View view)
        {
            if (_positioned_custom_layers.ContainsKey(layer))
            {
                foreach (GdiCustomLayer cust_layer in _positioned_custom_layers[layer])
                {
                    cust_layer.Render(this.Target, view);
                }
            }
        }

        protected override void DoDrawLine(View view,
            int argb,
            GeoCoordinate[] coordinates,
            float width,
            LineStyle style,
            LineCap start_cap,
            LineCap end_cap)
        {
            if (coordinates.Length > 1)
            {
                switch (style)
                {
                    case LineStyle.Custom:
                        throw new NotImplementedException();
                    case LineStyle.Dash:
                        this.Target.Pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        break;
                    case LineStyle.DashDot:
                        this.Target.Pen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;
                        break;
                    case LineStyle.DashDotDot:
                        this.Target.Pen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot;
                        break;
                    case LineStyle.Dot:
                        this.Target.Pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                        break;
                    case LineStyle.Solid:
                        this.Target.Pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                        break;
                }
                this.Target.Pen.StartCap = start_cap;
                this.Target.Pen.EndCap = end_cap;
                Color col = Color.FromArgb(argb);
                this.Target.Pen.Color = col;
                this.Target.Pen.Width = view.ConvertToTargetXSize(
                    this.Target,
                    width);
            try
            { 
                this.Target.Graphics.DrawLines(
                    this.Target.Pen,
                    view.ConvertToTargetCoordinates(this.Target, coordinates).ConvertToDrawing());
                this.Target.Pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            }
            catch (Exception)
            {
                // nasty GDI-bug!
                //http://blog.lavablast.com/post/2007/11/The-Mysterious-Parameter-Is-Not-Valid-Exception.aspx
            }
            }
        }

        protected override void DoDrawLineFixed(View view, int argb,
            GeoCoordinate[] coordinates, float width,
            LineStyle style,
            LineCap start_cap,
            LineCap end_cap)
        {
            if (coordinates.Length > 1)
            {
                switch (style)
                {
                    case LineStyle.Custom:
                        throw new NotImplementedException();
                    case LineStyle.Dash:
                        this.Target.Pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        break;
                    case LineStyle.DashDot:
                        this.Target.Pen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;
                        break;
                    case LineStyle.DashDotDot:
                        this.Target.Pen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot;
                        break;
                    case LineStyle.Dot:
                        this.Target.Pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                        break;
                    case LineStyle.Solid:
                        this.Target.Pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                        break;
                }
                Color col = Color.FromArgb(argb);
                this.Target.Pen.Color = col;
                this.Target.Pen.Width = width;
                this.Target.Pen.EndCap = end_cap;
                this.Target.Pen.StartCap = start_cap;

                try
                { 
                    this.Target.Graphics.DrawLines(
                        this.Target.Pen,
                        view.ConvertToTargetCoordinates(this.Target, coordinates).ConvertToDrawing());
                    this.Target.Pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                }
                catch (Exception)
                {
                    // nasty GDI-bug!
                    //http://blog.lavablast.com/post/2007/11/The-Mysterious-Parameter-Is-Not-Valid-Exception.aspx
                }
            }
        }

        protected override void DoDrawPolygon(View view, int argb, GeoCoordinate[] coordinates, float width, bool width_fixed)
        {
            this.Target.Pen.Color = Color.FromArgb(argb);
            if (width_fixed)
            {
                this.Target.Pen.Width = width;
            }
            else
            {
                this.Target.Pen.Width = view.ConvertToTargetXSize(this.Target, width);
            }
            if (coordinates.Length > 1)
            {
                try
                { 
                this.Target.Graphics.DrawPolygon(
                    this.Target.Pen,
                    view.ConvertToTargetCoordinates(this.Target, coordinates).ConvertToDrawing());
                }
                catch (Exception)
                {
                    // nasty GDI-bug!
                    //http://blog.lavablast.com/post/2007/11/The-Mysterious-Parameter-Is-Not-Valid-Exception.aspx
                }
            }
        }

        protected override void DoDrawCircle(View view, int argb, GeoCoordinate center, float radius, float width)
        {
            float target_radius = view.ConvertToTargetXSize(this.Target, radius);
            this.Target.Pen.Color = Color.FromArgb(argb);
            this.Target.Pen.Width = view.ConvertToTargetXSize(this.Target, width);
            PointF centerF = view.ConvertToTargetCoordinates(this.Target, center).ConvertToDrawing();

            try
            {
                this.Target.Graphics.DrawEllipse(
                    this.Target.Pen,
                    centerF.X - target_radius,
                    centerF.Y - target_radius,
                    target_radius * 2.0f,
                    target_radius * 2.0f);
            }
            catch (Exception)
            {
                // nasty GDI-bug!
                //http://blog.lavablast.com/post/2007/11/The-Mysterious-Parameter-Is-Not-Valid-Exception.aspx
            }
        }

        protected override void DoDrawCircleFixed(View view, int argb, GeoCoordinate center, float radius, float width)
        {
            float target_radius = radius;
            this.Target.Pen.Color = Color.FromArgb(argb);
            this.Target.Pen.Width = width;
            PointF centerF = view.ConvertToTargetCoordinates(this.Target, center).ConvertToDrawing();

            try
            {                
                this.Target.Graphics.DrawEllipse(
                    this.Target.Pen,
                    centerF.X - target_radius,
                    centerF.Y - target_radius,
                    target_radius * 2.0f,
                    target_radius * 2.0f);
            }
            catch (Exception)
            {
                // nasty GDI-bug!
                //http://blog.lavablast.com/post/2007/11/The-Mysterious-Parameter-Is-Not-Valid-Exception.aspx
            }
        }

        protected override void DoDrawText(View view, int argb, GeoCoordinate point, Font font, string text)
        {
            float size = view.ConvertToTargetXSize(this.Target, font.Size / 10000);
            PointF target_point = view.ConvertToTargetCoordinates(this.Target, point).ConvertToDrawing();
            Font target_font = new Font(font.FontFamily, size);
            Brush brush = new SolidBrush(Color.FromArgb(argb));

            try
            { 
                this.Target.Graphics.DrawString(text, target_font, brush, target_point);
            }
            catch (Exception)
            {
                // nasty GDI-bug!
                //http://blog.lavablast.com/post/2007/11/The-Mysterious-Parameter-Is-Not-Valid-Exception.aspx
            }
        }

        protected override void DoDrawTextFixed(View view, int argb, GeoCoordinate point, Font font, string text)
        {
            PointF target_point = view.ConvertToTargetCoordinates(this.Target, point).ConvertToDrawing();
            Font target_font = font;
            Brush brush = new SolidBrush(Color.FromArgb(argb));

            try
            {
                this.Target.Graphics.DrawString(text, target_font, brush, target_point);
            }
            catch (Exception)
            {
                // nasty GDI-bug!
                //http://blog.lavablast.com/post/2007/11/The-Mysterious-Parameter-Is-Not-Valid-Exception.aspx
            }
        }

        protected override void DoDrawRectangle(View view, int argb, GeoCoordinate top_left, float size, float width)
        {
            PointF target_top_left = view.ConvertToTargetCoordinates(this.Target, top_left).ConvertToDrawing();
            float target_size_x = view.ConvertToTargetXSize(this.Target, size);
            float target_size_y = view.ConvertToTargetYSize(this.Target, size);
            this.Target.Pen.Color = Color.FromArgb(argb);
            this.Target.Pen.Width = view.ConvertToTargetXSize(this.Target, width);

            try
            { 
                this.Target.Graphics.DrawRectangle(this.Target.Pen, target_top_left.X,
                    target_top_left.Y,
                    target_size_x,
                    target_size_y);
            }
            catch (Exception)
            {
                // nasty GDI-bug!
                //http://blog.lavablast.com/post/2007/11/The-Mysterious-Parameter-Is-Not-Valid-Exception.aspx
            }
        }

        protected override void DoDrawImage(View view, Image image, GeoCoordinate top_left, float size)
        {
            PointF target_top_left = view.ConvertToTargetCoordinates(this.Target, top_left).ConvertToDrawing();
            float target_size_x = view.ConvertToTargetXSize(this.Target, size);
            float target_size_y = view.ConvertToTargetYSize(this.Target, size);
            
            try
            {
                this.Target.Graphics.DrawImage(image, new RectangleF(target_top_left, new SizeF(target_size_x, target_size_y)));
            }
            catch (Exception)
            {
                // nasty GDI-bug!
                //http://blog.lavablast.com/post/2007/11/The-Mysterious-Parameter-Is-Not-Valid-Exception.aspx
            }
        }

        protected override void DoDrawImage(View view, Image image, GeoCoordinateBox box)
        {
            PointF target_top_left = view.ConvertToTargetCoordinates(this.Target, new GeoCoordinate(box.MaxLat, box.MinLon)).ConvertToDrawing();
            PointF target_bottom_right = view.ConvertToTargetCoordinates(this.Target, new GeoCoordinate(box.MinLat, box.MaxLon)).ConvertToDrawing();
            
            try
            {
                this.Target.Graphics.DrawImage(image, new RectangleF(target_top_left,
                    new SizeF(target_bottom_right.X - target_top_left.X + 1f, target_bottom_right.Y - target_top_left.Y + 1f)));
            }
            catch (Exception)
            {
                // nasty GDI-bug!
                //http://blog.lavablast.com/post/2007/11/The-Mysterious-Parameter-Is-Not-Valid-Exception.aspx
            }
        }

        protected override void DoDrawImage(View view, Image image, GeoCoordinate center)
        {
            PointF center_point = view.ConvertToTargetCoordinates(this.Target, center).ConvertToDrawing();

            PointF top_left = new PointF(
                center_point.X - (image.Width / 2),
                center_point.Y - (image.Height / 2));

            try
            {
                this.Target.Graphics.DrawImage(image, new RectangleF(top_left,
                    image.Size));
            }
            catch (Exception)
            {
                // nasty GDI-bug!
                //http://blog.lavablast.com/post/2007/11/The-Mysterious-Parameter-Is-Not-Valid-Exception.aspx
            }
        }

        protected override void DoFillPolygon(View view, int argb, GeoCoordinate[] coordinates)
        {
            try
            { 
                    this.Target.Graphics.FillPolygon(new SolidBrush(Color.FromArgb(argb))
                        , view.ConvertToTargetCoordinates(this.Target, coordinates).ConvertToDrawing());
                }
                catch (Exception)
                {
                    // nasty GDI-bug!
                    //http://blog.lavablast.com/post/2007/11/The-Mysterious-Parameter-Is-Not-Valid-Exception.aspx
                }
        }

        protected override void DoFillCircle(View view, int argb, GeoCoordinate center, float radius)
        {
            float target_raduis = view.ConvertToTargetXSize(this.Target, radius);
            PointF centerF = view.ConvertToTargetCoordinates(this.Target, center).ConvertToDrawing();

            try
            {
                this.Target.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(argb))
                    , centerF.X - target_raduis, centerF.Y - target_raduis, target_raduis * 2.0f, target_raduis * 2.0f);
            }
            catch (Exception)
            {
                // nasty GDI-bug!
                //http://blog.lavablast.com/post/2007/11/The-Mysterious-Parameter-Is-Not-Valid-Exception.aspx
            }
        }

        protected override void DoFillRectangle(View view, int argb, GeoCoordinate top_left, float size)
        {
            PointF target_top_left = view.ConvertToTargetCoordinates(this.Target, top_left).ConvertToDrawing();
            float target_size_x = view.ConvertToTargetXSize(this.Target, size);
            float target_size_y = view.ConvertToTargetYSize(this.Target, size);
            try
            {
                this.Target.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(argb)),
                    target_top_left.X,
                    target_top_left.Y,
                    target_size_x,
                    target_size_y);
            }
            catch (Exception)
            {
                // nasty GDI-bug!
                //http://blog.lavablast.com/post/2007/11/The-Mysterious-Parameter-Is-Not-Valid-Exception.aspx
            }
        }

        protected override void RenderAttributions(View view)
        {
            try
            {
                Font fnt_regular = new System.Drawing.Font("Times New Roman",10);
                //Font fnt_bold = new System.Drawing.Font("Times New Roman", 11);

                SizeF text_size = this.Target.Graphics.MeasureString(@"(c) OpenStreetMap contributors, CC-BY-SA", fnt_regular);

                PointF position = new PointF(
                    this.Target.XRes - 10 - text_size.Width,
                    this.Target.YRes - 10 - text_size.Height);

                //this.Target.Graphics.DrawString(@"(c) OpenStreetMap contributors, CC-BY-SA", fnt_bold, new SolidBrush(Color.White), position);
                this.Target.Graphics.DrawString(@"(c) OpenStreetMap contributors, CC-BY-SA", fnt_regular, new SolidBrush(Color.Black), position);
            }
            catch (Exception)
            {
                // nasty GDI-bug!
                //http://blog.lavablast.com/post/2007/11/The-Mysterious-Parameter-Is-Not-Valid-Exception.aspx
            }
        }

        protected override void RenderStatus(View view, double fps)
        {
            try
            {
                Font fnt_regular = new System.Drawing.Font("Times New Roman",10);

                this.Target.Graphics.DrawString(string.Format("Zoom@{0}", view.ZoomFactor), fnt_regular, new SolidBrush(Color.Black), new PointF(10, 10));
                this.Target.Graphics.DrawString(string.Format("Rendered@{0}fps", fps), fnt_regular, new SolidBrush(Color.Black), new PointF(10, 25));
            }
            catch (Exception)
            {
                // nasty GDI-bug!
                //http://blog.lavablast.com/post/2007/11/The-Mysterious-Parameter-Is-Not-Valid-Exception.aspx
            }
        }

        protected override void RenderCardinalDirections(View view)
        {
            try
            {
                Font fnt_regular = new System.Drawing.Font("Times New Roman", 10);

                Image image = global::OsmSharp.Osm.Renderer.Gdi.Properties.Resources.cardinal_image_small;

                this.Target.Graphics.DrawImage(image, new PointF(this.Target.XRes - image.Width - 10,10));
            }
            catch (Exception)
            {
                // nasty GDI-bug!
                //http://blog.lavablast.com/post/2007/11/The-Mysterious-Parameter-Is-Not-Valid-Exception.aspx
            }
        }

        protected override void DoFillEllipse(View view, int argb, bool fix, GeoCoordinate center, float x, float y)
        {
            float target_x = view.ConvertToTargetXSize(this.Target, x);
            float target_y = view.ConvertToTargetXSize(this.Target, y);
            PointF centerF = view.ConvertToTargetCoordinates(this.Target, center).ConvertToDrawing();

            try
            {
                this.Target.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(argb))
                    , centerF.X - x, centerF.Y - y, x * 2.0f, y * 2.0f);
            }
            catch (Exception)
            {
                // nasty GDI-bug!
                //http://blog.lavablast.com/post/2007/11/The-Mysterious-Parameter-Is-Not-Valid-Exception.aspx
            }
        }

        protected override void DoFillEllipseGradient(View view, int argb, int edge_argb, bool fix, GeoCoordinate center, float x, float y)
        {
            float target_x = view.ConvertToTargetXSize(this.Target, x);
            float target_y = view.ConvertToTargetXSize(this.Target, y);
            PointF centerF = view.ConvertToTargetCoordinates(this.Target, center).ConvertToDrawing();

            try
            {
                GraphicsPath path = new GraphicsPath();
                path.AddEllipse(centerF.X - x, centerF.Y - y, x * 2.0f, y * 2.0f);        

                PathGradientBrush brush = new PathGradientBrush(path);

                // Create new color blend to tell the PathGradientBrush what colors to use and where to put them
                ColorBlend GradientSpecifications = new ColorBlend(2);

                // Define positions of gradient colors, use intesity to adjust the middle color to
                // show more mask or less mask
                GradientSpecifications.Positions = new float[2] { 0, 1 };
                // Define gradient colors and their alpha values, adjust alpha of gradient colors to match intensity
                GradientSpecifications.Colors = new Color[2]
                {
                    Color.FromArgb(argb),
                    Color.FromArgb(edge_argb)
                };

                //brush.
                this.Target.Graphics.FillEllipse(brush
                    , centerF.X - x, centerF.Y - y, x * 2.0f, y * 2.0f);
            }
            catch (Exception)
            {
                // nasty GDI-bug!
                //http://blog.lavablast.com/post/2007/11/The-Mysterious-Parameter-Is-Not-Valid-Exception.aspx
            }
        }
    }
}