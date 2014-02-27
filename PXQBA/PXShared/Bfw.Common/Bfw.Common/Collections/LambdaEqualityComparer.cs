using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Common.Collections
{
    /// <summary>
    /// Allows users to specify lambda functions in places where an IEqualityComparer is required instead of
    /// having to provide a new class.
    /// </summary>
    /// <typeparam name="T">Type of object being compared.</typeparam>
    public class LambaEqualityComparer<T> : IEqualityComparer<T>
    {
        /// <summary>
        /// The lambda function used to make the comparison.
        /// </summary>
        private Func<T, T, bool> _lambda = null;

        /// <summary>
        /// The lambda hash function
        /// </summary>
        private Func<T, int> _lambdaHash = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="LambaEqualityComparer&lt;T&gt;"/> class,
        /// using a default hash function that gets the hash code of the underlying object.
        /// </summary>
        /// <param name="lambda">The lambda.</param>
        public LambaEqualityComparer(Func<T, T, bool> lambda)
            : this(lambda, x => x.GetHashCode())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LambaEqualityComparer&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="lambda">The lambda.</param>
        /// <param name="lambdaHash">The lambda hash.</param>
        public LambaEqualityComparer(Func<T, T, bool> lambda, Func<T, int> lambdaHash)
        {
            _lambda = lambda;
            _lambdaHash = lambdaHash;
        }

        #region IEqualityComparer<T> Members

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type to compare.</param>
        /// <param name="y">The second object of type to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(T x, T y)
        {
            return _lambda(x, y);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.
        /// </exception>
        public int GetHashCode(T obj)
        {
            return _lambdaHash(obj);
        }

        #endregion
    }
}
