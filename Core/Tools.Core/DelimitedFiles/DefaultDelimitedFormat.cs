using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Core.DelimitedFiles
{
    /// <summary>
    /// Describes a default delimiter format that leaves all data intact and 
    /// exports all columns.
    /// </summary>
    internal class DefaultDelimitedFormat :  IDelimitedFormat
    {
        #region IDelimitedFormat Members

        public string ConvertValue(string field, object value)
        {
            return value.ToString();
        }

        public bool DoExport(int index, string name)
        {
            return true;
        }

        #endregion
    }
}
