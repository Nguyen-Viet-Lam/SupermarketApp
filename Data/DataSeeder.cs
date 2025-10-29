using System;
using System.Linq;
using SupermarketApp.Data.Models;
using SupermarketApp.Services;

namespace SupermarketApp.Data
{
    public class DataSeeder
    {
        public static void SeedData()
        {
            using (var db = new SupermarketContext())
            {
                // Seed Nhân viên (nếu chưa có)
                if (!db.NhanVien.Any())
                {
                    var authService = new AuthService();
                    var (hashAdmin, saltAdmin) = authService.HashPassword("admin123");
                    var (hashNV1, saltNV1) = authService.HashPassword("nv123");
                    var (hashNV2, saltNV2) = authService.HashPassword("nv123");

                    db.NhanVien.AddRange(
                        new NhanVien
                        {
                            TenNV = "Nguyễn Văn Admin",
                            TaiKhoan = "admin",
                            MatKhauHash = hashAdmin,
                            Salt = saltAdmin,
                            VaiTro = "Admin",
                            TrangThai = true,
                            NgayTao = DateTime.Now
                        },
                        new NhanVien
                        {
                            TenNV = "Trần Thị Hoa",
                            TaiKhoan = "hoatt",
                            MatKhauHash = hashNV1,
                            Salt = saltNV1,
                            VaiTro = "Nhân viên",
                            TrangThai = true,
                            NgayTao = DateTime.Now
                        },
                        new NhanVien
                        {
                            TenNV = "Lê Văn Nam",
                            TaiKhoan = "namlv",
                            MatKhauHash = hashNV2,
                            Salt = saltNV2,
                            VaiTro = "Quản lý",
                            TrangThai = true,
                            NgayTao = DateTime.Now
                        }
                    );
                    db.SaveChanges();
                }

                // Seed Khách hàng
                if (!db.KhachHang.Any())
                {
                    db.KhachHang.AddRange(
                        new KhachHang { TenKH = "Nguyễn Thị Mai", SDT = "0901234567", Email = "mai@gmail.com", DiaChi = "123 Lê Lợi, Q1, TP.HCM", DiemTichLuy = 150, NgayTao = DateTime.Now.AddMonths(-6) },
                        new KhachHang { TenKH = "Trần Văn Hùng", SDT = "0912345678", Email = "hung@gmail.com", DiaChi = "456 Nguyễn Huệ, Q1, TP.HCM", DiemTichLuy = 280, NgayTao = DateTime.Now.AddMonths(-4) },
                        new KhachHang { TenKH = "Lê Thị Lan", SDT = "0923456789", Email = "lan@gmail.com", DiaChi = "789 Trần Hưng Đạo, Q5, TP.HCM", DiemTichLuy = 95, NgayTao = DateTime.Now.AddMonths(-3) },
                        new KhachHang { TenKH = "Phạm Minh Tuấn", SDT = "0934567890", Email = "tuan@gmail.com", DiaChi = "321 Hai Bà Trưng, Q3, TP.HCM", DiemTichLuy = 420, NgayTao = DateTime.Now.AddMonths(-8) },
                        new KhachHang { TenKH = "Hoàng Thị Hương", SDT = "0945678901", Email = "huong@gmail.com", DiaChi = "654 Điện Biên Phủ, Bình Thạnh, TP.HCM", DiemTichLuy = 180, NgayTao = DateTime.Now.AddMonths(-2) }
                    );
                    db.SaveChanges();
                }

                // Seed Nhà cung cấp
                if (!db.NhaCungCap.Any())
                {
                    db.NhaCungCap.AddRange(
                        new NhaCungCap { TenNCC = "Công ty TNHH Thực phẩm Việt Nam", DiaChi = "123 Xa Lộ Hà Nội, Q9, TP.HCM", SDT = "0283456789", Email = "contact@tpvn.com", MaSoThue = "0123456789", NguoiLienHe = "Nguyễn Văn A", TrangThai = true, NgayTao = DateTime.Now.AddYears(-2) },
                        new NhaCungCap { TenNCC = "Công ty CP Đồ uống Sài Gòn", DiaChi = "456 Võ Văn Kiệt, Q5, TP.HCM", SDT = "0287654321", Email = "info@saigondrink.com", MaSoThue = "0987654321", NguoiLienHe = "Trần Thị B", TrangThai = true, NgayTao = DateTime.Now.AddYears(-1) },
                        new NhaCungCap { TenNCC = "Công ty TNHH Bánh kẹo Hà Nội", DiaChi = "789 CMT8, Q10, TP.HCM", SDT = "0281234567", Email = "sales@hanoicandy.com", MaSoThue = "1122334455", NguoiLienHe = "Lê Văn C", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-9) },
                        new NhaCungCap { TenNCC = "Công ty CP Gia vị Việt", DiaChi = "321 Lạc Long Quân, Q11, TP.HCM", SDT = "0289876543", Email = "contact@vietspice.com", MaSoThue = "5566778899", NguoiLienHe = "Phạm Thị D", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-6) }
                    );
                    db.SaveChanges();
                }

