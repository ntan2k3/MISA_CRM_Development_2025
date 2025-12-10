<script setup>
import { computed, ref, watch } from "vue";
import { formatNumber, formatDate, formatText } from "@/utils/formatter";
import { debounce } from "lodash";

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
    default: "desc",
  },

  /**
   * icon cho điện thoại
   */
  icon: String,

  /**
   * Trạng thái loading
   */
  loading: Boolean,

  /**
   * Số lượng items checked — nhận từ cha
   */
  checkedItemCount: Number,

  /**
   * Danh sách ID được chọn từ parent
   * Nếu parent truyền prop này, component sẽ sync với nó
   */
  selectedIds: {
    type: Array,
    default: () => [],
  },
});
//#endregion Props

//#region Emits
/**
 * Emits ra ngoài cho component cha:
 * - edit: mở form sửa
 * - update:checkedItemCount: cập nhật số lượng tick
 * - selectionChange: gửi danh sách item được chọn
 * - update:selectedIds: sync selectedIds với parent (hỗ trợ v-model:selectedIds)
 * - update:sortBy: cập nhật cột đang sort
 * - update:sortDirection: cập nhật chiều sort
 */
const emit = defineEmits([
  "edit",
  "update:checkedItemCount",
  "selectionChange",
  "update:selectedIds",
  "update:sortBy",
  "update:sortDirection",
]);
//#endregion Emits

//#region State
/** Danh sách ID được tick (internal state) */
const checkedIds = ref([]);

/** Trạng thái checkbox "chọn tất cả" */
const isCheckAll = ref(false);
//#endregion State

//#region Computed
/**
 * Tạo Set để check ID nhanh hơn
 * - Array.includes() có complexity O(n)
 * - Set.has() có complexity O(1)
 * - Với 1000 rows, cải thiện performance 1000 lần
 */
const checkedIdsSet = computed(() => new Set(checkedIds.value));

/**
 * Tính toán selected rows một lần
 * Tránh filter lại mỗi lần emit
 */
const selectedRows = computed(() =>
  props.rows.filter((r) => checkedIdsSet.value.has(r.customerId))
);
//#endregion Computed

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
  // Lấy ra id cột muốn bỏ chọn
  const index = checkedIds.value.indexOf(rowId);
  if (index > -1) {
    checkedIds.value.splice(index, 1);
  } else {
    checkedIds.value.push(rowId);
  }
};

/**
 * Emit sự kiện chỉnh sửa khi double-click vào row
 * @param {Object} row Bản ghi được double-click
 */
const handleEdit = (row) => emit("edit", row.customerId);

/**
 * Sort một cột theo chiều asc/desc
 * @param {string} fieldKey Key của cột cần sort
 */
const handleSort = (fieldKey) => {
  let direction = "desc";

  if (props.sortBy === fieldKey) {
    direction = props.sortDirection === "asc" ? "desc" : "asc";
  }

  emit("update:sortBy", fieldKey);
  emit("update:sortDirection", direction);
};

/**
 * Method để clear toàn bộ selection
 * Parent component có thể gọi method này qua ref
 * @example tableRef.value?.clearSelection()
 */
const clearSelection = () => {
  checkedIds.value = [];
  isCheckAll.value = false;
};

/**
 * Method để set selection từ bên ngoài
 * @param {Array} ids Danh sách ID cần chọn
 * @example tableRef.value?.setSelection(['id1', 'id2'])
 */
const setSelection = (ids) => {
  checkedIds.value = [...ids];
};
//#endregion Methods

//#region Watchers
/**
 * Watch selectedIds từ parent để sync state
 */
watch(
  () => props.selectedIds,
  (newIds) => {
    // Tạo set mới lưu danh sách checkedIds mới
    const newSet = new Set(newIds);

    const currentSet = checkedIdsSet.value;

    // Chỉ update khi có sự khác biệt
    if (newSet.size !== currentSet.size || ![...newSet].every((id) => currentSet.has(id))) {
      checkedIds.value = [...newIds];
    }
  },
  { deep: true, immediate: true }
);

/**
 * Debounce emit để tránh emit quá nhiều
 * Khi user chọn nhanh nhiều checkbox, chỉ emit 1 lần sau 50ms
 */
const emitSelection = debounce(() => {
  emit("update:checkedItemCount", checkedIds.value.length);
  emit("selectionChange", selectedRows.value);
  emit("update:selectedIds", checkedIds.value);
}, 50);

