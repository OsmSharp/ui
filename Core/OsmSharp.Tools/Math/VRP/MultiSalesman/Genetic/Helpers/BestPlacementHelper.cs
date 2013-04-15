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
using OsmSharp.Tools.Math.AI.Genetic;

namespace OsmSharp.Tools.Math.VRP.MultiSalesman.Genetic.Helpers
{
    /// <summary>
    /// Helper class containing best placement optimisation algorithms.
    /// </summary>
    internal static class BestPlacementHelper
    {
        /// <summary>
        /// Places all the cities in the next individual starting with the city that makes the individual best or least worst.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="calculator"></param>
        /// <param name="individual"></param>
        /// <param name="cities_to_place"></param>
        /// <returns></returns>
        public static Individual Do(
            Problem problem,
            FitnessCalculator calculator,
            Individual individual,
            List<int> cities_to_place)
        {
            // initialize the best placement result.
            //BestPlacementResult result = null;

            // create the new individual.
            //Individual new_individual = null;
            throw new NotImplementedException();
            //cities_to_place = new List<int>(cities_to_place);
            //while (cities_to_place.Count > 0)
            //{
            //    // calculates the next best position for placement.
            //    result = BestPlacementHelper.CalculateBestPlacementInIndividual(
            //        problem,
            //        calculator,
            //        individual,
            //        cities_to_place);

            //    // calculate the new individual using the result.
            //    List<int> round = new_individual.Genomes[result.RoundIdx];
            //    if (round.Count == result.CityIdx)
            //    {
            //        round.Add(result.City);
            //    }
            //    else
            //    {
            //        round.Insert(result.CityIdx, result.City);
            //    }
            //    new_individual.CalculateFitness(problem, calculator);

            //    // remove the placed city.
            //    cities_to_place.Remove(result.City);
            //}

            //// return the result.
            //return new_individual;
        }

        /// <summary>
        /// DoFast.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="calculator"></param>
        /// <param name="individual"></param>
        /// <param name="cities_to_place"></param>
        /// <returns></returns>
        public static Individual DoFast(
            Problem problem,
            FitnessCalculator calculator,
            Individual individual,
            List<int> cities_to_place)
        {
            // initialize the best placement result.
            //BestPlacementResult result = null;

            // create the new individual.
            //Individual new_individual = null;// (individual.Copy() as Individual);
            throw new NotImplementedException();
            //cities_to_place = new List<int>(cities_to_place);
            //while (cities_to_place.Count > 0)
            //{
            //    int city_to_place = cities_to_place[cities_to_place.Count - 1];
            //    cities_to_place.RemoveAt(cities_to_place.Count - 1);

            //    // calculates the next best position for placement.
            //    result = BestPlacementHelper.CalculateBestPlacementInGenomes(
            //        problem,
            //        calculator,
            //        individual.Genomes,
            //        city_to_place);

            //    // calculate the new individual using the result.
            //    List<int> round = new_individual.Genomes[result.RoundIdx];
            //    if (round.Count == result.CityIdx)
            //    {
            //        round.Add(result.City);
            //    }
            //    else
            //    {
            //        round.Insert(result.CityIdx, result.City);
            //    }
            //    new_individual.CalculateFitness(problem, calculator, false);

            //    // remove the placed city.
            //    cities_to_place.Remove(result.City);
            //}

            //// return the result.
            //return new_individual;
        }


        public static Genome DoFast(
            Problem problem,
            FitnessCalculator calculator,
            Genome genome,
            List<int> cities_to_place)
        {
            List<Genome> genomes = new List<Genome>();
            genomes.Add(genome);
            return BestPlacementHelper.DoFast(
                problem,
                calculator,
                genomes,
                cities_to_place)[0];
        }

        public static List<Genome> DoFast(
            Problem problem,
            FitnessCalculator calculator,
            List<Genome> genomes,
            int city_to_place)
        {
            List<int> cities_to_place = new List<int>();
            cities_to_place.Add(city_to_place);
            return BestPlacementHelper.DoFast(
                problem,
                calculator,
                genomes,
                cities_to_place);
        }

