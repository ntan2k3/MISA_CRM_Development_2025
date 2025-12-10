namespace MISA.CRM2025.Core.Exceptions
{
    /// <summary>
    /// Base Exception dùng chung cho tất cả custom exceptions
    /// Giúp lưu status code + message + tên field (nếu cần)
    /// </summary>
    public class BaseException : Exception
    {
        /// <summary>
        /// Mã lỗi 
        /// </summary>
        public int StatusCode { get; }

        /// <summary>
        /// Tên trường gây lỗi (nếu lỗi liên quan tới dữ liệu)
        /// </summary>
        public string? Field { get; }

        protected BaseException(string message, int statusCode, string? field = null)
            : base(message)
        {
            StatusCode = statusCode;
            Field = field;
        }
    }
}
