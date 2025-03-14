using WebsiteShop.DomainModels;

namespace WebsiteShop.Web.Models
{
    public class SupplierSearchResult : PaginationSearchResult
    {
        public required List<Supplier> Data { get; set; }
    }
}
