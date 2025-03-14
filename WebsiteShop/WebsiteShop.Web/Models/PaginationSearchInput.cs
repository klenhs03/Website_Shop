namespace WebsiteShop.Web.Models
{/// <summary>
/// luu giu cac thong tin dau vao su dung cho chuc nang tim kiem va hien thi du lieu duoi dang phan trang
/// </summary>
    public class PaginationSearchInput
    {
        /// <summary>
        /// trang can hien thi
        /// </summary>
        public int Page { get; set; } = 1;
        /// <summary>
        /// So dong hien thi tren moi trang
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Chuoi gia tri can tim kiem
        /// </summary>
        public string SearchValue { get; set; } = "";
        public int CategoryID { get; set; } = 0;
        public int SupplierID { get; set; } = 0;
        public decimal MinPrice { get; set; } = 0;
        public decimal MaxPrice { get; set; } = 0;
    }
}
