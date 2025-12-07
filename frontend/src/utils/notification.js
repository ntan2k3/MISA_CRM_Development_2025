import { notification } from "ant-design-vue";

/**
 * Hiển thị thông báo
 * @param {string} type Loại thông báo: success, error, warning
 * @param {string} messageText Tiêu đề thông báo
 * @param {string} descriptionText Nội dung thông báo (tùy chọn)
 * Created by: nguyentruongan - 06/12/2025
 */
export const showNotification = (type, messageText, descriptionText = "") => {
  notification[type]({
    message: messageText,
    description: descriptionText,
    duration: 3,
  });
};
