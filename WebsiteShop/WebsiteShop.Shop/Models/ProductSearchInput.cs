namespace WebsiteShop.Shop.Models
{
    public class ProductSearchInput : PaginationSearchResult
    {
        public int CategoryID { get; set; } = 0;
        public int SupplierID { get; set; } = 0;
       
        public decimal MinPrice { get; set; } = 0;
        public decimal MaxPrice { get; set; } = 0;
    
    }
}
