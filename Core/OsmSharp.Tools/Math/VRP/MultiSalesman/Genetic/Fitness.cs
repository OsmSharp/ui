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

namespace OsmSharp.Tools.Math.VRP.MultiSalesman.Genetic
{
    /// <summary>
    /// Fitness object used to represent fitness of an individual.
    /// </summary>
    internal class Fitness : IComparable
    {
        /// <summary>
        /// The number of vehicles
        /// </summary>
        public int Vehicles { get; set; }

        /// <summary>
        /// The category the smallest round can be found at.
        /// </summary>
        public int SmallestRoundCategory { get; set; }

        /// <summary>
        /// The category the largest round can be found at.
        /// </summary>
        public int LargestRoundCategory { get; set; }

        /// <summary>
        /// The sum of the categories.
        /// </summary>
        public int CategorySum
        {
            get
            {
                return this.SmallestRoundCategory + this.LargestRoundCategory;
            }
        }

        /// <summary>
        /// The total time of all rounds combined.
        /// </summary>
        public double TotalTime { get; set; }

        /// <summary>
        /// The time of the shortest round.
        /// </summary>
        public double MinimumTime { get; set; }

        /// <summary>
        /// The time of the longest round.
        /// </summary>
        public double MaximumTime { get; set; }

        /// <summary>
        /// The time per round.
        /// </summary>
        public List<double> Times { get; set; }

        /// <summary>
        /// The largest category per round.
        /// </summary>
        public List<int> LargestRoundCategories { get; set; }

        /// <summary>
        /// The smallest category per round.
        /// </summary>
        public List<int> SmallestRoundCategories { get; set; }

        /// <summary>
        /// The problem this fitness needs to be evaluated against.
        /// </summary>
        public Problem Problem { get; set; }

        /// <summary>
        /// True if the solution is feasable.
        /// </summary>
        public bool Feasable { get; set; }

        /// <summary>
        /// Compares this fitness object to another.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj is Fitness)
            {
                //Fitness other = (obj as Fitness);
                //int this_weighed_count = this.CategorySum; // (int)((double)this.LargestRoundCategory * 1.2 + (double)this.SmallestRoundCategory);
                //if (this_weighed_count < this.TotalTolerance)
                //{
                //    this_weighed_count = 0;
                //}
                //int other_weighed_count = other.CategorySum; //  (int)((double)other.LargestRoundCategory * 1.2 + (double)other.SmallestRoundCategory);
                //if (other_weighed_count < other.TotalTolerance)
                //{
                //    other_weighed_count = 0;
                //}
                //int equals = this_weighed_count.CompareTo(other_weighed_count);
                //if (equals == 0)
                //{
                //    ////equals = this.SmallestRoundCategory.CompareTo(other.SmallestRoundCategory);
                //    ////if (equals == 0)
                //    ////{
                //    if (this.SmallestRoundCategory == 50 &&
                //        other.SmallestRoundCategory == 50)
                //    {
                //        equals =
                //            this.Vehicles.CompareTo(other.Vehicles);
                //        if (equals == 0)
                //        {
                //            equals =
                //                this.TotalTime.CompareTo(other.TotalTime);
                //        }
                //    }
                //    else
                //    {
                //        equals =
                //            this.TotalTime.CompareTo(other.TotalTime);
                //    }
                //    //}
                //}
                //return -equals;

                //Fitness other = (obj as Fitness);
                //int this_weighed_count = this.CategorySum; // (int)((double)this.LargestRoundCategory * 1.2 + (double)this.SmallestRoundCategory);
                //int other_weighed_count = other.CategorySum; //  (int)((double)other.LargestRoundCategory * 1.2 + (double)other.SmallestRoundCategory);
                //int equals = this_weighed_count.CompareTo(other_weighed_count);
                //if (equals == 0)
                //{
                //    //equals = this.SmallestRoundCategory.CompareTo(other.SmallestRoundCategory);
                //    //if (equals == 0)
                //    //{
                //        equals =
                //            this.Vehicles.CompareTo(other.Vehicles);
                //        if (equals == 0)
                //        {
                //            equals =
                //                this.TotalTime.CompareTo(other.TotalTime);
                //        }
                //    //}
                //}
                //return -equals;

                //Fitness other = (obj as Fitness);
                //int equals = this.LargestRoundCategory.CompareTo(other.LargestRoundCategory);
                //if (equals == 0)
                //{
                //    equals = this.SmallestRoundCategory.CompareTo(other.SmallestRoundCategory);
                //    if (equals == 0)
                //    {
                //        equals =
                //            this.Vehicles.CompareTo(other.Vehicles);
                //        if (equals == 0)
                //        {
                //            equals =
                //                this.TotalTime.CompareTo(other.TotalTime);
                //        }
                //    }
                //}
                //return -equals;
                //Fitness other = (obj as Fitness);
                //int equals =
                //        this.CategorySum.CompareTo(other.CategorySum);
                //if (equals == 0)
                //{
                //    equals =
                //        this.Vehicles.CompareTo(other.Vehicles);
                //    if (equals == 0)
                //    {
                //        equals =
                //            this.TotalTime.CompareTo(other.TotalTime);
                //    }
                //}
                //return -equals;

                Fitness other = (obj as Fitness);
                int equals = 0;
                if (this.Feasable && other.Feasable)
                { // if in regime only consider total time.
                    equals = this.TotalTime.CompareTo(other.TotalTime);
                }
                else
                { // compare the category sum.
                    equals = this.CategorySum.CompareTo(other.CategorySum);
                    if (equals == 0)
                    { // compare the vehicles.
                        equals = this.Vehicles.CompareTo(other.Vehicles);
                        if (equals == 0)
                        { // compare the total time.
                            equals = this.TotalTime.CompareTo(other.TotalTime);
                        }
                    }
                }
                return -equals;
            }
            else if (obj == null)
            {
                return 1;
            }
            throw new ArgumentOutOfRangeException();
        }

