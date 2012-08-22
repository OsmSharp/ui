using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Solvers;

namespace Tools.Math.AI.Genetic.Selectors
{
    public class RandomSelector<GenomeType, ProblemType, WeightType> :
        ISelector<GenomeType, ProblemType, WeightType>
        where ProblemType : IProblem
        where GenomeType : class
        where WeightType : IComparable
    {

        public Individual<GenomeType, ProblemType, WeightType> Select(Solver<GenomeType, ProblemType, WeightType> solver, 
            Population<GenomeType, ProblemType, WeightType> population, 
            HashSet<Individual<GenomeType, ProblemType, WeightType>> do_not_select_list)
        {
            int idx1 = Tools.Math.Random.StaticRandomGenerator.Get().Generate(population.Count);
            int idx2 = Tools.Math.Random.StaticRandomGenerator.Get().Generate(population.Count);
            if (population[idx1].Fitness.CompareTo(
                    population[idx2].Fitness) > 0)
            {
                return population[idx2];
            }
            return population[idx1];
        }
    }
}