        public static List<Genome> DoFast(
            Problem problem,
            FitnessCalculator calculator,
            List<Genome> genomes,
            List<int> cities_to_place)
        {
            // initialize the best placement result.
            BestPlacementResult result = null;

            // create the new individual.
            cities_to_place = new List<int>(cities_to_place);
            while (cities_to_place.Count > 0)
            {
                int city_to_place = cities_to_place[cities_to_place.Count - 1];
                cities_to_place.RemoveAt(cities_to_place.Count - 1);

                // calculates the next best position for placement.
                result = BestPlacementHelper.CalculateBestPlacementInGenomes(
                    problem,
                    calculator,
                    genomes,
                    city_to_place);

                // calculate the new individual using the result.
                List<int> round = genomes[result.RoundIdx];
                if (round.Count == result.CityIdx)
                {
                    round.Add(result.City);
                }
                else
                {
                    round.Insert(result.CityIdx, result.City);
                }

                // remove the placed city.
                cities_to_place.Remove(result.City);
            }

            // return the result.
            return genomes;
        }

        public static List<Genome> Do(
            Problem problem,
            FitnessCalculator calculator,
            List<Genome> genomes,
            List<int> cities_to_place)
        {
            // initialize the best placement result.
            BestPlacementResult result = null;

            // create the new individual.
            cities_to_place = new List<int>(cities_to_place);
            while (cities_to_place.Count > 0)
            {
                // calculates the next best position for placement.
                result = BestPlacementHelper.CalculateBestPlacementInGenomes(
                    problem,
                    calculator,
                    genomes,
                    cities_to_place);

                // calculate the new individual using the result.
                List<int> round = genomes[result.RoundIdx];
                if (round.Count == result.CityIdx)
                {
                    round.Add(result.City);
                }
                else
                {
                    round.Insert(result.CityIdx, result.City);
                }

                // remove the placed city.
                cities_to_place.Remove(result.City);
            }

            // return the result.
            return genomes;
        }

        public static List<Genome> Do(
            Problem problem,
            FitnessCalculator calculator,
            List<Genome> genomes,
            int city_to_place)
        {
            List<int> cities_to_place = new List<int>();
            cities_to_place.Add(city_to_place);
            return BestPlacementHelper.Do(
                problem,
                calculator,
                genomes,
                cities_to_place);
        }

        #region Place In Individual(s)

        /// <summary>
        /// Places cities into an individual.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="calculator"></param>
        /// <param name="individual"></param>
        /// <param name="cities_to_place"></param>
        /// <returns></returns>
        internal static BestPlacementResult CalculateBestPlacementInIndividual(
            Problem problem,
            FitnessCalculator calculator,
            Individual<List<Genome>, Problem, Fitness> individual,
            List<int> cities_to_place)
        {
            // initialize the best placement result.
            BestPlacementResult result = null;

            // place all the cities until there are no more left.
            // try to place every city in every round.
            foreach (int city_to_place in cities_to_place)
            {
                for (int round_idx = 0; round_idx < individual.Genomes.Count; round_idx++)
                {
                    BestPlacementResult new_result =
                        BestPlacementHelper.CalculateBestPlacementInIndividual(
                            problem,
                            calculator,
                            round_idx,
                            individual,
                            city_to_place);
                    if (result == null
                        || new_result.Fitness < result.Fitness)
                    {
                        result = new_result;
                    }
                }
            }

            // return the result.
            return result;
        }

        /// <summary>
        /// Places a city into an idividual.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="calculator"></param>
        /// <param name="round_idx"></param>
        /// <param name="individual"></param>
        /// <param name="city_to_place"></param>
        /// <returns></returns>
        internal static BestPlacementResult CalculateBestPlacementInIndividual(
            Problem problem,
            FitnessCalculator calculator,
            int round_idx,
            Individual<List<Genome>, Problem, Fitness> individual,
            int city_to_place)
        {
            // if the target round is empty best placement is impossible.
            Genome round = individual.Genomes[round_idx];

            // do best placement in the genome/round.
            BestPlacementResult result =
                BestPlacementHelper.CalculateBestPlacementInGenome(problem, calculator, round, city_to_place);

            // set the round index.
            result.RoundIdx = round_idx;
            if (!individual.FitnessCalculated)
            {
                individual.CalculateFitness(
                    problem, calculator);
            }
            result.Fitness = calculator.Adjust(
                problem,
                individual.Fitness,
                round_idx,
                result.Increase);

            // return the result.
            return result;
        }

        #endregion

        #region Place In Genome

        internal static BestPlacementResult CalculateBestPlacementInGenome(
            Problem problem,
            FitnessCalculator calculator,
            Genome genome,
            List<int> cities_to_place)
        {
            // initialize the best placement result.
            BestPlacementResult result
                = new BestPlacementResult();
            result.RoundIdx = -1;

            // try and place all cities.
            for (int city_idx = 0; city_idx < cities_to_place.Count; city_idx++)
            {
                // try to place first city.
                int city = cities_to_place[city_idx];

                // place the city and check the result.
                BestPlacementResult current_result =
                    BestPlacementHelper.CalculateBestPlacementInGenome(
                        problem,
                        calculator,
                        genome,
                        city);

                // check current result
                //if (result.Increase == null ||
                //    current_result.Increase < result.Increase)
                if (current_result.Increase < result.Increase)
                {
                    result = current_result;
                }
            }

            return result;
        }

