namespace WebsiteShop.DataLayers
{
    /// <summary>
    /// Định nghĩa các phép xử lí dữ liệu thường dùng trên bảng (Customers, Employees,Shipper,...)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICommonDAL<T> where T : class
    {
        /// <summary>
        /// Tìm kiếm và lấy danh sách dữ liệu( Kiểu là T) dưới dạng có phân trang
        /// </summary>
        /// <param name="page"> Trang cần hiển thị</param>
        /// <param name="pageSize">Số dòng được hiển thị trên mỗi Trang( bằng 0 nếu không phân trang)</param>
        /// <param name="searchValue"> Giá trị cần tìm kiếm ( chuỗi rỗng nếu lấy toàn bộ dữ liệu )</param>
        /// <returns></returns>
        List<T> List(int page = 1, int pageSize = 0, string searchValue = "");
        /// <summary>
        /// Đếm số lượng dòng dữ liệu tìm kiếm được
        /// </summary>
        /// <param name="searchValue"> Giá trị cần tìm kiếm ( chuỗi rỗng nếu đếm trên toàn bộ dữ liệu)</param>
        /// <returns></returns>
        int Count(string searchValue = "");
        /// <summary>
        /// Lấy một bản ghi dữ liệu dựa vào khoá chính/ id ( trả về null nếu dữ liệu không tồn tại)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T? Get(int id);




        /// <summary>
        /// Bổ sung một bản ghi vào CSDL, Hàm trả về ID của dữ liệu vừa bổ sung ( nếu có)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int Add(T data);
        /// <summary>
        /// cập nhật một bản ghi dữ liệu
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Update(T data);
        /// <summary>
        /// Xoá một bản ghi dữ liệu dựa vào giá trị của khoá chính/id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Delete(int id);
        /// <summary>
        /// Kiểm tra xem một bản ghi dữ liệu có khoá là id hiện đang có dữ liệu tham chiếu ở bảng khác hay không? 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool InUsed(int id);
    }
}
