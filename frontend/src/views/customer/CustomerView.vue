<script setup>
import MsButton from "@/components/ms-button/MsButton.vue";
</script>

<template>
  <div>
    <!-- Header -->
    <div class="header flex">
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
              variant="solid"
              class="add-btn"
              icon="icon-bg icon-add icon-16"
              positionIcon="left"
              >Thêm</ms-button
            >
            <ms-button
              type="primary"
              variant="outlined"
              class="import-btn"
              icon="icon-bg icon-import icon-16"
              positionIcon="left"
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
    <div class="content"></div>
  </div>
</template>

<style scoped>
.header {
  height: 56px;
  width: 100%;
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
</style>
