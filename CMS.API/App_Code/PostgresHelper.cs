using System;
using System.Collections;
using System.Data;

using Npgsql;

namespace CMS.API.App_Code
{
    public class PostgresHelper
    {
        #region private utility methods & constructors

        private PostgresHelper() { }

        /// <summary>
        /// This method is used to attach array of NpgsqlParameters to a NpsqlCommand.
        /// </summary>
        private static void AttachParameters(NpgsqlCommand command, NpgsqlParameter[] commandParameters)
        {
            foreach (NpgsqlParameter p in commandParameters)
            {
                if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
                {
                    p.Value = DBNull.Value;
                }
                command.Parameters.Add(p);
            }
        }

        /// <summary>
        /// This method assigns an array of values to an array of NpgsqlParameters.
        /// </summary>
        private static void AssignParameterValues(NpgsqlParameter[] commandParameters, object[] parameterValues)
        {
            if ((commandParameters == null) || (parameterValues == null))
            {
                return;
            }
            if (commandParameters.Length != parameterValues.Length)
            {
                throw new ArgumentException("Parameter count does not match Parameter Value count.");
            }
            // Iterate through the NpgsqlParameters, assigning the values from the corresponding position in the value array
            for (int i = 0, j = commandParameters.Length; i < j; i++)
            {
                commandParameters[i].Value = parameterValues[i];
            }
        }

        /// <summary>
        /// This method opens (if necessary) and assigns a connection, transaction, command type and parameters
        /// </summary>
        private static void PrepareCommand(NpgsqlCommand command, NpgsqlConnection connection, NpgsqlTransaction transaction,
            CommandType commandType, string commandText, NpgsqlParameter[] commandParameters)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            command.Connection = connection;
            command.CommandText = commandText;
            if (transaction != null)
            {
                command.Transaction = transaction;
            }
            command.CommandType = commandType;
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }

