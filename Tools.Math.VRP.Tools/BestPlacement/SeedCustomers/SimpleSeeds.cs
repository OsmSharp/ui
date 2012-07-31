using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.VRP.Core.BestPlacement.SeedCustomers
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
                int new_seed = Tools.Math.Random.StaticRandomGenerator.Get().Generate(weights.Size);
                while (seeds.Contains(new_seed))
                {
                    new_seed = Tools.Math.Random.StaticRandomGenerator.Get().Generate(weights.Size);
                }
                seeds.Add(new_seed);
            }

            // find shortest distance.
            float minimal = float.MaxValue;
            int selected_idx = -1;
            for (int x = 0; x < k; x++)
            {
                for (int y = 0; y < x; y++)
                {
                    float weight = weights.Weight(seeds[x], seeds[y]);
                    if (weight < minimal)
                    {
                        selected_idx = x;
                        minimal = weight;
                    }
                }
            }

            // select any new seed and see if replacing the selected would yield better results.
            int max_tries = 10;
            int tries = 0;
            while (tries < max_tries)
            {
                int seed = Tools.Math.Random.StaticRandomGenerator.Get().Generate(weights.Size);
                while (seeds.Contains(seed))
                {
                    seed = Tools.Math.Random.StaticRandomGenerator.Get().Generate(weights.Size);
                }

                // check the new minimal.
                float new_minimal = float.MaxValue;
                for (int x = 0; x < k; x++)
                {
                    if (x != selected_idx)
                    {
                        float weight = weights.Weight(seeds[x], seed);
                        if (weight < new_minimal)
                        {
                            new_minimal = weight;
                        }
                    }
                }

                // see if the new one has a higher minimal.
                if (new_minimal > minimal)
                { // ok there is an improvement!
                    tries = 0;
                    minimal = new_minimal;

                    seeds[selected_idx] = seed;
                }

                tries++; // increase the number of tries.
            }
            return seeds;
        }


        //private class WeightAndPair
        //{
        //    public WeightAndPair(int from, int to,
        //        float weight)
        //    {
        //        this.From = from;
        //        this.To = to;
        //        this.Weight = weight;
        //    }

        //    public int From { get; set; }

        //    public int To { get; set; }

        //    public float Weight { get; set; }
        //}

        //private class WeightAndPairComparer : IComparer<WeightAndPair>
        //{
        //    public int Compare(WeightAndPair x, WeightAndPair y)
        //    {
        //        return x.Weight.CompareTo(y.Weight);
        //    }
        //}
    }
}
