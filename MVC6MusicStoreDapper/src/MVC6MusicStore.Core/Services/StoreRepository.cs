using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Microsoft.Framework.Configuration;
using MVC6MusicStore.Core.Models;
using Dapper;
using System.Linq;
using Microsoft.AspNet.Mvc;

namespace MVC6MusicStore.Core.Services
{
    public sealed class StoreRepository : IStoreRepository
    {
        private readonly string connectionString;

        public StoreRepository(IConfiguration configuration)
        {
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

                ////LogService.Info("SQL query '{1}' was done in {0} ms.", watch.ElapsedMilliseconds, "[dbo].[HoleAlleAlbenDapper]");
                con.Close();
            }

            return result;
        }
    }
}