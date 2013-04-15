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
    /// A selector selecting individial using a tournament base selection.
    /// </summary>
    /// <typeparam name="GenomeType"></typeparam>
    /// <typeparam name="ProblemType"></typeparam>
    /// <typeparam name="WeightType"></typeparam>
    public class TournamentBasedSelector<GenomeType, ProblemType, WeightType> : 
        ISelector<GenomeType, ProblemType, WeightType>
        where ProblemType : IProblem
        where GenomeType : class
        where WeightType : IComparable
    {
        private double _tournament_size;

        private double _tournament_probability;

        /// <summary>
        /// Creates a new tournament base selector.
        /// </summary>
        /// <param name="tournament_size"></param>
        /// <param name="tournament_probility"></param>
        public TournamentBasedSelector(
            double tournament_size,
            double tournament_probility)
        {
            _tournament_size = tournament_size;
            _tournament_probability = tournament_probility;
        }

        #region ISelector<GenomeType> Members

        /// <summary>
        /// Selects an individual from the given population.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="population"></param>
        /// <param name="do_not_select_list"></param>
        /// <returns></returns>
        public Individual<GenomeType, ProblemType, WeightType> Select(
            Solver<GenomeType, ProblemType, WeightType> solver,
            Population<GenomeType, ProblemType, WeightType> population,
            ICollection<Individual<GenomeType, ProblemType, WeightType>> do_not_select_list)
        {
            Individual<GenomeType, ProblemType, WeightType> selected = null;
            while (selected == null)
            {
                selected = this.DoSelect(solver, population, do_not_select_list);
            }
            return selected;
        }

        private Individual<GenomeType, ProblemType, WeightType> DoSelect(
            Solver<GenomeType, ProblemType, WeightType> solver,
            Population<GenomeType, ProblemType, WeightType> population,
            ICollection<Individual<GenomeType, ProblemType, WeightType>> do_not_select_list)
        {
            do_not_select_list = null;
            Population<GenomeType, ProblemType, WeightType> temp_pop =
                new Population<GenomeType, ProblemType, WeightType>(true);

            int idx = 0;
            int tournament_size_int = (int)System.Math.Ceiling(((_tournament_size / 100f) * (double)population.Count));
            while (idx < tournament_size_int)
                //|| idx < population.Count - temp_pop.Count)
            { // keep looping until enough individuals are selected or until no more are available.
                // select next individual.
                int next_idx = solver.Random.Next(population.Count);
                Individual<GenomeType, ProblemType, WeightType> selected_individual = population[next_idx];

                //// check it's existence in the new population.
                //if (!temp_pop.Contains(selected_individual))
                //{ // individual can be selected.
                    temp_pop.Add(selected_individual);

                    idx++;
                //}
            }

            // sort the population..
            temp_pop.Sort(solver, solver.FitnessCalculator);

            // choose a candite.
            for (idx = 0; idx < temp_pop.Count; idx++)
            { // choose a candidate.
                if (solver.Random.NextDouble() <
                    (_tournament_probability))
                { // candidate choosen!
                    if (do_not_select_list == null
                        || !do_not_select_list.Contains(temp_pop[idx]))
                    {
                        return temp_pop[idx];
                    }
                }
            }
            return null;
        }

        #endregion
    }
}
