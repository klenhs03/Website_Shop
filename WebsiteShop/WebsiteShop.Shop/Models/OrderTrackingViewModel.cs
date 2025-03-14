namespace WebsiteShop.Shop.Models
{
    public class OrderTrackingViewModel
    {
        public int OrderID { get; set; }
        public DateTime OrderTime { get; set; }
        public int Status { get; set; }
        public string DeliveryProvince { get; set; }
        public string DeliveryAddress { get; set; }
        public List<OrderItemViewModel> OrderDetails { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
