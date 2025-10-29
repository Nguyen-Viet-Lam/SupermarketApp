using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SupermarketApp.Data;
using SupermarketApp.Data.Models;

namespace SupermarketApp.Services
{
    public class AuthService
    {
        /// <summary>
        /// Hash password with Salt using PBKDF2 (Password-Based Key Derivation Function 2)
        /// This is more secure than simple SHA256
        /// </summary>
        /// <param name="password">Plain text password</param>
        /// <returns>Tuple of (hash, salt)</returns>
        public (byte[] hash, byte[] salt) HashPassword(string password)
        {
            // Generate random salt (32 bytes)
            byte[] salt = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Hash password with salt using PBKDF2
            // 10000 iterations for good security vs performance balance
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(32); // 32 bytes = 256 bits
                return (hash, salt);
            }
        }

        /// <summary>
        /// Validate user credentials with Salt-based hashing
        /// </summary>
        public async Task<bool> ValidateAsync(string user, string pass)
        {
            using (var db = new SupermarketContext())
            {
                var nv = await db.NhanVien.FirstOrDefaultAsync(x => x.TaiKhoan == user && x.TrangThai);
                if (nv == null || nv.MatKhauHash == null || nv.Salt == null) 
                    return false;

                // Hash input password with stored salt
                using (var pbkdf2 = new Rfc2898DeriveBytes(pass, nv.Salt, 10000, HashAlgorithmName.SHA256))
                {
                    byte[] hash = pbkdf2.GetBytes(32);
                    return nv.MatKhauHash.SequenceEqual(hash);
                }
            }
        }

        /// <summary>
        /// Register new employee with secure password hashing
        /// </summary>
        public async Task<bool> RegisterAsync(string name, string user, string pass)
        {
            using (var db = new SupermarketContext())
            {
                var exists = await db.NhanVien.AnyAsync(x => x.TaiKhoan == user);
                if (exists) return false;

                // Generate hash and salt
                var (hash, salt) = HashPassword(pass);

                var nv = new NhanVien
                {
                    TenNV = name?.Trim(),
                    TaiKhoan = user?.Trim(),
                    MatKhauHash = hash,
                    Salt = salt,
                    VaiTro = "Nhân viên",
                    TrangThai = true,
                    NgayTao = DateTime.Now
                };
                db.NhanVien.Add(nv);
                await db.SaveChangesAsync();
                return true;
            }
        }
    }
}
