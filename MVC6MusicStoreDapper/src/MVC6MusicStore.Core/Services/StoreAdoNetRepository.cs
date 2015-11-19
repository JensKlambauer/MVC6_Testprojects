namespace MVC6MusicStore.Core.Services
{
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using MVC6MusicStore.Core.DAL;
    using MVC6MusicStore.Core.DAL.ADO.NET;
    using MVC6MusicStore.Core.Models;
    using MVC6MusicStore.Core.SqlCommands;

    public sealed class StoreAdoNetRepository : IStoreAdoNetRepository
    {
        private readonly IDatabaseAdapter databaseAdapter;
        private readonly ILogger<IStoreAdoNetRepository> logger;

        public StoreAdoNetRepository(IDatabaseAdapter databaseAdapter, ILogger<IStoreAdoNetRepository> logger)
        {
            this.databaseAdapter = databaseAdapter;
            this.logger = logger;
        }

        public IEnumerable<Album> GetAllAlbumsAdoNet()
        {
            using (var reader = this.databaseAdapter.ExecuteReader(new GetAllAlbumsCommand()))
            {
                var result = new List<Album>();

                reader.DoForCurrentResultSetAndMoveNext(
                   r =>
                   {
                       result.Add(new Album(reader));
                   });

                return result;
            }
        }

        public async Task<IEnumerable<Album>> GetAllAlbumsAdoNetAsync()
        {
            var watch = new Stopwatch();
            watch.Start();
            using (var reader = (SqlDataReader) await this.databaseAdapter.ExecuteReaderAsync(new GetAllAlbumsCommand()))
            {
                var result = new List<Album>();

                while (await reader.ReadAsync())
                {
                    result.Add(new Album(reader));
                }
                watch.Stop();

                this.logger.LogInformation("SQL query {0} was done in {1} ms.", new object[] { "[dbo].[HoleAlleAlben] Ado.Net Async after Reading", watch.ElapsedMilliseconds });
                return result;
            }
        }
    }
}