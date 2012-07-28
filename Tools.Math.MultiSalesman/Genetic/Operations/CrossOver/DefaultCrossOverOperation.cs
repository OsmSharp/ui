//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Tools.Math.AI.Genetic.Operations.CrossOver;
//using Tools.Math.AI.Genetic;
//using Tools.Math.AI.Genetic.Solvers;
//using Tools.Math.Random;
//using Tools.Math.VRP.MultiSalesman.Genetic.Helpers;

//namespace Tools.Math.VRP.MultiSalesman.Genetic.Operations.CrossOver
//{
//    public class DefaultCrossOverOperation : ICrossOverOperation<List<Genome>, Problem, Fitness>
//    {
//        public Individual<List<Genome>, Problem, Fitness> CrossOver(
//            Solver<List<Genome>, Problem, Fitness> solver, Individual<List<Genome>, Problem, Fitness> parent1, Individual<List<Genome>, Problem, Fitness> parent2)
//        {
//            bool succes = false;
//            List<Genome> genomes = null;
//            int to_count = solver.Problem.Vehicles;
//            while (!succes)
//            {
//                List<Genome> parent1_genomes = parent1.Copy().Genomes;
//                List<Genome> parent2_genomes = parent2.Copy().Genomes;

//                // FIRST: select genomes from parents without overlapping individuals.
//                bool parent1_possible = true;
//                bool parent2_possible = true;

//                genomes = new List<Genome>();
//                while (parent1_possible || parent2_possible)
//                {
//                    // try from parent 1
//                    bool ok = false;
//                    Genome selected_genome = null;
//                    foreach (Genome genome in parent1_genomes)
//                    {
//                        if (!IndividualHelper.Overlaps(genomes, genome))
//                        {
//                            genomes.Add(genome);
//                            selected_genome = genome;
//                            ok = true;
//                            break;
//                        }
//                    }
//                    if (!ok)
//                    {
//                        parent1_possible = false;
//                    }
//                    else
//                    {
//                        parent1_genomes.Remove(selected_genome);
//                    }
//                    if (genomes.Count == to_count)
//                    {
//                        break;
//                    }

//                    // try from parent 1
//                    foreach (Genome genome in parent2_genomes)
//                    {
//                        if (!IndividualHelper.Overlaps(genomes, genome))
//                        {
//                            genomes.Add(genome);
//                            selected_genome = genome;

//                            ok = true;
//                            break;
//                        }
//                    }
//                    if (!ok)
//                    {
//                        parent2_possible = false;
//                    }
//                    else
//                    {
//                        parent2_genomes.Remove(selected_genome);
//                    }
//                    if (genomes.Count == to_count)
//                    {
//                        break;
//                    }
//                }

//                // list the rest of the cities and divide them into new routes.
//                List<int> rest = new List<int>();
//                for (int city_to_place = 0; city_to_place < solver.Problem.Cities; city_to_place++)
//                {
//                    if (!IndividualHelper.Overlaps(genomes, city_to_place))
//                    {
//                        rest.Add(city_to_place);
//                    }
//                }

//                int extra_rounds = solver.Problem.Vehicles - genomes.Count;
//                if (rest.Count < extra_rounds)
//                {
//                    // no succes!
//                    to_count--;
//                }
//                else
//                {
//                    if (genomes.Count == solver.Problem.Vehicles)
//                    {
//                        // best-place the rest.            
//                        genomes = BestPlacementHelper.Do(
//                            solver.Problem,
//                            (solver.FitnessCalculator as FitnessCalculator),
//                            genomes,
//                            rest);
//                    }
//                    else
//                    {
//                        // create the rest of the routes.
//                        // place one random city in each round.
//                        IRandomGenerator random = StaticRandomGenerator.Get();
//                        List<Genome> rest_genomes = new List<Genome>();
//                        for (int round_idx = 0; round_idx < extra_rounds; round_idx++)
//                        {
//                            // select a random city to place.
//                            int city_idx = random.Generate(rest.Count);
//                            int city = rest[city_idx];
//                            rest.RemoveAt(city_idx);

//                            // create new genome.
//                            Genome genome = new Genome();
//                            genome.Add(city);
//                            rest_genomes.Add(genome);
//                        }

//                        // best-place the rest.            
//                        rest_genomes = BestPlacementHelper.Do(
//                            solver.Problem,
//                            (solver.FitnessCalculator as FitnessCalculator),
//                            rest_genomes,
//                            rest);

//                        genomes.AddRange(rest_genomes);
//                    }
//                    succes = true;
//                }
//            }

//            Individual new_individual = new Individual();
//            new_individual.Initialize(genomes);
//            return new_individual;
//        }
//    }
//}
