using System;

namespace MISA.CRM2025.Core.DTOs.Requests
{
    /// <summary>
    /// DTO dùng để truyền dữ liệu kiểm tra số điện thoại đã tồn tại hay chưa.
    /// <para/>Ngữ cảnh sử dụng: Gọi API kiểm tra trước khi thêm mới hoặc cập nhật khách hàng.
    /// </summary>
    /// <remarks>
    /// Created by: nguyentruongan - 06/12/2025
    /// </remarks>
    public class CheckPhoneRequest
    {
        #region Properties

        /// <summary>
        /// Số điện thoại cần kiểm tra.
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Id khách hàng hiện tại nếu đang chỉnh sửa, null nếu thêm mới.
        /// </summary>
        public Guid? Id { get; set; }

        #endregion
    }
}
