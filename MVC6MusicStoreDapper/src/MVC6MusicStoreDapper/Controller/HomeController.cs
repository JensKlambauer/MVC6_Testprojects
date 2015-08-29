using System.Linq;
using System.Threading.Tasks;
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

        public async Task<IActionResult> Index()
        {
            var albums = await this.storeRepository.GetAllAlbumsDapperAsync();

            return View();
        }
    }
}