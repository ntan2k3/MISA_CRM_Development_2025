using CsvHelper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MISA.CRM2025.Core.DTOs;
using MISA.CRM2025.Core.DTOs.Requests;
using MISA.CRM2025.Core.Entities;
using MISA.CRM2025.Core.Exceptions;
using MISA.CRM2025.Core.Interfaces.Repositories;
using MISA.CRM2025.Core.Interfaces.Services;
using MISA.CRM2025.Core.Mappings;
using System.Globalization;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;


namespace MISA.CRM2025.Core.Services
{
    /// <summary>
    /// Service xử lý toàn bộ nghiệp vụ liên quan đến khách hàng.
    /// Kế thừa BaseService để dùng chung CRUD cơ bản và bổ sung
    /// thêm các logic validate, import/export, upload avatar.
    /// </summary>
    /// Created by: nguyentruongan - 04/12/2025
    public class CustomerService : BaseService<Customer, CustomerRequest>, ICustomerService
    {
        /// <summary>
        /// Repository Customer — dùng để truy vấn DB cho phần nghiệp vụ riêng. 
        /// </summary>
        private readonly ICustomerRepository _customerRepository;

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

        /// <summary>
        /// Lấy thông tin chi tiết của một khách hàng dựa trên Id.
        /// </summary>
        /// <param name="id">Id của khách hàng.</param>
        /// <returns>Đối tượng customer tương ứng.</returns>
        /// <exception cref="NotFoundException">Ném lỗi nếu không tìm thấy khách hàng.</exception>
        /// Created by: nguyentruongan - 04/12/2025
        public override async Task<Customer> GetByIdAsync(Guid id)
        {
            // Lấy ra khách hàng theo id từ repo
            var customer = await _customerRepository.GetByIdAsync(id);
            
            // Không thấy -> NotFound
            if (customer == null)
                throw new NotFoundException("Không tìm thấy khách hàng.", field:"CustomerId");

            // Có thì trả về
            return customer;
        }


