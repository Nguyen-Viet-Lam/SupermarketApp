using System;
using System.Drawing;
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
    public partial class EmployeeForm : Form
    {
        private UIPanel pnlTop;
        private UIPanel pnlForm;
        private UILabel lblTitle;
        private UILabel lblName;
        private UILabel lblUsername;
        private UILabel lblPassword;
        private UILabel lblRole;
        private UILabel lblStatus;
        private UITextBox txtName;
        private UITextBox txtUsername;
        private UITextBox txtPassword;
        private UIComboBox cbRole;
        private UIComboBox cbStatus;
        private UIButton btnAdd;
        private UIButton btnUpdate;
        private UIButton btnResetPassword;
        private UIButton btnClear;
        private UIButton btnRefresh;
        private UIDataGridView dgvEmployees;
        private int? selectedId = null;

        public EmployeeForm()
        {
            InitializeComponent();
            this.Load += async (s,e)=> await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            using (var db = new SupermarketContext())
            {
                var data = await Task.Run(()=> db.NhanVien
                    .OrderBy(x=>x.MaNV)
                    .Select(x=> new { 
                        x.MaNV, 
                        x.TenNV, 
                        x.TaiKhoan, 
                        x.VaiTro, 
                        TrangThai = x.TrangThai ? "Hoạt động" : "Vô hiệu",
                        x.NgayTao
                    })
                    .ToList());
                
                dgvEmployees.DataSource = data;
                
                if (dgvEmployees.Columns.Count > 0)
                {
                    dgvEmployees.Columns["MaNV"].HeaderText = "Mã NV";
                    dgvEmployees.Columns["MaNV"].Width = 80;
                    dgvEmployees.Columns["TenNV"].HeaderText = "Tên nhân viên";
                    dgvEmployees.Columns["TenNV"].Width = 200;
                    dgvEmployees.Columns["TaiKhoan"].HeaderText = "Tài khoản";
                    dgvEmployees.Columns["TaiKhoan"].Width = 150;
                    dgvEmployees.Columns["VaiTro"].HeaderText = "Vai trò";
                    dgvEmployees.Columns["VaiTro"].Width = 120;
                    dgvEmployees.Columns["TrangThai"].HeaderText = "Trạng thái";
                    dgvEmployees.Columns["TrangThai"].Width = 100;
                    dgvEmployees.Columns["NgayTao"].HeaderText = "Ngày tạo";
                    dgvEmployees.Columns["NgayTao"].Width = 150;
                }
            }
        }

        private void DgvEmployees_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvEmployees.Rows[e.RowIndex].Cells["MaNV"].Value != null)
            {
                selectedId = Convert.ToInt32(dgvEmployees.Rows[e.RowIndex].Cells["MaNV"].Value);
                txtName.Text = dgvEmployees.Rows[e.RowIndex].Cells["TenNV"].Value?.ToString();
                txtUsername.Text = dgvEmployees.Rows[e.RowIndex].Cells["TaiKhoan"].Value?.ToString();
                cbRole.Text = dgvEmployees.Rows[e.RowIndex].Cells["VaiTro"].Value?.ToString();
                cbStatus.SelectedIndex = dgvEmployees.Rows[e.RowIndex].Cells["TrangThai"].Value?.ToString() == "Hoạt động" ? 0 : 1;
                
                // Disable username when editing
                txtUsername.Enabled = false;
                txtPassword.Watermark = "Để trống nếu không đổi mật khẩu";
            }
        }

        private async void BtnAdd_Click(object sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageHelper.ShowWarning("Vui lòng nhập tên nhân viên!");
                txtName.Focus();
                return;
            }

            if (txtName.Text.Trim().Length < 2)
            {
                MessageHelper.ShowWarning("Tên nhân viên phải có ít nhất 2 ký tự!");
                txtName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageHelper.ShowWarning("Vui lòng nhập tài khoản!");
                txtUsername.Focus();
                return;
            }

            if (txtUsername.Text.Trim().Length < 3)
            {
                MessageHelper.ShowWarning("Tài khoản phải có ít nhất 3 ký tự!");
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageHelper.ShowWarning("Vui lòng nhập mật khẩu!");
                txtPassword.Focus();
                return;
            }

            if (txtPassword.Text.Length < 6)
            {
                MessageHelper.ShowWarning("Mật khẩu phải có ít nhất 6 ký tự!");
                txtPassword.Focus();
                return;
            }

            if (cbRole.SelectedIndex < 0)
            {
                MessageHelper.ShowWarning("Vui lòng chọn vai trò!");
                cbRole.Focus();
                return;
            }
            
            try
            {
                using (var db = new SupermarketContext())
                {
                    // Check duplicate username
                    var username = txtUsername.Text.Trim();
                    var exists = await Task.Run(() => db.NhanVien.Any(x => x.TaiKhoan == username));
                    if (exists)
                    {
                        MessageHelper.ShowWarning("Tài khoản đã tồn tại!");
                        txtUsername.Focus();
                        return;
                    }

                    var authService = new AuthService();
                    var (hash, salt) = authService.HashPassword(txtPassword.Text);

                    db.NhanVien.Add(new NhanVien{ 
                        TenNV = txtName.Text.Trim(), 
                        TaiKhoan = username,
                        MatKhauHash = hash,
                        Salt = salt,
                        VaiTro = cbRole.Text,
                        TrangThai = cbStatus.SelectedIndex == 0,
                        NgayTao = DateTime.Now
                    });
                    await db.SaveChangesAsync();
                    MessageHelper.ShowSuccess("Đã thêm nhân viên thành công!");
                    await LoadDataAsync();
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("Lỗi: " + ex.Message);
            }
        }

        private async void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedId == null)
            {
                MessageHelper.ShowWarning("Vui lòng chọn nhân viên cần sửa!");
                return;
            }

            // Validation
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageHelper.ShowWarning("Vui lòng nhập tên nhân viên!");
                txtName.Focus();
                return;
            }

            if (txtName.Text.Trim().Length < 2)
            {
                MessageHelper.ShowWarning("Tên nhân viên phải có ít nhất 2 ký tự!");
                txtName.Focus();
                return;
            }

            if (cbRole.SelectedIndex < 0)
            {
                MessageHelper.ShowWarning("Vui lòng chọn vai trò!");
                cbRole.Focus();
                return;
            }
            
            try
            {
                using (var db = new SupermarketContext())
                {
                    var nv = await db.NhanVien.FindAsync(selectedId.Value);
                    if (nv == null)
                    {
                        MessageHelper.ShowWarning("Không tìm thấy nhân viên!");
                        return;
                    }

                    nv.TenNV = txtName.Text.Trim();
                    nv.VaiTro = cbRole.Text;
                    nv.TrangThai = cbStatus.SelectedIndex == 0;

                    // Update password if provided
                    if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                    {
                        if (txtPassword.Text.Length < 6)
                        {
                            MessageHelper.ShowWarning("Mật khẩu phải có ít nhất 6 ký tự!");
                            txtPassword.Focus();
                            return;
                        }

                        var authService = new AuthService();
                        var (hash, salt) = authService.HashPassword(txtPassword.Text);
                        nv.MatKhauHash = hash;
                        nv.Salt = salt;
                    }

                    await db.SaveChangesAsync();
                    MessageHelper.ShowSuccess("Đã cập nhật nhân viên thành công!");
                    await LoadDataAsync();
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("Lỗi: " + ex.Message);
            }
        }

        private async void BtnResetPassword_Click(object sender, EventArgs e)
        {
            if (selectedId == null)
            {
                MessageHelper.ShowWarning("Vui lòng chọn nhân viên cần reset mật khẩu!");
                return;
            }

            if (!MessageHelper.ShowAsk("Bạn có chắc muốn reset mật khẩu về '123456'?"))
            {
                return;
            }

            try
            {
                using (var db = new SupermarketContext())
                {
                    var nv = await db.NhanVien.FindAsync(selectedId.Value);
                    if (nv != null)
                    {
                        var authService = new AuthService();
                        var (hash, salt) = authService.HashPassword("123456");
                        nv.MatKhauHash = hash;
                        nv.Salt = salt;
                        await db.SaveChangesAsync();
                        MessageHelper.ShowSuccess("Đã reset mật khẩu về '123456' thành công!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("Lỗi: " + ex.Message);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            await LoadDataAsync();
            MessageHelper.ShowTipSuccess("Đã tải lại dữ liệu!");
        }

        private void ClearForm()
        {
            txtName.Clear();
            txtUsername.Clear();
            txtPassword.Clear();
            cbRole.SelectedIndex = -1;
            cbStatus.SelectedIndex = 0;
            selectedId = null;
            txtUsername.Enabled = true;
            txtPassword.Watermark = "Nhập mật khẩu...";
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlTop = new Sunny.UI.UIPanel();
            this.lblTitle = new Sunny.UI.UILabel();
            this.btnRefresh = new Sunny.UI.UIButton();
            this.pnlForm = new Sunny.UI.UIPanel();
            this.lblName = new Sunny.UI.UILabel();
            this.txtName = new Sunny.UI.UITextBox();
            this.lblUsername = new Sunny.UI.UILabel();
            this.txtUsername = new Sunny.UI.UITextBox();
            this.lblPassword = new Sunny.UI.UILabel();
            this.txtPassword = new Sunny.UI.UITextBox();
            this.lblRole = new Sunny.UI.UILabel();
            this.cbRole = new Sunny.UI.UIComboBox();
            this.lblStatus = new Sunny.UI.UILabel();
            this.cbStatus = new Sunny.UI.UIComboBox();
            this.btnAdd = new Sunny.UI.UIButton();
            this.btnUpdate = new Sunny.UI.UIButton();
            this.btnResetPassword = new Sunny.UI.UIButton();
            this.btnClear = new Sunny.UI.UIButton();
            this.dgvEmployees = new Sunny.UI.UIDataGridView();
            this.pnlTop.SuspendLayout();
            this.pnlForm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployees)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.lblTitle);
            this.pnlTop.Controls.Add(this.btnRefresh);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.FillColor = System.Drawing.Color.White;
            this.pnlTop.FillColor2 = System.Drawing.Color.White;
            this.pnlTop.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlTop.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.pnlTop.Size = new System.Drawing.Size(1200, 70);
            this.pnlTop.TabIndex = 0;
            this.pnlTop.Text = null;
            this.pnlTop.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.lblTitle.Location = new System.Drawing.Point(20, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(400, 40);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "👥 QUẢN LÝ NHÂN VIÊN";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.btnRefresh.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.btnRefresh.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnRefresh.Location = new System.Drawing.Point(1050, 17);
            this.btnRefresh.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(130, 36);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "🔄 Làm mới";
            this.btnRefresh.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // pnlForm
            // 
            this.pnlForm.Controls.Add(this.lblName);
            this.pnlForm.Controls.Add(this.txtName);
            this.pnlForm.Controls.Add(this.lblUsername);
            this.pnlForm.Controls.Add(this.txtUsername);
            this.pnlForm.Controls.Add(this.lblPassword);
            this.pnlForm.Controls.Add(this.txtPassword);
            this.pnlForm.Controls.Add(this.lblRole);
            this.pnlForm.Controls.Add(this.cbRole);
            this.pnlForm.Controls.Add(this.lblStatus);
            this.pnlForm.Controls.Add(this.cbStatus);
            this.pnlForm.Controls.Add(this.btnAdd);
            this.pnlForm.Controls.Add(this.btnUpdate);
            this.pnlForm.Controls.Add(this.btnResetPassword);
            this.pnlForm.Controls.Add(this.btnClear);
            this.pnlForm.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlForm.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.pnlForm.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.pnlForm.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.pnlForm.Location = new System.Drawing.Point(0, 70);
            this.pnlForm.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlForm.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlForm.Name = "pnlForm";
            this.pnlForm.Padding = new System.Windows.Forms.Padding(10);
            this.pnlForm.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.pnlForm.Size = new System.Drawing.Size(1200, 150);
            this.pnlForm.TabIndex = 1;
            this.pnlForm.Text = null;
            this.pnlForm.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblName
            // 
            this.lblName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblName.Location = new System.Drawing.Point(20, 15);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(120, 25);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Tên NV";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtName
            // 
            this.txtName.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtName.Location = new System.Drawing.Point(20, 45);
            this.txtName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtName.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtName.Name = "txtName";
            this.txtName.Padding = new System.Windows.Forms.Padding(5);
            this.txtName.ShowText = false;
            this.txtName.Size = new System.Drawing.Size(220, 35);
            this.txtName.TabIndex = 1;
            this.txtName.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtName.Watermark = "Nhập tên nhân viên...";
            // 
            // lblUsername
            // 
            this.lblUsername.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblUsername.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblUsername.Location = new System.Drawing.Point(260, 15);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(120, 25);
            this.lblUsername.TabIndex = 2;
            this.lblUsername.Text = "Tài khoản";
            this.lblUsername.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtUsername
            // 
            this.txtUsername.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtUsername.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtUsername.Location = new System.Drawing.Point(260, 45);
            this.txtUsername.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtUsername.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Padding = new System.Windows.Forms.Padding(5);
            this.txtUsername.ShowText = false;
            this.txtUsername.Size = new System.Drawing.Size(180, 35);
            this.txtUsername.TabIndex = 3;
            this.txtUsername.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtUsername.Watermark = "Nhập tài khoản...";
            // 
            // lblPassword
            // 
            this.lblPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblPassword.Location = new System.Drawing.Point(460, 15);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(120, 25);
            this.lblPassword.TabIndex = 4;
            this.lblPassword.Text = "Mật khẩu";
            this.lblPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtPassword
            // 
            this.txtPassword.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtPassword.Location = new System.Drawing.Point(460, 45);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtPassword.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Padding = new System.Windows.Forms.Padding(5);
            this.txtPassword.PasswordChar = '●';
            this.txtPassword.ShowText = false;
            this.txtPassword.Size = new System.Drawing.Size(180, 35);
            this.txtPassword.TabIndex = 5;
            this.txtPassword.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtPassword.Watermark = "Nhập mật khẩu...";
            // 
            // lblRole
            // 
            this.lblRole.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblRole.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblRole.Location = new System.Drawing.Point(660, 15);
            this.lblRole.Name = "lblRole";
            this.lblRole.Size = new System.Drawing.Size(120, 25);
            this.lblRole.TabIndex = 6;
            this.lblRole.Text = "Vai trò";
            this.lblRole.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbRole
            // 
            this.cbRole.DataSource = null;
            this.cbRole.FillColor = System.Drawing.Color.White;
            this.cbRole.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbRole.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(254)))));
            this.cbRole.Items.AddRange(new object[] {
            "Admin",
            "Quản lý",
            "Nhân viên"});
            this.cbRole.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.cbRole.Location = new System.Drawing.Point(660, 45);
            this.cbRole.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbRole.MinimumSize = new System.Drawing.Size(63, 0);
            this.cbRole.Name = "cbRole";
            this.cbRole.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.cbRole.Size = new System.Drawing.Size(150, 35);
            this.cbRole.SymbolSize = 24;
            this.cbRole.TabIndex = 7;
            this.cbRole.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.cbRole.Watermark = "Chọn vai trò...";
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblStatus.Location = new System.Drawing.Point(830, 15);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(120, 25);
            this.lblStatus.TabIndex = 8;
            this.lblStatus.Text = "Trạng thái";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbStatus
            // 
            this.cbStatus.DataSource = null;
            this.cbStatus.FillColor = System.Drawing.Color.White;
            this.cbStatus.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbStatus.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(254)))));
            this.cbStatus.Items.AddRange(new object[] {
            "Hoạt động",
            "Vô hiệu"});
            this.cbStatus.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.cbStatus.Location = new System.Drawing.Point(830, 45);
            this.cbStatus.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbStatus.MinimumSize = new System.Drawing.Size(63, 0);
            this.cbStatus.Name = "cbStatus";
            this.cbStatus.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.cbStatus.Size = new System.Drawing.Size(140, 35);
            this.cbStatus.SymbolSize = 24;
            this.cbStatus.TabIndex = 9;
            this.cbStatus.Text = "Hoạt động";
            this.cbStatus.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.cbStatus.Watermark = "";
            // 
            // btnAdd
            // 
            this.btnAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAdd.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.btnAdd.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            this.btnAdd.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            this.btnAdd.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnAdd.Location = new System.Drawing.Point(20, 95);
            this.btnAdd.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(120, 38);
            this.btnAdd.TabIndex = 10;
            this.btnAdd.Text = "➕ Thêm";
            this.btnAdd.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnAdd.Click += new System.EventHandler(this.BtnAdd_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnUpdate.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnUpdate.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnUpdate.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnUpdate.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnUpdate.Location = new System.Drawing.Point(155, 95);
            this.btnUpdate.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(120, 38);
            this.btnUpdate.TabIndex = 11;
            this.btnUpdate.Text = "✏️ Sửa";
            this.btnUpdate.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnUpdate.Click += new System.EventHandler(this.BtnUpdate_Click);
            // 
            // btnResetPassword
            // 
            this.btnResetPassword.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnResetPassword.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(158)))), ((int)(((byte)(11)))));
            this.btnResetPassword.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(119)))), ((int)(((byte)(6)))));
            this.btnResetPassword.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(119)))), ((int)(((byte)(6)))));
            this.btnResetPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnResetPassword.Location = new System.Drawing.Point(290, 95);
            this.btnResetPassword.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnResetPassword.Name = "btnResetPassword";
            this.btnResetPassword.Size = new System.Drawing.Size(150, 38);
            this.btnResetPassword.TabIndex = 12;
            this.btnResetPassword.Text = "🔑 Reset MK";
            this.btnResetPassword.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnResetPassword.Click += new System.EventHandler(this.BtnResetPassword_Click);
            // 
            // btnClear
            // 
            this.btnClear.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClear.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.btnClear.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.btnClear.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.btnClear.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnClear.Location = new System.Drawing.Point(455, 95);
            this.btnClear.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(120, 38);
            this.btnClear.TabIndex = 13;
            this.btnClear.Text = "🗑️ Xóa form";
            this.btnClear.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnClear.Click += new System.EventHandler(this.BtnClear_Click);
            // 
            // dgvEmployees
            // 
            this.dgvEmployees.AllowUserToAddRows = false;
            this.dgvEmployees.AllowUserToDeleteRows = false;
            this.dgvEmployees.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(249)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvEmployees.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvEmployees.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvEmployees.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(249)))), ((int)(((byte)(255)))));
            this.dgvEmployees.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 10F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvEmployees.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvEmployees.ColumnHeadersHeight = 38;
            this.dgvEmployees.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 10F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(254)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvEmployees.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvEmployees.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvEmployees.EnableHeadersVisualStyles = false;
            this.dgvEmployees.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dgvEmployees.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(254)))));
            this.dgvEmployees.Location = new System.Drawing.Point(0, 220);
            this.dgvEmployees.MultiSelect = false;
            this.dgvEmployees.Name = "dgvEmployees";
            this.dgvEmployees.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(249)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 10F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvEmployees.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvEmployees.RowHeadersVisible = false;
            this.dgvEmployees.RowHeadersWidth = 51;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.dgvEmployees.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvEmployees.RowTemplate.Height = 32;
            this.dgvEmployees.SelectedIndex = -1;
            this.dgvEmployees.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEmployees.Size = new System.Drawing.Size(1200, 480);
            this.dgvEmployees.TabIndex = 2;
            this.dgvEmployees.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvEmployees_CellClick);
            // 
            // EmployeeForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.dgvEmployees);
            this.Controls.Add(this.pnlForm);
            this.Controls.Add(this.pnlTop);
            this.Name = "EmployeeForm";
            this.Text = "Quản lý nhân viên";
            this.pnlTop.ResumeLayout(false);
            this.pnlForm.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployees)).EndInit();
            this.ResumeLayout(false);

        }
    }
}

