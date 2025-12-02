import api from "@/apis/config/APIConfig.js";
import BaseAPI from "@/apis/base/BaseAPI.js";

class CustomersAPI extends BaseAPI {
  constructor() {
    super();
    this.controller = "todos";
  }

  /**
   * Hàm kiểm tra trùng lặp theo nghiệp vụ riêng
   * @param {*} payload
   * @returns
   */
  checkDuplicate(payload) {
    return api.post(`${this.controller}/check-duplicate`, payload);
  }
}

export default new CustomersAPI();
