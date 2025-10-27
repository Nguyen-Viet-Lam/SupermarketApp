using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SupermarketApp.Data;
using SupermarketApp.Data.Models;

namespace SupermarketApp.Services
{
    public class ProductService
    {
        public async Task<List<SanPham>> GetAllAsync()
        {
            using (var db = new SupermarketContext())
            {
                return await db.SanPham.OrderBy(x => x.TenSP).ToListAsync();
            }
        }

        public async Task<SanPham> AddAsync(SanPham sp)
        {
            using (var db = new SupermarketContext())
            {
                db.SanPham.Add(sp);
                await db.SaveChangesAsync();
                return sp;
            }
        }

        public async Task<bool> UpdateAsync(SanPham sp)
        {
            using (var db = new SupermarketContext())
            {
                db.SanPham.Update(sp);
                return await db.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> DeleteAsync(int maSP)
        {
            using (var db = new SupermarketContext())
            {
                var sp = await db.SanPham.FindAsync(maSP);
                if (sp == null) return false;

                // Nếu sản phẩm đã xuất hiện trong hóa đơn, chuyển sang "soft delete" (vô hiệu hóa) thay vì xóa cứng
                var existsInInvoice = await db.CTHoaDon.AsNoTracking().AnyAsync(x => x.MaSP == maSP);
                if (existsInInvoice)
                {
                    if (sp.TrangThai)
                    {
                        sp.TrangThai = false;
                        db.SanPham.Update(sp);
                        await db.SaveChangesAsync();
                    }
                    return true;
                }

                // Chưa từng bán: cho phép xóa cứng
                db.SanPham.Remove(sp);
                return await db.SaveChangesAsync() > 0;
            }
        }
    }
}
