<script setup>
//#region Import
import MsButton from "@/components/ms-button/MsButton.vue";
import MsTable from "@/components/ms-table/MsTable.vue";
import MsInput from "@/components/ms-input/MsInput.vue";
import MsModal from "@/components/ms-modal/MsModal.vue";
import CustomersAPI from "@/apis/components/customers/CustomersAPI";
import { Modal } from "ant-design-vue";

import { useRouter, useRoute } from "vue-router";
import { computed, onMounted, ref, watch } from "vue";
import { debounce } from "lodash";
import { message } from "ant-design-vue";
import { usePagination } from "@/utils/composables/usePagination.js";

import "@/assets/css/customerView.css";
//#endregion

//#region Router
// Khởi tạo router để điều hướng
const router = useRouter(); // Để chuyển trang
const route = useRoute(); // Để đọc url hiện tại
//#endregion

//#region Computed
/**
 * Cache label loại khách hàng đang lọc hiện tại
 * Tránh phải find trong array mỗi lần render
 */
const currentCustomerTypeLabel = computed(() => {
  if (!customerTypeFilter.value) return "Tất cả khách hàng";
  return (
    customerTypeOptions.find((o) => o.key === customerTypeFilter.value)?.value ||
    "Tất cả khách hàng"
  );
});

/**
 * Tính toán thông tin pagination
 * Tránh tính toán lại mỗi lần render
 */
const paginationInfo = computed(() => ({
  from: total.value && pageSize.value ? (pageNumber.value - 1) * pageSize.value + 1 : 0,
  to: Math.min(pageNumber.value * pageSize.value, total.value),
}));
//#endregion

//#region States
// Trạng thái loading khi onMounted.
const isFirstLoad = ref(true);

// Trạng thái loading khi lấy dữ liệu
const isLoadingData = ref(false);

// Trạng thái loading khi import file
const isImportLoading = ref(false);

// Modal import file Excel
const isOpenImportModal = ref(false);

// File upload và trạng thái
const uploadedFile = ref(null);
const uploadSuccess = ref(false);

// Danh sách ID các row đã chọn
const selectedIds = ref([]);

// Tổng số item được chọn
const checkedCount = ref(0);

// Danh sách dữ liệu bảng
const rows = ref([]);

// Giá trị tìm kiếm
const searchValue = ref("");

// Sắp xếp
const sortBy = ref("createdDate");
const sortDirection = ref("desc");

// Lưu filter hiện tại
const customerTypeFilter = ref("");

// Ref để truy cập MsTable
const tableRef = ref(null);

//#endregion

//#region Pagination (usePagination)
/**
 * Sử dụng custom composable usePagination để quản lý logic phân trang
 *
 * Tham số truyền vào: 100 (pageSize mặc định là 100 bản ghi/trang)
 *
 * Composable này return ra:
 * - pageSize: Số bản ghi trên 1 trang (reactive, có thể thay đổi)
 * - pageNumber: Trang hiện tại (bắt đầu từ 1)
 * - total: Tổng số bản ghi (lấy từ API)
 * - totalPages: Tổng số trang (computed từ total và pageSize)
 * - nextPage(): Hàm chuyển sang trang kế tiếp
 * - prevPage(): Hàm quay lại trang trước
 * - startPage(): Hàm về trang đầu tiên
 * - endPage(): Hàm đến trang cuối cùng
 */
const { pageSize, pageNumber, total, totalPages, nextPage, prevPage, startPage, endPage } =
  usePagination(100);
//#endregion

