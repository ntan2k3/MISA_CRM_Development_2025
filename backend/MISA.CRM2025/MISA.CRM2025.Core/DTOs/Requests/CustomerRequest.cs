namespace MISA.CRM2025.Core.DTOs.Requests
{
    /// <summary>
    /// DTO dùng để nhận dữ liệu từ client khi tạo mới hoặc cập nhật thông tin khách hàng (Create/Update).
    /// <para/>Mục đích:
    /// - Nhận dữ liệu từ client để tạo mới hoặc cập nhật thông tin khách hàng.
    /// - Tách riêng dữ liệu gửi lên từ client với entity Customer.
    /// <para/>Ngữ cảnh sử dụng:
    /// - Sử dụng trong API rại mới, upadte.
    /// - Dữ liệu sẽ được validate trước khi chuyển xuống Service/Repository xử lý.
    /// </summary>
    /// Created by: nguyentruongan - 06/12/2025
    public class CustomerRequest
    {
        #region Property

        /// <summary>
        /// Avatar khách hàng
        /// </summary>
        public string? CustomerAvatarUrl { get; set; }
        /// <summary>
        /// Kiểu khách hàng
        /// </summary>
        public string? CustomerType { get; set; }
        /// <summary>
        /// Mã khách hàng
        /// </summary>
        public string CustomerCode { get; set; }
        /// <summary>
        /// Tên khách hàng
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// Mã số thuế
        /// </summary>
        public string? CustomerTaxCode { get; set; }
        /// <summary>
        ///  Địa chỉ (Giao hàng)
        /// </summary>
        public string? CustomerAddr { get; set; }
        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string CustomerPhoneNumber { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string CustomerEmail { get; set; }
        /// <summary>
        /// Ngày mua hàng gần nhất
        /// </summary>
        public DateTime? LastPurchaseDate { get; set; }
        /// <summary>
        /// Hàng hóa đã mua
        /// </summary>
        public string? PurchasedItemCode { get; set; }
        /// <summary>
        /// Tên hàng hóa đã mua
        /// </summary>
        public string? PurchasedItemName { get; set; }

        #endregion
    }
}
