/**
 * Constants cho Customer module
 * Tập trung tất cả magic strings và config
 */

// Field names
export const FIELD_NAMES = {
  CUSTOMER_CODE: "customerCode",
  CUSTOMER_NAME: "customerName",
  CUSTOMER_EMAIL: "customerEmail",
  CUSTOMER_PHONE: "customerPhoneNumber",
  CUSTOMER_TAX_CODE: "customerTaxCode",
  CUSTOMER_TYPE: "customerType",
  CUSTOMER_ADDR: "customerAddr",
  CUSTOMER_AVATAR_URL: "customerAvatarUrl",
  LAST_PURCHASE_DATE: "lastPurchaseDate",
  PURCHASED_ITEM_CODE: "purchasedItemCode",
  PURCHASED_ITEM_NAME: "purchasedItemName",
};

// Routes
export const ROUTES = {
  CUSTOMERS_LIST: "/customers",
  CUSTOMERS_ADD: "/customers/add",
  CUSTOMER_EDIT: "customer-edit",
};

// Error Messages
export const ERROR_MESSAGES = {
  REQUIRED: {
    [FIELD_NAMES.CUSTOMER_NAME]: "Tên khách hàng không được để trống",
    [FIELD_NAMES.CUSTOMER_EMAIL]: "Email không được để trống",
    [FIELD_NAMES.CUSTOMER_PHONE]: "Số điện thoại không được để trống",
  },
  PATTERN: {
    [FIELD_NAMES.CUSTOMER_EMAIL]: "Email không hợp lệ",
    [FIELD_NAMES.CUSTOMER_PHONE]: "Số điện thoại phải đúng định dạng và đủ 10 - 11 số",
  },
  EXISTS: {
    [FIELD_NAMES.CUSTOMER_EMAIL]: "Email đã tồn tại",
    [FIELD_NAMES.CUSTOMER_PHONE]: "Số điện thoại đã tồn tại",
  },
  SERVER_ERROR: {
    [FIELD_NAMES.CUSTOMER_EMAIL]: "Lỗi khi kiểm tra email",
    [FIELD_NAMES.CUSTOMER_PHONE]: "Lỗi khi kiểm tra số điện thoại",
  },
  API: {
    GET_NEW_CODE: "Lỗi khi tạo mã khách hàng",
    GET_CUSTOMER: "Lỗi khi lấy dữ liệu khách hàng",
    UPLOAD_AVATAR: "Tải ảnh lên thất bại",
    SUBMIT_FORM: "Lỗi khi gửi dữ liệu",
    NOT_FOUND: "Khách hàng không tồn tại hoặc ở trong thùng rác.",
    UPDATE_NOT_FOUND: "Khách hàng cần cập nhật không tồn tại hoặc ở trong thùng rác.",
  },
};

// Success Messages
export const SUCCESS_MESSAGES = {
  UPLOAD_AVATAR: "Tải ảnh lên thành công.",
  ADD_CUSTOMER: "Thêm khách hàng thành công!",
  UPDATE_CUSTOMER: "Cập nhật khách hàng thành công!",
};

// Validation Patterns
export const VALIDATION_PATTERNS = {
  [FIELD_NAMES.CUSTOMER_EMAIL]: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
  [FIELD_NAMES.CUSTOMER_PHONE]: /^0\d{9,10}$/,
};

// Validation Rules Configuration
export const VALIDATION_RULES = {
  [FIELD_NAMES.CUSTOMER_NAME]: { required: true },
  [FIELD_NAMES.CUSTOMER_EMAIL]: { required: true, pattern: true, serverCheck: true },
  [FIELD_NAMES.CUSTOMER_PHONE]: { required: true, pattern: true, serverCheck: true },
};

// Customer Types
export const CUSTOMER_TYPES = [
  { label: "NBH01", value: "NBH01" },
  { label: "LKHA", value: "LKHA" },
  { label: "VIP", value: "VIP" },
];

// Form Field Configurations
export const LEFT_FIELDS = [
  {
    label: "Mã khách hàng",
    placeholder: "Mã tự sinh",
    model: FIELD_NAMES.CUSTOMER_CODE,
    disabled: true,
  },
  {
    label: "Điện thoại",
    model: FIELD_NAMES.CUSTOMER_PHONE,
    required: true,
  },
  {
    label: "Mã số thuế",
    model: FIELD_NAMES.CUSTOMER_TAX_CODE,
  },
  {
    label: "Hàng hóa đã mua",
    model: FIELD_NAMES.PURCHASED_ITEM_CODE,
  },
  {
    label: "Tên hàng hóa đã mua",
    model: FIELD_NAMES.PURCHASED_ITEM_NAME,
  },
];

export const RIGHT_FIELDS = [
  {
    label: "Tên khách hàng",
    model: FIELD_NAMES.CUSTOMER_NAME,
    required: true,
  },
  {
    label: "Email",
    model: FIELD_NAMES.CUSTOMER_EMAIL,
    required: true,
  },
  {
    label: "Loại khách hàng",
    type: "select",
    options: CUSTOMER_TYPES,
    model: FIELD_NAMES.CUSTOMER_TYPE,
    icon: "icon-bg icon-select-type icon-16",
  },
  {
    label: "Ngày mua hàng gần nhất",
    type: "date",
    placeholder: "DD/MM/YYYY",
    model: FIELD_NAMES.LAST_PURCHASE_DATE,
    icon: "icon-bg icon-calendar icon-16",
  },
  {
    label: "Địa chỉ (Giao hàng)",
    model: FIELD_NAMES.CUSTOMER_ADDR,
  },
];

// Timing delay
export const DEBOUNCE_DELAY = 300; // ms

// Message Duration
export const MESSAGE_DURATION = 2; // seconds
