using System;
using System.Linq;
using System.Security.Cryptography;
using SupermarketApp.Data.Models;

namespace SupermarketApp.Data
{
    public static class DataSeederUpdate
    {
        /// <summary>
        /// Seed default admin account with secure password (Salt + PBKDF2)
        /// Call this method on app startup to ensure admin account exists
        /// </summary>
        public static void SeedDefaultAdmin(SupermarketContext db)
        {
            try
            {
                // Check if admin exists
                var adminExists = db.NhanVien.Any(x => x.TaiKhoan == "admin");
                
                if (!adminExists)
                {
                    Console.WriteLine("Creating default admin account...");
                    
                    // Create password hash with salt
                    string defaultPassword = "admin123";
                    byte[] salt = new byte[32];
                    using (var rng = RandomNumberGenerator.Create())
                    {
                        rng.GetBytes(salt);
                    }
                    
                    byte[] hash;
                    using (var pbkdf2 = new Rfc2898DeriveBytes(defaultPassword, salt, 10000, HashAlgorithmName.SHA256))
                    {
                        hash = pbkdf2.GetBytes(32);
                    }
                    
                    // Create admin account
                    var admin = new NhanVien
                    {
                        TenNV = "Administrator",
                        TaiKhoan = "admin",
                        MatKhauHash = hash,
                        Salt = salt,
                        VaiTro = "Admin",
                        TrangThai = true,
                        NgayTao = DateTime.Now
                    };
                    
                    db.NhanVien.Add(admin);
                    db.SaveChanges();
                    
                    Console.WriteLine("✓ Default admin created successfully!");
                    Console.WriteLine("  Username: admin");
                    Console.WriteLine("  Password: admin123");
                }
                else
                {
                    // Check if admin has valid salt
                    var admin = db.NhanVien.FirstOrDefault(x => x.TaiKhoan == "admin");
                    if (admin != null && (admin.Salt == null || admin.Salt.Length < 8))
                    {
                        Console.WriteLine("Updating admin password with valid Salt...");
                        
                            // Update admin password
                        string defaultPassword = "admin123";
                        byte[] salt = new byte[32];
                        using (var rng = RandomNumberGenerator.Create())
                        {
                            rng.GetBytes(salt);
                        }
                        
                        byte[] hash;
                        using (var pbkdf2 = new Rfc2898DeriveBytes(defaultPassword, salt, 10000, HashAlgorithmName.SHA256))
                        {
                            hash = pbkdf2.GetBytes(32);
                        }
                        
                        admin.MatKhauHash = hash;
                        admin.Salt = salt;
                        db.SaveChanges();
                        
                        Console.WriteLine("✓ Admin password updated!");
                        Console.WriteLine("  Username: admin");
                        Console.WriteLine("  Password: admin123");
                        Console.WriteLine($"  Salt Length: {salt.Length} bytes");
                    }
                    else if (admin != null)
                    {
                        Console.WriteLine($"Admin exists with Salt length: {admin.Salt?.Length ?? 0} bytes");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding admin: {ex.Message}");
            }
        }
    }
}
