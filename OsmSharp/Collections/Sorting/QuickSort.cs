// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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

namespace OsmSharp.Collections.Sorting
{
    /// <summary>
    /// An implementation of the quicksort algorithm.
    /// </summary>
    public static class QuickSort
    {
        /// <summary>
        /// Executes a quicksort algorithm given the value and swap methods.
        /// </summary>
        public static void Sort(Func<long, long> value, Action<long, long> swap, long left, long right)
        {
            if (left < right)
            { // left is still to the left.
                var pivot = QuickSort.Partition(value, swap, left, right);
                if (left <= pivot && pivot <= right)
                {
                    QuickSort.Sort(value, swap, left, pivot - 1);
                    QuickSort.Sort(value, swap, pivot + 1, right);
                }
            }
        }

        /// <summary>
        /// Partion the based on the pivot value.
        /// </summary>
        /// <return>The new left.</return>
        private static long Partition(Func<long, long> value, Action<long, long> swap, long left, long right)
        { // get the pivot value.
            if (left > right) { throw new ArgumentException("left should be smaller than or equal to right."); }
            if (left == right)
            { // sorting just one item results in that item being sorted already and a pivot equal to that item itself.
                return right;
            }

            // start with the left as pivot value.
            var pivot = left;
            var pivotValue = value(pivot);

            while (true)
            {
                // move the left to the right until the first value bigger than pivot.
                var leftValue = value(left + 1);
                while (leftValue <= pivotValue)
                {
                    left++;
                    if(left == right)
                    {
                        break;
                    }
                    leftValue = value(left + 1);
                }

                // move the right to left until the first value smaller than pivot.
                if (left != right)
                {
                    var rightValue = value(right);
                    while (rightValue > pivotValue)
                    {
                        right--;
                        if (left == right)
                        {
                            break;
                        }
                        rightValue = value(right);
                    }
                }

                if (left == right)
                { // we are done searching, left == right.
                    // make sure the pivot value is where it is supposed to be.
                    swap(pivot, left);
                    return left;
                }

                // swith left<->right.
                swap(left + 1, right);
            }
        }
    }
}
