using Dapper;

using WebsiteShop.DataLayers;
using WebsiteShop.DataLayers.SQLServer;
using WebsiteShop.DomainModels;
using System.Data;

namespace WebsiteShop.DataLayers.SQLServer
{
    public class ProductDAL : BaseDAL, IProductDAL
    {
        public ProductDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Product data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"insert into Products(ProductName,ProductDescription,SupplierID,CategoryID,Unit,Price,Photo,IsSelling)
                                    values(@ProductName,@ProductDescription,@SupplierID,@CategoryID,@Unit,@Price,@Photo,@IsSelling);
                                    select @@identity;
                                ";
                var parameters = new
                {
                    ProductName = data.ProductName ?? "",
                    ProductDescription = data.ProductDescription ?? "",
                    SupplierID = data.SupplierID,
                    CategoryID = data.CategoryID,
                    Unit = data.Unit ?? "",
                    Price = data.Price,
                    Photo = data.Photo,
                    IsSelling = data.IsSelling
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public long AddAttribute(ProductAttribute data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"insert into ProductAttributes(ProductID,AttributeName,AttributeValue,DisplayOrder)
                                    values(@ProductID,@AttributeName,@AttributeValue,@DisplayOrder);
                                    select @@identity;
                                ";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    AttributeName = data.AttributeName ?? "",
                    AttributeValue = data.AttributeValue ?? "",
                    DisplayOrder = data.DisplayOrder
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public long AddPhoto(ProductPhoto data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"insert into ProductPhotos(ProductID,Photo,Description,DisplayOrder,IsHidden)
                                    values(@ProductID,@Photo,@Description,@DisplayOrder,@IsHidden);
                                    select @@identity;
                                ";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    Photo = data.Photo ?? "",
                    DisplayOrder = data.DisplayOrder,
                    Description = data.Description ?? "",
                    IsHidden = data.IsHidden
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            int count = 0;
            if (!string.IsNullOrEmpty(searchValue))
            { searchValue = "%" + searchValue + "%"; }
            using (var connection = OpenConnection())
            {
                var sql = @"
                            SELECT COUNT(*)
                            FROM Products
                            WHERE (@searchValue = N'' OR ProductName LIKE @searchValue)
                            AND (@categoryID = 0 OR CategoryID = @categoryID)
                            AND (@supplierID = 0 OR SupplierID = @supplierID)
                            AND (@minPrice <= 0 OR Price >= @minPrice)
                            AND (@maxPrice <= 0 OR Price <= @maxPrice)";
                var parameters = new
                {
                    searchValue = searchValue,
                    categoryID = categoryID,
                    supplierID = supplierID,
                    minPrice = minPrice,
                    maxPrice = maxPrice
                };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return count;
        }

        public bool Delete(int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from Products where ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = productID
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool DeleteAttribute(long attributeID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductAttributes where AttributeID = @AttributeID";
                var parameters = new
                {
                    AttributeID = attributeID
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool DeletePhoto(long photoID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductPhotos where PhotoID = @PhotoID";
                var parameters = new
                {
                    PhotoID = photoID
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Product? Get(int productID)
        {
            Product? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Products where ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = productID
                };
                data = connection.QueryFirstOrDefault<Product>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public IList<ProductDetail> ListDetails(int productID)
        {
            List<ProductDetail> list = new List<ProductDetail>();
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT p.ProductID, p.ProductName, p.Photo, p.Unit, p.ProductDescription, p.Price
                    FROM Products as p
                    WHERE p.ProductID = @ProductID";
                var parameters = new { ProductID = productID };
                list = connection.Query<ProductDetail>(sql:sql, param: parameters, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            return list;
        }


        public ProductDetail? GetDescription(int productID)
        {
            ProductDetail? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select p.ProductID, p.ProductName, p.Photo, p.Unit, p.Price, p.ProductDescription
                            from Products as p
                            where p.ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = productID
                };
                data = connection.QueryFirstOrDefault<ProductDetail>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }   
            return data;
        }

        public ProductAttribute? GetAttribute(long attributeID)
        {
            ProductAttribute? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductAttributes where AttributeID = @AttributeID";
                var parameters = new
                {
                    AttributeID = attributeID
                };
                data = connection.QueryFirstOrDefault<ProductAttribute>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public ProductPhoto? GetPhoto(long photoID)
        {
            ProductPhoto? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductPhotos where PhotoId = @PhotoID";
                var parameters = new
                {
                    PhotoID = photoID
                };
                data = connection.QueryFirstOrDefault<ProductPhoto>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool IsUsed(int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from OrderDetails where ProductID = @ProductID)
                                select 1
                            else 
                                select 0";
                var parameters = new
                {
                    ProductID = productID
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public IList<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            List<Product> data;
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT *
                                FROM (
                                SELECT *,
                                ROW_NUMBER() OVER(ORDER BY ProductName) AS RowNumber
                                FROM Products
                                WHERE (@SearchValue = N'' OR ProductName LIKE @SearchValue)
                                AND (@CategoryID = 0 OR CategoryID = @CategoryID)
                                AND (@SupplierID = 0 OR SupplierId = @SupplierID)
                                AND (Price >= @MinPrice)
                                AND (@MaxPrice <= 0 OR Price <= @MaxPrice)
                                ) AS t
                                WHERE (@PageSize = 0)
                                OR (RowNumber BETWEEN (@Page - 1)*@PageSize + 1 AND @Page * @PageSize)";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue,
                    CategoryID = categoryID,
                    SupplierID = supplierID,
                    minPrice = minPrice,
                    maxPrice = maxPrice
                };
                data = connection.Query<Product>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public IList<ProductAttribute> ListAttributes(int productID)
        {
            List<ProductAttribute> data;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductAttributes where ProductID = @ProductID order by DisplayOrder asc";
                var parameters = new
                {
                    ProductID = productID
                };
                data = (connection.Query<ProductAttribute>(sql: sql, param: parameters, commandType: CommandType.Text)).ToList();
                connection.Close();
            }
            return data;
        }

        public IList<ProductAttribute> ListAttributes()
        {
            List<ProductAttribute> data;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductAttributes";
                data = (connection.Query<ProductAttribute>(sql: sql, commandType: CommandType.Text)).ToList();
                connection.Close();
            }
            return data;
        }

        public IList<ProductPhoto> ListPhotos(int productID)
        {
            List<ProductPhoto> data;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductPhotos where ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = productID
                };
                data = (connection.Query<ProductPhoto>(sql: sql, param: parameters, commandType: CommandType.Text)).ToList();
                connection.Close();
            }
            return data;
        }

        public IList<ProductPhoto> ListPhotos()
        {
            List<ProductPhoto> data;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductPhotos";
                data = (connection.Query<ProductPhoto>(sql: sql, commandType: CommandType.Text)).ToList();
                connection.Close();
            }
            return data;
        }

        public bool Update(Product data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {

                var sql = @"
                                   update Products 
                                   set ProductName = @ProductName,
                                       ProductDescription = @ProductDescription,
                                       SupplierId = @SupplierID,
                                       CategoryId = @CategoryID,
                                       Unit = @Unit,
                                       Price = @Price,
                                       Photo = @Photo,
                                       IsSelling = @IsSelling
                                    where ProductID = @ProductID
                                ";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    ProductName = data.ProductName ?? "",
                    ProductDescription = data.ProductDescription ?? "",
                    SupplierID = data.SupplierID,
                    CategoryID = data.CategoryID,
                    Unit = data.Unit ?? "",
                    Price = data.Price,
                    Photo = data.Photo,
                    IsSelling = data.IsSelling
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool UpdateAttribute(ProductAttribute data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @" Update ProductAttributes
                                  set 
                                    ProductID = @ProductID,
                                    AttributeName = @AttributeName,
                                    AttributeValue = @AttributeValue,
                                    DisplayOrder = @DisplayOrder
                             Where AttributeID  = @AttributeID";
                var parameters = new
                {
                    AttributeID = data.AttributeID,
                    ProductID = data.ProductID,
                    AttributeName = data.AttributeName ?? "",
                    AttributeValue = data.AttributeValue ?? "",
                    DisplayOrder = data.DisplayOrder,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool UpdatePhoto(ProductPhoto data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {

                var sql = @" Update ProductPhotos
                             set 
                                ProductID = @ProductID,
                                Photo= @Photo,
                                Description = @Description,
                                DisplayOrder = @DisplayOrder,
                                IsHidden = @IsHidden
                            Where PhotoID  = @PhotoID";
                var parameters = new
                {
                    PhotoID = data.PhotoID,
                    ProductID = data.ProductID,
                    Photo = data.Photo ?? "",
                    DisplayOrder = data.DisplayOrder,
                    Description = data.Description ?? "",
                    IsHidden = data.IsHidden,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
