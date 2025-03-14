using Azure;
using WebsiteShop.BusinessLayers;
using WebsiteShop.DataLayers.SQLServer;
using WebsiteShop.DataLayers;
using WebsiteShop.DomainModels;
using System.Buffers;

namespace WebsiteShop.BusinessLayers
{
    public static class ProductDataService
    {

        private static readonly IProductDAL productDB;
        /// <summary>
        /// Ctor
        /// </summary>
        static ProductDataService()
        {
            productDB = new ProductDAL(Configuration.ConnectionString);
        }
        /// <summary>
        /// Tìm kiếm và lấy danh sách mặt hàng (không phân trang)
        /// </summary>
        public static List<Product> ListOfProducts(string searchValalue = "")
        {
            return productDB.List().ToList();
        }
        /// <summary>
        /// TÌm kiếm và lấy danh sách mặt hàng dưới dạng phân trang
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <param name="categoryId"></param>
        /// <param name="supplierId"></param>
        /// <param name="minPrice"></param>
        /// <param name="maxPrice"></param>
        /// <returns></returns>
        public static List<Product> ListProducts(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "", int CategoryID = 0, int SupplierID = 0, decimal MinPrice = 0, decimal MaxPrice = 0)
        {
            rowCount = productDB.Count(searchValue, CategoryID, SupplierID, MinPrice, MaxPrice);
            return productDB.List(page, pageSize, searchValue, CategoryID, SupplierID, MinPrice, MaxPrice).ToList();
        }
        /// <summary>
        /// Lấy thông tin 1 mặt hàng theo mã mặt hàng
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        public static Product? GetProduct(int ProductId)
        {
            return productDB.Get(ProductId);
        }

        /// <summary>
        /// Lấy danh sách chi tiết sản phẩm dựa trên ProductID.
        /// </summary>
        /// <param name="productID">ID của sản phẩm cần lấy chi tiết.</param>
        /// <returns>Danh sách chi tiết sản phẩm.</returns>
        public static List<ProductDetail> ListProductDetails(int productID)
        {
            return productDB.ListDetails(productID).ToList();
        }


        /// <summary>
        /// Lấy chi tiết 1 mặt hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static ProductDetail? GetProductDescription(int productID)
        {
            return productDB.GetDescription(productID);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public static int AddProduct(Product product)
        {
            return productDB.Add(product);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public static bool UpdateProduct(Product product)
        {
            return productDB.Update(product);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public static bool DeleteProduct(int productID)
        {
            if (productDB.IsUsed(productID))
                return false;
            return productDB.Delete(productID);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public static bool IsUsedProduct(int productID)
        {
            return productDB.IsUsed(productID);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public static List<ProductPhoto> ListPhotos(int productID)
        {
            return productDB.ListPhotos(productID).ToList();
        }
        public static List<ProductPhoto> ListPhoto()
        {
            return productDB.ListPhotos().ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public static ProductPhoto? GetPhoto(long photoID)
        {
            return productDB.GetPhoto(photoID);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public static long AddPhoto(ProductPhoto data)
        {
            return productDB.AddPhoto(data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public static bool UpdatePhoto(ProductPhoto data)
        {
            return productDB.UpdatePhoto(data);
        }
        /// <summary>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public static bool DeletePhoto(long photoID)
        {
            return productDB.DeletePhoto(photoID);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public static List<ProductAttribute> ListAttributes()
        {
            return productDB.ListAttributes().ToList();
        }

        public static List<ProductAttribute> ListAttributes(int productID)
        {
            return productDB.ListAttributes(productID).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public static ProductAttribute? GetAttribute(int attributeID)
        {
            return productDB.GetAttribute(attributeID);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public static long AddAttribute(ProductAttribute data)
        {
            return productDB.AddAttribute(data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public static bool UpdateAttribute(ProductAttribute data)
        {
            return productDB.UpdateAttribute(data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public static bool DeleteAttribute(long attributeID)
        {
            return productDB.DeleteAttribute(attributeID);
        }

        public static List<Product> GetProductsByCategory(out int rowCount, int categoryId)
        {
            // Sử dụng phương thức ListProducts có sẵn để lọc sản phẩm theo categoryId
            return ListProducts(out rowCount, 1, 0, "", categoryId, 0, 0, 0);
        }


    }
}
