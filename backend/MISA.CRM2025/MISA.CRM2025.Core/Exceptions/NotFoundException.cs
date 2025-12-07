using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CRM2025.Core.Exceptions
{
    /// <summary>
    /// Exception khi bản ghi không tìm thấy
    /// Trả về mã 404
    /// </summary>
    public class NotFoundException : BaseException
    {
        public NotFoundException(string message, int statusCode = 404, string? field = null) : base(message, statusCode, field)
        {

        }
    }
}
