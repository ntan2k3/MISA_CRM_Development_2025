using MISA.CRM2025.Core.DTOs.Requests;
using MISA.CRM2025.Core.DTOs.Responses;
using MISA.CRM2025.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CRM2025.Core.Interfaces.Repositories
{
    /// <summary>
    /// Repository riêng cho Customer
    /// Thêm các phương thức đặc thù
    /// </summary>
    /// Created by: nguyentruongan - 03/12/2025
    public interface ICustomerRepository : IBaseRepository<Customer>
    {
        /// <summary>
        /// Lấy bản ghi theo email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<Customer> GetByEmailAsync(string email);

        /// <summary>
        /// Lấy bản ghi theo số điện thoại
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        Task<Customer> GetByPhoneAsync(string phoneNumber);

        /// <summary>
        /// Lấy ra mã khách hàng lớn nhất theo prefix
        /// </summary>
        /// <param name="prefix">prefix pattern có dạng KH + yyyyMM </param>
        /// <returns>Mã khách hàng lớn nhất</returns>
        Task<string> GetMaxCustomerCodeAsync(string prefix);

        /// <summary>
        /// Lấy danh sách khách hàng theo phân trang, có hỗ trợ tìm kiếm và lọc.
        /// </summary>
        /// <param name="query">
        /// Đối tượng CustomerQueryParameters chứa các tham số truy vấn:
        /// - Page: trang hiện tại muốn lấy
        /// - PageSize: số bản ghi trên mỗi trang
        /// - Search: từ khóa tìm kiếm (tên, email, số điện thoại,...)
        /// - SortBy: sắp xép theo trường
        /// - SortDirection: sắp xếp theo chiều giảm dần hoặc tăng dần
        /// - Filter: lọc theo trường
        /// </param>
        /// <returns>
        /// Trả về một Task với tuple gồm:
        /// - Data: danh sách khách hàng theo trang yêu cầu
        /// - TotalCount: tổng số khách hàng thỏa mãn điều kiện truy vấn
        /// </returns>
        Task<IEnumerable<Customer>> GetCustomersPagingAsync(CustomerQueryParameters query);

        /// <summary>
        /// Lấy ra tổng số bản ghi theo query 
        /// </summary>
        /// <param name="query"></param>
        /// <returns>
        /// tổng số khách hàng thỏa mãn điều kiện truy vấn
        /// </returns>
        Task<int> GetTotalCountAsync(CustomerQueryParameters query);

        /// <summary>
        /// Lấy danh sách khách hàng theo id
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<IEnumerable<Customer>> GetListByIdsAsync(List<Guid> ids);

        /// <summary>
        /// Dùng để upload avatar khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="avatarUrl"></param>
        /// <returns></returns>
        Task UploadAvatarAsync(Guid id, string avatarUrl);

        /// <summary>
        /// Xóa hàng loạt khách hàng
        /// </summary>
        /// <param name="ids">Danh sách id khách hàng</param>
        /// <returns></returns>
        Task<int> SoftDeleteManyAsync(List<Guid> ids);

        /// <summary>
        /// Gán hàng loạt loại khách hàng cho các khách hàng được chọn
        /// </summary>
        /// <param name="ids">Danh sách CustomerId cần gán</param>
        /// <param name="customerTypeId">Id loại khách hàng muốn gán</param>
        /// <returns>Số lượng bản ghi đã cập nhật thành công</returns>
        /// Created by: nguyentruongan - 06/12/2025
        Task<int> AssignCustomerTypeAsync(List<Guid> ids, string customerType);
    }
}
