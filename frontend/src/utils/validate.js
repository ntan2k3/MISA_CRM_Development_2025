import { z } from "zod";
/**
 * Schema validate thông tin khách hàng
 *
 * Các rule kiểm tra:
 * - Tên bắt buộc, không vượt quá 255 ký tự
 * - Email bắt buộc, đúng định dạng, không trùng
 * - SĐT bắt buộc, đúng định dạng 10–11 số, không trùng
 * - Các trường khác có thể rỗng hoặc optional
 *
 * Created by: nguyentruongan - 06/12/2025
 */
export const customerSchema = z.object({
  /**
   * Mã khách hàng (không bắt buộc)
   * @type {string | null}
   */
  customerCode: z.string(),

  /**
   * Tên khách hàng (bắt buộc, tối đa 255 ký tự)
   * @type {string}
   */
  customerName: z
    .string()
    .min(1, "Tên khách hàng không được để trống")
    .max(255, "Tên khách hàng không được vượt quá 255 ký tự"),
  /**
   * Email khách hàng
   * - Không được để trống
   * - Đúng định dạng email
   * - Không trùng trong hệ thống
   * @type {string}
   */
  customerEmail: z
    .string()
    .min(1, "Email không được để trống")
    .regex(/^[\p{L}\p{N}._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/u, "Email không đúng định dạng"),
  /**
   * Số điện thoại khách hàng
   * - Không được để trống
   * - Phải đủ 10-11 số, bắt đầu bằng 0
   * - Không trùng trong hệ thống
   * @type {string}
   */
  customerPhoneNumber: z
    .string()
    .min(1, "Số điện thoại không được để trống")
    .regex(/^0\d{9,10}$/, "Số điện thoại phải đúng định dạng và đủ 10-11 số"),

  /**
   * Mã số thuế (optional)
   * @type {string | null}
   */
  customerTaxCode: z.string().optional().nullable(),

  /**
   * Mã mặt hàng đã mua (optional)
   * @type {string | null}
   */
  purchasedItemCode: z.string().optional().nullable(),

  /**
   * Tên mặt hàng đã mua (optional)
   * @type {string | null}
   */
  purchasedItemName: z.string().optional().nullable(),

  /**
   * Kiểu khách hàng (optional)
   * @type {string | null}
   */
  customerType: z.string().optional().nullable(),

  /**
   * Ngày mua gần nhất (optional)
   * @type {string | null}
   */
  lastPurchaseDate: z.date().optional().nullable(),
  /**
   * Địa chỉ khách hàng (optional)
   * @type {string | null}
   */
  customerAddr: z.string().max(255, "Địa chỉ không được vượt quá 255 ký tự"),

  /**
   * Avatar khách hàng (optional)
   * @type {string | null}
   */
  customerAvatarUrl: z.string().optional().nullable(),
});