        /// <summary>
        /// Thêm mới khách hàng.
        /// Thực hiện validate, xử lý avatar tạm, map DTO sang Entity và lưu xuống DB.
        /// </summary>
        /// <param name="dto">Dữ liệu khách hàng từ request.</param>
        /// <param name="createdBy">Người tạo bản ghi.</param>
        /// <returns>Khách hàng vừa được thêm mới.</returns>
        /// Created by: nguyentruongan - 04/12/2025
        public override async Task<Customer> InsertAsync(CustomerRequest dto, string createdBy)
        {
            // Validate
            await ValidateCustomerAsync(null, dto);

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
        /// Cập nhật thông tin của khách hàng dựa trên Id.
        /// Thực hiện validate, xử lý avatar tạm và cập nhật các trường dữ liệu.
        /// </summary>
        /// <param name="id">Id khách hàng cần cập nhật.</param>
        /// <param name="dto">Dữ liệu cập nhật từ request.</param>
        /// <param name="modifiedBy">Người thực hiện chỉnh sửa.</param>
        /// <returns>Khách hàng sau khi được cập nhật.</returns>
        /// <exception cref="NotFoundException">Ném lỗi nếu không tồn tại khách hàng.</exception>
        /// Created by: nguyentruongan - 04/12/2025
        public override async Task<Customer> UpdateAsync(Guid id, CustomerRequest dto, string modifiedBy)
        {
            // Validate
            await ValidateCustomerAsync(id, dto);

            // Lấy ra customer hiện tại
            var customer = await _customerRepository.GetByIdAsync(id);

            // Không thấy -> NotFound
            if (customer == null)

                throw new NotFoundException($"Không tìm thấy khách hàng {id} để cập nhật.", field: "CustomerId");

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
        /// Xóa mềm một khách hàng (đặt IsDeleted = true).
        /// </summary>
        /// <param name="id">Id khách hàng cần xóa mềm.</param>
        /// <returns>Số lượng bản ghi bị ảnh hưởng.</returns>
        /// <exception cref="NotFoundException">Ném lỗi nếu không tìm thấy khách hàng.</exception>
        public override async Task<int> SoftDeleteAsync(Guid id)
        {
            // Lấy ra khách hàng cần xóa theo id
            var customer = await _repo.GetByIdAsync(id);

            // Không thấy -> NotFound
            if (customer == null)
                throw new NotFoundException("Không tìm thấy khách hàng để xóa.", field: "CustomerId");

            // Gọi repo để xóa
            return await _repo.SoftDeleteAsync(id);
        }

        /// <summary>
        /// Validate dữ liệu khi Insert/Update
        /// Bao gồm:
        /// - Check không được để trống tên, số điện thoại và email
        /// - Check định dạng email
        /// - Check định dạng số điện thoại (10 - 11 số)
        /// </summary>
        /// <param name="id">Nếu update truyền id, insert truyền null</param>
        /// <param name="dto">CustomerRequest</param>
        /// <returns></returns>
        /// <exception cref="ValidateException">Ném lỗi khi dữ liệu không hợp lệ.</exception>
        /// <exception cref="ConflictException">Ném lỗi khi dữ liệu bị trùng.</exception>
        /// Created by: nguyentruongan - 04/12/2025
        public async Task ValidateCustomerAsync(Guid? id, CustomerRequest dto)
        {
            // Tên không được để trống
            if (string.IsNullOrWhiteSpace(dto.CustomerName))
                throw new ValidateException("Tên khách hàng không được để trống.", field: "CustomerName");

            // Check định dạng email
            if (!IsValidEmail(dto.CustomerEmail))
                throw new ValidateException("Email không đúng định dạng", field: "CustomerEmail");

            // Email không được trùng
            var emailOwner = await _customerRepository.GetByEmailAsync(dto.CustomerEmail);
            if (emailOwner != null && emailOwner.CustomerId != id)
                throw new ConflictException("Email đã tồn tại.", field: "CustomerEmail");

            // Số điện thoại có 10 - 11 số
            if (!IsValidPhone(dto.CustomerPhoneNumber))
                throw new ValidateException("Sô điện thoại phải có định dạng từ 10 - 11 số", field: "CustomerPhoneNumber");


            // Số điện thoại không được trùng
            var phoneOwner = await _customerRepository.GetByPhoneAsync(dto.CustomerPhoneNumber);
            if (phoneOwner != null && phoneOwner.CustomerId != id)
                throw new ConflictException("Số điện thoại đã tồn tại.", field: "CustomerPhoneNumber");
        }

        /// <summary>
        /// Kiểm tra email có đúng định dạng hay không.
        /// </summary>
        /// <param name="email">Địa chỉ email cần kiểm tra.</param>
        /// <returns>True nếu hợp lệ, False nếu không.</returns>
        /// Created by: nguyentruongan - 04/12/2025
        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                var _ = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra định dạng số điện thoại Việt Nam (10–11 số, bắt đầu bằng 0).
        /// </summary>
        /// <param name="phone">Số điện thoại.</param>
        /// <returns>True nếu hợp lệ.</returns>
        /// Created by: nguyentruongan - 04/12/2025
        public bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            // Bắt đầu bằng 0 theo sau là 9 hoặc 10 chữ số
            string pattern = @"^0\d{9,10}$";

            return Regex.IsMatch(phone, pattern);
        }

        /// <summary>
        /// Tạo mã khách hàng
        /// Dịnh dạng: KH + yyyyMM + 6 chữ số tăng dần
        /// </summary>
        /// <returns>Mã khách hàng mới nhất</returns>
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
        
        /// <summary>
        /// Lấy ra tổng số bản ghi theo query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        /// Created by: nguyentruongan - 05/12/2025
        public async Task<int> GetTotalCountAsync(CustomerQueryParameters query)
        {
            return await _customerRepository.GetTotalCountAsync(query);
        }

        /// <summary>
        /// Lấy danh sách khách hàng theo phân trang và điều kiện lọc.
        /// </summary>
        /// <param name="query">Thông tin phân trang và bộ lọc.</param>
        /// <returns>Danh sách khách hàng.</returns>
        /// Created by: nguyentruongan - 05/12/2025
        public async Task<IEnumerable<Customer>> GetCustomersPagingAsync(CustomerQueryParameters query)
        {
            return await _customerRepository.GetCustomersPagingAsync(query);
        }

        /// <summary>
        /// Xuất file CSV cho danh sách khách hàng theo Id được chọn.
        /// Chỉ xuất các cột đã được khai báo trong CustomerCsvMap.
        /// </summary>
        /// <param name="ids">Danh sách Id khách hàng cần export.</param>
        /// <returns>Mảng byte của file CSV.</returns>
        /// Created by: nguyentruongan - 05/12/2025
        public async Task<byte[]> ExportCsvAsync(List<Guid> ids)
        {
            // Lấy ra danh sách khách hàng theo danh sách Ids
            var customers = await _customerRepository.GetListByIdsAsync(ids);

            // Chuyển từ entity sang DTO (chỉ chứa những trường cần xuất CSV)
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

            // Tạo bộ nhớ lưu file CSV
            using var memoryStream = new MemoryStream();

            // UTF-8 BOM để Excel đọc tiếng Việt không bị lỗi font
            using var writer = new StreamWriter(memoryStream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));

            // Tạo CSV Writer
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
        /// Import dữ liệu khách hàng từ file CSV.
        /// Tự động validate từng dòng và bỏ qua các dòng sai.
        /// </summary>
        /// <param name="csvStream">File CSV upload lên.</param>
        /// <returns>Số lượng khách hàng được import thành công.</returns>
        /// Created by: nguyentruongan - 05/12/2025
        public async Task<int> ImportCsvAsync(Stream csvStream)
        {
            // Đếm số record xử lý thành công
            int count = 0;

            // Tạo StreamReader để đọc text từ csvStream.
            using var streamReader = new StreamReader(csvStream);

            // Tạo đối tượng CsvReader để parse CSV.
            using var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture);

            csv.Context.RegisterClassMap<CustomerCsvMap>();

            // GetRecords<T>() trả về IEnumerable<T>
            List<CustomerCsv> records;

            // Bắt lỗi thiếu cột ngay khi đọc CSV
            try
            {
                records = csv.GetRecords<CustomerCsv>().ToList();
            }
            catch (HeaderValidationException ex)
            {
                throw new ValidateException($"File CSV thiếu cột dữ liệu", field: "File");
            }
           

            foreach (var dto in records)
            {
                try
                {
                    var customer = new CustomerRequest
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

                    await InsertAsync(customer, "");
                    count++;
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
            return count;
        }

        /// <summary>
        /// Upload ảnh tạm để preview trước khi submit form.
        /// Ảnh sẽ được lưu vào thư mục /wwwroot/uploads/temp.
        /// Khi người dùng submit form, ảnh này sẽ được move sang thư mục chính.
        /// </summary>
        /// <param name="file">File ảnh người dùng upload</param>
        /// <returns>Trả về đường dẫn URL tương đối để FE hiển thị preview</returns>
        /// Created by: nguyentruongan - 05/12/2025
        public async Task<string> UploadTempAvatarAsync(IFormFile file)
        {
            // Kiểm tra file null hoặc rỗng
            if (file == null || file.Length == 0)
                throw new ValidateException("File không hợp lệ hoặc không tồn tại", field: "File");

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
        /// Di chuyển file avatar từ thư mục tạm => thư mục chính.
        /// Được gọi khi người dùng submit form.
        /// Ví dụ: từ /uploads/temp/avatar.png -> /uploads/avatars/avatar.png
        /// </summary>
        /// <param name="tempUrl">URL ảnh tạm do FE gửi lên</param>
        /// <returns>URL mới sau khi move, hoặc null nếu không có ảnh</returns>
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
        /// Xóa mềm hàng loạt khách hàng.
        /// </summary>
        /// <param name="ids">Danh sách Id cần xóa mềm.</param>
        /// <returns>Số lượng bản ghi bị ảnh hưởng.</returns>
        /// Created by: nguyentruongan - 05/12/2025
        public async Task<int> SoftDeleteManyAsync(List<Guid> ids)
        {
            return await _customerRepository.SoftDeleteManyAsync(ids);
        }

        /// <summary>
        /// Gán loại khách hàng cho danh sách Id truyền vào.
        /// </summary>
        /// <param name="ids">Danh sách khách hàng.</param>
        /// <param name="customerType">Loại khách hàng cần gán.</param>
        /// <returns>Số lượng khách hàng được cập nhật.</returns>
        /// <exception cref="ValidateException">Ném lỗi nếu input không hợp lệ.</exception>
        /// Created by: nguyentruongan - 06/12/2025
        public async Task<int> AssignCustomerTypeAsync(List<Guid> ids, string customerType)
        {
            // Validate đầu vào
            if (ids == null || ids.Count == 0)
                throw new ValidateException("Danh sách khách hàng không được rỗng.", field: "CustomerIds");

            if (string.IsNullOrWhiteSpace(customerType))
                throw new ValidateException("Loại khách hàng không được để trống.", field: "CustomerType");

            // Gọi repo để update hàng loạt
            var result = await _customerRepository.AssignCustomerTypeAsync(ids, customerType);

            return result;
        }
    }
}
    