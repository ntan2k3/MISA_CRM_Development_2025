<script setup>
//#region Import
import MsButton from "@/components/ms-button/MsButton.vue";
import MsInput from "@/components/ms-input/MsInput.vue";

import CustomersAPI from "@/apis/components/customers/CustomersAPI";
import { ref, computed, reactive, onMounted, onUnmounted, nextTick } from "vue";
import { useRouter, useRoute } from "vue-router";
import { message } from "ant-design-vue";

import { validationRules, validationManager } from "@/utils/validation.js";
import {
  LEFT_FIELDS,
  RIGHT_FIELDS,
  MESSAGES,
  ROUTES,
} from "@/commons/constants/customerConstants.js";

import "@/assets/css/customerFormView.css";

//#endregion

//#region Routers
// Khởi tạo router và route để điều hướng và lấy thông tin route hiện tại
const router = useRouter();
const route = useRoute();
//#endregion

//#region States
/** Loading khi submit hoặc call API */
const isLoading = ref(false);

/** Link ảnh avatar hiển thị */
const imageUrl = ref(null);

/** Dữ liệu form chính */
const formData = reactive({});

/** Lỗi validate cho từng field */
const errors = reactive({});

/** Khởi tạo value mặc định cho formData và errors */
[...LEFT_FIELDS, ...RIGHT_FIELDS].forEach((item) => {
  formData[item.model] = null;
  errors[item.model] = "";
});

/** Thêm ref để tham chiếu đến input đầu tiên */
const firstInputRef = ref(null);
//#endregion

//#region Computed
/**
 * Kiểm tra đang ở chế độ sửa hay thêm mới
 */
const isEdit = computed(() => route.name === ROUTES.EDIT);
/**
 * Tiêu đề form dựa theo mode
 */
const title = computed(() => (isEdit.value ? "Sửa Khách hàng" : "Thêm Khách hàng"));

/**
 * Tính thứ tự tabindex hợp lý theo layout trái – phải
 * Ví dụ với 5 trường bên trái và 5 trường bên phải:
 * [LEFT_FIELDS[0], RIGHT_FIELDS[0], LEFT_FIELDS[1], RIGHT_FIELDS[1], ...]
 *
 * Mục đích: để tính toán tabindex hợp lý khi người dùng nhấn Tab
 * (di chuyển từ trái sang phải, từ trên xuống dưới)
 */
const allFieldsInOrder = computed(() => {
  // Tìm độ dài lớn nhất giữa 2 cột
  const maxLen = Math.max(LEFT_FIELDS.length, RIGHT_FIELDS.length);
  const ordered = [];

  // Duyệt qua từng hàng, lấy trường trái trước, sau đó trường phải
  for (let i = 0; i < maxLen; i++) {
    if (LEFT_FIELDS[i]) ordered.push(LEFT_FIELDS[i]);
    if (RIGHT_FIELDS[i]) ordered.push(RIGHT_FIELDS[i]);
  }

  return ordered;
});

/**
 * Tạo một object map để tra cứu nhanh tabindex của mỗi trường
 * Key: tên trường (model)
 * Value: số thứ tự tabindex (bắt đầu từ 1)
 *
 * Ví dụ:
 * {
 *   "customerCode": 1,
 *   "customerName": 2,
 *   "customerPhoneNumber": 3,
 *   ...
 * }
 *
 * Mục đích: để gán tabindex cho các input, giúp người dùng có thể
 * di chuyển giữa các trường bằng phím Tab theo đúng thứ tự
 */
const tabindexMap = computed(() => {
  const map = {};
  allFieldsInOrder.value.forEach((f, index) => {
    map[f.model] = index + 1;
  });
  return map;
});

//#endregion

//#region Validation Methods
/**
 * Validate một field với validation manager
 * @param {string} field - Tên field cần validate
 * @param {boolean} checkServer - Có check server không (true khi blur)
 * @returns {Promise<boolean>} - true nếu hợp lệ (không có lỗi), false nếu có lỗi
 */
const validateField = async (field, checkServer = false) => {
  // Lấy ID hiện tại nếu đang ở chế độ sửa
  const currentId = isEdit.value ? route.params.id : null;

  // Gọi validation manager để validate
  const error = await validationManager.validateField(
    field,
    formData[field],
    checkServer,
    currentId
  );

  // Cập nhật kết quả vào object errors để hiển thị lên UI
  errors[field] = error;

  return !error; // Return true nếu không có lỗi
};

/**
 * Validate tất cả các field required trước khi submit
 * @returns {Promise<boolean>} - true nếu tất cả hợp lệ
 */
