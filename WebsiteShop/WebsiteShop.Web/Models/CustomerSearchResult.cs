using WebsiteShop.DomainModels;

namespace WebsiteShop.Web.Models
{
    public class CustomerSearchResult : PaginationSearchResult
    {
        public required List<Customer> Data { get; set; }
    }
}
