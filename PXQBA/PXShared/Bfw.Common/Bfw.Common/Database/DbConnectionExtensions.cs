using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;

namespace Bfw.Common.Database
{
    /// <summary>
    /// Extensions to the DbConnection class.
    /// <see cref="System.Data.Common.DbConnection"/>
    /// </summary>
    public static class DbConnectionExtensions
    {
        /// <summary>
        /// Maximum allowed size for string parameters.
        /// </summary>
        private const int MAX_STRING_SIZE = 4000;

        /// <summary>
        /// Checks state of connection.
        /// </summary>
        /// <param name="connection">Connection to check.</param>
        /// <returns><c>true</c> if connection is null or its state is closed, <c>false</c> otherwise.</returns>
        public static bool IsNullOrClosed(this DbConnection connection)
        {
            return (connection == null || connection.State == ConnectionState.Closed);
        }

        /// <summary>
        /// Checks state of connection.
        /// </summary>
        /// <param name="connection">Connection to check</param>
        /// <returns><c>true</c> if connection's state is open, <c>false</c> otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">if connection is null.</exception>
        public static bool IsOpen(this DbConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection", "Given connection is null");
            }

            return connection.State == ConnectionState.Open;
        }

        /// <summary>
        /// Creates a DbCommand from the connection using the given SQL and arguments.
        /// </summary>
        /// <param name="factory">The factory used to generate the command.</param>
        /// <param name="commandText">SQL to run.</param>
        /// <param name="args">Arguments to the SQL statment.</param>
        /// <returns>
        /// New command to run against connection.
        /// </returns>
        public static DbCommand CreateCommand(this DbProviderFactory factory, string commandText, params object[] args)
        {
            var cmd = factory.CreateCommand();

            cmd.CommandText = commandText;

            for (var a = 0; a < args.Length; ++a)
            {
                var param = cmd.CreateParameter();
                var arg = args[a];
                param.ParameterName = string.Format("@{0}", a);

                if (arg == null)
                {
                    param.Value = DBNull.Value;
                }
                else
                {
                    if (arg.GetType() == typeof(Guid))
                    {
                        // Not all RDBMSes support GUIDs.
                        param.DbType = DbType.String;
                        param.Value = arg.ToString();
                        param.Size = MAX_STRING_SIZE;
                    }
                    else
                    {
                        param.Value = arg;
                    }
                    // PX-2749 - PX Activation Confirmation Page Requirement [Email length is greater than 4000 characters and this validation is blocker]
                    /*if (arg.GetType() == typeof(string))
                    {
                        param.Size = MAX_STRING_SIZE;
                    }*/
                }

                cmd.Parameters.Add(param);
            }

            return cmd;
        }
    }
}
