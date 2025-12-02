<script setup>
import { formatNumber, formatDate, formatText } from "@/utils/formatter";

//#region Props
defineProps({
  fields: {
    type: Array,
    required: true,
    validator: (value) => {
      return value.every((field) => {
        const validTypes = ["text", "number", "date", "custom"];
        return field.key && field.label && validTypes.includes(field.type || "text");
      });
    },
  },
  rows: {
    type: Array,
    required: true,
  },
});
//#endregion

//#region Emits
const emit = defineEmits(["edit", "delete"]);
//#endregion

//#region Methods
/**
 * Hàm định dạng giá trị dựa trên kiểu
 * @param value
 * @param type
 * @returns
 * createdby: pdthien - 15.10.2025
 */
const handleFormat = (value, type) => {
  switch (type) {
    case "number":
      return formatNumber(value);
    case "date":
      return formatDate(value);
    case "text":
      return formatText(value);
    default:
      return formatText(value);
  }
};

/**
 * Hàm xử lý sửa bản ghi
 * @param row
 * createdby: pdthien - 15.10.2025
 */
const handleEdit = (row) => {
  emit("edit", row);
};

/**
 * Hàm xử lý xóa bản ghi
 * @param row
 * createdby: pdthien - 15.10.2025
 */
// const handleDelete = (row) => {
//   emit("delete", row);
// };
//#endregion
</script>

<template>
  <div class="ms-table__container flex-col flex-1">
    <div class="ms-table flex-1">
      <table>
        <thead class="ms-table__head">
          <tr class="ms-table__row">
            <th class="ms-table__cell ms-table__cell--checkbox">
              <input type="checkbox" class="ms-table__checkbox" />
            </th>

            <th v-for="field in fields" :key="field.key" class="ms-table__cell">
              {{ field.label }}
            </th>
          </tr>
        </thead>

        <tbody class="ms-table__body">
          <tr
            v-for="(row, rowIndex) in rows"
            :key="rowIndex"
            class="ms-table__row"
            @click="handleEdit(row)"
          >
            <td class="ms-table__cell ms-table__cell--checkbox" @click.stop>
              <input type="checkbox" class="ms-table__checkbox" />
            </td>

            <td v-for="field in fields" :key="field.key" class="ms-table__cell">
              <template v-if="field.type === 'custom'">
                <slot :name="field.key" :row="row" :field="field" :value="row[field.key]">
                  {{ handleFormat(row[field.key], "text") }}
                </slot>
              </template>

              <template v-else>
                {{ handleFormat(row[field.key], field.type || "text") }}
              </template>
            </td>

            <!-- <td class="ms-table__actions">
              <button
                @click="handleEdit(row)"
                class="ms-table__btn ms-table__btn--edit flex-center"
              >
                <div class="icon-bg icon-edit icon-16"></div>
              </button>
              <button
                @click="handleDelete(row)"
                class="ms-table__btn ms-table__btn--delete flex-center"
              >
                <div class="icon-bg icon-delete icon-16"></div>
              </button>
            </td> -->
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<style scoped>
.ms-table__container {
  width: 100%;
  height: 100%;
  overflow: hidden;
}

.ms-table {
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
  height: 40px;
  background: white;
  position: relative;
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
  font-size: 13px;
  padding: 0.75rem;
  border-bottom: 1px solid #ddd;
  text-align: left;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

/* Cột checkbox cố định */
.ms-table__cell--checkbox {
  position: sticky;
  left: 0;
  width: 28px;
  z-index: 10;
  background-color: inherit;
  padding: 0.5rem;
  padding-left: 12px;
  padding-right: 4px;
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

/* .ms-table__actions {
  position: absolute;
  right: 0;
  top: 50%;
  transform: translateY(-50%);
  display: flex;
  gap: 8px;
  padding-right: 16px;
  opacity: 0;
  pointer-events: none;
}

.ms-table__row:hover .ms-table__actions {
  opacity: 1;
  pointer-events: auto;
}

.ms-table__btn {
  width: 28px;
  height: 28px;
  border-radius: 50%;
  border: none;
  background: white;
  cursor: pointer;
  padding: 6px;
  box-shadow: 0 0 4px rgba(0, 0, 0, 0.1), 0 4px 8px rgba(0, 0, 0, 0.1);
}

.ms-table__btn:hover {
  background: #d3d7de;
} */
</style>
