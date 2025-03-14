using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Logging;
using WebsiteShop.Shop;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddControllersWithViews()
           .AddMvcOptions(option =>
           {
               option.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
           });
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(option =>
            {
                option.Cookie.Name = "AuthenticationCookie";          //Tên của Cookie
                option.LoginPath = "/Account/Login";                  //URL của trang login
                option.AccessDeniedPath = "/Account/AccessDenined";   //URL của trang khi người sử dụng không được cấp quyền
                option.ExpireTimeSpan = TimeSpan.FromDays(360);       //Khoảng thời gian tồn tại của Cookie
            });
        builder.Services.AddSession(option =>
        {
            option.IdleTimeout = TimeSpan.FromMinutes(60);
            option.Cookie.HttpOnly = true;
            option.Cookie.IsEssential = true;
        });
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSession();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        ApplicationContext.Configure
        (
            context: app.Services.GetRequiredService<IHttpContextAccessor>(),
            enviroment: app.Services.GetRequiredService<IWebHostEnvironment>()
        );

        /// Khởi tạo cấu hình cho BusinessLayer
        string connectionString = builder.Configuration.GetConnectionString("LiteCommerceDB");
        WebsiteShop.BusinessLayers.Configuration.Initialize(connectionString);

        app.Run();
    }
}