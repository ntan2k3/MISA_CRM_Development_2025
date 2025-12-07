<script setup>
import { ref, watch } from "vue";
import { formatNumber, formatDate, formatText } from "@/utils/formatter";
import MsModal from "@/components/ms-modal/MsModal.vue";

//#region Props
/**
 * Props nhận cấu hình table, danh sách dữ liệu, sorting, loading,...
 */
const props = defineProps({
  /**
   * Danh sách các cột trong bảng
   * @example [{ key: "fullName", label: "Họ và tên", type: "text" }]
   */
  fields: {
    type: Array,
    required: true,
    validator: (value) =>
      value.every((field) => {
        const validTypes = ["text", "number", "date", "custom"];
        return field.key && field.label && validTypes.includes(field.type || "text");
      }),
  },

  /**
   * Danh sách dữ liệu hiển thị
   */
  rows: {
    type: Array,
    required: true,
  },

  /**
   * Tên cột đang sort
   */
  sortBy: String,

  /**
   * Chiều sort: asc | desc
   */
  sortDirection: {
    type: String,
    default: "asc",
  },

  /**
   * Trạng thái loading
   */
  loading: Boolean,

  /**
   * Số lượng items checked — nhận từ cha
   */
  checkedItemCount: Number,
});
//#endregion Props

//#region Emits
/**
 * Emits ra ngoài cho component cha:
 * - edit: mở form sửa
 * - update:checkedItemCount: cập nhật số lượng tick
 * - selectionChange: gửi danh sách item được chọn
 * - applyFilter: áp dụng filter
 */
const emit = defineEmits(["edit", "update:checkedItemCount", "selectionChange", "applyFilter"]);
//#endregion Emits

//#region State
/** Danh sách ID được tick */
const checkedIds = ref([]);

/** Trạng thái checkbox "chọn tất cả" */
const isCheckAll = ref(false);

/** Trạng thái mở modal filter */
const isFilterModalOpen = ref(false);

/** Cột đang filter */
const currentFilterField = ref(null);

/** Giá trị filter được chọn */
const selectedFilterValue = ref("");
//#endregion State

//#region Format Function
/**
 * Format giá trị theo type: number, date, text
 * @param {*} value Giá trị
 * @param {string} type Kiểu dữ liệu
 * @returns {*}
 */
const handleFormat = (value, type) => {
  switch (type) {
    case "number":
      return formatNumber(value);
    case "date":
      return formatDate(value);
    default:
      return formatText(value);
  }
};
//#endregion Format Function

//#region Methods
/**
 * Toggle chọn / bỏ chọn toàn bộ dòng
 */
const toggleCheckAll = () => {
  if (isCheckAll.value) {
    checkedIds.value = [];
  } else {
    checkedIds.value = props.rows.map((row) => row.customerId);
  }
};

/**
 * Toggle chọn / bỏ chọn một dòng
 * @param {string} rowId ID của dòng
 */
const toggleRow = (rowId) => {
  if (checkedIds.value.includes(rowId)) {
    checkedIds.value = checkedIds.value.filter((id) => id !== rowId);
  } else {
    checkedIds.value.push(rowId);
  }
};

/**
 * Emit sự kiện chỉnh sửa
 * @param {*} row Bản ghi được double-click
 */
const handleEdit = (row) => emit("edit", row.customerId);

/**
 * Sort một cột theo chiều asc/desc
 * @param {string} fieldKey Key của cột
 */
const handleSort = (fieldKey) => {
  let direction = "asc";

  if (props.sortBy === fieldKey) {
    direction = props.sortDirection === "asc" ? "desc" : "asc";
  }

  emit("update:sortBy", fieldKey);
  emit("update:sortDirection", direction);
};

/**
 * Mở modal filter của một cột
 * @param {*} field Cột đang filter
 */
const openFilterModal = (field) => {
  currentFilterField.value = field;
  selectedFilterValue.value = "";
  isFilterModalOpen.value = true;
};

/**
 * Đóng modal filter
 */
const closeFilterModal = () => {
  isFilterModalOpen.value = false;
  currentFilterField.value = null;
};

/**
 * Áp dụng filter
 */
const applyFilter = () => {
  if (!selectedFilterValue.value) return;

  emit("applyFilter", {
    fieldKey: currentFilterField.value.key,
    value: selectedFilterValue.value,
  });

  closeFilterModal();
};
//#endregion Methods

//#region Watchers
/**
 * Watch danh sách checkedIds:
 * - cập nhật trạng thái checkAll
 * - emit số lượng items được chọn
 * - emit danh sách rows được chọn
 */
watch(
  () => checkedIds.value,
  () => {
    const rowIds = props.rows.map((r) => r.customerId);

    isCheckAll.value = rowIds.length > 0 && rowIds.every((id) => checkedIds.value.includes(id));

    emit("update:checkedItemCount", checkedIds.value.length);

    emit(
      "selectionChange",
      props.rows.filter((r) => checkedIds.value.includes(r.customerId))
    );
  },
  { deep: true }
);
//#endregion Watchers
</script>