        /// <summary>
        /// Places the given city in the individual.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="calculator"></param>
        /// <param name="genomes"></param>
        /// <param name="city_to_place"></param>
        /// <returns></returns>
        public static BestPlacementResult CalculateBestPlacementInGenomes(
            Problem problem,
            FitnessCalculator calculator,
            List<Genome> genomes,
            int city_to_place)
        {
            List<int> cities_to_place = new List<int>();
            cities_to_place.Add(city_to_place);
            return BestPlacementHelper.CalculateBestPlacementInGenomes(problem, calculator, genomes, cities_to_place);
        }

        /// <summary>
        /// Places cities into an individual.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="calculator"></param>
        /// <param name="genomes"></param>
        /// <param name="cities_to_place"></param>
        /// <returns></returns>
        public static BestPlacementResult CalculateBestPlacementInGenomes(
            Problem problem,
            FitnessCalculator calculator,
            List<Genome> genomes,
            List<int> cities_to_place)
        {
            // initialize the best placement result.
            BestPlacementResult result = null;

            // place all the cities until there are no more left.
            // try to place every city in every round.
            for (int round_idx = 0; round_idx < genomes.Count; round_idx++)
            {
                Genome genome = genomes[round_idx];
                BestPlacementResult new_result =
                    BestPlacementHelper.CalculateBestPlacementInGenome(
                        problem,
                        calculator,
                        genome,
                        cities_to_place);
                if (result == null
                    || new_result.Increase < result.Increase)
                {
                    result = new_result;
                    result.RoundIdx = round_idx;
                }
            }

            // return the result.
            return result;
        }


        /// <summary>
        /// Calculates the best place to insert a city.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="calculator"></param>
        /// <param name="genome"></param>
        /// <param name="city_to_place"></param>
        /// <returns></returns>
        public static BestPlacementResult CalculateBestPlacementInGenome(
            Problem problem,
            FitnessCalculator calculator,
            Genome genome,
            int city_to_place)
        {
            // initialize the best placement result.
            BestPlacementResult result
                = new BestPlacementResult();
            result.RoundIdx = -1;
            result.City = city_to_place;

            // initialize the best increase.
            double increase = 0;

            if (genome.Count > 0)
            {
                if (genome.Count == 1)
                { // when there is only on city in the round, try placing after and calculate again.
                    // calculate the increase.
                    increase = problem.Weight(genome[0], city_to_place)
                        + (problem.Weight(city_to_place, genome[0]));

                    // set the result city.
                    result.CityIdx = 1;
                }
                else
                { // when there are multiple cities try to place in all position and keep the best.
                    for (int idx = 0; idx < genome.Count - 1; idx++)
                    {
                        // calculate the new weights.
                        double new_weights =
                            problem.Weight(genome[idx], city_to_place)
                                 + (problem.Weight(city_to_place, genome[idx + 1]));

                        // calculate the old weights.
                        double old_weight =
                            problem.Weight(genome[idx], genome[idx + 1]);

                        // calculate the difference to know the increase.
                        double new_increase =
                            new_weights - old_weight;
                        //if (increase == null || new_increase < increase)
                        if (new_increase < increase)
                        {
                            // set the new increase.
                            increase = new_increase;

                            // set the result city.
                            result.CityIdx = 1;
                        }
                    }
                }
            }
            else
            {
                throw new Exception("Cannot do best placement on an empty round!");
            }

            // calculate the fitness.
            result.Increase = increase;

            // return result.
            return result;
        }

        #endregion

        /// <summary>
        /// Class holding the result of a best placement.
        /// </summary>
        public class BestPlacementResult
        {
            /// <summary>
            /// Gets/sets the increase in time.
            /// </summary>
            public double Increase { get; set; }

            /// <summary>
            /// The fitness of the individual after best placement.
            /// </summary>
            public Fitness Fitness { get; set; }

            /// <summary>
            /// The index of the round the city is to be placed in.
            /// </summary>
            public int RoundIdx { get; set; }

            /// <summary>
            /// The index in the round to place the city at.
            /// </summary>
            public int CityIdx { get; set; }

            /// <summary>
            /// The city being placed.
            /// </summary>
            public int City { get; set; }
        }
    }
}
