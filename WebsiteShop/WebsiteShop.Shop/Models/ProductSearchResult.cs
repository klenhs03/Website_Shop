using WebsiteShop.DomainModels;

namespace WebsiteShop.Shop.Models
{
    public class ProductSearchResult : PaginationSearchResult
    {
        public int CategoryID { get; set; } = 0;
        public int SupplierID { get; set; } = 0;
       
        public decimal MinPrice { get; set; } = 0;
        public decimal MaxPrice { get; set; } = 0;
        public List<Product> Data { get; set; } = new List<Product>();
        public List<Category> Categories { get; set; } = new List<Category>();
        public PaginationSearchInput Condition { get; set; } = new PaginationSearchInput();
    }
}
