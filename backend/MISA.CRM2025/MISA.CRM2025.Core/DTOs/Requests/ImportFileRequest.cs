using Microsoft.AspNetCore.Http;

namespace MISA.CRM2025.Core.DTOs.Requests
{
    /// <summary>
    /// DTO dùng để nhận file từ client khi import dữ liệu.
    /// <para/>Mục đích:
    /// - Nhận file (CSV, Excel, ảnh, ...) từ client để thực hiện import dữ liệu vào hệ thống.
    /// <para/>Ngữ cảnh sử dụng:
    /// - Sử dụng đẻ import các file lên hệ thống.
    /// - Dữ liệu file sẽ được validate và xử lý trong Service/Repository.
    /// </summary>
    /// Created by: nguyentruongan - 06/12/2025
    public class ImportFileRequest
    {
        /// <summary>
        /// File được gửi từ client.
        /// </summary>
        public IFormFile File { get; set; }
    }
}
