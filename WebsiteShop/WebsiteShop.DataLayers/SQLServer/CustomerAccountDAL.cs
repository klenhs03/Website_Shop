using Dapper;
using WebsiteShop.DomainModels;

namespace WebsiteShop.DataLayers.SQLServer
{
    public class CustomerAccountDAL : BaseDAL, IUserAccountDAL
    {
        public CustomerAccountDAL(string connectionString) : base(connectionString)
        {
        }

        public UserAccount? Authorize(string username, string password)
        {
            UserAccount? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select CustomerID as UserId, 
		                            Email as UserName,
		                            CustomerName as DisplayName, 
		                            RoleNames
                            from Customers
                            where Email = @Email and Password = @Password";
                var parameters = new
                {
                    Email = username,
                    Password = password,
                };
                data = connection.QueryFirstOrDefault<UserAccount>(sql, parameters, commandType: System.Data.CommandType.Text);
            }
            return data;
        }

        public bool ChangePassword(string username, string newPassword)
        {
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE Customers 
                    SET Password = @NewPassword 
                    WHERE Email = @Email";
                var parameters = new
                {
                    Email = username,
                    NewPassword = newPassword
                };
                int rowsAffected = connection.Execute(sql, parameters, commandType: System.Data.CommandType.Text);
                return rowsAffected > 0;
            }
        }
    }
}
