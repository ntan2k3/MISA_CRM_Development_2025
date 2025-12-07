using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CRM2025.Core.DTOs.Requests
{
    /// <summary>
    /// DTO dùng cho dữ liệu gửi từ client lên (Create / Update).
    /// </summary>
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
