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
using OsmSharp.Tools.Math.AI.Genetic.Fitness;
using OsmSharp.Tools.Math.AI.Genetic.Solvers;

namespace OsmSharp.Tools.Math.AI.Genetic
{
    /// <summary>
    /// A class representing a population of individuals.
    /// </summary>
    /// <typeparam name="GenomeType"></typeparam>
    /// <typeparam name="ProblemType"></typeparam>
    /// <typeparam name="WeightType"></typeparam>
    public sealed class Population<GenomeType, ProblemType, WeightType> :
        IEnumerable<Individual<GenomeType, ProblemType, WeightType>>
        where ProblemType : IProblem
        where GenomeType : class
        where WeightType : IComparable
    {
        /// <summary>
        /// Flag telling the population to accept twins or not.
        /// </summary>
        private bool _accept_twins;

        /// <summary>
        /// Holds the individuals in this population.
        /// </summary>
        private List<Individual<GenomeType, ProblemType, WeightType>> _individuals;

        /// <summary>
        /// Initializes a new population.
        /// </summary>
        /// <param name="accept_twins"></param>
        public Population(
            bool accept_twins)
        {
            _accept_twins = accept_twins;
            _individuals = new List<Individual<GenomeType, ProblemType, WeightType>>();
        }

        /// <summary>
        /// Initializes a new population.
        /// </summary>
        /// <param name="individuals"></param>
        /// <param name="accept_twins"></param>
        public Population(
            IEnumerable<Individual<GenomeType, ProblemType, WeightType>> individuals,
            bool accept_twins)
        {
            _accept_twins = accept_twins;

            bool success = true;
            if (individuals != null)
            {
                _individuals = new List<Individual<GenomeType, ProblemType, WeightType>>();
                foreach (Individual<GenomeType, ProblemType, WeightType> individual in individuals)
                {
                    success = success &&
                        this.Add(individual);
                }
                if (!success && !accept_twins)
                {
                    throw new Exception("Cannot initialize a unique population with duplicate entities!");
                }
                //_individuals.AddRange(individuals);
            }
            else
            {
                _individuals = new List<Individual<GenomeType, ProblemType, WeightType>>();
            }
        }

        #region Sorting

        /// <summary>
        /// Sorts the population according to 
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="op"></param>
        public void Sort(
            Solver<GenomeType, ProblemType, WeightType> solver,
            IFitnessCalculator<GenomeType, ProblemType, WeightType> op)
        {
            // create comparer.
            FitnessComparer comparer = new FitnessComparer(solver,op);

            // sort using comparer.
            _individuals.Sort(comparer);
        }

        /// <summary>
        /// Class used to compare individuals using their fitness.
        /// </summary>
        private class FitnessComparer : Comparer<Individual<GenomeType, ProblemType, WeightType>>
        {
            /// <summary>
            /// Hold the operator.
            /// </summary>
            private IFitnessCalculator<GenomeType, ProblemType, WeightType> _op;

            /// <summary>
            /// Holds the solver.
            /// </summary>
            private Solver<GenomeType, ProblemType, WeightType> _solver;

            /// <summary>
            /// Creates a new fitness operator.
            /// </summary>
            /// <param name="solver"></param>
            /// <param name="op"></param>
            public FitnessComparer(
                Solver<GenomeType, ProblemType, WeightType> solver,
                IFitnessCalculator<GenomeType, ProblemType, WeightType> op)
            {
                _op = op;
                _solver = solver;
            }

            /// <summary>
            /// Compare two individuals based on fitness.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public override int Compare(Individual<GenomeType, ProblemType, WeightType> x, Individual<GenomeType, ProblemType, WeightType> y)
            {
                if (!x.FitnessCalculated)
                {
                    x.CalculateFitness(_solver.Problem,_op);
                }
                if (!y.FitnessCalculated)
                {
                    y.CalculateFitness(_solver.Problem, _op);
                }
                return x.Fitness.CompareTo(y.Fitness);
            }
        }

        #endregion

        #region List Functions

        /// <summary>
        /// Returns the element at the given index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns></returns>
        public Individual<GenomeType, ProblemType, WeightType> this[int index]
        {
            get
            {
                return _individuals[index];
            }
            set
            {
                _individuals[index] = value;
            }
        }
        
        /// <summary>
        /// Adds the individual to this population.
        /// </summary>
        /// <param name="individual"></param>
        public bool Add(Individual<GenomeType, ProblemType, WeightType> individual)
        {
            if (!_accept_twins && _individuals.Contains(individual))
            {
                return false;
            }
            else
            {
                _individuals.Add(individual);
                return true;
            }
        }

        /// <summary>
        /// Adds the individuals of the specified collection to the end of the population.
        /// </summary>
        /// <param name="collection">The collection whose elements should be added to the end of the population.The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>
        public bool AddRange(IEnumerable<Individual<GenomeType, ProblemType, WeightType>> collection)
        {
            bool success = true;
            foreach (Individual<GenomeType, ProblemType, WeightType> individual in collection)
            {
                success = success &&
                    this.Add(individual);
            }
            return success;
        }

        /// <summary>
        /// Creates a shallow copy of a range of elements in the source population.
        /// </summary>
        /// <param name="index">The zero-based population index at which the range</param>
        /// <param name="count">The number of elements in the range.</param>
        /// <returns></returns>
        public List<Individual<GenomeType, ProblemType, WeightType>> GetRange(int index, int count)
        {
            return _individuals.GetRange(index, count);
        }

        /// <summary>
        /// Returns then number of individuals in this population.
        /// </summary>
        public int Count
        {
            get
            {
                return _individuals.Count;
            }
        }

        /// <summary>
        /// Returns true if the individual is contained in the population.
        /// </summary>
        /// <param name="individual"></param>
        /// <returns></returns>
        public bool Contains(Individual<GenomeType, ProblemType, WeightType> individual)
        {
            return _individuals.Contains(individual);
        }

        /// <summary>
        /// Removes the individual at the given idx.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public void RemoveAt(int idx)
        {
            _individuals.RemoveAt(idx);
        }

        #endregion

        #region Enumerator

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Individual<GenomeType, ProblemType, WeightType>> GetEnumerator()
        {
            return new EnumeratorGeneric(this);
        }

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new EnumeratorGeneric(this);
        }

        /// <summary>
        /// Enumeration class specific for a population like this.
        /// </summary>
        private class EnumeratorGeneric : IEnumerator<Individual<GenomeType, ProblemType, WeightType>>
        {
            private int _idx;

            Population<GenomeType, ProblemType, WeightType> _pop;

            public EnumeratorGeneric(Population<GenomeType, ProblemType, WeightType> pop)
            {
                _idx = -1;
                _pop = pop;
            }

            #region IEnumerator<Individual<GenomeType, ProblemType>> Members

            public Individual<GenomeType, ProblemType, WeightType> Current
            {
                get 
                {
                    return _pop[_idx];
                }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                _pop = null;
            }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get 
                {
                    return _pop[_idx];
                }
            }

            public bool MoveNext()
            {
                _idx++;
                return _pop.Count > _idx;
            }

            public void Reset()
            {
                _idx = -1;
            }

            #endregion
        }

        #endregion
    }    
}
