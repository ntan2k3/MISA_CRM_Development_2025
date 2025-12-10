<script setup>
import MsButton from "@/components/ms-button/MsButton.vue";
import MsInput from "@/components/ms-input/MsInput.vue";

import CustomersAPI from "@/apis/components/customers/CustomersAPI";
import { ref, computed, reactive, onMounted } from "vue";
import { useRouter, useRoute } from "vue-router";
import { message } from "ant-design-vue";

import "@/assets/css/customerFormView.css";

//#region Routers
// Khởi tạo router và route để điều hướng và lấy thông tin route hiện tại
const router = useRouter();
const route = useRoute();
//#endregion

//#region Layout fields
/**
 * Cấu hình danh sách các input bên trái của form
 * Mỗi object đại diện cho 1 ô input
 */
const leftFields = [
  { label: "Mã khách hàng", placeholder: "Mã tự sinh", model: "customerCode", disabled: true },
  { label: "Điện thoại", model: "customerPhoneNumber", required: true },
  { label: "Mã số thuế", model: "customerTaxCode" },
  { label: "Hàng hóa đã mua", model: "purchasedItemCode" },
  { label: "Tên hàng hóa đã mua", model: "purchasedItemName" },
];

/**
 * Cấu hình danh sách các input bên phải của form
 * Tương tự như leftFields nhưng có thêm select và date
 */
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
/**
 * Cấu hình rule validate cho từng field:
 * - required: bắt buộc nhập
 * - pattern: regex định dạng hợp lệ
 * - serverCheck: có cần check trùng trên server hay không
 */
const validationRules = {
  customerName: {
    required: "Tên khách hàng không được để trống",
  },
  customerEmail: {
    required: "Email không được để trống",
    pattern: { regex: /^[^\s@]+@[^\s@]+\.[^\s@]+$/, message: "Email không hợp lệ" },
    serverCheck: true,
  },
  customerPhoneNumber: {
    required: "Số điện thoại không được để trống",
    pattern: {
      regex: /^0\d{9,10}$/,
      message: "Số điện thoại phải đúng định dạng và đủ 10 - 11 số",
    },
    serverCheck: true,
  },
};
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

/** Khởi tạo value mặc định cho form và errors */
[...leftFields, ...rightFields].forEach((item) => {
  formData[item.model] = null;
  errors[item.model] = "";
});

/** Cache kết quả kiểm tra trùng server: tránh gọi API nhiều lần */
const serverCheckCache = reactive({
  customerEmail: { value: null, exists: false },
  customerPhoneNumber: { value: null, exists: false },
});

//#endregion

//#region Computed
/**
 * Kiểm tra đang ở chế độ sửa hay thêm mới
 */
const isEdit = computed(() => route.name === "customer-edit");
/**
 * Tiêu đề form dựa theo mode
 */
const title = computed(() => (isEdit.value ? "Sửa Khách hàng" : "Thêm Khách hàng"));

/**
 * Tính thứ tự tabindex hợp lý theo layout trái – phải
 */
const allFieldsInOrder = computed(() => {
  const maxLen = Math.max(leftFields.length, rightFields.length);
  const ordered = [];

  for (let i = 0; i < maxLen; i++) {
    if (leftFields[i]) ordered.push(leftFields[i]);
    if (rightFields[i]) ordered.push(rightFields[i]);
  }

  return ordered;
});

/**
 * Map: tên field -> tabindex tương ứng
 */
const tabindexMap = computed(() => {
  const map = {};
  allFieldsInOrder.value.forEach((f, index) => {
    map[f.model] = index + 1;
  });
  return map;
});

//#endregion

//#region Methods
/**
 * Điều hướng quay về danh sách khách hàng
 */
const handleCancel = () => {
  router.push("/customers");
};

/**
 * Validate 1 field (client + server)
 * @param {string} field
 * @param {boolean} checkExist - true: blur -> check server
 */
