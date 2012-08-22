using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic;

namespace Tools.Math.VRP.MultiSalesman.Genetic
{
    internal class Individual : Individual<List<Genome>, Problem, Fitness>
    {
        public Individual(List<Genome> genome)
            :base(genome)
        {

        }

        protected Individual(bool fitness_calculated, Fitness fitness)
            : base(fitness_calculated, fitness)
        {

        }

        ///// <summary>
        ///// Make a proper copy of the individual.
        ///// </summary>
        ///// <returns></returns>
        //public override Individual<List<Genome>, Problem, Fitness> Copy()
        //{
        //    List<Genome> genomes = new List<Genome>();
        //    for (int idx = 0; idx < this.Genomes.Count; idx++)
        //    {
        //        genomes.Add(
        //            new Genome(this.Genomes[idx]));
        //    }
        //    Individual copy = null;
        //    //if (!this.FitnessCalculated)
        //    //{
        //    copy = new Individual(false, null);
        //    //}
        //    //else
        //    //{
        //    //    copy = new Individual(this.FitnessCalculated, this.Fitness);
        //    //}
        //    copy.Initialize(genomes);
        //    return copy;
        //}

        public int Count
        {
            get
            {
                int cnt = 0;
                foreach (Genome genome in this.Genomes)
                {
                    cnt = cnt + genome.Count;
                }
                return cnt;
            }
        }

        public override string ToString()
        {
            if (this.FitnessCalculated)
            {
                return string.Format("Individual: #{0} [{1}]",
                    this.Count,
                    this.Fitness.ToString());
            }
            else
            {
                return string.Format("Individual: #{0} [Not Calculated!]",
                    this.Count);
            }
        }

        public override void Validate(Problem problem)
        {
            if (problem.Cities != this.Count)
            {
                throw new Exception("Individual is not valid!");
            }

            foreach (Genome genome in this.Genomes)
            {
                if (genome.Count == 0)
                {
                    throw new Exception("Individual is not valid!");
                }
            }
        }
    }
}
