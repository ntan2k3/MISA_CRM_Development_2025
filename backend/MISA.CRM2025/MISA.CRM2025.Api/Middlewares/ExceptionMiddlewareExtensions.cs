namespace MISA.CRM2025.Api.Middlewares
{
    /// <summary>
    /// Class mở rộng (extension) giúp đăng ký ExceptionMiddleware 
    /// vào pipeline một cách ngắn gọn và rõ ràng.
    /// </summary>
    public static class ExceptionMiddlewareExtensions
    {
        /// <summary>
        /// Thêm Custom Exception Middleware vào pipeline xử lý request.
        /// </summary>
        public static IApplicationBuilder UseCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            // Đăng ký ExceptionMiddleware vào request pipeline.
            return app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
