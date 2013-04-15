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
using System.Drawing;
using OsmSharp.Tools.Math.Shapes;
using OsmSharp.Tools.Math.Geo;

namespace OsmSharp.Osm.Map.Elements
{
    public class ElementImage : ElementBase
    {
        private Image _image;

        private GeoCoordinateBox _box;

        private GeoCoordinate _center;

        /// <summary>
        /// Creates an image that scales.
        /// </summary>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="image"></param>
        public ElementImage(
            double top,
            double bottom,
            double left,
            double right,
            Image image,
            double? min_zoom,
            double? max_zoom)
            : base(min_zoom, max_zoom)
        {
            _image = image;
            GeoCoordinate top_left = new GeoCoordinate(top, left);
            GeoCoordinate bottom_right = new GeoCoordinate(bottom, right);
            _box = new GeoCoordinateBox(top_left, bottom_right);
        }

        /// <summary>
        /// Creates an image that scales.
        /// </summary>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="image"></param>
        public ElementImage(
            double top,
            double bottom,
            double left,
            double right,
            Image image)
        {
            _image = image;
            GeoCoordinate top_left = new GeoCoordinate(top, left);
            GeoCoordinate bottom_right = new GeoCoordinate(bottom, right);
            _box = new GeoCoordinateBox(top_left, bottom_right);
        }


        /// <summary>
        /// Creates an image that scales.
        /// </summary>
        /// <param name="top_left"></param>
        /// <param name="bottom_right"></param>
        /// <param name="image"></param>
        public ElementImage(
            GeoCoordinate top_left,
            GeoCoordinate bottom_right,
            Image image)
        {
            _image = image;
            _box = new GeoCoordinateBox(top_left, bottom_right);
        }

        /// <summary>
        /// Creates an image that scales.
        /// </summary>
        /// <param name="top_left"></param>
        /// <param name="bottom_right"></param>
        /// <param name="image"></param>
        public ElementImage(
            GeoCoordinate top_left,
            GeoCoordinate bottom_right,
            Image image,
            double? min_zoom,
            double? max_zoom)
            : base(min_zoom, max_zoom)
        {
            _image = image;
            _box = new GeoCoordinateBox(top_left, bottom_right);
        }

        /// <summary>
        /// Creates a new image at a given position that does not scale.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="image"></param>
        public ElementImage(
            GeoCoordinate center,
            Image image)
        {
            _image = image;
            _center = center;
        }

        public ElementImage SubTile(bool xTop, bool yTop)
        {
            Image im = new Bitmap(_image.Width >> 1, Image.Height >> 1);
            Graphics g = Graphics.FromImage(im);


            Rectangle rec = new Rectangle(
                xTop ? _image.Width >> 1 : 0,
                yTop ? _image.Height >> 1 : 0,
                _image.Width >> 1,
                _image.Height >> 1);

            g.DrawImage(_image, 0, 0, rec, GraphicsUnit.Pixel);

            double bott = this.Box.MinLat;
            double left = this.Box.MinLon;
            double h = (this.Box.MaxLat - this.Box.MinLat) / 2;
            double w = (this.Box.MaxLon - this.Box.MinLon) / 2;

            if (xTop) left += w;
            if (!yTop) bott += h;

            ElementImage tile = new ElementImage(bott + h, bott, left, left + w, im);

            return tile;
        }


        public static ElementImage Merge4Tiles(ElementImage[] tiles)
        {
            if (tiles.Length == 0) return null;

            Image im = new Bitmap(tiles[0].Image.Width << 1, tiles[0].Image.Height << 1);
            Graphics g = Graphics.FromImage(im);

            double bott = tiles.Min(t => t.Box.MinLat);
            double left = tiles.Min(t => t.Box.MinLon);
            double h = tiles.Max(t => t.Box.MaxLat) - bott;
            double w = tiles.Max(t => t.Box.MaxLon) - left;

            for (int i = 0; i < tiles.Length; i++)
            {
                Point pt = new Point();

                int x = Convert.ToInt32((tiles[i].Box.MinLon - left) / w * im.Width);
                int y = Convert.ToInt32((tiles[i].Box.MinLat - bott) / h * im.Width);

                pt.X = x;
                pt.Y = im.Height >> 1 - y;

                g.DrawImageUnscaled(tiles[i].Image, pt);
            }

            ElementImage tile = new ElementImage(bott + h, bott, left, left + w, im);
            return tile;

        }

        public Image Image
        {
            get
            {
                return _image;
            }
        }

        public GeoCoordinate Center
        {
            get
            {
                return _center;
            }
        }

        public GeoCoordinateBox Box
        {
            get
            {
                return _box;
            }
        }

        /// <summary>
        /// Returns true if the element is visible inside the given box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public override bool IsVisibleIn(GeoCoordinateBox box)
        {
            if (_box != null)
            {
                return box.Overlaps(_box);
            }
            else
            {
                return box.IsInside(_center);
            }
        }

        public override double ShortestDistanceTo(GeoCoordinate coordinate)
        {
            if (_box != null)
            {
                return coordinate.Distance(this.Box.Center);
            }
            return coordinate.Distance(this.Center);
        }
    }
}
