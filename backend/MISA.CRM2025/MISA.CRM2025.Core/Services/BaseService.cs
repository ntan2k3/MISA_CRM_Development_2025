using MISA.CRM2025.Core.Exceptions;
using MISA.CRM2025.Core.Interfaces.Repositories;
using MISA.CRM2025.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CRM2025.Core.Services
{
    /// <summary>
    /// Chứa các hành vi CRUD cơ bản, phần dùng chung sẽ implement ở đây.
    /// Phần nghiệp vụ đặc thù thì override hoặc bổ sung tại service con.
    /// 
    /// T: Entity tương ứng với bảng trong database
    /// TCreateDto: DTO dùng cho thao tác tạo mới / cập nhật
    /// </summary>
    /// <typeparam name="T">Entity</typeparam>
    /// <typeparam name="TCreateDto">DTO dùng cho Insert/Update</typeparam>
    /// Created By: nguyentruongan - 04/12/2025
    public abstract class BaseService<T, TCreateDto> : IBaseService<T, TCreateDto>
    {
        /// <summary>
        /// Repository tương ứng với entity T.
        /// Dùng để truy cập database 
        /// </summary>
        protected readonly IBaseRepository<T> _repo;

        /// <summary>
        /// Inject repository thông qua constructor.
        /// </summary>
        /// <param name="repo">Repository tương ứng với Entity T</param>
        public BaseService(IBaseRepository<T> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Lấy toàn bộ bản ghi của entity.
        /// Phương thức này hầu như không chứa nghiệp vụ, 
        /// chỉ gọi xuống repository.
        /// </summary>
        /// <returns>Danh sách entity</returns>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        /// <summary>
        /// Lấy đối tượng theo mã định danh.
        /// Thường dùng cho trang detail hoặc edit.
        /// </summary>
        /// <param name="id">Guid của bản ghi</param>
        /// <returns>Entity tương ứng</returns>
        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new NotFoundException("Không tìm thấy bản ghi.", field: "Id");

            return entity;
        }

        /// <summary>
        /// Thêm mới một bản ghi.
        /// </summary>
        /// <param name="dto">DTO dữ liệu tạo mới</param>
        /// <param name="createdBy">Người tạo (username)</param>
        /// <returns>Entity sau khi thêm</returns>
        public abstract Task<T> InsertAsync(TCreateDto dto, string createdBy);

        /// <summary>
        /// Cập nhật bản ghi theo id.
        /// Đây cũng là abstract như Insert vì nghiệp vụ tùy từng entity.
        /// </summary>
        /// <param name="id">Id của bản ghi cần sửa</param>
        /// <param name="dto">DTO dữ liệu cập nhật</param>
        /// <param name="modifiedBy">Người sửa</param>
        /// <returns>Entity sau cập nhật</returns>
        public abstract Task<T> UpdateAsync(Guid id, TCreateDto dto, string modifiedBy);

        /// <summary>
        /// Xóa mềm một bản ghi.
        /// Chỉ gọi xuống repository, không chứa nghiệp vụ.
        /// </summary>
        /// <param name="id">Id của bản ghi cần xóa</param>
        /// <returns>Số bản ghi bị ảnh hưởng (1 nếu thành công)</returns>
        public virtual async Task<int> SoftDeleteAsync(Guid id)
        {
            var exist = await _repo.GetByIdAsync(id);
            if (exist == null)
                throw new NotFoundException("Không tìm thấy bản ghi để xóa.", field: "Id");

            return await _repo.SoftDeleteAsync(id); 
        }

    }
}
