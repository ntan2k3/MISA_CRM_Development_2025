<script setup>
import MsButton from "@/components/ms-button/MsButton.vue";
import MsTable from "@/components/ms-table/MsTable.vue";
import MsInput from "@/components/ms-input/MsInput.vue";
import MsModal from "@/components/ms-modal/MsModal.vue";
import CustomersAPI from "@/apis/components/customers/CustomersAPI";
import { Modal } from "ant-design-vue";

import { useRouter, useRoute } from "vue-router";
import { onMounted, ref, watch } from "vue";
import { debounce } from "lodash";
import { message } from "ant-design-vue";
import { usePagination } from "@/utils/hooks/usePagination.js";

import "@/assets/css/customerView.css";

//#region Router
// Khởi tạo router để điều hướng
const router = useRouter();
const route = useRoute();
//#endregion

//#region States
// Trạng thái loading
const loading = ref(false);

// Modal import file Excel
const isOpenImportModal = ref(false);

// Modal gắn loại khách hàng
const isAssignTypeModalOpen = ref(false);
const selectedCustomerType = ref("");

// File upload và trạng thái
const uploadedFile = ref(null);
const uploadSuccess = ref(false);

// Giá trị tìm kiếm
const searchValue = ref("");

// Danh sách ID các row đã chọn
const selectedIds = ref([]);

// Tổng số item được chọn
const checkedCount = ref(0);

// Danh sách dữ liệu bảng
const rows = ref([]);

// Sắp xếp
const sortBy = ref("");
const sortDirection = ref("asc");

// Lưu filter hiện tại
const filters = ref({});

//#endregion

//#region Pagination (usePagination)
const { pageSize, pageNumber, total, totalPages, nextPage, prevPage, startPage, endPage } =
  usePagination(100);
//#endregion

//#region Table & Pagination Variables
// Cấu hình các cột cho bảng
const fields = [
  {
    key: "customerType",
    label: "Loại khách hàng",
    type: "text",
    width: "175px",
    filter: true,
    options: [
      { key: "", value: "Tất cả" },
      { key: "VIP", value: "VIP" },
      { key: "LKHA", value: "LKHA" },
      { key: "NBH01", value: "NBH01" },
    ],
  },
  { key: "customerCode", label: "Mã khách hàng", type: "text", width: "150px" },
  { key: "customerName", label: "Tên khách hàng", type: "text", width: "300px" },
  { key: "customerTaxCode", label: "Mã số thuế", type: "text", width: "160px" },
  { key: "customerAddr", label: "Địa chỉ (Giao hàng)", type: "text", width: "300px" },
  {
    key: "customerPhoneNumber",
    label: "Điện thoại",
    type: "text",
    width: "160px",
    icon: "icon-bg icon-phone icon-16",
  },
  { key: "customerEmail", label: "Email", type: "text", width: "235px" },
  { key: "lastPurchaseDate", label: "Ngày mua hàng gần nhất", type: "date", width: "200px" },
  { key: "purchasedItemCode", label: "Hàng hóa đã mua", type: "text", width: "300px" },
  { key: "purchasedItemName", label: "Tên hàng hóa đã mua", type: "text", width: "300px" },
];

// Các tùy chọn pageSize
const pageSizeOptions = [
  { label: "10 Bản ghi trên trang", value: 10 },
  { label: "20 Bản ghi trên trang", value: 20 },
  { label: "50 Bản ghi trên trang", value: 50 },
  { label: "100 Bản ghi trên trang", value: 100 },
];
//#endregion

//#region Methods
/** Cập nhật danh sách row được chọn */
const handleSelectionChange = (items) => {
  selectedIds.value = items.map((i) => i.customerId);
  checkedCount.value = selectedIds.value.length;
};

/** Mở modal import file Excel */
const openImportModal = () => {
  isOpenImportModal.value = true;
  uploadedFile.value = null;
  uploadSuccess.value = false;
};

