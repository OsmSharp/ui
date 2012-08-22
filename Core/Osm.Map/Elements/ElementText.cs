using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Geo;
using Tools.Math.Shapes;
using System.Drawing;

namespace Osm.Map.Elements
{
    public class ElementText : ElementBase
    {
        /// <summary>
        /// The color of this dot.
        /// </summary>
        private int _color;

        /// <summary>
        /// The size of the text.
        /// </summary>
        private Font _font;

        /// <summary>
        /// The center of the text to place.
        /// </summary>
        private GeoCoordinate _center;

        /// <summary>
        /// The text to place.
        /// </summary>
        private string _text;

        /// <summary>
        /// Creates a new dot element.
        /// </summary>
        /// <param name="color"></param>
        public ElementText(int color, Font font, GeoCoordinate center, string text)
        {
            _color = color;
            _font = font;
            _center = center;
            _text = text;
        }

        /// <summary>
        /// Returns the color of this dot.
        /// </summary>
        public int Color
        {
            get
            {
                return _color;
            }
        }

        /// <summary>
        /// Returns the size.
        /// </summary>
        public Font Font
        {
            get
            {
                return _font;
            }
            set
            {
                _font = value;
            }
        }

        /// <summary>
        /// Returns the text.
        /// </summary>
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
            }
        }

        public GeoCoordinate Center
        {
            get
            {
                return _center;
            }
        }

        #region IElement Members

        public override bool IsVisibleIn(GeoCoordinateBox box)
        {
            return box.IsInside(_center);
        }

        public override double ShortestDistanceTo(GeoCoordinate coordinate)
        {
            return coordinate.Distance(this.Center);
        }

        #endregion
    }
}
