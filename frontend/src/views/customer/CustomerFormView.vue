<script setup>
import MsButton from "@/components/ms-button/MsButton.vue";
import MsForm from "@/components/ms-form/MsForm.vue";
import { useRouter, useRoute } from "vue-router";
import { computed } from "vue";

const router = useRouter();
const route = useRoute();

const handleCancel = () => {
  router.push("/customers");
};

const isEdit = computed(
  () => route.name === "customer-edit" || route.path.includes("/customers/edit")
);

const title = computed(() => (isEdit.value ? "Sửa Khách hàng" : "Thêm Khách hàng"));

const leftCol = [
  { label: "Mã khách hàng", placeholder: "Mã tự sinh" },
  {
    label: "Loại khách hàng",
    type: "select",
    options: [
      { label: "NBH01", value: "NBH01" },
      { label: "LKHA", value: "LKHA" },
      { label: "VIP", value: "VIP" },
    ],
  },
  { label: "Ngày mua hàng gần nhất", type: "date", placeholder: "DD/MM/YYYY" },
  { label: "Hàng hóa đã mua" },
  { label: "Tên hàng hóa đã mua" },
];

const rightCol = [
  { label: "Tên khách hàng", required: true },
  { label: "Mã số thuế" },
  { label: "Điện thoại" },
  { label: "Email" },
  { label: "Địa chỉ (Giao hàng)" },
];
</script>

<template>
  <div class="flex-col flex-1">
    <!-- Header -->
    <div class="header flex">
      <div class="header-toolbar flex-1 flex items-center justify-between">
        <!-- Left -->
        <div class="toolbar-left flex items-center">
          <div class="title">{{ title }}</div>
          <div v-if="!isEdit" class="select flex items-center">
            Mẫu tiêu chuẩn
            <div class="icon-bg icon-dropdown icon-16"></div>
          </div>
          <div class="layout-setting">Sửa bố cục</div>
        </div>

        <!-- Right -->
        <div class="toolbar-right flex items-center">
          <div class="group-btn flex">
            <ms-button
              type="default"
              variant="solid"
              class="cancel-btn"
              positionIcon="left"
              @click="handleCancel"
              >Hủy bỏ</ms-button
            >
            <ms-button type="primary" variant="outlined" class="save-add-btn" positionIcon="left"
              >Lưu và thêm</ms-button
            >
            <ms-button type="primary" variant="solid" class="save-btn" positionIcon="left"
              >Lưu</ms-button
            >
          </div>
        </div>
      </div>
    </div>

    <!-- Content -->
    <div class="content flex-col flex-1">
      <ms-form :leftCol="leftCol" :rightCol="rightCol" />
    </div>
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

.title {
  font-size: 20px;
  margin-right: 8px;
  font-weight: 500;
}

.select {
  margin-right: 8px;
  font-weight: 500;
  color: #1f2229;
  cursor: pointer;
  padding: 5px 0 0 8px;
  gap: 8px;
}

.layout-setting {
  font-size: 13px;
  padding-top: 6px;
  color: #4262f0;
  cursor: pointer;
}

.layout-setting:hover {
  text-decoration: underline;
  text-decoration-color: #4262f0;
}

.group-btn {
  gap: 8px;
}

.cancel-btn {
  border: 1px solid #d3d7de !important;
}

.cancel-btn,
.save-add-btn,
.save-btn {
  padding: 5px 16px !important;
  transition: 0.3s all ease-in-out;
}

.content {
  padding-top: 32px;
  background-color: white;
}
</style>
