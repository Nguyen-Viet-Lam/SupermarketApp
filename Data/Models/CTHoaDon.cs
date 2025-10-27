namespace SupermarketApp.Data.Models
{
    public class CTHoaDon
    {
        public int MaHD { get; set; }
        public int MaSP { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGiaBan { get; set; }

        public decimal ThanhTien => SoLuong * DonGiaBan;

        // Navigation Properties
        public virtual HoaDon HoaDon { get; set; }
        public virtual SanPham SanPham { get; set; }
    }
}
