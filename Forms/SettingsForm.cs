using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sunny.UI;
using SupermarketApp.Utils;
using SupermarketApp.Data;
using SupermarketApp.Data.Models;
using SupermarketApp.Services;

namespace SupermarketApp.Forms
{
    public partial class SettingsForm : Form
    {
        private readonly SettingsService _settingsService = new SettingsService();
        private string _currentUsername;
        private UIPanel pnlTop;
        private UILabel lblTitle;
        private UITabControl tabControl;
        private TabPage tabGeneral;
        private TabPage tabDatabase;
        private TabPage tabSecurity;
        private TabPage tabBackup;
        
        // General Settings
        private UILabel lblStoreName;
        private UITextBox txtStoreName;
        private UILabel lblStoreAddress;
        private UITextBox txtStoreAddress;
        private UILabel lblStorePhone;
        private UITextBox txtStorePhone;
        private UIButton btnSaveGeneral;
        
        // Database Settings
        private UILabel lblConnectionString;
        private UITextBox txtConnectionString;
        private UIButton btnTestConnection;
        private UIButton btnSaveDatabase;
        
        // Security Settings
        private UILabel lblCurrentPassword;
        private UITextBox txtCurrentPassword;
        private UILabel lblNewPassword;
        private UITextBox txtNewPassword;
        private UILabel lblConfirmPassword;
        private UITextBox txtConfirmPassword;
        private UIButton btnChangePassword;
        
        // Backup Settings
        private UILabel lblBackupPath;
        private UITextBox txtBackupPath;
        private UIButton btnBrowseBackup;
        private UIButton btnBackupNow;
        private UIButton btnRestore;
        private UIButton btnBrowseRestore;

        public SettingsForm(string username = "admin")
        {
            _currentUsername = username;
            InitializeComponent();
            this.Load += async (s, e) => await LoadSettingsAsync();
        }

        private async Task LoadSettingsAsync()
        {
            try
            {
                using (var db = new SupermarketContext())
                {
                    // Load general settings from database
                    var storeName = await _settingsService.GetSettingAsync("StoreName");
                    var storeAddress = await _settingsService.GetSettingAsync("StoreAddress");
                    var storePhone = await _settingsService.GetSettingAsync("StorePhone");
                    
                    txtStoreName.Text = storeName ?? "Siêu thị ABC";
                    txtStoreAddress.Text = storeAddress ?? "123 Đường ABC, Quận 1, TP.HCM";
                    txtStorePhone.Text = storePhone ?? "028-1234-5678";
                }
                
                // Load connection string from config
                txtConnectionString.Text = AppConfigHelper.GetConnectionString() ?? "Server=localhost;Database=SupermarketDB;Trusted_Connection=true;TrustServerCertificate=True";
                
                // Load backup path
                var backupPath = await _settingsService.GetSettingAsync("BackupPath");
                txtBackupPath.Text = backupPath ?? Path.Combine(Application.StartupPath, "Backups");
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("Lỗi tải cài đặt: " + ex.Message);
            }
        }

