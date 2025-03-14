using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteShop.BusinessLayers;
using WebsiteShop.Web.AppCodes;

namespace WebsiteShop.Web.Controllers
{
    public class AccountController : Controller
    {
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

            //Kiểm tra thông tin đầu vào
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Nhập tên và mật khẩu!");
                return View();
            }

            //TODO: Kiểm tra xem username và password 
            var userAccount = UserAccountService.Authorize(UserTypes.Employee, username, password);
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại");
                return View();
            }

            // Đăng nhập thành công: Ghi nhận trạng thái đăng nhập  
            // 1. Tạo thông tin của người dùng
            var userData = new WebUserData()
            {
                UserId = userAccount.UserId,
                UserName = userAccount.UserName,
                DisplayName = userAccount.DisplayName,
                Photo = userAccount.Photo,
                Roles = userAccount.RoleNames.Split(',').ToList()
            };

            // 2. Ghi nhận trạng thái đăng nhập
            await HttpContext.SignInAsync(userData.CreatePrincipal());

            // 3. Quay về trang chủ
            return RedirectToAction("Index", "Home");

        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenined()
        {
            return View();
        }

        public IActionResult ChangePassword()
        {
            return View();  // Trả về view đổi mật khẩu
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(string oldPassword, string newPassword, string confirmPassword)
        {
            var userData = User.GetUserData();
            var username = userData.UserName;
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("NewPassword", "Mật khẩu mới và xác nhận mật khẩu không khớp!");
                return View("ChangePassword");
            }

            // Kiểm tra mật khẩu cũ 
            var userAccount = UserAccountService.Authorize(UserTypes.Employee, username, oldPassword);
            if (userAccount == null)
            {
                ModelState.AddModelError("OldPassword", "Mật khẩu cũ không chính xác!");
                return View("ChangePassword");
            }
            bool isPasswordChanged = UserAccountService.ChangePassword(username, newPassword);
            if (!isPasswordChanged)
            {
                ModelState.AddModelError("Confirm", "Đổi mật khẩu thất bại!");
                return View("ChangePassword");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
