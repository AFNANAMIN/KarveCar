﻿namespace SqlLayer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using iAnywhere.Data.SQLAnywhere;
   

    namespace DataAccessLayer
    {

        public class OleDbQueryExecutor 
        {
            private SAConnection _connection;
            private object asyncBatchLock = new object();
            // this is simply the engine for cloning a table
            private object cloneTableLock = new object();
            //private SADataAdapter _adapter;
            //private SACommandBuilder  _builder;
            private object asyncLock2 = new object();
            private SACommand _command;
           
            private SATransaction _transaction;
            private string _connectionString;
            private ConnectionState _currentState;
            private object asyncLoadLock = new object();
            private object dataAdapterLock = new object();
            private string _defaultConnectionString = "EngineName=DBRENT_NET16;DataBaseName=DBRENT_NET16;Uid=cv;Pwd=1929;Host=172.26.0.45";

            public string ConnectionString
            {
                get { return _connectionString; }
                set { _connectionString = value; _connection = new SAConnection(_connectionString); }
            }
            public IDbConnection CurrentConnection
            {
                get { return _connection; }
            }
            public OleDbQueryExecutor()
            {
                _connectionString = _defaultConnectionString;
            }
            public OleDbQueryExecutor(string connectionString)
            {
                _connectionString = connectionString;
                _connection = new SAConnection(_connectionString);
            }
            public void BeginTransaction()
            {
                if (_currentState != ConnectionState.Open) Open();
                _transaction = _connection.BeginTransaction();
            }

            public void Commit()
            {
                if (_transaction != null)
                {
                    _transaction.Commit();
                }
                if (_currentState == ConnectionState.Open) Close();

            }

            public void Rollback()
            {
                if (_transaction != null)
                {
                    _transaction.Rollback();
                }
                if (_currentState == ConnectionState.Open) Close();
            }

            public bool Open()
            {

                try
                {
                    if (_currentState != ConnectionState.Open)
                    {
                        _connection.Open();
                        _currentState = _connection.State;
                    }
                }
                catch (System.Exception e)
                {
                    return false;
                }
                return true;

            }

            public bool Close()
            {
                try
                {
                    if (_currentState != ConnectionState.Closed)
                    {
                        _connection.Close();
                        _currentState = _connection.State;
                    }
                }
                catch (System.Exception e)
                {
                 
                    return false;
                }
                return true;
            }

            public DataSet LoadDataSet(string sqlQuery)
            {

                DataSet dts = new DataSet();
                try
                {
                    if (_transaction == null)
                    {
                        Open();
                        _command = new SACommand(sqlQuery, _connection);
                    }
                    else
                    {
                        _command = new SACommand(sqlQuery, _connection, _transaction);
                    }
                    SADataAdapter dta = new SADataAdapter(_command);

                    dta.FillSchema(dts, SchemaType.Source);
                    dta.Fill(dts);

                    if (_transaction == null)
                    {
                        Close();
                    }
                }
                catch (System.Exception e)
                {
                    if (_transaction != null)
                    {
                        _transaction.Rollback();
                    }
                    string msg = "Loading dataset error :" + e.Message;
                    throw new System.Exception(msg);
                }
                finally
                {
                    Close();
                }
                return dts;

            }

          
            /// <summary>
            /// Create a OdbcCommand from a transaction 
            /// </summary>
            /// <param name="sqlString"></param>
            /// <returns>An OdbcCommand</returns>
            private SACommand CommandTransaction(string sqlString)
            {
                SACommand cmd = null;
                if (_transaction == null)
                {
                    Open();
                    cmd = new SACommand(sqlString, _connection);
                }
                else
                {
                    cmd = new SACommand(sqlString, _connection, _transaction);
                }
                return cmd;
            }
            /// <summary>
            /// Close a connection if a transaction is not active.
            /// </summary>
            private void CloseInTransaction()
            {
                if (_transaction != null)
                {
                    Close();
                }
            }
            /// <summary>
            /// Calls a stored procedure if needed.
            /// </summary>
            /// <param name="statementProcedureName">Stored procedure name</param>
            /// <param name="statementProcList">List of parameters for a stored procedure</param>
            public void CallStoredProcedure(string statementProcedureName, IList<string> statementProcList)
            {
                string sqlString = "";
                string sqlParams = "";

                foreach (string param in statementProcList)
                {
                    if (!string.IsNullOrEmpty(sqlString))
                    {
                        sqlString = sqlString + ",";
                    }
                    sqlParams = sqlParams + "'" + param + "'";
                }
                sqlString = "call " + statementProcedureName + " (" + sqlParams + ")";
                try
                {
                    SACommand command = CommandTransaction(sqlString);

                    if (command != null)
                    {
                        command.ExecuteScalar();
                    }

                }
                catch (System.Exception e)
                {
                    Rollback();
                    string msg = "DataAccessLayer";
                    throw new System.Exception(msg);
                }
                finally
                {
                    CloseInTransaction();
                }

            }
            public DataTable ExecuteSelectCommand(string CommandName, CommandType cmdType)
            {
                SACommand cmd = null;
                DataTable table = new DataTable();

                cmd = _connection.CreateCommand();

                cmd.CommandType = cmdType;
                cmd.CommandText = CommandName;

                try
                {
                    if (_connection.State != ConnectionState.Open)
                    {
                        _connection.Open();
                    }
                    SADataAdapter da = null;
                    using (da = new SADataAdapter(cmd))
                    {
                        da.Fill(table);
                    }
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    cmd.Dispose();
                    cmd = null;
                    _connection.Close();
                }
                return table;
            }

          
            public DataTable ExecuteParamerizedSelectCommand(string CommandName, CommandType cmdType, SAParameter[] param)
            {
                SACommand cmd = null;
                DataTable table = new DataTable();
                cmd = _connection.CreateCommand();
                cmd.CommandType = cmdType;
                cmd.CommandText = CommandName;
                cmd.Parameters.AddRange(param);

                try
                {
                    _connection.Open();

                    SADataAdapter da = null;
                    using (da = new SADataAdapter(cmd))
                    {
                        da.Fill(table);
                    }
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    cmd.Dispose();
                    cmd = null;
                    _connection.Close();
                }
                return table;
            }

            public string ExecuteQueryDataTable(string sqlQuery)
            {
                DataSet dts = LoadDataSet(sqlQuery);
                string executeQueryString = "";
                bool hasRow = (dts.Tables.Count > 0) && (dts.Tables[0].Rows.Count > 0);
                if (hasRow)
                {
                    DataRow row = dts.Tables[0].Rows[0];
                    executeQueryString = row.ItemArray[0] as string;
                }
                return executeQueryString;
            }
            public IList<string> ExecuteQueryFields(string sqlQuery)
            {
                IList<string> sList = new List<string>();
                DataSet dts = LoadDataSet(sqlQuery);
                // now we have to load the data set 
                return sList;
            }
            /// <summary>
            /// Execute an insert, update, delete with OdbcParameters. It returns with 
            /// </summary>
            /// <param name="CommandName">Parameters</param>
            /// <param name="cmdType">type of the command</param>
            /// <param name="pars">Parametes</param>
            /// <returns></returns>
            public bool ExecuteNonQuery(string CommandName, CommandType cmdType, SAParameter[] pars)
            {
                SACommand cmd = null;
                int res = 0;
                cmd = _connection.CreateCommand();
                cmd.CommandType = cmdType;
                cmd.CommandText = CommandName;
                cmd.Parameters.AddRange(pars);

                try
                {
                    _connection.Open();
                    res = cmd.ExecuteNonQuery();
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    cmd.Dispose();
                    cmd = null;
                    _connection.Close();
                }

                if (res >= 1)
                {
                    return true;
                }
                return false;
            }

     

            public Task<DataSet> FillSchemaAsync(SADataAdapter dta, DataSet dataSet, CancellationToken cancellationToken)
            {
                var result = new TaskCompletionSource<DataSet>();
                if (cancellationToken == CancellationToken.None || !cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        lock (asyncLock2)
                        {
                            dta.FillSchema(dataSet, SchemaType.Source);
                            result.SetResult(dataSet);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        result.SetException(ex);
                    }
                }
                else
                {
                    result.SetCanceled();
                }
                return result.Task;
            }
            public Task<DataSet> FillAsync(SADataAdapter dta, DataSet dataSet, CancellationToken cancellationToken)
            {
                var result = new TaskCompletionSource<DataSet>();
                if (cancellationToken == CancellationToken.None || !cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        lock (asyncLock2)
                        {
                            dta.Fill(dataSet);
                            result.SetResult(dataSet);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        result.SetException(ex);
                    }
                }
                else
                {
                    result.SetCanceled();
                }
                return result.Task;
            }
            private SADataAdapter CreateDataAdapter(string sqlQuery)
            {
                SADataAdapter dta = null;
                lock (dataAdapterLock)
                {
                    try
                    {
                        if (_transaction == null)
                        {
                            Open();
                            _command = new SACommand(sqlQuery, _connection);
                        }
                        else
                        {
                            _command = new SACommand(sqlQuery, _connection, _transaction);
                        }
                        dta = new SADataAdapter(_command);

                    }
                    catch (System.Exception e)
                    {
                        if (_transaction != null)
                        {
                            _transaction.Rollback();
                        }
                        string msg = "Loading dataset error :" + e.Message;
                        throw new System.Exception(msg);
                    }
                }
                return dta;
            }
          
        }
    }


}
