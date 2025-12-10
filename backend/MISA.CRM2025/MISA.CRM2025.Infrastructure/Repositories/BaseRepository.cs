using Dapper;
using Microsoft.Extensions.Configuration;
using MISA.CRM2025.Core.Interfaces.Repositories;
using MySqlConnector;
using System.Data;
using System.Text.RegularExpressions;

namespace MISA.CRM2025.Infrastructure.Repositories
{
    /// <summary>
    /// Repository cơ sở cung cấp các thao tác CRUD chung cho tất cả entity.
    /// <para/>Ngữ cảnh sử dụng: Là lớp nền tảng để các repository khác kế thừa và tái sử dụng logic làm việc với database.
    /// </summary>
    /// <typeparam name="T">Entity tương ứng với bảng trong database.</typeparam>
    /// Created by: nguyentruongan - 03/12/2025
    public class BaseRepository<T> : IBaseRepository<T>, IDisposable where T : class
    {
        /// <summary>
        /// Chuỗi kết nối database
        /// </summary>
        protected readonly string connectionString;

        /// <summary>
        /// Đối tượng kết nối database
        /// </summary>
        protected IDbConnection dbConnection;

        /// <summary>
        /// Khởi tạo repository và thiết lập kết nối đến database.
        /// <para/>Ngữ cảnh: Tự động được gọi khi khởi tạo repository kế thừa.
        /// </summary>
        /// <param name="config">Đối tượng cấu hình chứa chuỗi kết nối database.</param>
        public BaseRepository(IConfiguration config)
        {
            connectionString = config.GetConnectionString("DefaultConnection");
            dbConnection = new MySqlConnection(connectionString);
        }

        /// <summary>
        /// Chuyển đổi chuỗi PascalCase thành snake_case.
        /// <para/>Ngữ cảnh: Dùng khi ánh xạ property C# sang tên cột trong database.</para>
        /// </summary>
        /// <param name="name">Tên cần chuyển đổi.</param>
        /// <returns>Chuỗi đã được format sang snake_case.</returns>
        public string ToSnakeCase(string name)
        {
            return Regex.Replace(name, "([a-z])([A-Z])", "$1_$2").ToLower();
        }

        /// <summary>
        /// Lấy tất cả bản ghi chưa bị xóa mềm.
        /// <para/>Ngữ cảnh: Dùng khi hiển thị danh sách dữ liệu cơ bản.</para>
        /// </summary>
        /// <returns>Danh sách entity T.</returns>
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            // Lấy tên table
            var tableName = ToSnakeCase(typeof(T).Name);

            // Viết câu lệnh sql đẻ truy vấn dữ liệu
            var sql = $"SELECT * FROM {tableName} WHERE is_deleted = 0";

            return await dbConnection.QueryAsync<T>(sql);
        }

        /// <summary>
        /// Lấy một bản ghi theo Id.
        /// <para/>Ngữ cảnh: Xem chi tiết hoặc load dữ liệu trước khi update.</para>
        /// </summary>
        /// <param name="id">Khóa chính của bản ghi cần lấy.</param>
        /// <returns>Bản ghi nếu tồn tại, null nếu không tìm thấy.</returns>
        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            // Lấy tên table
            var tableName = ToSnakeCase(typeof(T).Name);

            // Viết câu lệnh sql đẻ truy vấn dữ liệu
            var sql = $"SELECT * FROM {tableName} WHERE {ToSnakeCase(typeof(T).Name + "Id")} = @Id AND is_deleted = 0";

            return await dbConnection.QuerySingleOrDefaultAsync<T>(sql, new { Id = id });
        }

        /// <summary>
        /// Thêm mới một bản ghi vào database.
        /// <para/>Ngữ cảnh: Dùng khi tạo mới entity.</para>
        /// </summary>
        /// <param name="entity">Dữ liệu entity cần thêm.</param>
        /// <returns>Số bản ghi bị ảnh hưởng (1 nếu thêm thành công).</returns>
        public virtual async Task<int> InsertAsync(T entity)
        {
            // Lấy ra các cột của table
            var tableName = ToSnakeCase(typeof(T).Name);

            // Lấy ra các property của class T
            var props = typeof(T).GetProperties();

            // Lấy ra các trường trong table
            var columns = string.Join(", ", props.Select(p => ToSnakeCase(p.Name)));

            var parameters = string.Join(", ", props.Select(p => "@" + p.Name));

            var sql = $"INSERT INTO {tableName} ({columns}) VALUES ({parameters})";

            return await dbConnection.ExecuteAsync(sql, entity);
        }

        /// <summary>
        /// Cập nhật toàn bộ thông tin một bản ghi.
        /// <para/>Ngữ cảnh: Dùng khi chỉnh sửa entity.</para>
        /// </summary>
        /// <param name="entity">Dữ liệu mới của entity.</param>
        /// <returns>Số bản ghi bị ảnh hưởng (1 nếu cập nhật thành công).</returns>
        public virtual async Task<int> UpdateAsync(T entity)
        {
            // Lấy tên table
            var tableName = ToSnakeCase(typeof(T).Name);

            // Lấy ra các property của class T
            var props = typeof(T).GetProperties();

            var setClause = string.Join(", ", props.Select(p => $"{ToSnakeCase(p.Name)}=@{p.Name}"));

            var sql = $@"UPDATE {tableName} 
                            SET {setClause} 
                            WHERE {ToSnakeCase(typeof(T).Name + "Id")} = @{typeof(T).Name + "Id"}";

            return await dbConnection.ExecuteAsync(sql, entity);
        }

        /// <summary>
        /// Xóa mềm bản ghi bằng cách đặt is_deleted = 1.
        /// <para/>Ngữ cảnh: Dùng khi xóa dữ liệu nhưng vẫn cần lưu lại lịch sử.</para>
        /// </summary>
        /// <param name="id">Id của bản ghi cần xóa.</param>
        /// <returns>Số bản ghi bị ảnh hưởng.</retur
        public virtual async Task<int> SoftDeleteAsync(Guid id)
        {
            // Lấy ra tên table
            var tableName = ToSnakeCase(typeof(T).Name);

            // Viết câu lệnh sql để truy vấn dữ liệu
            var sql = $"UPDATE {tableName} SET is_deleted = 1 WHERE {ToSnakeCase(typeof(T).Name + "Id")} = @Id";

            return await dbConnection.ExecuteAsync(sql, new { Id = id });
        }

        /// <summary>
        /// Giải phóng tài nguyên kết nối.
        /// <para/>Ngữ cảnh: Tự động được gọi khi repository bị hủy.</para>
        /// </summary>
        public void Dispose()
        {
            dbConnection?.Dispose();
        }
    }
}
