using System.Data.SqlClient;

namespace MVC6MusicStore.Core.DAL.ADO.NET
{
    public interface ISqlCommand
    {
        SqlCommand GetCommand();
    }
}