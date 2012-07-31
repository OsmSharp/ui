using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.VRP.Core.BestPlacement.SeedCustomers
{
    /// <summary>
    /// Represents an abstraction of a seed customer.
    /// </summary>
    public interface ISeedCustomerSelector
    {
        /// <summary>
        /// Selects k seed customers.
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        ICollection<int> SelectSeeds(IProblemWeights weights, int k);
    }
}
