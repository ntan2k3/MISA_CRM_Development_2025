using Dapper;
using Microsoft.Extensions.Configuration;
using MISA.CRM2025.Core.DTOs.Requests;
using MISA.CRM2025.Core.Entities;
using MISA.CRM2025.Core.Interfaces.Repositories;

namespace MISA.CRM2025.Infrastructure.Repositories
{
    /// <summary>
    /// Repository xử lý truy vấn database cho bảng Customer.
    /// <para/>Mục đích:
    /// - Kế thừa BaseRepository để dùng các CRUD chung.
    /// - Bổ sung các truy vấn đặc thù như kiểm tra trùng Email, Phone, phân trang, lọc, sắp xếp.
    /// <para/>Ngữ cảnh sử dụng:
    /// - Dùng trong Service thao tác dữ liệu khách hàng.
    /// </summary>
    /// Created by: nguyentruongan - 06/12/2025
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(IConfiguration config) : base(config)
        {
        }

        /// <summary>
        /// Lấy khách hàng theo email.
        /// <para/>Ngữ cảnh sử dụng: Validate trùng email khi thêm mới hoặc cập nhật.
        /// </summary>
        /// <param name="email">Email cần kiểm tra</param>
        /// <returns>Customer nếu tồn tại, null nếu không tìm thấy</returns>
        public async Task<Customer> GetByEmailAsync(string email)
        {
            // // Câu lệnh SQL truy vấn khách hàng theo email
            var sql = $"SELECT * FROM customer WHERE customer_email = @CustomerEmail";

            return await dbConnection.QuerySingleOrDefaultAsync<Customer>(sql, new { CustomerEmail = email });
        }

        /// <summary>
        /// Lấy khách hàng theo số điện thoại.
        /// <para/>Ngữ cảnh sử dụng: Validate trùng số điện thoại khi thêm mới hoặc cập nhật.
        /// </summary>
        /// <param name="phoneNumber">Số điện thoại cần kiểm tra</param>
        /// <returns>Customer nếu tồn tại, null nếu không tìm thấy</returns>
        public async Task<Customer> GetByPhoneAsync(string phoneNumber)
        {
            // Câu lệnh SQL truy vấn khách hàng theo số điện thoại
            var sql = $"SELECT * FROM customer WHERE customer_phone_number = @CustomerPhoneNumber";

            return await dbConnection.QuerySingleOrDefaultAsync<Customer>(sql, new { CustomerPhoneNumber = phoneNumber });

        }

        /// <summary>
        /// Lấy ra mã khách hàng lớn nhất theo prefix.
        /// <para/>Ngữ cảnh sử dụng: Tự động sinh mã khách hàng mới.
        /// </summary>
        /// <param name="prefix">Prefix của mã khách hàng</param>
        /// <returns>Mã khách hàng lớn nhất hiện tại có prefix tương ứng</returns>
        public async Task<string> GetMaxCustomerCodeAsync(string prefix)
        {
            // Viết câu lênh truy vấn lấy ra mã khách hàng lớn nhất
            // Lấy ra cột customer_code từ bảng customer
            // Sau đó so khớp chuỗi customer_code với PrefixPattern
            // Sắp xếp các chuỗi đã khớp theo thứ tự giảm dần
            // Lấy ra giá trị duy nhất và là giá trị lớn nhất
            var sql = @"SELECT customer_code 
                        FROM customer
                        WHERE customer_code LIKE @PrefixPattern
                        ORDER BY customer_code DESC
                        LIMIT 1;
                        ";

            return await dbConnection.QueryFirstOrDefaultAsync<string>(sql, new { PrefixPattern = prefix + "%" });
        }

        /// <summary>
        /// Lấy tổng số lượng khách hàng theo điều kiện search + filter.
        /// <para/>Ngữ cảnh sử dụng: Phục vụ tính toán phân trang trên frontend.
        /// </summary>
        /// <param name="query">Tham số query từ FE: Search, Filter</param>
        /// <returns>Tổng số bản ghi thỏa điều kiện</returns>
        public async Task<int> GetTotalCountAsync(CustomerQueryParameters query)
        {
            // Tạo SqlBuilder để xây dựng SQL động
            var builder = new SqlBuilder();

            // Template câu SQL, /**where**/ sẽ được SqlBuilder thay thế bằng các điều kiện
            var template = builder.AddTemplate("SELECT COUNT(*) FROM customer /**where**/");

            // Điều kiện lấy ra những bản ghi chưa xáo
            builder.Where("is_deleted = 0");

            // Search: tìm kiếm theo tên, email, số điện thoại
            if (!string.IsNullOrEmpty(query.Search))
            {
                builder.Where("(customer_name LIKE @Search OR customer_email LIKE @Search OR customer_phone_number LIKE @Search)",
                              new { Search = $"%{query.Search}%" });
            }

            // Filter: lọc theo loại khách hàng
            if (!string.IsNullOrEmpty(query.CustomerType))
            {
                builder.Where("customer_type LIKE @CustomerType", new { CustomerType = $"%{query.CustomerType}%" });
            }

            // Thực hiện truy vấn với Dapper
            return await dbConnection.ExecuteScalarAsync<int>(template.RawSql, template.Parameters);

        }

        /// <summary>
        /// Lấy danh sách khách hàng có phân trang, kèm search, filter, sort.
        /// <para/>Ngữ cảnh sử dụng: Hiển thị danh sách khách hàng theo yêu cầu frontend.
        /// </summary>
        /// <param name="query">Tham số query từ FE: Search, Filter, Sort, Page</param>
        /// <returns>Dữ liệu khách hàng theo trang</returns>