const validateAllRequiredFields = async () => {
  // Lấy danh sách field có rule validation
  const fieldsToValidate = Object.keys(validationRules);

  // Validate tất cả field (với server check)
  const validationErrors = await validationManager.validateMultipleFields(
    fieldsToValidate,
    formData,
    true, // Check server
    isEdit.value ? route.params.id : null
  );

  // Cập nhật errors
  Object.keys(validationErrors).forEach((field) => {
    errors[field] = validationErrors[field];
  });

  // Return true nếu không có lỗi
  return !validationManager.hasErrors(validationErrors);
};
//#endregion

//#region Methods
/**
 * Điều hướng quay về danh sách khách hàng
 */
const handleCancel = () => {
  router.push(ROUTES.LIST);
};

/**
 * Submit form chung cho cả thêm / sửa
 * @param {Function} afterSuccess callback khi submit thành công
 */
const handleSubmitForm = async (afterSuccess) => {
  isLoading.value = true;

  try {
    const isValid = await validateAllRequiredFields();
    if (!isValid) {
      message.warning(MESSAGES.VALIDATE_WARNING, 2);
      return;
    }

    if (isEdit.value) {
      // Cập nhật khách hàng
      const customerId = route.params.id;
      const res = await CustomersAPI.update(customerId, formData);
      const data = res.data.data;

      if (!data) {
        message.error(MESSAGES.UPDATE_NOT_FOUND_ERROR, 2);
        return;
      }

      message.success(MESSAGES.UPDATE_SUCCESS, 2);
      router.push(ROUTES.ADD);
    } else {
      // Thêm mới khách hàng
      await CustomersAPI.add(formData);
      message.success(MESSAGES.ADD_SUCCESS, 2);
    }

    if (typeof afterSuccess === "function") afterSuccess();
  } catch (err) {
    message.error(err.response?.data?.error?.message || MESSAGES.SUBMIT_ERROR, 2);
  } finally {
    isLoading.value = false;
  }
};

/**
 * Lưu và thêm tiếp
 */
const handleSaveAndAdd = () => {
  handleSubmitForm(() => {
    [...LEFT_FIELDS, ...RIGHT_FIELDS].forEach((item) => (formData[item.model] = null));

    // Reset avatar ở đây
    imageUrl.value = null;
    formData.customerAvatarUrl = null;

    getNewCustomerCode();
  });
};

/**
 * Lưu và quay về danh sách
 */
const handleSave = () => {
  handleSubmitForm(() => {
    router.push(ROUTES.LIST);
  });
};

/**
 * Lấy mã khách hàng mới từ server
 */
const getNewCustomerCode = async () => {
  try {
    const res = await CustomersAPI.getNewCustomerCode();
    formData.customerCode = res.data.data;
  } catch (err) {
    message.error(err.response?.data?.error?.message || MESSAGES.CUSTOMER_CODE_ERROR, 2);
  }
};

/**
 * Lấy thông tin khách hàng theo ID khi sửa
 */
const getCustomerById = async () => {
  const customerId = route.params.id;
  if (!customerId) return;

  isLoading.value = true;
  try {
    const res = await CustomersAPI.getById(customerId);
    const data = res.data.data;
    if (!data) {
      message.error(MESSAGES.NOT_FOUND_ERROR, 2);
      return;
    }

    // Có thì gán cho formData để hiển thị lên các trường input
    Object.assign(formData, res.data.data);

    // Lấy URL avatar hiển thị
    const baseUrl = import.meta.env.VITE_API_BASE_URL;
    imageUrl.value = formData.customerAvatarUrl ? baseUrl + formData.customerAvatarUrl : "";
  } catch (err) {
    message.error(err.response?.data?.error?.message || MESSAGES.LOAD_DATA_ERROR, 2);
  } finally {
    isLoading.value = false;
  }
};

/**
 * Upload ảnh avatar tạm thời
 */
const uploadTempAvatar = async (e) => {
  const file = e.target.files[0];
  if (!file) return;

  isLoading.value = true;

  const form = new FormData();
  form.append("file", file);

  try {
    const res = await CustomersAPI.uploadTempAvatar(form);
    const baseUrl = import.meta.env.VITE_API_BASE_URL;
    const tempUrl = res.data?.data;

    // Ghép URL đầy đủ để hiển thị ảnh
    imageUrl.value = baseUrl + tempUrl;

    // Lưu đường dẫn vào form để gửi lên khi submit
    formData.customerAvatarUrl = tempUrl;

    message.success(MESSAGES.UPLOAD_SUCCESS, 2);
  } catch (err) {
    message.error(err.response?.data?.error?.message || MESSAGES.UPLOAD_ERROR, 2);
  } finally {
    isLoading.value = false;
    e.target.value = "";
  }
};

/**
 * Function để set ref cho input customerName
 * @param {string} model - Tên model của field
 * @returns {Function} - Hàm callback nhận element
 */
const setInputRef = (model) => {
  return (el) => {
    if (model === "customerName") {
      firstInputRef.value = el;
    }
  };
};

//#endregion

//#region Lifecycle Hooks

