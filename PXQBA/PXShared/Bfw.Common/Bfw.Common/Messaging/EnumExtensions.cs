using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Bfw.Common
{
    /// <summary>
    /// Provides convenient helper methods for enumerations.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Returns an IEnumerable for all the name/value pairs in an enumeration.
        /// </summary>
        /// <param name="enumType">Type of the enum.</param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, object>> ToIEnumerable(this Enum enumType)
        {
            var type = enumType.GetType();
            foreach (var val in Enum.GetValues(type))
            {
                yield return new KeyValuePair<string, object>(Enum.GetName(type, val), val);
            }
        }

        /// <summary>
        /// Returns the description of the Enum if its specified on Enum decleration.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static string GetDescription(this Enum item)
        {
            var attributes =
                item.GetType().GetField(item.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false) as
                DescriptionAttribute[];
            return (attributes != null && attributes.Length > 0) ? attributes[0].Description : item.ToString();
        }

        /// <summary>
        /// Parses the string and returns native Enum value.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="item">The enum.</param>
        /// <param name="value">The value to parse.</param>
        /// <param name="ignoreCase">Whether to ignore case when parsing.</param>
        /// <param name="result">The result of the parse, cast to type T.</param>
        /// <returns><c>true</c> if the parse was successful, <c>false</c> otherwise.</returns>
        public static bool TryParse<T>(this T item, string value, bool ignoreCase, out T result)
        {
            result = item;
            if (string.IsNullOrEmpty(value)) return false;

            value = value.Replace(' ', '_');

            if (Enum.IsDefined(typeof(T), value))
            {
                result = (T)Enum.Parse(typeof(T), value);
                return true;
            }
            foreach (var name in Enum.GetNames(typeof(T)))
            {
                var compareMode = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                if (!name.Equals(value, compareMode)) continue;
                result = (T)Enum.Parse(typeof(T), value, ignoreCase);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Parses the string and returns native Enum value
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="item">The enum.</param>
        /// <param name="value">The value to parse.</param>
        /// <param name="result">The result of the parse, cast to type T.</param>
        /// <returns><c>true</c> if the parse was successful, <c>false</c> otherwise.</returns>
        public static bool TryParse<T>(this T item, string value, out T result)
        {
            return item.TryParse(value, false, out result);
        }

        /// <summary>
        /// Parses the string and returns native Enum value
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="item">The enum.</param>
        /// <param name="value">The value to parse.</param>
        /// <param name="result">The result of the parse, cast to type T.</param>
        /// <returns><c>true</c> if the parse was successful, <c>false</c> otherwise.</returns>
        public static bool TryParse<T>(this T item, object value, out T result)
        {
            result = item;
            var enumValue = Convert.ChangeType(value, Enum.GetUnderlyingType(typeof(T)), CultureInfo.InvariantCulture);
            if (Enum.IsDefined(typeof(T), enumValue))
            {
                result = (T)Enum.ToObject(typeof(T), enumValue);
                return true;
            }
            return false;
        }
    }
}