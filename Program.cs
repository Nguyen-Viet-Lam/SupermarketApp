using Sunny.UI;
using SupermarketApp.Forms;
using SupermarketApp.Data;
using System;
using System.Drawing;
using System.Windows.Forms;
 
namespace SupermarketApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // ⚠️ Font Unicode tiếng Việt sẽ được set trong từng form
            // Segoe UI 10pt dùng cho toàn bộ controls
            // Excel export now uses ClosedXML (no license setup required)

            // Sunny.UI: bỏ thiết lập UIStyleManager.Style để tránh lỗi CS0120 (thuộc tính không static trên phiên bản hiện tại).
            // Màu sắc giao diện đã được đặt trực tiếp trong các Form (header/sidebar, button).

            // Seed dữ liệu mẫu (chỉ chạy lần đầu, nếu database trống)
            try
            {
                using (var db = new SupermarketContext())
                {
                    // Ensure admin account exists with secure password
                    DataSeederUpdate.SeedDefaultAdmin(db);
                    
                    // Seed other sample data - CHỈ TẠO KHI KHÔNG CÓ DỮ LIỆU
                    // DataSeeder.SeedData(); // ĐÃ TẮT - Không tự động seed
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
