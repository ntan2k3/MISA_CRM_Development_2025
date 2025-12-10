using CsvHelper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MISA.CRM2025.Core.DTOs;
using MISA.CRM2025.Core.DTOs.Requests;
using MISA.CRM2025.Core.Entities;
using MISA.CRM2025.Core.Interfaces.Repositories;
using MISA.CRM2025.Core.Interfaces.Services;
using MISA.CRM2025.Core.Mappings;
using System.Globalization;
using System.Text;


namespace MISA.CRM2025.Core.Services
{
    /// <summary>
    /// Service xử lý nghiệp vụ khách hàng.
    /// <para/>Mục đích:
    /// - Kế thừa BaseService để dùng CRUD cơ bản.
    /// - Thêm validate nghiệp vụ, xử lý avatar, import/export CSV, gán loại khách hàng.
    /// <para/>Ngữ cảnh sử dụng: Gọi bởi Controller để thao tác dữ liệu khách hàng.
    /// </summary>
    /// Created by: nguyentruongan - 04/12/2025
    public class CustomerService : BaseService<Customer, CustomerRequest>, ICustomerService
    {
        /// <summary>
        /// Repository thao tác DB cho nghiệp vụ riêng
        /// </summary>
        private readonly ICustomerRepository _customerRepository;

        /// <summary>
        /// Lấy đường dẫn WebRootPath
        /// </summary>
        private readonly IHostingEnvironment _env;

        /// <summary>
        /// Khởi tạo CustomerService với các dependency cần thiết.
        /// </summary>
        /// <param name="customerRepository">Repository thao tác dữ liệu khách hàng.</param>
        /// <param name="env">Đối tượng cung cấp thông tin môi trường WebRootPath.</param>
        public CustomerService(ICustomerRepository customerRepository, IHostingEnvironment env) : base(customerRepository)
        {
            _customerRepository = customerRepository;
            _env = env;
        }

        #region CRUD cơ bản

        /// <summary>
        /// Lấy thông tin chi tiết khách hàng theo Id
        /// <para/>Sử dụng khi cần hiển thị hoặc xử lý thông tin chi tiết khách hàng trên UI hoặc logic nghiệp vụ
        /// </summary>
        /// <param name="id">Id của khách hàng cần lấy</param>
        /// <returns>Customer tương ứng với Id, trả về null nếu không tìm thấy</returns>
        /// <remarks></remarks>
        /// Created by: nguyentruongan - 04/12/2025
        public override async Task<Customer> GetByIdAsync(Guid id)
        {
            // Trả về khách hàng theo id 
            return await _customerRepository.GetByIdAsync(id);
        }


        /// <summary>
        /// Thêm mới khách hàng
        /// <para/>Sử dụng khi người dùng muốn tạo khách hàng mới từ form hoặc API
        /// xử lý avatar, map DTO -> Entity và lưu vào DB
        /// </summary>
        /// <param name="dto">Dữ liệu khách hàng từ request</param>
        /// <param name="createdBy">Người tạo bản ghi</param>
        /// <returns>Customer vừa được tạo, trả về null nếu validate lỗi</returns>
        /// <remarks></remarks>
        /// Created by: nguyentruongan - 04/12/2025
        public override async Task<Customer> InsertAsync(CustomerRequest dto, string createdBy)
        {
            // Validate logic nghiệp vụ
            if (!await ValidateBusinessAsync(null, dto)) return null!;

            // Xử lý avatar: nếu client trước đó upload tạm, move sang folder chính
            var avatarUrl = MoveTempAvatarToAvatarFolder(dto.CustomerAvatarUrl) ?? "";

            // Map từ DTO -> Entity
            var customer = new Customer
            {
                // Sinh id mới mỗi khi tạo mới
                CustomerId = Guid.NewGuid(), 
                CustomerAvatarUrl = avatarUrl,
                CustomerType = dto.CustomerType,
                CustomerCode = dto.CustomerCode,
                CustomerName = dto.CustomerName,
                CustomerTaxCode = dto.CustomerTaxCode,
                CustomerAddr = dto.CustomerAddr,
                CustomerPhoneNumber = dto.CustomerPhoneNumber,
                CustomerEmail = dto.CustomerEmail,

                LastPurchaseDate = dto.LastPurchaseDate,
                PurchasedItemCode = dto.PurchasedItemCode,
                PurchasedItemName = dto.PurchasedItemName,

                CreatedDate = DateTime.UtcNow,
                CreatedBy = createdBy,
                IsDeleted = false
            };

            // Gọi repo để lưu vào DB
            await _customerRepository.InsertAsync(customer);

            // Trả về customer vừa tạo
            return customer;
        }

