using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using MVC6MusicStore.Core.Services;

namespace MVC6MusicStoreDapper.Controller
{
    public class HomeController : Microsoft.AspNet.Mvc.Controller
    {
        private readonly IStoreRepository storeRepository;
        private readonly IStoreAdoNetRepository storeAdoNetRepository;

        public HomeController(IStoreRepository storeRepository, IStoreAdoNetRepository storeAdoNetRepository)
        {
            this.storeRepository = storeRepository;
            this.storeAdoNetRepository = storeAdoNetRepository;
        }

        public IActionResult Index()
        {
            ////var albums = await this.storeRepository.GetAllAlbumsDapperAsync();
            var albums = this.storeRepository.GetAllAlbumsDapper();
            ////var albums = this.storeAdoNetRepository.GetAllAlbumsAdoNet();

            return View();
        }
    }
}