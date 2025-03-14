using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteShop.BusinessLayers;
using System.Security.Claims;
using WebsiteShop.DomainModels;
using WebsiteShop.Shop.AppCodes;

namespace WebsiteShop.Shop.AppCode
{
    public class AccountController : Controller
    {
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            ViewBag.Username = username;

            // Kiểm tra thông tin đầu vào
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Nhập tên và mật khẩu!");
                return View();
            }

            // Kiểm tra username và password
            var userAccount = UserAccountService.Authorize(UserTypes.Customer, username, password);
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại");
                return View();
            }

            // Đăng nhập thành công: Ghi nhận trạng thái đăng nhập
            var userData = new WebUserData()
            {
                UserId = userAccount.UserId,
                UserName = userAccount.UserName,
                DisplayName = userAccount.DisplayName,
                Phone = userAccount.Phone, // Thêm phone vào userData
                Email = userAccount.Email,
                Province = userAccount.Province, // Thêm city vào userData
                Photo = userAccount.Photo,
                Roles = userAccount.RoleNames.Split(',').ToList() // Lưu role vào Cookie
            };

            // Ghi nhận trạng thái đăng nhập vào Cookie
            var principal = userData.CreatePrincipal(); // Tạo ClaimsPrincipal từ userData
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(principal.Identity));

            // Điều hướng đến trang chủ hoặc checkout nếu có ReturnUrl
            var returnUrl = HttpContext.Request.Query["ReturnUrl"].ToString();
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }



        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // Xóa thông tin phiên làm việc của người dùng
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(string oldPassword, string newPassword, string confirmPassword)
        {
            var userData = User.GetUserData();
            var username = userData.UserName;

            // Kiểm tra mật khẩu mới và xác nhận mật khẩu
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("NewPassword", "Mật khẩu mới và xác nhận mật khẩu không khớp!");
                return View("ChangePassword");
            }

            // Kiểm tra mật khẩu cũ
            var userAccount = UserAccountService.Authorize(UserTypes.Customer, username, oldPassword);
            if (userAccount == null)
            {
                ModelState.AddModelError("OldPassword", "Mật khẩu cũ không chính xác!");
                return View("ChangePassword");
            }

            // Thực hiện thay đổi mật khẩu
            bool isPasswordChanged = UserAccountService.ChangePassword(username, newPassword);
            if (!isPasswordChanged)
            {
                ModelState.AddModelError("Confirm", "Đổi mật khẩu thất bại!");
                return View("ChangePassword");
            }

            return RedirectToAction("Index", "Home");
        }

        // Phương thức để hiển thị thông tin Profile
        [Authorize]
        public IActionResult Profile()
        {
            var userData = User.GetUserData();  // Lấy thông tin người dùng từ session

            // Chuyển đổi UserId từ string sang int (nếu có thể)
            if (int.TryParse(userData.UserId, out int userId))
            {
                var customer = CommonDataService.GetCustomer(userId); // Lấy thông tin khách hàng theo UserId đã chuyển đổi

                if (customer == null)
                {
                    return RedirectToAction("Index", "Home"); // Nếu không tìm thấy khách hàng, chuyển về trang chủ
                }

                return View(customer); // Trả về view Profile với thông tin khách hàng
            }

            // Nếu UserId không hợp lệ, chuyển hướng về trang chủ
            return RedirectToAction("Index", "Home");
        }


        // Phương thức để lưu thông tin khi người dùng chỉnh sửa Profile
        [HttpPost]
        [Authorize]
        public IActionResult SaveProfile(Customer data)
        {
            var userData = User.GetUserData();  // Lấy thông tin người dùng từ session để xác thực
            if (int.TryParse(userData.UserId, out int userId) && userId != data.CustomerID)
            {
                ModelState.AddModelError("Error", "Thông tin không hợp lệ.");
                return View("Profile", data); // Nếu ID không khớp, trả về trang Profile với thông báo lỗi
            }

            // Kiểm tra tính hợp lệ của dữ liệu nhập vào
            if (string.IsNullOrWhiteSpace(data.CustomerName))
                ModelState.AddModelError(nameof(data.CustomerName), "Tên khách hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập điện thoại của khách hàng");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập email của khách hàng");
            if (string.IsNullOrWhiteSpace(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Vui lòng nhập địa chỉ của khách hàng");
            if (string.IsNullOrEmpty(data.Province))
                ModelState.AddModelError(nameof(data.Province), "Hãy chọn tỉnh/thành cho khách hàng");

            if (!ModelState.IsValid)
            {
                return View("Profile", data); // Nếu có lỗi, quay lại trang Profile với dữ liệu đã nhập
            }

            try
            {
                bool result = CommonDataService.UpdateCustomer(data); // Cập nhật thông tin khách hàng trong cơ sở dữ liệu
                if (!result)
                {
                    ModelState.AddModelError("Error", "Đã xảy ra lỗi khi cập nhật thông tin khách hàng.");
                    return View("Profile", data); // Nếu không thành công, quay lại trang Profile
                }

                // Cập nhật lại thông tin trong session nếu cập nhật thành công
                userData.UserName = data.CustomerName;
                userData.Phone = data.Phone;
                userData.Email = data.Email;
                userData.Address = data.Address;
                userData.Province = data.Province;
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(userData.CreatePrincipal().Identity));

                TempData["SuccessMessage"] = "Thông tin khách hàng đã được cập nhật thành công.";
                return RedirectToAction("Profile"); // Quay lại trang Profile sau khi cập nhật thành công
            }
            catch
            {
                ModelState.AddModelError("Error", "Hệ thống tạm thời gián đoạn. Vui lòng thử lại sau.");
                return View("Profile", data); // Nếu có lỗi hệ thống, quay lại trang Profile
            }
        }





    }
}

