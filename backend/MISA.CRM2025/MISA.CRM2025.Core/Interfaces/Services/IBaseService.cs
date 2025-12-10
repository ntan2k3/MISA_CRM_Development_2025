namespace MISA.CRM2025.Core.Interfaces.Services
{
    /// <summary>
    /// Interface service cơ bản cho các entity trong hệ thống.
    /// Dùng làm lớp nền tảng để chuẩn hoá các thao tác CRUD phổ biến.
    /// </summary>
    /// <typeparam name="T">
    /// Kiểu entity đại diện cho bảng dữ liệu (ví dụ: Customer, Employee,...)
    /// </typeparam>
    /// <typeparam name="TCreateDto">
    /// Kiểu DTO dùng cho thao tác thêm/sửa dữ liệu (Request DTO)
    /// </typeparam>
    /// Created by: nguyentruongan - 03/12/2025
    public interface IBaseService<T, TCreateDto> 
    {
        /// <summary>
        /// Lấy toàn bộ bản ghi của entity.
        /// </summary>
        /// <returns>
        /// Danh sách tất cả entity T.
        /// </returns>
        /// Created by: nguyentruongan - 03/12/2025
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Lấy một bản ghi theo Id.
        /// </summary>
        /// <param name="id">
        /// Id của bản ghi cần lấy.
        /// </param>
        /// <returns>
        /// Trả về entity T tương ứng với Id nếu tồn tại, null nếu không tìm thấy.
        /// </returns>
        /// Created by: nguyentruongan - 03/12/2025
        Task<T> GetByIdAsync(Guid id);

        /// <summary>
        /// Thêm mới một bản ghi.
        /// </summary>
        /// <param name="dto">
        /// DTO chứa dữ liệu tạo mới.
        /// </param>
        /// <param name="createdBy">
        /// Người thực hiện thao tác tạo.
        /// </param>
        /// <returns>
        /// Trả về entity T sau khi tạo thành công.
        /// </returns>
        /// Created by: nguyentruongan - 03/12/2025
        Task<T> InsertAsync(TCreateDto dto, string createdBy);

        /// <summary>
        /// Cập nhật bản ghi theo Id.
        /// </summary>
        /// <param name="id">
        /// Id bản ghi cần cập nhật.
        /// </param>
        /// <param name="dto">
        /// DTO chứa dữ liệu cập nhật.
        /// </param>
        /// <param name="modifiedBy">
        /// Người thực hiện thao tác cập nhật.
        /// </param>
        /// <returns>
        /// Trả về entity T sau khi cập nhật thành công.
        /// </returns>
        /// Created by: nguyentruongan - 03/12/2025
        Task<T> UpdateAsync(Guid id, TCreateDto dto, string modifiedBy);

        /// <summary>
        /// Xóa mềm một bản ghi theo Id.
        /// (Không xóa khỏi DB mà chỉ đánh dấu đã xóa)
        /// </summary>
        /// <param name="id">
        /// Id bản ghi cần xóa mềm.
        /// </param>
        /// <returns>
        /// Số lượng bản ghi bị ảnh hưởng (thường = 1 nếu thành công).
        /// </returns>
        /// Created by: nguyentruongan - 03/12/2025
        Task<int> SoftDeleteAsync(Guid id);
    }
}
