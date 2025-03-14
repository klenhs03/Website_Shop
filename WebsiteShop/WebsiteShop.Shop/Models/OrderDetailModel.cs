using WebsiteShop.DomainModels;

namespace WebsiteShop.Shop.Models
{
    public class OrderDetailModel
    {
        public Order? Order { get; set; }
        public required List<OrderDetail> Details { get; set; }
    }
}