/**
 * On mounted:
 * - Nếu sửa -> load customer theo id
 * - Nếu thêm mới -> lấy mã khách hàng mới
 * - Focus vào input customerName
 */
onMounted(async () => {
  // Đợi API load xong
  if (isEdit.value) {
    await getCustomerById();
  } else {
    await getNewCustomerCode();
  }

  // Sau khi load xong, focus vào input customerName
  await nextTick();

  // Kiểm tra firstInputRef.value có tồn tại và có method focus không
  if (firstInputRef.value && typeof firstInputRef.value.focus === "function") {
    firstInputRef.value.focus();
  }
});

/**
 * onUnmounted: Được gọi TRƯỚC KHI component bị hủy (unmount)
 *
 * Nhiệm vụ: Dọn dẹp các tài nguyên để tránh memory leak
 *
 */
onUnmounted(() => {
  // Clear cache khi component unmount để tránh memory leak
  validationManager.clearAllCache();
});
//#endregion
</script>

<template>
  <div class="customer-form-container flex-col flex-1">
    <!-- Header -->
    <div class="customer-header flex">
      <div class="customer-toolbar flex-1 flex items-center justify-between">
        <!-- Left -->
        <div class="toolbar-left flex items-center">
          <div class="toolbar-title">{{ title }}</div>
          <div v-if="!isEdit" class="toolbar-select flex items-center">
            Mẫu tiêu chuẩn
            <div class="icon-bg icon-dropdown icon-16"></div>
          </div>
          <div class="toolbar-layout">Sửa bố cục</div>
        </div>

        <!-- Right -->
        <div class="toolbar-right flex items-center">
          <div class="toolbar-buttons flex">
            <ms-button
              type="default"
              variant="solid"
              class="btn-cancel"
              positionIcon="left"
              @click="handleCancel"
              >Hủy bỏ</ms-button
            >
            <ms-button
              type="primary"
              variant="outlined"
              class="btn-save-add"
              positionIcon="left"
              @click="handleSaveAndAdd"
              >Lưu và thêm</ms-button
            >
            <ms-button
              type="primary"
              variant="solid"
              class="btn-save"
              positionIcon="left"
              @click="handleSave"
              >Lưu</ms-button
            >
          </div>
        </div>
      </div>
    </div>

    <!-- Content -->
    <div class="customer-content flex-col flex-1">
      <!-- Image Section -->
      <a-spin :spinning="isLoading">
        <div class="form-section-image">
          <div class="form-title-image">Ảnh</div>
          <div class="form-avatar">
            <input type="file" class="input-upload-image" @change="uploadTempAvatar" />
            <img v-if="imageUrl" :src="imageUrl" class="avatar-preview" />
          </div>
        </div>

        <!-- General Info -->
        <div class="form-info">
          <div class="form-header">Thông tin chung</div>
          <div class="form-body">
            <div class="form-grid flex">
              <!-- Left Column -->
              <div class="form-col flex-col">
                <div
                  v-for="item in LEFT_FIELDS"
                  class="form-row flex items-center justify-between"
                  :key="item.label"
                >
                  <div class="form-label" :title="item.label">
                    {{ item.label }} <span v-if="item.required" style="color: red">*</span>
                  </div>
                  <div class="flex-1 flex-col gap-4 input-field">
                    <ms-input
                      :ref="setInputRef(item.model)"
                      :type="item.type"
                      :placeholder="item.placeholder || ''"
                      :options="item.options"
                      v-model="formData[item.model]"
                      :disabled="item.disabled"
                      :icon="item.icon"
                      :tabindex="tabindexMap[item.model]"
                      @blur="() => validateField(item.model, true)"
                      @input="() => validateField(item.model, false)"
                    />
                    <div v-if="errors[item.model]" class="error-msg">
                      {{ errors[item.model] }}
                    </div>
                  </div>
                </div>
              </div>

              <!-- Right Column -->
              <div class="form-col flex-col">
                <div
                  v-for="item in RIGHT_FIELDS"
                  class="form-row flex items-center justify-between"
                  :key="item.label"
                >
                  <div class="form-label" :title="item.label">
                    {{ item.label }} <span v-if="item.required" style="color: red">*</span>
                  </div>
                  <div class="flex-1 flex-col gap-4 input-field">
                    <ms-input
                      :ref="setInputRef(item.model)"
                      :type="item.type"
                      :placeholder="item.placeholder || ''"
                      :options="item.options"
                      v-model="formData[item.model]"
                      :disabled="item.disabled"
                      :icon="item.icon"
                      :tabindex="tabindexMap[item.model]"
                      @blur="() => validateField(item.model, true)"
                      @input="() => validateField(item.model, false)"
                    />
                    <div v-if="errors[item.model]" class="error-msg">
                      {{ errors[item.model] }}
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </a-spin>
    </div>
  </div>
</template>

<style scoped></style>