        public override string ToString()
        {
            return string.Format("L:{0} S:{1} Vechicles:{2} Time:{3}[{4}-{5}]",
                this.LargestRoundCategory,
                this.SmallestRoundCategory,
                this.Vehicles,
                System.Math.Round(this.TotalTime, 2),
                System.Math.Round(this.MinimumTime, 2),
                System.Math.Round(this.MaximumTime, 2));
        }

        #region Operator Overloading

        public static bool operator ==(Fitness left, Fitness right)
        {
            return left.CompareTo(right) == 0;
        }

        public static bool operator !=(Fitness left, Fitness right)
        {
            return left.CompareTo(right) != 0;
        }

        public static bool operator <(Fitness left, Fitness right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >(Fitness left, Fitness right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(Fitness left, Fitness right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static bool operator >=(Fitness left, Fitness right)
        {
            return left.CompareTo(right) <= 0;
        }

        #endregion

        protected bool Equals(Fitness other)
        {
            return Vehicles == other.Vehicles && SmallestRoundCategory == other.SmallestRoundCategory && LargestRoundCategory == other.LargestRoundCategory && TotalTime.Equals(other.TotalTime) && MinimumTime.Equals(other.MinimumTime) && MaximumTime.Equals(other.MaximumTime) && Equals(LargestRoundCategories, other.LargestRoundCategories) && Equals(SmallestRoundCategories, other.SmallestRoundCategories) && Feasable.Equals(other.Feasable);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Fitness)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Vehicles;
                hashCode = (hashCode * 397) ^ SmallestRoundCategory;
                hashCode = (hashCode * 397) ^ LargestRoundCategory;
                hashCode = (hashCode * 397) ^ TotalTime.GetHashCode();
                hashCode = (hashCode * 397) ^ MinimumTime.GetHashCode();
                hashCode = (hashCode * 397) ^ MaximumTime.GetHashCode();
                hashCode = (hashCode * 397) ^ (LargestRoundCategories != null ? LargestRoundCategories.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (SmallestRoundCategories != null ? SmallestRoundCategories.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Feasable.GetHashCode();
                return hashCode;
            }
        }
    }
}
