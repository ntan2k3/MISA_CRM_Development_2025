namespace MISA.CRM2025.Core.DTOs.Requests
{
    /// <summary>
    /// DTO nhận thông số query từ client để phân trang, tìm kiếm, lọc, và sắp xếp danh sách khách hàng.
    /// <para/>Mục đích:
    /// - Chuẩn hóa các tham số đầu vào từ client khi gọi API lấy danh sách khách hàng.
    /// - Hỗ trợ phân trang, tìm kiếm theo từ khóa, lọc theo loại khách hàng, và sắp xếp dữ liệu.
    /// <para/>Ngữ cảnh sử dụng:
    /// - Sử dụng trong API GetCustomers hoặc các API trả về danh sách khách hàng có phân trang.
    /// </summary>
    /// Created by: nguyentruongan - 06/12/2025
    public class CustomerQueryParameters
    {
        #region Property

        /// <summary>
        /// Trang hiện tại
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Số bản ghi trên trang
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// Từ khóa tìm kiếm (tên, số điện thoại, email)
        /// </summary>
        public string? Search { get; set; }

        /// <summary>
        /// Sắp xếp theo trường
        /// </summary>
        public string? SortBy { get; set; } 

        /// <summary>
        /// Chiều sắp xếp (Giảm dần hoặc tăng dần) 
        /// </summary>
        public string? SortDirection { get; set; }

        /// <summary>
        /// Lọc theo trường
        /// </summary>
        public string? CustomerType { get; set; }

        #endregion
    }
}
