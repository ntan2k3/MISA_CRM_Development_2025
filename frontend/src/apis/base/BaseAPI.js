import api from "@/apis/config/APIConfig.js";

export default class BaseAPI {
  constructor() {
    this.controller = null;
  }
  /**
   * Phương thức lấy tất cả dữ liệu có query
   * @param {*} query
   * @return
   */
  getAll(query = {}) {
    return api.get(`${this.controller}`, {
      params: query,
    });
  }

  /**
   * Phương thức lấy dữ liệu theo id
   * @param {*} id
   */
  getById(id) {
    return api.get(`${this.controller}/${id}`);
  }

  /**
   * Phương thức tạo dữ liệu mới
   * @param {*} body
   */
  add(body) {
    return api.post(`${this.controller}`, body);
  }

  /**
   * Hàm cập nhật dữ liệu
   * @param {*} id
   * @param {*} body
   */
  update(id, body) {
    return api.put(`${this.controller}/${id}`, body);
  }

  /**
   * Hàm xóa bản ghi
   * @param {*} id
   */
  delete(id) {
    return api.delete(`${this.controller}/delete/${id}`);
  }

  /**
   * Hàm lấy dữ liệu phân trang
   * @param {*} payload
   */
  paging(payload) {
    return api.post(`${this.controller}/paging`, payload);
  }
}