        public async Task<IEnumerable<Customer>> GetCustomersPagingAsync(CustomerQueryParameters query)
        {
            // Tạo SqlBuilder để xây dựng SQL động
            var builder = new SqlBuilder();

            // Template câu SQL, /**where**/ cho điều kiện, /**orderby**/ cho sort
            var template = builder.AddTemplate("SELECT * FROM customer /**where**/ /**orderby**/ LIMIT @PageSize OFFSET @Offset");

            // Điều kiện lấy ra những bản ghi chưa xáo
            builder.Where("is_deleted = 0");

            // Search
            if (!string.IsNullOrEmpty(query.Search))
            {
                builder.Where("(customer_name LIKE @Search OR customer_email LIKE @Search OR customer_phone_number LIKE @Search)",
                              new { Search = $"%{query.Search}%" });
            }

            // Filter
            if (!string.IsNullOrEmpty(query.CustomerType))
            {
                builder.Where("customer_type LIKE @CustomerType", new { CustomerType = $"%{query.CustomerType}%" });
            }

            // Sort: các cột có thể sắp xếp và và chiều sắp xếp để tránh bị tấn công sql
            var allowedSortColumns = new List<string> { "customer_type", "customer_code" ,"customer_name", "customer_tax_code", "customer_email", "customer_phone_number", "customer_addr", "last_purchased_date", "purchased_item_code", "purchased_item_name", "created_date" };
            
            var sortColumn = "created_date"; // mặc định sort theo ngày tạo
            
            var sortDirection = "DESC";       // mặc định xếp theo chiều giảm dần

            // Nếu người dùng truyền cột sort hợp lệ
            if (!string.IsNullOrEmpty(query.SortBy) && allowedSortColumns.Contains(ToSnakeCase(query.SortBy)))

                sortColumn = ToSnakeCase(query.SortBy); // chuyển về dạng snake_case nếu người dùng truyền lên PascalCase.

            // Nếu người dùng truyền chiều sort hợp lệ
            if (!string.IsNullOrEmpty(query.SortDirection) &&
                (query.SortDirection.Equals("ASC", System.StringComparison.OrdinalIgnoreCase) ||
                 query.SortDirection.Equals("DESC", System.StringComparison.OrdinalIgnoreCase)))
            {
                sortDirection = query.SortDirection.ToUpper(); // Chuyển về dạng viết hoa.
            }

            // Thêm ORDER BY bằng phương thức OrderBy() của SqlBuilder
            builder.OrderBy($"{sortColumn} {sortDirection}");

            // Paging
            var offset = (query.PageNumber - 1) * query.PageSize;

            // Kết hợp tham số paging và các tham số trong SqlBuilder
            var parameters = new DynamicParameters(template.Parameters);
            parameters.Add("PageSize", query.PageSize);
            parameters.Add("Offset", offset);

            // Thực hiện truy vấn với Dapper
            return await dbConnection.QueryAsync<Customer>(template.RawSql, parameters);
        }

        /// <summary>
        /// Lấy danh sách khách hàng theo id.
        /// <para/>Ngữ cảnh sử dụng: Khi cần lấy nhiều khách hàng theo danh sách id cụ thể để có thể xuất CSV, xóa hàng loạt, gán loại khách hàng,....
        /// </summary>
        /// <param name="ids">Danh sách CustomerId</param>
        /// <returns>Danh sách khách hàng thỏa điều kiện</returns>
        public async Task<IEnumerable<Customer>> GetListByIdsAsync(List<Guid> ids)
        {
            // Câu lệnh truy vấn để lấy ra danh sách khách hàng có id nằm trong danh sách Ids
            var sql = "SELECT * FROM customer WHERE customer_id IN @Ids AND is_deleted = 0";

            // SQL truy vấn và trả về danh sách khách hàng
            return await dbConnection.QueryAsync<Customer>(sql, new { Ids = ids });
        }

        /// <summary>
        /// Xóa hàng loạt khách hàng (Soft Delete).
        /// <para/>Ngữ cảnh sử dụng: Khi cần xóa nhiều khách hàng cùng lúc.
        /// </summary>
        /// <param name="ids">Danh sách CustomerId cần xóa</param>
        /// <returns>Số bản ghi đã bị xóa mềm</returns>
        public async Task<int> SoftDeleteManyAsync(List<Guid> ids)
        {
            var sql = "UPDATE customer SET is_deleted = 1 WHERE customer_id IN @Ids";

            var result = await dbConnection.ExecuteAsync(sql, new { Ids = ids });
            
            // Trả về số bản ghi bị xóa
            return result;
        }

        /// <summary>
        /// Gán hàng loạt loại khách hàng cho các khách hàng được chọn.
        /// <para/>Ngữ cảnh sử dụng: Khi cần cập nhật CustomerType cho nhiều khách hàng cùng lúc.
        /// </summary>
        /// <param name="ids">Danh sách CustomerId cần gán</param>
        /// <param name="customerType">Loại khách hàng muốn gán</param>
        /// <returns>Số bản ghi đã cập nhật thành công</returns>
        public async Task<int> AssignCustomerTypeAsync(List<Guid> ids, string customerType)
        {
            var sql = @"
                        UPDATE customer
                        SET customer_type = @CustomerType
                        WHERE customer_id IN @Ids
                        AND is_deleted = 0
                        ";

            var parameters = new { CustomerType = customerType, Ids = ids };

            var result = await dbConnection.ExecuteAsync(sql, parameters);

            return result;
        }
    }
}
