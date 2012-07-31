//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Tools.Math.AI.Genetic.Operations.Mutations;
//using Tools.Math.AI.Genetic;
//using Tools.Math.AI.Genetic.Solvers;
//using Tools.Math.VRP.Core.BestPlacement;
//using Tools.Math.VRP.Core.Routes;

//namespace Osm.Routing.Core.VRP.NoDepot.MaxTime.Genetic.Mutation
//{
//    internal class RelocationMutation :
//        IMutationOperation<List<Genome>, Problem, Fitness>
//    {

//        public string Name
//        {
//            get
//            {
//                return "REL";
//            }
//        }

//        public Individual<List<Genome>, Problem, Fitness> Mutate(Solver<List<Genome>, Problem, Fitness> solver, 
//            Individual<List<Genome>, Problem, Fitness> mutating)
//        {
//            Genome genome = mutating.Genomes[0];

//            // get a random route.
//            int point_x_source = Tools.Math.Random.StaticRandomGenerator.Get().Generate(genome.Customers.Length);
//            int customer = genome.Customers[point_x_source];

//            // find the source route.
//            int total = 0;
//            int source_route_idx = -1;
//            for (int idx = 0; idx < genome.Sizes.Length; idx++)
//            {
//                total = total + genome.Sizes[idx];
//                if (point_x_source <= total)
//                {
//                    source_route_idx = idx;
//                    break;
//                }
//            }

//            // copy the orginal and remove the customer.
//            Genome mutated = new Genome();
//            mutated.Sizes = genome.Sizes.Clone() as int[];
//            mutated.Sizes[source_route_idx] = mutated.Sizes[source_route_idx] - 1;

//            List<int> customers = new List<int>(genome.Customers);
//            customers.RemoveAt(point_x_source);
//            mutated.Customers = customers.ToArray();

//            // try reinsertion.
//            CheapestInsertionResult result = new CheapestInsertionResult();
//            result.Increase = float.MaxValue;
//            int target_idx = -1;
//            for (int idx = 0; idx < genome.Sizes.Length; idx++)
//            {
//                IRoute route = mutated.Route(idx);

//                if (mutated.Sizes[idx] > 0)
//                {
//                    CheapestInsertionResult current_result =
//                        CheapestInsertionHelper.CalculateBestPlacement(solver.Problem, route, customer);
//                    if (current_result.Increase < result.Increase)
//                    {
//                        target_idx = idx;
//                        result = current_result;
//                    }
//                }
//            }

//            // insert the customer.
//            customers = new List<int>(mutated.Customers);
//            for (int idx = 0; idx < customers.Count; idx++)
//            {
//                if (customers[idx] == result.CustomerBefore)
//                {
//                    if (customers.Count - 1 == idx)
//                    {
//                        customers.Add(customer);
//                    }
//                    else
//                    {
//                        customers.Insert(idx + 1, customer);
//                    }
//                    break;
//                }
//            }

//            // set the mutated.
//            mutated.Sizes[target_idx] = mutated.Sizes[target_idx] + 1;
//            mutated.Customers = customers.ToArray();

//            // remove all zero's.
//            List<int> sizes = new List<int>(mutated.Sizes);
//            while (sizes.Remove(0))
//            {

//            }
//            mutated.Sizes = sizes.ToArray<int>();
            
//            List<Genome> genomes = new List<Genome>();

//            if (!mutated.IsValid())
//            {
//                throw new Exception();
//            }
//            genomes.Add(mutated);
//            Individual<List<Genome>, Problem, Fitness> individual = new Individual<List<Genome>, Problem, Fitness>(genomes);
//            //individual.Initialize();
//            return individual;
//        }
//    }
//}
