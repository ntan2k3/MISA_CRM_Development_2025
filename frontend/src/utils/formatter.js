/**
 * Hàm định dạng số
 * @param {*} value
 * @returns
 * createdby: pdthien - 15.10.2025
 */
export const formatNumber = (value) => {
  if (value === null || value === undefined) return ''
  return new Intl.NumberFormat().format(value)
}

/**
 * Hàm định dạng ngày tháng
 * @param {*} value
 * @returns
 * createdby: pdthien - 15.10.2025
 */
export const formatDate = (value) => {
  if (!value) return ''
  const date = new Date(value)
  return new Intl.DateTimeFormat().format(date)
}

/**
 * Hàm định dạng chuỗi
 * @param {*} value
 * @returns
 * createdby: pdthien - 15.10.2025
 */
export const formatText = (value) => {
  if (value === null || value === undefined) return ''
  return String(value)
}
