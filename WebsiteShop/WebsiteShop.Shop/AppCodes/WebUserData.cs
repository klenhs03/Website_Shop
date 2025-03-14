using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using WebsiteShop.DomainModels;

namespace WebsiteShop.Shop.AppCodes
{
    /// <summary>
    /// Lưu giữ thông tin của người dùng được ghi trong Cookie
    /// </summary>
    public class WebUserData
    {
        public string UserId { get; set; } = "";
        public string UserName { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";
        public string Province { get; set; } = "";
        public string Photo { get; set; } = "";
        public string Email { get; set; } = "";
        public List<string>? Roles { get; set; }

        /// <summary>
        /// Danh sách các Claim
        /// </summary>
        private List<Claim> Claims
        {
            get
            {
                List<Claim> claims = new List<Claim>()
                {
                    new Claim(nameof(UserId), UserId),
                    new Claim(nameof(UserName), UserName),
                    new Claim(nameof(DisplayName), DisplayName),
                    new Claim(nameof(Phone), Phone),  // Thêm Phone vào Claims
                   new Claim(nameof(Address), Address),
                    new Claim(nameof(Province), Province),
                    new Claim(nameof(Email), Email),
                    new Claim(nameof(Photo), Photo)
                };
                if (Roles != null)
                    foreach (var role in Roles)
                        claims.Add(new Claim(ClaimTypes.Role, role));
                return claims;
            }

        }

        /// <summary>
        /// Tạo ClaimsPrincipal dựa trên thông tin của người dùng ( cần lưu trong Cookie)
        /// </summary>
        /// <returns></returns>
        public ClaimsPrincipal CreatePrincipal()
        {
            var identity = new ClaimsIdentity(Claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            return principal;
        }

    }
}
