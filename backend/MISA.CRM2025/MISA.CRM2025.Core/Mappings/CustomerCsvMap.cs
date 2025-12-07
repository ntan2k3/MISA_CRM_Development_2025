using CsvHelper.Configuration;
using MISA.CRM2025.Core.DTOs;
using MISA.CRM2025.Core.DTOs.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CRM2025.Core.Mappings
{
    /// <summary>
    /// ClassMap cho CsvHelper: ánh xạ giữa tên cột CSV (snake_case) và DTO CustomerCreateDto (PascalCase).
    /// Khi export/import CSV, dùng map này để consistent.
    /// </summary>
    public class CustomerCsvMap : ClassMap<CustomerCsv>
    {
        public CustomerCsvMap()
        {
            Map(m => m.CustomerType).Name("customer_type");
            Map(m => m.CustomerCode).Name("customer_code");
            Map(m => m.CustomerName).Name("customer_name");
            Map(m => m.CustomerTaxCode).Name("customer_tax_code");
            Map(m => m.CustomerAddr).Name("customer_addr");
            Map(m => m.CustomerPhoneNumber).Name("customer_phone_number");
            Map(m => m.CustomerEmail).Name("customer_email");
            Map(m => m.LastPurchaseDate).Name("last_purchase_date").TypeConverterOption.Format("dd/MM/yyyy");
            Map(m => m.PurchasedItemCode).Name("purchased_item_code");
            Map(m => m.PurchasedItemName).Name("purchased_item_name");
        }
    }
}