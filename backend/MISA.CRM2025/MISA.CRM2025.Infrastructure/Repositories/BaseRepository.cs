using Microsoft.Extensions.Configuration;
using MISA.CRM2025.Core.Interfaces.Repositories;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dapper;

namespace MISA.CRM2025.Infrastructure.Repositories
{
    /// <summary>
    /// Base repository chứa các phương thức CRUD cơ bản
    /// Generic T → tái sử dụng cho mọi bảng.
    /// </summary>
    /// <typeparam name="T">Entity</typeparam>
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
        /// Lấy chuỗi kết nối
        /// Kết nối đến database
        /// </summary>
        /// <param name="config"></param>
        public BaseRepository(IConfiguration config)
        {
            connectionString = config.GetConnectionString("DefaultConnection");
            dbConnection = new MySqlConnection(connectionString);
        }

        /// <summary>
        /// Hàm chuyển đổi từ PascalCase sang snake_case
        /// </summary>
        /// <param name="name"></param>
        /// <returns>name dạng snake_case</returns>
        public string ToSnakeCase(string name)
        {
            return Regex.Replace(name, "([a-z])([A-Z])", "$1_$2").ToLower();
        }
        /// <summary>
        /// Lấy tất cả bản ghi (trừ các bản ghi xóa mềm)
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            // Lấy tên table
            var tableName = ToSnakeCase(typeof(T).Name);

            // Viết câu lệnh sql đẻ truy vấn dữ liệu
            var sql = $"SELECT * FROM {tableName} WHERE is_deleted = 0";

            return await dbConnection.QueryAsync<T>(sql);
        }

        /// <summary>
        /// Lấy bản ghi theo id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            // Lấy tên table
            var tableName = ToSnakeCase(typeof(T).Name);

            // Viết câu lệnh sql đẻ truy vấn dữ liệu
            var sql = $"SELECT * FROM {tableName} WHERE {ToSnakeCase(typeof(T).Name + "Id")} = @Id AND is_deleted = 0";

            return await dbConnection.QuerySingleOrDefaultAsync<T>(sql, new { Id = id });
        }

        /// <summary>
        /// Thêm mới bản ghi vào database
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
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
        /// Cập nhật bản ghi 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
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
        /// Xóa mềm bản ghi
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<int> SoftDeleteAsync(Guid id)
        {
            // Lấy ra tên table
            var tableName = ToSnakeCase(typeof(T).Name);

            // Viết câu lệnh sql để truy vấn dữ liệu
            var sql = $"UPDATE {tableName} SET is_deleted = 1 WHERE {ToSnakeCase(typeof(T).Name + "Id")} = @Id";

            return await dbConnection.ExecuteAsync(sql, new { Id = id });
        }

        /// <summary>
        /// Giải phóng tài nguyên kết nối database
        /// </summary>
        public void Dispose()
        {
            dbConnection?.Dispose();
        }
    }
}
