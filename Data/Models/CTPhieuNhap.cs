namespace SupermarketApp.Data.Models
{
    public class CTPhieuNhap
    {
        public int MaPN { get; set; }
        public int MaSP { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGiaNhap { get; set; }

        public decimal ThanhTien => SoLuong * DonGiaNhap;

        // Navigation Properties
        public virtual PhieuNhap PhieuNhap { get; set; }
        public virtual SanPham SanPham { get; set; }
    }
}

