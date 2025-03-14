using WebsiteShop.BusinessLayers;
using WebsiteShop.DataLayers;
using WebsiteShop.DomainModels;

namespace WebsiteShop.BusinessLayers
{

    public static class UserAccountService
    {
        private static readonly IUserAccountDAL employeeDB;
        private static readonly IUserAccountDAL customerDB;

        static UserAccountService()
        {
            string connectionString = Configuration.ConnectionString;

            employeeDB = new DataLayers.SQLServer.EmployeeAccountDAL(connectionString);
            customerDB = new DataLayers.SQLServer.CustomerAccountDAL(connectionString);
        }

        public static UserAccount? Authorize(UserTypes userType, string username, string password)
        {
            if (userType == UserTypes.Employee)
                return employeeDB.Authorize(username, password);
            else
                return customerDB.Authorize(username, password);
        }
        public static bool ChangePassword(string username, string newPassword)
        {
            // Thử đổi mật khẩu trên customerDB trước
            if (customerDB.ChangePassword(username, newPassword))
                return true;

            // Nếu không thành công, thử trên employeeDB
            return employeeDB.ChangePassword(username, newPassword);
        }
    }

    public enum UserTypes
    {
        Employee,
        Customer
    }
}
