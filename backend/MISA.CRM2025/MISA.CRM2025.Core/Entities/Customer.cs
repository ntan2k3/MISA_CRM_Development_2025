namespace MISA.CRM2025.Core.Entities
{
    /// <summary>
    /// Class đại diện cho thực thể Khách hàng (Customer) trong hệ thống.
    /// <para/>Mục đích:
    /// - Lưu trữ toàn bộ thông tin liên quan đến khách hàng.
    /// - Dùng làm kiểu dữ liệu truyền giữa các tầng Repository/Service/API.
    /// - Hỗ trợ import/export dữ liệu khách hàng.
    /// <para/>Ngữ cảnh sử dụng:
    /// - Thực hiện các thao tác CRUD khách hàng.
    /// - Xử lý nghiệp vụ liên quan đến phân loại, tìm kiếm, thống kê khách hàng.
    /// - Import danh sách khách hàng từ file CSV hoặc export ra CSV.
    /// </summary>
    /// Created by: nguyentruongan - 03/12/2025
    public class Customer
    {
        #region Property

        /// <summary>
        /// Khóa chính của khách hàng.
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Đường dẫn avatar khách hàng.
        /// </summary>
        public string? CustomerAvatarUrl { get; set; }

        /// <summary>
        /// Kiểu khách hàng (VD: Cá nhân, Doanh nghiệp,...)
        /// </summary>
        public string? CustomerType { get; set; }

        /// <summary>
        /// Mã khách hàng.
        /// </summary>
        public string CustomerCode { get; set; }

        /// <summary>
        /// Tên khách hàng.
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// Mã số thuế của khách hàng (nếu có).
        /// </summary>
        public string? CustomerTaxCode { get; set; }

        /// <summary>
        /// Địa chỉ giao hàng của khách hàng.
        /// </summary>
        public string? CustomerAddr { get; set; }

        /// <summary>
        /// Số điện thoại khách hàng.
        /// </summary>
        public string CustomerPhoneNumber { get; set; }

        /// <summary>
        /// Email khách hàng.
        /// </summary>
        public string CustomerEmail { get; set; }

        /// <summary>
        /// Ngày mua hàng gần nhất.
        /// </summary>
        public DateTime? LastPurchaseDate { get; set; }

        /// <summary>
        /// Mã hàng hóa đã mua gần nhất (nếu có).
        /// </summary>
        public string? PurchasedItemCode { get; set; }

        /// <summary>
        /// Tên hàng hóa đã mua gần nhất.
        /// </summary>
        public string? PurchasedItemName { get; set; }

        /// <summary>
        /// Trạng thái xóa mềm: false - chưa xóa, true - đã xóa.
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Ngày tạo bản ghi.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Người tạo bản ghi.
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Ngày chỉnh sửa gần nhất.
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Người chỉnh sửa gần nhất.
        /// </summary>
        public string? ModifiedBy { get; set; }

        #endregion
    }
}