            return;
        }

        #endregion private utility methods & constructors

        #region ExecuteNonQuery

        /// <summary>
        /// Execute a NpgsqlCommand (that returns no resultset and takes no parameters) against the database specified in the connection string.
        /// </summary>
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connectionString, commandType, commandText, (NpgsqlParameter[])null);
        }

        /// <summary>
        /// Excute a NpgsqlCommand (that returns no resultset) against the database specified in the connection string using the provided parameters.
        /// </summary>
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params NpgsqlParameter[] commandParameters)
        {
            using (NpgsqlConnection cn = new NpgsqlConnection(connectionString))
            {
                cn.Open();
                return ExecuteNonQuery(cn, commandType, commandText, commandParameters);
            }
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns no resultset and takes no parameters) against the provided NpgsqlConnection.
        /// </summary>
        public static int ExecuteNonQuery(NpgsqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connection, commandType, commandText, (NpgsqlParameter[])null);
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns no resultset) against the specified NpgsqlConnection using the provided parameters.
        /// </summary>
        public static int ExecuteNonQuery(NpgsqlConnection connection, CommandType commandType, string commandText, params NpgsqlParameter[] commandParameters)
        {
            NpgsqlCommand cmd = new NpgsqlCommand();
            PrepareCommand(cmd, connection, (NpgsqlTransaction)null, commandType, commandText, commandParameters);

            int retval = cmd.ExecuteNonQuery();

            cmd.Parameters.Clear();
            return retval;
        }

        /// <summary>
        /// Execute a stored procedure via a NpgsqlCommand (that returns no resultset) against the specified NpgsqlConnection using the provided parameter values.
        /// </summary>
        public static int ExecuteNonQuery(NpgsqlConnection connection, string spName, params object[] parameterValues)
        {
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                NpgsqlParameter[] commandParameters = PostgresHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns no resultset and tasks no parameters) against the provided NpgsqlTransaction.
        /// </summary>
        public static int ExecuteNonQuery(NpgsqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(transaction, commandType, commandText, (NpgsqlParameter[])null);
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns no resultset) against the specified NpgsqlTransaction using the provided parameters.
        /// </summary>
        public static int ExecuteNonQuery(NpgsqlTransaction transaction, CommandType commandType, string commandText, params NpgsqlParameter[] commandParameters)
        {
            NpgsqlCommand cmd = new NpgsqlCommand();
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);

            int retval = cmd.ExecuteNonQuery();

            cmd.Parameters.Clear();
            return retval;
        }

        /// <summary>
        /// Execute a stored procedure via a NpgsqlCommand (that returns no resultset) against the specified NpgsqlTransaction using the provided parameter values.
        /// </summary>
        public static int ExecuteNonQuery(NpgsqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                NpgsqlParameter[] commandParameters = PostgresHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion ExecuteNonQuery

        #region ExecuteDataSet

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a resultset and takes no parameters) against the database specified in the connection string.
        /// </summary>
        public static DataSet ExecuteDataSet(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteDataSet(connectionString, commandType, commandText, (NpgsqlParameter[])null);
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a resultset) against the database specified in the connection string using the provided parameters.
        /// </summary>
        public static DataSet ExecuteDataSet(string connectionString, CommandType commandType, string commandText, params NpgsqlParameter[] commandParameters)
        {
            using (NpgsqlConnection cn = new NpgsqlConnection(connectionString))
            {
                cn.Open();
                return ExecuteDataSet(cn, commandType, commandText, commandParameters);
            }
        }

        /// <summary>
        /// Execute a stored procedure via a NpgsqlCommand (that returns a resultset) against the database specified in the connection string using the provided parameter values.
        /// </summary>
        public static DataSet ExecuteDataSet(string connectionString, string spName, params object[] parameterValues)
        {
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                NpgsqlParameter[] commandParameters = PostgresHelperParameterCache.GetSpParameterSet(connectionString, spName);
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteDataSet(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteDataSet(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a resultset and takes no parameters) against the provided NpgsqlConnection.
        /// </summary>
        public static DataSet ExecuteDataSet(NpgsqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteDataSet(connection, commandType, commandText, (NpgsqlParameter[])null);
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a resultset) against the specified NpgsqlConnection using the provided parameters.
        /// </summary>
        public static DataSet ExecuteDataSet(NpgsqlConnection connection, CommandType commandType, string commandText, params NpgsqlParameter[] commandParameters)
        {
            NpgsqlCommand cmd = new NpgsqlCommand();
            PrepareCommand(cmd, connection, (NpgsqlTransaction)null, commandType, commandText, commandParameters);

            NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            da.Fill(ds);

            cmd.Parameters.Clear();

            return ds;
        }

        /// <summary>
        /// Execute a stored procedure via a NpgsqlCommand (that returns a resultset) against the specified NpgsqlConnection using the provided parameter values.
        /// </summary>
        public static DataSet ExecuteDataSet(NpgsqlConnection connection, string spName, params object[] parameterValues)
        {
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                NpgsqlParameter[] commandParameters = PostgresHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteDataSet(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteDataSet(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a resultset and takes no parameters) against the provided NpgsqlTransaction.
        /// </summary>
        public static DataSet ExecuteDataSet(NpgsqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteDataSet(transaction, commandType, commandText, (NpgsqlParameter[])null);
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a resultset) against the specified NpgsqlTransaction using the provided parameters.
        /// </summary>
        public static DataSet ExecuteDataSet(NpgsqlTransaction transaction, CommandType commandType, string commandText, params NpgsqlParameter[] commandParameters)
        {
            NpgsqlCommand cmd = new NpgsqlCommand();
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);

            NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            da.Fill(ds);

            cmd.Parameters.Clear();

            return ds;
        }

        /// <summary>
        /// Execute a stored procedure via a NpgsqlCommand (that returns a resultset) against the specified NpgsqlTransaction using the provided parameter values.
        /// </summary>
        public static DataSet ExecuteDataSet(NpgsqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                NpgsqlParameter[] commandParameters = PostgresHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteDataSet(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteDataSet(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion ExecuteDataSet

        #region ExecuteReader

        private enum NpgsqlConnectionOwnership
        {
            Internal,
            External
        }

        /// <summary>
        /// Create and prepare a NpgsqlCommand, and call ExecuteReader with the appropriate CommandBehavior.
        /// </summary>
        private static NpgsqlDataReader ExecuteReader(NpgsqlConnection connection, NpgsqlTransaction transaction, CommandType commandType,
            string commandText, NpgsqlParameter[] commandParameters, NpgsqlConnectionOwnership connectionOwnership)
        {
            NpgsqlCommand cmd = new NpgsqlCommand();
            PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters);

            NpgsqlDataReader dr;

            if (connectionOwnership == NpgsqlConnectionOwnership.External)
            {
                dr = cmd.ExecuteReader();
            }
            else
            {
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }

            cmd.Parameters.Clear();

            return dr;
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a resultset and takes no parameters) against the database specified in the connection string.
        /// </summary>
        public static NpgsqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteReader(connectionString, commandType, commandText, (NpgsqlParameter[])null);
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a resultset) against the database specified in the connection string using the provided parameters.
        /// </summary>
        public static NpgsqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params NpgsqlParameter[] commandParameters)
        {
            NpgsqlConnection cn = new NpgsqlConnection(connectionString);
            cn.Open();

            try
            {
                return ExecuteReader(cn, null, commandType, commandText, commandParameters, NpgsqlConnectionOwnership.Internal);
            }
            catch
            {
                cn.Close();
                throw;
            }
        }

        /// <summary>
        /// Execute a stored procedure via a NpgsqlCommand (that returns a resultset) against the database specified in the connection string using the provided parameter values.
        /// </summary>
        public static NpgsqlDataReader ExecuteReader(string connectionString, string spName, params object[] parameterValues)
        {
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                NpgsqlParameter[] commandParameters = PostgresHelperParameterCache.GetSpParameterSet(connectionString, spName);
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
            }
        }


        /// <summary>
        /// Execute a NpgsqlCommand (that returns a resultset and takes no parameters) against the provided NpgsqlConnection.
        /// </summary>
        public static NpgsqlDataReader ExecuteReader(NpgsqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteReader(connection, commandType, commandText, (NpgsqlParameter[])null);
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a resultset) against the specified NpgsqlConnection using the provided parameters.
        /// </summary>
        public static NpgsqlDataReader ExecuteReader(NpgsqlConnection connection, CommandType commandType, string commandText, params NpgsqlParameter[] commandParameters)
        {
            return ExecuteReader(connection, (NpgsqlTransaction)null, commandType, commandText, commandParameters, NpgsqlConnectionOwnership.External);
        }

        /// <summary>
        /// Execute a stored procedure via a NpgsqlCommand (that returns a resultset) against the specified NpgsqlConnection using the provided parameter values.
        /// </summary>
        public static NpgsqlDataReader ExecuteReader(NpgsqlConnection connection, string spName, params object[] parameterValues)
        {
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                NpgsqlParameter[] commandParameters = PostgresHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteReader(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteReader(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a resultset and takes no parameters) against the provided NpgsqlTransaction.
        /// </summary>
        public static NpgsqlDataReader ExecuteReader(NpgsqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteReader(transaction, commandType, commandText, (NpgsqlParameter[])null);
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a resultset) against the specified NpgsqlTransaction using the provided parameters.
        /// </summary>
        public static NpgsqlDataReader ExecuteReader(NpgsqlTransaction transaction, CommandType commandType, string commandText, params NpgsqlParameter[] commandParameters)
        {
            return ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, NpgsqlConnectionOwnership.External);
        }

        /// <summary>
        /// Execute a stored procedure via a NpgsqlCommand (that returns a resultset) against the specified NpgsqlTransaction using the provided parameter values.
        /// </summary>
        public static NpgsqlDataReader ExecuteReader(NpgsqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                NpgsqlParameter[] commandParameters = PostgresHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteReader(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion ExecuteReader

        #region ExecuteScalar

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a 1x1 resultset and takes no parameters) against the database specified in the connection string.
        /// </summary>
        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteScalar(connectionString, commandType, commandText, (NpgsqlParameter[])null);
        }


        /// <summary>
        /// Execute a NpgsqlCommand (that returns a 1x1 resultset) against the database specified in the connection string using the provided parameters.
        /// </summary>
        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params NpgsqlParameter[] commandParameters)
        {
            using (NpgsqlConnection cn = new NpgsqlConnection(connectionString))
            {
                cn.Open();
                return ExecuteScalar(cn, commandType, commandText, commandParameters);
            }
        }

        /// <summary>
        /// Execute a stored procedure via a NpgsqlCommand (that returns a 1x1 resultset) against the database specified in the connection string using the provided parameter values.
        /// </summary>
        public static object ExecuteScalar(string connectionString, string spName, params object[] parameterValues)
        {
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                NpgsqlParameter[] commandParameters = PostgresHelperParameterCache.GetSpParameterSet(connectionString, spName);
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a 1x1 resultset and takes no parameters) against the provided NpgsqlConnection.
        /// </summary>
        public static object ExecuteScalar(NpgsqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteScalar(connection, commandType, commandText, (NpgsqlParameter[])null);
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a 1x1 resultset) against the specified NpgsqlConnection using the provided parameters.
        /// </summary>
        public static object ExecuteScalar(NpgsqlConnection connection, CommandType commandType, string commandText, params NpgsqlParameter[] commandParameters)
        {
            NpgsqlCommand cmd = new NpgsqlCommand();
            PrepareCommand(cmd, connection, (NpgsqlTransaction)null, commandType, commandText, commandParameters);

            object retval = cmd.ExecuteScalar();

            cmd.Parameters.Clear();
            return retval;
        }

        /// <summary>
        /// Execute a stored procedure via a NpgsqlCommand (that returns a 1x1 resultset) against the specified NpgsqlConnection using the provided parameter values.
        /// </summary>
        public static object ExecuteScalar(NpgsqlConnection connection, string spName, params object[] parameterValues)
        {
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                NpgsqlParameter[] commandParameters = PostgresHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteScalar(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a 1x1 resultset and takes no parameters) against the provided NpgsqlTransaction.
        /// </summary>
        public static object ExecuteScalar(NpgsqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteScalar(transaction, commandType, commandText, (NpgsqlParameter[])null);
        }

        /// <summary>
        /// Execute a NpgsqlCommand (that returns a 1x1 resultset) against the specified NpgsqlTransaction using the provided parameters.
        /// </summary>
        public static object ExecuteScalar(NpgsqlTransaction transaction, CommandType commandType, string commandText, params NpgsqlParameter[] commandParameters)
        {
            NpgsqlCommand cmd = new NpgsqlCommand();
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);

            object retval = cmd.ExecuteScalar();

            cmd.Parameters.Clear();
            return retval;
        }

        /// <summary>
        /// Execute a stored procedure via a NpgsqlCommand (that returns a 1x1 resultset) against the specified NpgsqlTransaction using the provided parameter values.
        /// </summary>
        public static object ExecuteScalar(NpgsqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                NpgsqlParameter[] commandParameters = PostgresHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion ExecuteScalar
    }

    public sealed class PostgresHelperParameterCache
    {
        #region private methods, variables, and constructors

        private PostgresHelperParameterCache() { }

        private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// resolves at run time the appropriate set of NpgsqlParameters for a stored procedure
        /// </summary>
        private static NpgsqlParameter[] DiscoverSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
        {
            using (NpgsqlConnection cn = new NpgsqlConnection(connectionString))
            using (NpgsqlCommand cmd = new NpgsqlCommand(spName, cn))
            {
                cn.Open();
                cmd.CommandType = CommandType.StoredProcedure;

                NpgsqlCommandBuilder.DeriveParameters(cmd);

                if (!includeReturnValueParameter && cmd.Parameters.Count > 0)
                {
                    cmd.Parameters.RemoveAt(0);
                }

                NpgsqlParameter[] discoveredParameters = new NpgsqlParameter[cmd.Parameters.Count];

                cmd.Parameters.CopyTo(discoveredParameters, 0);

                return discoveredParameters;
            }
        }

        private static NpgsqlParameter[] CloneParameters(NpgsqlParameter[] originalParameters)
        {
            NpgsqlParameter[] clonedParameters = new NpgsqlParameter[originalParameters.Length];

            for (int i = 0, j = originalParameters.Length; i < j; i++)
            {
                clonedParameters[i] = (NpgsqlParameter)((ICloneable)originalParameters[i]).Clone();
            }

            return clonedParameters;
        }

        #endregion private methods, variables, and constructors

        #region caching functions

        /// <summary>
        /// add parameter array to the cache
        /// </summary>
        public static void CacheParameterSet(string connectionString, string commandText, params NpgsqlParameter[] commandParameters)
        {
            string hashKey = connectionString + ":" + commandText;
            paramCache[hashKey] = commandParameters;
        }

        /// <summary>
        /// retrieve parameter array from the cache
        /// </summary>
        public static NpgsqlParameter[] GetCachedParameterSet(string connectionString, string commandText)
        {
            string hashKey = connectionString + ":" + commandText;
            NpgsqlParameter[] cachedParameters = paramCache[hashKey] as NpgsqlParameter[];

            if (cachedParameters == null)
            {
                return null;
            }
            else
            {
                return CloneParameters(cachedParameters);
            }
        }

        #endregion caching functions

        #region Parameter Discovery Functions

        /// <summary>
        /// Retrieves the set of NpgsqlParameters appropriate for the stored procedure
        /// </summary>
        public static NpgsqlParameter[] GetSpParameterSet(string connectionString, string spName)
        {
            return GetSpParameterSet(connectionString, spName, false);
        }

        /// <summary>
        /// Retrieves the set of NpgsqlParameters appropriate for the stored procedure
        /// </summary>
        public static NpgsqlParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
        {
            string hashKey = connectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");

            NpgsqlParameter[] cachedParameters;

            cachedParameters = (NpgsqlParameter[])paramCache[hashKey];

            if (cachedParameters == null)
            {
                cachedParameters = (NpgsqlParameter[])(paramCache[hashKey] = DiscoverSpParameterSet(connectionString, spName, includeReturnValueParameter));
            }

            return CloneParameters(cachedParameters);
        }

        #endregion Parameter Discovery Functions
    }
}