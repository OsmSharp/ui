// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// Foobar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Foobar is distributed in the hope that it will be useful,
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
using Tools.Math.AI.Genetic.Solvers;

namespace Tools.Math.AI.Genetic.Selectors
{
    /// <summary>
    /// Interface abstracting the implementation of selection of individuals.
    /// </summary>
    /// <typeparam name="GenomeType"></typeparam>
    public interface ISelector<GenomeType, ProblemType, WeightType>
        where ProblemType : IProblem
        where GenomeType : class
        where WeightType : IComparable
    {
        /// <summary>
        /// Selects an individual from the population to use for cross-over.
        /// </summary>
        /// <param name="population"></param>
        /// <returns></returns>
        Individual<GenomeType, ProblemType, WeightType> Select(
            Solver<GenomeType, ProblemType, WeightType> solver,
            Population<GenomeType, ProblemType, WeightType> population,
            HashSet<Individual<GenomeType, ProblemType, WeightType>> do_not_select_list);
    }
}