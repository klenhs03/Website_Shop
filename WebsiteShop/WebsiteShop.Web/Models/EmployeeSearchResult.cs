using WebsiteShop.DomainModels;

namespace WebsiteShop.Web.Models
{
    public class EmployeeSearchResult : PaginationSearchResult
    {
        public required List<Employee> Data { get; set; }
    }
}
