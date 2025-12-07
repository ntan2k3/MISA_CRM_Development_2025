using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CRM2025.Core.DTOs.Requests
{
    /// <summary>
    /// Request để gán hàng loạt loại khách hàng
    /// </summary>
    public class AssignCustomerTypeRequest
    {
        /// <summary>
        /// Danh sách CustomerId cần gán
        /// </summary>
        public List<Guid> CustomerIds { get; set; }

        /// <summary>
        /// Loại khách hàng muốn gán
        /// </summary>
        public string CustomerType { get; set; }
    }
}