/** Đóng modal import */
const closeImportModal = () => {
  isOpenImportModal.value = false;
};

/** Mở modal gắn loại khách hàng */
const openAssignTypeModal = () => {
  isAssignTypeModalOpen.value = true;
  selectedCustomerType.value = ""; // Reset khi mở modal
};

/** Đóng modal gắn loại khách hàng */
const closeAssignTypeModal = () => {
  isAssignTypeModalOpen.value = false;
};

/** Xử lý khi chọn file upload */
const handleFileChange = (e) => {
  const file = e.target.files[0];
  if (file) {
    uploadedFile.value = file;
    uploadSuccess.value = true;

    e.target.value = null;
  }
};

/** Chuyển sang view thêm khách hàng */
const handleAdd = () => {
  router.push("/customers/add");
};

/** Chuyển sang view sửa khách hàng */
const handleEdit = (id) => {
  if (!id) return;
  router.push({ name: "customer-edit", params: { id } });
};

/** Áp dụng filter từ bảng */
const handleApplyFilter = async ({ fieldKey, value }) => {
  if (value === "" || value == null) {
    delete filters.value[fieldKey];
  } else {
    filters.value[fieldKey] = value;
  }
  pageNumber.value = 1;
};

/** Lấy tổng số bản ghi */
const getTotalData = async () => {
  try {
    const res = await CustomersAPI.getTotalData({
      customerType: filters.value.customerType || undefined,
      search: searchValue.value || undefined,
    });
    total.value = res.data.data;
  } catch (error) {
    console.error(error);
  }
};

/** Lấy dữ liệu bảng từ server */
const loadData = async () => {
  loading.value = true;
  try {
    const res = await CustomersAPI.getAll({
      pageSize: pageSize.value,
      pageNumber: pageNumber.value,
      search: searchValue.value,
      sortBy: sortBy.value,
      sortDirection: sortDirection.value,
      customerType: filters.value.customerType || undefined,
    });
    rows.value = res.data.data;
  } catch (error) {
    const err = error.response?.data?.error;
    if (err) message.error(err.message);
    else message.error("Lỗi khi lấy dữ liệu");
  } finally {
    loading.value = false;
  }
};

/** Xuất file CSV/Excel các bản ghi được chọn */
const handleExportCsv = async (ids) => {
  try {
    const res = await CustomersAPI.exportCsv(ids, {
      responseType: "blob",
    });

    // Lấy filename từ header nếu backend trả về
    const contentDisposition = res.headers["content-disposition"];
    let fileName = "customers.csv";

    if (contentDisposition) {
      const match = contentDisposition.match(/filename="?(.+)"?/);
      if (match && match[1]) fileName = match[1];
    }

    // Tạo URL tạm cho file blob
    const url = window.URL.createObjectURL(new Blob([res.data]));
    const link = document.createElement("a");

    link.href = url;
    link.setAttribute("download", fileName);
    document.body.appendChild(link);

    link.click();
    document.body.removeChild(link);

    window.URL.revokeObjectURL(url);

    message.success("Xuất file thành công.");
  } catch (error) {
    const err = error.response?.data?.error;
    if (err) message.error(err.message);
    else message.error("Lỗi khi xuất file.");
  }
};

/** Import file CSV/Excel */
const handleImportCsv = async () => {
  if (!uploadedFile.value) {
    message.warning("Vui lòng tải lên file CSV trước khi gửi.");
    return;
  }

  loading.value = true;
  const formData = new FormData();
  formData.append("file", uploadedFile.value);

  try {
    const res = await CustomersAPI.importCsv(formData);

    message.success(`${res.data.data}`);

    // Đóng import modal
    closeImportModal();

    // reset lại page và file
    pageNumber.value = 1;
    uploadedFile.value = null;
    uploadSuccess.value = false;

    //load lại data
    await loadData();
    await getTotalData();
  } catch (error) {
    const err = error.response?.data?.error;
    if (err) message.error(err.message);
    else message.error("Lỗi khi nhập file.");
  } finally {
    loading.value = false;
  }
};

