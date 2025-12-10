using MISA.CRM2025.Core.DTOs.Responses;
using MISA.CRM2025.Core.Exceptions;


namespace MISA.CRM2025.Api.Middlewares
    {
        /// <summary>
        /// Middleware xử lý Exception toàn cục
        /// Bắt tất cả Exception ném ra từ Controller/Service
        /// Trả về JSON: { message, field, status }
        /// </summary>
        public class ExceptionMiddleware
        {
            /// <summary>
            /// Delegate đại diện cho bước tiếp theo trong pipeline middleware.
            /// </summary>
            private readonly RequestDelegate _next;

            public ExceptionMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            /// <summary>
            /// Hàm chính xử lý mỗi request khi middleware được kích hoạt.
            /// </summary>
            public async Task InvokeAsync(HttpContext context)
            {
                try
                {
                    // Tiếp tục chạy sang middleware tiếp theo hoặc vào Controller.
                    await _next(context);
                }
                catch (BaseException ex)
                {
                    // Xử lý lỗi custom của hệ thống (Validation, Conflict, NotFound...)
                    context.Response.StatusCode = ex.StatusCode;

                    // Trả về JSON
                    context.Response.ContentType = "application/json";

                    var response = new ApiResponse<object>
                    {
                        Data = null,
                        Meta = null,
                        Error = new ApiError
                        {
                            StatusCode = ex.StatusCode,
                            Field = ex.Field,
                            Message = ex.Message
                        }
                    };

                    await context.Response.WriteAsJsonAsync(response);
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi hệ thống: lỗi không lường trước 
                    context.Response.StatusCode = 500;

                    // Trả về JSON
                    context.Response.ContentType = "application/json";

                    var response = new ApiResponse<object>
                    {
                        Data = null,
                        Meta = null,
                        Error = new ApiError
                        {
                            StatusCode = 500,
                            Field = "",
                            Message = "Lỗi hệ thống"
                        }
                    };

                    await context.Response.WriteAsJsonAsync(response);
                }
            }
        }
    }
