namespace MISA.CRM2025.Core.DTOs
{
    /// <summary>
    /// Lớp DTO dùng để ánh xạ dữ liệu khách hàng từ file CSV.
    /// <para/>Mục đích: Lấy dữ liệu từ file CSV và chuyển đổi thành đối tượng CustomerCsv để xử lý nhập khẩu (import) hoặc xuất khẩu (export) dữ liệu khách hàng trong hệ thống CRM.
    /// <para/>Ngữ cảnh sử dụng: 
    /// <list type="bullet">
    /// <item>Import: Sử dụng khi import danh sách khách hàng từ file CSV lên hệ thống.</item>
    /// <item>Export: Sử dụng khi xuất dữ liệu khách hàng từ hệ thống ra file CSV.</item>
    /// </list>
    /// </summary>
    /// Created by nguyentruongan - 05/12/2025
    public class CustomerCsv
    {
        #region Property

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
