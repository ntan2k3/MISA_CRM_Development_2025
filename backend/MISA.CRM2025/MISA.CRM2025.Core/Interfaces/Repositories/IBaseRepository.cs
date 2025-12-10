namespace MISA.CRM2025.Core.Interfaces.Repositories
{
    /// <summary>
    /// Interface nền tảng cho Repository.
    /// Định nghĩa các chức năng CRUD dùng chung cho toàn bộ entity trong hệ thống.
    /// </summary>
    /// <typeparam name="T">
    /// Loại entity mà repository sẽ thao tác.
    /// </typeparam>
    /// Created by: nguyentruongan - 03/12/2025
    public interface IBaseRepository<T>
    {
        /// <summary>
        /// Lấy toàn bộ bản ghi của entity T.
        /// </summary>
        /// <returns>
        /// Danh sách tất cả entity trong hệ thống.
        /// </returns>
        /// Created by: nguyentruongan - 03/12/2025
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Lấy một bản ghi theo Id.
        /// </summary>
        /// <param name="id">
        /// Id duy nhất của bản ghi cần lấy.
        /// </param>
        /// <returns>
        /// Entity T tương ứng với Id, hoặc null nếu không tồn tại.
        /// </returns>
        /// Created by: nguyentruongan - 03/12/2025
        Task<T> GetByIdAsync(Guid id);

        /// <summary>
        /// Thêm bản ghi mới vào hệ thống.
        /// </summary>
        /// <param name="entity">
        /// Đối tượng entity T cần thêm.
        /// </param>
        /// <returns>
        /// Số lượng bản ghi bị ảnh hưởng (thường là 1).
        /// </returns>
        /// Created by: nguyentruongan - 03/12/2025
        Task<int> InsertAsync(T entity);

        /// <summary>
        /// Cập nhật thông tin bản ghi.
        /// </summary>
        /// <param name="entity">
        /// Entity chứa dữ liệu đã thay đổi.
        /// </param>
        /// <returns>
        /// Số lượng bản ghi bị ảnh hưởng.
        /// </returns>
        /// Created by: nguyentruongan - 03/12/2025
        Task<int> UpdateAsync(T entity);

        /// <summary>
        /// Xóa mềm một bản ghi (đánh dấu is_deleted = 1).
        /// </summary>
        /// <param name="id">
        /// Id của bản ghi cần xóa mềm.
        /// </param>
        /// <returns>
        /// Số bản ghi bị ảnh hưởng.
        /// </returns>
        /// Created by: nguyentruongan - 03/12/2025
        Task<int> SoftDeleteAsync(Guid id);
    }
}
