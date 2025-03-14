using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteShop.BusinessLayers;
using WebsiteShop.DomainModels;
using WebsiteShop.Shop.AppCodes;

namespace WebsiteShop.Shop.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.CUSTOMER}")]
    public class CheckoutController : Controller
    {
        private const string SHOPPING_CART = "ShoppingCart";
      

        private List<Models.CartItem> GetShoppingCart()
        {
            return ApplicationContext.GetSessionData<List<Models.CartItem>>(SHOPPING_CART) ?? new List<Models.CartItem>();
        }

        public IActionResult Index()
        {
            var userData = User.GetUserData();

            if (userData == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Customer? customer = null;

            // Lấy thông tin khách hàng từ UserId hoặc email
            if (int.TryParse(userData.UserId, out int userId))
            {
                customer = CommonDataService.GetCustomer(userId);
            }

            if (customer == null && !string.IsNullOrEmpty(userData.Email))
            {
                customer = CommonDataService.GetCustomerByEmail(userData.Email);
            }

            if (customer == null)
            {
                return RedirectToAction("Profile", "Account");
            }

            var shoppingCart = GetShoppingCart();

            if (!shoppingCart.Any())
            {
                ViewBag.Message = "Không có đơn hàng nào !"; 
                return View("Index"); // Trả về View Index, nhưng không gửi ViewModel
            }

            var cartTotal = shoppingCart.Sum(item => item.Quantity * item.SalePrice);

            // Tạo view model cho checkout
            var viewModel = new Models.CheckoutViewModel
            {
                Customer = customer,
                CartItems = shoppingCart,
                CartSubtotal = cartTotal,
                CartTotal = cartTotal,
                DisplayName = customer.CustomerName,
                Email = customer.Email,
                Provinces = CommonDataService.ListOfProvinces()
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult PlaceOrder(Models.CheckoutViewModel model)
        {
            var userData = User.GetUserData();
            if (userData == null)
            {
                return Json("Vui lòng đăng nhập.");
            }

            Customer? customer = null;

            // Lấy thông tin khách hàng từ email hoặc UserId
            if (!string.IsNullOrEmpty(userData.Email))
            {
                customer = CommonDataService.GetCustomerByEmail(userData.Email);
            }

            if (customer == null && int.TryParse(userData.UserId, out int userId))
            {
                customer = CommonDataService.GetCustomer(userId);
            }

            if (customer == null)
            {
                return Json("Không tìm thấy thông tin khách hàng.");
            }

            var shoppingCart = GetShoppingCart();
            if (!shoppingCart.Any())
            {
                return Json("Giỏ hàng trống.");
            }

            // Kiểm tra Province từ danh sách hợp lệ
            var selectedProvince = model.Province;
            if (!CommonDataService.ListOfProvinces().Any(p => p.ProvinceName == selectedProvince))
            {
                return Json(new { success = false, message = "Tỉnh/Thành phố không hợp lệ." });
            }

            // Cập nhật thông tin khách hàng
            customer.Email = model.Email;
            customer.Phone = model.Phone;
            customer.Province = selectedProvince;

            CommonDataService.UpdateCustomer(customer);

            // Tạo danh sách chi tiết đơn hàng
            var orderDetails = shoppingCart.Select(item => new OrderDetail
            {
                ProductID = item.ProductID,
                Quantity = item.Quantity,
                SalePrice = item.SalePrice
            }).ToList();

            // Sử dụng EmployeeID mặc định (ví dụ là 1, nhân viên mặc định trong hệ thống)
            int employeeID = 1; // Nhân viên mặc định

            // Tạo đơn hàng thông qua OrderDataService với employeeID mặc định
            int orderID = OrderDataService.InitOrder(
                employeeID, // Truyền employeeID mặc định
                customer.CustomerID,
                selectedProvince,
                model.deliveryAddress,
                orderDetails
            );

            if (orderID > 0)
            {
                // Xóa giỏ hàng sau khi đặt hàng thành công
                ApplicationContext.SetSessionData(SHOPPING_CART, new List<Models.CartItem>());

                // Chuyển hướng đến trang theo dõi đơn hàng và truyền orderID
                return RedirectToAction("Tracking", "Order", new { orderId = orderID });
            }

            return Json(new { success = false, message = "Đặt hàng thất bại." });
        }



    }
}
