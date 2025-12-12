/**
 * File chứa các constants dùng chung cho Customer module
 * Bao gồm: cấu hình form fields, table columns, options cho dropdowns, pagination
 */

//#region Form Fields Configuration

/**
 * Cấu hình danh sách các input bên trái của form
 * Mỗi object đại diện cho 1 ô input
 */
export const LEFT_FIELDS = [
  {
    label: "Mã khách hàng",
    placeholder: "Mã tự sinh",
    model: "customerCode",
    disabled: true,
  },
  {
    label: "Điện thoại",
    model: "customerPhoneNumber",
    required: true,
  },
  {
    label: "Mã số thuế",
    model: "customerTaxCode",
  },
  {
    label: "Hàng hóa đã mua",
    model: "purchasedItemCode",
  },
  {
    label: "Tên hàng hóa đã mua",
    model: "purchasedItemName",
  },
];

/**
 * Cấu hình danh sách các input bên phải của form
 * Tương tự như leftFields nhưng có thêm select và date
 */
export const RIGHT_FIELDS = [
  {
    label: "Tên khách hàng",
    model: "customerName",
    required: true,
  },
  {
    label: "Email",
    model: "customerEmail",
    required: true,
  },
  {
    label: "Loại khách hàng",
    type: "select",
    options: [
      { label: "NBH01", value: "NBH01" },
      { label: "LKHA", value: "LKHA" },
      { label: "VIP", value: "VIP" },
    ],
    model: "customerType",
    icon: "icon-bg icon-select-type icon-16",
  },
  {
    label: "Ngày mua hàng gần nhất",
    type: "date",
    placeholder: "DD/MM/YYYY",
    model: "lastPurchaseDate",
    icon: "icon-bg icon-calendar icon-16",
  },
  {
    label: "Địa chỉ (Giao hàng)",
    model: "customerAddr",
  },
];

//#endregion

//#region Table Configuration

/**
 * Cấu hình các cột cho bảng danh sách khách hàng
 */
export const TABLE_FIELDS = [
  {
    key: "customerType",
    label: "Loại khách hàng",
    type: "text",
    width: "175px",
  },
  {
    key: "customerCode",
    label: "Mã khách hàng",
    type: "text",
    width: "150px",
  },
  {
    key: "customerName",
    label: "Tên khách hàng",
    type: "text",
    width: "300px",
  },
  {
    key: "customerTaxCode",
    label: "Mã số thuế",
    type: "text",
    width: "160px",
  },
  {
    key: "customerPhoneNumber",
    label: "Điện thoại",
    type: "text",
    width: "160px",
    icon: "icon-bg icon-phone icon-16",
  },
  {
    key: "customerEmail",
    label: "Email",
    type: "text",
    width: "235px",
  },
  {
    key: "customerAddr",
    label: "Địa chỉ (Giao hàng)",
    type: "text",
    width: "300px",
  },
  {
    key: "lastPurchaseDate",
    label: "Ngày mua hàng gần nhất",
    type: "date",
    width: "200px",
  },
  {
    key: "purchasedItemCode",
    label: "Hàng hóa đã mua",
    type: "text",
    width: "300px",
  },
  {
    key: "purchasedItemName",
    label: "Tên hàng hóa đã mua",
    type: "text",
    width: "300px",
  },
];

//#endregion

//#region Dropdown Options

/**
 * Các tùy chọn số bản ghi trên mỗi trang
 */
export const PAGE_SIZE_OPTIONS = [
  { label: "10 Bản ghi trên trang", value: 10 },
  { label: "20 Bản ghi trên trang", value: 20 },
  { label: "50 Bản ghi trên trang", value: 50 },
  { label: "100 Bản ghi trên trang", value: 100 },
];

/**
 * Các tùy chọn loại khách hàng (bao gồm "Tất cả")
 * Dùng cho dropdown filter
 */
export const CUSTOMER_TYPE_OPTIONS = [
  { key: "", value: "Tất cả khách hàng" },
  { key: "VIP", value: "VIP" },
  { key: "LKHA", value: "LKHA" },
  { key: "NBH01", value: "NBH01" },
];

/**
 * Options cho dropdown gắn loại (không bao gồm "Tất cả")
 * Dùng cho chức năng gắn loại hàng loạt
 */
export const ASSIGN_TYPE_OPTIONS = [
  { label: "VIP", value: "VIP" },
  { label: "LKHA", value: "LKHA" },
  { label: "NBH01", value: "NBH01" },
];
//#endregion

//#region Default Values

/**
 * Thời gian debounce cho search input (milliseconds)
 */
export const SEARCH_DEBOUNCE_TIME = 300;

//#endregion

//#region Messages

/**
 * Các message thông báo dùng chung
 */
export const MESSAGES = {
  // Success messages
  ADD_SUCCESS: "Thêm khách hàng thành công!",
  UPDATE_SUCCESS: "Cập nhật khách hàng thành công!",
  DELETE_SUCCESS: (count) => `Xóa thành công ${count} khách hàng`,
  EXPORT_SUCCESS: "Xuất file thành công.",
  IMPORT_SUCCESS: (count) => `Import thành công ${count} bản ghi.`,
  UPLOAD_SUCCESS: "Tải ảnh lên thành công.",
  ASSIGN_TYPE_SUCCESS: "Đã gắn loại khách hàng thành công",

  // Warning messages
  VALIDATE_WARNING: "Vui lòng kiểm tra lại thông tin nhập vào!",
  SELECT_WARNING: "Vui lòng chọn ít nhất một khách hàng.",
  UPLOAD_FILE_WARNING: "Vui lòng tải lên file CSV trước khi gửi.",

  // Error messages
  NOT_FOUND_ERROR: "Khách hàng không tồn tại hoặc ở trong thùng rác.",
  UPDATE_NOT_FOUND_ERROR: "Khách hàng cần cập nhật không tồn tại hoặc ở trong thùng rác.",
  LOAD_DATA_ERROR: "Lỗi khi lấy dữ liệu",
  SUBMIT_ERROR: "Lỗi khi gửi dữ liệu",
  CUSTOMER_CODE_ERROR: "Lỗi khi tạo mã khách hàng",
  EXPORT_ERROR: "Lỗi khi xuất file.",
  IMPORT_ERROR: "Lỗi khi nhập file.",
  IMPORT_NO_DATA_ERROR: "Không bản nào import thành công do trùng dữ liệu.",
  UPLOAD_ERROR: "Tải ảnh lên thất bại",
  DELETE_BATCH_ERROR: "Thao tác xóa hàng loạt thất bại.",
  ASSIGN_TYPE_ERROR: "Gắn loại khách hàng thất bại.",

  // Confirm messages
  DELETE_CONFIRM: (count) => `Bạn có chắc chắn muốn xóa ${count} khách hàng đã chọn không?`,
};

//#endregion

//#region Routes

/**
 * Đường dẫn routes
 */
export const ROUTES = {
  LIST: "/customers",
  ADD: "/customers/add",
  EDIT: "customer-edit",
};

//#endregion
