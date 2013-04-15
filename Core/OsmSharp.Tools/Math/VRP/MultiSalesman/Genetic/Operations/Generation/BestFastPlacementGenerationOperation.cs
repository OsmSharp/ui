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
using OsmSharp.Tools.Math.AI.Genetic.Operations.Generation;
using OsmSharp.Tools.Math.AI.Genetic;
using OsmSharp.Tools.Math.AI.Genetic.Solvers;
using OsmSharp.Tools.Math.Random;
using OsmSharp.Tools.Math.VRP.MultiSalesman.Genetic;
using OsmSharp.Tools.Math.VRP.MultiSalesman.Genetic.Helpers;

namespace OsmSharp.Tools.Math.VRP.MultiSalesman.Solver.Operations.Generation
{
    /// <summary>
    /// Generates new individuals by using best placement.
    /// </summary>
    internal class BestFastPlacementGenerationOperation :
        IGenerationOperation<List<Genome>, Problem, Fitness>
    {
        public string Name
        {
            get
            {
                return "NotSet";
            }
        }

        /// <summary>
        /// Generates one individual.
        /// </summary>
        /// <param name="solver"></param>
        /// <returns></returns>
        public Individual<List<Genome>, Problem, Fitness> Generate(
            Solver<List<Genome>, Problem, Fitness> solver)
        {
            IRandomGenerator random = new RandomGenerator();

            // generate a list of cities to place.
            List<int> cities = new List<int>();
            for (int city_to_place = 0; city_to_place < solver.Problem.Cities; city_to_place++)
            {
                cities.Add(city_to_place);
            }

            // create new individuals.
            Individual individual =
                new Individual(new List<Genome>());
            //individual.Initialize();

            // place one random city in each round.
            for (int round_idx = 0; round_idx < solver.Problem.InitialVehicles; round_idx++)
            {
                // select a random city to place.
                int city_idx = random.Generate(cities.Count);
                int city = cities[city_idx];
                cities.RemoveAt(city_idx);

                // create new genome.
                Genome genome = new Genome();
                genome.Add(city);
                individual.Genomes.Add(genome);
            }

            individual = BestPlacementHelper.DoFast(
                solver.Problem,
                (solver.FitnessCalculator as FitnessCalculator),
                individual,
                cities);

            return individual;
        }
    }
}
