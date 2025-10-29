using System;
using System.Collections.Generic;

namespace SupermarketApp.Data.Models
{
    public class PhieuNhap
    {
        public int MaPN { get; set; }
        public DateTime NgayNhap { get; set; }
        public int MaNCC { get; set; }
        public int MaNV { get; set; }
        public decimal TongTien { get; set; }
        
        public string TrangThai { get; set; } // "Chờ duyệt", "Đã duyệt", "Đã hủy"

        // Navigation Properties
        public virtual NhaCungCap NhaCungCap { get; set; }
        public virtual NhanVien NhanVien { get; set; }
        public virtual List<CTPhieuNhap> ChiTiets { get; set; } = new List<CTPhieuNhap>();
    }
}

