# MISA CRM - Customer Relationship Management API

Hệ thống quản lý khách hàng (CRM) được xây dựng trên nền tảng .NET 8 với kiến trúc 3 tầng, cung cấp các API RESTful để quản lý thông tin khách hàng.

## 📋 Mục lục

- [Tính năng](#-tính-năng)
- [Công nghệ sử dụng](#-công-nghệ-sử-dụng)
- [Cấu trúc dự án](#-cấu-trúc-dự-án)
- [Yêu cầu hệ thống](#-yêu-cầu-hệ-thống)
- [Cài đặt và chạy](#-cài-đặt-và-chạy)

## ✨ Tính năng

- **CRUD**: Thêm, sửa, xóa, lấy danh sách khách hàng
- **Phân trang**: Hỗ trợ phân trang cho danh sách khách hàng
- **Sắp xếp**: Sắp xếp theo các cột tùy chọn
- **Tìm kiếm**: Tìm kiếm theo tên, email, số điện thoại
- **Lọc**: Lọc theo loại khách hàng
- **Import CSV**: Nhập danh sách khách hàng từ file CSV
- **Export CSV**: Xuất danh sách khách hàng ra file CSV
- **Validation**: Kiểm tra trùng lặp email, số điện thoại, check định dạng email, số điện thoại và không được bỏ trống các trường yêu càu.

## 🛠 Công nghệ sử dụng

| Công nghệ      | Phiên bản | Mục đích        |
| -------------- | --------- | --------------- |
| .NET           | 8.0       | Framework chính |
| ASP.NET Core   | 8.0       | Web API         |
| MySQL          | 8.0+      | Database        |
| Dapper         | 2.1.66    | Micro ORM       |
| MySqlConnector | 2.5.0     | MySQL driver    |
| Swashbuckle    | 6.6.2     | Swagger/OpenAPI |

## 📁 Cấu trúc dự án

```
Misa.CRM2025/
├── MISA.CRM2025.Core/                  # Business Logic Layer
│   ├── DTOs/
│   │   ├── Requests/                   # Request DTOs
│   │   └── Responses/                  # Response DTOs (ApiResponse, CustomerResponse)
│   ├── Entities/
│   │   └── Customer.cs
│   ├── Exception/                      # Custom Exceptions
│   │   ├── BaseException.cs
│   │   ├── ConflictException.cs
│   │   ├── NotFoundException.cs
│   │   ├── ValidationException.cs
│   ├── Interfaces/
│   │   ├── Repositories/
│   │   └── Services/
│   └── Services/
│       ├── BaseService.cs
│       └── CustomerService.cs
│
├── MISA.CRM2025.Infrastructure/        # Data Access Layer
│   └── Repositories/
│       ├── BaseRepository.cs
│       └── CustomerRepository.cs
│
├── Misa.CRM2025.Api/                   # Presentation Layer (Web API)
│   ├── Controllers/
│   │   ├── BaseController.cs
│   │   └── CustomerController.cs
│   ├── Middleware/
│   │   ├── ExceptionMiddleware.cs
│   │   └── ExceptionMiddlewareExtensions.cs
│   ├── wwwroot                         # Lưu avatar
│   │   └── uploads
│   │       ├── avatars
│   │       └── temp
│   ├── appsettings.json
│   └── Program.cs
│
└── README.md
```

## 💻 Yêu cầu hệ thống

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MySQL 8.0+](https://dev.mysql.com/downloads/mysql/)
- IDE: Visual Studio 2022 / VS Code / Rider

## 🚀 Cài đặt và chạy

### 1. Clone repository

```bash
git clone <repository-url>
cd MISA_CRM_Development_2025
```

### 2. Tạo Database

Tạo database MySQL với tên `misa_crm_development_2025` và bảng `customer`:

```sql
CREATE TABLE misa_crm_development_2025.customer (
  customer_id char(36) NOT NULL DEFAULT (UUID()) COMMENT 'Id khách hàng',
  customer_avatar_url varchar(255) DEFAULT NULL,
  customer_type varchar(20) DEFAULT NULL COMMENT 'Loại khách hàng',
  customer_code varchar(20) NOT NULL COMMENT 'Mã khách hàng',
  customer_name varchar(255) NOT NULL COMMENT 'Tên khách hàng',
  customer_tax_code varchar(20) DEFAULT NULL COMMENT 'Mã số thuế',
  customer_addr varchar(255) DEFAULT NULL COMMENT 'Địa chỉ',
  customer_phone_number varchar(50) NOT NULL COMMENT 'Số điện thoại',
  customer_email varchar(100) NOT NULL COMMENT 'Email khách hàng',
  last_purchase_date datetime DEFAULT NULL COMMENT 'Ngày mua gần nhất',
  purchased_item_code varchar(100) DEFAULT NULL COMMENT 'Hàng hóa đã mua',
  purchased_item_name varchar(100) DEFAULT NULL COMMENT 'Tên hàng hóa đã mua',
  is_deleted tinyint NOT NULL DEFAULT 0,
  created_date datetime NOT NULL COMMENT 'Ngày tạo',
  created_by varchar(100) NOT NULL COMMENT 'Người tạo',
  modified_date datetime DEFAULT NULL COMMENT 'Ngày sửa',
  modified_by varchar(100) DEFAULT NULL COMMENT 'Người sửa',
  PRIMARY KEY (customer_id)
)
ENGINE = INNODB,
AVG_ROW_LENGTH = 342,
CHARACTER SET utf8mb4,
COLLATE utf8mb4_0900_as_ci,
COMMENT = 'Khách hàng';

ALTER TABLE misa_crm_development_2025.customer
ADD UNIQUE INDEX uix_customer_customer_code (customer_code);

ALTER TABLE misa_crm_development_2025.customer
ADD UNIQUE INDEX uix_customer_customer_email (customer_email);

ALTER TABLE misa_crm_development_2025.customer
ADD UNIQUE INDEX uix_customer_customer_id (customer_id);

ALTER TABLE misa_crm_development_2025.customer
ADD UNIQUE INDEX uix_customer_customer_phone_number (customer_phone_number);
```

### 3. Cấu hình connection string

Mở file `Misa.Crm.Development/appsettings.json` và cập nhật:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=misa_crm_development_2025;User=root;Password=YOUR_PASSWORD;"
  }
}
```

### 4. Chạy ứng dụng

```bash
# Restore packages
dotnet restore

# Build
dotnet build

# Run
cd MISA.CRM2025
dotnet run
```

## 👥 Tác giả

- **nguyentruongan** - 07/12/2025
