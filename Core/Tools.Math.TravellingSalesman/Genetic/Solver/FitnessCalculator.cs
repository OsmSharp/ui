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
using Tools.Math.AI.Genetic.Fitness;
using Tools.Math.AI.Genetic.Solvers;
using Tools.Math.AI.Genetic;

namespace Tools.Math.TSP.Genetic.Solver
{
    public class FitnessCalculator :
        IFitnessCalculator<List<int>, GeneticProblem, Fitness>
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
            Individual<List<int>, GeneticProblem, Fitness> individual)
        {
            return this.Fitness(problem, individual.Genomes);
        }

        public Fitness Fitness(
            GeneticProblem problem,
            Individual<List<int>, GeneticProblem, Fitness> individual, bool validate)
        {
            return this.Fitness(problem, individual.Genomes);
        }

        public Fitness Fitness(
            GeneticProblem problem,
            List<int> genomes)
        {
            double[][] weights = problem.BaseProblem.WeightMatrix;
            double weight = weights[problem.First][genomes[0]];
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

        public Fitness AverageFitness(GeneticProblem problem, IEnumerable<Individual<List<int>, GeneticProblem, Fitness>> population)
        {
            throw new NotImplementedException();
        }
    }
}