//#region Table, customerTypeOptions
// Cấu hình các cột cho bảng
const fields = [
  {
    key: "customerType",
    label: "Loại khách hàng",
    type: "text",
    width: "175px",
  },
  { key: "customerCode", label: "Mã khách hàng", type: "text", width: "150px" },
  { key: "customerName", label: "Tên khách hàng", type: "text", width: "300px" },
  { key: "customerTaxCode", label: "Mã số thuế", type: "text", width: "160px" },
  {
    key: "customerPhoneNumber",
    label: "Điện thoại",
    type: "text",
    width: "160px",
    icon: "icon-bg icon-phone icon-16",
  },
  { key: "customerEmail", label: "Email", type: "text", width: "235px" },
  { key: "customerAddr", label: "Địa chỉ (Giao hàng)", type: "text", width: "300px" },
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

// Các tùy chọn loại khách hàng
const customerTypeOptions = [
  { key: "", value: "Tất cả khách hàng" },
  { key: "VIP", value: "VIP" },
  { key: "LKHA", value: "LKHA" },
  { key: "NBH01", value: "NBH01" },
];

// Options cho dropdown gắn loại (không bao gồm "Tất cả")
const assignTypeOptions = [
  { label: "VIP", value: "VIP" },
  { label: "LKHA", value: "LKHA" },
  { label: "NBH01", value: "NBH01" },
];
//#endregion

//#region Methods
/**
 * Hàm xử lý khi người dùng tick/untick checkbox trong bảng
 * @param {Array} items - Mảng các row object đang được chọn
 * Được gọi bởi: MsTable component khi selection thay đổi
 * Ví dụ:
 * items = [
 *   { customerId: 1, customerName: "A" },
 *   { customerId: 5, customerName: "B" }
 * ]
 * → selectedIds = [1, 5]
 * → checkedCount = 2
 */
const handleSelectionChange = (items) => {
  // Chỉ lấy ra id rồi lưu vào mảng
  selectedIds.value = items.map((i) => i.customerId);
  // Số lượng id đã chọn
  checkedCount.value = selectedIds.value.length;
};

/**
 * Mở modal import file Excel
 */
const openImportModal = () => {
  isOpenImportModal.value = true;
  uploadedFile.value = null;
  uploadSuccess.value = false;
};

/**
 *  Đóng modal import
 */
const closeImportModal = () => {
  isOpenImportModal.value = false;
};

/**
 * Hàm xử lý khi người dùng chọn file từ input file
 * @param {Event} e - Event object từ input file
 * Được gọi khi: Change event của <input type="file">
 */
const handleFileChange = (e) => {
  const file = e.target.files[0];
  if (file) {
    uploadedFile.value = file;
    uploadSuccess.value = true;

    e.target.value = null;
  }
};

/**
 * Chuyển sang view thêm khách hàng
 */
const handleAdd = () => {
  router.push("/customers/add");
};

/**
 * Chuyển sang view sửa khách hàng
 */
const handleEdit = (id) => {
  if (!id) return;
  router.push({ name: "customer-edit", params: { id } });
};

/**
 * Hàm lấy tổng số bản ghi (có áp dụng filter và search)
 */
const getTotalData = async () => {
  try {
    const res = await CustomersAPI.getTotalData({
      customerType: customerTypeFilter.value || undefined,
      search: searchValue.value || undefined,
    });
    total.value = res.data.data;
  } catch (error) {
    console.error(error.response?.data?.error?.message || error);
  }
};

/**
 * Lấy dữ liệu bảng từ server
 * Được gọi khi:
 * - Component mount lần đầu
 * - Thay đổi trang (pageNumber)
 * - Thay đổi số bản ghi/trang (pageSize)
 * - Thay đổi cột sắp xếp (sortBy, sortDirection)
 * - Thay đổi filter (customerTypeFilter)
 * - Gõ tìm kiếm (sau debounce 300ms)
 */
const loadData = async () => {
  isLoadingData.value = true;
  try {
    const res = await CustomersAPI.getAll({
      pageSize: pageSize.value,
      pageNumber: pageNumber.value,
      search: searchValue.value,
      sortBy: sortBy.value,
      sortDirection: sortDirection.value,
      customerType: customerTypeFilter.value || undefined,
    });

    const data = res.data.data;

    rows.value = data;
  } catch (error) {
    message.error(error.response?.data?.error?.message || "Lỗi khi lấy dữ liệu", 2);
  } finally {
    isLoadingData.value = false;
  }
};

/**
 * Xuất file CSV/Excel các bản ghi được chọn
 */
const handleExportCsv = async (ids) => {
  try {
    const res = await CustomersAPI.exportCsv(ids, {
      responseType: "blob",
    });

    let fileName = "customers.csv";

    // Tạo URL tạm cho file blob
    const url = window.URL.createObjectURL(new Blob([res.data]));

    // Tạo thẻ a để thực hiện việc download file
    const link = document.createElement("a");

    link.href = url;
    link.setAttribute("download", fileName);
    document.body.appendChild(link);

    // Trigger click để download
    link.click();

    // Loại bỏ thẻ a
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);

    // clear checkbox
    tableRef.value?.clearSelection();

    message.success("Xuất file thành công.", 2);
  } catch (error) {
    message.error(error.response?.data?.error?.message || "Lỗi khi xuất file.", 2);
  }
};

