using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sunny.UI;
using SupermarketApp.Utils;
using SupermarketApp.Data;
using SupermarketApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace SupermarketApp.Forms
{
    public partial class CustomerForm : Form
    {
        private UIPanel pnlTop;
        private UIPanel pnlForm;
        private UILabel lblTitle;
        private UILabel lblName;
        private UILabel lblPhone;
        private UILabel lblEmail;
        private UILabel lblAddress;
        private UILabel lblCategory;
        private UIComboBox cbCategory;
        private UITextBox txtName;
        private UITextBox txtPhone;
        private UITextBox txtEmail;
        private UITextBox txtAddress;
        private UIButton btnAdd;
        private UIButton btnUpdate;
        private UIButton btnDelete;
        private UIButton btnClear;
        private UIButton btnRefresh;
        private UIDataGridView dgvCustomers;
        private int? selectedId = null;

        // Context menu để gán loại KH nhanh
        private ContextMenuStrip cmsCategory;

        public CustomerForm()
        {
            InitializeComponent();
            InitCategoryMenu();
            this.Load += async (s,e)=> await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            using (var db = new SupermarketContext())
            {
                var data = await db.KhachHang
                    .AsNoTracking()
                    .OrderBy(x => x.MaKH)
                    .Select(x => new { x.MaKH, x.TenKH, x.LoaiKhachHang, x.SDT, x.Email, x.DiaChi, x.DiemTichLuy })
                    .ToListAsync();
                
                dgvCustomers.DataSource = data;
                
                if (dgvCustomers.Columns.Count > 0)
                {
                    dgvCustomers.Columns["MaKH"].HeaderText = "Mã KH";
                    dgvCustomers.Columns["MaKH"].Width = 70;
                    dgvCustomers.Columns["MaKH"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    
                    dgvCustomers.Columns["TenKH"].HeaderText = "Tên khách hàng";
                    dgvCustomers.Columns["TenKH"].Width = 220;

                    if (dgvCustomers.Columns.Contains("LoaiKhachHang"))
                    {
                        dgvCustomers.Columns["LoaiKhachHang"].HeaderText = "Loại KH";
                        dgvCustomers.Columns["LoaiKhachHang"].Width = 150;
                    }
                    
                    dgvCustomers.Columns["SDT"].HeaderText = "Số điện thoại";
                    dgvCustomers.Columns["SDT"].Width = 140;
                    dgvCustomers.Columns["SDT"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    
                    dgvCustomers.Columns["Email"].HeaderText = "Email";
                    dgvCustomers.Columns["Email"].Width = 200;
                    
                    dgvCustomers.Columns["DiaChi"].HeaderText = "Địa chỉ";
                    dgvCustomers.Columns["DiaChi"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    
                    dgvCustomers.Columns["DiemTichLuy"].HeaderText = "Điểm";
                    dgvCustomers.Columns["DiemTichLuy"].Width = 70;
                    dgvCustomers.Columns["DiemTichLuy"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            }
        }

        private void DgvCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvCustomers.Rows[e.RowIndex].Cells["MaKH"].Value != null)
            {
                selectedId = Convert.ToInt32(dgvCustomers.Rows[e.RowIndex].Cells["MaKH"].Value);
                txtName.Text = dgvCustomers.Rows[e.RowIndex].Cells["TenKH"].Value?.ToString();
                txtPhone.Text = dgvCustomers.Rows[e.RowIndex].Cells["SDT"].Value?.ToString();
                txtEmail.Text = dgvCustomers.Rows[e.RowIndex].Cells["Email"].Value?.ToString();
                txtAddress.Text = dgvCustomers.Rows[e.RowIndex].Cells["DiaChi"].Value?.ToString();

                // Đồng bộ "Loại KH" lên combobox
                var loai = dgvCustomers.Rows[e.RowIndex].Cells["LoaiKhachHang"].Value?.ToString();
                if (!string.IsNullOrEmpty(loai))
                {
                    int idx = cbCategory.Items.IndexOf(loai);
                    if (idx >= 0) cbCategory.SelectedIndex = idx;
                    else cbCategory.Text = loai;
                }
                else
                {
                    cbCategory.SelectedIndex = 0; // Mặc định "Khách vãng lai"
                }
            }
        }

        private async void BtnAdd_Click(object sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageHelper.ShowWarning("Vui lòng nhập tên khách hàng!");
                txtName.Focus();
                return;
            }

            if (txtName.Text.Trim().Length < 2)
            {
                MessageHelper.ShowWarning("Tên khách hàng phải có ít nhất 2 ký tự!");
                txtName.Focus();
                return;
            }

            // Validate phone if not empty
            if (!string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                string phone = txtPhone.Text.Trim();
                if (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^[0-9]{10,11}$"))
                {
                    MessageHelper.ShowWarning("Số điện thoại không hợp lệ! (10-11 số)");
                    txtPhone.Focus();
                    return;
                }
            }

            // Validate email if not empty
            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                string email = txtEmail.Text.Trim();
                if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    MessageHelper.ShowWarning("Email không hợp lệ!");
                    txtEmail.Focus();
                    return;
                }
            }
            
            try
            {
                using (var db = new SupermarketContext())
                {
                    // Check duplicate phone
                    if (!string.IsNullOrWhiteSpace(txtPhone.Text))
                    {
                        var phone = txtPhone.Text.Trim();
                        var exists = await db.KhachHang
                            .AsNoTracking()
                            .AnyAsync(x => x.SDT == phone);
                        if (exists)
                        {
                            MessageHelper.ShowWarning("Số điện thoại đã tồn tại!");
                            txtPhone.Focus();
                            return;
                        }
                    }

                    db.KhachHang.Add(new KhachHang{
                        TenKH = txtName.Text.Trim(),
                        SDT = string.IsNullOrWhiteSpace(txtPhone.Text)? null : txtPhone.Text.Trim(),
                        Email = string.IsNullOrWhiteSpace(txtEmail.Text)? null : txtEmail.Text.Trim(),
                        DiaChi = string.IsNullOrWhiteSpace(txtAddress.Text)? null : txtAddress.Text.Trim(),
                        LoaiKhachHang = string.IsNullOrWhiteSpace(cbCategory.Text) ? "Khách vãng lai" : cbCategory.Text.Trim(),
                        NgayTao = DateTime.Now,
                        DiemTichLuy = 0
                    });
                    await db.SaveChangesAsync();
                    MessageHelper.ShowSuccess("Đã thêm khách hàng thành công!");
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
                MessageHelper.ShowWarning("Vui lòng chọn khách hàng cần sửa!");
                return;
            }

            // Validation
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageHelper.ShowWarning("Vui lòng nhập tên khách hàng!");
                txtName.Focus();
                return;
            }

            if (txtName.Text.Trim().Length < 2)
            {
                MessageHelper.ShowWarning("Tên khách hàng phải có ít nhất 2 ký tự!");
                txtName.Focus();
                return;
            }

            // Validate phone if not empty
            if (!string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                string phone = txtPhone.Text.Trim();
                if (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^[0-9]{10,11}$"))
                {
                    MessageHelper.ShowWarning("Số điện thoại không hợp lệ! (10-11 số)");
                    txtPhone.Focus();
                    return;
                }
            }

            // Validate email if not empty
            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                string email = txtEmail.Text.Trim();
                if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    MessageHelper.ShowWarning("Email không hợp lệ!");
                    txtEmail.Focus();
                    return;
                }
            }
            
            try
            {
                using (var db = new SupermarketContext())
                {
                    var kh = await db.KhachHang.FindAsync(selectedId.Value);
                    if (kh == null)
                    {
                        MessageHelper.ShowWarning("Không tìm thấy khách hàng!");
                        return;
                    }

                    // Check duplicate phone (exclude current customer)
                    if (!string.IsNullOrWhiteSpace(txtPhone.Text))
                    {
                        var phone = txtPhone.Text.Trim();
                        var exists = await db.KhachHang
                            .AsNoTracking()
                            .AnyAsync(x => x.SDT == phone && x.MaKH != selectedId.Value);
                        if (exists)
                        {
                            MessageHelper.ShowWarning("Số điện thoại đã tồn tại!");
                            txtPhone.Focus();
                            return;
                        }
                    }

                    kh.TenKH = txtName.Text.Trim();
                    kh.SDT = string.IsNullOrWhiteSpace(txtPhone.Text)? null : txtPhone.Text.Trim();
                    kh.Email = string.IsNullOrWhiteSpace(txtEmail.Text)? null : txtEmail.Text.Trim();
                    kh.DiaChi = string.IsNullOrWhiteSpace(txtAddress.Text)? null : txtAddress.Text.Trim();
                    kh.LoaiKhachHang = string.IsNullOrWhiteSpace(cbCategory.Text) ? "Khách vãng lai" : cbCategory.Text.Trim();
                    await db.SaveChangesAsync();
                    MessageHelper.ShowSuccess("Đã cập nhật khách hàng thành công!");
                    await LoadDataAsync();
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("Lỗi: " + ex.Message);
            }
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            if (selectedId == null)
            {
                MessageHelper.ShowWarning("Vui lòng chọn khách hàng cần xóa!");
                return;
            }
            
            if (!MessageHelper.ShowAsk("Bạn có chắc muốn xóa khách hàng này?"))
            {
                return;
            }
            
            try
            {
                using (var db = new SupermarketContext())
                {
                    var kh = await db.KhachHang.FindAsync(selectedId.Value);
                    if (kh != null)
                    {
                        db.KhachHang.Remove(kh);
                        await db.SaveChangesAsync();
                        MessageHelper.ShowSuccess("Đã xóa khách hàng thành công!");
                        await LoadDataAsync();
                        ClearForm();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("Không thể xóa. Có thể khách hàng đã phát sinh hóa đơn.\n" + ex.Message);
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
            txtName.Text = txtPhone.Text = txtEmail.Text = txtAddress.Text = "";
            selectedId = null;
            if (cbCategory != null && cbCategory.Items.Count > 0)
            {
                cbCategory.SelectedIndex = 0;
            }
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
            this.pnlForm = new Sunny.UI.UIPanel();
            this.btnRefresh = new Sunny.UI.UIButton();
            this.btnClear = new Sunny.UI.UIButton();
            this.btnDelete = new Sunny.UI.UIButton();
            this.btnUpdate = new Sunny.UI.UIButton();
            this.btnAdd = new Sunny.UI.UIButton();
            this.txtAddress = new Sunny.UI.UITextBox();
            this.lblAddress = new Sunny.UI.UILabel();
            this.cbCategory = new Sunny.UI.UIComboBox();
            this.lblCategory = new Sunny.UI.UILabel();
            this.txtEmail = new Sunny.UI.UITextBox();
            this.lblEmail = new Sunny.UI.UILabel();
            this.txtPhone = new Sunny.UI.UITextBox();
            this.lblPhone = new Sunny.UI.UILabel();
            this.txtName = new Sunny.UI.UITextBox();
            this.lblName = new Sunny.UI.UILabel();
            this.dgvCustomers = new Sunny.UI.UIDataGridView();
            this.pnlTop.SuspendLayout();
            this.pnlForm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomers)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.lblTitle);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.FillColor = System.Drawing.Color.White;
            this.pnlTop.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlTop.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Padding = new System.Windows.Forms.Padding(15);
            this.pnlTop.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.pnlTop.Size = new System.Drawing.Size(1000, 60);
            this.pnlTop.TabIndex = 0;
            this.pnlTop.Text = null;
            this.pnlTop.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.lblTitle.Location = new System.Drawing.Point(15, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(458, 35);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "👥 QUẢN LÝ KHÁCH HÀNG";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlForm
            // 
            this.pnlForm.Controls.Add(this.btnRefresh);
            this.pnlForm.Controls.Add(this.btnClear);
            this.pnlForm.Controls.Add(this.btnDelete);
            this.pnlForm.Controls.Add(this.btnUpdate);
            this.pnlForm.Controls.Add(this.btnAdd);
            this.pnlForm.Controls.Add(this.txtAddress);
            this.pnlForm.Controls.Add(this.lblAddress);
            this.pnlForm.Controls.Add(this.cbCategory);
            this.pnlForm.Controls.Add(this.lblCategory);
            this.pnlForm.Controls.Add(this.txtEmail);
            this.pnlForm.Controls.Add(this.lblEmail);
            this.pnlForm.Controls.Add(this.txtPhone);
            this.pnlForm.Controls.Add(this.lblPhone);
            this.pnlForm.Controls.Add(this.txtName);
            this.pnlForm.Controls.Add(this.lblName);
            this.pnlForm.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlForm.FillColor = System.Drawing.Color.White;
            this.pnlForm.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pnlForm.Location = new System.Drawing.Point(0, 60);
            this.pnlForm.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlForm.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlForm.Name = "pnlForm";
            this.pnlForm.Padding = new System.Windows.Forms.Padding(15);
            this.pnlForm.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.pnlForm.Size = new System.Drawing.Size(1000, 160);
            this.pnlForm.TabIndex = 1;
            this.pnlForm.Text = null;
            this.pnlForm.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.btnRefresh.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.btnRefresh.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.btnRefresh.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            this.btnRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnRefresh.Location = new System.Drawing.Point(535, 100);
            this.btnRefresh.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(130, 40);
            this.btnRefresh.TabIndex = 12;
            this.btnRefresh.Text = "🔃 Tải lại";
            this.btnRefresh.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // btnClear
            // 
            this.btnClear.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClear.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnClear.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnClear.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnClear.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(78)))), ((int)(((byte)(216)))));
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnClear.Location = new System.Drawing.Point(405, 100);
            this.btnClear.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(120, 40);
            this.btnClear.TabIndex = 11;
            this.btnClear.Text = "🔄 Làm mới";
            this.btnClear.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnClear.Click += new System.EventHandler(this.BtnClear_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDelete.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnDelete.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.btnDelete.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.btnDelete.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.btnDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnDelete.Location = new System.Drawing.Point(275, 100);
            this.btnDelete.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(120, 40);
            this.btnDelete.TabIndex = 10;
            this.btnDelete.Text = "🗑️ Xóa";
            this.btnDelete.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnUpdate.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnUpdate.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnUpdate.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnUpdate.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(78)))), ((int)(((byte)(216)))));
            this.btnUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnUpdate.Location = new System.Drawing.Point(145, 100);
            this.btnUpdate.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(120, 40);
            this.btnUpdate.TabIndex = 9;
            this.btnUpdate.Text = "✏️ Sửa";
            this.btnUpdate.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnUpdate.Click += new System.EventHandler(this.BtnUpdate_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAdd.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(197)))), ((int)(((byte)(94)))));
            this.btnAdd.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(74)))));
            this.btnAdd.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(74)))));
            this.btnAdd.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(128)))), ((int)(((byte)(61)))));
            this.btnAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnAdd.Location = new System.Drawing.Point(15, 100);
            this.btnAdd.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(120, 40);
            this.btnAdd.TabIndex = 8;
            this.btnAdd.Text = "➕ Thêm";
            this.btnAdd.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnAdd.Click += new System.EventHandler(this.BtnAdd_Click);
            // 
            // txtAddress
            // 
            this.txtAddress.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtAddress.Location = new System.Drawing.Point(574, 45);
            this.txtAddress.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtAddress.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Padding = new System.Windows.Forms.Padding(5);
            this.txtAddress.ShowText = false;
            this.txtAddress.Size = new System.Drawing.Size(260, 35);
            this.txtAddress.TabIndex = 7;
            this.txtAddress.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtAddress.Watermark = "Địa chỉ khách hàng...";
            // 
            // lblAddress
            // 
            this.lblAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblAddress.Location = new System.Drawing.Point(570, 15);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(120, 25);
            this.lblAddress.TabIndex = 6;
            this.lblAddress.Text = "Địa chỉ";
            this.lblAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbCategory
            // 
            this.cbCategory.DataSource = null;
            this.cbCategory.FillColor = System.Drawing.Color.White;
            this.cbCategory.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.cbCategory.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.cbCategory.Items.AddRange(new object[] {
            "Khách vãng lai",
            "Khách thân quen"});
            this.cbCategory.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.cbCategory.Location = new System.Drawing.Point(840, 45);
            this.cbCategory.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbCategory.MinimumSize = new System.Drawing.Size(1, 16);
            this.cbCategory.Name = "cbCategory";
            this.cbCategory.Padding = new System.Windows.Forms.Padding(5, 5, 30, 5);
            this.cbCategory.Size = new System.Drawing.Size(160, 35);
            this.cbCategory.SymbolSize = 24;
            this.cbCategory.TabIndex = 14;
            this.cbCategory.Text = "Khách vãng lai";
            this.cbCategory.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.cbCategory.Watermark = "Chọn loại";
            // 
            // lblCategory
            // 
            this.lblCategory.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblCategory.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblCategory.Location = new System.Drawing.Point(836, 15);
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Size = new System.Drawing.Size(120, 25);
            this.lblCategory.TabIndex = 13;
            this.lblCategory.Text = "Loại KH";
            this.lblCategory.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtEmail
            // 
            this.txtEmail.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtEmail.Location = new System.Drawing.Point(366, 45);
            this.txtEmail.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtEmail.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Padding = new System.Windows.Forms.Padding(5);
            this.txtEmail.ShowText = false;
            this.txtEmail.Size = new System.Drawing.Size(200, 35);
            this.txtEmail.TabIndex = 5;
            this.txtEmail.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtEmail.Watermark = "example@email.com";
            // 
            // lblEmail
            // 
            this.lblEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblEmail.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblEmail.Location = new System.Drawing.Point(362, 15);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(120, 25);
            this.lblEmail.TabIndex = 4;
            this.lblEmail.Text = "Email";
            this.lblEmail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtPhone
            // 
            this.txtPhone.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtPhone.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtPhone.Location = new System.Drawing.Point(208, 45);
            this.txtPhone.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtPhone.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Padding = new System.Windows.Forms.Padding(5);
            this.txtPhone.ShowText = false;
            this.txtPhone.Size = new System.Drawing.Size(150, 35);
            this.txtPhone.TabIndex = 3;
            this.txtPhone.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtPhone.Watermark = "0xxxxxxxxx";
            // 
            // lblPhone
            // 
            this.lblPhone.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblPhone.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblPhone.Location = new System.Drawing.Point(204, 15);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(120, 25);
            this.lblPhone.TabIndex = 2;
            this.lblPhone.Text = "Số điện thoại";
            this.lblPhone.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtName
            // 
            this.txtName.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtName.Location = new System.Drawing.Point(0, 45);
            this.txtName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtName.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtName.Name = "txtName";
            this.txtName.Padding = new System.Windows.Forms.Padding(5);
            this.txtName.ShowText = false;
            this.txtName.Size = new System.Drawing.Size(200, 35);
            this.txtName.TabIndex = 1;
            this.txtName.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtName.Watermark = "Nhập tên khách hàng...";
            // 
            // lblName
            // 
            this.lblName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblName.Location = new System.Drawing.Point(11, 15);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(120, 25);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Tên KH";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dgvCustomers
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(246)))), ((int)(((byte)(255)))));
            this.dgvCustomers.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvCustomers.BackgroundColor = System.Drawing.Color.White;
            this.dgvCustomers.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCustomers.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvCustomers.ColumnHeadersHeight = 40;
            this.dgvCustomers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(254)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(64)))), ((int)(((byte)(175)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvCustomers.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvCustomers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCustomers.EnableHeadersVisualStyles = false;
            this.dgvCustomers.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.dgvCustomers.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(254)))));
            this.dgvCustomers.Location = new System.Drawing.Point(0, 220);
            this.dgvCustomers.Name = "dgvCustomers";
            this.dgvCustomers.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(246)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCustomers.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvCustomers.RowHeadersWidth = 51;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.dgvCustomers.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvCustomers.RowTemplate.Height = 35;
            this.dgvCustomers.SelectedIndex = -1;
            this.dgvCustomers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCustomers.Size = new System.Drawing.Size(1000, 380);
            this.dgvCustomers.StripeOddColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(246)))), ((int)(((byte)(255)))));
            this.dgvCustomers.TabIndex = 2;
            this.dgvCustomers.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvCustomers_CellClick);
            // 
            // CustomerForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.Controls.Add(this.dgvCustomers);
            this.Controls.Add(this.pnlForm);
            this.Controls.Add(this.pnlTop);
            this.Name = "CustomerForm";
            this.Text = "Quản lý khách hàng";
            this.pnlTop.ResumeLayout(false);
            this.pnlForm.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomers)).EndInit();
            this.ResumeLayout(false);

        }
        // Khởi tạo menu chuột phải để gán nhanh "Loại KH"
        private void InitCategoryMenu()
        {
            cmsCategory = new ContextMenuStrip();

            // Load category options from CAIDAT (TenCaiDat='CustomerCategories'); fallback to defaults
            var options = new System.Collections.Generic.List<string>();
            try
            {
                using (var db = new SupermarketContext())
                {
                    var val = db.CaiDat
                        .AsNoTracking()
                        .Where(x => x.TenCaiDat == "CustomerCategories")
                        .Select(x => x.GiaTri)
                        .FirstOrDefault();

                    if (!string.IsNullOrWhiteSpace(val))
                    {
                        options = val.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(s => s.Trim())
                                     .Where(s => s.Length > 0)
                                     .Distinct(StringComparer.CurrentCultureIgnoreCase)
                                     .ToList();
                    }
                }
            }
            catch { }

            if (options.Count == 0)
            {
                options = new System.Collections.Generic.List<string> { "Khách vãng lai", "Khách thân quen", "VIP" };
            }

            // Build context menu items dynamically
            foreach (var opt in options)
            {
                var item = new ToolStripMenuItem($"Gán loại: {opt}");
                item.Click += (s, e) => UpdateCustomerCategory(opt);
                cmsCategory.Items.Add(item);
            }

            dgvCustomers.ContextMenuStrip = cmsCategory;

            // Ensure right-click selects the row
            dgvCustomers.CellMouseDown += DgvCustomers_CellMouseDown;

            // Also populate the category ComboBox for add/update
            if (cbCategory != null)
            {
                cbCategory.Items.Clear();
                foreach (var opt in options)
                    cbCategory.Items.Add(opt);
                if (cbCategory.Items.Count > 0)
                    cbCategory.SelectedIndex = 0;
            }
        }

        // Chọn dòng khi click chuột phải để context menu thao tác đúng dòng
        private void DgvCustomers_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                dgvCustomers.ClearSelection();
                dgvCustomers.Rows[e.RowIndex].Selected = true;

                if (e.ColumnIndex >= 0)
                {
                    dgvCustomers.CurrentCell = dgvCustomers.Rows[e.RowIndex].Cells[e.ColumnIndex];
                }
                else if (dgvCustomers.Rows[e.RowIndex].Cells.Count > 0)
                {
                    dgvCustomers.CurrentCell = dgvCustomers.Rows[e.RowIndex].Cells[0];
                }

                var idObj = dgvCustomers.Rows[e.RowIndex].Cells["MaKH"].Value;
                selectedId = idObj != null ? Convert.ToInt32(idObj) : (int?)null;
            }
        }

        // Cập nhật LoạiKhachHang cho khách hàng đang chọn
        private async void UpdateCustomerCategory(string category)
        {
            if (selectedId == null)
            {
                MessageHelper.ShowWarning("Vui lòng chọn khách hàng trước khi gán loại!");
                return;
            }

            try
            {
                using (var db = new SupermarketContext())
                {
                    var kh = await db.KhachHang.FindAsync(selectedId.Value);
                    if (kh == null)
                    {
                        MessageHelper.ShowWarning("Không tìm thấy khách hàng!");
                        return;
                    }

                    kh.LoaiKhachHang = category;
                    await db.SaveChangesAsync();
                }

                await EnsureCustomerCategoryPersisted(category);

                MessageHelper.ShowSuccess($"Đã gán loại '{category}' cho khách hàng!");
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("Không thể cập nhật loại khách hàng: " + ex.Message);
            }
        }

        // Đảm bảo loại KH mới được thêm vào cấu hình CAIDAT (TenCaiDat='CustomerCategories') và cập nhật combobox
        private async System.Threading.Tasks.Task EnsureCustomerCategoryPersisted(string category)
        {
            if (string.IsNullOrWhiteSpace(category)) return;
            category = category.Trim();

            try
            {
                using (var db = new SupermarketContext())
                {
                    var setting = db.CaiDat.FirstOrDefault(x => x.TenCaiDat == "CustomerCategories");
                    if (setting == null)
                    {
                        setting = new SupermarketApp.Data.Models.CaiDat
                        {
                            TenCaiDat = "CustomerCategories",
                            GiaTri = category,
                            MoTa = "Danh sách loại khách hàng cho UI",
                            NgayCapNhat = DateTime.Now,
                            NguoiCapNhat = "System"
                        };
                        db.CaiDat.Add(setting);
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        var parts = (setting.GiaTri ?? "")
                            .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim())
                            .Where(s => !string.IsNullOrWhiteSpace(s))
                            .ToList();

                        bool exists = parts.Any(s => string.Equals(s, category, StringComparison.CurrentCultureIgnoreCase));
                        if (!exists)
                        {
                            parts.Add(category);
                            setting.GiaTri = string.Join(";", parts);
                            setting.NgayCapNhat = DateTime.Now;
                            setting.NguoiCapNhat = "System";
                            await db.SaveChangesAsync();
                        }
                    }
                }

                // Cập nhật danh sách combobox tại UI nếu chưa có
                if (cbCategory != null)
                {
                    bool found = false;
                    foreach (var item in cbCategory.Items)
                    {
                        if (string.Equals(Convert.ToString(item), category, StringComparison.CurrentCultureIgnoreCase))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        cbCategory.Items.Add(category);
                    }
                }
            }
            catch (Exception ex)
            {
                // Best-effort, không chặn luồng chính
                Console.WriteLine("EnsureCustomerCategoryPersisted warning: " + ex.Message);
            }
        }

    }
}
