using Dapper;
using WebsiteShop.DomainModels;

namespace WebsiteShop.DataLayers.SQLServer
{
    public class EmployeeAccountDAL : BaseDAL, IUserAccountDAL
    {
        public EmployeeAccountDAL(string connectionString) : base(connectionString)
        {
        }

        public UserAccount? Authorize(string username, string password)
        {
            UserAccount? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select EmployeeID as UserId, 
		                            Email as UserName,
		                            FullName as DisplayName, 
		                            Photo, 
		                            RoleNames
                            from Employees
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
                var sql = @"UPDATE Employees 
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
