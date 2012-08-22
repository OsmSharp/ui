using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.GeoCoding.Nomatim
{
    public class GeoCoderQuery : IGeoCoderQuery
    {
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
                return builder.ToString();
            }
        }

        #endregion
    }
}
