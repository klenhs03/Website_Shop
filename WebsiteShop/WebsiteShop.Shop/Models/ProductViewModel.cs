using WebsiteShop.DomainModels;

namespace WebsiteShop.Shop.Models
{
    public class ProductViewModel
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int RowCount { get; set; }
        public string SearchValue { get; set; } = string.Empty;
        public List<Product> Data { get; set; } = new List<Product>();
        public List<Category> Categories { get; set; } = new List<Category>();
        public PaginationSearchInput Condition { get; set; } = new PaginationSearchInput();

        public int? CategoryID { get; set; } 
        public int? SupplierID { get; set; }  
        public decimal MinPrice { get; set; } = 0;  
        public decimal MaxPrice { get; set; } = decimal.MaxValue;  
    }
}