        private async void BtnSaveGeneral_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtStoreName.Text))
                {
                    MessageHelper.ShowWarning("Vui lòng nhập tên cửa hàng!");
                    txtStoreName.Focus();
                    return;
                }

                // Save settings to database
                await _settingsService.SaveSettingAsync("StoreName", txtStoreName.Text, _currentUsername);
                await _settingsService.SaveSettingAsync("StoreAddress", txtStoreAddress.Text, _currentUsername);
                await _settingsService.SaveSettingAsync("StorePhone", txtStorePhone.Text, _currentUsername);
                
                MessageHelper.ShowSuccess("Đã lưu cài đặt chung thành công!");
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("Lỗi: " + ex.Message);
            }
        }

        private async void BtnTestConnection_Click(object sender, EventArgs e)
        {
            try
            {
                btnTestConnection.Enabled = false;
                btnTestConnection.Text = "Đang test...";
                
                var connString = txtConnectionString.Text;
                if (string.IsNullOrWhiteSpace(connString))
                {
                    connString = AppConfigHelper.GetConnectionString();
                }
                
                var isValid = await _settingsService.TestConnectionAsync(connString);
                
                if (isValid)
                {
                    var serverInfo = await _settingsService.GetServerInfoAsync(connString);
                    MessageHelper.ShowSuccess($"Kết nối database thành công!\n\n{(serverInfo != null ? "Server: " + serverInfo.Split('\n')[0] : "")}");
                }
                else
                {
                    MessageHelper.ShowError("Không thể kết nối đến database!\nVui lòng kiểm tra connection string.");
                }
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("Lỗi kết nối: " + ex.Message);
            }
            finally
            {
                btnTestConnection.Enabled = true;
                btnTestConnection.Text = "Test Connection";
            }
        }

        private async void BtnSaveDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtConnectionString.Text))
                {
                    MessageHelper.ShowWarning("Vui lòng nhập connection string!");
                    return;
                }
                
                // Test connection first
                var isValid = await _settingsService.TestConnectionAsync(txtConnectionString.Text);
                if (!isValid)
                {
                    MessageHelper.ShowError("Connection string không hợp lệ! Vui lòng test connection trước.");
                    return;
                }
                
                // Update App.config (requires restart)
                AppConfigHelper.UpdateConnectionString("SupermarketDB", txtConnectionString.Text);
                
                MessageHelper.ShowSuccess("Đã lưu cài đặt database!\n\nVui lòng khởi động lại ứng dụng để áp dụng thay đổi.");
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("Lỗi: " + ex.Message);
            }
        }

        private async void BtnChangePassword_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtCurrentPassword.Text))
                {
                    MessageHelper.ShowWarning("Vui lòng nhập mật khẩu hiện tại!");
                    txtCurrentPassword.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtNewPassword.Text))
                {
                    MessageHelper.ShowWarning("Vui lòng nhập mật khẩu mới!");
                    txtNewPassword.Focus();
                    return;
                }

                if (txtNewPassword.Text.Length < 6)
                {
                    MessageHelper.ShowWarning("Mật khẩu mới phải có ít nhất 6 ký tự!");
                    txtNewPassword.Focus();
                    return;
                }

                if (txtNewPassword.Text != txtConfirmPassword.Text)
                {
                    MessageHelper.ShowWarning("Mật khẩu xác nhận không khớp!");
                    txtConfirmPassword.Focus();
                    return;
                }

                // Change password logic
                var authService = new AuthService();
                
                // Validate current password
                var isValid = await authService.ValidateAsync(_currentUsername, txtCurrentPassword.Text);
                if (!isValid)
                {
                    MessageHelper.ShowError("Mật khẩu hiện tại không đúng!");
                    txtCurrentPassword.Focus();
                    return;
                }
                
                // Check new password != current
                if (txtCurrentPassword.Text == txtNewPassword.Text)
                {
                    MessageHelper.ShowWarning("Mật khẩu mới phải khác mật khẩu hiện tại!");
                    txtNewPassword.Focus();
                    return;
                }
                
                // Update password
                using (var db = new SupermarketContext())
                {
                    var user = db.NhanVien.FirstOrDefault(x => x.TaiKhoan == _currentUsername);
                    if (user != null)
                    {
                        var (hash, salt) = authService.HashPassword(txtNewPassword.Text);
                        user.MatKhauHash = hash;
                        user.Salt = salt;
                        await db.SaveChangesAsync();
                        
                        MessageHelper.ShowSuccess("Đã đổi mật khẩu thành công!\n\nVui lòng đăng nhập lại.");
                ClearPasswordFields();
                        
                        // Close settings và logout
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("Lỗi: " + ex.Message);
            }
        }

        private void BtnBrowseBackup_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Chọn thư mục lưu backup";
                folderDialog.SelectedPath = txtBackupPath.Text;
                
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtBackupPath.Text = folderDialog.SelectedPath;
                }
            }
        }

        private async void BtnBackupNow_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtBackupPath.Text))
                {
                    MessageHelper.ShowWarning("Vui lòng chọn thư mục backup!");
                    return;
                }

                if (!Directory.Exists(txtBackupPath.Text))
                {
                    Directory.CreateDirectory(txtBackupPath.Text);
                }

                string backupFile = Path.Combine(txtBackupPath.Text, $"SupermarketDB_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak");
                
                btnBackupNow.Enabled = false;
                btnBackupNow.Text = "Đang backup...";
                
                // Backup database
                var success = await _settingsService.BackupDatabaseAsync(backupFile);
                
                if (success)
                {
                    // Save backup path setting
                    await _settingsService.SaveSettingAsync("BackupPath", txtBackupPath.Text, _currentUsername);
                    MessageHelper.ShowSuccess($"Đã tạo backup thành công!\n\nFile: {Path.GetFileName(backupFile)}\nThư mục: {txtBackupPath.Text}");
                }
                else
                {
                    MessageHelper.ShowError("Backup thất bại! Vui lòng kiểm tra quyền truy cập.");
                }
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("Lỗi backup: " + ex.Message);
            }
            finally
            {
                btnBackupNow.Enabled = true;
                btnBackupNow.Text = "Backup Now";
            }
        }

        private async void BtnBrowseRestore_Click(object sender, EventArgs e)
        {
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "Backup files (*.bak)|*.bak|All files (*.*)|*.*";
                openDialog.Title = "Chọn file backup để restore";
                openDialog.InitialDirectory = txtBackupPath.Text;
                
                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    if (MessageHelper.ShowAsk("⚠️ CẢNH BÁO ⚠️\n\nBạn có chắc chắn muốn restore database?\nTất cả dữ liệu hiện tại sẽ bị mất!\n\nThao tác này KHÔNG THỂ HOÀN TÁC!"))
                    {
                        try
                        {
                            btnRestore.Enabled = false;
                            btnBrowseRestore.Enabled = false;
                            btnBrowseRestore.Text = "Đang restore...";
                            
                            // Restore database
                            var success = await _settingsService.RestoreDatabaseAsync(openDialog.FileName);
                            
                            if (success)
                            {
                                MessageHelper.ShowSuccess("Đã restore database thành công!\n\nỨng dụng sẽ khởi động lại.");
                                Application.Restart();
                            }
                            else
                            {
                                MessageHelper.ShowError("Restore thất bại! Vui lòng kiểm tra file backup.");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageHelper.ShowError("Lỗi restore: " + ex.Message);
                        }
                        finally
                        {
                            btnRestore.Enabled = true;
                            btnBrowseRestore.Enabled = true;
                            btnBrowseRestore.Text = "Browse & Restore";
                        }
                    }
                }
            }
        }

        private void ClearPasswordFields()
        {
            txtCurrentPassword.Clear();
            txtNewPassword.Clear();
            txtConfirmPassword.Clear();
        }

        private void InitializeComponent()
        {
            this.pnlTop = new Sunny.UI.UIPanel();
            this.lblTitle = new Sunny.UI.UILabel();
            this.tabControl = new Sunny.UI.UITabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.lblStoreName = new Sunny.UI.UILabel();
            this.txtStoreName = new Sunny.UI.UITextBox();
            this.lblStoreAddress = new Sunny.UI.UILabel();
            this.txtStoreAddress = new Sunny.UI.UITextBox();
            this.lblStorePhone = new Sunny.UI.UILabel();
            this.txtStorePhone = new Sunny.UI.UITextBox();
            this.btnSaveGeneral = new Sunny.UI.UIButton();
            this.tabDatabase = new System.Windows.Forms.TabPage();
            this.lblConnectionString = new Sunny.UI.UILabel();
            this.txtConnectionString = new Sunny.UI.UITextBox();
            this.btnTestConnection = new Sunny.UI.UIButton();
            this.btnSaveDatabase = new Sunny.UI.UIButton();
            this.tabSecurity = new System.Windows.Forms.TabPage();
            this.lblCurrentPassword = new Sunny.UI.UILabel();
            this.txtCurrentPassword = new Sunny.UI.UITextBox();
            this.lblNewPassword = new Sunny.UI.UILabel();
            this.txtNewPassword = new Sunny.UI.UITextBox();
            this.lblConfirmPassword = new Sunny.UI.UILabel();
            this.txtConfirmPassword = new Sunny.UI.UITextBox();
            this.btnChangePassword = new Sunny.UI.UIButton();
            this.tabBackup = new System.Windows.Forms.TabPage();
            this.lblBackupPath = new Sunny.UI.UILabel();
            this.txtBackupPath = new Sunny.UI.UITextBox();
            this.btnBrowseBackup = new Sunny.UI.UIButton();
            this.btnBackupNow = new Sunny.UI.UIButton();
            this.btnRestore = new Sunny.UI.UIButton();
            this.btnBrowseRestore = new Sunny.UI.UIButton();
            this.pnlTop.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.tabDatabase.SuspendLayout();
            this.tabSecurity.SuspendLayout();
            this.tabBackup.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.lblTitle);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.FillColor = System.Drawing.Color.White;
            this.pnlTop.FillColor2 = System.Drawing.Color.White;
            this.pnlTop.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlTop.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Padding = new System.Windows.Forms.Padding(20);
            this.pnlTop.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.pnlTop.Size = new System.Drawing.Size(770, 80);
            this.pnlTop.TabIndex = 0;
            this.pnlTop.Text = null;
            this.pnlTop.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.lblTitle.Location = new System.Drawing.Point(3, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(400, 40);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "⚙️ CÀI ĐẶT HỆ THỐNG";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabGeneral);
            this.tabControl.Controls.Add(this.tabDatabase);
            this.tabControl.Controls.Add(this.tabSecurity);
            this.tabControl.Controls.Add(this.tabBackup);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControl.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.tabControl.ItemSize = new System.Drawing.Size(120, 40);
            this.tabControl.Location = new System.Drawing.Point(0, 80);
            this.tabControl.MainPage = "";
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(770, 353);
            this.tabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl.TabIndex = 1;
            this.tabControl.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // tabGeneral
            // 
            this.tabGeneral.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(249)))), ((int)(((byte)(255)))));
            this.tabGeneral.Controls.Add(this.lblStoreName);
            this.tabGeneral.Controls.Add(this.txtStoreName);
            this.tabGeneral.Controls.Add(this.lblStoreAddress);
            this.tabGeneral.Controls.Add(this.txtStoreAddress);
            this.tabGeneral.Controls.Add(this.lblStorePhone);
            this.tabGeneral.Controls.Add(this.txtStorePhone);
            this.tabGeneral.Controls.Add(this.btnSaveGeneral);
            this.tabGeneral.Location = new System.Drawing.Point(0, 40);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Size = new System.Drawing.Size(669, 313);
            this.tabGeneral.TabIndex = 0;
            this.tabGeneral.Text = "Cài đặt chung";
            // 
            // lblStoreName
            // 
            this.lblStoreName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblStoreName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblStoreName.Location = new System.Drawing.Point(30, 30);
            this.lblStoreName.Name = "lblStoreName";
            this.lblStoreName.Size = new System.Drawing.Size(150, 30);
            this.lblStoreName.TabIndex = 0;
            this.lblStoreName.Text = "Tên cửa hàng:";
            this.lblStoreName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtStoreName
            // 
            this.txtStoreName.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtStoreName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtStoreName.Location = new System.Drawing.Point(200, 30);
            this.txtStoreName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtStoreName.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtStoreName.Name = "txtStoreName";
            this.txtStoreName.Padding = new System.Windows.Forms.Padding(5);
            this.txtStoreName.ShowText = false;
            this.txtStoreName.Size = new System.Drawing.Size(400, 35);
            this.txtStoreName.TabIndex = 1;
            this.txtStoreName.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtStoreName.Watermark = "";
            // 
            // lblStoreAddress
            // 
            this.lblStoreAddress.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblStoreAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblStoreAddress.Location = new System.Drawing.Point(30, 80);
            this.lblStoreAddress.Name = "lblStoreAddress";
            this.lblStoreAddress.Size = new System.Drawing.Size(150, 30);
            this.lblStoreAddress.TabIndex = 2;
            this.lblStoreAddress.Text = "Địa chỉ:";
            this.lblStoreAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtStoreAddress
            // 
            this.txtStoreAddress.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtStoreAddress.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtStoreAddress.Location = new System.Drawing.Point(200, 80);
            this.txtStoreAddress.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtStoreAddress.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtStoreAddress.Name = "txtStoreAddress";
            this.txtStoreAddress.Padding = new System.Windows.Forms.Padding(5);
            this.txtStoreAddress.ShowText = false;
            this.txtStoreAddress.Size = new System.Drawing.Size(400, 35);
            this.txtStoreAddress.TabIndex = 3;
            this.txtStoreAddress.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtStoreAddress.Watermark = "";
            // 
            // lblStorePhone
            // 
            this.lblStorePhone.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblStorePhone.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblStorePhone.Location = new System.Drawing.Point(30, 130);
            this.lblStorePhone.Name = "lblStorePhone";
            this.lblStorePhone.Size = new System.Drawing.Size(150, 30);
            this.lblStorePhone.TabIndex = 4;
            this.lblStorePhone.Text = "Số điện thoại:";
            this.lblStorePhone.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtStorePhone
            // 
            this.txtStorePhone.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtStorePhone.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtStorePhone.Location = new System.Drawing.Point(200, 130);
            this.txtStorePhone.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtStorePhone.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtStorePhone.Name = "txtStorePhone";
            this.txtStorePhone.Padding = new System.Windows.Forms.Padding(5);
            this.txtStorePhone.ShowText = false;
            this.txtStorePhone.Size = new System.Drawing.Size(400, 35);
            this.txtStorePhone.TabIndex = 5;
            this.txtStorePhone.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtStorePhone.Watermark = "";
            // 
            // btnSaveGeneral
            // 
            this.btnSaveGeneral.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSaveGeneral.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnSaveGeneral.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnSaveGeneral.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnSaveGeneral.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnSaveGeneral.Location = new System.Drawing.Point(200, 200);
            this.btnSaveGeneral.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnSaveGeneral.Name = "btnSaveGeneral";
            this.btnSaveGeneral.Size = new System.Drawing.Size(150, 40);
            this.btnSaveGeneral.TabIndex = 6;
            this.btnSaveGeneral.Text = "💾 Lưu";
            this.btnSaveGeneral.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnSaveGeneral.Click += new System.EventHandler(this.BtnSaveGeneral_Click);
            // 
            // tabDatabase
            // 
            this.tabDatabase.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(249)))), ((int)(((byte)(255)))));
            this.tabDatabase.Controls.Add(this.lblConnectionString);
            this.tabDatabase.Controls.Add(this.txtConnectionString);
            this.tabDatabase.Controls.Add(this.btnTestConnection);
            this.tabDatabase.Controls.Add(this.btnSaveDatabase);
            this.tabDatabase.Location = new System.Drawing.Point(0, 40);
            this.tabDatabase.Name = "tabDatabase";
            this.tabDatabase.Size = new System.Drawing.Size(669, 313);
            this.tabDatabase.TabIndex = 1;
            this.tabDatabase.Text = "Database";
            // 
            // lblConnectionString
            // 
            this.lblConnectionString.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblConnectionString.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblConnectionString.Location = new System.Drawing.Point(30, 30);
            this.lblConnectionString.Name = "lblConnectionString";
            this.lblConnectionString.Size = new System.Drawing.Size(150, 30);
            this.lblConnectionString.TabIndex = 0;
            this.lblConnectionString.Text = "Connection String:";
            this.lblConnectionString.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtConnectionString
            // 
            this.txtConnectionString.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtConnectionString.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.txtConnectionString.Location = new System.Drawing.Point(200, 30);
            this.txtConnectionString.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtConnectionString.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtConnectionString.Multiline = true;
            this.txtConnectionString.Name = "txtConnectionString";
            this.txtConnectionString.Padding = new System.Windows.Forms.Padding(5);
            this.txtConnectionString.ShowText = false;
            this.txtConnectionString.Size = new System.Drawing.Size(600, 100);
            this.txtConnectionString.TabIndex = 1;
            this.txtConnectionString.TextAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.txtConnectionString.Watermark = "";
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTestConnection.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.btnTestConnection.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            this.btnTestConnection.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            this.btnTestConnection.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.btnTestConnection.Location = new System.Drawing.Point(200, 150);
            this.btnTestConnection.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(150, 35);
            this.btnTestConnection.TabIndex = 2;
            this.btnTestConnection.Text = "🔍 Test";
            this.btnTestConnection.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnTestConnection.Click += new System.EventHandler(this.BtnTestConnection_Click);
            // 
            // btnSaveDatabase
            // 
            this.btnSaveDatabase.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSaveDatabase.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnSaveDatabase.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnSaveDatabase.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnSaveDatabase.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.btnSaveDatabase.Location = new System.Drawing.Point(370, 150);
            this.btnSaveDatabase.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnSaveDatabase.Name = "btnSaveDatabase";
            this.btnSaveDatabase.Size = new System.Drawing.Size(150, 35);
            this.btnSaveDatabase.TabIndex = 3;
            this.btnSaveDatabase.Text = "💾 Lưu";
            this.btnSaveDatabase.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnSaveDatabase.Click += new System.EventHandler(this.BtnSaveDatabase_Click);
            // 
            // tabSecurity
            // 
            this.tabSecurity.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(249)))), ((int)(((byte)(255)))));
            this.tabSecurity.Controls.Add(this.lblCurrentPassword);
            this.tabSecurity.Controls.Add(this.txtCurrentPassword);
            this.tabSecurity.Controls.Add(this.lblNewPassword);
            this.tabSecurity.Controls.Add(this.txtNewPassword);
            this.tabSecurity.Controls.Add(this.lblConfirmPassword);
            this.tabSecurity.Controls.Add(this.txtConfirmPassword);
            this.tabSecurity.Controls.Add(this.btnChangePassword);
            this.tabSecurity.Location = new System.Drawing.Point(0, 40);
            this.tabSecurity.Name = "tabSecurity";
            this.tabSecurity.Size = new System.Drawing.Size(669, 313);
            this.tabSecurity.TabIndex = 2;
            this.tabSecurity.Text = "Bảo mật";
            // 
            // lblCurrentPassword
            // 
            this.lblCurrentPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblCurrentPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblCurrentPassword.Location = new System.Drawing.Point(30, 30);
            this.lblCurrentPassword.Name = "lblCurrentPassword";
            this.lblCurrentPassword.Size = new System.Drawing.Size(150, 30);
            this.lblCurrentPassword.TabIndex = 0;
            this.lblCurrentPassword.Text = "Mật khẩu hiện tại:";
            this.lblCurrentPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtCurrentPassword
            // 
            this.txtCurrentPassword.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtCurrentPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtCurrentPassword.Location = new System.Drawing.Point(200, 30);
            this.txtCurrentPassword.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtCurrentPassword.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtCurrentPassword.Name = "txtCurrentPassword";
            this.txtCurrentPassword.Padding = new System.Windows.Forms.Padding(5);
            this.txtCurrentPassword.PasswordChar = '●';
            this.txtCurrentPassword.ShowText = false;
            this.txtCurrentPassword.Size = new System.Drawing.Size(300, 35);
            this.txtCurrentPassword.TabIndex = 1;
            this.txtCurrentPassword.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtCurrentPassword.Watermark = "";
            // 
            // lblNewPassword
            // 
            this.lblNewPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblNewPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblNewPassword.Location = new System.Drawing.Point(30, 80);
            this.lblNewPassword.Name = "lblNewPassword";
            this.lblNewPassword.Size = new System.Drawing.Size(150, 30);
            this.lblNewPassword.TabIndex = 2;
            this.lblNewPassword.Text = "Mật khẩu mới:";
            this.lblNewPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtNewPassword
            // 
            this.txtNewPassword.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtNewPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtNewPassword.Location = new System.Drawing.Point(200, 80);
            this.txtNewPassword.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtNewPassword.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtNewPassword.Name = "txtNewPassword";
            this.txtNewPassword.Padding = new System.Windows.Forms.Padding(5);
            this.txtNewPassword.PasswordChar = '●';
            this.txtNewPassword.ShowText = false;
            this.txtNewPassword.Size = new System.Drawing.Size(300, 35);
            this.txtNewPassword.TabIndex = 3;
            this.txtNewPassword.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtNewPassword.Watermark = "";
            // 
            // lblConfirmPassword
            // 
            this.lblConfirmPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblConfirmPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblConfirmPassword.Location = new System.Drawing.Point(30, 130);
            this.lblConfirmPassword.Name = "lblConfirmPassword";
            this.lblConfirmPassword.Size = new System.Drawing.Size(150, 30);
            this.lblConfirmPassword.TabIndex = 4;
            this.lblConfirmPassword.Text = "Xác nhận mật khẩu:";
            this.lblConfirmPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtConfirmPassword
            // 
            this.txtConfirmPassword.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtConfirmPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtConfirmPassword.Location = new System.Drawing.Point(200, 130);
            this.txtConfirmPassword.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtConfirmPassword.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtConfirmPassword.Name = "txtConfirmPassword";
            this.txtConfirmPassword.Padding = new System.Windows.Forms.Padding(5);
            this.txtConfirmPassword.PasswordChar = '●';
            this.txtConfirmPassword.ShowText = false;
            this.txtConfirmPassword.Size = new System.Drawing.Size(300, 35);
            this.txtConfirmPassword.TabIndex = 5;
            this.txtConfirmPassword.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtConfirmPassword.Watermark = "";
            // 
            // btnChangePassword
            // 
            this.btnChangePassword.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnChangePassword.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnChangePassword.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.btnChangePassword.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.btnChangePassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnChangePassword.Location = new System.Drawing.Point(200, 200);
            this.btnChangePassword.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnChangePassword.Name = "btnChangePassword";
            this.btnChangePassword.Size = new System.Drawing.Size(200, 40);
            this.btnChangePassword.TabIndex = 6;
            this.btnChangePassword.Text = "🔒 Đổi mật khẩu";
            this.btnChangePassword.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnChangePassword.Click += new System.EventHandler(this.BtnChangePassword_Click);
            // 
            // tabBackup
            // 
            this.tabBackup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(249)))), ((int)(((byte)(255)))));
            this.tabBackup.Controls.Add(this.lblBackupPath);
            this.tabBackup.Controls.Add(this.txtBackupPath);
            this.tabBackup.Controls.Add(this.btnBrowseBackup);
            this.tabBackup.Controls.Add(this.btnBackupNow);
            this.tabBackup.Controls.Add(this.btnRestore);
            this.tabBackup.Controls.Add(this.btnBrowseRestore);
            this.tabBackup.Location = new System.Drawing.Point(0, 40);
            this.tabBackup.Name = "tabBackup";
            this.tabBackup.Size = new System.Drawing.Size(770, 313);
            this.tabBackup.TabIndex = 3;
            this.tabBackup.Text = "Backup & Restore";
            // 
            // lblBackupPath
            // 
            this.lblBackupPath.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblBackupPath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblBackupPath.Location = new System.Drawing.Point(30, 30);
            this.lblBackupPath.Name = "lblBackupPath";
            this.lblBackupPath.Size = new System.Drawing.Size(150, 30);
            this.lblBackupPath.TabIndex = 0;
            this.lblBackupPath.Text = "Thư mục backup:";
            this.lblBackupPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtBackupPath
            // 
            this.txtBackupPath.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtBackupPath.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtBackupPath.Location = new System.Drawing.Point(200, 30);
            this.txtBackupPath.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtBackupPath.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtBackupPath.Name = "txtBackupPath";
            this.txtBackupPath.Padding = new System.Windows.Forms.Padding(5);
            this.txtBackupPath.ShowText = false;
            this.txtBackupPath.Size = new System.Drawing.Size(400, 35);
            this.txtBackupPath.TabIndex = 1;
            this.txtBackupPath.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtBackupPath.Watermark = "";
            // 
            // btnBrowseBackup
            // 
            this.btnBrowseBackup.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBrowseBackup.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.btnBrowseBackup.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.btnBrowseBackup.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.btnBrowseBackup.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.btnBrowseBackup.Location = new System.Drawing.Point(620, 30);
            this.btnBrowseBackup.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnBrowseBackup.Name = "btnBrowseBackup";
            this.btnBrowseBackup.Size = new System.Drawing.Size(100, 35);
            this.btnBrowseBackup.TabIndex = 2;
            this.btnBrowseBackup.Text = "📁 Chọn";
            this.btnBrowseBackup.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnBrowseBackup.Click += new System.EventHandler(this.BtnBrowseBackup_Click);
            // 
            // btnBackupNow
            // 
            this.btnBackupNow.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBackupNow.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.btnBackupNow.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            this.btnBackupNow.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            this.btnBackupNow.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnBackupNow.Location = new System.Drawing.Point(200, 100);
            this.btnBackupNow.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnBackupNow.Name = "btnBackupNow";
            this.btnBackupNow.Size = new System.Drawing.Size(150, 40);
            this.btnBackupNow.TabIndex = 3;
            this.btnBackupNow.Text = "💾 Backup";
            this.btnBackupNow.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnBackupNow.Click += new System.EventHandler(this.BtnBackupNow_Click);
            // 
            // btnRestore
            // 
            this.btnRestore.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRestore.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(158)))), ((int)(((byte)(11)))));
            this.btnRestore.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(119)))), ((int)(((byte)(6)))));
            this.btnRestore.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(119)))), ((int)(((byte)(6)))));
            this.btnRestore.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnRestore.Location = new System.Drawing.Point(370, 100);
            this.btnRestore.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(150, 40);
            this.btnRestore.TabIndex = 4;
            this.btnRestore.Text = "🔄 Restore";
            this.btnRestore.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnRestore.Click += new System.EventHandler(this.BtnBrowseRestore_Click);
            // 
            // btnBrowseRestore
            // 
            this.btnBrowseRestore.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBrowseRestore.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.btnBrowseRestore.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.btnBrowseRestore.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.btnBrowseRestore.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.btnBrowseRestore.Location = new System.Drawing.Point(540, 100);
            this.btnBrowseRestore.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnBrowseRestore.Name = "btnBrowseRestore";
            this.btnBrowseRestore.Size = new System.Drawing.Size(100, 40);
            this.btnBrowseRestore.TabIndex = 5;
            this.btnBrowseRestore.Text = "📁 Chọn file";
            this.btnBrowseRestore.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnBrowseRestore.Click += new System.EventHandler(this.BtnBrowseRestore_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(770, 433);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.pnlTop);
            this.Name = "SettingsForm";
            this.Text = "Cài đặt hệ thống";
            this.pnlTop.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tabDatabase.ResumeLayout(false);
            this.tabSecurity.ResumeLayout(false);
            this.tabBackup.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
