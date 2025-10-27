using Sunny.UI;
using System;
using System.Windows.Forms;

namespace SupermarketApp.Utils
{
    /// <summary>
    /// Helper class để hiển thị MessageBox với tiếng Việt hoàn toàn
    /// Loại bỏ tất cả thông báo tiếng Trung Quốc
    /// </summary>
    public static class MessageHelper
    {
        /// <summary>
        /// Hiển thị thông báo thành công (Xanh lá cây)
        /// </summary>
        public static void ShowSuccess(string message)
        {
            MessageBox.Show(message, "✓ Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Hiển thị thông báo cảnh báo (Cam)
        /// </summary>
        public static void ShowWarning(string message)
        {
            MessageBox.Show(message, "⚠ Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Hiển thị thông báo lỗi (Đỏ)
        /// </summary>
        public static void ShowError(string message)
        {
            MessageBox.Show(message, "✗ Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Hiển thị thông báo thông tin (Xanh dương)
        /// </summary>
        public static void ShowInfo(string message)
        {
            MessageBox.Show(message, "ℹ Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Hiển thị hộp xác nhận với 2 nút (Có/Không)
        /// </summary>
        public static bool ShowAsk(string message)
        {
            var result = MessageBox.Show(message, "❓ Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return result == DialogResult.Yes;
        }

        /// <summary>
        /// Hiển thị hộp thoại tùy chỉnh với tiêu đề riêng
        /// </summary>
        public static void Show(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Hiển thị thông báo tạm thời (Tooltip) bằng Label tạm thời
        /// Thay vì SunnyUI UIMessageTip (hiển thị tiếng Trung)
        /// </summary>
        public static void ShowTip(string message)
        {
            // Thay vì: UIMessageTip.ShowOk(message); <- Hiển thị tiếng Trung
            MessageBox.Show(message, "✓ Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Hiển thị thông báo lỗi tạm thời bằng MessageBox
        /// Thay vì SunnyUI UIMessageTip (hiển thị tiếng Trung)
        /// </summary>
        public static void ShowTipError(string message)
        {
            // Thay vì: UIMessageTip.ShowError(message); <- Hiển thị tiếng Trung
            MessageBox.Show(message, "✗ Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Hiển thị thông báo cảnh báo tạm thời bằng MessageBox
        /// Thay vì SunnyUI UIMessageTip (hiển thị tiếng Trung)
        /// </summary>
        public static void ShowTipWarning(string message)
        {
            // Thay vì: UIMessageTip.ShowWarning(message); <- Hiển thị tiếng Trung
            MessageBox.Show(message, "⚠ Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Hiển thị thông báo thành công tạm thời bằng MessageBox
        /// Thay vì SunnyUI UIMessageTip (hiển thị tiếng Trung)
        /// </summary>
        public static void ShowTipSuccess(string message)
        {
            // Thay vì: UIMessageTip.ShowOk(message); <- Hiển thị tiếng Trung
            MessageBox.Show(message, "✓ Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}


