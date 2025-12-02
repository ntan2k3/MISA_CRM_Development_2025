<script setup>
import MsButton from "@/components/ms-button/MsButton.vue";
import MsTable from "@/components/ms-table/MsTable.vue";
import { useRouter } from "vue-router";
import { ref } from "vue";

const fields = [
  { key: "customerType", label: "Loại khách hàng", type: "text" },
  { key: "customerCode", label: "Mã khách hàng", type: "text" },
  { key: "customerName", label: "Tên khách hàng", type: "text" },
  { key: "taxCode", label: "Mã số thuế", type: "text" },
  { key: "address", label: "Địa chỉ (Giao hàng)", type: "text" },
  { key: "phone", label: "Điện thoại", type: "text" },
  { key: "lastPurchaseDate", label: "Ngày mua hàng gần nhất", type: "date" },
  { key: "itemCode", label: "Hàng hóa đã mua", type: "text" },
  { key: "itemName", label: "Tên hàng hóa đã mua", type: "text" },
];

const rows = [
  {
    customerCode: "KH0000390",
    customerType: "Cá nhân",
    customerName: "Nguyễn Văn A",
    taxCode: "1234567890",
    address: "12 Lý Thường Kiệt, Hà Nội",
    phone: "0912345678",
    lastPurchaseDate: "2024-11-02",
    itemCode: "BBTLTL08",
    itemName: "Bút bi Thiên Long TL-08",
  },
  {
    customerCode: "KH0000389",
    customerType: "Doanh nghiệp",
    customerName: "Công ty TNHH Minh Phát",
    taxCode: "0315567891",
    address: "25 Nguyễn Thị Minh Khai, Q1, TP.HCM",
    phone: "02839876543",
    lastPurchaseDate: "2024-10-20",
    itemCode: "GA4DA70GSM",
    itemName: "Giấy A4 Double A 70gsm",
  },
  {
    customerCode: "KH0000388",
    customerType: "Cá nhân",
    customerName: "Trần Thị B",
    taxCode: "-",
    address: "55 Phan Chu Trinh, Đà Nẵng",
    phone: "0938765432",
    lastPurchaseDate: "2024-09-18",
    itemCode: "BNA4",
    itemName: "Bìa nhựa A4",
  },
  {
    customerCode: "KH0000387",
    customerType: "Doanh nghiệp",
    customerName: "Công ty ABC",
    taxCode: "0109876543",
    address: "89 Trần Hưng Đạo, Hà Nội",
    phone: "02466778899",
    lastPurchaseDate: "2024-11-25",
    itemCode: "MICLBP6030",
    itemName: "Máy in Canon LBP6030",
  },
  {
    customerCode: "KH0000387",
    customerType: "Doanh nghiệp",
    customerName: "Công ty ABC",
    taxCode: "0109876543",
    address: "89 Trần Hưng Đạo, Hà Nội",
    phone: "02466778899",
    lastPurchaseDate: "2024-11-25",
    itemCode: "MICLBP6030",
    itemName: "Máy in Canon LBP6030",
  },
  {
    customerCode: "KH0000387",
    customerType: "Doanh nghiệp",
    customerName: "Công ty ABC",
    taxCode: "0109876543",
    address: "89 Trần Hưng Đạo, Hà Nội",
    phone: "02466778899",
    lastPurchaseDate: "2024-11-25",
    itemCode: "MICLBP6030",
    itemName: "Máy in Canon LBP6030",
  },
  {
    customerCode: "KH0000387",
    customerType: "Doanh nghiệp",
    customerName: "Công ty ABC",
    taxCode: "0109876543",
    address: "89 Trần Hưng Đạo, Hà Nội",
    phone: "02466778899",
    lastPurchaseDate: "2024-11-25",
    itemCode: "MICLBP6030",
    itemName: "Máy in Canon LBP6030",
  },
  {
    customerCode: "KH0000387",
    customerType: "Doanh nghiệp",
    customerName: "Công ty ABC",
    taxCode: "0109876543",
    address: "89 Trần Hưng Đạo, Hà Nội",
    phone: "02466778899",
    lastPurchaseDate: "2024-11-25",
    itemCode: "MICLBP6030",
    itemName: "Máy in Canon LBP6030",
  },
  {
    customerCode: "KH0000387",
    customerType: "Doanh nghiệp",
    customerName: "Công ty ABC",
    taxCode: "0109876543",
    address: "89 Trần Hưng Đạo, Hà Nội",
    phone: "02466778899",
    lastPurchaseDate: "2024-11-25",
    itemCode: "MICLBP6030",
    itemName: "Máy in Canon LBP6030",
  },
  {
    customerCode: "KH0000387",
    customerType: "Doanh nghiệp",
    customerName: "Công ty ABC",
    taxCode: "0109876543",
    address: "89 Trần Hưng Đạo, Hà Nội",
    phone: "02466778899",
    lastPurchaseDate: "2024-11-25",
    itemCode: "MICLBP6030",
    itemName: "Máy in Canon LBP6030",
  },
  {
    customerCode: "KH0000387",
    customerType: "Doanh nghiệp",
    customerName: "Công ty ABC",
    taxCode: "0109876543",
    address: "89 Trần Hưng Đạo, Hà Nội",
    phone: "02466778899",
    lastPurchaseDate: "2024-11-25",
    itemCode: "MICLBP6030",
    itemName: "Máy in Canon LBP6030",
  },
  {
    customerCode: "KH0000387",
    customerType: "Doanh nghiệp",
    customerName: "Công ty ABC",
    taxCode: "0109876543",
    address: "89 Trần Hưng Đạo, Hà Nội",
    phone: "02466778899",
    lastPurchaseDate: "2024-11-25",
    itemCode: "MICLBP6030",
    itemName: "Máy in Canon LBP6030",
  },
  {
    customerCode: "KH0000387",
    customerType: "Doanh nghiệp",
    customerName: "Công ty ABC",
    taxCode: "0109876543",
    address: "89 Trần Hưng Đạo, Hà Nội",
    phone: "02466778899",
    lastPurchaseDate: "2024-11-25",
    itemCode: "MICLBP6030",
    itemName: "Máy in Canon LBP6030",
  },
  {
    customerCode: "KH0000387",
    customerType: "Doanh nghiệp",
    customerName: "Công ty ABC",
    taxCode: "0109876543",
    address: "89 Trần Hưng Đạo, Hà Nội",
    phone: "02466778899",
    lastPurchaseDate: "2024-11-25",
    itemCode: "MICLBP6030",
    itemName: "Máy in Canon LBP6030",
  },
  {
    customerCode: "KH0000387",
    customerType: "Doanh nghiệp",
    customerName: "Công ty ABC",
    taxCode: "0109876543",
    address: "89 Trần Hưng Đạo, Hà Nội",
    phone: "02466778899",
    lastPurchaseDate: "2024-11-25",
    itemCode: "MICLBP6030",
    itemName: "Máy in Canon LBP6030",
  },
  {
    customerCode: "KH0000387",
    customerType: "Doanh nghiệp",
    customerName: "Công ty ABC",
    taxCode: "0109876543",
    address: "89 Trần Hưng Đạo, Hà Nội",
    phone: "02466778899",
    lastPurchaseDate: "2024-11-25",
    itemCode: "MICLBP6030",
    itemName: "Máy in Canon LBP6030",
  },
];

