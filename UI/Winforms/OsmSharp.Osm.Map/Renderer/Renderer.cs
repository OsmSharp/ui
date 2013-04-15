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
using OsmSharp.Osm.Renderer.Tweakers;
using System.Drawing.Drawing2D;

namespace OsmSharp.Osm.Renderer
{
    /// <summary>
    /// Represents a renderer for an osm map.
    /// </summary>
    public abstract class Renderer<TargetType>
        where TargetType : ITarget
    {
        public delegate void ChangeDelegate();

        public event ChangeDelegate Change;

        /// <summary>
        /// The map this renderer is for.
        /// </summary>
        private OsmSharp.Osm.Map.Map _map;

        /// <summary>
        /// The target this renderer is for.
        /// 
        /// WARNING: properties of this target can change during the lifetime of this renderer.
        /// </summary>
        private TargetType _target;

        /// <summary>
        /// Creates a new renderer for a given map.
        /// </summary>
        /// <param name="map"></param>
        protected Renderer(Osm.Map.Map map, TargetType target)
        {
            _map = map;
            _target = target;
        }

        /// <summary>
        /// Renders the map.
        /// </summary>
        public void Render(View view)
        {
            long start_ticks = DateTime.Now.Ticks;

            this.OnBeforeRender(view);

            for (int idx = 0;idx <  _map.Layers.Count; idx++)
            {
                ILayer layer = _map.Layers[idx];

                if (layer != null && layer.Visible)
                {
                    layer.Changed -= new OsmSharp.Osm.Map.Map.LayerChangedDelegate(layer_Changed);

                    this.Render(layer,view);

                    layer.Changed += new OsmSharp.Osm.Map.Map.LayerChangedDelegate(layer_Changed);
                }
            }

            if (this.Target.DisplayAttributions)
            {
                this.RenderAttributions(view);
            }

            if (this.Target.DisplayCardinalDirections)
            {
                this.RenderCardinalDirections(view);
            }


            long end_ticks = DateTime.Now.Ticks;
            TimeSpan span = new TimeSpan(end_ticks - start_ticks);
            double fps = System.Math.Round(1.0 / span.TotalSeconds, 2);

            if (this.Target.DisplayStatus)
            {
                this.RenderStatus(view,fps);
            }

            this.OnAfterRender(view);
        }

        void layer_Changed(ILayer layer)
        {
            if (Change != null)
            {
                Change();
            }
        }

        /// <summary>
        /// Renders the attributions of the data source(s).
        /// </summary>
        /// <param name="view"></param>
        protected abstract void RenderAttributions(View view);

        /// <summary>
        /// Renders the status of the renderer.
        /// </summary>
        /// <param name="view"></param>
        protected abstract void RenderStatus(View view, double fps);

        /// <summary>
        /// Renders the cardinal directions.
        /// </summary>
        /// <param name="view"></param>
        protected abstract void RenderCardinalDirections(View view);

        /// <summary>
        /// Renders the given layer on the target for the given view.
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="view"></param>
        private void Render(ILayer layer, View view)
        {
            if (layer.MinZoom > 0)
            {
                if (layer.MinZoom > view.ZoomFactor)
                { // zoom is outside of bounds.
                    return;
                }
            } 
            if (layer.MaxZoom > 0)
            {
                if (layer.MaxZoom < view.ZoomFactor)
                { // zoom is outside of bounds.
                    return;
                }
            }
            IList<IElement> elements = layer.GetElements(view.Box, view.ZoomFactor);

            foreach (IElement element in elements)
            {
                if (element.IsVisibleAt(view.ZoomFactor))
                {
                    this.DrawElement(view, element);
                }
            }
        }


        private void DrawElement(View view, IElement element)
        {
            if (element == null)
            {
                return;
            }
            if (element is ElementImage)
            {
                ElementImage el_image = (element as ElementImage);
                if (el_image.Box != null)
                {
                    this.DrawImage(view, el_image.Image, el_image.Box, null);
                }
                else
                {
                    this.DrawImage(view, el_image.Image, el_image.Center, null);
                }
            }
            else if (element is ElementDot)
            {
                ElementDot el_dot = (element as ElementDot);
                if (el_dot.FixedWidth)
                {
                    this.DrawCircleFixed(view, el_dot.Color, el_dot.Dot.Point, (float)el_dot.Radius, (float)el_dot.Radius, null);
                }
                else
                {
                    this.DrawCircle(view, el_dot.Color, el_dot.Dot.Point, (float)el_dot.Radius, (float)el_dot.Radius, null);
                }
            }
            else if (element is ElementLine)
            {
                ElementLine el_line = element as ElementLine;
                if (el_line.FixedWidth)
                {
                    this.DrawLineFixed(view, el_line.Color, el_line.Line.Points, (float)el_line.Width,el_line.Style, el_line.StartCap,el_line.EndCap, null);
                }
                else
                {
                    this.DrawLine(view, el_line.Color, el_line.Line.Points, (float)el_line.Width, el_line.Style, el_line.StartCap, el_line.EndCap, null);
                }
            }
            else if (element is ElementPolygon)
            {
                ElementPolygon el_polygon = element as ElementPolygon;
                if (el_polygon.Filled)
                {
                    this.FillPolygon(view, el_polygon.Color, el_polygon.Polygon.Points, null);
                }
                else
                {
                    this.DrawPolygon(view, el_polygon.Color, el_polygon.Polygon.Points, (float)el_polygon.Width,el_polygon.FixedWidth, null);
                }
            }
            else if (element is ElementText)
            {
                ElementText el_text = element as ElementText;
                this.DrawText(view, el_text.Color, el_text.Center, el_text.Font, el_text.Text, null);
            }
            else if (element is ElementEllipse)
            {
                ElementEllipse el_ellipse = element as ElementEllipse;
                if(el_ellipse.EdgeColor > 0)
                {
                    this.FillEllipseGradient(view, el_ellipse.Color, el_ellipse.EdgeColor, el_ellipse.FixedWidth, el_ellipse.Dot.Point, (float)el_ellipse.X, (float)el_ellipse.Y, null);
                }
                else
                {
                    this.FillEllipse(view, el_ellipse.Color, el_ellipse.FixedWidth, el_ellipse.Dot.Point, (float)el_ellipse.X, (float)el_ellipse.Y, null);
                }
            }
            else
            {
                throw new ArgumentException(
                    string.Format("Renderer does not support elements of type {0}!",
                    element.GetType().ToString()));
            }
        }


        protected abstract void OnBeforeRender(View view);
        protected abstract void OnBeforeRender(ILayer layer, View view);

        protected abstract void OnAfterRender(View view);
        protected abstract void OnAfterRender(ILayer layer, View view);

        public TargetType Target
        {
            get
            {
                return _target;
            }
        }


        #region Rendering Functions
        
        #region Draw

        /// <summary>
        /// Draws a line.
        /// </summary>
        /// <param name="argb"></param>
        /// <param name="coordinates"></param>
        /// <param name="width"></param>
        public void DrawLine(View view, int argb,
            GeoCoordinate[] coordinates,
            float width,
            LineStyle style,
            LineCap start_cap,
            LineCap stop_cap,
            Tweak tweak)
        {
            if (tweak == null)
            {
                tweak = DefaultTweak.Instance;
            }

            this.DoDrawLine(view,
                tweak.TweakColor(view.ZoomFactor, argb),
                coordinates,
                tweak.TweakWidth(view.ZoomFactor, width),
                style,
                start_cap,
                stop_cap);
        }

        /// <summary>
        /// Draws a line.
        /// </summary>
        /// <param name="argb"></param>
        /// <param name="coordinates"></param>
        /// <param name="width"></param>
        public void DrawLineFixed(View view, int argb,
            GeoCoordinate[] coordinates,
            float width,
            LineStyle style,
            LineCap start_cap,
            LineCap stop_cap,
            Tweak tweak)
        {
            if (tweak == null)
            {
                tweak = DefaultTweak.Instance;
            }
            this.DoDrawLineFixed(view,
                tweak.TweakColor(view.ZoomFactor, argb),
                coordinates,
                width,
                style,
                start_cap,
                stop_cap);
        }

        ///// <summary>
        ///// Draws a line.
        ///// </summary>
        ///// <param name="argb"></param>
        ///// <param name="coordinates"></param>
        ///// <param name="width"></param>
        //public void DrawLine(View view, int argb,
        //    GeoCoordinate[] coordinates,
        //    float width,
        //    LineStyle style,
        //    LineCap start_cap,
        //    LineCap stop_cap,
        //    Tweak tweak)
        //{
        //    if (tweak == null)
        //    {
        //        tweak = DefaultTweak.Instance;
        //    }
        //    this.DoDrawLine(view,
        //        tweak.TweakColor(view.ZoomFactor, argb),
        //        coordinates,
        //        tweak.TweakWidth(view.ZoomFactor, width)
        //        , style,
        //        start_cap,stop_cap);
        //}

        /// <summary>
        /// Draws a line with a given style.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="argb"></param>
        /// <param name="coordinates"></param>
        /// <param name="width"></param>
        /// <param name="style"></param>
        protected abstract void DoDrawLine(View view, int argb,
            GeoCoordinate[] coordinates, float width,
            LineStyle style,
            LineCap start_cap,
            LineCap end_cap);



        /// <summary>
        /// Draws a line with a given style.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="argb"></param>
        /// <param name="coordinates"></param>
        /// <param name="width"></param>
        /// <param name="style"></param>
        protected abstract void DoDrawLineFixed(View view, int argb,
            GeoCoordinate[] coordinates, float width,
            LineStyle style,
            LineCap start_cap,
            LineCap end_cap);

        /// <summary>
        /// Draws a polygon.
        /// </summary>
        /// <param name="argb"></param>
        /// <param name="coordinates"></param>
        /// <param name="width"></param>
        public void DrawPolygon(View view, int argb,
            GeoCoordinate[] coordinates,
            float width,
            bool width_fixed,
            Tweak tweak)
        {
            if (tweak == null)
            {
                tweak = DefaultTweak.Instance;
            }
            this.DoDrawPolygon(view,
                tweak.TweakColor(view.ZoomFactor, argb),
                coordinates,
                tweak.TweakWidth(view.ZoomFactor, width),
                width_fixed);
        }

        /// <summary>
        /// Draws a polygon.
        /// </summary>
        /// <param name="argb"></param>
        /// <param name="coordinates"></param>
        /// <param name="width"></param>
        protected abstract void DoDrawPolygon(View view, int argb,
            GeoCoordinate[] coordinates,
            float width,
            bool width_fixed);

        /// <summary>
        /// Draws a circle.
        /// </summary>
        /// <param name="argb"></param>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="width"></param>
        public void DrawCircle(View view, int argb,
            GeoCoordinate center,
            float radius,
            float width,
            Tweak tweak)
        {
            if (tweak == null)
            {
                tweak = DefaultTweak.Instance;
            }
            this.DoDrawCircle(
                view,
                tweak.TweakColor(view.ZoomFactor, argb),
                center,
                tweak.TweakWidth(view.ZoomFactor, radius),
                tweak.TweakWidth(view.ZoomFactor, width));
        }


        /// <summary>
        /// Draws a circle.
        /// </summary>
        /// <param name="argb"></param>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="width"></param>
        public void DrawCircleFixed(View view, int argb,
            GeoCoordinate center,
            float radius,
            float width,
            Tweak tweak)
        {
            if (tweak == null)
            {
                tweak = DefaultTweak.Instance;
            }
            this.DoDrawCircleFixed(
                view,
                tweak.TweakColor(view.ZoomFactor, argb),
                center,
                radius,
                width);
        }

        /// <summary>
        /// Draws a circle.
        /// </summary>
        /// <param name="argb"></param>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="width"></param>
        protected abstract void DoDrawCircle(View view, int argb,
            GeoCoordinate center,
            float radius,
            float width);


        /// <summary>
        /// Draws a circle without scaling.
        /// </summary>
        /// <param name="argb"></param>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="width"></param>
        protected abstract void DoDrawCircleFixed(View view, int argb,
            GeoCoordinate center,
            float radius,
            float width);



        /// <summary>
        /// Draws text.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="argb"></param>
        /// <param name="point"></param>
        /// <param name="font"></param>
        /// <param name="text"></param>
        public void DrawText(View view, int argb,
            GeoCoordinate point, System.Drawing.Font font, string text,
            Tweak tweak)
        {
            if (tweak == null)
            {
                tweak = DefaultTweak.Instance;
            }
            this.DoDrawText(view,
                tweak.TweakColor(view.ZoomFactor, argb),
                point,
                new System.Drawing.Font(font.FontFamily, tweak.TweakFontSize(view.ZoomFactor, font.Size)),
                text);
        }

        /// <summary>
        /// Draws text.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="argb"></param>
        /// <param name="point"></param>
        /// <param name="font"></param>
        /// <param name="text"></param>
        protected abstract void DoDrawText(View view, int argb,
            GeoCoordinate point, System.Drawing.Font font, string text);

        /// <summary>
        /// Draws text.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="argb"></param>
        /// <param name="point"></param>
        /// <param name="font"></param>
        /// <param name="text"></param>
        protected abstract void DoDrawTextFixed(View view, int argb,
            GeoCoordinate point, System.Drawing.Font font, string text);

        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="argb"></param>
        /// <param name="top_left"></param>
        /// <param name="bottom_right"></param>
        /// <param name="width"></param>
        public void DrawRectangle(View view, int argb,
            GeoCoordinate top_left, float size, float width,
            Tweak tweak)
        {
            if (tweak == null)
            {
                tweak = DefaultTweak.Instance;
            }
            this.DoDrawRectangle(
                view,
                tweak.TweakColor(view.ZoomFactor, argb),
                top_left,
                tweak.TweakWidth(view.ZoomFactor, size),
                tweak.TweakWidth(view.ZoomFactor, width));
        }

        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="argb"></param>
        /// <param name="top_left"></param>
        /// <param name="bottom_right"></param>
        /// <param name="width"></param>
        protected abstract void DoDrawRectangle(View view, int argb,
            GeoCoordinate top_left, float size, float width);

        /// <summary>
        /// Draws an image.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="image"></param>
        /// <param name="top_left"></param>
        /// <param name="size"></param>
        public void DrawImage(View view,
            System.Drawing.Image image, GeoCoordinate top_left, float size,
            Tweak tweak)
        {
            if (tweak == null)
            {
                tweak = DefaultTweak.Instance;
            }
            this.DoDrawImage(view, image, top_left, tweak.TweakWidth(view.ZoomFactor, size));
        }

        /// <summary>
        /// Draws an image.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="image"></param>
        /// <param name="top_left"></param>
        /// <param name="size"></param>
        protected abstract void DoDrawImage(View view,
            System.Drawing.Image image, GeoCoordinate top_left, float size);

        /// <summary>
        /// Draws an image.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="image"></param>
        /// <param name="top_left"></param>
        /// <param name="size"></param>
        public void DrawImage(View view,
            System.Drawing.Image image, GeoCoordinateBox box, Tweak tweak)
        {
            if (tweak == null)
            {
                tweak = DefaultTweak.Instance;
            }
            this.DoDrawImage(view, image, box);
        }

        /// <summary>
        /// Draws an image.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="image"></param>
        /// <param name="box"></param>
        protected abstract void DoDrawImage(View view, System.Drawing.Image image, GeoCoordinateBox box);


        /// <summary>
        /// Draws an image.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="image"></param>
        /// <param name="top_left"></param>
        /// <param name="size"></param>
        public void DrawImage(View view,
            System.Drawing.Image image, GeoCoordinate center,
            Tweak tweak)
        {
            if (tweak == null)
            {
                tweak = DefaultTweak.Instance;
            }
            this.DoDrawImage(view, image, center);
        }       
        
        /// <summary>
        /// Draws an image.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="image"></param>
        /// <param name="top_left"></param>
        /// <param name="size"></param>
        protected abstract void DoDrawImage(View view,
            System.Drawing.Image image, GeoCoordinate center);

        #endregion

        #region Fill

        /// <summary>
        /// Fills a polygon.
        /// </summary>
        /// <param name="argb"></param>
        /// <param name="coordinates"></param>
        /// <param name="width"></param>
        public void FillPolygon(View view, int argb,
            GeoCoordinate[] coordinates,
            Tweak tweak)
        {
            if (tweak == null)
            {
                tweak = DefaultTweak.Instance;
            }
            this.DoFillPolygon(view,
                tweak.TweakColor(view.ZoomFactor, argb),
                coordinates);
        }

        /// <summary>
        /// Fills a polygon.
        /// </summary>
        /// <param name="argb"></param>
        /// <param name="coordinates"></param>
        /// <param name="width"></param>
        protected abstract void DoFillPolygon(View view, int argb,
            GeoCoordinate[] coordinates);

        /// <summary>
        /// Fills an ellipse.
        /// </summary>
        /// <param name="argb"></param>
        /// <param name="coordinate"></param>
        /// <param name="radius"></param>
        public void FillCircle(View view, int argb,
            GeoCoordinate center,
            float radius,
            Tweak tweak)
        {
            if (tweak == null)
            {
                tweak = DefaultTweak.Instance;
            }
            this.DoFillCircle(
                view,
                tweak.TweakColor(view.ZoomFactor, argb),
                center,
                tweak.TweakWidth(view.ZoomFactor, radius));
        }


        /// <summary>
        /// Fills an ellipse.
        /// </summary>
        /// <param name="argb"></param>
        /// <param name="coordinate"></param>
        /// <param name="radius"></param>
        protected abstract void DoFillCircle(View view, int argb,
            GeoCoordinate center,
            float radius);


        /// <summary>
        /// Fills an ellipse.
        /// </summary>
        /// <param name="argb"></param>
        /// <param name="coordinate"></param>
        /// <param name="radius"></param>
        public void FillEllipse(View view, int argb,
            bool fix,
            GeoCoordinate center,
            float x,
            float y,
            Tweak tweak)
        {
            if (tweak == null)
            {
                tweak = DefaultTweak.Instance;
            }
            this.DoFillEllipse(
                view,
                tweak.TweakColor(view.ZoomFactor, argb),
                fix,
                center,
                tweak.TweakWidth(view.ZoomFactor, x),
                tweak.TweakWidth(view.ZoomFactor, x));
        }


        /// <summary>
        /// Fills an ellipse.
        /// </summary>
        /// <param name="argb"></param>
        /// <param name="coordinate"></param>
        /// <param name="radius"></param>
        protected abstract void DoFillEllipse(View view, int argb,
            bool fix,
            GeoCoordinate center,
            float x,
            float y);

        /// <summary>
        /// Fills an ellipse.
        /// </summary>
        /// <param name="argb"></param>
        /// <param name="coordinate"></param>
        /// <param name="radius"></param>
        public void FillEllipseGradient(View view, int argb,
            int edge_argb,
            bool fix,
            GeoCoordinate center,
            float x,
            float y,
            Tweak tweak)
        {
            if (tweak == null)
            {
                tweak = DefaultTweak.Instance;
            }
            this.DoFillEllipseGradient(
                view,
                tweak.TweakColor(view.ZoomFactor, argb),
                tweak.TweakColor(view.ZoomFactor, edge_argb),
                fix,
                center,
                tweak.TweakWidth(view.ZoomFactor, x),
                tweak.TweakWidth(view.ZoomFactor, x));
        }


        /// <summary>
        /// Fills an ellipse.
        /// </summary>
        /// <param name="argb"></param>
        /// <param name="coordinate"></param>
        /// <param name="radius"></param>
        protected abstract void DoFillEllipseGradient(View view, int argb,
            int edge_argb,
            bool fix,
            GeoCoordinate center,
            float x,
            float y);

        /// <summary>
        /// Fill a rectangle.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="argb"></param>
        /// <param name="top_left"></param>
        /// <param name="bottom_right"></param>
        public void FillRectangle(View view, int argb,
            GeoCoordinate top_left, float size,
            Tweak tweak)
        {
            if (tweak == null)
            {
                tweak = DefaultTweak.Instance;
            }
            this.DoFillRectangle(
                view,
                tweak.TweakColor(view.ZoomFactor, argb),
                top_left,
                tweak.TweakWidth(view.ZoomFactor, size));
        }

        /// <summary>
        /// Fill a rectangle.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="argb"></param>
        /// <param name="top_left"></param>
        /// <param name="bottom_right"></param>
        protected abstract void DoFillRectangle(View view, int argb,
            GeoCoordinate top_left, float size);


        #endregion

        #endregion
    }
}
