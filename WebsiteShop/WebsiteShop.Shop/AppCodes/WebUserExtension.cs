using WebsiteShop.Shop;
using System.Security.Claims;
using WebsiteShop.Shop.AppCodes;

namespace WebsiteShop.Shop
{
    /// <summary>
    /// Tạo thêm phương thức ( hàm) mở rộng cho Principal để lấy thông
    /// tin của người dùng dựa trên Cookie
    /// </summary>
    public static class WebUserExtensions
    {
        public static WebUserData? GetUserData(this ClaimsPrincipal principal)
        {
            try
            {
                if (principal == null || principal.Identity == null || !principal.Identity.IsAuthenticated)
                    return null;

                var userData = new WebUserData();

                userData.UserId = principal.FindFirstValue(nameof(userData.UserId)) ?? "";
                userData.UserName = principal.FindFirstValue(nameof(userData.UserName)) ?? "";
                userData.DisplayName = principal.FindFirstValue(nameof(userData.DisplayName)) ?? "";
                userData.Photo = principal.FindFirstValue(nameof(userData.Photo)) ?? "";
                userData.Phone = principal.FindFirstValue(nameof(userData.Phone)) ?? "";
                userData.Email = principal.FindFirstValue(nameof(userData.Email)) ?? "";
                userData.Province = principal.FindFirstValue(nameof(userData.Province)) ?? "";


                userData.Roles = new List<string>();
                foreach (var item in principal.FindAll(ClaimTypes.Role))
                {
                    userData.Roles.Add(item.Value);
                }

                return userData;
            }
            catch
            {
                return null;
            }
        }
    }
}
