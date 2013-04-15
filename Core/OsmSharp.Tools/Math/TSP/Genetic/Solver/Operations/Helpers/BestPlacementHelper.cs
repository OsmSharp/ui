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
using OsmSharp.Tools.Math.Random;

namespace OsmSharp.Tools.Math.TSP.Genetic.Solver.Operations.Helpers
{
    internal class BestPlacementHelper
    {
        /// <summary>
        /// Calculates and places this city in the least cost position.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="calculator"></param>
        /// <param name="genomes"></param>
        /// <param name="city_to_place"></param>
        public void Do(GeneticProblem problem, FitnessCalculator calculator, List<int> genomes, int city_to_place)
        {
            // calculate the result.
            BestPlacementResult result = this.CalculateBestPlacement(problem, calculator, genomes, city_to_place);

            // do the placement.
            this.Do(genomes, result);
        }

        /// <summary>
        /// Calculates and places the given cities one by one in the least cost position.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="calculator"></param>
        /// <param name="genomes"></param>
        /// <param name="cities_to_place"></param>
        public void Do(GeneticProblem problem, FitnessCalculator calculator, 
            List<int> genomes, List<int> cities_to_place)
        {
            this.RaiseNewStart();
            while (cities_to_place.Count > 0)
            {
                // calculate the result.
                BestPlacementResult result = this.CalculateBestPlacement(problem, calculator, genomes, cities_to_place);

                // remove from the cities to place list.
                cities_to_place.Remove(result.City);

                // do the placement.
                this.Do(genomes, result);
            }
        }

        /// <summary>
        /// Calculates and places the given cities by randomly selecting them and placing them into the least cost position.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="calculator"></param>
        /// <param name="genomes"></param>
        /// <param name="cities_to_place"></param>
        public void DoFast(GeneticProblem problem, FitnessCalculator calculator, List<int> genomes, List<int> cities_to_place)
        {
            this.RaiseNewStart();
            while (cities_to_place.Count > 0)
            {
                // select a random city.
                int random_idx = StaticRandomGenerator.Get().Generate(cities_to_place.Count);
                int city = cities_to_place[random_idx];
                this.RaiseNewCity(city);
                
                // remove the selected city.
                cities_to_place.RemoveAt(random_idx);

                // calculate the result.
                BestPlacementResult result = this.CalculateBestPlacement(problem, calculator, genomes, city);

                // do the placement.
                this.Do(genomes, result);
                this.RaiseNewRoute(genomes);
            }
        }

        /// <summary>
        /// Applies the result to the genome.
        /// </summary>
        /// <param name="genomes"></param>
        /// <param name="result"></param>
        public void Do(List<int> genomes, BestPlacementResult result)
        {
            if (genomes.Count == 0)
            {
                genomes.Add(result.City);
            }
            else if (genomes.Count == result.CityIdx)
            {
                genomes.Add(result.City);
            }
            else
            {
                genomes.Insert(result.CityIdx, result.City);
            }
        }

        #region Calculations

