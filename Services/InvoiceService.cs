using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SupermarketApp.Data;
using SupermarketApp.Data.Models;

namespace SupermarketApp.Services
{
    public class InvoiceService
    {
        public class InvoiceItemDto
        {
            public int MaSP { get; set; }
            public string TenSP { get; set; }
            public int SoLuong { get; set; }
            public decimal DonGiaBan { get; set; }
            public decimal ThanhTien { get { return SoLuong * DonGiaBan; } }
        }

        public async Task<int> CreateInvoiceAsync(int maNV, int? maKH, List<InvoiceItemDto> items)
        {
            if (items == null || items.Count == 0)
            {
                throw new ArgumentException("Giỏ hàng trống!");
            }

            using (var db = new SupermarketContext())
            {
                // Sử dụng Transaction để đảm bảo tính toàn vẹn dữ liệu
                using (var transaction = await db.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // 1. Kiểm tra tồn kho
                        var ids = items.Select(i => i.MaSP).ToList();
                        var sanPhams = await db.SanPham.Where(x => ids.Contains(x.MaSP)).ToListAsync();
                        
                        if (sanPhams.Count != ids.Distinct().Count())
                        {
                            throw new Exception("Có sản phẩm không tồn tại trong hệ thống!");
                        }

                        var spDict = sanPhams.ToDictionary(x => x.MaSP);

                        foreach (var item in items)
                        {
                            if (!spDict.ContainsKey(item.MaSP))
                            {
                                throw new Exception($"Sản phẩm {item.TenSP} không tồn tại!");
                            }

                            var sp = spDict[item.MaSP];
                            if (sp.SoLuong < item.SoLuong)
                            {
                                throw new Exception($"Sản phẩm '{sp.TenSP}' không đủ tồn kho!\nTồn kho hiện tại: {sp.SoLuong}, yêu cầu: {item.SoLuong}");
                            }
                        }

                        // 2. Tạo hóa đơn
                        var hd = new HoaDon
                        {
                            MaNV = maNV,
                            MaKH = maKH,
                            NgayLap = DateTime.UtcNow,
                            TongTien = 0
                        };
                        db.HoaDon.Add(hd);
                        await db.SaveChangesAsync();

                        // 3. Tạo chi tiết hóa đơn và TRỪ TỒN KHO
                        foreach (var item in items)
                        {
                            // Thêm chi tiết hóa đơn
                            db.CTHoaDon.Add(new CTHoaDon 
                            { 
                                MaHD = hd.MaHD, 
                                MaSP = item.MaSP, 
                                SoLuong = item.SoLuong, 
                                DonGiaBan = item.DonGiaBan 
                            });

                            // Trừ tồn kho
                            var sp = spDict[item.MaSP];
                            sp.SoLuong -= item.SoLuong;
                            db.SanPham.Update(sp); // Explicit update để chắc chắn
                        }
                        await db.SaveChangesAsync();

                        // 4. Cập nhật tổng tiền hóa đơn
                        hd.TongTien = items.Sum(x => x.ThanhTien);
                        db.HoaDon.Update(hd);
                        await db.SaveChangesAsync();

                        // 5. Cập nhật điểm tích lũy khách hàng (nếu có)
                        if (maKH.HasValue)
                        {
                            var kh = await db.KhachHang.FindAsync(maKH.Value);
                            if (kh != null)
                            {
                                // Tích 1 điểm cho mỗi 10,000 VNĐ
                                int diemThem = (int)(hd.TongTien / 10000m);
                                kh.DiemTichLuy += diemThem;
                                db.KhachHang.Update(kh);
                                await db.SaveChangesAsync();
                            }
                        }

                        // Commit transaction
                        await transaction.CommitAsync();
                        return hd.MaHD;
                    }
                    catch (Exception)
                    {
                        // Rollback nếu có lỗi
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }
    }
}
