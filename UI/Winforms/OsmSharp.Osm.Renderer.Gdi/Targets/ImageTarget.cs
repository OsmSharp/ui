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
using System.IO;
using System.Drawing.Imaging;

namespace OsmSharp.Osm.Renderer.Gdi.Targets
{
    public class ImageTarget : 
        IGdiTarget
    {
        private Graphics _graphics;

        private Pen _pen;

        private Image _image;

        public ImageTarget(Image image)
        {
            _image = image;

            this.DisplayAttributions = true;
        }

        public ImageTarget(int width,int height)
        {
            _image = new Bitmap(width, height);

            this.DisplayAttributions = true;
        }

        #region IGdiTarget Members

        public Graphics Graphics
        {
            get 
            {
                if (_graphics == null)
                {
                    _graphics = Graphics.FromImage(_image);
                }
                return _graphics;
            }
        }

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
                return _image.Width; 
            }
        }

        public int YRes
        {
            get 
            {
                return _image.Height;
            }
        }

        #endregion

        public void Save(FileInfo file)
        {
            _image.Save(file.FullName,ImageFormat.Png);
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
