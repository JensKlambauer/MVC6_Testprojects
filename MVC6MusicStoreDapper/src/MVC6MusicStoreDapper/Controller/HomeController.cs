using System.Linq;
using Microsoft.AspNet.Mvc;
using MVC6MusicStore.Core.Services;

namespace MVC6MusicStoreDapper.Controller
{
    public class HomeController : Microsoft.AspNet.Mvc.Controller
    {
        private readonly IStoreRepository storeRepository;

        public HomeController(IStoreRepository storeRepository)
        {
            this.storeRepository = storeRepository;
        }

        public IActionResult Index()
        {
            var albums = this.storeRepository.GetAllAlbumsDapper().ToList();

            return View();
        }
    }
}