        /// <summary>
        /// Cập nhật thông tin khách hàng theo Id
        /// <para/>Sử dụng khi cần chỉnh sửa thông tin khách hàng từ form hoặc API
        /// xử lý avatar và cập nhật các trường dữ liệu
        /// </summary>
        /// <param name="id">Id khách hàng cần cập nhật</param>
        /// <param name="dto">Dữ liệu cập nhật từ request</param>
        /// <param name="modifiedBy">Người thực hiện chỉnh sửa</param>
        /// <returns>Customer sau khi cập nhật, trả về null nếu không tìm thấy hoặc validate lỗi</returns>
        /// <remarks></remarks>
        /// Created by: nguyentruongan - 04/12/2025
        public override async Task<Customer> UpdateAsync(Guid id, CustomerRequest dto, string modifiedBy)
        {
            // Lấy ra customer hiện tại
            var customer = await _customerRepository.GetByIdAsync(id);

            // Không thấy -> trả null
            if (customer == null)

                return null;

            // Validate logic nghiệp vụ
            if (!await ValidateBusinessAsync(id, dto)) return null!;

            // Nếu người dùng update avatar thì chuyển tư temp -> avatars
            var avatarUrl = MoveTempAvatarToAvatarFolder(dto.CustomerAvatarUrl);

            // Nếu có ảnh update thì gán lại cho khách hàng hiện tại, không thì không cập nhật.
            if(avatarUrl != null)

                customer.CustomerAvatarUrl = avatarUrl;

            // Gán các field cần cập nhật
            customer.CustomerType = dto.CustomerType;
            customer.CustomerName = dto.CustomerName;
            customer.CustomerTaxCode = dto.CustomerTaxCode;
            customer.CustomerAddr = dto.CustomerAddr;
            customer.CustomerPhoneNumber = dto.CustomerPhoneNumber;
            customer.CustomerEmail = dto.CustomerEmail;

            customer.LastPurchaseDate = dto.LastPurchaseDate;
            customer.PurchasedItemCode = dto.PurchasedItemCode;
            customer.PurchasedItemName = dto.PurchasedItemName;

            customer.ModifiedDate = DateTime.UtcNow;
            customer.ModifiedBy = modifiedBy;

            // Lưu vào repo
            await _customerRepository.UpdateAsync(customer);

            // Trả về khách hàng sau cập nhật
            return customer;
        }

        /// <summary>
        /// Xóa mềm một khách hàng (IsDeleted = true)
        /// <para/>Sử dụng khi muốn xóa khách hàng nhưng vẫn giữ dữ liệu trong DB
        /// </summary>
        /// <param name="id">Id khách hàng cần xóa</param>
        /// <returns>Số lượng bản ghi bị ảnh hưởng</returns>
        /// <remarks></remarks>
        /// Created by: nguyentruongan - 04/12/2025
        public override async Task<int> SoftDeleteAsync(Guid id)
        {
            // Gọi repo để xóa
            return await _repo.SoftDeleteAsync(id);
        }

        #endregion

        #region Business Logic

        /// <summary>
        /// Thực hiện validate nghiệp vụ trước khi thêm mới hoặc cập nhật khách hàng.
        /// <para/>Ngữ cảnh sử dụng: Kiểm tra trùng email và số điện thoại trước khi ghi dữ liệu xuống DB.
        /// </summary>
        /// <param name="id">Id khách hàng hiện tại (nếu đang update), null nếu thêm mới.</param>
        /// <param name="dto">Thông tin khách hàng cần kiểm tra.</param>
        /// <returns>
        /// True nếu dữ liệu hợp lệ (không bị trùng email/phone).  
        /// False nếu phát hiện email hoặc số điện thoại đã được khách hàng khác sử dụng.
        /// </returns>
        /// Created by: nguyentruongan - 04/12/2025
        private async Task<bool> ValidateBusinessAsync(Guid? id, CustomerRequest dto)
        {
            var emailOwner = await _customerRepository.GetByEmailAsync(dto.CustomerEmail);
            if (emailOwner != null && emailOwner.CustomerId != id)
                return false;

            var phoneOwner = await _customerRepository.GetByPhoneAsync(dto.CustomerPhoneNumber);
            if (phoneOwner != null && phoneOwner.CustomerId != id)
                return false;

            return true;
        }

