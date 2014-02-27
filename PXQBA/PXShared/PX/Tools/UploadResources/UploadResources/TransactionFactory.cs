using System.Configuration;
using System.Data.SqlClient;
using UploadResources.Exception;
using System.Configuration;

namespace UploadResources.DataAccess
{
    public class ConnectionTokenWrapper : ConnectionTokenWrapperBase
    {
        public ConnectionTokenWrapper()
        {
        }

        public ConnectionTokenWrapper(bool TransactionMode)
            : base(TransactionMode)
        {
        }

        public SqlConnection SqlConnection
        {
            get { return sqlConnection; }
        }

        public SqlTransaction SqlTransaction
        {
            get { return sqlTransaction; }
        }

        public bool KeepConnectionAlive
        {
            get { return keepConnnectionAlive; }
            set { keepConnnectionAlive = value; }
        }

        /// <summary>
        /// Commit the current transaction
        /// </summary>
        public void Commit()
        {
            if (transactionMode)
                sqlTransaction.Commit();
        }

        /// <summary>
        /// Rollback the current trasaction
        /// </summary>
        public void Rollback()
        {
            if (transactionMode)
                sqlTransaction.Rollback();
        }

        /// <summary>
        /// Close and dispose the connection and transaction objects
        /// </summary>
        public void Dispose()
        {
            if (sqlConnection != null)
            {
                sqlConnection.Close();
                sqlConnection.Dispose();
            }

            if (transactionMode)
                if (sqlTransaction != null) sqlTransaction.Dispose();
        }
    }

    /// <summary>
    /// Base class for Connection Wrapper
    /// </summary>
    public class ConnectionTokenWrapperBase
    {
        protected SqlConnection sqlConnection;
        protected SqlTransaction sqlTransaction;
        protected bool keepConnnectionAlive;
        protected bool transactionMode;


        /// <summary>
        /// Default Constructor
        /// </summary>
        public ConnectionTokenWrapperBase()
        {
            CreateConnection();
        }

        public ConnectionTokenWrapperBase(bool TransactionMode)
        {
            transactionMode = TransactionMode;
            CreateConnection();
        }

        /// <summary>
        /// Connect to the database and initate a transaction if required
        /// </summary>
        protected void CreateConnection()
        {

            string ConnStr = "";
            try
            {
                ConnStr = ConfigurationManager.AppSettings.Get("Conn"); 
                sqlConnection = new SqlConnection(ConnStr);
                sqlConnection.Open();
                if (transactionMode)
                    sqlTransaction = sqlConnection.BeginTransaction();
            }
            catch (System.Exception ex)
            {
                var dalException = new DALException("CreateConnection", ex);
                dalException.Data.Add("ConnectionString", ConnStr);
                throw new DALException("CreateConnection", dalException);
            }
        }
    }

    /// <summary>
    /// TransactionFactory class provides all methods for transaction handling.
    /// </summary>
    public static class TransactionFactory
    {
        /// <summary>
        /// Create a connection and returns a ConnectionWrapper as a property of DC
        /// </summary>
        /// <param name="dc"></param>
        public static void ConstructToken(BaseDC dc)
        {
            dc.ConnectionWrapper = new ConnectionTokenWrapper();
            ((ConnectionTokenWrapper)dc.ConnectionWrapper).KeepConnectionAlive = false;
            
        }

        /// <summary>
        /// Create a connection with transaction mode and returns a ConnectionWrapper as a property of DC
        /// </summary>
        /// <param name="dc"></param>
        public static void ConstructTransactionToken(BaseDC dc)
        {
            dc.ConnectionWrapper = new ConnectionTokenWrapper(true);
            ((ConnectionTokenWrapper)dc.ConnectionWrapper).KeepConnectionAlive = true;
            
        }


        /// <summary>
        /// Commit the Transaction associated with the ConnectionWrapper with the passed in DC.
        /// </summary>
        /// <param name="dc">DataContainer</param>
        public static void Commit(BaseDC dc)
        {
            ((ConnectionTokenWrapper)dc.ConnectionWrapper).Commit();
        }

        /// <summary>
        /// Rollback the Transaction associated with the ConnectionWrapper with the passed in DC.
        /// </summary>
        /// <param name="dc">DataContainer</param>
        public static void Rollback(BaseDC dc)
        {
            ((ConnectionTokenWrapper)dc.ConnectionWrapper).Rollback();
        }


        /// <summary>
        /// Dispose the Connection associated with the ConnectionWrapper with the passed in DC.
        /// </summary>
        /// <param name="dc">DataContainer</param>
        public static void Dispose(BaseDC dc)
        {
            if (dc.ConnectionWrapper != null) ((ConnectionTokenWrapper)dc.ConnectionWrapper).Dispose();
        }
    }
}
