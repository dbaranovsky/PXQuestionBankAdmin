using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;


namespace Bfw.Common.Collections
{
    /// <summary>
    /// Provides basic list and IEnumerable manipulations.  Of note are the implementations of
    /// Filter, Map, and Reduce.
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Reduces a list of elements, of type T, to one value of type R.
        /// </summary>
        /// <typeparam name="T">Type of each element in list.</typeparam>
        /// <typeparam name="R">Type of the reduced value.</typeparam>
        /// <param name="list">List of elements of type T.</param>
        /// <param name="func">Function that accumulates elements from list into the resulting R value.</param>
        /// <param name="seed">Starting value for R.</param>
        /// <returns>Single value representing the reduction of list.</returns>
        public static R Reduce<T, R>(this IEnumerable<T> list, Func<T, R, R> func, R seed)
        {
            R accumulator = seed;

            foreach (T item in list)
            {
                accumulator = func(item, accumulator);
            }

            return accumulator;
        }

        /// <summary>
        /// Returns the default value if object is null, returns the object if its not null
        /// </summary>
        /// <typeparam name="T">Type of element</typeparam>
        /// <param name="possibleNullObject">Element that might be null</param>
        /// <param name="defaultValue">returned value is possibleNullObject is null</param>
        /// <returns></returns>
        public static T IfNull<T>(this T possibleNullObject, T defaultValue)
        {
            if (possibleNullObject == null)
                return defaultValue;
            return possibleNullObject;
        }

        /// <summary>
        /// Maps a list of items from type T to type R using the supplied function.
        /// </summary>
        /// <typeparam name="T">Type of the elements in list.</typeparam>
        /// <typeparam name="R">Type to which elements in list will be mapped using func.</typeparam>
        /// <param name="list">List whose elements are of type T.</param>
        /// <param name="func">Function that maps an element from list from type T to type R.</param>
        /// <returns>New enumerable containing all elements from list mapped from type T to type R.</returns>
        public static IEnumerable<R> Map<T, R>(this IEnumerable<T> list, Func<T, R> func)
        {
            foreach (T item in list)
            {
                yield return func(item);
            }
        }

