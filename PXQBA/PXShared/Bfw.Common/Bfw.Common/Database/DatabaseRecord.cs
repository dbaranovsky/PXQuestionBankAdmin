using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Common.Database
{
    /// <summary>
    /// Class to represent a record in a database table.
    /// </summary>
    public class DatabaseRecord
    {
        /// <summary>
        /// Hash of column name to data object.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        private IDictionary<string, object> Columns { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseRecord"/> class, with an empty column set.
        /// </summary>
        public DatabaseRecord()
        {
            Columns = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> for specified column name.
        /// </summary>
        public object this[string column]
        {
            get
            {
                return Columns[column];
            }
            set
            {
                Columns[column] = value;
            }
        }

        /// <summary>
        /// Returns the data value of a given column as an integer.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <returns>The data in the column as an integer.</returns>
        public int Int32(string column) { return Convert.ToInt32(Columns[column]); }

        /// <summary>
        /// Returns the data value of a given column as a long integer.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <returns>The data in the column as a long integer.</returns>
        public long Int64(string column) { return Convert.ToInt64(Columns[column]); }

        /// <summary>
        /// Returns the data value of a given column as a string.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <returns>The data in the column as a string.</returns>
        public string String(string column) { return Convert.ToString(Columns[column]); }

        /// <summary>
        /// Returns the data value of a given column as a bool.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <returns>The data in the column as a bool.</returns>
        public bool Bool(string column) { return Convert.ToBoolean(Columns[column]); }

        /// <summary>
        /// Returns the data value of a given column as a datetime.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <returns>The data in the column as a datetime.</returns>
        public DateTime DateTime(string column) { return Convert.ToDateTime(Columns[column]); }

        /// <summary>
        /// Return true or false indicating whether the column value is dbnull
        /// </summary>
        /// <param name="column">column name</param>
        /// <returns>is db null</returns>
        public bool IsDBNull(string column) { return Columns[column] == DBNull.Value; }
    }
}
