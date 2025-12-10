using CsvHelper.Configuration;
using MISA.CRM2025.Core.DTOs;

namespace MISA.CRM2025.Core.Mappings
{
    /// <summary>
    /// Lớp ánh xạ (ClassMap) dùng cho thư viện CsvHelper.
    /// <para/>Mục đích: Xác định cách map giữa tên cột trong file CSV (định dạng snake_case)
    /// và các thuộc tính của DTO CustomerCsv (định dạng PascalCase).
    /// <para/>Ngữ cảnh sử dụng: Dùng khi export dữ liệu khách hàng ra CSV hoặc import CSV vào hệ thống,
    /// đảm bảo tính nhất quán giữa dữ liệu CSV và đối tượng trong hệ thống.
    /// <list type="bullet">
    /// <item>Map(): Dùng để ánh xạ từng thuộc tính sang tên cột CSV.</item>
    /// <item>TypeConverterOption.Format(): Dùng để định dạng cột ngày tháng khi export.</item>
    /// </list>
    /// </summary>
    public class CustomerCsvMap : ClassMap<CustomerCsv>
    {
        /// <summary>
        /// Hàm khởi tạo thiết lập toàn bộ ánh xạ giữa CustomerCsv và tên cột CSV.
        /// <para/>Ngữ cảnh sử dụng: Tự động chạy khi CsvHelper khởi tạo mapping.
        /// </summary>
        /// <remarks></remarks>
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