        /// <summary>
        /// Filters any IEnumerable of type T using the supplied conditional function.
        /// </summary>
        /// <typeparam name="T">Type of the elements in list.</typeparam>
        /// <param name="list">List of elements of type T.</param>
        /// <param name="func">Function that determines whether an item from the list meets the 
        /// required conditions.</param>
        /// <returns>New enumerable that contains all items from list that met the conditions.</returns>
        public static IEnumerable<T> Filter<T>(this IEnumerable<T> list, Func<T, bool> func)
        {
            foreach (T item in list)
            {
                if (func(item))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Filters any IEnumerable of type T using the supplied conditional function.
        /// </summary>
        /// <typeparam name="T">Type of the elements in list.</typeparam>
        /// <param name="list">List of elements of type T.</param>
        /// <param name="func">Function that determines whether an item from the list meets the
        /// required conditions.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>New enumerable that contains all items from list that met the conditions.</returns>
        public static IEnumerable<T> Filter<T>(this IEnumerable<T> list, Func<T, bool> func, int limit)
        {
            int count = 0;
            foreach (T item in list)
            {
                if (count >= limit)
                {
                    yield break;
                }

                if (func(item))
                {
                    ++count;
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Finds the first match of type T using the supplied conditional function.
        /// </summary>
        /// <typeparam name="T">Type of the elements in list.</typeparam>
        /// <param name="list">List of elements of type T.</param>
        /// <param name="func">Function that determines whether an item from the list meets the
        /// required conditions.</param>
        /// <param name="nullValue">The null value.</param>
        /// <returns>New item of type T that matches the first condition.</returns>
        public static T Find<T>(IEnumerable<T> list, Func<T, bool> func, T nullValue)
        {
            foreach (T item in list)
            {
                if (func(item))
                {
                    return item;
                }
            }
            return nullValue;
        }

        /// <summary>
        /// Returns true if the given value is contained in the collection.
        /// </summary>
        /// <typeparam name="T">Collection element type.</typeparam>
        /// <param name="list">Collection to search</param>
        /// <param name="value">Value to find in the collection.</param>
        /// <param name="func">Function that implements the equivalence operation.</param>
        /// <returns><c>true</c> if value is contained in the collection, <c>false</c> otherwise.</returns>
        public static bool Contains<T>(this IEnumerable<T> list, T value, Func<T, T, bool> func)
        {
            return list.Contains<T>(value, new LambaEqualityComparer<T>(func));
        }

        /// <summary>
        /// Returns distinct list of values from the collection.
        /// </summary>
        /// <typeparam name="TEntity">Collection element type.</typeparam>
        /// <param name="list">Collection.</param>
        /// <param name="func">Function that implements the equivalence operation.</param>
        /// <returns>Distinct collection of elements from list.</returns>
        public static IEnumerable<TEntity> Distinct<TEntity>(this IEnumerable<TEntity> list, Func<TEntity, TEntity, bool> func)
        {
            return list.Distinct(new LambaEqualityComparer<TEntity>(func, x => x.GetHashCode()));
        }

        /// <summary>
        /// Determines if a collection is null or empty.
        /// </summary>
        /// <typeparam name="TEntity">Collection element type.</typeparam>
        /// <param name="list">Collection.</param>
        /// <returns><c>true</c> if the collection is null or contains zero elements, <c>false</c> otherwise.</returns>
        public static bool IsNullOrEmpty<TEntity>(this IEnumerable<TEntity> list)
        {
            if (list == null)
            {
                return true;
            }

            if (list.Count() == 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Folds all values in list to a single string, separating each element of list with delimiter. 
        /// In order to accomodate basic formating the caller can specify a getString function.
        /// </summary>
        /// <typeparam name="T">Type of the elements in list.</typeparam>
        /// <param name="list">Collection of elements of type T.</param>
        /// <param name="delimiter">String used to separate elements in list.</param>
        /// <param name="getString">Delegate that accepts an element of type T and returns a string representation of it.</param>
        /// <returns>String representing the fold of all elements from list.</returns>
        public static string Fold<T>(this IEnumerable<T> list, string delimiter, Func<T, string> getString)
        {
            // All elements are accumulated into a string builder via Map.
            Func<string, StringBuilder, StringBuilder> accumulator =
                delegate(string item, StringBuilder seed) { return seed.AppendFormat("{0}{1}", delimiter, item); };

            // We use Map and Reduce in order to fold the list's elements into a single string.
            StringBuilder sb = list.Map(getString).Reduce(accumulator, new StringBuilder());

            // Provided the string builder contains characters, we strip the first occurance of the delimiter.
            if (sb.Length > 0)
            {
                sb.Remove(0, delimiter.Length);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Folds all elements into a single string, assuming that each element provides a useful ToString implementation.
        /// </summary>
        /// <typeparam name="T">Collection element type.</typeparam>
        /// <param name="list">Collection.</param>
        /// <param name="delimiter">String used to separate elements.</param>
        /// <returns>String containing all elements of the collection separated by delimiter.</returns>
        public static string Fold<T>(this IEnumerable<T> list, string delimiter)
        {
            return list.Fold(delimiter, x => x.ToString());
        }

        /// <summary>
        /// Folds all elements into a single string, assuming that each element provides a useful ToString implementation and defaults
        /// the delimiter to the comma (,) character.
        /// </summary>
        /// <typeparam name="T">Collection element type.</typeparam>
        /// <param name="list">Collection.</param>
        /// <returns>String containing all elements from collection folded into a single string.</returns>
        public static string Fold<T>(this IEnumerable<T> list)
        {
            return list.Fold(",");
        }


        public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> source, int size)
        {
            if (source == null)
                throw new ArgumentNullException("list");

            if (size < 1)
                throw new ArgumentOutOfRangeException("size");

            int index = 1;
            IEnumerable<T> partition = source.Take(size).AsEnumerable();

            while (partition.Any())
            {
                yield return partition;
                partition = source.Skip(index++ * size).Take(size).AsEnumerable();
            }
        }
    }
}