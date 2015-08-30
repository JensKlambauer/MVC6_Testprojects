using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Transactions;

namespace MVC6MusicStore.Core.DAL.ADO.NET
{
    public interface IDatabaseAdapter
    {
        IDataReader ExecuteReader(ISqlCommand sqlCommand);

        IDataReader ExecuteReader(SqlCommand sqlCommand);

        int ExecuteCommand(ISqlCommand sqlCommand);

        int ExecuteCommand(ISqlCommand sqlCommand, TransactionScopeOption transactionScopeOption);

        void SaveChanges(DataTable table, ISqlAdapterCommands commands);

        void ExecuteCommands(IEnumerable<ISqlCommand> commands);

        void ExecuteCommands(IEnumerable<SqlCommand> commands);
      
        void ExecuteCommands(IEnumerable<ISqlCommand> commands, TransactionScopeOption transactionScopeOption);

        void ExecuteCommands(IEnumerable<SqlCommand> commands, TransactionScopeOption transactionScopeOption);

        string CurrentDatabaseName { get; }
        Task<IDataReader> ExecuteReaderAsync(ISqlCommand sqlCommand);
    }
}