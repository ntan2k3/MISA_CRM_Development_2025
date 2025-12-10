using MISA.CRM2025.Core.Exceptions;
using MISA.CRM2025.Core.Interfaces.Repositories;
using MISA.CRM2025.Core.Interfaces.Services;

namespace MISA.CRM2025.Core.Services
{
    /// <summary>
    /// Service cơ sở chứa các hành vi CRUD dùng chung cho tất cả entity.
    /// <para/>Ngữ cảnh sử dụng: Là lớp nền tảng để các service con kế thừa.
    /// Các nghiệp vụ đặc thù sẽ override hoặc bổ sung tại service con.
    /// </summary>
    /// <typeparam name="T">Entity tương ứng với bảng trong database.</typeparam>
    /// <typeparam name="TCreateDto">DTO dùng cho thao tác tạo mới hoặc cập nhật.</typeparam>
    /// Created by: nguyentruongan – 04/12/2025
    public abstract class BaseService<T, TCreateDto> : IBaseService<T, TCreateDto>
    {
        /// <summary>
        /// Repository tương ứng với entity, dùng thao tác dữ liệu với database.
        /// </summary>
        protected readonly IBaseRepository<T> _repo;

        /// <summary>
        /// Khởi tạo service và inject repository cho entity tương ứng.
        /// <para/>Ngữ cảnh sử dụng: Tự động chạy khi tạo service con.
        /// </summary>
        /// <param name="repo">Repository dùng để truy cập dữ liệu cho entity T.</param>
        public BaseService(IBaseRepository<T> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Lấy toàn bộ bản ghi chưa bị xóa mềm.
        /// <para/>Ngữ cảnh: Dùng trong màn hình danh sách, tải dữ liệu cơ bản.</para>
        /// </summary>
        /// <returns>Danh sách tất cả entity.</returns>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        /// <summary>
        /// Lấy một bản ghi theo Id.
        /// <para/>Ngữ cảnh: Màn hình xem chi tiết, form sửa, load dữ liệu để binding.</para>
        /// </summary>
        /// <param name="id">Guid của bản ghi cần lấy.</param>
        /// <returns>Entity tương ứng nếu tồn tại.</returns>
        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            return await _repo.GetByIdAsync(id);
        }

        /// <summary>
        /// Thêm mới một bản ghi vào database.
        /// <para/>Ngữ cảnh: Dùng cho thao tác Create của entity.</para>
        /// </summary>
        /// <param name="dto">Dữ liệu từ client gửi lên dùng để tạo mới.</param>
        /// <param name="createdBy">Tên/Id của người thực hiện thao tác tạo mới.</param>
        /// <returns>Entity sau khi được thêm thành công.</returns>
        public abstract Task<T> InsertAsync(TCreateDto dto, string createdBy);

        /// <summary>
        /// Cập nhật dữ liệu một bản ghi theo Id.
        /// <para/>Ngữ cảnh: Dùng cho thao tác Update của entity tại màn hình sửa.</para>
        /// </summary>
        /// <param name="id">Id của bản ghi cần cập nhật.</param>
        /// <param name="dto">DTO chứa dữ liệu mới.</param>
        /// <param name="modifiedBy">Tên/Id của người sửa.</param>
        /// <returns>Entity sau khi cập nhật thành công.</returns>
        public abstract Task<T> UpdateAsync(Guid id, TCreateDto dto, string modifiedBy);

        /// <summary>
        /// Xóa mềm một bản ghi bằng cách đặt is_deleted = 1.
        /// <para/>Ngữ cảnh: Dùng cho thao tác xóa nhưng vẫn muốn lưu lịch sử.</para>
        /// </summary>
        /// <param name="id">Id của bản ghi cần xóa.</param>
        /// <returns>Số bản ghi bị ảnh hưởng (1 nếu xóa thành công).</returns>
        public virtual async Task<int> SoftDeleteAsync(Guid id)
        {
            return await _repo.SoftDeleteAsync(id); 
        }

    }
}
