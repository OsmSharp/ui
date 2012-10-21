// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// Foobar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Foobar is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Foobar. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Structures.StringTrees;
using Tools.Math.Geo;

namespace Tools.GeoCoding.Custom.Memory
{
    public class InMemoryGeoCoder : IGeoCoder
    {
        private IndexPostalCodes _index;

        public InMemoryGeoCoder()
        {
            _index = new IndexPostalCodes();
        }

        /// <summary>
        /// Adds a new entry.
        /// </summary>
        /// <param name="country"></param>
        /// <param name="postal_code"></param>
        /// <param name="commune"></param>
        /// <param name="street"></param>
        /// <param name="house_number"></param>
        /// <param name="value"></param>
        public void Add(string country, string postal_code, string commune, string street, string house_number, GeoCoordinate value)
        {
            IndexCommunes communes = _index.SearchExact(postal_code);
            if (communes == null)
            {
                communes = new IndexCommunes();
                _index.Add(postal_code, communes);
            }
            IndexStreets streets = communes.SearchExact(commune);
            if (streets == null)
            {
                streets = new IndexStreets();
                communes.Add(commune, streets);
            }
            IndexHouseNumbers numbers = streets.SearchExact(street);
            if (numbers == null)
            {
                numbers = new IndexHouseNumbers();
                streets.Add(street, numbers);
            }
            numbers.Add(house_number, value);
        }

        /// <summary>
        /// Does the actual geocoding.
        /// </summary>
        /// <param name="country"></param>
        /// <param name="postal_code"></param>
        /// <param name="commune"></param>
        /// <param name="street"></param>
        /// <param name="house_number"></param>
        /// <returns></returns>
        public IGeoCoderResult Code(string country, string postal_code, 
            string commune, string street, string house_number)
        {
            throw new NotImplementedException();
        }
    }
}
