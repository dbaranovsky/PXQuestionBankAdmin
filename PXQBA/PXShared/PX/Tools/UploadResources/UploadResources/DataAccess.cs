using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UploadResources.Exception;

namespace UploadResources.DataAccess
{

    public static class DataAccess
    {


        private static string GenerateParameterList(IEnumerable<SqlParameter> spParams)
        {
            var strParams = "";
            if (spParams != null)
            {
                foreach (var sqlParameter in spParams)
                {
                    if (sqlParameter.SqlDbType == SqlDbType.Binary || sqlParameter.SqlDbType == SqlDbType.Image ||
                        sqlParameter.SqlDbType == SqlDbType.Structured || sqlParameter.SqlDbType == SqlDbType.VarBinary ||
                        sqlParameter.SqlDbType == SqlDbType.Variant) continue;
                    strParams += sqlParameter.ParameterName + "=" + sqlParameter.Value + ",";
                }
            }
            return strParams;
        }


        private static SqlCommand PrepareCommand(ConnectionTokenWrapperBase connectionTokenWrapper,
                                                 string StoredProcedure, IEnumerable<SqlParameter> spParams)
        {
            var sqlCommand = new SqlCommand(StoredProcedure)
            {
                CommandType = CommandType.StoredProcedure,
                Connection = ((ConnectionTokenWrapper)connectionTokenWrapper).SqlConnection,
                Transaction = ((ConnectionTokenWrapper)connectionTokenWrapper).SqlTransaction,
                CommandTimeout = 6000
            };

            if (spParams != null)
            {
                foreach (var param in spParams)
                    sqlCommand.Parameters.Add(param);
            }

            return sqlCommand;
        }

      
        public static DataSet ExecuteDataSet(ConnectionTokenWrapperBase connectionTokenWrapper, string StoredProcedure,
                                             IEnumerable<SqlParameter> spParams)
        {
            try
            {
                var ds = new DataSet();
                var sqlDataAdapter =
                    new SqlDataAdapter(PrepareCommand(connectionTokenWrapper, StoredProcedure, spParams));
                sqlDataAdapter.Fill(ds);
                return ds;
            }
            catch (System.Exception ex)
            {
                var dalException = new DALException("DataAccess.ExecuteDataSet", ex);
                dalException.Data.Add("SPName", StoredProcedure);
                dalException.Data.Add("SPParams", GenerateParameterList(spParams));
                throw dalException;
            }
        }


        public static int ExecuteNonQuery(ConnectionTokenWrapperBase connectionTokenWrapper, string StoredProcedure,
                                          IEnumerable<SqlParameter> spParams)
        {
            try
            {
                return PrepareCommand(connectionTokenWrapper, StoredProcedure, spParams).ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                var dalException = new DALException("DataAccess.ExcecuteNonQuery", ex);
                dalException.Data.Add("SPName", StoredProcedure);
                dalException.Data.Add("SPParams", GenerateParameterList(spParams));
                throw dalException;
            }
        }

        public static SqlDataReader ExecuteReader(ConnectionTokenWrapperBase connectionTokenWrapper, string StoredProcedure, IEnumerable<SqlParameter> spParams)
        {
            try
            {
                if (!((ConnectionTokenWrapper)connectionTokenWrapper).KeepConnectionAlive)
                    return
                        PrepareCommand(connectionTokenWrapper, StoredProcedure, spParams).ExecuteReader(
                            CommandBehavior.CloseConnection);
                else
                    return PrepareCommand(connectionTokenWrapper, StoredProcedure, spParams).ExecuteReader();
            }
            catch (System.Exception ex)
            {
                var dalException = new DALException("DataAccess.ExecuteReader", ex);
                dalException.Data.Add("SPName", StoredProcedure);
                dalException.Data.Add("SPParams", GenerateParameterList(spParams));
                throw dalException;
            }
        }

    
        public static object ExecuteScaler(ConnectionTokenWrapperBase connectionTokenWrapper, string StoredProcedure,
                                           IEnumerable<SqlParameter> spParams)
        {
            try
            {
                return PrepareCommand(connectionTokenWrapper, StoredProcedure, spParams).ExecuteScalar();
            }
            catch (System.Exception ex)
            {
                var dalException = new DALException("DataAccess.ExecuteScaler", ex);
                dalException.Data.Add("SPName", StoredProcedure);
                dalException.Data.Add("SPParams", GenerateParameterList(spParams));
                throw dalException;
            }
        }
    }
}