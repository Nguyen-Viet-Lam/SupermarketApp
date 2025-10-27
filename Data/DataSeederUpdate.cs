using System;
using System.Linq;
using System.Security.Cryptography;
using SupermarketApp.Data.Models;
using Microsoft.Data.SqlClient;
using SupermarketApp;

namespace SupermarketApp.Data
{
    public static class DataSeederUpdate
    {
        /// <summary>
        /// Seed default admin account with secure password (Salt + PBKDF2)
        /// Call this method on app startup to ensure admin account exists
        /// </summary>
        public static void SeedDefaultAdmin(SupermarketContext db)
        {
            try
            {
                // Check if admin exists
                var adminExists = db.NhanVien.Any(x => x.TaiKhoan == "admin");
                
                if (!adminExists)
                {
                    Console.WriteLine("Creating default admin account...");
                    
                    // Create password hash with salt
                    string defaultPassword = "admin123";
                    byte[] salt = new byte[32];
                    using (var rng = RandomNumberGenerator.Create())
                    {
                        rng.GetBytes(salt);
                    }
                    
                    byte[] hash;
                    using (var pbkdf2 = new Rfc2898DeriveBytes(defaultPassword, salt, 10000, HashAlgorithmName.SHA256))
                    {
                        hash = pbkdf2.GetBytes(32);
                    }
                    
                    // Create admin account
                    var admin = new NhanVien
                    {
                        TenNV = "Administrator",
                        TaiKhoan = "admin",
                        MatKhauHash = hash,
                        Salt = salt,
                        VaiTro = "Admin",
                        TrangThai = true,
                        NgayTao = DateTime.Now
                    };
                    
                    db.NhanVien.Add(admin);
                    db.SaveChanges();
                    
                    Console.WriteLine("✓ Default admin created successfully!");
                    Console.WriteLine("  Username: admin");
                    Console.WriteLine("  Password: admin123");
                }
                else
                {
                    // Check if admin has valid salt
                    var admin = db.NhanVien.FirstOrDefault(x => x.TaiKhoan == "admin");
                    if (admin != null && (admin.Salt == null || admin.Salt.Length < 8))
                    {
                        Console.WriteLine("Updating admin password with valid Salt...");
                        
                        // Update admin password
                        string defaultPassword = "admin123";
                        byte[] salt = new byte[32];
                        using (var rng = RandomNumberGenerator.Create())
                        {
                            rng.GetBytes(salt);
                        }
                        
                        byte[] hash;
                        using (var pbkdf2 = new Rfc2898DeriveBytes(defaultPassword, salt, 10000, HashAlgorithmName.SHA256))
                        {
                            hash = pbkdf2.GetBytes(32);
                        }
                        
                        admin.MatKhauHash = hash;
                        admin.Salt = salt;
                        db.SaveChanges();
                        
                        Console.WriteLine("✓ Admin password updated!");
                        Console.WriteLine("  Username: admin");
                        Console.WriteLine("  Password: admin123");
                        Console.WriteLine($"  Salt Length: {salt.Length} bytes");
                    }
                    else if (admin != null)
                    {
                        Console.WriteLine($"Admin exists with Salt length: {admin.Salt?.Length ?? 0} bytes");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding admin: {ex.Message}");
            }
        }
        /// <summary>
        /// Ensure DB column 'LoaiKhachHang' exists, backfill values, and seed default 'Khách vãng lai' customer.
        /// Call at startup before UI loads to keep reports consistent.
        /// </summary>
        public static void EnsureCustomerCategories(SupermarketContext db)
        {
            try
            {
                var conn = new Microsoft.Data.SqlClient.SqlConnection(AppConfigHelper.GetConnectionString());
                conn.Open();

                // 1) Ensure column LoaiKhachHang exists
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='KHACHHANG' AND COLUMN_NAME='LoaiKhachHang'";
                    var existsObj = cmd.ExecuteScalar();
                    var exists = Convert.ToInt32(existsObj) > 0;
                    if (!exists)
                    {
                        using (var cmdAlter = conn.CreateCommand())
                        {
                            cmdAlter.CommandText = "ALTER TABLE KHACHHANG ADD LoaiKhachHang NVARCHAR(100) NULL";
                            cmdAlter.ExecuteNonQuery();
                        }
                    }
                }

                // 2) Backfill LoaiKhachHang for existing rows
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
UPDATE KHACHHANG
SET LoaiKhachHang = CASE 
    WHEN (SDT IS NULL OR LTRIM(RTRIM(SDT)) = '') AND (Email IS NULL OR LTRIM(RTRIM(Email)) = '') THEN N'Khách vãng lai'
    ELSE N'Khách thân quen'
END
WHERE LoaiKhachHang IS NULL";
                    cmd.ExecuteNonQuery();
                }

                // 3) Ensure default 'Khách vãng lai' customer exists
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM KHACHHANG WHERE TenKH = N'Khách vãng lai'";
                    var countObj = cmd.ExecuteScalar();
                    var count = Convert.ToInt32(countObj);
                    if (count == 0)
                    {
                        using (var cmdIns = conn.CreateCommand())
                        {
                            cmdIns.CommandText = @"
INSERT INTO KHACHHANG (TenKH, SDT, Email, DiaChi, LoaiKhachHang, DiemTichLuy, NgayTao)
VALUES (N'Khách vãng lai', NULL, NULL, NULL, N'Khách vãng lai', 0, GETDATE())";
                            cmdIns.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("EnsureCustomerCategories warning: " + ex.Message);
            }
        }

        /// <summary>
        /// Ensure setting 'CustomerCategories' exists in CAIDAT table.
        /// Seeds default value if missing or empty so UI can load category options.
        /// </summary>
        public static void EnsureCustomerCategorySetting(SupermarketContext db)
        {
            try
            {
                using (var conn = new Microsoft.Data.SqlClient.SqlConnection(AppConfigHelper.GetConnectionString()))
                {
                    conn.Open();
                    var defaultValue = "Khách vãng lai;Khách thân quen;VIP";

                    // Check if setting exists
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT COUNT(*) FROM CAIDAT WHERE TenCaiDat = N'CustomerCategories'";
                        var count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count == 0)
                        {
                            using (var cmdIns = conn.CreateCommand())
                            {
                                cmdIns.CommandText = @"
INSERT INTO CAIDAT (TenCaiDat, GiaTri, MoTa, NgayCapNhat, NguoiCapNhat)
VALUES (N'CustomerCategories', @val, N'Danh sách loại khách hàng cho UI', GETDATE(), N'System')";
                                var p = cmdIns.CreateParameter();
                                p.ParameterName = "@val";
                                p.Value = defaultValue;
                                cmdIns.Parameters.Add(p);
                                cmdIns.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // If value empty, update to default
                            using (var cmdGet = conn.CreateCommand())
                            {
                                cmdGet.CommandText = "SELECT TOP 1 GiaTri FROM CAIDAT WHERE TenCaiDat = N'CustomerCategories'";
                                var valObj = cmdGet.ExecuteScalar();
                                var val = valObj == null ? null : Convert.ToString(valObj);

                                if (string.IsNullOrWhiteSpace(val))
                                {
                                    using (var cmdUpd = conn.CreateCommand())
                                    {
                                        cmdUpd.CommandText = "UPDATE CAIDAT SET GiaTri = @val, NgayCapNhat = GETDATE(), NguoiCapNhat = N'System' WHERE TenCaiDat = N'CustomerCategories'";
                                        var p2 = cmdUpd.CreateParameter();
                                        p2.ParameterName = "@val";
                                        p2.Value = defaultValue;
                                        cmdUpd.Parameters.Add(p2);
                                        cmdUpd.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("EnsureCustomerCategorySetting warning: " + ex.Message);
            }
        }

        /// <summary>
        /// Seed dữ liệu sản phẩm và nhà cung cấp đa dạng
        /// </summary>
        public static void SeedProductsAndSuppliers(SupermarketContext db)
        {
            try
            {
                // Kiểm tra nếu đã có sản phẩm
                if (!db.SanPham.Any())
                {
                    Console.WriteLine("Seeding sample products and suppliers...");

                    // Danh sách nhà cung cấp
                    var suppliers = new[]
                    {
                        new { Name = "Công ty TNHH Thực phẩm ABC", Phone = "0281234567", Address = "123 Nguyễn Văn Linh, Q.1, TP.HCM" },
                        new { Name = "Công ty CP Nước giải khát XYZ", Phone = "0287654321", Address = "456 Lê Lợi, Q.1, TP.HCM" },
                        new { Name = "Công ty TNHH Đồ gia dụng DEF", Phone = "0289876543", Address = "789 Trần Hưng Đạo, Q.5, TP.HCM" },
                        new { Name = "Công ty TNHH Mỹ phẩm GHI", Phone = "0284567890", Address = "321 Nguyễn Trãi, Q.1, TP.HCM" },
                        new { Name = "Công ty CP Vệ sinh JKL", Phone = "0281357924", Address = "654 Lý Thường Kiệt, Q.10, TP.HCM" },
                        new { Name = "Công ty TNHH Thực phẩm đông lạnh MNO", Phone = "0282468135", Address = "987 Hải Thượng Lãn  Ông, Q.5, TP.HCM" }
                    };

                    // Danh sách sản phẩm đa dạng
                    var products = new[]
                    {
                        // Thực phẩm tươi sống
                        new { Name = "Thịt heo ba chỉ", Category = "Thực phẩm tươi sống", Unit = "kg", Price = 120000m },
                        new { Name = "Thịt bò Mỹ", Category = "Thực phẩm tươi sống", Unit = "kg", Price = 250000m },
                        new { Name = "Cá basa fillet", Category = "Thực phẩm tươi sống", Unit = "kg", Price = 150000m },
                        new { Name = "Tôm sú loại 1", Category = "Thực phẩm tươi sống", Unit = "kg", Price = 350000m },
                        new { Name = "Gà công nghiệp nguyên con", Category = "Thực phẩm tươi sống", Unit = "con", Price = 80000m },
                        new { Name = "Trứng gà ta", Category = "Thực phẩm tươi sống", Unit = "quả", Price = 3500m },
                        new { Name = "Rau muống", Category = "Rau c củ quả", Unit = "bó", Price = 5000m },
                        new { Name = "Cải thìa", Category = "Rau c củ quả", Unit = "kg", Price = 20000m },
                        new { Name = "Cà chua", Category = "Rau c củ quả", Unit = "kg", Price = 25000m },
                        new { Name = "Khoai tây Đà Lạt", Category = "Rau c củ quả", Unit = "kg", Price = 18000m },

                        // Nước giải khát
                        new { Name = "Coca Cola lon 330ml", Category = "Nước giải khát", Unit = "lon", Price = 12000m },
                        new { Name = "Pepsi chai 1.5L", Category = "Nước giải khát", Unit = "chai", Price = 18000m },
                        new { Name = "Red Bull", Category = "Nước giải khát", Unit = "lon", Price = 15000m },
                        new { Name = "Nước suối Aquafina 500ml", Category = "Nước giải khát", Unit = "chai", Price = 8000m },
                        new { Name = "Nước cam ép Twister", Category = "Nước giải khát", Unit = "chai", Price = 12000m },
                        new { Name = "Sting dâu", Category = "Nước giải khát", Unit = "chai", Price = 10000m },
                        new { Name = "Trà xanh 0 độ", Category = "Nước giải khát", Unit = "chai", Price = 8000m },
                        new { Name = "Cà phê hòa tan G7", Category = "Đồ uống", Unit = "hộp", Price = 45000m },

                        // Đồ gia dụng
                        new { Name = "Bột giặt Omo", Category = "Đồ gia dụng", Unit = "túi", Price = 25000m },
                        new { Name = "Nước rửa chén Sunlight", Category = "Đồ gia dụng", Unit = "chai", Price = 35000m },
                        new { Name = "Nước lau sàn Mr. Muscle", Category = "Đồ gia dụng", Unit = "chai", Price = 40000m },
                        new { Name = "Khăn giấy Pulppy", Category = "Đồ gia dụng", Unit = "gói", Price = 15000m },
                        new { Name = "Túi nilon đựng rác", Category = "Đồ gia dụng", Unit = "cuộn", Price = 20000m },
                        new { Name = "Bàn chải đánh răng P/S", Category = "Đồ gia dụng", Unit = "cái", Price = 12000m },
                        new { Name = "Kem đánh răng Colgate", Category = "Đồ gia dụng", Unit = "tuýp", Price = 25000m },
                        new { Name = "Dầu gội đầu Clear", Category = "Đồ gia dụng", Unit = "chai", Price = 55000m },
                        new { Name = "Sữa tắm Lifebuoy", Category = "Đồ gia dụng", Unit = "chai", Price = 45000m },

                        // Mỹ phẩm
                        new { Name = "Son môi Maybelline", Category = "Mỹ phẩm", Unit = "thỏi", Price = 120000m },
                        new { Name = "Kem chống nắng Anessa", Category = "Mỹ phẩm", Unit = "tuýp", Price = 350000m },
                        new { Name = "Sữa rửa mặt Cetaphil", Category = "Mỹ phẩm", Unit = "tuýp", Price = 180000m },
                        new { Name = "Nước hoa hồng", Category = "Mỹ phẩm", Unit = "chai", Price = 220000m },
                        new { Name = "Phấn phủ Innisfree", Category = "Mỹ phẩm", Unit = "hộp", Price = 150000m },
                        new { Name = "Serum vitamin C", Category = "Mỹ phẩm", Unit = "chai", Price = 280000m },
                        new { Name = "Kem dưưỡng ẩm La Roche-Posay", Category = "Mỹ phẩm", Unit = "tuýp", Price = 320000m },
                        new { Name = "Tẩy trang Bioderma", Category = "Mỹ phẩm", Unit = "chai", Price = 380000m },

                        // Đồ đông lạnh
                        new { Name = "Chả giò", Category = "Đồ đông lạnh", Unit = "cái", Price = 5000m },
                        new { Name = "Xúc xích", Category = "Đồ đông lạnh", Unit = "cái", Price = 8000m },
                        new { Name = "Thịt gà đông lạnh", Category = "Đồ đông lạnh", Unit = "kg", Price = 65000m },
                        new { Name = "Cá viên", Category = "Đồ đông lạnh", Unit = "kg", Price = 85000m },
                        new { Name = "Mực ống đông lạnh", Category = "Đồ đông lạnh", Unit = "kg", Price = 120000m },
                        new { Name = "Tôm đông lạnh", Category = "Đồ đông lạnh", Unit = "kg", Price = 180000m },
                        new { Name = "Bánh bao nhân thịt", Category = "Đồ đông lạnh", Unit = "cái", Price = 10000m },
                        new { Name = "Nem chua rán", Category = "Đồ đông lạnh", Unit = "cái", Price = 12000m },
                        new { Name = "Xíu mại", Category = "Đồ đông lạnh", Unit = "cái", Price = 15000m }
                    };

                    // Thêm sản phẩm vào database
                    foreach (var product in products)
                    {
                        var sp = new SanPham
                        {
                            TenSP = product.Name,
                            LoaiSP = product.Category,
                            DonVi = product.Unit,
                            DonGia = product.Price,
                            SoLuong = new Random().Next(10, 100),
                            Barcode = $"SP{new Random().Next(1000, 9999)}",
                            MoTa = $"Sản phẩm {product.Name} - {product.Category}",
                            TrangThai = true,
                            NgayTao = DateTime.Now
                        };
                        db.SanPham.Add(sp);
                    }

                    db.SaveChanges();
                    Console.WriteLine($"✓ Seeded {products.Length} products successfully!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding products: {ex.Message}");
            }
        }
    }
}

