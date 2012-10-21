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
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace Tools.GeoCoding
{
    /// <summary>
    /// Facade to this assembly.
    /// </summary>
    public static class Facade
    {
        /// <summary>
        /// Holds a cache of all found geocoders.
        /// </summary>
        private static Dictionary<string, IGeoCoder> _coders;

        /// <summary>
        /// Geocodes the given address and returns the result.
        /// </summary>
        /// <param name="coder"></param>
        /// <param name="country"></param>
        /// <param name="postal_code"></param>
        /// <param name="commune"></param>
        /// <param name="street"></param>
        /// <param name="house_number"></param>
        /// <returns></returns>
        public static IGeoCoderResult Code(
            string coder_assembly,
            string country,
            string postal_code,
            string commune,
            string street,
            string house_number)
        {
            IGeoCoder coder_instance = null;

            // create and cache the coder class.
            if (_coders == null)
            {
                _coders = new Dictionary<string, IGeoCoder>();
            }
            if (!_coders.TryGetValue(coder_assembly, out coder_instance))
            {
                Assembly assembly = Assembly.LoadFile(
                    string.Format(@"{0}\{1}.dll",
                    new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName,
                    coder_assembly));
                coder_instance = assembly.CreateInstance(coder_assembly + ".GeoCoder") as IGeoCoder;


                _coders.Add(coder_assembly, coder_instance);
            }

            return coder_instance.Code(
                country,
                postal_code,
                commune,
                street,
                house_number);
        }
    }
}
