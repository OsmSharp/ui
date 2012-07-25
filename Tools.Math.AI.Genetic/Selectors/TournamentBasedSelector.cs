using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Solvers;

namespace Tools.Math.AI.Genetic.Selectors
{
    public class TournamentBasedSelector<GenomeType, ProblemType, WeightType> : 
        ISelector<GenomeType, ProblemType, WeightType>
        where ProblemType : IProblem
        where GenomeType : IEquatable<GenomeType>
        where WeightType : IComparable
    {
        private double _tournament_size;

        private double _tournament_probability;

        public TournamentBasedSelector(
            double tournament_size,
            double tournament_probility)
        {
            _tournament_size = tournament_size;
            _tournament_probability = tournament_probility;
        }

        #region ISelector<GenomeType> Members

        public Individual<GenomeType, ProblemType, WeightType> Select(
            Solver<GenomeType, ProblemType, WeightType> solver,
            Population<GenomeType, ProblemType, WeightType> population,
            HashSet<Individual<GenomeType, ProblemType, WeightType>> do_not_select_list)
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
            HashSet<Individual<GenomeType, ProblemType, WeightType>> do_not_select_list)
        {
            do_not_select_list = null;
            Population<GenomeType, ProblemType, WeightType> temp_pop =
                new Population<GenomeType, ProblemType, WeightType>(true);

            int idx = 0;
            int tournament_size_int = (int)((_tournament_size / 100f) * (double)population.Count);
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
