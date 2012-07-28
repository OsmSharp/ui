using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Operations.Mutations;
using Tools.Math.AI.Genetic;
using Tools.Math.AI.Genetic.Solvers;
using Tools.Math.Random;
using Tools.Math.VRP.MultiSalesman.Genetic;
using Tools.Math.VRP.MultiSalesman.Genetic.Helpers;

namespace Tools.Math.VRP.MultiSalesman.Solver.Operations.Mutation
{
    internal class TakeOutMutationOperation : IMutationOperation<List<Genome>, Problem, Fitness>
    {
        public string Name
        {
            get
            {
                return "NotSet";
            }
        }

        public Individual<List<Genome>, Problem, Fitness> Mutate(
            Solver<List<Genome>, Problem, Fitness> solver, Individual<List<Genome>, Problem, Fitness> mutating)
        {
            //if (mutating.Genomes.Count < 2)
            //{
            //    return mutating;
            //}

            //Individual<List<Genome>, Problem, Fitness> copy = mutating.Copy();
            //if (!copy.FitnessCalculated)
            //{
            //    copy.CalculateFitness(solver.Problem, solver.FitnessCalculator);
            //}
            //// select a random round giving preference to the big ones.
            //Genome big_one = IndividualHelper.SelectRandom(copy, false);
            //if(big_one.Count <= 1)
            //{ // big_one has to have at least 2 customers to be able to remove one.
            //    // do nothing if this is the case
            //    return copy;
            //}

            //if (copy.Genomes.Count == 0)
            //{
            //    return copy;
            //}

            //// select a random customer from the big one and remove it.
            //int customer_idx = StaticRandomGenerator.Get().Generate(big_one.Count);
            //int customer = big_one[customer_idx];
            //big_one.RemoveAt(customer_idx);

            //// select all genomes except the big one.
            //List<Genome> genomes = IndividualHelper.Except(copy.Genomes, big_one);

            //// do best placement in all the other genomes.
            //genomes = BestPlacementHelper.DoFast(
            //    solver.Problem,
            //    (solver.FitnessCalculator as FitnessCalculator),
            //    genomes,
            //    customer);
            //copy.CalculateFitness(solver.Problem, solver.FitnessCalculator);
            //return copy;

            throw new NotImplementedException("Not re-implemented after refactoring GA");
        }
    }
}
