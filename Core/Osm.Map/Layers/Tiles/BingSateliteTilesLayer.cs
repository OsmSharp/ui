using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Map.Elements;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using Tools.Math.Geo;
using System.Diagnostics;
using System.IO;

namespace Osm.Map.Layers.Tiles
{
    public class BingSateliteTilesLayer : TilesLayer
    {
        private string _version = "838";

        public BingSateliteTilesLayer()
            :base("http://ecn.t{0}.tiles.virtualearth.net/tiles/a{1}.jpeg?g={2}&mkt={3}{4}")
        {

        }

        protected override Elements.ElementImage LoadMissingTileFromServer(int zoom, int x, int y)
        {
            string quad_key = this.TileXYToQuadKey(x,y,zoom);
            int server = Tools.Math.Random.StaticRandomGenerator.Get().Generate(4);
            string url = string.Format(this.TilesUrl, server, quad_key, _version, "en", string.Empty);
            try
            {
                // calculate the tiles bounding box and set its properties.
                GeoCoordinate top_left = TileToWorldPos(x, y, zoom);
                GeoCoordinate bottom_right = TileToWorldPos(x + 1, y + 1, zoom);


                // get file from tile server.
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(
                    url);
                request.Timeout = 60000;
                Debug.WriteLine(url);

                WebResponse myResp = request.GetResponse();

                Stream stream = myResp.GetResponseStream();
                Image img = Bitmap.FromStream(stream);

                ElementImage tile = new ElementImage(top_left, bottom_right, img);

                stream.Close();
                stream.Dispose();

                return tile;
            }
            catch (WebException)
            {

            }
            return null;
        }

        internal string TileXYToQuadKey(int x, int y, int zoom)
        {
            StringBuilder quadKey = new StringBuilder();
            for (int i = zoom; i > 0; i--)
            {
                char digit = '0';
                int mask = 1 << (i - 1);
                if ((x & mask) != 0)
                {
                    digit++;
                }
                if ((y & mask) != 0)
                {
                    digit++;
                    digit++;
                }
                quadKey.Append(digit);
            }
            return quadKey.ToString();
        }
    }
}
