using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CRM2025.Core.Exceptions
{
    /// <summary>
    /// Exception khi xảy ra xung đột dữ liệu (ví dụ trùng key)
    /// Trả về HTTP 409
    /// </summary>
    public class ConflictException : BaseException
    {
        public ConflictException(string message, int statusCode = 409, string? field = null) : base(message, statusCode, field)
        {
        }
    }
}
