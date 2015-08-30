using System.Data;
using System.Data.SqlClient;
using MVC6MusicStore.Core.DAL.ADO.NET;

namespace MVC6MusicStore.Core.SqlCommands
{
    internal sealed class GetAllAlbumsCommand : ISqlCommand
    {
        public SqlCommand GetCommand()
        {
            var result = new SqlCommand("[dbo].[HoleAlleAlben]")
            {
                CommandType = CommandType.StoredProcedure
            };

            return result;
        }
    }
}