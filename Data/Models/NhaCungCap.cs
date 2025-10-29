using System;
using System.Collections.Generic;

namespace SupermarketApp.Data.Models
{
    public class NhaCungCap
    {
        public int MaNCC { get; set; }
        public string TenNCC { get; set; }
        public string DiaChi { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }
        public string MaSoThue { get; set; }
        public string NguoiLienHe { get; set; }
        public bool TrangThai { get; set; }
        public DateTime NgayTao { get; set; }
        

        // Navigation Properties
        public virtual List<PhieuNhap> PhieuNhaps { get; set; } = new List<PhieuNhap>();
    }
}

