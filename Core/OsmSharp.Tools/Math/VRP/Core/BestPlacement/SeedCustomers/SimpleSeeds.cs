// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
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

namespace OsmSharp.Tools.Math.VRP.Core.BestPlacement.SeedCustomers
{
    /// <summary>
    /// A seed customer selector that maximizes the distance between the selected customers.
    /// </summary>
    public class SimpleSeeds : ISeedCustomerSelector
    {
        /// <summary>
        /// Selects seed customers.
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public ICollection<int> SelectSeeds(IProblemWeights weights, int k)
        {
            List<int> seeds = new List<int>();

            // randomly select k seeds.
            while (seeds.Count < k)
            {
                int new_seed = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(weights.Size);
                while (seeds.Contains(new_seed))
                {
                    new_seed = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(weights.Size);
                }
                seeds.Add(new_seed);
            }

            // find shortest distance.
            double minimal = double.MaxValue;
            int selected_idx = -1;
            for (int x = 0; x < k; x++)
            {
                for (int y = 0; y < x; y++)
                {
                    double weight = weights.Weight(seeds[x], seeds[y]);
                    if (weight < minimal)
                    {
                        selected_idx = x;
                        minimal = weight;
                    }
                }
            }

            // select any new seed and see if replacing the selected would yield better results.
            int max_tries = 2000;
            int tries = 0;
            while (tries < max_tries)
            {
                int seed = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(weights.Size);
                while (seeds.Contains(seed))
                {
                    seed = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(weights.Size);
                }

                // check the new minimal.
                double new_minimal = double.MaxValue;
                for (int x = 0; x < k; x++)
                {
                    if (x != selected_idx)
                    {
                        double weight = weights.Weight(seeds[x], seed);
                        if (weight < new_minimal)
                        {
                            new_minimal = weight;
                        }
                    }
                }

                // see if the new one has a higher minimal.
                if (new_minimal > minimal)
                { // ok there is an improvement!
                    OsmSharp.Tools.Output.OutputStreamHost.WriteLine(string.Format("Seed new minimal: {0}->{1}",
                        minimal, new_minimal));

                    tries = 0;
                    minimal = new_minimal;


                    seeds[selected_idx] = seed;
                }

                tries++; // increase the number of tries.
            }

            OsmSharp.Tools.Output.OutputStreamHost.WriteLine(string.Format("Seed distance: {0}", 
                minimal));

            return seeds;
        }
    }
}
