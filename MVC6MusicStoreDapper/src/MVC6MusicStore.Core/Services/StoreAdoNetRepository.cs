using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Framework.Logging;
using MVC6MusicStore.Core.DAL.ADO.NET;
using MVC6MusicStore.Core.Models;
using MVC6MusicStore.Core.DAL;
using MVC6MusicStore.Core.SqlCommands;

namespace MVC6MusicStore.Core.Services
{
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
    }
}