        /// <summary>
        /// Sinh mã khách hàng tự động theo định dạng: KH + yyyyMM + 6 chữ số tăng dần.
        /// <para/>Ngữ cảnh sử dụng: Tạo mã khách hàng mới khi thêm mới một bản ghi.
        /// </summary>
        /// <returns>Mã khách hàng mới theo chuẩn hệ thống.</returns>
        /// <remarks></remarks>
        /// Created by: nguyentruongan - 04/12/2025
        public async Task<string> GenerateCustomerCode()
        {
            // Tạo prefix

            string prefix = $"KH{DateTime.UtcNow:yyyyMM}";

            // Lấy mã lớn nhất hiện tại trong DB
            var maxCustomerCode = await _customerRepository.GetMaxCustomerCodeAsync(prefix);

            int nextNumber = 1;

            // Lấy phần số cuối cùng + 1
            if (!string.IsNullOrEmpty(maxCustomerCode))
            {
                // Lấy phần số cuối cùng
                string numberPart = maxCustomerCode.Substring(prefix.Length);

                nextNumber = int.Parse(numberPart) + 1;
            }

            // Tạo code mới, next number đưa về định dạng 6 chữ số
            string newCode = $"{prefix}{nextNumber:D6}";

            // Trả về code mới
            return newCode;
        }

        #endregion

        #region Paging, Filtering, Search, Sort

        /// <summary>
        /// Lấy tổng số khách hàng theo bộ lọc
        /// <para/>Sử dụng để hiển thị số lượng tổng cho phân trang hoặc thống kê
        /// </summary>
        /// <param name="query">Thông tin filter/phân trang</param>
        /// <returns>Số lượng khách hàng thỏa điều kiện</returns>
        /// <remarks></remarks>
        /// Created by: nguyentruongan - 05/12/2025
        public async Task<int> GetTotalCountAsync(CustomerQueryParameters query)
        {
            return await _customerRepository.GetTotalCountAsync(query);
        }

        /// <summary>
        /// Lấy danh sách khách hàng theo phân trang và bộ lọc
        /// <para/>Sử dụng khi hiển thị danh sách khách hàng trên UI với phân trang
        /// </summary>
        /// <param name="query">Thông tin phân trang và điều kiện lọc</param>
        /// <returns>Danh sách khách hàng</returns>
        /// <remarks></remarks>
        /// Created by: nguyentruongan - 05/12/2025
        public async Task<IEnumerable<Customer>> GetCustomersPagingAsync(CustomerQueryParameters query)
        {
            return await _customerRepository.GetCustomersPagingAsync(query);
        }

        #endregion

        #region Import / Export CSV

        /// <summary>
        /// Xuất CSV cho danh sách khách hàng theo Id
        /// <para/>Sử dụng khi người dùng muốn tải file CSV
        /// </summary>
        /// <param name="ids">Danh sách Id khách hàng cần xuất</param>
        /// <returns>File CSV dưới dạng byte[]</returns>
        /// <remarks></remarks>
        /// Created by: nguyentruongan - 05/12/2025
        public async Task<byte[]> ExportCsvAsync(List<Guid> ids)
        {
            // Lấy ra danh sách khách hàng theo danh sách Ids
            var customers = await _customerRepository.GetListByIdsAsync(ids);

            // Chuyển từ entity sang DTO (chỉ chứa những trường cần xuất CSV)
            // Select tạo danh sách mới csvData dùng để ghi CSV.
            var csvData = customers.Select(c => new CustomerCsv
            {
                CustomerType = c.CustomerType,
                CustomerCode = c.CustomerCode,
                CustomerName = c.CustomerName,
                CustomerTaxCode = c.CustomerTaxCode,
                CustomerAddr = c.CustomerAddr,
                CustomerPhoneNumber = c.CustomerPhoneNumber,
                CustomerEmail = c.CustomerEmail,
                LastPurchaseDate = c.LastPurchaseDate,  
                PurchasedItemCode = c.PurchasedItemCode,
                PurchasedItemName = c.PurchasedItemName
            }).ToList();

            // Tạo bộ nhớ lưu file CSV, không ghi trực tiếp vào ổ cứng.
            using var memoryStream = new MemoryStream();

            // Ghi text vào memoryStream, UTF-8 BOM để Excel đọc tiếng Việt không bị lỗi font
            using var writer = new StreamWriter(memoryStream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));

