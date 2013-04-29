using System;

namespace OsmSharp.UI
{
    /// <summary>
    /// Enum helper class for windows phone.
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// Tries to parse an enum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumText"></param>
        /// <param name="enumValue"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static bool TryParse<T>(string enumText, bool ignoreCase, out T enumValue)
            where T : struct
        {
            enumValue = default(T);
            if (Enum.IsDefined(typeof(T), enumText))
            {
                enumValue = (T)Enum.Parse(typeof(T), enumText, false);
                return true;
            }
            return false;
        }
    }
}