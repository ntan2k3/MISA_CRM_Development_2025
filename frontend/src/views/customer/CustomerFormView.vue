<script setup>
import MsButton from "@/components/ms-button/MsButton.vue";
import MsInput from "@/components/ms-input/MsInput.vue";

import CustomersAPI from "@/apis/components/customers/CustomersAPI";
import { useRouter, useRoute } from "vue-router";
import { ref, computed, reactive, onMounted } from "vue";
import { message } from "ant-design-vue";

import "@/assets/css/customerFormView.css";

//#region Routers
// Khởi tạo router và route để điều hướng và lấy thông tin route hiện tại
const router = useRouter();
const route = useRoute();
//#endregion

//#region Layout fields
// Cấu hình các trường thông tin cột bên trái
const leftFields = [
  { label: "Mã khách hàng", placeholder: "Mã tự sinh", model: "customerCode", disabled: true },
  { label: "Điện thoại", model: "customerPhoneNumber", required: true },
  { label: "Mã số thuế", model: "customerTaxCode" },
  { label: "Hàng hóa đã mua", model: "purchasedItemCode" },
  { label: "Tên hàng hóa đã mua", model: "purchasedItemName" },
];

// Cấu hình các trường thông tin cột bên phải
const rightFields = [
  { label: "Tên khách hàng", model: "customerName", required: true },
  { label: "Email", model: "customerEmail", required: true },
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
  { label: "Địa chỉ (Giao hàng)", model: "customerAddr" },
];
//#endregion

//#region Validation Rules
// Cấu hình các rule validate cho form
const validationRules = {
  customerName: {
    required: "Tên khách hàng không được để trống",
  },
  customerEmail: {
    required: "Email không được để trống",
    pattern: { regex: /^[^\s@]+@[^\s@]+\.[^\s@]+$/, message: "Email không hợp lệ" },
    serverCheck: true, // Kiểm tra tồn tại trên server
  },
  customerPhoneNumber: {
    required: "Số điện thoại không được để trống",
    pattern: {
      regex: /^0\d{9,10}$/,
      message: "Số điện thoại phải đúng định dạng và đủ 10 - 11 số",
    },
    serverCheck: true, // Kiểm tra tồn tại trên server
  },
};
//#endregion

//#region States
// Trạng thái loading
const isLoading = ref(false);

// URL hiển thị avatar
const imageUrl = ref(null);

// Dữ liệu form
const formData = reactive({});

// Dữ liệu lỗi từng field
const errors = reactive({});

// Khởi tạo các field cho formData
[...leftFields, ...rightFields].forEach((item) => (formData[item.model] = null));

// Khởi tạo các field cho errors
[...leftFields, ...rightFields].forEach((item) => (errors[item.model] = ""));
//#endregion

//#region Computed
// Kiểm tra chế độ edit hay thêm mới
const isEdit = computed(
  () => route.name === "customer-edit" || route.path.includes("/customers/edit")
);

// Tiêu đề form tùy theo mode
const title = computed(() => (isEdit.value ? "Sửa Khách hàng" : "Thêm Khách hàng"));

// Kiểm tra xem có lỗi nào tồn tại trong form hay không
const hasError = computed(() => {
  return Object.values(errors).some((err) => err && err.length > 0);
});
//#endregion

//#region Methods
/** Hủy form -> quay về danh sách khách hàng */
const handleCancel = () => {
  router.push("/customers");
};

/**
 * Validate một field cụ thể
 * @param {string} field Tên field cần validate
 * @returns {boolean} True nếu hợp lệ, false nếu không
 */
const validateField = async (field) => {
  const rawValue = formData[field];
  const value = rawValue !== null && rawValue !== undefined ? String(rawValue).trim() : "";
  errors[field] = "";

  const rule = validationRules[field];
  if (!rule) return true;

  // Kiểm tra required
  if (rule.required && !value) {
    errors[field] = rule.required;
    return false;
  }

  // Kiểm tra pattern
  if (rule.pattern && value && !rule.pattern.regex.test(value)) {
    errors[field] = rule.pattern.message;
    return false;
  }

  // Kiểm tra tồn tại trên server
  if (rule.serverCheck && value) {
    try {
      const res = await CustomersAPI.getAll({ search: value });
      const list = Array.isArray(res.data.data) ? res.data.data : [];
      const exists = list.some((item) => item.customerId !== route.params.id);
      if (exists) {
        errors[field] = field === "customerEmail" ? "Email đã tồn tại" : "Số điện thoại đã tồn tại";
        return false;
      }
    } catch {
      errors[field] = `Lỗi khi kiểm tra ${field}`;
      return false;
    }
  }

  return true;
};

/**
 * Gửi form chung
 * @param {Function} afterSuccess Hàm thực thi sau khi submit thành công
 */
