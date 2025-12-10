using MISA.CRM2025.Core.DTOs.Requests;
using MISA.CRM2025.Core.Entities;

namespace MISA.CRM2025.Core.Interfaces.Repositories
{
    /// <summary>
    /// Repository chuyên xử lý các thao tác dữ liệu của Customer.
    /// Chứa các phương thức đặc thù ngoài CRUD cơ bản.
    /// </summary>
    /// Created by: nguyentruongan - 03/12/2025
    public interface ICustomerRepository : IBaseRepository<Customer>
    {
        /// <summary>
        /// Lấy khách hàng theo email.
        /// </summary>
        /// <param name="email">
        /// Email cần tìm kiếm.
        /// </param>
        /// <returns>
        /// Trả về Customer nếu tồn tại, null nếu không tìm thấy.
        /// </returns>
        /// Created by: nguyentruongan - 03/12/2025
        Task<Customer> GetByEmailAsync(string email);

        /// <summary>
        /// Lấy khách hàng theo số điện thoại.
        /// </summary>
        /// <param name="phoneNumber">
        /// Số điện thoại cần tìm kiếm.
        /// </param>
        /// <returns>
        /// Trả về Customer nếu tồn tại, null nếu không tìm thấy.
        /// </returns>
        /// Created by: nguyentruongan - 03/12/2025
        Task<Customer> GetByPhoneAsync(string phoneNumber);

        /// <summary>
        /// Lấy mã khách hàng lớn nhất dựa theo prefix
        /// để phục vụ sinh mã tự động.
        /// </summary>
        /// <param name="prefix">
        /// Tiền tố mã khách hàng, ví dụ: "KH202512".
        /// </param>
        /// <returns>
        /// Chuỗi mã khách hàng có giá trị lớn nhất trong DB theo prefix.
        /// </returns>
        /// Created by: nguyentruongan - 04/12/2025
        Task<string> GetMaxCustomerCodeAsync(string prefix);

        /// <summary>
        /// Lấy danh sách khách hàng theo phân trang
        /// kèm tìm kiếm, sắp xếp và lọc.
        /// </summary>
        /// <param name="query">
        /// Đối tượng chứa các tham số truy vấn:
        /// - Page: trang hiện tại  
        /// - PageSize: số bản ghi mỗi trang  
        /// - Search: từ khóa tìm kiếm  
        /// - SortBy: trường sắp xếp  
        /// - SortDirection: chiều sắp xếp (ASC/DESC)  
        /// - CustomerType: điều kiện lọc  
        /// </param>
        /// <returns>
        /// Danh sách khách hàng thuộc trang tương ứng.
        /// </returns>
        /// Created by: nguyentruongan - 04/12/2025
        Task<IEnumerable<Customer>> GetCustomersPagingAsync(CustomerQueryParameters query);

        /// <summary>
        /// Lấy tổng số bản ghi khách hàng thỏa mãn điều kiện truy vấn.
        /// </summary>
        /// <param name="query">
        /// Tham số tìm kiếm, lọc và phân trang.
        /// </param>
        /// <returns>
        /// Số lượng bản ghi phù hợp.
        /// </returns>
        /// Created by: nguyentruongan - 04/12/2025
        Task<int> GetTotalCountAsync(CustomerQueryParameters query);

        /// <summary>
        /// Lấy danh sách khách hàng theo nhiều Id.
        /// </summary>
        /// <param name="ids">
        /// Danh sách Id khách hàng.
        /// </param>
        /// <returns>
        /// Danh sách Customer tương ứng với các Id truyền vào.
        /// </returns>
        /// Created by: nguyentruongan - 04/12/2025
        Task<IEnumerable<Customer>> GetListByIdsAsync(List<Guid> ids);

        /// <summary>
        /// Xóa mềm nhiều khách hàng cùng lúc.
        /// </summary>
        /// <param name="ids">
        /// Danh sách Id cần xóa mềm.
        /// </param>
        /// <returns>
        /// Số lượng bản ghi đã xóa thành công.
        /// </returns>
        /// Created by: nguyentruongan - 05/12/2025
        Task<int> SoftDeleteManyAsync(List<Guid> ids);

        /// <summary>
        /// Gán loại khách hàng cho nhiều khách hàng cùng lúc.
        /// </summary>
        /// <param name="ids">
        /// Danh sách CustomerId cần cập nhật.
        /// </param>
        /// <param name="customerType">
        /// Loại khách hàng cần gán.
        /// </param>
        /// <returns>
        /// Số bản ghi cập nhật thành công.
        /// </returns>
        /// Created by: nguyentruongan - 06/12/2025
        Task<int> AssignCustomerTypeAsync(List<Guid> ids, string customerType);
    }
}
