using WebsiteShop.DomainModels;

namespace WebsiteShop.Web.Models
{
    public class CategorySearchResult : PaginationSearchResult
    {
        public required List<Category> Data { get; set; }
    }
}