const handleSubmitForm = async (afterSuccess) => {
  isLoading.value = true;

  try {
    const validName = await validateField("customerName");
    const validEmail = await validateField("customerEmail");
    const validPhone = await validateField("customerPhoneNumber");

    if (!validName || !validEmail || !validPhone) return;

    if (isEdit.value) {
      // Cập nhật khách hàng
      const customerId = route.params.id;
      const res = await CustomersAPI.update(customerId, formData);
      const data = res.data.data;

      if (!data) {
        message.error("Khách hàng cần cập nhật không tồn tại hoặc ở trong thùng rác.");
        return;
      }

      message.success("Cập nhật khách hàng thành công!");
      router.push("/customers/add");
    } else {
      // Thêm mới khách hàng
      await CustomersAPI.add(formData);
      message.success("Thêm khách hàng thành công!");
    }

    if (typeof afterSuccess === "function") afterSuccess();
  } catch (error) {
    const err = error.response?.data?.error;
    if (err) message.error(err.message);
    else message.error("Lỗi khi gửi dữ liệu lên.");
  } finally {
    isLoading.value = false;
  }
};

/** Xử lý lưu và thêm tiếp */
const handleSaveAndAdd = () => {
  handleSubmitForm(() => {
    [...leftFields, ...rightFields].forEach((item) => (formData[item.model] = null));

    // Reset avatar ở đây
    imageUrl.value = null;
    formData.customerAvatarUrl = null;

    getNewCustomerCode();
  });
};

/** Xử lý lưu và quay về danh sách */
const handleSave = () => {
  handleSubmitForm(() => {
    router.push("/customers");
  });
};

/** Lấy mã khách hàng mới từ server */
const getNewCustomerCode = async () => {
  try {
    const res = await CustomersAPI.getNewCustomerCode();
    formData.customerCode = res.data.data;
  } catch (error) {
    const err = error.response?.data?.error;
    if (err) message.error(err.message);
    else message.error("Lỗi khi tạo mới mã khách hàng.");
  }
};

/** Lấy thông tin khách hàng theo ID */
const getCustomerById = async () => {
  const customerId = route.params.id;
  if (!customerId) return;

  isLoading.value = true;
  try {
    const res = await CustomersAPI.getById(customerId);
    const data = res.data.data;
    if (!data) {
      message.error("Khách hàng không tồn tại hoặc ở trong thùng rác.");
      return;
    }

    // Có thì gán cho formData để hiển thị lên các trường input
    Object.assign(formData, res.data.data);

    // Lấy URL avatar hiển thị
    const baseUrl = import.meta.env.VITE_API_BASE_URL;
    imageUrl.value = formData.customerAvatarUrl ? baseUrl + formData.customerAvatarUrl : "";
  } catch (error) {
    const err = error.response?.data?.error;
    if (err) message.error(err.message);
    else message.error("Lỗi khi lấy dữ liệu khách hàng.");
  } finally {
    isLoading.value = false;
  }
};

/** Upload ảnh avatar tạm thời */
const uploadTempAvatar = async (e) => {
  const file = e.target.files[0];
  if (!file) return;

  isLoading.value = true;

  const form = new FormData();
  form.append("file", file);

  try {
    const res = await CustomersAPI.uploadTempAvatar(form);
    const baseUrl = import.meta.env.VITE_API_BASE_URL;
    const tempUrl = res.data?.tempAvatarUrl;

    imageUrl.value = baseUrl + tempUrl;
    formData.customerAvatarUrl = tempUrl;

    message.success("Tải ảnh lên thành công.");
  } catch (error) {
    const err = error.response?.data?.error;
    if (err) message.error(err.message);
    else message.error("Tải ảnh lên thất bại.");
  } finally {
    isLoading.value = false;
    e.target.value = "";
  }
};

//#endregion

//#region Lifecycle Hooks
onMounted(() => {
  isEdit.value ? getCustomerById() : getNewCustomerCode();
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
              <div class="form-col flex-col" :class="[hasError ? 'has-error-gap' : '']">
                <div
                  v-for="item in leftFields"
                  class="form-row flex items-center justify-between"
                  :key="item.label"
                >
                  <div class="form-label" :title="item.label">
                    {{ item.label }} <span v-if="item.required" style="color: red">*</span>
                  </div>
                  <div class="flex-1 flex-col gap-4 input-field">
                    <ms-input
                      :type="item.type"
                      :placeholder="item.placeholder || ''"
                      :options="item.options"
                      v-model="formData[item.model]"
                      :disabled="item.disabled"
                      :icon="item.icon"
                      @blur="() => validateField(item.model)"
                      @input="() => validateField(item.model)"
                    />
                    <div
                      v-if="errors[item.model]"
                      class="error-msg"
                      style="color: red; font-size: 12px"
                    >
                      {{ errors[item.model] }}
                    </div>
                  </div>
                </div>
              </div>

              <!-- Right Column -->
              <div class="form-col flex-col" :class="[hasError ? 'has-error-gap' : '']">
                <div
                  v-for="item in rightFields"
                  class="form-row flex items-center justify-between"
                  :key="item.label"
                >
                  <div class="form-label" :title="item.label">
                    {{ item.label }} <span v-if="item.required" style="color: red">*</span>
                  </div>
                  <div class="flex-1 flex-col gap-4 input-field">
                    <ms-input
                      :type="item.type"
                      :placeholder="item.placeholder || ''"
                      :options="item.options"
                      v-model="formData[item.model]"
                      :disabled="item.disabled"
                      :icon="item.icon"
                      @blur="() => validateField(item.model)"
                      @input="() => validateField(item.model)"
                    />
                    <div
                      v-if="errors[item.model]"
                      class="error-msg"
                      style="color: red; font-size: 12px"
                    >
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