            // Tạo CSV Writer để ghi file CSV theo chuẩn.
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            // Chỉ xuất đúng các cột đã map trong CustomerCsvMap
            csv.Context.RegisterClassMap<CustomerCsvMap>();

            // Ghi records vào CSV
            csv.WriteRecords(csvData);

            // Flush dữ liệu từ writer → memoryStream
            writer.Flush();

            // Trả về mảng byte của file CSV
            return memoryStream.ToArray();
        }


        /// <summary>
        /// Import khách hàng từ file CSV
        /// <para/>Sử dụng khi người dùng upload CSV để tạo nhiều khách hàng cùng lúc
        /// </summary>
        /// <param name="csvStream">File CSV</param>
        /// <returns>Số lượng khách hàng import thành công</returns>
        /// <remarks></remarks>
        /// Created by: nguyentruongan - 05/12/2025
        public async Task<int> ImportCsvAsync(Stream csvStream)
        {
            // Đếm số record xử lý thành công
            int count = 0;

            // Tạo StreamReader để đọc text từ csvStream.
            using var streamReader = new StreamReader(csvStream);

            // Tạo đối tượng CsvReader để parse CSV thành các obj
            using var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture);

            csv.Context.RegisterClassMap<CustomerCsvMap>();

            // GetRecords<T>() trả về IEnumerable<T>
            var records = csv.GetRecords<CustomerCsv>().ToList();

            foreach (var dto in records)
            {
                var customerRequest = new CustomerRequest
                {
                    CustomerType = dto.CustomerType,
                    CustomerCode = dto.CustomerCode,
                    CustomerName = dto.CustomerName,
                    CustomerTaxCode = dto.CustomerTaxCode,
                    CustomerAddr = dto.CustomerAddr,
                    CustomerPhoneNumber = dto.CustomerPhoneNumber,
                    CustomerEmail = dto.CustomerEmail,
                    LastPurchaseDate = dto.LastPurchaseDate,
                    PurchasedItemCode = dto.PurchasedItemCode,
                    PurchasedItemName = dto.PurchasedItemName
                };

                var inserted = await InsertAsync(customerRequest, "");
                if (inserted != null) 
                    count++;
            }

