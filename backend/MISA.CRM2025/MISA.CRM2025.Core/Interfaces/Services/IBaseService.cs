using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CRM2025.Core.Interfaces.Services
{
    /// <summary>
    /// Interface base cho service
    /// Bao gồm 2 generic:
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TCreateDto"></typeparam>
    /// Created by: nguyentruongan - 03/12/2025
    public interface IBaseService<T, TCreateDto> 
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
        /// THêm bản ghi
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="createdBy"></param>
        /// <returns></returns>
        Task<T> InsertAsync(TCreateDto dto, string createdBy);

        /// <summary>
        /// Sửa bản ghi
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <param name="modifiedBy"></param>
        /// <returns></returns>
        Task<T> UpdateAsync(Guid id, TCreateDto dto, string modifiedBy);

        /// <summary>
        /// Xóa mềm
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> SoftDeleteAsync(Guid id);
    }
}
