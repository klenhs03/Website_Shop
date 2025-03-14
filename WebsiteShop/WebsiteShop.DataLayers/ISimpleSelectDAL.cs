namespace WebsiteShop.DataLayers
{
    public interface ISimpleSelectDAL<T> where T : class
    {
        /// <summary>
        /// Select toanf bộ dữ liệu trong một bảng
        /// </summary>
        /// <returns></returns>
        List<T> List();
    }
}

