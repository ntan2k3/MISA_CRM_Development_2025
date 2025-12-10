import { reactive } from "vue";
import CustomersAPI from "@/apis/components/customers/CustomersAPI";
import {
  ERROR_MESSAGES,
  VALIDATION_PATTERNS,
  VALIDATION_RULES,
  FIELD_NAMES,
} from "@/commons/constants/customerConstants";

/**
 * Composable để xử lý validation cho customer form
 */
export function useCustomerValidation(customerId = null) {
  const errors = reactive({});
  const serverCheckCache = reactive(new Map());
  const validationTimers = reactive(new Map());

  /**
   * Validate required field
   */
  const validateRequired = (field, value) => {
    if (!value) {
      errors[field] = ERROR_MESSAGES.REQUIRED[field];
      return false;
    }
    return true;
  };

  /**
   * Validate pattern (regex)
   */
  const validatePattern = (field, value) => {
    const pattern = VALIDATION_PATTERNS[field];
    if (pattern && !pattern.test(value)) {
      errors[field] = ERROR_MESSAGES.PATTERN[field];
      return false;
    }
    return true;
  };

  /**
   * Kiểm tra cache server validation
   */
  const checkCache = (field, value) => {
    const cached = serverCheckCache.get(field);
    if (cached && cached.value === value) {
      if (cached.exists) {
        errors[field] = ERROR_MESSAGES.EXISTS[field];
        return false;
      }
      return true;
    }
    return null; // chưa có cache
  };

  /**
   * Gọi API check trùng - Generic function cho cả email và phone
   */
  const checkFieldExistence = async (field, value) => {
    const apiCalls = {
      [FIELD_NAMES.CUSTOMER_EMAIL]: () =>
        CustomersAPI.checkEmailExist({ email: value, id: customerId }),
      [FIELD_NAMES.CUSTOMER_PHONE]: () =>
        CustomersAPI.checkPhoneExist({ phoneNumber: value, id: customerId }),
    };

    const apiCall = apiCalls[field];
    if (!apiCall) return false;

    const res = await apiCall();
    return res.data.data;
  };

  /**
   * Validate với server (check trùng)
   */
  const validateServerCheck = async (field, value) => {
    // Kiểm tra cache trước
    const cacheResult = checkCache(field, value);
    if (cacheResult !== null) {
      return cacheResult;
    }

    try {
      const exists = await checkFieldExistence(field, value);

      // Lưu vào cache
      serverCheckCache.set(field, { value, exists });

      if (exists) {
        errors[field] = ERROR_MESSAGES.EXISTS[field];
        return false;
      }

      return true;
    } catch {
      errors[field] = ERROR_MESSAGES.SERVER_ERROR[field];
      return false;
    }
  };

  /**
   * Validate một field (chính)
   * @param {string} field - Tên field cần validate
   * @param {any} rawValue - Giá trị cần validate
   * @param {boolean} checkServer - Có gọi server check không
   */
  const validateField = async (field, rawValue, checkServer = false) => {
    const value = rawValue ? String(rawValue).trim() : "";
    errors[field] = "";

    const rule = VALIDATION_RULES[field];
    if (!rule) return true;

    // 1. Validate required
    if (rule.required && !validateRequired(field, value)) {
      return false;
    }

    // 2. Validate pattern
    if (rule.pattern && value && !validatePattern(field, value)) {
      return false;
    }

    // 3. Validate server check
    if (rule.serverCheck && value) {
      if (checkServer) {
        return await validateServerCheck(field, value);
      } else {
        // Chỉ check cache khi đang input
        const cacheResult = checkCache(field, value);
        if (cacheResult === false) {
          return false;
        }
      }
    }

    return true;
  };

  /**
   * Validate field với debounce (dùng khi input)
   */
  const validateFieldDebounced = (field, rawValue, delay = 300) => {
    // Clear timer cũ
    if (validationTimers.has(field)) {
      clearTimeout(validationTimers.get(field));
    }

    // Set timer mới
    const timer = setTimeout(async () => {
      await validateField(field, rawValue, false);
      validationTimers.delete(field);
    }, delay);

    validationTimers.set(field, timer);
  };

  /**
   * Validate tất cả các field bắt buộc
   */
  const validateAllRequired = async (formData) => {
    const results = await Promise.all([
      validateField(FIELD_NAMES.CUSTOMER_NAME, formData[FIELD_NAMES.CUSTOMER_NAME], false),
      validateField(FIELD_NAMES.CUSTOMER_EMAIL, formData[FIELD_NAMES.CUSTOMER_EMAIL], true),
      validateField(FIELD_NAMES.CUSTOMER_PHONE, formData[FIELD_NAMES.CUSTOMER_PHONE], true),
    ]);

    return results.every((result) => result === true);
  };

  /**
   * Clear lỗi của một field
   */
  const clearError = (field) => {
    errors[field] = "";
  };

  /**
   * Clear tất cả lỗi
   */
  const clearAllErrors = () => {
    Object.keys(errors).forEach((key) => {
      errors[key] = "";
    });
  };

  /**
   * Clear cache validation
   */
  const clearCache = () => {
    serverCheckCache.clear();
  };

  /**
   * Clear tất cả timers
   */
  const clearAllTimers = () => {
    validationTimers.forEach((timer) => clearTimeout(timer));
    validationTimers.clear();
  };

  return {
    errors,
    validateField,
    validateFieldDebounced,
    validateAllRequired,
    clearError,
    clearAllErrors,
    clearCache,
    clearAllTimers,
  };
}
