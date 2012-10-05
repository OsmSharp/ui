using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.GeoCoding;

namespace Tools.GeoCoding
{
    /// <summary>
    /// Describes a generic geo coder.
    /// </summary>
    public interface IGeoCoder
    {
        /// <summary>
        /// Does the actual geocoding.
        /// </summary>
        /// <param name="country"></param>
        /// <param name="postal_code"></param>
        /// <param name="commune"></param>
        /// <param name="street"></param>
        /// <param name="house_number"></param>
        /// <returns></returns>
        IGeoCoderResult Code(string country,
            string postal_code,
            string commune,
            string street,
            string house_number);
    }
}
