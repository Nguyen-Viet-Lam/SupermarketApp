using System;
using System.Collections.Generic;

namespace SupermarketApp.Data.Models
{
    public class SanPham
    {
        public int MaSP { get; set; }
        public string TenSP { get; set; }
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }
        public string DonVi { get; set; } // Đơn vị tính (cái, hộp, kg, lít...)
        public string LoaiSP { get; set; }
        public string Barcode { get; set; }
        public string MoTa { get; set; }
        public bool TrangThai { get; set; }
        public DateTime NgayTao { get; set; }

        // Navigation Properties
        public virtual List<CTHoaDon> ChiTietHoaDons { get; set; } = new List<CTHoaDon>();
    }
}
