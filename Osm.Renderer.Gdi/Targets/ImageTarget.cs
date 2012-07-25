using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace Osm.Renderer.Gdi.Targets
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
