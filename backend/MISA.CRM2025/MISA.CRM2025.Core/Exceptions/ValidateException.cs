using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CRM2025.Core.Exceptions
{
    public class ValidateException : BaseException
    {
        public ValidateException(string message, int statusCode = 400, string? field = null) : base(message, statusCode, field)
        {
        }
    }
}
