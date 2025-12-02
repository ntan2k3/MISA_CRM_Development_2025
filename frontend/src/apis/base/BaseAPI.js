import api from "@/apis/config/APIConfig.js";

export default class BaseAPI {
  constructor() {
    this.controller = null;
  }
  /**
   * Phương thức lấy tất cả dữ liệu
   */
  getAll() {
    return api.get(`${this.controller}`);
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
    return api.update(`${this.controller}/update/${id}`, body);
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