/** Xóa mềm nhiều khách hàng cùng lúc */
const handleSoftDeleteMany = (ids) => {
  if (selectedIds.value.length === 0) {
    message.warning("Vui lòng chọn ít nhất một khách hàng.");
    return;
  }

  Modal.confirm({
    title: "Xác nhận xóa",
    content: `Bạn có chắc chắn muốn xóa ${selectedIds.value.length} khách hàng đã chọn không?`,
    okText: "Xóa",
    okType: "danger",
    cancelText: "Hủy",
    async onOk() {
      loading.value = true;
      try {
        const res = await CustomersAPI.softDeleteMany(ids);
        await loadData();
        await getTotalData();
        selectedIds.value = [];
        checkedCount.value = 0;
        message.success(`${res.data.data}`);
      } catch (error) {
        const err = error.response?.data?.error;
        if (err) message.error(err.message);
        else message.error("Thao tác xóa hàng loạt thất bại.");
      } finally {
        loading.value = false;
      }
    },
  });
};

/** Gắn loại khách hàng hàng loạt */
const assignCustomerType = async () => {
  if (!selectedCustomerType.value) {
    message.warning("Vui lòng chọn một loại khách hàng.");
    return;
  }

  loading.value = true;
  try {
    await CustomersAPI.assignCustomerType(selectedIds.value, selectedCustomerType.value);
    message.success(`Gắn loại khách hàng thành công`);
    closeAssignTypeModal();
    loadData();
  } catch (error) {
    const err = error.response?.data?.error;
    if (err) message.error(err.message);
    else message.error("Gắn loại khách hàng thất bại.");
  } finally {
    loading.value = false;
  }
};

/** Debounce load dữ liệu khi tìm kiếm */
const debouncedLoadData = debounce(() => {
  pageNumber.value = 1;
  loadData();
  getTotalData();
}, 500);
//#endregion

//#region Lifecycle Hooks
onMounted(() => {
  loadData();
  getTotalData();
});

onMounted(() => {
  // Lấy query từ URL
  const { search, page, size, sortBy: sBy, sortDirection: sDir, customerType } = route.query;

  if (search) searchValue.value = search;
  if (page) pageNumber.value = Number(page);
  if (size) pageSize.value = Number(size);
  if (sBy) sortBy.value = sBy;
  if (sDir) sortDirection.value = sDir;
  if (customerType) filters.value.customerType = customerType;

  // Load dữ liệu
  loadData();
  getTotalData();
});

//#endregion

//#region Watchers

// Watch pageSize để reset pageNumber & loadData
watch(pageSize, () => {
  pageNumber.value = 1;
  loadData();
});

// Watch pageNumber, sortBy, sortDirection để loadData
watch([pageNumber, sortBy, sortDirection, filters], () => {
  loadData();
});

// Watch searchValue để debounce
watch(searchValue, () => {
  debouncedLoadData();
});

// Watch filter để loadData và getTotalData
watch(
  filters,
  () => {
    pageNumber.value = 1;
    loadData();
    getTotalData();
  },
  { deep: true }
);

// Đồng bộ URL với các state: search, filter, pagination, sort
watch(
  [searchValue, pageNumber, pageSize, sortBy, sortDirection, filters],
  () => {
    router.replace({
      query: {
        search: searchValue.value || undefined,
        page: pageNumber.value !== 1 ? pageNumber.value : undefined,
        size: pageSize.value !== 100 ? pageSize.value : undefined,
        sortBy: sortBy.value || undefined,
        sortDirection: sortDirection.value !== "asc" ? sortDirection.value : undefined,
        customerType: filters.value.customerType || undefined,
      },
    });
  },
  { deep: true }
);

//#endregion
</script>

