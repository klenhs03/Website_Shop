using WebsiteShop.DomainModels;

namespace WebsiteShop.Web.Models
{
    public class ShipperSearchResult : PaginationSearchResult
    {
        public required List<Shipper> Data { get; set; }
    }
}
