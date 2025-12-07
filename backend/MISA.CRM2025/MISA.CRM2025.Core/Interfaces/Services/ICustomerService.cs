using Microsoft.AspNetCore.Http;
using MISA.CRM2025.Core.DTOs.Requests;
using MISA.CRM2025.Core.DTOs.Responses;
using MISA.CRM2025.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CRM2025.Core.Interfaces.Services
{
    /// <summary>
    /// Service riêng cho Customer
    /// Thêm các phương thức xử lý nghiệp vụ đặc thù
    /// </summary>
    /// Created by: nguyentruongan - 03/12/2025
    public interface ICustomerService : IBaseService<Customer, CustomerRequest>
    {
        /// <summary>
        /// Check định dạng email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        /// Created by: nguyentruongan - 04/12/2025
        bool IsValidEmail(string email);

        /// <summary>
        /// Check định dạng số điện thoại
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        /// Created by: nguyentruongan - 04/12/2025
        bool IsValidPhone(string phone);

        /// <summary>
        /// Validate dữ liệu khi Insert/Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns>Trả về các lỗi</returns>
        /// Created by: nguyentruongan - 04/12/2025
        Task ValidateCustomerAsync(Guid? id, CustomerRequest dto);

        /// <summary>
        /// Dùng để tự động sinh mã khách hàng
        /// </summary>
        /// <returns>Mã khách hàng</returns>
        /// Created by: nguyentruongan - 04/12/2025
        Task<string> GenerateCustomerCode();

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
        /// </returns>
        /// Created by: nguyentruongan - 04/12/2025
        Task<IEnumerable<Customer>> GetCustomersPagingAsync(CustomerQueryParameters query);

        /// <summary>
        /// Lấy ra tổng số bản ghi theo query 
        /// </summary>
        /// <param name="query"></param>
        /// <returns>
        /// tổng số khách hàng thỏa mãn điều kiện truy vấn
        /// </returns>
        /// Created by: nguyentruongan - 04/12/2025
        Task<int> GetTotalCountAsync(CustomerQueryParameters query);

        /// <summary>
        /// Export theo danh sách khách hàng được chọn
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// Created by: nguyentruongan - 04/12/2025
        Task<byte[]> ExportCsvAsync(List<Guid> ids);

        /// <summary>
        /// Nhập file csv
        /// </summary>
        /// <param name="csvStream"></param>
        /// <returns></returns>
        /// Created by: nguyentruongan - 04/12/2025
        Task<int> ImportCsvAsync(Stream csvStream);

        /// <summary>
        /// Upload ảnh tạm để preview
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        Task<string> UploadTempAvatarAsync(IFormFile file);

        /// <summary>
        /// Xóa hàng loạt khách hàng
        /// </summary>
        /// <param name="ids"></param>
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
