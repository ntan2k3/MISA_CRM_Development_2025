/**
 * File tiện ích xử lý validation cho form
 * Tách riêng để tái sử dụng cho nhiều component khác nhau
 */

import CustomersAPI from "@/apis/components/customers/CustomersAPI";

/**
 * Cấu hình các rule validate cho từng field
 * Mỗi field có thể có các loại rule sau:
 * - required: Bắt buộc nhập (string message)
 * - pattern: Kiểm tra format theo regex { regex, message }
 * - serverCheck: Kiểm tra trùng lặp trên server (boolean)
 * - serverCheckField: Tên field để check server ('email' | 'phone')
 */
export const validationRules = {
  customerName: {
    required: "Tên khách hàng không được để trống",
  },
  customerEmail: {
    required: "Email không được để trống",
    pattern: {
      regex: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
      message: "Email không hợp lệ",
    },
    serverCheck: true,
    serverCheckField: "email",
  },
  customerPhoneNumber: {
    required: "Số điện thoại không được để trống",
    pattern: {
      regex: /^0\d{9,10}$/,
      message: "Số điện thoại phải đúng định dạng và đủ 10 - 11 số",
    },
    serverCheck: true,
    serverCheckField: "phone",
  },
  // Có thể thêm validate rule mới vào đây
};

/**
 * Class quản lý validation với cache để tối ưu hiệu suất
 */
export class ValidationManager {
  constructor() {
    /**
     * Cache kết quả kiểm tra server
     * Cấu trúc: { fieldName: { value: string, exists: boolean, timestamp: number } }
     * timestamp giúp invalidate cache sau một khoảng thời gian
     */
    this.serverCheckCache = new Map();

    /**
     * Thời gian cache hợp lệ (ms) - mặc định 5 phút
     * Sau thời gian này sẽ gọi lại API để đảm bảo dữ liệu mới nhất
     */
    this.cacheTimeout = 5 * 60 * 1000;
  }

  /**
   * Kiểm tra cache có còn hợp lệ không
   * @param {string} field - Tên field cần check, fieldName: { value: string, exists: boolean, timestamp: number }
   * @param {string} value - Giá trị cần check
   * @returns {boolean} - true nếu cache còn hợp lệ
   */
  isCacheValid(field, value) {
    // Lấy ra cache theo field name
    const cached = this.serverCheckCache.get(field);
    // Nếu ko có thì trả false
    if (!cached) return false;

    // Kiểm tra giá trị nhập vào có = giá trị cached không
    const isValueMatch = cached.value === value;

    // Kiểm tra cached còn hạn không
    const isNotExpired = Date.now() - cached.timestamp < this.cacheTimeout;

    return isValueMatch && isNotExpired;
  }

  /**
   * Lưu kết quả check server vào cache
   * @param {string} field - Tên field
   * @param {string} value - Giá trị đã check
   * @param {boolean} exists - Kết quả có tồn tại hay không
   */
  setCacheResult(field, value, exists) {
    this.serverCheckCache.set(field, {
      value,
      exists,
      timestamp: Date.now(),
    });
  }

  /**
   * Lấy kết quả từ cache
   * @param {string} field - Tên field
   * @returns {object|null} - Object chứa kết quả hoặc null
   */
  getCacheResult(field) {
    return this.serverCheckCache.get(field) || null;
  }

  /**
   * Xóa cache của một field cụ thể
   * @param {string} field - Tên field cần xóa cache
   */
  clearCache(field) {
    this.serverCheckCache.delete(field);
  }

  /**
   * Xóa toàn bộ cache
   * Sử dụng khi cần reset form hoàn toàn
   */
  clearAllCache() {
    this.serverCheckCache.clear();
  }

  /**
   * Validate client-side: required + pattern
   * @param {string} value - Giá trị cần validate
   * @param {object} rule - Rule validation từ validationRules
   * @returns {string} - Thông báo lỗi (rỗng nếu hợp lệ)
   */
  validateClientSide(value, rule) {
    // Kiểm tra required
    if (rule.required && !value) {
      return rule.required;
    }

    // Kiểm tra pattern (regex)
    if (rule.pattern && value && !rule.pattern.regex.test(value)) {
      return rule.pattern.message;
    }

    return ""; // Không có lỗi
  }