/**
 * Import file CSV/Excel
 */
const handleImportCsv = async () => {
  if (!uploadedFile.value) {
    message.warning("Vui lòng tải lên file CSV trước khi gửi.", 2);
    return;
  }

  isImportLoading.value = true;
  isLoadingData.value = true;
  const formData = new FormData();
  formData.append("file", uploadedFile.value);

  try {
    const res = await CustomersAPI.importCsv(formData);

    // Lấy ra số bản ghi import thành công
    const data = res.data.data;
    if (!data) {
      message.error("Không bản nào import thành công do trùng dữ liệu.", 2);
      return;
    }
    message.success(`Import thành công ${data} bản ghi.`, 2);

    // Đóng import modal
    closeImportModal();

    // reset lại page và file
    pageNumber.value = 1;
    uploadedFile.value = null;
    uploadSuccess.value = false;

    //load lại data
    await Promise.all([loadData(), getTotalData()]);
  } catch (error) {
    message.error(error.response?.data?.error?.message || "Lỗi khi nhập file.", 2);
  } finally {
    isImportLoading.value = false;
    isLoadingData.value = false;
  }
};

/**
 * Hàm xử lý khi chọn loại khách hàng trong dropdown filter
 */
const handleSelectCustomerType = (type) => {
  customerTypeFilter.value = type; // "" là tất cả
};

/**
 * Xóa mềm nhiều khách hàng cùng lúc
 */
const handleSoftDeleteMany = (ids) => {
  if (selectedIds.value.length === 0) {
    message.warning("Vui lòng chọn ít nhất một khách hàng.", 2);
    return;
  }

  Modal.confirm({
    title: "Xác nhận xóa",
    content: `Bạn có chắc chắn muốn xóa ${selectedIds.value.length} khách hàng đã chọn không?`,
    okText: "Xóa",
    okType: "danger",
    cancelText: "Hủy",
    async onOk() {
      isLoadingData.value = true;
      try {
        const res = await CustomersAPI.softDeleteMany(ids);

        const data = res.data.data;

        // Nếu data = 0 || null thì báo lỗi
        if (!data) {
          message.error(data?.error?.message);
          return;
        }

        // Xóa xong thì gọi load lại data và số trang
        await Promise.all([loadData(), getTotalData()]);

        // reset lại các ô checjked về rỗng và số lượng ô được checked về 0
        selectedIds.value = [];
        checkedCount.value = 0;

        // Trả ra thông báo xóa thành công
        message.success(`Xóa thành công ${data} khách hàng`, 2);
      } catch (error) {
        message.error(
          error.response?.data?.error?.message || "Thao tác xóa hàng loạt thất bại.",
          2
        );
      } finally {
        isLoadingData.value = false;
      }
    },
  });
};

/**
 * Gắn loại khách hàng hàng loạt
 */
const handleAssignCustomerType = async (type) => {
  if (!type) return;

  if (selectedIds.value.length === 0) {
    message.warning("Vui lòng chọn ít nhất một khách hàng.", 2);
    return;
  }

  // Gọi API và xử lý
  isLoadingData.value = true;
  try {
    await CustomersAPI.assignCustomerType(selectedIds.value, type);
    message.success(`Đã gắn loại khách hàng thành công`, 2);

    // clear checkbox
    tableRef.value?.clearSelection();
    await loadData();
  } catch (error) {
    message.error(error.response?.data?.error?.message || "Gắn loại khách hàng thất bại.", 2);
  } finally {
    isLoadingData.value = false;
  }
};

/**
 * Debounce load dữ liệu khi tìm kiếm
 */
const debouncedLoadData = debounce(() => {
  pageNumber.value = 1;
  loadData();
  getTotalData();
}, 300);
//#endregion

