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
using OsmSharp.Osm.Map.Elements;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using OsmSharp.Tools.Math.Geo;
using System.Diagnostics;
using System.IO;

namespace OsmSharp.Osm.Map.Layers.Tiles
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
            int server = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(4);
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
