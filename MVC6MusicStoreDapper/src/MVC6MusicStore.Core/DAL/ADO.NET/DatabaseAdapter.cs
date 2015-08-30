using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Transactions;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.Logging;

namespace MVC6MusicStore.Core.DAL.ADO.NET
{
    public sealed class DatabaseAdapter : IDatabaseAdapter
    {
        private readonly ILogger<IDatabaseAdapter> logger;
        private readonly string connectionString;
        
        public DatabaseAdapter(IConfiguration configuration, ILogger<IDatabaseAdapter> logger)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            this.connectionString = configuration.Get("Data:DefaultConnection:ConnectionString");
            this.logger = logger;
        }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The DataReader uses the CloseConnection CommandBehavior.")]
        public IDataReader ExecuteReader(ISqlCommand sqlCommand)
        {
            if (sqlCommand == null)
            {
                throw new ArgumentNullException("sqlCommand");
            }

            var connection = new SqlConnection(this.connectionString);

            try
            {
                connection.Open();

                using (var command = sqlCommand.GetCommand())
                {
                    command.Connection = connection;

                    var watch = new Stopwatch();
                    watch.Start();

                    var reader = command.ExecuteReader(CommandBehavior.CloseConnection);

                    watch.Stop();
                    this.logger.LogInformation("SQL query {0} was done in {1} ms.", new object[] { command.CommandText + " AdoNet", watch.ElapsedMilliseconds });

                    return reader;
                }
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                
                this.logger.LogError(exception.Message);

                throw;
            }
        }
      
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The DataReader uses the CloseConnection CommandBehavior.")]
        public IDataReader ExecuteReader(SqlCommand sqlCommand)
        {
            if (sqlCommand == null)
            {
                throw new ArgumentNullException("sqlCommand");
            }

            var connection = new SqlConnection(this.connectionString);

            try
            {
                connection.Open();

                using (sqlCommand)
                {
                    sqlCommand.Connection = connection;

                    ////string.Format(CultureInfo.InvariantCulture, "Executing Database command '{0}'.", sqlCommand.CommandText).LogAsInformation();

                    return sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
                }
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }

                this.logger.LogError(exception.Message);
                throw;
            }
        }

        public int ExecuteCommand(ISqlCommand sqlCommand)
        {
            if (sqlCommand == null)
            {
                throw new ArgumentNullException("sqlCommand");
            }

            using (var connection = new SqlConnection(this.connectionString))
            {
                try
                {
                    connection.Open();

                    using (var command = sqlCommand.GetCommand())
                    {
                        command.Connection = connection;
                        command.CommandTimeout = 1000;

                        ////string.Format(CultureInfo.InvariantCulture, "Executing Database command '{0}'.", command.CommandText).LogAsInformation();

                        return command.ExecuteNonQuery();
                    }
                }
                catch (Exception exception)
                {
                    this.logger.LogError(exception.Message);
                    throw;
                }
            }
        }
     
        public int ExecuteCommand(ISqlCommand sqlCommand, TransactionScopeOption transactionScopeOption)
        {
            if (sqlCommand == null)
            {
                throw new ArgumentNullException("sqlCommand");
            }

            int rowsAffected;

            using (var scope = new TransactionScope(transactionScopeOption))
            {
                try
                {
                    using (var sqlConnection = new SqlConnection(this.connectionString))
                    {
                        sqlConnection.Open();

                        using (var command = sqlCommand.GetCommand())
                        {
                            command.Connection = sqlConnection;

                            ////string.Format(CultureInfo.InvariantCulture, "Executing Database command '{0}'.", command.CommandText).LogAsInformation();

                            rowsAffected = command.ExecuteNonQuery();
                        }
                    }

                    scope.Complete();
                }
                catch (Exception exception)
                {
                    this.logger.LogError(exception.Message);
                    throw;
                }
            }

            return rowsAffected;
        }
        
        public void SaveChanges(DataTable table, ISqlAdapterCommands commands)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }

            if (commands == null)
            {
                throw new ArgumentNullException("commands");
            }

            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                using (var adapter = new SqlDataAdapter())
                {
                    commands.SetCommands(adapter, connection);

                    using (var dataSet = new DataSet())
                    {
                        using (DataTable changes = table.GetChanges())
                        {
                            if (changes != null)
                            {
                                dataSet.Tables.Add(changes);
                                adapter.UpdateBatchSize = 1000;
                                adapter.Update(dataSet, table.TableName);
                            }
                        }
                    }
                }
            }
        }
        
        public void ExecuteCommands(IEnumerable<ISqlCommand> commands)
        {
            if (commands == null)
            {
                throw new ArgumentNullException("commands");
            }

            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                foreach (var command in commands)
                {
                    try
                    {
                        using (var sqlCommand = command.GetCommand())
                        {
                            sqlCommand.Connection = connection;
                            sqlCommand.ExecuteNonQuery();
                        }
                    }
                    catch (SqlException sqlException)
                    {
                        this.logger.LogError(sqlException.Message);
                        throw;
                    }
                }
            }
        }
        
        public void ExecuteCommands(IEnumerable<SqlCommand> commands)
        {
            if (commands == null)
            {
                throw new ArgumentNullException("commands");
            }

            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                foreach (var command in commands)
                {
                    try
                    {
                        using (command)
                        {
                            command.Connection = connection;
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (SqlException sqlException)
                    {
                        this.logger.LogError(sqlException.Message);
                        throw;
                    }
                }
            }
        }
      
        public void ExecuteCommands(IEnumerable<ISqlCommand> commands, TransactionScopeOption transactionScopeOption)
        {
            if (commands == null)
            {
                throw new ArgumentNullException("commands");
            }

            var cmds = commands.ToList();

            ////string.Format(CultureInfo.InvariantCulture, "Try to execute '{0}' sql commands within transaction scope.", cmds.Count()).LogAsInformation();

            using (var scope = new TransactionScope(transactionScopeOption))
            {
                using (var sqlConnection = new SqlConnection(this.connectionString))
                {
                    sqlConnection.Open();

                    foreach (var command in cmds)
                    {
                        try
                        {
                            using (var sqlCommand = command.GetCommand())
                            {
                                sqlCommand.Connection = sqlConnection;
                                sqlCommand.ExecuteNonQuery();
                            }
                        }
                        catch (SqlException sqlException)
                        {
                            this.logger.LogError(sqlException.Message);
                            throw;
                        }
                    }
                }

                scope.Complete();
            }
        }
      
        public void ExecuteCommands(IEnumerable<SqlCommand> commands, TransactionScopeOption transactionScopeOption)
        {
            if (commands == null)
            {
                throw new ArgumentNullException("commands");
            }

            var cmds = commands.ToList();

            ////string.Format(CultureInfo.InvariantCulture, "Try to execute '{0}' sql commands within transaction scope.", cmds.Count()).LogAsInformation();

            using (var scope = new TransactionScope(transactionScopeOption))
            {
                using (var sqlConnection = new SqlConnection(this.connectionString))
                {
                    sqlConnection.Open();

                    foreach (var command in cmds)
                    {
                        try
                        {
                            using (command)
                            {
                                command.Connection = sqlConnection;
                                command.ExecuteNonQuery();
                            }
                        }
                        catch (SqlException sqlException)
                        {
                            this.logger.LogError(sqlException.Message);
                        }
                    }
                }

                scope.Complete();
            }
        }
        
        public string CurrentDatabaseName
        {
            get
            {
                using (var connection = new SqlConnection(this.connectionString))
                {
                    connection.Open();
                    return connection.Database;
                }
            }
        }
    }
}