            return count;
        }

        #endregion

        /// <summary>
        /// Upload ảnh tạm để preview
        /// <para/>Sử dụng khi người dùng muốn xem trước avatar trước khi submit form
        /// </summary>
        /// <param name="file">File ảnh upload</param>
        /// <returns>URL ảnh tạm để FE hiển thị</returns>
        /// <remarks></remarks>
        /// Created by: nguyentruongan - 05/12/2025
        public async Task<string> UploadTempAvatarAsync(IFormFile file)
        {
            // Xác định đường dẫn thư mục tạm trong wwwroot (wwwroot/uploads/temp)
            var folderPath = Path.Combine(_env.WebRootPath, "uploads", "temp");

            // Nếu thư mục chưa tồn tại thì tự tạo
            if (!Directory.Exists(folderPath))

                Directory.CreateDirectory(folderPath);

            // Tạo tên file unique để tránh trùng (GUID + đuôi file)
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            // Tạo đường dẫn vật lý đầy đủ đến file (wwwroot/uploads/temp/avatar.png)
            var filePath = Path.Combine(folderPath, fileName);

            // Copy dữ liệu file từ request vào file vật lý trong server
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Trả về URL (đường dẫn tương đối) để FE lấy hiển thị
            return $"/uploads/temp/{fileName}";
        }

        /// <summary>
        /// Move file avatar từ temp sang avatars
        /// <para/>Sử dụng khi submit form để lưu ảnh chính thức
        /// </summary>
        /// <param name="tempUrl">URL ảnh tạm</param>
        /// <returns>URL ảnh mới hoặc null nếu không có</returns>
        /// <remarks></remarks>
        /// Created by: nguyentruongan - 05/12/2025
        private string? MoveTempAvatarToAvatarFolder(string? tempUrl)
        {
            if (string.IsNullOrWhiteSpace(tempUrl))
                return null;

            // Lấy tên file từ url tạm (định dạng /uploads/temp/{fileName})
            var fileName = Path.GetFileName(tempUrl);

            // Đường dẫn thực tế
            var tempPath = Path.Combine(_env.WebRootPath, "uploads", "temp", fileName);

            // Tạo đường dẫn đến thư mục avatars (nơi lưu ảnh chính thức)
            var avatarFolder = Path.Combine(_env.WebRootPath, "uploads", "avatars");

            // Tạo folder nếu chưa tồn tại
            if (!Directory.Exists(avatarFolder))
                Directory.CreateDirectory(avatarFolder);

            // Đường dẫn file sau khi move (/uploads/avatars/{fileName})
            var finalPath = Path.Combine(avatarFolder, fileName);

            // Nếu file tồn tại trong temp thì move sang avatars
            if (File.Exists(tempPath))
            {
                // Nếu tồn tại file cùng tên ở avatars thì ghi đè
                if (File.Exists(finalPath))
                    File.Delete(finalPath);

                File.Move(tempPath, finalPath);
            }
            // Nếu temp không tồn tại, ta vẫn trả url final 
            return $"/uploads/avatars/{fileName}";
        }

        /// <summary>
        /// Xóa mềm nhiều khách hàng
        /// <para/>Sử dụng khi muốn xóa nhiều khách hàng cùng lúc mà vẫn giữ dữ liệu
        /// </summary>
        /// <param name="ids">Danh sách Id cần xóa</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        /// <remarks></remarks>
        /// Created by: nguyentruongan - 05/12/2025
        public async Task<int> SoftDeleteManyAsync(List<Guid> ids)
        {
            return await _customerRepository.SoftDeleteManyAsync(ids);
        }

        /// <summary>
        /// Gán loại khách hàng cho nhiều khách hàng
        /// <para/>Sử dụng khi cần update type của nhiều khách hàng cùng lúc
        /// </summary>
        /// <param name="ids">Danh sách Id khách hàng</param>
        /// <param name="customerType">Loại khách hàng cần gán</param>
        /// <returns>Số khách hàng được cập nhật</returns>
        /// <remarks></remarks>
        /// Created by: nguyentruongan - 06/12/2025
        public async Task<int> AssignCustomerTypeAsync(List<Guid> ids, string customerType)
        {
            // Gọi repo để update hàng loạt
            var result = await _customerRepository.AssignCustomerTypeAsync(ids, customerType);

            return result;
        }

        #region Check tồn tại Email / Phone

        /// <summary>
        /// Kiểm tra email đã tồn tại hay chưa (tránh trùng khi thêm/sửa)
        /// </summary>
        /// <param name="email">Email cần kiểm tra</param>
        /// <param name="id">Id khách hàng đang sửa (null nếu thêm mới)</param>
        /// <returns>True nếu tồn tại, false nếu chưa tồn tại</returns>
        /// Created by: nguyentruongan - 08/12/2025
        public async Task<bool> IsEmailExistAsync(string email, Guid? id = null)
        {
            var existingCustomer = await _customerRepository.GetByEmailAsync(email);
            // Nếu tồn tại và không phải bản ghi đang edit thì trả về true
            return existingCustomer != null && existingCustomer.CustomerId != id;
        }

        /// <summary>
        /// Kiểm tra số điện thoại đã tồn tại hay chưa (tránh trùng khi thêm/sửa)
        /// </summary>
        /// <param name="phoneNumber">Số điện thoại cần kiểm tra</param>
        /// <param name="id">Id khách hàng đang sửa (null nếu thêm mới)</param>
        /// <returns>True nếu tồn tại, false nếu chưa tồn tại</returns>
        /// Created by: nguyentruongan - 08/12/2025
        public async Task<bool> IsPhoneNumberExistAsync(string phoneNumber, Guid? id = null)
        {
            var existingCustomer = await _customerRepository.GetByPhoneAsync(phoneNumber);
            // Nếu tồn tại và không phải bản ghi đang edit thì trả về true
            return existingCustomer != null && existingCustomer.CustomerId != id;
        }

        #endregion

    }
}
    