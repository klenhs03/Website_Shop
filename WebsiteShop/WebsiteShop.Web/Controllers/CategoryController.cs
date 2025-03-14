using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteShop.BusinessLayers;
using WebsiteShop.DomainModels;
using WebsiteShop.Web.Models;

namespace WebsiteShop.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMINISTRATOR},{WebUserRoles.EMPLOYEE}")]
    public class CategoryController : Controller
    {
        private const int PAGE_SIZE = 5;
        private const string CATEGORY_SEARCH_CONDITION = "CategorySearchCondition";


        public IActionResult Index()
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(CATEGORY_SEARCH_CONDITION);
            if (condition == null)
                condition = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            return View(condition);
        }
        public IActionResult Search(PaginationSearchInput condition)
        {
            int rowCount;
            var data = CommonDataService.ListOfCategories(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            CategorySearchResult model = new CategorySearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(CATEGORY_SEARCH_CONDITION, condition);

            return View(model);

        }
        public IActionResult Create()
        {
            ViewBag.Title = "Thêm loại hàng";
            var data = new Category()
            {
                CategoryID = 0,
            };
            return View("Edit", data);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật loại hàng";
            var data = CommonDataService.GetCategory(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
        public IActionResult Delete(int id = 0)
        {
            // Xử lí theo phương thức post
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteCategory(id);
                return RedirectToAction("Index");
            }
            // Xử lí theo phương thức Get
            var data = CommonDataService.GetCategory(id);
            if (data == null)
                return RedirectToAction("Index");

            return View(data);
        }
        public IActionResult Save(Category data)
        {
            ViewBag.Title = data.CategoryID == 0 ? "Bổ sung loại hàng" : "Cập nhật thông tin loại hàng";

            // Kiểm tra các dữ liệu đầu vào để phát hiện các trường hợp không hợp lệ
            if (string.IsNullOrWhiteSpace(data.CategoryName))
                ModelState.AddModelError(nameof(data.CategoryName), "*");
            if (string.IsNullOrWhiteSpace(data.Description))
                ModelState.AddModelError(nameof(data.Description), "*");

            //TODO: Kiểm tra dữ liệu đầu vào đúng hay không ? ==> View->Task list
            if (ModelState.IsValid == false)
            {
                return View("Edit", data);
            }

            if (data.CategoryID == 0)
            {
                int id = CommonDataService.AddCategory(data);
                if (id <= 0)
                {
                    ModelState.AddModelError("InvalidCategoryName", "Tên loại hàng bị trùng");
                    return View("Edit", data);
                }
            }
            else
            {
                bool result = CommonDataService.UpdateCategory(data);
                if (!result)
                {
                    ModelState.AddModelError("InvalidCategoryName", "Tên loại hàng bị trùng");
                    return View("Edit", data);
                }
            }
            return RedirectToAction("Index");
        }
    }
}
