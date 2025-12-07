using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CRM2025.Core.Interfaces.Repositories
{
    /// <summary>
    /// Interface base cho Repository.
    /// Định nghĩa các chức năng CRUD dùng chung.
    /// </summary>
    /// <typeparam name="T">Entity</typeparam>
    /// Created by: nguyentruongan - 03/12/2025
    public interface IBaseRepository<T>
    {
        /// <summary>
        /// Lấy toàn bộ bản ghi
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Lấy bản ghi theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> GetByIdAsync(Guid id);

        /// <summary>
        /// Thêm bản ghi
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        Task<int> InsertAsync(T entity);

        /// <summary>
        /// Sửa bản ghi
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        Task<int> UpdateAsync(T entity);

        /// <summary>
        /// Xóa mềm bản ghi
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        Task<int> SoftDeleteAsync(Guid id);
    }
}
