using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Operations.Generation;
using Tools.Math.AI.Genetic;
using Tools.Math.AI.Genetic.Solvers;
using Tools.Math.Random;
using Tools.Math.VRP.MultiSalesman.Genetic;
using Tools.Math.VRP.MultiSalesman.Genetic.Helpers;

namespace Tools.Math.VRP.MultiSalesman.Solver.Operations.Generation
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