  /**
   * Validate server-side: kiểm tra trùng lặp
   * @param {string} field - Tên field cần validate
   * @param {string} value - Giá trị cần validate
   * @param {object} rule - Rule validation
   * @param {string|null} currentId - ID của record đang sửa (null nếu thêm mới)
   * @returns {Promise<string>} - Thông báo lỗi (rỗng nếu hợp lệ)
   */
  async validateServerSide(field, value, rule, currentId = null) {
    // Nếu rule không có phần serverCheck hoặc chưa nhập giá trị thì không cần check server
    if (!rule.serverCheck || !value) {
      return "";
    }

    // Kiểm tra cache trước, nếu trả về true thì không gọi API
    if (this.isCacheValid(field, value)) {
      const cached = this.getCacheResult(field);
      if (cached.exists) {
        return this.getServerErrorMessage(rule.serverCheckField);
      }
      return ""; // Cache cho biết không trùng
    }

    // Gọi API kiểm tra trùng lặp
    try {
      let exists = false;

      if (rule.serverCheckField === "email") {
        const res = await CustomersAPI.checkEmailExist({
          email: value,
          id: currentId,
        });
        exists = res.data.data;
      } else if (rule.serverCheckField === "phone") {
        const res = await CustomersAPI.checkPhoneExist({
          phoneNumber: value,
          id: currentId,
        });
        exists = res.data.data;
      }

      // Lưu kết quả vào cache
      this.setCacheResult(field, value, exists);

      if (exists) {
        return this.getServerErrorMessage(rule.serverCheckField);
      }

      return ""; // Không trùng
    } catch (error) {
      console.error(`Lỗi khi kiểm tra ${field}:`, error);
      return `Lỗi khi kiểm tra ${field}`;
    }
  }

  /**
   * Lấy thông báo lỗi cho server check
   * @param {string} fieldType - Loại field ('email' | 'phone')
   * @returns {string} - Thông báo lỗi
   */
  getServerErrorMessage(fieldType) {
    const messages = {
      email: "Email đã tồn tại",
      phone: "Số điện thoại đã tồn tại",
    };
    return messages[fieldType] || "Dữ liệu đã tồn tại";
  }

  /**
   * Validate một field hoàn chỉnh (client + server)
   * @param {string} field - Tên field
   * @param {string} value - Giá trị cần validate
   * @param {boolean} checkServer - Có gọi API check server không
   * @param {string|null} currentId - ID record đang sửa
   * @returns {Promise<string>} - Thông báo lỗi (rỗng nếu hợp lệ)
   */
  async validateField(field, value, checkServer = false, currentId = null) {
    // Lấy ra rule của field
    const rule = validationRules[field];
    if (!rule) return ""; // Không có rule -> bỏ qua

    // Chuẩn hóa giá trị nhập vào
    const normalizedValue = value ? String(value).trim() : "";

    // Bước 1: Validate client-side (Khi nhập ô input)
    const clientError = this.validateClientSide(normalizedValue, rule);

    // Nếu có lỗi thì return
    if (clientError) return clientError;

    // Bước 2: Validate server-side (Khi blur ô input, tùy trường nếu checkServer = true)
    if (checkServer) {
      const serverError = await this.validateServerSide(field, normalizedValue, rule, currentId);

      // Nếu có lỗi thì return
      if (serverError) return serverError;
    } else {
      // checkServer = false, nhưng vẫn kiểm tra cache để hiển thị lỗi nhanh
      if (rule.serverCheck && this.isCacheValid(field, normalizedValue)) {
        const cached = this.getCacheResult(field);
        if (cached.exists) {
          return this.getServerErrorMessage(rule.serverCheckField);
        }
      }
    }

    return ""; // Không có lỗi
  }

  /**
   * Validate nhiều field cùng lúc
   * @param {Array<string>} fields - Danh sách tên field cần validate
   * @param {object} formData - Object chứa dữ liệu form
   * @param {boolean} checkServer - Có check server không
   * @param {string|null} currentId - ID record đang sửa
   * @returns {Promise<object>} - Object chứa lỗi { fieldName: errorMessage }
   */
  async validateMultipleFields(fields, formData, checkServer = false, currentId = null) {
    const errors = {};

    // Validate tuần tự để tránh race condition
    for (const field of fields) {
      const error = await this.validateField(field, formData[field], checkServer, currentId);
      if (error) {
        errors[field] = error;
      }
    }

    return errors;
  }

  /**
   * Kiểm tra form có lỗi không
   * @param {object} errors - Object chứa errors
   * @returns {boolean} - true nếu có lỗi
   */
  hasErrors(errors) {
    return Object.values(errors).some((error) => error !== "");
  }
}

/**
 * Export singleton instance để sử dụng chung trong toàn bộ app
 * Giúp cache được chia sẻ giữa các component
 */
export const validationManager = new ValidationManager();
