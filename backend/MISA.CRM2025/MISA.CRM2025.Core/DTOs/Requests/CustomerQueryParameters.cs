using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CRM2025.Core.DTOs.Requests
{
    /// <summary>
    /// DTO nhận thông số query từ client để phân trang, tìm kiếm, lọc, sắp xếp
    /// </summary>
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
