using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;

namespace Bfw.Common.Database
{
    /// <summary>
    /// Class to contain utility extensions to IDataReader interface.
    /// </summary>
    public static class IDataReaderExtensions
    {
        /// <summary>
        /// Reads all items from an <see cref="IDataReader"/> object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="reader">The reader from which to read objects.</param>
        /// <param name="mapper">The mapper useed to map records to objects of type <c>TEntity</c>.</param>
        /// <returns>The list of objects that was read.</returns>
        public static IList<TEntity> ReadAll<TEntity>(this IDataReader reader, Func<IDataReader, TEntity> mapper)
        {
            var entities = new List<TEntity>();

            try
            {
                while (reader.Read())
                {
                    entities.Add(mapper(reader));
                }
            }
            finally
            {
                reader.Close();
            }

            return entities;
        }
    }
}
