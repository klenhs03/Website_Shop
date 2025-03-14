namespace WebsiteShop.DomainModels
{
    public class ProductDetail
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = "";
        public string ProductDescription { get; set; } = "";
        public string Photo { get; set; } = "";
        public decimal Price { get; set; }
    }
}
