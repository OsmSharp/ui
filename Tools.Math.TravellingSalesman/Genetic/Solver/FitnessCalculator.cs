using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Fitness;
using Tools.Math.AI.Genetic.Solvers;
using Tools.Math.AI.Genetic;
using Tools.Math.Graph;

namespace Tools.Math.TSP.Genetic.Solver
{
    public class FitnessCalculator :
        IFitnessCalculator<int, GeneticProblem, Fitness>
    {
        public static double EPSILON = 1;

        #region IFitnessCalculator<int,Problem> Members

        public double Epsilon
        {
            get 
            {
                return EPSILON; 
            }
        }

        public Fitness Fitness(
            GeneticProblem problem,
            Individual<int, GeneticProblem, Fitness> individual)
        {
            return this.Fitness(problem, individual.Genomes);
        }

        public Fitness Fitness(
            GeneticProblem problem,
            Individual<int, GeneticProblem, Fitness> individual, bool validate)
        {
            return this.Fitness(problem, individual.Genomes);
        }

        public Fitness Fitness(
            GeneticProblem problem,
            IList<int> genomes)
        {
            float[][] weights = problem.BaseProblem.WeightMatrix;
            float weight = weights[problem.First][genomes[0]];
            int idx;
            for (idx = 0; idx < genomes.Count - 1; idx++)
            {
                //weight = weight + (weights[genomes[idx]][genomes[idx + 1]]);
                weight = weight + (weights[genomes[idx]][genomes[idx + 1]]);
            }
            weight = weight + (weights[genomes[idx]][problem.Last]);

            return new Fitness(weight);
        }

        #endregion

        #region IFitnessCalculator<int,GeneticProblem,Fitness> Members

        //public Fitness FitnessPart(GeneticProblem problem, int first, int second)
        //{
        //    return new Fitness(problem.Weight(first, second));
        //}

        //public Fitness FitnessFirstPart(GeneticProblem problem, IList<int> genomes)
        //{
        //    float weight = problem.Weight(problem.First, genomes[0]);
        //    int idx;
        //    for (idx = 0; idx < genomes.Count - 1; idx++)
        //    {
        //        weight = weight + (problem.Weight(genomes[idx], genomes[idx + 1]));
        //    }
        //    //weight = weight + (problem.Weight(genomes[idx], problem.Last));

        //    return new Fitness(weight);
        //}

        //public Fitness FitnessLastPart(GeneticProblem problem, IList<int> genomes)
        //{
        //    //float weight = problem.Weight(problem.First, genomes[0]);
        //    float weight = 0;
        //    int idx;
        //    for (idx = 0; idx < genomes.Count - 1; idx++)
        //    {
        //        weight = weight + 
        //            (problem.Weight(genomes[idx], genomes[idx + 1]));
        //    }
        //    weight = weight + (problem.Weight(genomes[idx], problem.Last));

        //    return new Fitness(weight);
        //}

        //public Fitness FitnessPart(GeneticProblem problem, IList<int> genome_part)
        //{
        //    float weight = 0;
        //    int idx;
        //    for (idx = 0; idx < genome_part.Count - 1; idx++)
        //    {
        //        weight = weight + (problem.Weight(genome_part[idx], genome_part[idx + 1]));
        //    }
        //    return new Fitness(weight);
        //}

        #endregion


        public Fitness AverageFitness(GeneticProblem problem, IEnumerable<Individual<int, GeneticProblem, Fitness>> population)
        {
            throw new NotImplementedException();
        }
    }
}
