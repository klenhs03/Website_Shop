using Dapper;
using WebsiteShop.DomainModels;

namespace WebsiteShop.DataLayers.SQLServer
{
    public class ProvinceDAL : BaseDAL, ISimpleSelectDAL<Province>
    {
        public ProvinceDAL(string connectionString) : base(connectionString)
        {
        }

        public List<Province> List()
        {
            List<Province> data = new List<Province>();
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Provinces";
                data = connection.Query<Province>(sql:sql,commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            } 
            return data;
                
        }
    }
}
