namespace WebsiteShop.Shop.Models
{
    public class OrderItemViewModel
    {
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal SalePrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
