// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// Foobar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Foobar is distributed in the hope that it will be useful,
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

namespace Tools.Core
{
    /// <summary>
    /// Class containing some utilities and extension methods.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Removes one element from an array and returns the result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T[] Remove<T>(this T[] array, T value)
        {
            List<T> list = new List<T>(array);
            list.Remove(value);
            return list.ToArray();
        }

        /// <summary>
        /// Removes one element from an array and returns the result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T[] Add<T>(this T[] array, T value)
        {
            List<T> list = new List<T>(array);
            list.Add(value);
            return list.ToArray();
        }

        /// <summary>
        /// Removes one element from an array and returns the result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T[] AddRange<T>(this T[] array, IEnumerable<T> value)
        {
            List<T> list = new List<T>(array);
            list.AddRange(value);
            return list.ToArray();
        }

        /// <summary>
        /// Tests two IEnumerables for equal values and equal count.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static Boolean EqualValues<T>(this IEnumerable<T> self, IEnumerable<T> items)
            where T : IEquatable<T>
        {
            // get enumerator.
            IEnumerator<T> enum1 = self.GetEnumerator();
            IEnumerator<T> enum2 = items.GetEnumerator();
            bool enum1_has_next = enum1.MoveNext();
            bool enum2_has_next = enum2.MoveNext();
            
            // start comparing.
            bool equals = true;
            if (enum1_has_next && enum2_has_next)
            { // at least one in collections.
                while (equals
                    && (enum1_has_next && enum2_has_next))
                {
                    if (!enum1.Current.Equals(enum2.Current))
                    {
                        equals = false;
                    }
                    else
                    {
                        enum1_has_next = enum1.MoveNext();
                        enum2_has_next = enum2.MoveNext();
                    }
                }

                return equals && (enum1_has_next == enum2_has_next);
            }
            else 
            { // one of the collection or both have zero elements.
                return (enum1_has_next != enum2_has_next);
            }
        }

        /// <summary>
        /// Shuffles the list using Fisher-Yates shuffle.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
        /// Generates a random string.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomString(int length)
        {
            byte[] randBuffer = new byte[length];
            System.Security.Cryptography.RandomNumberGenerator.Create().GetBytes(randBuffer);
            return System.Convert.ToBase64String(randBuffer).Remove(length);
        }

        /// <summary>
        /// Converts a number of milliseconds from 1/1/1970 into a standard DateTime.
        /// </summary>
        /// <param name="unixTime"></param>
        /// <returns></returns>
        public static DateTime FromUnixTime(this long milliseconds)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(milliseconds);
        }

        /// <summary>
        /// Converts a standard DateTime into the number of milliseconds since 1/1/1970.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static long ToUnixTime(this DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalMilliseconds);
        }

    }
}
