using Microsoft.AspNetCore.Http;
using MISA.CRM2025.Core.DTOs.Requests;
using MISA.CRM2025.Core.Entities;

namespace MISA.CRM2025.Core.Interfaces.Services
{
    /// <summary>
    /// Service nghiệp vụ dành riêng cho thực thể Customer.
    /// <para/>Mục đích: Định nghĩa các hành vi xử lý nghiệp vụ đặc thù cho khách hàng,
    /// mở rộng từ BaseService.
    /// <para/>Ngữ cảnh sử dụng: Tầng Business Logic, được gọi từ CustomerController
    /// hoặc các service khác khi thao tác với khách hàng.
    /// <list type="bullet">
    /// <item>GenerateCustomerCode(): Tự động sinh mã khách hàng.</item>
    /// <item>GetCustomersPagingAsync(): Lấy danh sách khách hàng có phân trang + tìm kiếm.</item>
    /// <item>ExportCsvAsync(): Xuất danh sách khách hàng ra CSV.</item>
    /// <item>ImportCsvAsync(): Nhập dữ liệu từ file CSV.</item>
    /// <item>IsEmailExistAsync(): Kiểm tra trùng email.</item>
    /// <item>IsPhoneNumberExistAsync(): Kiểm tra trùng số điện thoại.</item>
    /// </list>
    /// </summary>
    public interface ICustomerService : IBaseService<Customer, CustomerRequest>
    {
        /// <summary>
        /// Sinh mã khách hàng tự động theo quy tắc.
        /// <para/>Ngữ cảnh sử dụng: Khi tạo mới khách hàng, hệ thống cần sinh mã
        /// định danh có format chuẩn (ví dụ: KH202512000123).
        /// </summary>
        /// <returns>Mã khách hàng mới dưới dạng chuỗi.</returns>
        Task<string> GenerateCustomerCode();

        /// <summary>
        /// Lấy danh sách khách hàng có phân trang, tìm kiếm và sắp xếp.
        /// <para/>Ngữ cảnh sử dụng: Trang danh sách khách hàng, lọc dữ liệu, phân trang.</para>
        /// </summary>
        /// <param name="query">
        /// Tham số truy vấn gồm:
        /// - Page: Trang hiện tại.
        /// - PageSize: Số bản ghi mỗi trang.
        /// - Search: Từ khóa tìm kiếm.
        /// - SortBy: Trường sắp xếp.
        /// - SortDirection: Chiều sắp xếp (ASC/DESC).
        /// - CustomerType: Lọc theo loại khách hàng.
        /// </param>
        /// <returns>Danh sách khách hàng phù hợp theo trang.</returns>
        Task<IEnumerable<Customer>> GetCustomersPagingAsync(CustomerQueryParameters query);

        /// <summary>
        /// Lấy tổng số bản ghi theo điều kiện truy vấn.
        /// <para/>Ngữ cảnh sử dụng: Tính tổng số trang cho phân trang.</para>
        /// </summary>
        /// <param name="query">Tham số truy vấn cùng format với paging.</param>
        /// <returns>Tổng số bản ghi thỏa mãn điều kiện.</returns>
        Task<int> GetTotalCountAsync(CustomerQueryParameters query);

        /// <summary>
        /// Export danh sách khách hàng ra file CSV dựa vào danh sách Id.
        /// <para/>Ngữ cảnh sử dụng: Tính năng Export CSV trên UI.</para>
        /// </summary>
        /// <param name="ids">Danh sách Id khách hàng được chọn.</param>
        /// <returns>File CSV dạng mảng byte.</returns>
        Task<byte[]> ExportCsvAsync(List<Guid> ids);

        /// <summary>
        /// Import dữ liệu khách hàng từ file CSV.
        /// <para/>Ngữ cảnh sử dụng: Upload file CSV trên UI để import dữ liệu hàng loạt.</para>
        /// </summary>
        /// <param name="csvStream">Stream của file CSV upload từ client.</param>
        /// <returns>Số lượng bản ghi được import thành công.</returns>
        Task<int> ImportCsvAsync(Stream csvStream);

        /// <summary>
        /// Upload ảnh tạm để preview trước khi lưu chính thức.
        /// <para/>Ngữ cảnh sử dụng: Khi người dùng chọn ảnh đại diện trong form thông tin khách hàng.</para>
        /// </summary>
        /// <param name="file">File ảnh được upload.</param>
        /// <returns>Đường dẫn ảnh tạm thời.</returns>
        Task<string> UploadTempAvatarAsync(IFormFile file);

        /// <summary>
        /// Xóa mềm nhiều khách hàng cùng lúc.
        /// <para/>Ngữ cảnh sử dụng: Chức năng xóa hàng loạt trong bảng danh sách.</para>
        /// </summary>
        /// <param name="ids">Danh sách Id khách hàng cần xóa.</param>
        /// <returns>Số bản ghi đã xóa.</returns>
        Task<int> SoftDeleteManyAsync(List<Guid> ids);

        /// <summary>
        /// Gán loại khách hàng cho nhiều khách hàng được chọn.
        /// <para/>Ngữ cảnh sử dụng: Thao tác bulk update trên giao diện danh sách.</para>
        /// </summary>
        /// <param name="ids">Danh sách Id khách hàng.</param>
        /// <param name="customerType">Loại khách hàng cần gán.</param>
        /// <returns>Số bản ghi cập nhật thành công.</returns>
        Task<int> AssignCustomerTypeAsync(List<Guid> ids, string customerType);

        /// <summary>
        /// Kiểm tra email có bị trùng hay không.
        /// <para/>Ngữ cảnh sử dụng: Validate trước khi thêm/sửa khách hàng.</para>
        /// </summary>
        /// <param name="email">Email cần kiểm tra.</param>
        /// <param name="id">Id khách hàng đang sửa (null nếu thêm mới).</param>
        /// <returns>True nếu trùng, False nếu hợp lệ.</returns>
        Task<bool> IsEmailExistAsync(string email, Guid? id = null);

        /// <summary>
        /// Kiểm tra số điện thoại có bị trùng hay không.
        /// <para/>Ngữ cảnh sử dụng: Validate trước khi thêm/sửa khách hàng.</para>
        /// </summary>
        /// <param name="phoneNumber">Số điện thoại cần kiểm tra.</param>
        /// <param name="id">Id khách hàng đang sửa (null nếu thêm mới).</param>
        /// <returns>True nếu trùng, False nếu hợp lệ.</returns>
        Task<bool> IsPhoneNumberExistAsync(string phoneNumber, Guid? id = null);
    }
}