                // Seed Sản phẩm với nhiều loại hàng phong phú
                if (!db.SanPham.Any())
                {
                    db.SanPham.AddRange(
                        // THỰC PHẨM TƯƠI SỐNG
                        new SanPham { TenSP = "Gạo ST25 cao cấp", DonGia = 25000, SoLuong = 500, DonVi = "Kg", LoaiSP = "Thực phẩm tươi sống", Barcode = "8934561000011", MoTa = "Gạo ST25 thơm ngon, chất lượng cao", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-5) },
                        new SanPham { TenSP = "Gạo Jasmine Thái Lan", DonGia = 18000, SoLuong = 300, DonVi = "Kg", LoaiSP = "Thực phẩm tươi sống", Barcode = "8934561000012", MoTa = "Gạo thơm nhập khẩu Thái Lan", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-4) },
                        new SanPham { TenSP = "Thịt heo ba chỉ", DonGia = 95000, SoLuong = 80, DonVi = "Kg", LoaiSP = "Thực phẩm tươi sống", Barcode = "8934561000022", MoTa = "Thịt heo tươi ngon, nguồn gốc rõ ràng", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-5) },
                        new SanPham { TenSP = "Thịt bò Úc", DonGia = 180000, SoLuong = 25, DonVi = "Kg", LoaiSP = "Thực phẩm tươi sống", Barcode = "8934561000023", MoTa = "Thịt bò nhập khẩu Úc cao cấp", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-3) },
                        new SanPham { TenSP = "Cá hồi Na Uy", DonGia = 350000, SoLuong = 30, DonVi = "Kg", LoaiSP = "Thực phẩm tươi sống", Barcode = "8934561000033", MoTa = "Cá hồi nhập khẩu Na Uy", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-4) },
                        new SanPham { TenSP = "Cá basa fillet", DonGia = 85000, SoLuong = 60, DonVi = "Kg", LoaiSP = "Thực phẩm tươi sống", Barcode = "8934561000034", MoTa = "Cá basa đông lạnh", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-4) },
                        new SanPham { TenSP = "Rau cải ngọt hữu cơ", DonGia = 15000, SoLuong = 120, DonVi = "Kg", LoaiSP = "Thực phẩm tươi sống", Barcode = "8934561000044", MoTa = "Rau sạch hữu cơ", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-3) },
                        new SanPham { TenSP = "Rau muống tươi", DonGia = 12000, SoLuong = 100, DonVi = "Bó", LoaiSP = "Thực phẩm tươi sống", Barcode = "8934561000045", MoTa = "Rau muống tươi ngon", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-2) },
                        new SanPham { TenSP = "Cà chua cherry", DonGia = 45000, SoLuong = 80, DonVi = "Kg", LoaiSP = "Thực phẩm tươi sống", Barcode = "8934561000046", MoTa = "Cà chua cherry ngọt", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-2) },
                        new SanPham { TenSP = "Chuối sứ", DonGia = 18000, SoLuong = 150, DonVi = "Nải", LoaiSP = "Thực phẩm tươi sống", Barcode = "8934561000047", MoTa = "Chuối sứ thơm ngon", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-1) },
                        
                        // ĐỒ UỐNG
                        new SanPham { TenSP = "Coca Cola lon 330ml", DonGia = 10000, SoLuong = 500, DonVi = "Lon", LoaiSP = "Đồ uống", Barcode = "8934561000055", MoTa = "Nước ngọt Coca Cola", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-5) },
                        new SanPham { TenSP = "Pepsi lon 330ml", DonGia = 9500, SoLuong = 450, DonVi = "Lon", LoaiSP = "Đồ uống", Barcode = "8934561000056", MoTa = "Nước ngọt Pepsi", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-5) },
                        new SanPham { TenSP = "Sting dâu 330ml", DonGia = 9000, SoLuong = 450, DonVi = "Chai", LoaiSP = "Đồ uống", Barcode = "8934561000066", MoTa = "Nước tăng lực Sting", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-5) },
                        new SanPham { TenSP = "Red Bull 250ml", DonGia = 15000, SoLuong = 200, DonVi = "Lon", LoaiSP = "Đồ uống", Barcode = "8934561000067", MoTa = "Nước tăng lực Red Bull", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-4) },
                        new SanPham { TenSP = "Trà xanh C2 500ml", DonGia = 8000, SoLuong = 380, DonVi = "Chai", LoaiSP = "Đồ uống", Barcode = "8934561000077", MoTa = "Trà xanh không độ", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-4) },
                        new SanPham { TenSP = "Trà sữa trân châu", DonGia = 25000, SoLuong = 100, DonVi = "Ly", LoaiSP = "Đồ uống", Barcode = "8934561000078", MoTa = "Trà sữa trân châu thơm ngon", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-2) },
                        new SanPham { TenSP = "Nước suối Aquafina 500ml", DonGia = 5000, SoLuong = 600, DonVi = "Chai", LoaiSP = "Đồ uống", Barcode = "8934561000088", MoTa = "Nước khoáng tinh khiết", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-6) },
                        new SanPham { TenSP = "Nước suối Lavie 500ml", DonGia = 4500, SoLuong = 550, DonVi = "Chai", LoaiSP = "Đồ uống", Barcode = "8934561000089", MoTa = "Nước suối Lavie", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-6) },
                        new SanPham { TenSP = "Sữa tươi Vinamilk 1L", DonGia = 25000, SoLuong = 200, DonVi = "Hộp", LoaiSP = "Đồ uống", Barcode = "8934561000090", MoTa = "Sữa tươi Vinamilk", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-3) },
                        new SanPham { TenSP = "Nước cam ép tươi", DonGia = 35000, SoLuong = 80, DonVi = "Lít", LoaiSP = "Đồ uống", Barcode = "8934561000091", MoTa = "Nước cam ép tươi nguyên chất", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-1) },
                        
                        // BÁNH KẸO
                        new SanPham { TenSP = "Snack Lays vị tự nhiên", DonGia = 12000, SoLuong = 200, DonVi = "Gói", LoaiSP = "Bánh kẹo", Barcode = "8934561000099", MoTa = "Snack khoai tây", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-4) },
                        new SanPham { TenSP = "Snack Pringles vị BBQ", DonGia = 18000, SoLuong = 150, DonVi = "Hộp", LoaiSP = "Bánh kẹo", Barcode = "8934561000101", MoTa = "Snack Pringles vị BBQ", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-3) },
                        new SanPham { TenSP = "Kẹo Alpenliebe vị sữa", DonGia = 18000, SoLuong = 150, DonVi = "Gói", LoaiSP = "Bánh kẹo", Barcode = "8934561000100", MoTa = "Kẹo sữa thơm ngon", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-3) },
                        new SanPham { TenSP = "Kẹo Mentos vị bạc hà", DonGia = 15000, SoLuong = 120, DonVi = "Gói", LoaiSP = "Bánh kẹo", Barcode = "8934561000102", MoTa = "Kẹo Mentos thơm mát", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-2) },
                        new SanPham { TenSP = "Bánh Oreo gói 137g", DonGia = 22000, SoLuong = 180, DonVi = "Gói", LoaiSP = "Bánh kẹo", Barcode = "8934561000111", MoTa = "Bánh quy kem", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-5) },
                        new SanPham { TenSP = "Bánh quy bơ Danisa", DonGia = 35000, SoLuong = 100, DonVi = "Hộp", LoaiSP = "Bánh kẹo", Barcode = "8934561000113", MoTa = "Bánh quy bơ thơm ngon", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-4) },
                        new SanPham { TenSP = "Bánh mì tươi", DonGia = 8000, SoLuong = 200, DonVi = "Cái", LoaiSP = "Bánh kẹo", Barcode = "8934561000114", MoTa = "Bánh mì tươi nóng", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-1) },
                        new SanPham { TenSP = "Bánh croissant", DonGia = 12000, SoLuong = 80, DonVi = "Cái", LoaiSP = "Bánh kẹo", Barcode = "8934561000115", MoTa = "Bánh croissant Pháp", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-1) },
                        
                        // GIA VỊ
                        new SanPham { TenSP = "Nước mắm Nam Ngư 500ml", DonGia = 28000, SoLuong = 100, DonVi = "Chai", LoaiSP = "Gia vị", Barcode = "8934561000122", MoTa = "Nước mắm truyền thống", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-6) },
                        new SanPham { TenSP = "Nước mắm Phú Quốc 500ml", DonGia = 35000, SoLuong = 80, DonVi = "Chai", LoaiSP = "Gia vị", Barcode = "8934561000123", MoTa = "Nước mắm Phú Quốc đặc sản", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-5) },
                        new SanPham { TenSP = "Dầu ăn Neptune 1L", DonGia = 45000, SoLuong = 90, DonVi = "Chai", LoaiSP = "Gia vị", Barcode = "8934561000133", MoTa = "Dầu ăn tinh luyện", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-5) },
                        new SanPham { TenSP = "Dầu oliu Extra Virgin", DonGia = 120000, SoLuong = 40, DonVi = "Chai", LoaiSP = "Gia vị", Barcode = "8934561000134", MoTa = "Dầu oliu nguyên chất", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-4) },
                        new SanPham { TenSP = "Tương ớt Cholimex 270g", DonGia = 15000, SoLuong = 120, DonVi = "Chai", LoaiSP = "Gia vị", Barcode = "8934561000144", MoTa = "Tương ớt đặc biệt", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-4) },
                        new SanPham { TenSP = "Tương cà Cholimex 270g", DonGia = 12000, SoLuong = 100, DonVi = "Chai", LoaiSP = "Gia vị", Barcode = "8934561000145", MoTa = "Tương cà chua", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-4) },
                        new SanPham { TenSP = "Muối i-ốt", DonGia = 8000, SoLuong = 150, DonVi = "Gói", LoaiSP = "Gia vị", Barcode = "8934561000146", MoTa = "Muối i-ốt tinh khiết", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-6) },
                        new SanPham { TenSP = "Đường trắng", DonGia = 15000, SoLuong = 200, DonVi = "Kg", LoaiSP = "Gia vị", Barcode = "8934561000147", MoTa = "Đường trắng tinh luyện", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-5) },
                        
                        // ĐỒ DÙNG GIA ĐÌNH
                        new SanPham { TenSP = "Nước rửa chén Sunlight 750g", DonGia = 32000, SoLuong = 85, DonVi = "Chai", LoaiSP = "Đồ dùng gia đình", Barcode = "8934561000155", MoTa = "Nước rửa chén diệt khuẩn", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-5) },
                        new SanPham { TenSP = "Nước rửa chén Joy 750g", DonGia = 28000, SoLuong = 90, DonVi = "Chai", LoaiSP = "Đồ dùng gia đình", Barcode = "8934561000156", MoTa = "Nước rửa chén Joy", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-4) },
                        new SanPham { TenSP = "Bột giặt OMO matic 3.5kg", DonGia = 135000, SoLuong = 60, DonVi = "Gói", LoaiSP = "Đồ dùng gia đình", Barcode = "8934561000166", MoTa = "Bột giặt cho máy giặt", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-4) },
                        new SanPham { TenSP = "Bột giặt Tide 3kg", DonGia = 120000, SoLuong = 70, DonVi = "Gói", LoaiSP = "Đồ dùng gia đình", Barcode = "8934561000167", MoTa = "Bột giặt Tide", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-4) },
                        new SanPham { TenSP = "Nước lau sàn Vim 1L", DonGia = 38000, SoLuong = 70, DonVi = "Chai", LoaiSP = "Đồ dùng gia đình", Barcode = "8934561000177", MoTa = "Nước lau sàn hương hoa", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-3) },
                        new SanPham { TenSP = "Nước lau kính Mr. Muscle", DonGia = 25000, SoLuong = 60, DonVi = "Chai", LoaiSP = "Đồ dùng gia đình", Barcode = "8934561000178", MoTa = "Nước lau kính trong suốt", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-3) },
                        new SanPham { TenSP = "Khăn giấy Kleenex", DonGia = 18000, SoLuong = 120, DonVi = "Gói", LoaiSP = "Đồ dùng gia đình", Barcode = "8934561000179", MoTa = "Khăn giấy mềm mại", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-2) },
                        new SanPham { TenSP = "Giấy vệ sinh Charmin", DonGia = 45000, SoLuong = 80, DonVi = "Cuộn", LoaiSP = "Đồ dùng gia đình", Barcode = "8934561000180", MoTa = "Giấy vệ sinh mềm mại", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-2) },
                        
                        // SẢN PHẨM CHĂM SÓC CÁ NHÂN
                        new SanPham { TenSP = "Kem đánh răng P/S 230g", DonGia = 28000, SoLuong = 110, DonVi = "Tuýp", LoaiSP = "Sản phẩm chăm sóc cá nhân", Barcode = "8934561000188", MoTa = "Kem đánh răng toàn diện", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-5) },
                        new SanPham { TenSP = "Kem đánh răng Colgate 200g", DonGia = 25000, SoLuong = 100, DonVi = "Tuýp", LoaiSP = "Sản phẩm chăm sóc cá nhân", Barcode = "8934561000189", MoTa = "Kem đánh răng Colgate", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-4) },
                        new SanPham { TenSP = "Dầu gội Clear Men 630g", DonGia = 118000, SoLuong = 65, DonVi = "Chai", LoaiSP = "Sản phẩm chăm sóc cá nhân", Barcode = "8934561000199", MoTa = "Dầu gội sạch gàu", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-4) },
                        new SanPham { TenSP = "Dầu gội Head & Shoulders 400ml", DonGia = 85000, SoLuong = 80, DonVi = "Chai", LoaiSP = "Sản phẩm chăm sóc cá nhân", Barcode = "8934561000201", MoTa = "Dầu gội trị gàu", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-3) },
                        new SanPham { TenSP = "Sữa tắm Lifebuoy 850g", DonGia = 95000, SoLuong = 75, DonVi = "Chai", LoaiSP = "Sản phẩm chăm sóc cá nhân", Barcode = "8934561000200", MoTa = "Sữa tắm diệt khuẩn", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-4) },
                        new SanPham { TenSP = "Sữa tắm Dove 400ml", DonGia = 65000, SoLuong = 90, DonVi = "Chai", LoaiSP = "Sản phẩm chăm sóc cá nhân", Barcode = "8934561000202", MoTa = "Sữa tắm Dove mềm mại", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-3) },
                        new SanPham { TenSP = "Kem dưỡng da Nivea", DonGia = 45000, SoLuong = 60, DonVi = "Hộp", LoaiSP = "Sản phẩm chăm sóc cá nhân", Barcode = "8934561000203", MoTa = "Kem dưỡng da Nivea", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-2) },
                        new SanPham { TenSP = "Son dưỡng môi Vaseline", DonGia = 25000, SoLuong = 80, DonVi = "Tuýp", LoaiSP = "Sản phẩm chăm sóc cá nhân", Barcode = "8934561000204", MoTa = "Son dưỡng môi Vaseline", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-2) },
                        
                        // ĐỒ ĐÔNG LẠNH
                        new SanPham { TenSP = "Kem Wall's Vani", DonGia = 15000, SoLuong = 100, DonVi = "Hộp", LoaiSP = "Đồ đông lạnh", Barcode = "8934561000205", MoTa = "Kem Wall's vị vani", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-1) },
                        new SanPham { TenSP = "Kem Tràng Tiền", DonGia = 12000, SoLuong = 80, DonVi = "Que", LoaiSP = "Đồ đông lạnh", Barcode = "8934561000206", MoTa = "Kem Tràng Tiền truyền thống", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-1) },
                        new SanPham { TenSP = "Chả cá Lã Vọng", DonGia = 35000, SoLuong = 50, DonVi = "Gói", LoaiSP = "Đồ đông lạnh", Barcode = "8934561000207", MoTa = "Chả cá Lã Vọng đông lạnh", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-2) },
                        new SanPham { TenSP = "Bánh bao đông lạnh", DonGia = 25000, SoLuong = 60, DonVi = "Gói", LoaiSP = "Đồ đông lạnh", Barcode = "8934561000208", MoTa = "Bánh bao đông lạnh tiện lợi", TrangThai = true, NgayTao = DateTime.Now.AddMonths(-1) }
                    );
                    db.SaveChanges();
                }

                // Seed Hóa đơn mẫu để có dữ liệu cho báo cáo
                if (!db.HoaDon.Any())
                {
                    var random = new Random();
                    var nhanVienIds = db.NhanVien.Select(x => x.MaNV).ToList();
                    var khachHangIds = db.KhachHang.Select(x => x.MaKH).ToList();
                    var sanPhamIds = db.SanPham.Select(x => x.MaSP).ToList();

                    // Tạo 50 hóa đơn mẫu trong 3 tháng qua
                    for (int i = 1; i <= 50; i++)
                    {
                        var ngayLap = DateTime.Now.AddDays(-random.Next(1, 90)); // Trong 3 tháng qua
                        var nhanVienId = nhanVienIds[random.Next(nhanVienIds.Count)];
                        var khachHangId = random.Next(10) < 7 ? khachHangIds[random.Next(khachHangIds.Count)] : (int?)null; // 70% có khách hàng

                        var hoaDon = new HoaDon
                        {
                            NgayLap = ngayLap,
                            MaNV = nhanVienId,
                            MaKH = khachHangId,
                            TongTien = 0 // Sẽ tính sau
                        };

                        db.HoaDon.Add(hoaDon);
                        db.SaveChanges(); // Lưu để có MaHD

                        // Tạo chi tiết hóa đơn (1-5 sản phẩm mỗi hóa đơn)
                        var soSanPham = random.Next(1, 6);
                        var tongTien = 0m;

                        for (int j = 0; j < soSanPham; j++)
                        {
                            var sanPhamId = sanPhamIds[random.Next(sanPhamIds.Count)];
                            var sanPham = db.SanPham.Find(sanPhamId);
                            var soLuong = random.Next(1, 6);
                            var donGiaBan = sanPham.DonGia;

                            var chiTiet = new CTHoaDon
                            {
                                MaHD = hoaDon.MaHD,
                                MaSP = sanPhamId,
                                SoLuong = soLuong,
                                DonGiaBan = donGiaBan
                            };

                            db.CTHoaDon.Add(chiTiet);
                            tongTien += soLuong * donGiaBan;
                        }

                        // Cập nhật tổng tiền
                        hoaDon.TongTien = tongTien;
                        db.SaveChanges();
                    }
                }

                Console.WriteLine("✅ Đã thêm dữ liệu mẫu thành công!");
            }
        }
    }
}

