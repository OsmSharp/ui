using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Core.DelimitedFiles
{
    /// <summary>
    /// Provides an interface to enable a custom format for the delimited files.
    /// </summary>
    public interface IDelimitedFormat
    {
        /// <summary>
        /// Converts a value in a given field to another value.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        string ConvertValue(string field, object value);

        /// <summary>
        /// Returns true if the column has to be exported.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        bool DoExport(int index, string name);
    }
}
