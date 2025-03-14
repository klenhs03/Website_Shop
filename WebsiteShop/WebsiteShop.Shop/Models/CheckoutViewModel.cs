using WebsiteShop.DomainModels;

namespace WebsiteShop.Shop.Models
{
    public class CheckoutViewModel
    {
        internal string deliveryAddress;

        public Customer Customer { get; set; }
        public List<CartItem> CartItems { get; set; }
        public decimal CartSubtotal { get; set; }
        public decimal CartTotal { get; set; }

        public string DisplayName { get; set; }  // Tên người dùng
        public string Phone { get; set; }  // Số điện thoại
        public string Province { get; set; }
        public string Email { get; set; }
        public string DeliveryAddress { get; set; }  // Đổi tên thành PascalCase
        public List<Province> Provinces { get; set; }

        // Constructor khởi tạo mặc định để tránh lỗi null
        public CheckoutViewModel()
        {
            CartItems = new List<CartItem>();
            Provinces = new List<Province>();
        }
    }
}
