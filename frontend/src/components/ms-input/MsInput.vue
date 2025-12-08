<script setup>
import dayjs from "dayjs";

//#region Props
/**
 * Props cấu hình của input component
 */
defineProps({
  /**
   * Placeholder hiển thị
   * @type {string}
   */
  placeholder: String,

  /**
   * Loại input: text | date | select
   * @type {string}
   */
  type: {
    type: String,
    default: "text",
  },

  /**
   * Các lựa chọn cho select
   * @type {Array}
   */
  options: {
    type: Array,
    default: () => [],
  },

  /**
   * Trạng thái disabled
   * @type {boolean}
   */
  disabled: {
    type: Boolean,
    default: false,
  },

  /**
   * Icon custom hiển thị bên phải input/select/date
   * @type {string|null}
   */
  icon: {
    type: String,
    default: null,
  },

  /**
   * Cho phép clear giá trị (với date và select)
   * @type {boolean}
   */
  allowClear: {
    type: Boolean,
    default: true,
  },

  /**
   * Giá trị của input (v-model)
   * @type {string|number|Date}
   */
  modelValue: [String, Number, Date],
});
//#endregion Props

//#region Emits
/**
 * Emits:
 * - update:modelValue: cập nhật giá trị v-model
 */
const emit = defineEmits(["update:modelValue"]);
//#endregion Emits

//#region Methods
/**
 * Xử lý input của kiểu text
 * @param {Event} e Sự kiện input
 */
const handleInput = (e) => {
  emit("update:modelValue", e.target.value);
};

/**
 * Xử lý change của select
 * @param {*} value Giá trị được chọn
 */
const handleSelectChange = (value) => {
  emit("update:modelValue", value);
};

/**
 * Xử lý change của DatePicker
 * @param {*} date Giá trị chọn (kiểu dayjs hoặc null)
 */
const handleDateChange = (date) => {
  emit("update:modelValue", date ? date.toDate() : null);
};
//#endregion Methods
</script>

<template>
  <div class="ms-input flex-1">
    <!-- kiểu text -->
    <input
      v-if="type === 'text'"
      type="text"
      :placeholder="placeholder"
      :value="modelValue"
      @input="handleInput"
      :disabled="disabled"
      @blur="$emit('blur', $event)"
      maxlength="255"
      minlength="1"
    />

    <!-- kiểu date - dùng Antd DatePicker -->
    <a-date-picker
      v-else-if="type === 'date'"
      :placeholder="placeholder"
      class="ms-input-date"
      :allowClear="allowClear"
      format="DD/MM/YYYY"
      :value="modelValue ? dayjs(modelValue) : null"
      @change="handleDateChange"
    >
      <template #suffixIcon>
        <div :class="icon"></div>
      </template>
    </a-date-picker>

    <!-- select -->
    <a-select
      v-else-if="type === 'select'"
      class="ms-input-select"
      :placeholder="placeholder"
      :options="options"
      :allowClear="allowClear"
      :value="modelValue"
      @change="handleSelectChange"
    >
      <template #suffixIcon>
        <div :class="icon"></div>
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

/* Text disabled */
.ms-input input:disabled {
  background: #f5f5f5;
  border-color: #d9d9d9;
  color: #a0a0a0;
  cursor: not-allowed;
}

.ms-input input:disabled:hover,
.ms-input input:disabled:focus {
  border-color: #d9d9d9;
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
