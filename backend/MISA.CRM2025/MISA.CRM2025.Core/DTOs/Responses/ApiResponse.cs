using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CRM2025.Core.DTOs.Responses
{
    /// <summary>
    /// Lớp dùng để chuẩn hóa dữ liệu trả về cho API.
    /// tất cả API đều trả về theo 1 format:
    /// {
    ///   data: ...,
    ///   meta: {
    ///        page,
    ///        pageSize,
    ///        total
    ///        }
    ///   error: ...
    /// }
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// Created by: nguyentruongan - 05/12/2025
    public class ApiResponse<T>
    {
        #region Property

        /// <summary>
        /// Dữ liệu chính trả về từ API (kết quả query, kết quả xử lý...)
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Thông tin đi kèm, thường dùng cho phân trang (page, size, total)
        /// </summary>
        public MetaData? Meta { get; set; }

        /// <summary>
        /// Lỗi trả về (nếu có). Khi có lỗi → Data = null, Meta = null
        /// </summary>
        public ApiError? Error { get; set; }

        #endregion
    }

    /// <summary>
    /// Định dạng các lỗi trả về
    /// </summary>
    public class ApiError
    {
        #region Property

        /// <summary>
        /// Mã lỗi
        /// </summary>
        public int StatusCode { get; set; }  

        /// <summary>
        /// Tên trường bị lỗi
        /// </summary>
        public string? Field { get; set; }

        /// <summary>
        /// Thông báo lỗi hiển thị người dùng
        /// </summary>
        public string Message { get; set; }

        #endregion
    }

    /// <summary>
    /// Định dạng metadata
    /// </summary>
    public class MetaData
    {
        #region Property

        /// <summary>
        /// Số trang hiện tại
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Số bản ghi mỗi trang
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Tổng số bản ghi
        /// </summary>
        public int Total { get; set; }

        #endregion
    }
}
