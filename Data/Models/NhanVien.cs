using System;
using System.Collections.Generic;

namespace SupermarketApp.Data.Models
{
    public class NhanVien
    {
        public int MaNV { get; set; }
        public string TenNV { get; set; }
        public string TaiKhoan { get; set; }
        public byte[] MatKhauHash { get; set; }
        public byte[] Salt { get; set; }
        public string VaiTro { get; set; }
        public bool TrangThai { get; set; }
        public DateTime NgayTao { get; set; }

        // Navigation Properties
        public virtual List<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
    }
}
