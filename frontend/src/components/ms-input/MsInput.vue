<script setup>
//#region Props
defineProps({
  placeholder: String,
  type: {
    type: String,
    default: "text",
  },
  options: {
    type: Array,
    default: () => [],
  },
});
//#endregion
</script>

<template>
  <div class="ms-input flex-1">
    <!-- kiểu text -->
    <input v-if="type === 'text'" type="text" :placeholder="placeholder" />

    <!-- kiểu date - dùng Antd DatePicker -->
    <a-date-picker
      v-else-if="type === 'date'"
      :placeholder="placeholder"
      class="ms-input-date"
      allowClear
      format="DD/MM/YYYY"
    >
      <template #suffixIcon>
        <div class="icon-bg icon-calendar icon-16"></div>
      </template>
    </a-date-picker>

    <!-- select -->
    <a-select
      v-else-if="type === 'select'"
      class="ms-input-select"
      :placeholder="placeholder"
      :options="options"
      allowClear
    >
      <template #suffixIcon>
        <div class="icon-bg icon-select-type icon-16"></div>
      </template>
    </a-select>
  </div>
</template>

<style scoped>
.ms-input {
  height: 32px;
  border-radius: 4px;
}

/* Text input */
.ms-input input {
  width: 100%;
  height: 100%;
  border: 1px solid #d3d7de;
  outline: none;
  font-size: 13px;
  border-radius: 4px;
  padding: 6px 12px;
  color: #1f2229;
  max-height: 32px;
  background: #fff;
}

.ms-input input::placeholder {
  color: #bfbfc9;
}

/* Focus input */
.ms-input input:focus {
  border-color: #4096ff;
}

.ms-input input:hover {
  border-color: #4096ff;
}

/* Date picker */
.ms-input-date {
  width: 100%;
  height: 32px;
  border-radius: 4px;
  border: 1px solid #d3d7de;
}

.ms-input-date:focus-within {
  border-color: #4096ff;
  box-shadow: none;
}

.ms-input-date:hover {
  border-color: #4096ff;
}

.ms-input-date :deep(.ant-picker) {
  width: 100%;
  height: 32px !important;
  border: 1px solid #d3d7de !important;
  border-radius: 4px !important;
  padding: 0 12px !important;
}

.ms-input-date :deep(.ant-picker-input > input) {
  font-size: 13px !important;
}

/* Select */

.ms-input-select {
  width: 100%;
  height: 100%;
  border-radius: 4px;
}

.ms-input-select :deep(.ant-select-selector) {
  width: 100%;
  height: 100% !important;
  border-radius: 4px !important;
  padding: 0 12px !important;
}

.ms-input-select :deep(.ant-select-selection-item) {
  font-size: 13px;
}
</style>
