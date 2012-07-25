//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Tools.Math.AI.Genetic.Solvers;
//using Tools.Math.AI.Genetic.Selectors;
//using Tools.Math.AI.Genetic.Operations.Mutations;
//using Tools.Math.AI.Genetic.Operations.CrossOver;
//using Tools.Math.AI.Genetic.Operations.Generation;
//using Tools.Math.AI.Genetic.Fitness;

//namespace Tools.Math.TSP.Genetic.Solver
//{
//    public class Solver : Solver<int, GeneticProblem, Fitness>
//    {
//        public Solver(
//            GeneticProblem problem,
//            SolverSettings settings,
//            ISelector<int, GeneticProblem, Fitness> selector,
//            IMutationOperation<int, GeneticProblem, Fitness> mutation,
//            ICrossOverOperation<int, GeneticProblem, Fitness> cross_over,
//            IGenerationOperation<int, GeneticProblem, Fitness> generation,
//            IFitnessCalculator<int, GeneticProblem, Fitness> fitness_calculator,
//            bool accept_only_better_cross_over, bool accept_only_better_mutation)
//            :base(problem, settings,selector, mutation,cross_over,generation,fitness_calculator, accept_only_better_cross_over, accept_only_better_mutation)
//        {

//        }

//        protected override AI.Genetic.Operations.Generation.IGenerationOperation<int, GeneticProblem, Fitness> CreateGenerationOperation()
//        {
//            return new Operations.Generation.BestPlacementGenerationOperation();
//            //return new Operations.Generation.LKGenerationOperation();
//        }

//        protected override IMutationOperation<int, GeneticProblem, Fitness> CreateMutationOperation()
//        {
//            return new Operations.Mutation.BestDetailedPlacementMutationOperation();
//        }

//        protected override ICrossOverOperation<int, GeneticProblem, Fitness> CreateCrossoverOperation()
//        {
//            return new Operations.CrossOver.SequentialContructiveCrossoverOperator();
//        }
//    }
//}
