<script setup>
import MsButton from "@/components/ms-button/MsButton.vue";

//#region Props
/**
 * Props cấu hình cho Modal
 */
defineProps({
  /**
   * Tiêu đề modal
   * @type {string}
   */
  title: {
    type: String,
    default: "Modal Title",
  },

  /**
   * Trạng thái hiển thị modal
   * @type {boolean}
   */
  show: {
    type: Boolean,
    default: false,
  },

  /**
   * Text nút Hủy
   * @type {string}
   */
  cancelText: {
    type: String,
    default: "Hủy",
  },

  /**
   * Text nút Xác nhận
   * @type {string}
   */
  confirmText: {
    type: String,
    default: "Xác nhận",
  },

  /**
   * Cho phép hiển thị nút đóng (X)
   * @type {boolean}
   */
  closable: {
    type: Boolean,
    default: true,
  },
});
//#endregion Props

//#region Emits
/**
 * Emits ra ngoài cho component cha xử lý:
 * - update:show: đóng/mở modal
 * - cancel: khi bấm Hủy hoặc nút X
 * - confirm: khi bấm nút Xác nhận
 */
const emit = defineEmits(["update:show", "cancel", "confirm"]);
//#endregion Emits

//#region Methods
/**
 * Đóng modal
 * - Emit update:show = false
 * - Emit cancel
 */
const closeModal = () => {
  emit("update:show", false);
  emit("cancel");
};

/**
 * Xác nhận modal
 * - Emit confirm
 */
const confirmModal = () => {
  emit("confirm");
};
//#endregion Methods
</script>

<template>
  <div v-if="show" class="ms-modal-overlay">
    <div class="ms-modal-box flex-col justify-between">
      <!-- Header -->
      <div class="ms-modal-header flex justify-between items-center">
        <div class="ms-modal-title">
          <slot name="title">{{ title }}</slot>
        </div>
        <button v-if="closable" class="ms-modal-close" @click="closeModal">×</button>
      </div>

      <!-- Body -->
      <div class="ms-modal-body">
        <slot>Modal content goes here...</slot>
      </div>

      <!-- Footer -->
      <div class="ms-modal-footer flex justify-end gap-8">
        <slot name="footer">
          <ms-button class="cancel-btn" @click="closeModal">{{ cancelText }}</ms-button>
          <ms-button type="primary" variant="solid" class="confirm-btn" @click="confirmModal">{{
            confirmText
          }}</ms-button>
        </slot>
      </div>
    </div>
  </div>
</template>

<style scoped>
.ms-modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.3);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 9999;
}

.ms-modal-box {
  background: #fff;
  border-radius: 8px;
  min-width: 400px;
  max-width: 800px;
  min-height: 200px;
  padding: 20px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.2);
}

.ms-modal-header {
  font-weight: bold;
  font-size: 16px;
  margin-bottom: 10px;
}

.ms-modal-title {
  flex: 1;
}

.ms-modal-close {
  background: transparent;
  border: none;
  font-size: 20px;
  cursor: pointer;
}

.ms-modal-body {
  margin-bottom: 16px;
}

.ms-modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 8px;
}

.ms-modal-btn {
  padding: 6px 12px;
  border-radius: 4px;
  border: none;
  cursor: pointer;
}

.cancel-btn {
  padding: 5px 16px !important;
}
.confirm-btn {
  padding: 5px 16px !important;
}
</style>
