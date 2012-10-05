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
