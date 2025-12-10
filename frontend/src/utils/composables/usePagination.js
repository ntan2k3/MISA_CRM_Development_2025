/**
 * hook quản lý phân trang dùng trong Vue 3.
 * Cung cấp state + computed + các hàm điều khiển trang.
 *
 * @param {number} defaultSize - Số bản ghi mặc định mỗi trang.
 * @returns Object chứa state phân trang & các hàm điều khiển.
 * Created by: nguyentruongan - 06/12/2025
 */

import { ref, computed } from "vue";

export function usePagination(defaultSize = 100) {
  //#region States

  // số bản ghi mỗi trang
  const pageSize = ref(defaultSize);

  // trang hiện tại
  const pageNumber = ref(1);

  // tổng số bản ghi
  const total = ref(0);

  //#endregion

  //#region Computed
  const totalPages = computed(() => Math.ceil(total.value / pageSize.value || 1));

  //#endregion

  //#region Methods

  /** Sang trang tiếp theo */
  const nextPage = () => {
    if (pageNumber.value < totalPages.value) {
      pageNumber.value++;
    }
  };

  /** Quay về trang trước */
  const prevPage = () => {
    if (pageNumber.value > 1) {
      pageNumber.value--;
    }
  };

  /** Nhảy về trang đầu */
  const startPage = () => {
    pageNumber.value = 1;
  };

  /** Nhảy đến trang cuối */
  const endPage = () => {
    pageNumber.value = totalPages.value;
  };

  //#endregion

  return {
    pageSize,
    pageNumber,
    total,
    totalPages,
    nextPage,
    prevPage,
    startPage,
    endPage,
  };
}
