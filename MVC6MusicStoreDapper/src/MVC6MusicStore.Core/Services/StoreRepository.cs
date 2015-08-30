using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Microsoft.Framework.Configuration;
using MVC6MusicStore.Core.Models;
using Dapper;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Logging;

namespace MVC6MusicStore.Core.Services
{
    public sealed class StoreRepository : IStoreRepository
    {
        private readonly ILogger<IStoreRepository> logger;

        private readonly string connectionString;

        public StoreRepository(IConfiguration configuration, ILogger<IStoreRepository> logger)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            this.logger = logger;
            this.connectionString = configuration.Get("Data:DefaultConnection:ConnectionString");
        }

        public IEnumerable<Album> GetAllAlbumsDapper()
        {
            List<Album> result;
            using (var con = new SqlConnection(this.connectionString))
            {
                con.Open();
                var watch = new Stopwatch();
                watch.Start();

                result = con.Query<Album, Genre, Artist, Album>(
                    "[dbo].[HoleAlleAlben]",
                    (album, genre, artist) =>
                    {
                        album.Genre = genre;
                        album.Artist = artist;
                        return album;
                    },
                    commandType: CommandType.StoredProcedure,
                    splitOn: "GenreId, ArtistId").ToList();
                watch.Stop();

                this.logger.LogInformation("SQL query {0} was done in {1} ms.", new object[] { "[dbo].[HoleAlleAlben] Dapper.net", watch.ElapsedMilliseconds});
                con.Close();
            }

            return result;
        }

        public async Task<IEnumerable<Album>> GetAllAlbumsDapperAsync()
        {
            List<Album> albums;
            using (var con = new SqlConnection(this.connectionString))
            {
                con.Open();
                var watch = new Stopwatch();
                watch.Start();

                var result = await con.QueryAsync<Album, Genre, Artist, Album>(
                    "[dbo].[HoleAlleAlben]",
                    (album, genre, artist) =>
                    {
                        album.Genre = genre;
                        album.Artist = artist;
                        return album;
                    },
                    commandType: CommandType.StoredProcedure,
                    splitOn: "GenreId, ArtistId");
                albums = result.ToList();
                watch.Stop();

                this.logger.LogInformation("SQL query {0} was done in {1} ms.", new object[] { "[dbo].[HoleAlleAlben] Dapper.net Async", watch.ElapsedMilliseconds});
                con.Close();
            }

            return albums;
        }
    }
}