const isOpenModal = ref(false);

const router = useRouter();

// Hàm mở modal
const openModal = () => {
  isOpenModal.value = true;
};

// Hàm đóng modal
const closeModal = () => {
  isOpenModal.value = false;
};

// Hàm chuyến sang view thêm khách hàng
const handleAdd = () => {
  router.push("/customers/add");
};

// Hàm chuyến sang view sửa khách hàng
const handleEdit = () => {
  router.push("/customers/edit");
};
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
        <div class="toolbar-right flex items-center">
          <!-- Input -->
          <div class="ai-search-container">
            <div class="icon-bg icon-ai-search icon-16"></div>
            <input type="text" class="ai-search-input" placeholder="Tìm kiếm thông minh" />
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
              @click="openModal"
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
      <ms-table :fields="fields" :rows="rows" @edit="handleEdit" />
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
            <b class="summary-debt">32.600.000</b>
          </div>
        </div>
      </div>

      <div class="pagination-right flex items-center">
        <div class="pagination-pagesize">
          <div class="pagination-content flex items-center justify-between">
            <div class="text">100 Bản ghi trên trang</div>
            <div class="icon icon-bg icon-dropdown icon-16"></div>
          </div>
        </div>

        <div class="pagination-pager flex items-center">
          <div class="pagination-btn flex-center">
            <div class="icon-bg icon-start-page icon-16"></div>
          </div>
          <div class="pagination-btn flex-center">
            <div class="icon-bg icon-prev icon-16"></div>
          </div>

          <div class="pagination-current flex items-center">
            <b class="count-to">1</b>
            <span>đến</span>
            <b class="count-from">22</b>
          </div>

          <div class="pagination-btn flex-center">
            <div class="icon-bg icon-next icon-16"></div>
          </div>
          <div class="pagination-btn flex-center">
            <div class="icon-bg icon-end-page icon-16"></div>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal - Export excel -->
    <div class="customer-view__modal flex justify-center" :class="[isOpenModal ? '' : 'hide']">
      <div class="box-modal flex-col justify-end">
        <div class="close-btn" @click="closeModal">
          <div class="icon-bg icon-close icon-10"></div>
        </div>
        <div class="file-box flex-center flex-col">
          <input type="file" class="file-input" />
          <div class="icon-bg icon-upload-file icon-38"></div>
          <div class="text-description">
            <p>Kéo và thả tài liệu vào đây hoặc Chọn file của bạn</p>
            <p>Chúng tôi hỗ trợ các file .csv, .xls và .xlsx</p>
          </div>
        </div>
        <div class="group-btn flex">
          <ms-button class="cancel-btn" @click="closeModal">Hủy</ms-button>
          <ms-button type="primary" class="save-btn">Lưu</ms-button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.customer-view {
  height: 100%;
  min-height: 0;
}