<template>
  <div class="customer-view flex-col flex-1">
    <!-- Header -->
    <div class="customer-view__header flex">
      <div class="header-toolbar flex-1 flex items-center justify-between">
        <!-- Left -->
        <div class="toolbar-left flex items-center">
          <!-- Select -->
          <div class="select flex items-center">
            <div class="icon-bg icon-folder icon-16"></div>
            <div>Tất cả khách hàng</div>
            <div class="icon-bg icon-dropdown icon-16"></div>
          </div>
          <div class="link-icon-container flex items-center">
            <!-- Edit -->
            <div class="edit-link">Sửa</div>
            <!-- Reload -->
            <div class="refresh-link flex justify-center">
              <div class="icon-bg icon-refresh icon-16"></div>
            </div>
          </div>
        </div>
        <!-- Right -->
        <div v-if="checkedCount > 0" class="toolbar-right flex items-center">
          <!-- Button -->
          <div class="group-btn flex">
            <!-- Xóa -->
            <ms-button
              type="danger"
              variant="outlined"
              class="delete-btn"
              @click="handleSoftDeleteMany(selectedIds)"
              >Xóa</ms-button
            >
            <!-- Xuất CSV -->
            <ms-button
              type="primary"
              variant="outlined"
              class="export-btn"
              icon="icon-bg icon-export icon-16"
              positionIcon="right"
              @click="handleExportCsv(selectedIds)"
              >Xuất Excel</ms-button
            >
            <!-- Gắn loại khách hàng -->
            <ms-button type="primary" class="assign-btn" @click="openAssignTypeModal"
              >Gắn loại khách hàng</ms-button
            >
          </div>
        </div>

        <div v-else class="toolbar-right flex items-center">
          <!-- Input -->
          <div class="ai-search-container">
            <div class="icon-bg icon-ai-search icon-16"></div>
            <input
              type="text"
              class="ai-search-input"
              placeholder="Tìm kiếm thông minh"
              v-model="searchValue"
            />
            <div class="icon-ai icon-16"></div>
          </div>
          <!-- Statistic -->
          <div class="statistic-container">
            <button class="statistic-btn flex items-center">
              <div class="icon-bg icon-statistic icon-16"></div>
            </button>
          </div>
          <!-- Button -->
          <div class="group-btn flex">
            <ms-button
              type="primary"
              class="add-btn"
              icon="icon-bg icon-add icon-16"
              positionIcon="left"
              @click="handleAdd"
              >Thêm</ms-button
            >
            <ms-button
              type="primary"
              variant="outlined"
              class="import-btn"
              icon="icon-bg icon-import icon-16"
              positionIcon="left"
              @click="openImportModal"
              >Nhập từ Excel</ms-button
            >
          </div>

          <!-- Menu -->
          <div class="show-menu">
            <ms-button type="secondary" variant="outlined" class="more-btn">
              <div class="icon-bg icon-more icon-16"></div>
            </ms-button>
          </div>

          <!-- Menu -->
          <div class="show-menu">
            <ms-button type="secondary" variant="outlined" class="switch-view-btn">
              <div class="flex gap-4">
                <div class="icon-bg icon-menu icon-16"></div>
                <div class="icon-bg icon-dropdown icon-16"></div>
              </div>
            </ms-button>
          </div>
        </div>
      </div>
    </div>

    <!-- Content - Table -->
    <div class="customer-view__content flex-col flex-1 scrollable-content">
      <ms-table
        :fields="fields"
        :rows="rows"
        :sortBy="sortBy"
        :sortDirection="sortDirection"
        @update:sortBy="sortBy = $event"
        @update:sortDirection="sortDirection = $event"
        @applyFilter="handleApplyFilter"
        v-model:checkedItemCount="checkedCount"
        @selectionChange="handleSelectionChange"
        @edit="handleEdit"
        :loading="loading"
      />
    </div>

    <!--Footer - Pagination -->
    <div class="customer-view__pagination flex items-center justify-between">
      <div class="pagination-setting" title="Tùy chỉnh trường">
        <div class="icon-bg icon-setting icon-16"></div>
      </div>

      <div class="pagination-summary flex items-center flex-1">
        <div class="summary-records flex items-center">
          <div class="summary__item flex-col">
            <span>Tổng số:</span>
            <b class="summary__count">{{ rows.length }}</b>
          </div>
          <div class="summary__item flex-col">
            <span>Công nợ</span>
            <b class="summary-debt">0</b>
          </div>
        </div>
      </div>

      <div class="pagination-right flex items-center">
        <div class="pagination-pagesize">
          <ms-input
            class="pageSize-select"
            type="select"
            placeholder="100 Bản ghi trên trang"
            :options="pageSizeOptions"
            v-model="pageSize"
            icon="icon-bg icon-dropdown icon-16"
            :allowClear="false"
          />
        </div>

        <div class="pagination-pager flex items-center">
          <div
            class="pagination-btn flex-center"
            :class="[pageNumber === 1 ? '' : 'active']"
            @click="startPage"
          >
            <div class="icon-bg icon-start-page icon-16"></div>
          </div>
          <div
            class="pagination-btn flex-center"
            :class="[pageNumber === 1 ? '' : 'active']"
            @click="prevPage"
          >
            <div class="icon-bg icon-prev icon-16"></div>
          </div>

          <div class="pagination-current flex items-center">
            <b class="count-to">{{ total && pageSize ? (pageNumber - 1) * pageSize + 1 : 0 }}</b>
            <span>đến</span>
            <b class="count-from">{{ Math.min(pageNumber * pageSize, total) }}</b>
          </div>

          <div
            class="pagination-btn flex-center"
            :class="[pageNumber == totalPages ? '' : 'active']"
            @click="nextPage"
          >
            <div class="icon-bg icon-next icon-16"></div>
          </div>
          <div
            class="pagination-btn flex-center"
            :class="[pageNumber == totalPages ? '' : 'active']"
            @click="endPage"
          >
            <div class="icon-bg icon-end-page icon-16"></div>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal - import excel -->
    <ms-modal
      v-model:show="isOpenImportModal"
      title="Nhập dữ liệu từ Excel"
      cancel-text="Hủy"
      confirm-text="Lưu"
      @confirm="handleImportCsv"
    >
      <div class="modal-import modal-body flex-col gap-2">
        <!-- Nếu chưa upload file -->
        <div v-if="!uploadSuccess" class="file-box flex-center flex-col">
          <input type="file" class="file-input" @change="handleFileChange" />
          <div class="icon-bg icon-upload-file icon-40"></div>
          <div class="text-description">
            <p>Kéo và thả tài liệu vào đây hoặc Chọn file của bạn</p>
            <p>Hỗ trợ file .csv, .xls, .xlsx</p>
          </div>
        </div>

        <!-- Nếu upload thành công -->
        <div v-else class="file-success flex-center flex-col">
          <div class="icon-bg icon-success icon-38"></div>
          <div class="text-description">
            <p style="color: #31b491">Tải file thành công!</p>
            <p>{{ uploadedFile.name }}</p>
            <!-- Nút chọn lại file -->
            <ms-button
              variant="outlined"
              @click="
                () => {
                  uploadedFile = null;
                  uploadSuccess = false;
                }
              "
            >
              Chọn lại file
            </ms-button>
          </div>
        </div>
      </div>
    </ms-modal>

    <!-- Modal gắn loại khách hàng -->
    <ms-modal
      v-model:show="isAssignTypeModalOpen"
      title="Gắn loại khách hàng"
      cancel-text="Hủy"
      confirm-text="Áp dụng"
      @confirm="assignCustomerType"
    >
      <div class="modal-body flex-col gap-4">
        <label v-for="type in ['VIP', 'NBH01', 'LKHA']" :key="type" class="flex items-center gap-8">
          <input type="radio" name="customerType" :value="type" v-model="selectedCustomerType" />
          {{ type }}
        </label>
      </div>
    </ms-modal>
  </div>
</template>

<style scoped></style>
