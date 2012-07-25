using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.VRP.Core;
using Tools.Math.Units.Time;
using Osm.Core;

namespace Osm.Routing.Core.VRP.NoDepot.MinMaxTime.VNS
{
    /// <summary>
    /// Uses a Variable Neighbourhood Search technique.
    /// </summary>
    /// <typeparam name="ResolvedType"></typeparam>
    public class VNSSimple<ResolvedType> : RouterMinMaxTime<ResolvedType>
        where ResolvedType : ILocationObject
    {
        public VNSSimple()
            : base(null, null, null)
        {

        }

        /// <summary>
        /// Does the actual calculation.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="customers"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        protected override int[][] DoCalculation(IProblemWeights problem, ICollection<int> customers, 
            Second min, Second max)
        {
            List<int> customers_to_place = new List<int>(customers);

            while (customers_to_place.Count > 0)
            {

            }

            return null;
        }
    }
}