<template>
  <div class="ms-table flex-col flex-1">
    <a-spin
      :spinning="loading"
      tip="Đang tải dữ liệu..."
      size="medium"
      style="display: flex; justify-content: center; align-items: center; min-height: 500px"
    >
      <table>
        <colgroup>
          <col style="width: 50px" />
          <col
            v-for="field in fields"
            :key="field.key"
            :style="{ width: field.width || '150px' }"
          />
        </colgroup>

        <thead class="ms-table__head">
          <tr class="ms-table__row">
            <th class="ms-table__cell ms-table__cell--checkbox">
              <input
                type="checkbox"
                class="ms-table__checkbox"
                :checked="isCheckAll"
                @change="toggleCheckAll"
              />
            </th>
            <th
              v-for="field in fields"
              :key="field.key"
              class="ms-table__cell ms-table__header"
              @click="handleSort(field.key)"
            >
              {{ field.label }}

              <!-- Nếu có filter thì hiện icon -->
              <div
                v-if="field.filter"
                class="filter-btn flex-center"
                @click.stop="openFilterModal(field)"
              >
                <div class="icon-filter icon-bg icon-16"></div>
              </div>
            </th>
          </tr>
        </thead>

        <tbody class="ms-table__body">
          <tr
            v-for="row in rows"
            :key="row.customerId"
            class="ms-table__row"
            @dblclick="handleEdit(row)"
          >
            <td class="ms-table__cell ms-table__cell--checkbox" @dblclick.prevent.stop>
              <input
                type="checkbox"
                class="ms-table__checkbox"
                :checked="checkedIds.includes(row.customerId)"
                @change="() => toggleRow(row.customerId)"
              />
            </td>
            <td
              v-for="field in fields"
              :key="field.key"
              class="ms-table__cell"
              :title="row[field.key]"
            >
              {{ handleFormat(row[field.key], field.type || "text") || "--" }}
            </td>
          </tr>
        </tbody>
      </table>
    </a-spin>

    <!-- Filter Modal -->
    <ms-modal
      v-model:show="isFilterModalOpen"
      :title="currentFilterField?.label || 'Filter'"
      cancel-text="Hủy"
      confirm-text="Áp dụng"
      @confirm="applyFilter"
    >
      <div class="modal-body flex-col gap-8">
        <label
          v-for="option in currentFilterField?.options || []"
          :key="option.key"
          class="flex items-center gap-8"
        >
          <input type="radio" :value="option.key" v-model="selectedFilterValue" />
          {{ option.value }}
        </label>
      </div>
    </ms-modal>
  </div>
</template>

<style scoped>
.empty-container {
  background-color: red;
}

.ms-table__container {
  width: 100%;
  height: 100%;
  overflow: hidden;
}

.ms-table {
  position: relative;
  overflow-x: auto;
  overflow-y: auto;
  min-height: 0;
  background-color: white;
}

.ms-table::-webkit-scrollbar {
  width: 11px; /* chiều rộng scrollbar */
  height: 11px; /* nếu scroll ngang */
}

.ms-table::-webkit-scrollbar-track {
  background: #f0f2f4; /* màu track */
  border-radius: 8px;
  padding: 2px; /* padding giữa track và thumb */
}

.ms-table::-webkit-scrollbar-thumb {
  background-color: #c1c4cd; /* màu thumb mặc định */
  border-radius: 8px;
  border: 3px solid #f0f2f4;
}

.ms-table::-webkit-scrollbar-thumb:hover {
  background-color: #7c869c; /* màu khi hover */
}

table {
  width: 100%;
  border-collapse: separate;
  border-spacing: 0;
  table-layout: fixed;
}

/* head */
.ms-table__head {
  border: 1px solid #e9e9e9;
  table-layout: fixed;
}

/* th */
.ms-table__head th {
  background-color: #f0f2f4;
  font-weight: 600;
  position: sticky;
  top: 0;
  z-index: 20;
}

.ms-table__head th:hover {
  border-right: 1px solid #ddd;
}

.ms-table__head th:first-child {
  z-index: 30;
}

/* body */
.ms-table__body {
  min-height: 0;
  overflow-y: auto;
}

/* tr */
.ms-table__row {
  position: relative;
  height: 40px;
  background: white;
}

.ms-table__row:hover {
  background: #f0f2f4;
  cursor: pointer;
}

.ms-table__row:first-child {
  background: #e7ebfd;
}

.ms-table__row:first-child:hover {
  background: #d0d8fb;
}

/* td */
.ms-table__cell {
  position: relative;
  font-size: 13px;
  padding: 0.75rem;
  border-bottom: 1px solid #ddd;
  text-align: left;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.filter-btn {
  position: absolute;
  top: 50%;
  right: 4px;
  transform: translateY(-50%);
  width: 24px;
  height: 24px;
  border-radius: 50%;
}

.filter-btn:hover {
  background-color: #c5c5c5;
}

/* Cột checkbox cố định */
.ms-table__cell--checkbox {
  position: sticky;
  left: 0;
  width: 28px;
  z-index: 10;
  background-color: inherit;
  padding: 0.5rem;
  padding-left: 24px;
  padding-right: 8px;
}

.ms-table__checkbox {
  position: relative;
  top: 2px;
  width: 16px;
  height: 16px;
  margin: 0;
  cursor: pointer;
  appearance: none;
  -webkit-appearance: none;
  border: 1px solid #7c869c;
  border-radius: 4px;
  background-color: #fff;
  outline: none;
}

/* khi checked */
.ms-table__checkbox:checked {
  background-color: #4262f0;
  border-color: #4262f0;
}

.ms-table__checkbox:checked::after {
  position: absolute;
  content: "✔";
  color: white;
  font-size: 12px;
  position: relative;
  top: -1px;
  left: 2px;
}
</style>
