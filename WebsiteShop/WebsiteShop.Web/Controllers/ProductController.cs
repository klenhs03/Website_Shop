using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteShop.BusinessLayers;
using WebsiteShop.DomainModels;
using WebsiteShop.Web.Models;

namespace WebsiteShop.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMINISTRATOR},{WebUserRoles.EMPLOYEE}")]
    public class ProductController : Controller
    {

        private const int PAGE_SIZE = 10;
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchCondition";
        public IActionResult Index()
        {
            ProductSearchInput? condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
            if (condition == null)
                condition = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    CategoryID = 0,
                    SupplierID = 0,
                    MinPrice = 0,
                    MaxPrice = 0
                };

            return View(condition);
        }
        public IActionResult Search(ProductSearchInput condition)
        {
            int rowCount = 0;
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
            var model = new ProductSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data,
                CategoryID = condition.CategoryID,
                SupplierID = condition.SupplierID,
                MinPrice = condition.MinPrice,
                MaxPrice = condition.MaxPrice
            };
            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung mặt hàng mới";
            var data = new Product()
            {
                ProductID = 0,
                Photo = "nophoto.jpg"
            };
            return View("Edit", data);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin mặt hàng";
            var data = ProductDataService.GetProduct(id);
            if (data == null)
                return RedirectToAction("Index");
            if (string.IsNullOrEmpty(data.Photo))
                data.Photo = "nophoto.jpg";

            return View(data);
        }
        [HttpPost]//Attribute => Chỉ nhận dữ liệu gửi lên dưới dạng Post
        public IActionResult Save(Product data, IFormFile? _Photo)
        {
            ViewBag.Title = data.ProductID == 0 ? "Bổ sung mặt hàng mới" : "Cập nhật thông tin mặt hàng";

            if (string.IsNullOrWhiteSpace(data.ProductName))
                ModelState.AddModelError(nameof(data.ProductName), "Tên không được để trống");
            if (data.CategoryID == 0)
                ModelState.AddModelError(nameof(data.CategoryID), "Tên không được để trống");
            if (data.SupplierID == 0)
                ModelState.AddModelError(nameof(data.SupplierID), "Tên không được để trống");
            if (string.IsNullOrWhiteSpace(data.Unit))
                ModelState.AddModelError(nameof(data.Unit), "Đơn vị tính không được để trống");
            if (data.Price < 0)
                ModelState.AddModelError(nameof(data.Price), "Vui lòng nhập giá");


            if (!ModelState.IsValid)
            {

                return View("Edit", data);
            }
            if (_Photo != null)
            {

                //Tên file sẽ lưu trên server
                string fileName = $"{DateTime.Now.Ticks}_{_Photo.FileName}";
                //Đường dẫn đến file sẽ lưu trên server (vd: D:\MyWeb\wwwroot\images\employees\photo.png)
                string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images\products", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    _Photo.CopyTo(stream);
                }
                data.Photo = fileName;
            }
            try
            {
                if (data.ProductID == 0)
                {
                    int id = ProductDataService.AddProduct(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError("Error", "Không cập nhật được mặt hàng.");
                        return View("Edit", data);
                    }
                }
                else
                {
                    bool result = ProductDataService.UpdateProduct(data);
                    if (!result)
                    {
                        ModelState.AddModelError("Error", "Không cập nhật được mặt hàng.");
                        return View("Edit", data);
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return View("Edit", data);
            }

        }
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                bool result = ProductDataService.DeleteProduct(id);
                return RedirectToAction("Index");
            }
            var data = ProductDataService.GetProduct(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
        public IActionResult Photo(int id = 0, string method = "", int photoId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung ảnh cho mặt hàng";

                    var data = new ProductPhoto()
                    {
                        PhotoID = 0,
                        DisplayOrder = 0,
                        ProductID = id
                    };
                    return View(data);
                case "edit":
                    ViewBag.Title = "Cập nhật ảnh cho mặt hàng";
                    var md = ProductDataService.GetPhoto(photoId);
                    if (md == null)
                        return RedirectToAction("Edit");
                    return View(md);
                case "delete":
                    //TODO: Xóa ảnh có mã photoID (Xóa trực tiếp, không cần phải xác nhận)
                    ProductDataService.DeletePhoto(photoId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }
        public IActionResult SavePhoto(ProductPhoto data, IFormFile? _Photo = null)
        {
            if (string.IsNullOrWhiteSpace(data.Description))
                ModelState.AddModelError(nameof(data.Description), "Mô tả không được để trống");
            if (data.DisplayOrder == 0)
                ModelState.AddModelError(nameof(data.DisplayOrder), "Vui lòng chọn/nhập thứ tự hiển thị");

            if (!ModelState.IsValid && data.PhotoID > 0) // NẾu trường hợp Modelstate không hợp lệ
            {
                ViewBag.Title = "Cập nhật ảnh cho mặt hàng";
                return View("Photo", data);
            }
            if (!ModelState.IsValid) // NẾu trường hợp Modelstate không hợp lệ
            {
                ViewBag.Title = "Bổ sung ảnh cho mặt hàng";
                return View("Photo", data);
            }
            if (_Photo != null)
            {

                //Tên file sẽ lưu trên server
                string fileName = $"{DateTime.Now.Ticks}_{_Photo.FileName}";
                //Đường dẫn đến file sẽ lưu trên server (vd: D:\MyWeb\wwwroot\images\employees\photo.png)
                string filePath = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images\products", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    _Photo.CopyTo(stream);
                }
                data.Photo = fileName;
            }
            if (data.PhotoID == 0)
            {
                long id = ProductDataService.AddPhoto(data);
                if (id <= 0)
                {
                    ViewBag.Title = "Bổ sung ảnh cho mặt hàng";
                    return View("Edit", data);
                }
            }
            else
            {
                bool result = ProductDataService.UpdatePhoto(data);
                if (!result)
                {
                    ViewBag.Title = "Cập nhật thông tin cho ảnh mặt hàng";
                    return View("Edit", data);
                }
            }
            return RedirectToAction("Edit");
        }

        public IActionResult SaveAttribute(ProductAttribute data)
        {
            if (string.IsNullOrWhiteSpace(data.AttributeName))
                ModelState.AddModelError(nameof(data.AttributeName), "Tên thuộc tính không được để trống");
            if (string.IsNullOrWhiteSpace(data.AttributeValue))
                ModelState.AddModelError(nameof(data.AttributeValue), "Giá trị thuộc tính không được để trống");
            if (data.DisplayOrder == 0)
                ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị lớn hơn 0");

            if (!ModelState.IsValid && data.AttributeID > 0) // NẾu trường hợp Modelstate không hợp lệ
            {
                ViewBag.Title = "Cập nhật thuộc tính của mặt hàng";
                return View("Attribute", data);
            }
            if (!ModelState.IsValid) // Nếu trường hợp Modelstate không hợp lệ
            {
                ViewBag.Title = "Bổ sung thuộc tính của mặt hàng";
                return View("Attribute", data);
            }
            if (data.AttributeID == 0)
            {
                long id = ProductDataService.AddAttribute(data);
                if (id <= 0)
                {
                    ViewBag.Title = "Bổ sung thuộc tính của mặt hàng";
                    return View("Edit", data);
                }
            }
            else
            {
                bool result = ProductDataService.UpdateAttribute(data);
                if (!result)
                {
                    ViewBag.Title = "Cập nhật thông tin thuộc tính của ảnh mặt hàng";
                    return View("Edit", data);
                }
            }
            return RedirectToAction("Edit");
        }
        public IActionResult Attribute(int id = 0, string method = "", int attributeId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính của mặt hàng";

                    var data = new ProductAttribute()
                    {
                        AttributeID = 0,
                        DisplayOrder = 0,
                        ProductID = id
                    };
                    return View(data);
                case "edit":
                    ViewBag.Title = "Cập nhật thuộc tính của mặt hàng";
                    var md = ProductDataService.GetAttribute(attributeId);
                    if (md == null)
                        return RedirectToAction("Edit");
                    return View(md);
                case "delete":
                    ProductDataService.DeleteAttribute(attributeId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }
    }
}