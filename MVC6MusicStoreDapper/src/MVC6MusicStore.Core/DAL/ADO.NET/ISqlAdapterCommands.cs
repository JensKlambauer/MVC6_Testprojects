using System.Data.SqlClient;

namespace MVC6MusicStore.Core.DAL.ADO.NET
{
    public interface ISqlAdapterCommands
    {
        SqlCommand GetSelectCommand();

        SqlCommand GetInsertCommand();

        SqlCommand GetUpdateCommand();
        
        SqlCommand GetDeleteCommand();
       
        void SetCommands(SqlDataAdapter sqlDataAdapter, SqlConnection connection);
    }
}