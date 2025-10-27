using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SupermarketApp.Data;
using SupermarketApp.Data.Models;

namespace SupermarketApp.Services
{
    public class SettingsService
    {
        public async Task<string> GetSettingAsync(string key)
        {
            using (var db = new SupermarketContext())
            {
                var setting = await db.CaiDat.FirstOrDefaultAsync(x => x.TenCaiDat == key);
                return setting?.GiaTri;
            }
        }

        public async Task<bool> SaveSettingAsync(string key, string value, string updatedBy = "System")
        {
            try
            {
                using (var db = new SupermarketContext())
                {
                    var setting = await db.CaiDat.FirstOrDefaultAsync(x => x.TenCaiDat == key);
                    
                    if (setting != null)
                    {
                        setting.GiaTri = value;
                        setting.NgayCapNhat = DateTime.Now;
                        setting.NguoiCapNhat = updatedBy;
                    }
                    else
                    {
                        setting = new CaiDat
                        {
                            TenCaiDat = key,
                            GiaTri = value,
                            NgayCapNhat = DateTime.Now,
                            NguoiCapNhat = updatedBy
                        };
                        db.CaiDat.Add(setting);
                    }
                    
                    await db.SaveChangesAsync();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> TestConnectionAsync(string connectionString)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GetServerInfoAsync(string connectionString)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT @@VERSION";
                        var version = await cmd.ExecuteScalarAsync();
                        return version?.ToString();
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> BackupDatabaseAsync(string backupPath)
        {
            try
            {
                using (var db = new SupermarketContext())
                {
                    var dbName = db.Database.GetDbConnection().Database;
                    var sql = $"BACKUP DATABASE [{dbName}] TO DISK = N'{backupPath}' WITH NOFORMAT, NOINIT, NAME = N'{dbName} Backup', SKIP, NOREWIND, NOUNLOAD, STATS = 10";
                    await db.Database.ExecuteSqlRawAsync(sql);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RestoreDatabaseAsync(string backupPath)
        {
            try
            {
                using (var db = new SupermarketContext())
                {
                    var dbName = db.Database.GetDbConnection().Database;
                    var sql = $"RESTORE DATABASE [{dbName}] FROM DISK = N'{backupPath}' WITH REPLACE, RECOVERY";
                    await db.Database.ExecuteSqlRawAsync(sql);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}














