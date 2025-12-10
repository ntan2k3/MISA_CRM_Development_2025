namespace MISA.CRM2025.Core.DTOs.Responses
{
    /// <summary>
    /// Lớp dùng để chuẩn hóa dữ liệu trả về cho API.
    /// <para/>Mục đích:
    /// - Tất cả API trả về theo một định dạng chuẩn gồm Data, Meta, Error.
    /// - Giúp frontend dễ dàng xử lý dữ liệu trả về và lỗi.
    /// <para/>Ngữ cảnh sử dụng:
    /// - Dùng làm chuẩn response cho tất cả API trong hệ thống.
    /// - Dễ dàng phân trang, báo lỗi hoặc trả dữ liệu.
    /// <para/>Định dạng chuẩn:
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
    /// <typeparam name="T">Kiểu dữ liệu trả về trong Data</typeparam>
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
    /// Định dạng các lỗi trả về từ API.
    /// <para/>Mục đích: Chuẩn hóa thông tin lỗi để frontend hiển thị.
    /// <para/>Ngữ cảnh sử dụng: Khi API trả lỗi liên quan đến request hoặc xử lý dữ liệu.
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
    /// Định dạng metadata cho API response.
    /// <para/>Mục đích: Cung cấp thông tin phân trang cho frontend.
    /// <para/>Ngữ cảnh sử dụng: Khi API trả danh sách có phân trang.
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