        /// <summary>
        /// Searches for the best place to insert the given city.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="calculator"></param>
        /// <param name="genomes"></param>
        /// <param name="city_to_place"></param>
        /// <returns></returns>
        public BestPlacementResult CalculateBestPlacement(
            GeneticProblem problem,
            FitnessCalculator calculator,
            List<int> genomes,
            int city_to_place)
        {  // initialize the best placement result.
            BestPlacementResult result
                = new BestPlacementResult();
            result.City = city_to_place;

            // initialize the best increase.
            double increase = 0;

            if (genomes.Count > 0)
            {
                // calculate from first.
                double new_weights =
                    problem.Weight(problem.First, city_to_place)
                        + (problem.Weight(city_to_place, genomes[0]));

                // calculate the old weights.
                double old_weight =
                    problem.Weight(problem.First, genomes[0]);

                // calculate the difference to know the increase.
                double new_increase =
                    new_weights - (old_weight);

                // set the new increase.
                increase = new_increase;

                // set the result city.
                result.CityIdx = 0;

                for (int idx = 0; idx < genomes.Count - 1; idx++)
                {
                    // calculate the new weights.
                    new_weights =
                        problem.Weight(genomes[idx], city_to_place)
                            + (problem.Weight(city_to_place, genomes[idx + 1]));

                    // calculate the old weights.
                    old_weight =
                        problem.Weight(genomes[idx], genomes[idx + 1]);

                    // calculate the difference to know the increase.
                    new_increase =
                        new_weights - (old_weight);
                    //if (increase == null || new_increase < increase)
                    if (new_increase < increase)
                    {
                        // set the new increase.
                        increase = new_increase;

                        // set the result city.
                        result.CityIdx = idx + 1;
                    }
                }

                // test to the last.
                // calculate the new weights.
                new_weights =
                    problem.Weight(genomes[genomes.Count - 1], city_to_place)
                        + (problem.Weight(city_to_place, problem.Last));

                // calculate the old weights.
                old_weight =
                    problem.Weight(genomes[genomes.Count - 1], problem.Last);

                // calculate the difference to know the increase.
                new_increase =
                    new_weights - (old_weight);
                //if (increase == null || new_increase < increase)
                if (new_increase < increase)
                {
                    // set the new increase.
                    increase = new_increase;

                    // set the result city.
                    result.CityIdx = genomes.Count;
                }
            }
            else
            {
                // calculate the new weights.
                double new_weights =
                    problem.Weight(problem.First, city_to_place)
                        + (problem.Weight(city_to_place, problem.Last));

                // calculate the old weights.
                double old_weight =
                    problem.Weight(problem.First, problem.Last);

                // calculate the difference to know the increase.
                double new_increase =
                    new_weights - (old_weight);

                // set the new increase.
                increase = new_increase;

                // set the result city.
                result.CityIdx = 0;                
            }

            // calculate the fitness.
            result.Increase = increase;

            // return result.
            return result;
        }

        /// <summary>
        /// Searches for the city that can be placed best with the least increase in cost.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="calculator"></param>
        /// <param name="genomes"></param>
        /// <param name="cities_to_place"></param>
        /// <returns></returns>
        public BestPlacementResult CalculateBestPlacement(
            GeneticProblem problem,
            FitnessCalculator calculator,
            List<int> genomes,
            List<int> cities_to_place)
        {
            // initialize the best placement result.
            BestPlacementResult result
                = new BestPlacementResult();

            // try and place all cities.
            for (int city_idx = 0; city_idx < cities_to_place.Count; city_idx++)
            {
                // try to place first city.
                int city = cities_to_place[city_idx];

                // place the city and check the result.
                BestPlacementResult current_result =
                    this.CalculateBestPlacement(
                        problem,
                        calculator,
                        genomes,
                        city);

                // check current result
                if (current_result.Increase < result.Increase)
                {
                    result = current_result;
                }
            }

            return result;
        }

        #endregion           

        #region Events

        public delegate void NewRouteDelegate(List<int> route);
        public event NewRouteDelegate NewRoute;
        public void RaiseNewRoute(List<int> route)
        {
            if (NewRoute != null)
            {
                NewRoute(route);
            }
        }

        public delegate void NewCityDelegate(int city);
        public event NewCityDelegate NewCity;
        public void RaiseNewCity(int city)
        {
            if (NewCity != null)
            {
                NewCity(city);
            }
        }

        public delegate void NewStartDelegate();
        public event NewStartDelegate NewStart;
        public void RaiseNewStart()
        {
            if (NewStart != null)
            {
                NewStart();
            }
        }

        #endregion

        internal static BestPlacementHelper Instance()
        {
            return new BestPlacementHelper();
        }
    }

    internal class BestPlacementResult
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
        /// The index in the genome to place the city at.
        /// </summary>
        public int CityIdx { get; set; }

        /// <summary>
        /// The city being placed.
        /// </summary>
        public int City { get; set; }
    }
}
