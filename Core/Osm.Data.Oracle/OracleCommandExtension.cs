using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;

namespace Osm.Data.Oracle
{
    public static class OracleCommandExtension
    {
        public static OracleCommand AddParameterCollection<TValue>(this OracleCommand command, string name, IEnumerable<TValue> collection)
        {
            var oraParams = new List<OracleParameter>();
            var counter = 0;
            var collectionParams = new StringBuilder(":");
            foreach (var obj in collection)
            {
                var param = name + counter;
                collectionParams.Append(param);
                collectionParams.Append(", :");
                command.Parameters.Add(param, obj);
                counter++;
            }
            collectionParams.Remove(collectionParams.Length - 3, 3);
            command.CommandText = command.CommandText.Replace(":" + name, collectionParams.ToString());
            command.Parameters.AddRange(oraParams.ToArray());
            return command;
        }
    }
}
