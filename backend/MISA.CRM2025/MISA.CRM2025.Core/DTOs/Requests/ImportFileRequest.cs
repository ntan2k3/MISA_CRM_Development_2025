using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CRM2025.Core.DTOs.Requests
{
    public class ImportFileRequest
    {
        public IFormFile File { get; set; }
    }
}
