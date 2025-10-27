using System;
using System.Collections.Generic;

namespace SupermarketApp.Data.Models
{
    public class KhachHang
    {
        public int MaKH { get; set; }
        public string TenKH { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }
        public string DiaChi { get; set; }
        public string LoaiKhachHang { get; set; } // Phân loại: "Khách vãng lai", "Khách thân quen", ...
        public int DiemTichLuy { get; set; }
        public DateTime NgayTao { get; set; }

        // Navigation Properties
        public virtual List<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
    }
}
