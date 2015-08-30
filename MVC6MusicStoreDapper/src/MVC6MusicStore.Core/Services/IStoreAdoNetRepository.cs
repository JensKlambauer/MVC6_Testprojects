using System.Collections.Generic;
using System.Threading.Tasks;
using MVC6MusicStore.Core.Models;

namespace MVC6MusicStore.Core.Services
{
    public interface IStoreAdoNetRepository
    {
        IEnumerable<Album> GetAllAlbumsAdoNet();
        Task<IEnumerable<Album>> GetAllAlbumsAdoNetAsync();
    }
}