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
using System.Text;
using System.Globalization;
using System.Security.Cryptography;

namespace OsmSharp.Tools
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
            RandomNumberGenerator generator = new RNGCryptoServiceProvider();
            generator.GetBytes(randBuffer);
            return System.Convert.ToBase64String(randBuffer).Remove(length);
        }

        /// <summary>
        /// Converts a number of milliseconds from 1/1/1970 into a standard DateTime.
        /// </summary>
        /// <param name="milliseconds"></param>
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

        /// <summary>
        /// Returns a trucated string if the string is larger than the given max length.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="max_length"></param>
        /// <returns></returns>
        public static string Truncate(this string value, int max_length)
        {
            if (value != null && value.Length > max_length)
            {
                return value.Substring(0, max_length);
            }
            return value;
        }

        /// <summary>
        /// Converts a list of tags to a dictionary of tags.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static IDictionary<string, string> ConvertFrom(this List<KeyValuePair<string, string>> tags)
        {
            Dictionary<string, string> new_tags = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> tag in tags)
            {
                new_tags[tag.Key] = tag.Value;
            }
            return new_tags;
        }

        /// <summary>
        /// Retuns a string of a fixed length.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string PadRightAndCut(this string s, int length)
        {
            return s.ToStringEmptyWhenNull().PadRight(length).Substring(0, length);
        }

        /// <summary>
        /// Matches two string that contain a given percentage of the same characters.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public static bool LevenshteinMatch(this string s, string t, float percentage)
        {
            if (s == null || t == null)
            {
                return false;
            }
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];
            int match = -1;
            int size = System.Math.Max(n, m);

            if (size == 0)
            { // empty strings cannot be matched.
                return false;
            }

            // Step 1
            if (n == 0)
            {
                match = m;
            }

            if (m == 0)
            {
                match = n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = System.Math.Min(
                        System.Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            match = d[n, m];

            // calculate the percentage.
            return ((float)(size - match) / (float)size) > (percentage / 100.0);
        }

        /// <summary>
        /// Returns a string with init caps.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string InitCap(this string value)
        {
#if WINDOWS_PHONE
            // use other code, ToTileCase is not supported in windows phone.
            if (value == null)
                return null;
            if (value.Length == 0)
                return value;

            StringBuilder result = new StringBuilder(value);
            result[0] = char.ToUpper(result[0]);
            for (int i = 1; i < result.Length; ++i)
            {
                if (char.IsWhiteSpace(result[i - 1]))
                    result[i] = char.ToUpper(result[i]);
                else
                    result[i] = char.ToLower(result[i]);
            }
            return result.ToString();
#else
            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value.ToLower());
#endif
        }


        /// <summary>
        /// Returns the numeric part of the string for the beginning part of the string only.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string NumericPartFloat(this string value)
        {
            string ret_string = string.Empty;
            if (value != null && value.Length > 0)
            {
                StringBuilder numbers = new StringBuilder();
                for (int c = 1;c <= value.Length;c++)
                {
                    float result_never_used;
                    string value_tested = value.Substring(0, c);
                    if (float.TryParse(value_tested, out result_never_used))
                    {
                        ret_string = value_tested;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return ret_string;
        }

        /// <summary>
        /// Returns the numeric part of the string for the beginning part of the string only.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string NumericPartInt(this string value)
        {
            string ret_string = string.Empty;
            if (value != null && value.Length > 0)
            {
                StringBuilder numbers = new StringBuilder();
                for (int c = 1; c <= value.Length; c++)
                {
                    int result_never_used;
                    string value_tested = value.Substring(0, c);
                    if (int.TryParse(value_tested, out result_never_used))
                    {
                        ret_string = value_tested;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return ret_string;
        }

        /// <summary>
        /// Splists this string into parts with sizes given in the array.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="sizes"></param>
        /// <returns></returns>
        public static string[] SplitMultiple(this string value,int[] sizes)
        {
            string[] result = new string[sizes.Length];

            int position = 0;
            for (int i = 0; i < sizes.Length; i++)
            {
                result[i] = value.Substring(position, sizes[i]);

                position = position + sizes[i];
            }

            return result;
        }



        /// <summary>
        /// Returns the result of the ToString() method or an empty string
        /// when the given object is null.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToStringEmptyWhenNull(this object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            return obj.ToString();
        }
    }
}
