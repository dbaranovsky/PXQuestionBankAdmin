using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Macmillan.PXQBA.Common.Helpers
{
    /// <summary>
    /// Helper to work with enums
    /// </summary>
    public class EnumHelper
    {
        /// <summary>
        /// Gets enum values into multiselection
        /// </summary>
        /// <param name="enumType">Type of the enum</param>
        /// <returns>Multiselection model</returns>
        public static IEnumerable<KeyValuePair<string, string>> GetEnumValues(Type enumType)
        {
            if (enumType.BaseType != typeof(Enum))
                throw new ArgumentException("T must be of type System.Enum");

            Type unde = Enum.GetUnderlyingType(enumType);

            IEnumerable<Enum> enumItems = Enum.GetValues(enumType).Cast<Enum>();

            IEnumerable<KeyValuePair<string, string>> items = enumItems.Select(item =>
                new KeyValuePair<string, string>(
                    Convert.ChangeType(item, unde).ToString(),
                    GetEnumDescription(item)));

            return items;
        }

        /// <summary>
        /// Returns description of the enum value
        /// </summary>
        /// <param name="value">Enum value</param>
        /// <returns>Enum description</returns>
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            return value.ToString();
        }

        /// <summary>
        /// Get Enum item by description
        /// </summary>
        /// <param name="enumType">Enum type</param>
        /// <param name="description">Enum description</param>
        /// <returns>Enum item</returns>
        public static object GetItemByDescription(Type enumType, string description)
        {
            var enumItems = GetEnumValues(enumType);
            return Enum.Parse(enumType, enumItems.First(item => item.Value.ToUpper() == description.ToUpper()).Key);
        }

        /// <summary>
        /// Checks if value equals to enum description
        /// </summary>
        /// <param name="entry">Entry to check</param>
        /// <param name="value">Enum value to check with</param>
        /// <returns></returns>
        public static bool Equals(string entry, Enum value)
        {
            return entry.ToUpper() == GetEnumDescription(value).ToUpper();
        }

        /// <summary>
        /// Parses string into Enum type
        /// </summary>
        /// <typeparam name="T">enum type</typeparam>
        /// <param name="value">Value to parse</param>
        /// <returns></returns>
        public static T Parse<T>(string value)
        {
            return (T) Enum.Parse(typeof (T), value);
        }
    }
}