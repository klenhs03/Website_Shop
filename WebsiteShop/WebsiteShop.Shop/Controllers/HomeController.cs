using Microsoft.AspNetCore.Mvc;
using WebsiteShop.BusinessLayers;
using WebsiteShop.Shop.Models;
using System.Diagnostics;

namespace WebsiteShop.Shop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            int rowCount;
            var categories = CommonDataService.ListOfCategories(out rowCount, 1, 0, "");
            var products = ProductDataService.GetProductsByCategory(out rowCount, 2);  // Lọc sản phẩm có CategoryID = 2

            var viewModel = new HomeViewModel
            {
                Categories = categories,
                FeaturedProducts = products
            };

            return View(viewModel);
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
