using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Data.Oracle
{
    public static class Extensions
    {
        public static object ConvertToDBValue<T>(this Nullable<T> nullable) where T : struct
        {
            if (nullable.HasValue)
            {
                return nullable.Value;
            }
            else
            {
                return DBNull.Value;
            }
        }

        public static string ToStringEmptyWhenNull(this object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            return obj.ToString();
        }
    }
}
