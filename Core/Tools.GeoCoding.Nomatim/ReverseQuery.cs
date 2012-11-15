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
using System.Configuration;
using System.Linq;
using System.Text;

namespace OsmSharp.Tools.GeoCoding.Nomatim
{
    //public class ReverseQuery : IGeoCoderQuery
    //{
    //    private readonly string _url;
    //        private readonly double _lat;
    //        private readonly double _lon;
    //        // http://nominatim.openstreetmap.org/reverse?format=xml&lat=52.5487429714954&lon=-1.81602098644987&zoom=18&addressdetails=1
    //        private static string _REVERSE_URL = ConfigurationManager.AppSettings["NomatimReverse"] + "&format=xml&zoom=18&addressdetails=1";

    //    public ReverseQuery(string url, double lat,double lon)
    //    {
    //        _url = string.IsNullOrEmpty(url)?_REVERSE_URL: url;
    //        _lat = lat;
    //        _lon = lon;
    //    }

    //    #region IGeoCoderQuery Members

    //    public string Query
    //    {
    //        get
    //        {
    //                            return string.Format(System.Globalization.CultureInfo.InvariantCulture, _url, _lat, _lon);
    //        }
    //    }

    //    #endregion
    //}
}