/**
 * Watch danh sách checkedIds để:
 * - Cập nhật trạng thái checkAll
 * - Emit số lượng items được chọn
 * - Emit danh sách rows được chọn
 * - Emit update:selectedIds để sync với parent (v-model)
 */
watch(
  () => checkedIds.value,
  () => {
    const rowIds = props.rows.map((r) => r.customerId);

    // Cập nhật trạng thái "chọn tất cả"
    isCheckAll.value = rowIds.length > 0 && rowIds.every((id) => checkedIds.value.includes(id));

    // Emit các events
    emitSelection();
  },
  { deep: true }
);

/**
 * Loại bỏ các ID không còn tồn tại
 * Khi rows thay đổi (filter, search, pagination), tự động xóa các ID không hợp lệ
 * Tránh lỗi khi user đã chọn row nhưng row đó bị filter ra khỏi danh sách
 */
watch(
  () => props.rows,
  (newRows) => {
    const validIds = new Set(newRows.map((r) => r.customerId));
    const filteredIds = checkedIds.value.filter((id) => validIds.has(id));

    // Chỉ update nếu có ID không hợp lệ
    if (filteredIds.length !== checkedIds.value.length) {
      checkedIds.value = filteredIds;
    }
  }
);
//#endregion Watchers

//#region Expose
/**
 * Expose methods để parent có thể gọi qua ref
 * @example
 * const tableRef = ref(null);
 * tableRef.value?.clearSelection();
 * tableRef.value?.setSelection(['id1', 'id2']);
 */
defineExpose({
  clearSelection,
  setSelection,
});
//#endregion Expose
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
              <div class="header-content flex justify-between">
                {{ field.label }}
                <!-- Icon sort -->
                <span class="sort-icon">
                  <div
                    v-if="props.sortBy === field.key"
                    :class="{
                      'icon-bg icon-sort-asc icon-16': props.sortDirection === 'asc',
                      'icon-bg icon-sort-desc icon-16': props.sortDirection === 'desc',
                    }"
                  ></div>
                  <div v-else class="icon-bg icon-sort-asc icon-16 default-arrow"></div>
                </span>
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
                :checked="checkedIdsSet.has(row.customerId)"
                @change="() => toggleRow(row.customerId)"
              />
            </td>
            <td
              v-for="field in fields"
              :key="field.key"
              class="ms-table__cell"
              :title="row[field.key]"
            >
              <div v-if="field.icon" class="flex items-center gap-4">
                <div :class="field.icon"></div>
                {{ handleFormat(row[field.key], field.type || "text") || "--" }}
              </div>
              <span v-else>
                {{ handleFormat(row[field.key], field.type || "text") || "--" }}
              </span>
            </td>
          </tr>
        </tbody>
      </table>

      <!-- Empty state -->
      <div v-if="rows.length === 0" class="table-empty-overlay flex-col flex-center">
        <div class="empty-text">Không có bản ghi nào</div>
      </div>
    </a-spin>
  </div>
</template>

<style scoped>
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
  width: 11px;
  height: 11px;
}

.ms-table::-webkit-scrollbar-track {
  background: #f0f2f4;
  border-radius: 8px;
  padding: 2px;
}

.ms-table::-webkit-scrollbar-thumb {
  background-color: #c1c4cd;
  border-radius: 8px;
  border: 3px solid #f0f2f4;
}

.ms-table::-webkit-scrollbar-thumb:hover {
  background-color: #7c869c;
}

table {
  width: 100%;
  border-collapse: separate;
  border-spacing: 0;
  table-layout: fixed;
}

.ms-table__head {
  border: 1px solid #e9e9e9;
  table-layout: fixed;
}

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

.ms-table__body {
  min-height: 0;
  overflow-y: auto;
}

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
  opacity: 0;
}

.ms-table__head th:hover .filter-btn {
  opacity: 1;
  transition: opacity 0.2s;
}

.filter-btn:hover {
  background-color: #c5c5c5;
}

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

.sort-icon {
  opacity: 0;
  transition: all 0.2s ease-in-out;
}

.ms-table__header:hover .sort-icon {
  opacity: 1;
}

.table-empty-overlay {
  position: absolute;
  top: 200px;
  left: 0;
  right: 0;
  bottom: 0;
  background: #fff;
  z-index: 100;
  opacity: 0.9;
}

.empty-text {
  margin-top: 12px;
  color: #7c869c;
  font-size: 14px;
}
</style>
