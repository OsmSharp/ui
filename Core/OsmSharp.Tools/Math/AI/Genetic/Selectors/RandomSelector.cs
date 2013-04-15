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
using OsmSharp.Tools.Math.AI.Genetic.Solvers;

namespace OsmSharp.Tools.Math.AI.Genetic.Selectors
{
    /// <summary>
    /// A selector to select random individuals from a population.
    /// </summary>
    /// <typeparam name="GenomeType"></typeparam>
    /// <typeparam name="ProblemType"></typeparam>
    /// <typeparam name="WeightType"></typeparam>
    public class RandomSelector<GenomeType, ProblemType, WeightType> :
        ISelector<GenomeType, ProblemType, WeightType>
        where ProblemType : IProblem
        where GenomeType : class
        where WeightType : IComparable
    {
        /// <summary>
        /// Selects an individual from the given population.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="population"></param>
        /// <param name="do_not_select_list"></param>
        /// <returns></returns>
        public Individual<GenomeType, ProblemType, WeightType> Select(Solver<GenomeType, ProblemType, WeightType> solver, 
            Population<GenomeType, ProblemType, WeightType> population, 
            ICollection<Individual<GenomeType, ProblemType, WeightType>> do_not_select_list)
        {
            int idx1 = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(population.Count);
            int idx2 = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(population.Count);
            if (population[idx1].Fitness.CompareTo(
                    population[idx2].Fitness) > 0)
            {
                return population[idx2];
            }
            return population[idx1];
        }
    }
}
