using Sunny.UI;
using SupermarketApp.Forms;
using SupermarketApp.Data;
using SupermarketApp.Services;
using System;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
 
namespace SupermarketApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Thiết lập ngôn ngữ (Culture) cho toàn ứng dụng.
            // Mặc định vi-VN; có thể đổi bằng cách lưu cài đặt "UILanguage" (ví dụ: "en-US").
            try
            {
                var svc = new SettingsService();
                var lang = svc.GetSettingAsync("UILanguage").GetAwaiter().GetResult();
                var cultureName = string.IsNullOrWhiteSpace(lang) ? "vi-VN" : lang.Trim();
                var culture = new CultureInfo(cultureName);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
                // Áp dụng cho Resources nếu cần
                SupermarketApp.Properties.Resources.Culture = culture;
            }
            catch { }

            // Sunny.UI: bỏ thiết lập UIStyleManager.Style để tránh lỗi CS0120 (thuộc tính không static trên phiên bản hiện tại).
            // Màu sắc giao diện đã được đặt trực tiếp trong các Form (header/sidebar, button).

            // Seed dữ liệu mẫu (chỉ chạy lần đầu, nếu database trống)
            try
            {
                using (var db = new SupermarketContext())
                {
                    // Ensure admin account exists with secure password
                    DataSeederUpdate.SeedDefaultAdmin(db);

                    // Ensure DB có cột 'LoaiKhachHang', backfill và seed 'Khách vãng lai'
                    DataSeederUpdate.EnsureCustomerCategories(db);

                    // Ensure CAIDAT contains 'CustomerCategories' setting for UI category options
                    DataSeederUpdate.EnsureCustomerCategorySetting(db);
                    
                    // Seed other sample data if needed
                    // DataSeeder.SeedData();
                }
            }
            catch (Exception ex)
            {
                // Bỏ qua lỗi nếu database chưa được migrate
                Console.WriteLine("Seed data warning: " + ex.Message);
            }

            Application.Run(new LoginForm());
        }
    }
}
