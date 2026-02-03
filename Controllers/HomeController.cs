using System.Diagnostics;
using Ecommerce2.Models;
using Ecommerce2.Models.DTOs;
using Ecommerce2.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeRepository _homeRepository;

        public HomeController(ILogger<HomeController> logger, IHomeRepository homeRepository)
        {
            _homeRepository = homeRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string sTerm = "", int genreId = 0)
        {
            //IEnumerable<Book> books  = await _homeRepository.GetBooks(sTerm, generyId);
            //return View(books);
            IEnumerable<Book> books = await _homeRepository.GetBooks(sTerm, genreId);
            IEnumerable<Genre> genres = await _homeRepository.Genres();
            BookDisplayModel bookDisplayModel = new BookDisplayModel
            {
                Books = books,
                Genres = genres,
                sTerm = sTerm,
                genreId = genreId
            };

            return View(bookDisplayModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
