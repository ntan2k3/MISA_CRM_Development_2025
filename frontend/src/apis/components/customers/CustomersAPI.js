import api from "@/apis/config/APIConfig.js";
import BaseAPI from "@/apis/base/BaseAPI.js";

class CustomersAPI extends BaseAPI {
  constructor() {
    super();
    this.controller = "Customers";
  }

  /**
   * Lấy ra tổng số bản ghi
   * @param {*} query
   * @returns
   */
  getTotalData(query = {}) {
    return api.get(`${this.controller}/count`, {
      params: query,
    });
  }

  /**
   * Xóa mềm hàng loạt bản ghi
   * @param {*} ids
   * @returns
   */
  softDeleteMany(ids) {
    return api.delete(`${this.controller}/bulk`, { data: ids });
  }

  /**
   * Tự sinh mã khách hàng
   * @returns
   */
  getNewCustomerCode() {
    return api.get(`${this.controller}/new-code`);
  }

  /**
   * export excel
   * @param {*} ids
   * @returns
   */
  exportCsv(ids) {
    return api.post(`${this.controller}/export`, ids);
  }

  /**
   * import excel
   * @param {*} file
   * @returns
   */
  importCsv(file) {
    return api.post(`${this.controller}/import`, file, {
      headers: { "Content-Type": "multipart/form-data" },
    });
  }

  /**
   * upload ảnh tạm thời
   * @param {*} file
   * @returns
   */
  uploadTempAvatar(file) {
    return api.post(`${this.controller}/upload-temp-avatar`, file, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
  }

  /**
   * upload ảnh tạm thời
   * @param {*} ids,
   * @param {*} type,
   * @returns
   */
  assignCustomerType(ids, type) {
    return api.post(`${this.controller}/assign-type`, {
      CustomerIds: ids,
      CustomerType: type,
    });
  }
}

export default new CustomersAPI();
