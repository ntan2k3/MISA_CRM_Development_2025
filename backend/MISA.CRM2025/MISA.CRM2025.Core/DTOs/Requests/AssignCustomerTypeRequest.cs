namespace MISA.CRM2025.Core.DTOs.Requests
{
    /// <summary>
    /// Lớp request dùng để gán loại khách hàng cho nhiều khách hàng cùng lúc.
    /// <para/>Mục đích: Chuyển danh sách khách hàng sang một loại khách hàng cụ thể trong hệ thống.
    /// <para/>Ngữ cảnh sử dụng: 
    /// - Sử dụng trong các API cập nhật loại khách hàng hàng loạt.
    /// </summary>
    /// Created by: nguyentruongan - 06/12/2025
    public class AssignCustomerTypeRequest
    {
        /// <summary>
        /// Danh sách CustomerId cần gán
        /// </summary>
        public List<Guid> CustomerIds { get; set; }

        /// <summary>
        /// Loại khách hàng muốn gán
        /// </summary>
        public string CustomerType { get; set; }
    }
}