//#region Lifecycle Hooks
onMounted(async () => {
  // Set tất cả state từ query
  const { search, page, size, sortBy: sBy, sortDirection: sDir, customerType } = route.query;

  if (search) searchValue.value = search;
  if (page) pageNumber.value = Number(page);
  if (size) pageSize.value = Number(size);
  if (sBy) sortBy.value = sBy;
  if (sDir) sortDirection.value = sDir;
  if (customerType) customerTypeFilter.value = customerType;

  // Load data 1 lần duy nhất
  await Promise.all([loadData(), getTotalData()]);

  // Đánh dấu đã load xong
  isFirstLoad.value = false;
});

//#endregion

//#region Watchers

// Watch pageSize để reset pageNumber & loadData
watch([pageSize, sortBy, sortDirection], () => {
  if (isFirstLoad.value) return;
  pageNumber.value = 1;
  loadData();
});

// Watch pageNumber, sortBy, sortDirection để loadData
watch(pageNumber, () => {
  if (isFirstLoad.value) return;
  loadData();
});

// Watch searchValue để debounce
watch(searchValue, () => {
  debouncedLoadData();
});

// Watch filter để loadData và getTotalData
watch(customerTypeFilter, () => {
  if (isFirstLoad.value) return;
  pageNumber.value = 1;
  loadData();
  getTotalData();
});

// Đồng bộ URL với các state: search, filter, pagination, sort
watch([searchValue, pageNumber, pageSize, sortBy, sortDirection, customerTypeFilter], () => {
  router.replace({
    query: {
      search: searchValue.value || undefined,
      page: pageNumber.value !== 1 ? pageNumber.value : undefined,
      size: pageSize.value !== 100 ? pageSize.value : undefined,
      sortBy: sortBy.value || undefined,
      sortDirection: sortDirection.value !== "asc" ? sortDirection.value : undefined,
      customerType: customerTypeFilter.value || undefined,
    },
  });
});

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
          <a-dropdown placement="bottomLeft" trigger="click">
            <div class="select flex items-center cursor-pointer">
              <div class="icon-bg icon-folder icon-16"></div>
              <div>
                {{ currentCustomerTypeLabel }}
              </div>
              <div class="icon-bg icon-dropdown icon-16"></div>
            </div>

            <template #overlay>
              <a-menu @click="({ key }) => handleSelectCustomerType(key)">
                <a-menu-item
                  v-for="opt in customerTypeOptions"
                  :key="opt.key"
                  class="flex items-center justify-between"
                >
                  <div class="flex items-center justify-between">
                    <span>{{ opt.value }}</span>

                    <!-- ICON TICK -->
                    <span v-if="customerTypeFilter === opt.key" style="color: #4caf50"> ✔ </span>
                  </div>
                </a-menu-item>
              </a-menu>
            </template>
          </a-dropdown>

          <div class="link-icon-container flex items-center">
            <!-- Edit -->
            <div class="edit-link">Sửa</div>
            <!-- Reload -->
            <div class="refresh-link flex justify-center">
              <div class="icon-bg icon-refresh icon-16"></div>
            </div>
          </div>

          <!-- Button -->
          <div v-if="checkedCount > 0" class="group-btn flex ml-8">
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
            <a-dropdown placement="bottomLeft" trigger="click">
              <ms-button type="primary" variant="outlined" class="assign-btn">
                Gắn loại khách hàng
                <div class="icon-bg icon-dropdown icon-16 ml-4"></div>
              </ms-button>

              <template #overlay>
                <a-menu @click="({ key }) => handleAssignCustomerType(key)">
                  <a-menu-item v-for="opt in assignTypeOptions" :key="opt.value">
                    {{ opt.label }}
                  </a-menu-item>
                </a-menu>
              </template>
            </a-dropdown>
          </div>
        </div>
        <!-- Right -->
        <div v-if="checkedCount == 0" class="toolbar-right flex items-center">
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
        ref="tableRef"
        :fields="fields"
        :rows="rows"
        :sortBy="sortBy"
        :sortDirection="sortDirection"
        @update:sortBy="sortBy = $event"
        @update:sortDirection="sortDirection = $event"
        v-model:checkedItemCount="checkedCount"
        @selectionChange="handleSelectionChange"
        @edit="handleEdit"
        :loading="isLoadingData"
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
            <b class="count-to">{{ paginationInfo.from }}</b>
            <span>đến</span>
            <b class="count-from">{{ paginationInfo.to }}</b>
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
      :loading="isImportLoading"
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
  </div>
</template>

<style scoped></style>
