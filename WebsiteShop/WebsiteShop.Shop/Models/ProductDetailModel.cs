using WebsiteShop.DomainModels;

namespace WebsiteShop.Shop.Models
{
    public class ProductDetailModel
    {
        public Product? Product { get; set; }
        public string? ProductDescription { get; set; }

        public required List<ProductDetail> Details { get; set; }

        // Constructor để khởi tạo Details
        public ProductDetailModel()
        {
            Details = new List<ProductDetail>();  // Khởi tạo danh sách
        }
    }
}
