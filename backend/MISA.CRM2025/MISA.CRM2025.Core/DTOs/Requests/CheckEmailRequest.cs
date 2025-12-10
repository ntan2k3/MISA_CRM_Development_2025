using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CRM2025.Core.DTOs.Requests
{
    /// <summary>
    /// DTO dùng để truyền dữ liệu kiểm tra email tồn tại hay chưa.
    /// <para/>Ngữ cảnh sử dụng: Gọi API kiểm tra trước khi thêm mới hoặc cập nhật khách hàng.
    /// </summary>
    /// <remarks>
    /// Created by: nguyentruongan - 06/12/2025
    /// </remarks>
    public class CheckEmailRequest
    {
        #region Properties

        /// <summary>
        /// Email cần kiểm tra.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Id khách hàng hiện tại nếu đang chỉnh sửa, null nếu thêm mới.
        /// </summary>
        public Guid? Id { get; set; }

        #endregion
    }
}
