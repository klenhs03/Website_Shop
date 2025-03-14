using WebsiteShop.DomainModels;

namespace WebsiteShop.Shop.Models
{
    public class HomeViewModel
    {
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Product> FeaturedProducts { get; set; } 
    }
}
