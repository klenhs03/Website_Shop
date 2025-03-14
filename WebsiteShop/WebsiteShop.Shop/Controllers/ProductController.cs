using Microsoft.AspNetCore.Mvc;
using WebsiteShop.BusinessLayers;
using WebsiteShop.DomainModels;
using WebsiteShop.Shop.AppCode;
using WebsiteShop.Shop.Models;

namespace WebsiteShop.Shop.Controllers
{
    public class ProductController : Controller
    {
        private const int PAGE_SIZE = 40;
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchCondition";
        public IActionResult Index()
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(PRODUCT_SEARCH_CONDITION);
            if (condition == null)
                condition = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    CategoryID = 0,
                    SupplierID = 0,

                };
            return View(condition);
        }

        public IActionResult Search(PaginationSearchInput condition)
        {
            int rowCount;
            var data = ProductDataService.ListProducts(
                out rowCount,
                condition.Page,
                condition.PageSize,
                condition.SearchValue ?? "",
                condition.CategoryID,
                condition.SupplierID,
                condition.MinPrice,
                condition.MaxPrice
            );



            int categoryCount;
            var categories = CommonDataService.ListOfCategories(out categoryCount, 1, 0, "");

            var model = new ProductSearchResult
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                Categories = categories,
                Data = data ?? new List<Product>(),
                RowCount = rowCount,
                Condition = condition
            };

            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);
            return View(model);
        }



        public IActionResult Details(int id = 0)
        {
            // Lấy thông tin sản phẩm
            var product = ProductDataService.GetProduct(id);
            if (product == null)
                return RedirectToAction("Index");

            // Lấy mô tả sản phẩm
            var description = ProductDataService.GetProductDescription(id)?.ProductDescription;

            // Lấy danh sách chi tiết sản phẩm
            var details = ProductDataService.ListProductDetails(id);

            // Tạo model
            var model = new ProductDetailModel()
            {
                Product = product,
                ProductDescription = description,
                Details = details ?? new List<ProductDetail>()
            };

            return View(model);
        }


    }
}