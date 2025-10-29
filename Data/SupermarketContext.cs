using Microsoft.EntityFrameworkCore;
using SupermarketApp.Data.Models;  

namespace SupermarketApp.Data
{
    public class SupermarketContext : DbContext
    {
        public DbSet<NhanVien> NhanVien { get; set; }
        public DbSet<KhachHang> KhachHang { get; set; }
        public DbSet<SanPham> SanPham { get; set; }
        public DbSet<HoaDon> HoaDon { get; set; }
        public DbSet<CTHoaDon> CTHoaDon { get; set; }
        public DbSet<NhaCungCap> NhaCungCap { get; set; }
        public DbSet<PhieuNhap> PhieuNhap { get; set; }
        public DbSet<CTPhieuNhap> CTPhieuNhap { get; set; }
        public DbSet<CaiDat> CaiDat { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(AppConfigHelper.GetConnectionString());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // SanPham Configuration
            modelBuilder.Entity<SanPham>(e => {
                e.ToTable("SANPHAM");
                e.HasKey(x => x.MaSP);
                e.Property(x => x.DonGia).HasColumnType("decimal(18,2)");
                e.Property(x => x.TenSP).IsRequired().HasMaxLength(200);
                e.Property(x => x.DonVi).HasMaxLength(50);
                e.Property(x => x.LoaiSP).HasMaxLength(100);
                e.Property(x => x.Barcode).HasMaxLength(50);
            });

            // HoaDon Configuration
            modelBuilder.Entity<HoaDon>(e => {
                e.ToTable("HOADON");
                e.HasKey(x => x.MaHD);
                e.Property(x => x.TongTien).HasColumnType("decimal(18,2)");
                
                // Foreign Key: HoaDon -> NhanVien
                e.HasOne(x => x.NhanVien)
                    .WithMany(x => x.HoaDons)
                    .HasForeignKey(x => x.MaNV)
                    .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key: HoaDon -> KhachHang (nullable)
                e.HasOne(x => x.KhachHang)
                    .WithMany(x => x.HoaDons)
                    .HasForeignKey(x => x.MaKH)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // CTHoaDon Configuration
            modelBuilder.Entity<CTHoaDon>(e => {
                e.ToTable("CTHOADON");
                e.HasKey(x => new { x.MaHD, x.MaSP });
                e.Property(x => x.DonGiaBan).HasColumnType("decimal(18,2)");
                
                // Foreign Key: CTHoaDon -> HoaDon
                e.HasOne(x => x.HoaDon)
                    .WithMany(x => x.ChiTiets)
                    .HasForeignKey(x => x.MaHD)
                    .OnDelete(DeleteBehavior.Cascade);

                // Foreign Key: CTHoaDon -> SanPham
                e.HasOne(x => x.SanPham)
                    .WithMany(x => x.ChiTietHoaDons)
                    .HasForeignKey(x => x.MaSP)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // KhachHang Configuration
            modelBuilder.Entity<KhachHang>(e => {
                e.ToTable("KHACHHANG");
                e.HasKey(x => x.MaKH);
                e.Property(x => x.DiemTichLuy).HasDefaultValue(0);
                e.Property(x => x.TenKH).IsRequired().HasMaxLength(200);
                e.Property(x => x.SDT).HasMaxLength(20);
                e.Property(x => x.Email).HasMaxLength(100);
                e.Property(x => x.DiaChi).HasMaxLength(500);
                e.Property(x => x.LoaiKH).HasMaxLength(50).HasDefaultValue("Vãng lai");
            });

            // NhanVien Configuration
            modelBuilder.Entity<NhanVien>(e => {
                e.ToTable("NHANVIEN");
                e.HasKey(x => x.MaNV);
                e.Property(x => x.TenNV).IsRequired().HasMaxLength(200);
                e.Property(x => x.TaiKhoan).IsRequired().HasMaxLength(50);
                e.HasIndex(x => x.TaiKhoan).IsUnique();
                e.Property(x => x.VaiTro).HasMaxLength(50);
            });

            // NhaCungCap Configuration
            modelBuilder.Entity<NhaCungCap>(e => {
                e.ToTable("NHACUNGCAP");
                e.HasKey(x => x.MaNCC);
                e.Property(x => x.TenNCC).IsRequired().HasMaxLength(200);
                e.Property(x => x.DiaChi).HasMaxLength(500);
                e.Property(x => x.SDT).HasMaxLength(20);
                e.Property(x => x.Email).HasMaxLength(100);
                e.Property(x => x.MaSoThue).HasMaxLength(50);
                e.Property(x => x.NguoiLienHe).HasMaxLength(200);
            });

            // PhieuNhap Configuration
            modelBuilder.Entity<PhieuNhap>(e => {
                e.ToTable("PHIEUNHAP");
                e.HasKey(x => x.MaPN);
                e.Property(x => x.TongTien).HasColumnType("decimal(18,2)");
                e.Property(x => x.TrangThai).HasMaxLength(50).HasDefaultValue("Chờ duyệt");
                
                // Foreign Key: PhieuNhap -> NhaCungCap
                e.HasOne(x => x.NhaCungCap)
                    .WithMany(x => x.PhieuNhaps)
                    .HasForeignKey(x => x.MaNCC)
                    .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key: PhieuNhap -> NhanVien
                e.HasOne(x => x.NhanVien)
                    .WithMany()
                    .HasForeignKey(x => x.MaNV)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // CTPhieuNhap Configuration
            modelBuilder.Entity<CTPhieuNhap>(e => {
                e.ToTable("CTPHIEUNHAP");
                e.HasKey(x => new { x.MaPN, x.MaSP });
                e.Property(x => x.DonGiaNhap).HasColumnType("decimal(18,2)");
                
                // Foreign Key: CTPhieuNhap -> PhieuNhap
                e.HasOne(x => x.PhieuNhap)
                    .WithMany(x => x.ChiTiets)
                    .HasForeignKey(x => x.MaPN)
                    .OnDelete(DeleteBehavior.Cascade);

                // Foreign Key: CTPhieuNhap -> SanPham
                e.HasOne(x => x.SanPham)
                    .WithMany()
                    .HasForeignKey(x => x.MaSP)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // CaiDat Configuration
            modelBuilder.Entity<CaiDat>(e => {
                e.ToTable("CAIDAT");
                e.HasKey(x => x.MaCaiDat);
                e.Property(x => x.TenCaiDat).IsRequired().HasMaxLength(100);
                e.HasIndex(x => x.TenCaiDat).IsUnique();
                e.Property(x => x.MoTa).HasMaxLength(500);
                e.Property(x => x.NguoiCapNhat).HasMaxLength(100);
            });
        }
    }
}
