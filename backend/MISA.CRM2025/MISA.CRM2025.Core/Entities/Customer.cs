using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CRM2025.Core.Entities
{
    /// <summary>
    /// Đại diện cho bảng customer trong database
    /// </summary>
    public class Customer
    {
        #region Property

        /// <summary>
        /// Khóa chính
        /// </summary>
        public Guid CustomerId { get; set; } 
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
        /// <summary>
        /// Trạng thái xóa mềm, 0 - Chưa xóa, 1 - Đã xóa, mặc định là 0.
        /// </summary>
        public Boolean IsDeleted { get; set; } = false;
        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// Người tạo
        /// </summary>
        public string CreatedBy { get; set; }
        /// <summary>
        /// Ngày sửa
        /// </summary>
        public DateTime? ModifiedDate { get; set; }
        /// <summary>
        /// Người sửa
        /// </summary>
        public string? ModifiedBy { get; set; }

        #endregion
    }
}