.customer-view__header {
  height: 56px;
  width: 100%;
  flex-shrink: 0;
}

.header-toolbar {
  padding: 0 16px;
}

.select {
  background: #fff;
  border: 1px solid #d3d7de;
  padding-right: 8px;
  padding-left: 12px;
  height: 32px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 13px;
  font-weight: 600;
  gap: 8px;
}

.link-icon-container {
  margin-top: 2px;
  font-size: 13px;
}

.edit-link {
  color: #4262f0;
  margin-left: 16px;
  cursor: pointer;
}

.edit-link:hover {
  text-decoration: underline;
}

.refresh-link {
  width: 24px;
  height: 24px;
  padding-top: 4px;
  margin-left: 16px;
  border-radius: 50%;
  cursor: pointer;
}

.refresh-link:hover {
  background-color: #c5c9d3;
}

.toolbar-right {
  gap: 8px;
}

.ai-search-container {
  position: relative;
  width: 284px;
  height: 32px;
  padding: 1px;
  border-radius: 4px;
}

.ai-search-input {
  position: relative;
  height: 100%;
  width: 100%;
  font-size: 13px;
  outline: none;
  border: none;
  border-radius: 3px;
  padding: 0 16px 0 32px;
  background: linear-gradient(90deg, rgba(66, 98, 240, 0.1) 0%, rgba(159, 115, 241, 0.1) 100%), #fff;
  text-overflow: ellipsis;
}

.ai-search-input::placeholder {
  background: linear-gradient(109deg, #4262f0 24.05%, #9f73f1 71.93%);
  -webkit-background-clip: text;
  color: transparent;
  font-weight: 400;
}

.icon-ai-search {
  position: absolute;
  top: 8px;
  left: 8px;
  z-index: 2;
}

.icon-ai {
  position: absolute;
  top: 8px;
  right: 8px;
  z-index: 2;
}

.statistic-container {
  position: relative;
  height: 32px;
  padding: 1px;
  border-radius: 4px;
}

.statistic-btn {
  position: relative;
  height: 100%;
  padding: 0 7px;
  font-weight: 500;
  border: none;
  border-radius: 3px;
  background: linear-gradient(90deg, #e7ebfd 0%, #ece7fd 32.21%, #e5f7ff 66.11%, #fdefe7 100%);
  cursor: pointer;
}

.ai-search-container::before,
.statistic-container::before {
  content: "";
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: linear-gradient(251deg, #4262f0 24.05%, #9f73f1 71.93%);
  border-radius: 4px;
}

.group-btn {
  gap: 8px;
}

.add-btn,
.import-btn {
  padding-right: 16px !important;
}

.more-btn {
  width: 36px !important;
}

.switch-view-btn {
  width: 55px !important;
  padding: 7px !important;
}

.scrollable-content {
  min-height: 0;
  overflow: auto;
}

.customer-view__pagination {
  position: sticky;
  bottom: 0;
  height: 56px;
  background: white;
  border-top: 1px solid #d3d7de;
  padding: 8px 16px;
  font-size: 13px;
  z-index: 20;
}

.pagination-setting {
  height: 24px;
  width: 24px;
  padding: 4px;
  border-radius: 50%;
  cursor: pointer;
}

.pagination-setting:hover {
  background-color: #f0f2f4;
}

.pagination-summary {
  gap: 8px;
  color: #586074;
}

.summary-records {
  height: 39px;
  padding: 0 16px;
  gap: 32px;
}

.summary__item b {
  font-weight: 700;
  color: #1f2229;
}

.pagination-pagesize {
  min-width: 182px;
  cursor: pointer;
}

.pagination-content {
  border: 1px solid #d3d7de;
  border-radius: 4px;
  padding: 0 8px 0 16px;
  min-height: 32px;
}

.pagination-pager {
  margin-left: 16px;
}

.pagination-btn {
  width: 24px;
  height: 24px;
  padding: 4px;
  opacity: 0.45;
}

.pagination-current {
  padding: 4px;
  gap: 8px;
  color: #586074;
}

.pagination-current b {
  min-width: 16px;
  text-align: center;
}

.customer-view__modal {
  position: absolute;
  top: 0;
  left: 0;
  z-index: 100;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
}

.hide {
  display: none;
}

.box-modal {
  position: relative;
  width: 600px;
  height: 300px;
  background-color: white;
  padding: 20px;
  border-radius: 10px;
  gap: 10px;
  margin-top: 200px;
}

.close-btn {
  position: absolute;
  top: 20px;
  right: 20px;
  cursor: pointer;
}

.file-box {
  position: relative;
  height: 180px;
  width: 100%;
  background-color: #f4f8ff;
  border: 1px dashed #005eff;
  border-radius: 8px;
}

.file-input {
  position: absolute;
  inset: 0;
  opacity: 0;
  cursor: pointer;
}

.text-description {
  text-align: center;
  font-size: 13px;
}

.text-description p {
  margin: 4px 0;
}

.group-btn {
  align-self: end;
  justify-content: end;
  gap: 8px;
}

.cancel-btn,
.save-btn {
  padding: 8px 16px !important;
}
</style>
