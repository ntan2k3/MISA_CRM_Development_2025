using Dapper;
using Microsoft.Extensions.Configuration;
using MISA.CRM2025.Core.DTOs.Requests;
using MISA.CRM2025.Core.DTOs.Responses;
using MISA.CRM2025.Core.Entities;
using MISA.CRM2025.Core.Interfaces.Repositories;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CRM2025.Infrastructure.Repositories
{
    /// <summary>
    /// Repository xử lý truy vấn database cho bảng Customer.
    /// Kế thừa BaseRepository để dùng các CRUD chung,
    /// đồng thời bổ sung các truy vấn đặc thù như kiểm tra trùng Email, Phone.
    /// </summary>
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(IConfiguration config) : base(config)
        {
        }
        /// <summary>
        /// Lấy khách hàng theo email.
        /// Dùng để phục vụ validate trùng email khi thêm mới hoặc cập nhật.
        /// </summary>
        /// <param name="email">Email cần kiểm tra</param>
        /// <returns>
        /// Customer nếu tồn tại, 
        /// null nếu không tìm thấy
        /// </returns>
        public async Task<Customer> GetByEmailAsync(string email)
        {
            // // Câu lệnh SQL truy vấn khách hàng theo email
            var sql = $"SELECT * FROM customer WHERE customer_email = @CustomerEmail";

            return await dbConnection.QuerySingleOrDefaultAsync<Customer>(sql, new { CustomerEmail = email });
        }

        /// <summary>
        /// Lấy khách hàng theo số điện thoại.
        /// Dùng để phục vụ validate trùng số điện thoại.
        /// </summary>
        /// <param name="phoneNumber">Số điện thoại cần kiểm tra</param>
        /// <returns>
        /// Customer nếu tồn tại,
        /// null nếu không tìm thấy.
        /// </returns>
        public async Task<Customer> GetByPhoneAsync(string phoneNumber)
        {
            // Câu lệnh SQL truy vấn khách hàng theo số điện thoại
            var sql = $"SELECT * FROM customer WHERE customer_phone_number = @CustomerPhoneNumber";

            return await dbConnection.QuerySingleOrDefaultAsync<Customer>(sql, new { CustomerPhoneNumber = phoneNumber });

        }

        /// <summary>
        /// Lấy ra mã khách hàng lớn nhất theo prefix
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns>Mã khách hàng lớn nhất</returns>
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
        /// </summary>
        /// <param name="query">Tham số query từ FE: Search, Filter</param>
        /// <returns>Tổng số bản ghi thỏa điều kiện</returns>
        public async Task<int> GetTotalCountAsync(CustomerQueryParameters query)
        {
            // viết câu lệnh sql truy vấn đếm toàn bộ bản ghi
            var sql = "SELECT COUNT(*) FROM customer WHERE is_deleted = 0";
            
            var parameters = new DynamicParameters();

            // Search
            if (!string.IsNullOrEmpty(query.Search))
            {
                sql += " AND (customer_name LIKE @Search OR customer_email LIKE @Search OR customer_phone_number LIKE  @Search)";
                
                // Thêm tham số cho dapper
                parameters.Add("Search", $"%{query.Search}%");
            }

            // Filter
            if (!string.IsNullOrEmpty(query.CustomerType))
            {
                sql += " AND customer_type LIKE @CustomerType";

                parameters.Add("CustomerType", $"%{query.CustomerType}%");
            }

            return await dbConnection.ExecuteScalarAsync<int>(sql, parameters);
        }

        /// <summary>
        /// Lấy danh sách khách hàng có phân trang, kèm search, filter, sort.
        /// </summary>
        /// <param name="query">Tham số query từ FE: Search, Filter, Sort, Page</param>
        /// <returns>Dữ liệu khách hàng theo trang</returns>
        public async Task<IEnumerable<Customer>> GetCustomersPagingAsync(CustomerQueryParameters query)
        {
            // viết câu lệnh sql truy vấn toàn bộ bản ghi
            var sql = "SELECT * FROM customer WHERE is_deleted = 0";

            // Tạo đối tượng giúp truyền tham số vào sql
            var parameters = new DynamicParameters();

            // Search
            if (!string.IsNullOrEmpty(query.Search))
            {
                sql += " AND (customer_name LIKE @Search OR customer_email LIKE @Search OR customer_phone_number LIKE @Search)";

                //@Search là tham số của Dapper, được thay thế bằng giá trị "%{query.Search}%"
                parameters.Add("Search", $"%{query.Search}%");
            }

            // Filter
            if (!string.IsNullOrEmpty(query.CustomerType))
            {
                sql += " AND customer_type LIKE @CustomerType";

                parameters.Add("CustomerType", $"%{query.CustomerType}%");
            }

            // Sort
            if (!string.IsNullOrEmpty(query.SortBy))
                sql += $" ORDER BY {ToSnakeCase(query.SortBy)} {(query.SortDirection).ToUpper()}";
            else
                // Nếu không truyền thì mặc định sort theo ngày tạo mới nhất
                sql += " ORDER BY created_date DESC";

            // Phân trang
            // Số bản ghi cần bỏ qua để đến trang hiện tại
            var offset = (query.PageNumber - 1) * query.PageSize;

            // Lấy số bản ghi mong muốn theo trang
            sql += " LIMIT @PageSize OFFSET @Offset";

            parameters.Add("PageSize", query.PageSize);
            parameters.Add("Offset", offset);

            return await dbConnection.QueryAsync<Customer>(sql, parameters);
        }

        /// <summary>
        /// Lấy danh sách khách hàng theo id
        /// </summary>
        /// <param name="ids"></param>
        /// <returns>trả về danh sách khách hàng</returns>
        public async Task<IEnumerable<Customer>> GetListByIdsAsync(List<Guid> ids)
        {
            if (ids == null || ids.Count == 0)

                return new List<Customer>();

            // Câu lệnh truy vấn để lấy ra danh sách khách hàng có id nằm trong danh sách Ids
            var sql = "SELECT * FROM customer WHERE customer_id IN @Ids AND is_deleted = 0";

            // SQL truy vấn và trả về danh sách khách hàng
            return await dbConnection.QueryAsync<Customer>(sql, new { Ids = ids });
        }

        /// <summary>
        /// Upload avatar khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="avatarUrl"></param>
        /// <returns></returns>
        public async Task UploadAvatarAsync(Guid id, string avatarUrl)
        {
            var sql = "UPDATE customer SET customer_avatar_url = @CustomerAvatarUrl WHERE CustomerId = @Id";
            await dbConnection.ExecuteAsync(sql, new { CustomerAvatarUrl = avatarUrl, Id = id });
        }

        /// <summary>
        /// Xóa hàng loạt khách hàng
        /// </summary>
        /// <param name="ids">Danh sách id khách hàng</param>
        /// <returns></returns>
        public async Task<int> SoftDeleteManyAsync(List<Guid> ids)
        {
            if (ids == null || ids.Count == 0)
                return 0;

            var sql = "UPDATE customer SET is_deleted = 1 WHERE customer_id IN @Ids";
            var result = await dbConnection.ExecuteAsync(sql, new { Ids = ids });
            return result;
        }

        /// <summary>
        /// Gán hàng loạt loại khách hàng cho các khách hàng được chọn
        /// </summary>
        /// <param name="ids">Danh sách CustomerId cần gán</param>
        /// <param name="customerType">Loại khách hàng muốn gán</param>
        /// <returns>Số bản ghi đã cập nhật thành công</returns>
        public async Task<int> AssignCustomerTypeAsync(List<Guid> ids, string customerType)
        {
            if (ids == null || ids.Count == 0)
                return 0;

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
