using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MISA.CRM2025.Core.DTOs.Requests;
using MISA.CRM2025.Core.DTOs.Responses;
using MISA.CRM2025.Core.Entities;
using MISA.CRM2025.Core.Exceptions;
using MISA.CRM2025.Core.Interfaces.Services;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MISA.CRM2025.Api.Controllers
{
    // Định nghĩa route gốc cho controller: /api/v1/customers
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Lấy toàn bộ dữ liệu theo query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCustomers([FromQuery] CustomerQueryParameters query)
        {
            // Lấy toàn bộ dữ liệu theo query
            var data = await _customerService.GetCustomersPagingAsync(query);

            // Lấy ra toàn bộ bản ghi
            var total = await _customerService.GetTotalCountAsync(query);

            // Trả về response theo định dạng ApiResponse
            return Ok(new ApiResponse<IEnumerable<Customer>>
            {
                Data = data,
                Meta = new MetaData
                {
                    Page = query.PageNumber,
                    PageSize = query.PageSize,
                    Total = total
                },
                Error = null
            });

        }

        /// <summary>
        /// API GET: Lấy khách hàng theo Id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(Guid id)
        {
            // Lấy ra khách hàng theo id
            var result = await _customerService.GetByIdAsync(id);

            // Thành công thì trả về mã 200
            return Ok(new ApiResponse<Customer> { Data = result, Meta = null, Error = null });
        }

        /// <summary>
        /// API POST: Thêm mới khách hàng
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> InsertCustomer([FromBody] CustomerRequest customer)
        {
            // Tạm thời hardcore người tạo
            var createdBy = "admin";

            var result = await _customerService.InsertAsync(customer, createdBy);
            
            // Tạo thành công trả về mã 201 và dữ liệu
            return CreatedAtAction(nameof(GetCustomerById), new { id = result.CustomerId },
                new ApiResponse<Customer>
                {
                    Data = result,
                    Meta = null,
                    Error = null
                });
        }

        /// <summary>
        /// API PUT: Cập nhật khách hàng theo Id
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] CustomerRequest customer)
        {
            // Tạm thời hardcore người sửa
            var modifiedBy = "admin";

            var result = await _customerService.UpdateAsync(id, customer, modifiedBy);

            return Ok(new ApiResponse<Customer>
            {
                Data = result,
                Meta = null,
                Error = null
            });
        }

        /// <summary>
        /// API DELETE: Xóa mềm khách hàng
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteCustomer(Guid id)
        {
            await _customerService.SoftDeleteAsync(id);

            return Ok(new ApiResponse<string>
            {
                Data = $"Xóa thành công khách hàng {id}",
                Meta = null,
                Error = null
            });
        }

        /// <summary>
        /// Xóa hàng loạt khách hàng
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete("bulk")]
        public async Task<IActionResult> SoftDeleteListCustomers([FromBody] List<Guid> ids)
        {
            if (ids == null || !ids.Any())

                return BadRequest(new ApiResponse<string>
                {
                    Data = null,
                    Meta = null,
                    Error = new ApiError
                    {
                        StatusCode = 400,
                        Field = "CustomerIds",
                        Message = "Không có dữ liệu đê xóa."
                    }
                });

            var result = await _customerService.SoftDeleteManyAsync(ids);

            return Ok(new ApiResponse<string>
            {
                Data = $"Xóa thành công {result} khách hàng",
                Meta = null,
                Error = null
            });
        }

        /// <summary>
        /// Import file csv
        /// </summary>
        /// <returns></returns>
        [HttpPost("import")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ImportCsvAsync([FromForm] ImportFileRequest request)
        {
            var file = request.File;

            if (file == null || file.Length == 0)

                return BadRequest(new ApiResponse<string>
                {
                    Data = null,
                    Meta = null,
                    Error = new ApiError
                    {
                        StatusCode = 400,
                        Field = "file",
                        Message = "File không hợp lệ."
                    }
                });

            // Đọc file CSV gửi lên
            using var stream = request.File.OpenReadStream();

            // Lấy ra số bản ghi import thành công
            var count = await _customerService.ImportCsvAsync(stream);

            return Ok(new ApiResponse<string>
            {
                Data = $"Import thành công {count} khách hàng",
                Meta = null,
                Error = null
            });
        }

        /// <summary>
        /// Export file csv
        /// </summary>
        /// <returns></returns>
        [HttpPost("export")]
        public async Task<IActionResult> ExportCsvAsync([FromBody] List<Guid> ids)
        {
            if (ids == null || ids.Count == 0)

                return BadRequest(new ApiResponse<string>
                {
                    Data = null,
                    Meta = null,
                    Error =
                    new ApiError
                    {
                        StatusCode = 400,
                        Field = "customerIds",
                        Message = "Không có bản ghi nào được chọn"
                    }
                });

            // Lấy ra nội dung file CSV
            var bytes = await _customerService.ExportCsvAsync(ids);
            
            // Đặt tên file
            var fileName = $"customers_selected_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";

            return File(bytes, "text/csv", fileName);
        }

        /// <summary>
        /// Lấy xuống mã khách hàng mới nhất
        /// </summary>
        /// <returns></returns>
        [HttpGet("new-code")]
        public async Task<IActionResult> GetNewCustomerCode()
        {
            var newCode = await _customerService.GenerateCustomerCode();

            return Ok(newCode);
        }

        /// <summary>
        /// Lấy tổng số bản ghi theo query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("count")]
        public async Task<IActionResult> GetTotalCustomers([FromQuery] CustomerQueryParameters query)
        {
            var total = await _customerService.GetTotalCountAsync(query);
            return Ok(total);
        }

        /// <summary>
        /// Lấy ảnh avatar
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet("{id}/avatar")]
        public async Task<IActionResult> GetAvatar(Guid id)
        {
            // Lấy ra khách hàng theo id 
            var customer = await _customerService.GetByIdAsync(id);

            // Nếu không thấy thì trả về mã 404 
            if (customer == null)

                return NotFound(new ApiResponse<string>
                {
                    Data = null,
                    Meta = null,
                    Error = new ApiError
                    {
                        StatusCode = 404,
                        Field = "CustomerId",
                        Message = "Không tìm thấy khách hàng"
                    }
                });

            // Nếu CustomerAvatarUrl không tồn tại hoặc rỗng, trả về chuỗi rỗng
            if (string.IsNullOrEmpty(customer.CustomerAvatarUrl))
            {
                return Ok(new ApiResponse<string>
                {
                    Data = "", 
                    Meta = null,
                    Error = null
                });
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", customer.CustomerAvatarUrl.TrimStart('/'));

            // Nếu avatar không tồn tại trên server thì trả về cuỗi rỗng
            if (!System.IO.File.Exists(filePath))

                return Ok(new ApiResponse<string>
                {
                    Data = "", // hoặc null
                    Meta = null,
                    Error = null
                });

            // Xác định MIME type dựa vào phần mở rộng file để trả về đúng định dạng
            var mimeType = "image/" + Path.GetExtension(filePath).TrimStart('.');

            // Đọc file avatar từ hệ thống thành mảng byte
            var bytes = System.IO.File.ReadAllBytes(filePath);

            // Client sẽ nhận được file và có thể hiển thị trực tiếp
            return File(bytes, mimeType);
        }

        /// <summary>
        /// Upload ảnh tạm để preview
        /// </summary>
        [HttpPost("upload-temp-avatar")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadTempAvatar([FromForm] ImportFileRequest request)
        {
            var url = await _customerService.UploadTempAvatarAsync(request.File);

            return Ok(new { tempAvatarUrl = url });
        }

        /// <summary>
        /// Gán hàng loạt loại khách hàng cho các khách hàng được chọn
        /// </summary>
        /// <param name="request">Chứa danh sách CustomerId và loại khách hàng muốn gán</param>
        /// <returns>Số lượng khách hàng được cập nhật</returns>
        [HttpPost("assign-type")]

        public async Task<IActionResult> AssignCustomerType([FromBody] AssignCustomerTypeRequest request)
        {
            // Gọi service để gán loại khách hàng
            var updatedCount = await _customerService.AssignCustomerTypeAsync(request.CustomerIds, request.CustomerType);

            return Ok(new ApiResponse<string>
            {
                Data = $"Cập nhật thành công {updatedCount} khách hàng sang loại '{request.CustomerType}'.",
                Meta = null,
                Error = null
            });
        }
    }
}
  