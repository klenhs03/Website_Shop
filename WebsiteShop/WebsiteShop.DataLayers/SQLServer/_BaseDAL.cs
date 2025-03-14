using Microsoft.Data.SqlClient;

namespace WebsiteShop.DataLayers.SQLServer
{
   /// <summary>
   /// Lớp cơ sở ( lớp cha) của các lớp cài đặt các phép xử lý dữ liệu trên SQL Server
   /// </summary>
    public abstract class BaseDAL
    {
        /// <summary>
        /// Chuỗi tham số kết nối đến CSDL SQL Server
        /// </summary>
        protected string connectionString = "";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public BaseDAL (string connectionString)
        {
            this.connectionString = connectionString;
        }
        /// <summary>
        /// Tạo và mở một kết nối đến CSDL (SQL Server)
        /// </summary>
        /// <returns></returns>
      protected SqlConnection OpenConnection()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }


    }
}
