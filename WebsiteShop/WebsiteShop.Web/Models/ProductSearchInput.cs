namespace WebsiteShop.Web.Models
{
    public class ProductSearchInput : PaginationSearchResult
    {
        public int CategoryID { get; set; } = 0;
        public int SupplierID { get; set; } = 0;
        public decimal MinPrice { get; set; } = 0;
        public decimal MaxPrice { get; set; } = 0;
        public int CustomerID { get; set; } = 0;
        public string DeliveryAddress { get; set; } = "";

        public string Province { get; set; } = "";
    }
}
