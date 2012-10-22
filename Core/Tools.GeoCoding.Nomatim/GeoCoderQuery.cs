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

namespace Tools.GeoCoding.Nomatim
{
    public class GeoCoderQuery
    {
		/// <summary>
		/// The url of the nomatim service.
		/// </summary>
		//private static string _GEOCODER_URL = "http://nominatim.openstreetmap.org/search?q={0}&format=xml&polygon=1&addressdetails=1";
		private static string _GEOCODER_URL = ConfigurationManager.AppSettings["NomatimAddress"] + "&format=xml&polygon=1&addressdetails=1";

        private string _country;
        private string _postal_code;
        private string _commune;
        private string _street;
        private string _house_number;

        public GeoCoderQuery(string country,
            string postal_code,
            string commune,
            string street,
            string house_number)
        {
            _country = country;
            _postal_code = postal_code;
            _commune = commune;
            _street = street;
            _house_number = house_number;
        }

        #region IGeoCoderQuery Members

        public string Query
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(_street);
                builder.Append(" ");
                builder.Append(_house_number);
                builder.Append(" ");
                builder.Append(_postal_code);
                builder.Append(" ");
                builder.Append(_commune);
                builder.Append(" ");
                builder.Append(_country);
                builder.Append(" ");
				return string.Format(System.Globalization.CultureInfo.InvariantCulture, _GEOCODER_URL, builder);
            }
        }

        #endregion
    }
}