const validateField = async (field, checkExist = false) => {
  // Lấy ra value và khởi tạo lỗi cho các trường input
  const rawValue = formData[field];
  const value = rawValue ? String(rawValue).trim() : "";
  errors[field] = "";

  // Lấy ra các rule của từng trường
  const rule = validationRules[field];
  if (!rule) return true;

  // required
  if (rule.required && !value) {
    errors[field] = rule.required;
    return false;
  }

  // pattern
  if (rule.pattern && value && !rule.pattern.regex.test(value)) {
    errors[field] = rule.pattern.message;
    return false;
  }

  // server check (email / phone)
  if (rule.serverCheck && value) {
    // Không check server khi đang nhập -> chỉ check cache
    if (!checkExist) {
      if (serverCheckCache[field].value === value && serverCheckCache[field].exists) {
        errors[field] = field === "customerEmail" ? "Email đã tồn tại" : "Số điện thoại đã tồn tại";
        return false;
      }
      return true;
    }

    // blur -> gọi server khi:
    // - giá trị thay đổi
    // - cache chưa có
    if (serverCheckCache[field].value !== value || serverCheckCache[field].exists === undefined) {
      try {
        let exists = false;

        if (field === "customerEmail") {
          const res = await CustomersAPI.checkEmailExist({ email: value, id: route.params.id });
          exists = res.data.data;
        } else if (field === "customerPhoneNumber") {
          const res = await CustomersAPI.checkPhoneExist({
            phoneNumber: value,
            id: route.params.id,
          });
          exists = res.data.data;
        }

        // Lưu cache
        serverCheckCache[field] = { value, exists };

        if (exists) {
          errors[field] =
            field === "customerEmail" ? "Email đã tồn tại" : "Số điện thoại đã tồn tại";
          return false;
        }
      } catch {
        errors[field] = `Lỗi khi kiểm tra ${field}`;
        return false;
      }
    } else {
      // Dùng cache nếu chưa đổi
      if (serverCheckCache[field].exists) {
        errors[field] = field === "customerEmail" ? "Email đã tồn tại" : "Số điện thoại đã tồn tại";
        return false;
      }
    }
  }

  return true;
};

/**
 * Submit form chung cho cả thêm / sửa
 * @param {Function} afterSuccess callback khi submit thành công
 */
const handleSubmitForm = async (afterSuccess) => {
  isLoading.value = true;

  try {
    const validName = await validateField("customerName", false);
    const validEmail = await validateField("customerEmail", true);
    const validPhone = await validateField("customerPhoneNumber", true);

    if (!validName || !validEmail || !validPhone) return;

    if (isEdit.value) {
      // Cập nhật khách hàng
      const customerId = route.params.id;
      const res = await CustomersAPI.update(customerId, formData);
      const data = res.data.data;

      if (!data) {
        message.error("Khách hàng cần cập nhật không tồn tại hoặc ở trong thùng rác.", 2);
        return;
      }

      message.success("Cập nhật khách hàng thành công!", 2);
      router.push("/customers/add");
    } else {
      // Thêm mới khách hàng
      await CustomersAPI.add(formData);
      message.success("Thêm khách hàng thành công!", 2);
    }

    if (typeof afterSuccess === "function") afterSuccess();
  } catch (err) {
    message.error(err.response?.data?.error?.message || "Lỗi khi gửi dữ liệu", 2);
  } finally {
    isLoading.value = false;
  }
};

/**
 * Lưu và thêm tiếp
 */
const handleSaveAndAdd = () => {
  handleSubmitForm(() => {
    [...leftFields, ...rightFields].forEach((item) => (formData[item.model] = null));

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
    router.push("/customers");
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
    message.error(err.response?.data?.error?.message || "Lỗi khi tạo mã khách hàng", 2);
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
      message.error("Khách hàng không tồn tại hoặc ở trong thùng rác.", 2);
      return;
    }

    // Có thì gán cho formData để hiển thị lên các trường input
    Object.assign(formData, res.data.data);

    // Lấy URL avatar hiển thị
    const baseUrl = import.meta.env.VITE_API_BASE_URL;
    imageUrl.value = formData.customerAvatarUrl ? baseUrl + formData.customerAvatarUrl : "";
  } catch (err) {
    message.error(err.response?.data?.error?.message || "Lỗi khi lấy dữ liệu khách hàng", 2);
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

    imageUrl.value = baseUrl + tempUrl;
    formData.customerAvatarUrl = tempUrl;

    message.success("Tải ảnh lên thành công.", 2);
  } catch (err) {
    message.error(err.response?.data?.error?.message || "Tải ảnh lên thất bại", 2);
  } finally {
    isLoading.value = false;
    e.target.value = "";
  }
};

//#endregion

//#region Lifecycle Hooks

/**
 * On mounted:
 * - Nếu sửa -> load customer theo id
 * - Nếu thêm mới -> lấy mã khách hàng mới
 */
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
              <div class="form-col flex-col">
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
