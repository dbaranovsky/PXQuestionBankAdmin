using System;

namespace Bfw.Common.Database
{
    public interface IDatabaseManager
    {
        void EndSession();

        int ExecuteNonQuery(System.Data.Common.DbCommand cmd);

        int ExecuteNonQuery(System.Data.Common.DbConnection connection, string sql, params object[] args);

        int ExecuteNonQuery(string sql, params object[] args);

        object ExecuteScalar(System.Data.Common.DbCommand cmd);

        object ExecuteScalar(System.Data.Common.DbConnection connection, string sql, params object[] args);

        object ExecuteScalar(string sql, params object[] args);

        void ConfigureConnection(string connectionStringName);

        System.Data.Common.DbConnection OpenConnection();

        System.Collections.Generic.IEnumerable<DatabaseRecord> Query(System.Data.Common.DbCommand cmd);

        System.Collections.Generic.IEnumerable<DatabaseRecord> Query(System.Data.Common.DbConnection connection, string sql, params object[] args);

        System.Collections.Generic.IEnumerable<DatabaseRecord> Query(string sql, params object[] args);

        void StartSession();
    }
}
