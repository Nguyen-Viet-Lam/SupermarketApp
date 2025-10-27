using System;
using System.Collections.Generic;

namespace SupermarketApp.Data.Models
{
    public class HoaDon
    {
        public int MaHD { get; set; }
        public DateTime NgayLap { get; set; }
        public int MaNV { get; set; }
        public int? MaKH { get; set; }
        public decimal TongTien { get; set; }
        public string GhiChu { get; set; }

        // Navigation Properties
        public virtual NhanVien NhanVien { get; set; }
        public virtual KhachHang KhachHang { get; set; }
        public virtual List<CTHoaDon> ChiTiets { get; set; } = new List<CTHoaDon>();
    }
}
