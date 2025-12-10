using Microsoft.AspNetCore.Mvc;
using MISA.CRM2025.Core.DTOs.Requests;
using MISA.CRM2025.Core.DTOs.Responses;
using MISA.CRM2025.Core.Entities;
using MISA.CRM2025.Core.Interfaces.Services;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MISA.CRM2025.Api.Controllers
{
    /// <summary>
    /// Controller xử lý các yêu cầu liên quan đến khách hàng.
    /// <para/>Mục đích:
    /// - Cung cấp API CRUD khách hàng.
    /// - Hỗ trợ import/export CSV, upload ảnh, gán loại khách hàng.
    /// <para/>Ngữ cảnh sử dụng:
    /// - Dùng bởi frontend hoặc client để thao tác dữ liệu khách hàng.
    /// </summary>
    /// Created by: nguyentruongan - 06/12/2025
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        #region Helper Methods

        /// <summary>
        /// Validate dữ liệu cơ bản của CustomerRequest.
        /// <para/>Ngữ cảnh sử dụng: Trước khi thêm mới hoặc cập nhật khách hàng.
        /// </summary>
        /// <param name="customer">Dữ liệu khách hàng.</param>
        /// <returns>Danh sách lỗi nếu có, rỗng nếu dữ liệu hợp lệ.</returns>
        private List<string> ValidateCustomerRequest(CustomerRequest customer)
        {
            var errors = new List<string>();

            if (customer == null)
            {
                errors.Add("Dữ liệu khách hàng không được rỗng.");
                return errors;
            }

            if (string.IsNullOrWhiteSpace(customer.CustomerName))
                errors.Add("Họ tên không được để trống.");

            if (string.IsNullOrWhiteSpace(customer.CustomerEmail))
            {
                errors.Add("Email không được để trống.");
            }

            if (!string.IsNullOrWhiteSpace(customer.CustomerEmail))
            {
                var emailRegex = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";
                if (!Regex.IsMatch(customer.CustomerEmail, emailRegex))
                    errors.Add("Email không hợp lệ.");
            }

            if (string.IsNullOrWhiteSpace(customer.CustomerPhoneNumber))
            {
                errors.Add("Số điện thoại không được để trống.");
            }

            if (!string.IsNullOrWhiteSpace(customer.CustomerPhoneNumber))
            {
                var phoneRegex = @"^\d{10,11}$";
                if (!Regex.IsMatch(customer.CustomerPhoneNumber, phoneRegex))
                    errors.Add("Số điện thoại không hợp lệ.");
            }

            return errors;
        }

        /// <summary>
        /// Kiểm tra danh sách Guid hợp lệ.
        /// </summary>
        /// <param name="ids">Danh sách Guid</param>
        /// <returns>true nếu tất cả Guid hợp lệ, false nếu có Guid rỗng hoặc null</return
        private bool AreValidGuids(List<Guid> ids) => ids != null && ids.All(id => id != Guid.Empty);

        #endregion

        /// <summary>
        /// Lấy danh sách khách hàng theo phân trang + search + filter + sort.
        /// <para/>Ngữ cảnh sử dụng: Hiển thị danh sách khách hàng trên frontend.
        /// </summary>
        /// <param name="query">Tham số lọc và phân trang từ client</param>
        /// <returns>ApiResponse chứa danh sách khách hàng và metadata phân trang</returns>
        [HttpGet]
        public async Task<IActionResult> GetCustomers([FromQuery] CustomerQueryParameters query)
        {
            // Nếu như số trang hiện tại hoặc số bản ghi mỗi trang <= 0 thì trả lỗi
            if (query.PageNumber <= 0 || query.PageSize <= 0)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Data = null,
                    Meta = null,
                    Error = new ApiError { StatusCode = 1, Field = "PageNumber/PageSize", Message = "Số trang và kích thước bản ghi trang phải lớn hơn 0." }
                });
            }

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
        /// Lấy chi tiết khách hàng theo Id.
        /// <para/>Ngữ cảnh sử dụng: Hiển thị thông tin chi tiết khách hàng.
        /// </summary>
        /// <param name="id">Id khách hàng cần lấy</param>
        /// <returns>ApiResponse chứa thông tin khách hàng</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(Guid id)
        {
            // Nếu id rỗng trả lỗi
            if (id == Guid.Empty)

                return BadRequest(new ApiResponse<string> 
                { 
                    Data = null, 
                    Meta = null, 
                    Error = new ApiError 
                    { 
                        StatusCode = 1, 
                        Field = "id", 
                        Message = "Id khách hàng không được rỗng." 
                    } 
                });

            // Lấy ra khách hàng theo id
            var result = await _customerService.GetByIdAsync(id);

            // Trả về mã 200
            return Ok(new ApiResponse<Customer> { Data = result, Meta = null, Error = null });
        }

        /// <summary>
        /// Thêm mới khách hàng.
        /// <para/>Ngữ cảnh sử dụng: Tạo khách hàng mới từ frontend.
        /// </summary>
        /// <param name="customer">Dữ liệu khách hàng cần tạo</param>
        /// <returns>ApiResponse chứa khách hàng đã được tạo</returns>
        [HttpPost]
        public async Task<IActionResult> InsertCustomer([FromBody] CustomerRequest customer)
        {
            var errors = ValidateCustomerRequest(customer);

            // Nếu có lỗi validate thì trả lỗi
            if (errors.Any())
                return BadRequest(new ApiResponse<IEnumerable<string>> 
                { 
                    Data = errors, 
                    Meta = null, 
                    Error = new ApiError 
                    { 
                        StatusCode = 1, 
                        Field = "Customer", 
                        Message = "Dữ liệu khách hàng không hợp lệ." 
                    } 
                });

            var result = await _customerService.InsertAsync(customer, "admin");
            
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
        /// Cập nhật thông tin khách hàng.
        /// <para/>Ngữ cảnh sử dụng: Cập nhật thông tin khách hàng đã tồn tại.
        /// </summary>
        /// <param name="id">Id khách hàng cần cập nhật</param>
        /// <param name="customer">Dữ liệu cập nhật</param>
        /// <returns>ApiResponse chứa khách hàng sau khi cập nhật</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] CustomerRequest customer)
        {
            // Nếu id rỗng trả lỗi
            if (id == Guid.Empty)

                return BadRequest(new ApiResponse<string>
                {
                    Data = null,
                    Meta = null,
                    Error = new ApiError
                    {
                        StatusCode = 1,
                        Field = "id",
                        Message = "Id khách hàng không được rỗng."
                    }
                });

            var errors = ValidateCustomerRequest(customer);

            // Nếu có lỗi validate thì trả lỗi
            if (errors.Any())
                return BadRequest(new ApiResponse<IEnumerable<string>>
                {
                    Data = errors,
                    Meta = null,
                    Error = new ApiError
                    {
                        StatusCode = 1,
                        Field = "Customer",
                        Message = "Dữ liệu khách hàng không hợp lệ."
                    }
                });

            var result = await _customerService.UpdateAsync(id, customer, "admin");

            return Ok(new ApiResponse<Customer>
            {
                Data = result,
                Meta = null,
                Error = null
            });
        }

        /// <summary>
        /// Xóa mềm 1 khách hàng.
        /// <para/>Ngữ cảnh sử dụng: Khi cần xóa khách hàng mà không mất dữ liệu.
        /// </summary>
        /// <param name="id">Id khách hàng cần xóa</param>
        /// <returns>ApiResponse thông báo kết quả</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteCustomer(Guid id)
        {
            // Nếu id rỗng trả lỗi
            if (id == Guid.Empty)

                return BadRequest(new ApiResponse<string>
                {
                    Data = null,
                    Meta = null,
                    Error = new ApiError
                    {
                        StatusCode = 1,
                        Field = "id",
                        Message = "Id khách hàng không được rỗng."
                    }
                });

            // Lấy ra số bản ghi xóa thành công
            var result = await _customerService.SoftDeleteAsync(id);

            return Ok(new ApiResponse<string>
            {
                Data = "Xóa khách hàng thành công.",
                Meta = null,
                Error = null
            });
        }

        /// <summary>
        /// Xóa mềm nhiều khách hàng.
        /// <para/>Ngữ cảnh sử dụng: Khi cần xóa nhiều khách hàng cùng lúc.
        /// </summary>
        /// <param name="ids">Danh sách Id khách hàng cần xóa</param>
        /// <returns>ApiResponse thông báo kết quả</returns>
        [HttpDelete("bulk")]
        public async Task<IActionResult> SoftDeleteListCustomers([FromBody] List<Guid> ids)
        {
            // Kiểm tra danh sách khách hàng
            if (!AreValidGuids(ids))

                return BadRequest(new ApiResponse<string>
                {
                    Data = null,
                    Meta = null,
                    Error = new ApiError
                    {
                        StatusCode = 1,
                        Field = "ids",
                        Message = "Danh sách Id khách hàng không hợp lệ."
                    }
                });

            // Lấy ra số bản ghi xóa thành công
            var result = await _customerService.SoftDeleteManyAsync(ids);

            var msg = result > 0 ? $"Xóa thành công {result} khách hàng" : null;

            return Ok(new ApiResponse<string>
            {
                Data = msg,
                Meta = null,
                Error = null
            });
        }

        /// <summary>
        /// Import khách hàng từ file CSV.
        /// <para/>Ngữ cảnh sử dụng: Nhập dữ liệu khách hàng từ file CSV.
        /// </summary>
        /// <param name="request">File CSV gửi từ client</param>
        /// <returns>ApiResponse thông báo số bản ghi import thành công</returns>
        [HttpPost("import")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ImportCsvAsync([FromForm] ImportFileRequest request)
        {
            // Lấy ra file từ request
            var file = request.File;

            if (file == null)

                return BadRequest(new ApiResponse<string>
                {
                    Data = null,
                    Meta = null,
                    Error = new ApiError
                    {
                        StatusCode = 1,
                        Field = "File",
                        Message = "File CSV không được rỗng."
                    }
                });

            // Đọc file CSV gửi lên
            using var stream = file.OpenReadStream();

            // Lấy ra số bản ghi import thành công
            var count = await _customerService.ImportCsvAsync(stream);

            return Ok(new ApiResponse<int>
            {
                Data = count,
                Meta = null,
                Error = null
            });
        }

        /// <summary>
        /// Export danh sách khách hàng thành CSV theo Id.
        /// <para/>Ngữ cảnh sử dụng: Xuất dữ liệu khách hàng ra file CSV.
        /// </summary>
        /// <param name="ids">Danh sách Id khách hàng cần xuất</param>
        /// <returns>File CSV chứa thông tin khách hàng</returns>
        [HttpPost("export")]
        public async Task<IActionResult> ExportCsvAsync([FromBody] List<Guid> ids)
        {
            // Kiểm tra danh sách khách hàng
            if (!AreValidGuids(ids))

                return BadRequest(new ApiResponse<string>
                {
                    Data = null,
                    Meta = null,
                    Error = new ApiError
                    {
                        StatusCode = 1,
                        Field = "ids",
                        Message = "Danh sách Id khách hàng không hợp lệ."
                    }
                });
                
            // Lấy ra nội dung file CSV
            var bytes = await _customerService.ExportCsvAsync(ids);
            
            // Đặt tên file
            var fileName = $"customers_selected_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";

            return File(bytes, "text/csv", fileName);
        }

        /// <summary>
        /// Lấy mã khách hàng mới.
        /// <para/>Ngữ cảnh sử dụng: Sinh mã khách hàng tự động khi tạo mới.
        /// </summary>
        /// <returns>ApiResponse chứa mã khách hàng mới</returns>
        [HttpGet("new-code")]
        public async Task<IActionResult> GetNewCustomerCode()
        {
            var newCode = await _customerService.GenerateCustomerCode();

            return Ok(new ApiResponse<string>
            {
                Data = newCode,
                Meta = null,
                Error = null
            });
        }

        /// <summary>
        /// Lấy tổng số khách hàng theo bộ lọc.
        /// <para/>Ngữ cảnh sử dụng: Hiển thị tổng số khách hàng trên giao diện.
        /// </summary>
        /// <param name="query">Tham số lọc</param>
        /// <returns>ApiResponse chứa tổng số bản ghi</returns>
        [HttpGet("count")]
        public async Task<IActionResult> GetTotalCustomers([FromQuery] CustomerQueryParameters query)
        {
            var total = await _customerService.GetTotalCountAsync(query);

            return Ok(new ApiResponse<int>
            {
                Data = total,
                Meta = null,
                Error = null
            });
        }

        /// <summary>
        /// Upload ảnh tạm để preview trước khi lưu chính thức.
        /// <para/>Ngữ cảnh sử dụng: Cho phép người dùng xem trước avatar trước khi lưu.
        /// </summary>
        /// <param name="request">File ảnh gửi lên</param>
        /// <returns>ApiResponse chứa URL ảnh tạm</returns>
        [HttpPost("upload-temp-avatar")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadTempAvatar([FromForm] ImportFileRequest request)
        {
            // Lấy ra file từ request
            var file = request.File;

            if (file == null)

                return BadRequest(new ApiResponse<string>
                {
                    Data = null,
                    Meta = null,
                    Error = new ApiError
                    {
                        StatusCode = 1,
                        Field = "File",
                        Message = "File ảnh không được rỗng."
                    }
                });

            // Lấy ra đường dẫn url tạm
            var url = await _customerService.UploadTempAvatarAsync(request.File);

            return Ok(new ApiResponse<string>
            {
                Data = url,
                Meta = null,
                Error = null
            });
        }

        /// <summary>
        /// Gán loại khách hàng cho nhiều khách hàng cùng lúc.
        /// <para/>Ngữ cảnh sử dụng: Cập nhật CustomerType cho nhiều khách hàng.
        /// </summary>
        /// <param name="request">Danh sách Id khách hàng và loại khách hàng cần gán</param>
        /// <returns>ApiResponse thông báo số khách hàng được cập nhật</returns>
        [HttpPost("assign-type")]
        public async Task<IActionResult> AssignCustomerType([FromBody] AssignCustomerTypeRequest request)
        {
            if (!AreValidGuids(request?.CustomerIds))

                return BadRequest(new ApiResponse<string> 
                { 
                    Data = null, 
                    Meta = null, 
                    Error = new ApiError { 
                        StatusCode = 1, 
                        Field = "Ids", 
                        Message = "Danh sách Id khách hàng không hợp lệ." 
                    } 
                });

            if (string.IsNullOrWhiteSpace(request.CustomerType))

                return BadRequest(new ApiResponse<string> 
                { 
                    Data = null, 
                    Meta = null, 
                    Error = new ApiError 
                    { 
                        StatusCode = 1, 
                        Field = "CustomerType", 
                        Message = "Loại khách hàng không được rỗng." 
                    } 
                });

            // Gọi service để gán loại khách hàng
            var updatedCount = await _customerService.AssignCustomerTypeAsync(request.CustomerIds, request.CustomerType);

            return Ok(new ApiResponse<string>
            {
                Data = $"Cập nhật thành công {updatedCount} khách hàng sang loại '{request.CustomerType}'.",
                Meta = null,
                Error = null
            });
        }

        /// <summary>
        /// Kiểm tra email đã tồn tại hay chưa.
        /// <para/>Ngữ cảnh sử dụng: Trước khi thêm mới hoặc cập nhật khách hàng để tránh trùng email.
        /// </summary>
        /// <param name="request">Đối tượng chứa Email cần kiểm tra và Id khách hàng (nếu edit)</param>
        /// <returns>
        /// ApiResponse chứa true nếu email đã tồn tại, false nếu chưa tồn tại.
        /// Trả về BadRequest nếu email rỗng.
        /// </returns>
        /// <remarks></remarks>
        [HttpPost("check-email")]
        public async Task<IActionResult> CheckEmailExist([FromBody] CheckEmailRequest request)
        {
            // Check email có rỗng không
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new ApiResponse<string>
                {
                    Data = null,
                    Meta = null,
                    Error = new ApiError
                    {
                        StatusCode = 1,
                        Field = "email",
                        Message = "Email không được để trống."
                    }
                });
            }

            var exists = await _customerService.IsEmailExistAsync(request.Email, request.Id);

            return Ok(new ApiResponse<bool>
            {
                Data = exists,
                Meta = null,
                Error = null
            });
        }

        /// <summary>
        /// Kiểm tra số điện thoại đã tồn tại hay chưa.
        /// <para/>Ngữ cảnh sử dụng: Trước khi thêm mới hoặc cập nhật khách hàng để tránh trùng số điện thoại.
        /// </summary>
        /// <param name="request">Đối tượng chứa PhoneNumber cần kiểm tra và Id khách hàng (nếu edit)</param>
        /// <returns>
        /// ApiResponse chứa true nếu số điện thoại đã tồn tại, false nếu chưa tồn tại.
        /// Trả về BadRequest nếu số điện thoại rỗng.
        /// </returns>
        /// <remarks></remarks>
        [HttpPost("check-phone")]
        public async Task<IActionResult> CheckPhoneExist([FromBody] CheckPhoneRequest request)
        {
            // check số điện thoãi rỗng hay không
            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                return BadRequest(new ApiResponse<string>
                {
                    Data = null,
                    Meta = null,
                    Error = new ApiError
                    {
                        StatusCode = 1,
                        Field = "phoneNumber",
                        Message = "Số điện thoại không được để trống."
                    }
                });
            }

            var exists = await _customerService.IsPhoneNumberExistAsync(request.PhoneNumber, request.Id);

            return Ok(new ApiResponse<bool>
            {
                Data = exists,
                Meta = null,
                Error = null
            });
        }
    }
}
  