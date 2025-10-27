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
    public partial class SupplierForm : Form
    {
        private UIPanel pnlTop;
        private UIPanel pnlForm;
        private UILabel lblTitle;
        private UILabel lblName;
        private UILabel lblAddress;
        private UILabel lblPhone;
        private UILabel lblEmail;
        private UILabel lblTaxCode;
        private UILabel lblContact;
        private UITextBox txtName;
        private UITextBox txtAddress;
        private UITextBox txtPhone;
        private UITextBox txtEmail;
        private UITextBox txtTaxCode;
        private UITextBox txtContact;
        private UIButton btnAdd;
        private UIButton btnUpdate;
        private UIButton btnDelete;
        private UIButton btnClear;
        private UIButton btnRefresh;
        private UIDataGridView dgvSuppliers;
        private int? selectedId = null;

        public SupplierForm()
        {
            InitializeComponent();
            this.Load += async (s,e)=> await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            using (var db = new SupermarketContext())
            {
                var data = await db.NhaCungCap
                    .AsNoTracking()
                    .OrderBy(x => x.MaNCC)
                    .Select(x => new {
                        x.MaNCC,
                        x.TenNCC,
                        x.DiaChi,
                        x.SDT,
                        x.Email,
                        x.MaSoThue,
                        x.NguoiLienHe,
                        TrangThai = x.TrangThai ? "Hoạt động" : "Ngừng",
                        x.NgayTao
                    })
                    .ToListAsync();
                
                dgvSuppliers.DataSource = data;
                
                if (dgvSuppliers.Columns.Count > 0)
                {
                    dgvSuppliers.Columns["MaNCC"].HeaderText = "Mã NCC";
                    dgvSuppliers.Columns["MaNCC"].Width = 80;
                    dgvSuppliers.Columns["TenNCC"].HeaderText = "Tên nhà cung cấp";
                    dgvSuppliers.Columns["TenNCC"].Width = 220;
                    dgvSuppliers.Columns["DiaChi"].HeaderText = "Địa chỉ";
                    dgvSuppliers.Columns["DiaChi"].Width = 200;
                    dgvSuppliers.Columns["SDT"].HeaderText = "SĐT";
                    dgvSuppliers.Columns["SDT"].Width = 110;
                    dgvSuppliers.Columns["Email"].HeaderText = "Email";
                    dgvSuppliers.Columns["Email"].Width = 150;
                    dgvSuppliers.Columns["MaSoThue"].HeaderText = "MST";
                    dgvSuppliers.Columns["MaSoThue"].Width = 100;
                    dgvSuppliers.Columns["NguoiLienHe"].HeaderText = "Người liên hệ";
                    dgvSuppliers.Columns["NguoiLienHe"].Width = 150;
                    dgvSuppliers.Columns["TrangThai"].HeaderText = "Trạng thái";
                    dgvSuppliers.Columns["TrangThai"].Width = 100;
                    dgvSuppliers.Columns["NgayTao"].HeaderText = "Ngày tạo";
                    dgvSuppliers.Columns["NgayTao"].Width = 150;
                }
            }
        }

        private void DgvSuppliers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvSuppliers.Rows[e.RowIndex].Cells["MaNCC"].Value != null)
            {
                selectedId = Convert.ToInt32(dgvSuppliers.Rows[e.RowIndex].Cells["MaNCC"].Value);
                txtName.Text = dgvSuppliers.Rows[e.RowIndex].Cells["TenNCC"].Value?.ToString();
                txtAddress.Text = dgvSuppliers.Rows[e.RowIndex].Cells["DiaChi"].Value?.ToString();
                txtPhone.Text = dgvSuppliers.Rows[e.RowIndex].Cells["SDT"].Value?.ToString();
                txtEmail.Text = dgvSuppliers.Rows[e.RowIndex].Cells["Email"].Value?.ToString();
                txtTaxCode.Text = dgvSuppliers.Rows[e.RowIndex].Cells["MaSoThue"].Value?.ToString();
                txtContact.Text = dgvSuppliers.Rows[e.RowIndex].Cells["NguoiLienHe"].Value?.ToString();
            }
        }

        private async void BtnAdd_Click(object sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageHelper.ShowWarning("Vui lòng nhập tên nhà cung cấp!");
                txtName.Focus();
                return;
            }

            if (txtName.Text.Trim().Length < 2)
            {
                MessageHelper.ShowWarning("Tên nhà cung cấp phải có ít nhất 2 ký tự!");
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
                    db.NhaCungCap.Add(new NhaCungCap{ 
                        TenNCC = txtName.Text.Trim(), 
                        DiaChi = string.IsNullOrWhiteSpace(txtAddress.Text) ? null : txtAddress.Text.Trim(), 
                        SDT = string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text.Trim(), 
                        Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                        MaSoThue = string.IsNullOrWhiteSpace(txtTaxCode.Text) ? null : txtTaxCode.Text.Trim(),
                        NguoiLienHe = string.IsNullOrWhiteSpace(txtContact.Text) ? null : txtContact.Text.Trim(),
                        NgayTao = DateTime.Now,
                        TrangThai = true
                    });
                    await db.SaveChangesAsync();
                    MessageHelper.ShowSuccess("Đã thêm nhà cung cấp thành công!");
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
                MessageHelper.ShowWarning("Vui lòng chọn nhà cung cấp cần sửa!");
                return;
            }

            // Validation
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageHelper.ShowWarning("Vui lòng nhập tên nhà cung cấp!");
                txtName.Focus();
                return;
            }

            if (txtName.Text.Trim().Length < 2)
            {
                MessageHelper.ShowWarning("Tên nhà cung cấp phải có ít nhất 2 ký tự!");
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
                    var ncc = await db.NhaCungCap.FindAsync(selectedId.Value);
                    if (ncc == null)
                    {
                        MessageHelper.ShowWarning("Không tìm thấy nhà cung cấp!");
                        return;
                    }

                    ncc.TenNCC = txtName.Text.Trim();
                    ncc.DiaChi = string.IsNullOrWhiteSpace(txtAddress.Text) ? null : txtAddress.Text.Trim();
                    ncc.SDT = string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text.Trim();
                    ncc.Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim();
                    ncc.MaSoThue = string.IsNullOrWhiteSpace(txtTaxCode.Text) ? null : txtTaxCode.Text.Trim();
                    ncc.NguoiLienHe = string.IsNullOrWhiteSpace(txtContact.Text) ? null : txtContact.Text.Trim();
                    
                    await db.SaveChangesAsync();
                    MessageHelper.ShowSuccess("Đã cập nhật nhà cung cấp thành công!");
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
                MessageHelper.ShowWarning("Vui lòng chọn nhà cung cấp cần xóa!");
                return;
            }

            if (!MessageHelper.ShowAsk("Bạn có chắc muốn xóa nhà cung cấp này?\n\nLưu ý: Không thể xóa nếu đã có phiếu nhập hàng."))
            {
                return;
            }

            try
            {
                using (var db = new SupermarketContext())
                {
                    var ncc = await db.NhaCungCap.FindAsync(selectedId.Value);
                    if (ncc == null)
                    {
                        MessageHelper.ShowWarning("Không tìm thấy nhà cung cấp!");
                        return;
                    }

                    // Kiểm tra xem đã có phiếu nhập chưa
                    var existsInPhieuNhap = await db.PhieuNhap
                        .AsNoTracking()
                        .AnyAsync(x => x.MaNCC == selectedId.Value);
                    if (existsInPhieuNhap)
                    {
                        MessageHelper.ShowWarning("Không thể xóa! Nhà cung cấp đã có phiếu nhập hàng.\n\nBạn có thể vô hiệu hóa nhà cung cấp thay vì xóa.");
                        return;
                    }

                    db.NhaCungCap.Remove(ncc);
                    await db.SaveChangesAsync();
                    MessageHelper.ShowSuccess("Đã xóa nhà cung cấp thành công!");
                    await LoadDataAsync();
                    ClearForm();
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
            txtAddress.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            txtTaxCode.Clear();
            txtContact.Clear();
            selectedId = null;
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
            this.lblAddress = new Sunny.UI.UILabel();
            this.txtAddress = new Sunny.UI.UITextBox();
            this.lblPhone = new Sunny.UI.UILabel();
            this.txtPhone = new Sunny.UI.UITextBox();
            this.lblEmail = new Sunny.UI.UILabel();
            this.txtEmail = new Sunny.UI.UITextBox();
            this.lblTaxCode = new Sunny.UI.UILabel();
            this.txtTaxCode = new Sunny.UI.UITextBox();
            this.lblContact = new Sunny.UI.UILabel();
            this.txtContact = new Sunny.UI.UITextBox();
            this.btnAdd = new Sunny.UI.UIButton();
            this.btnUpdate = new Sunny.UI.UIButton();
            this.btnDelete = new Sunny.UI.UIButton();
            this.btnClear = new Sunny.UI.UIButton();
            this.dgvSuppliers = new Sunny.UI.UIDataGridView();
            this.pnlTop.SuspendLayout();
            this.pnlForm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSuppliers)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.lblTitle);
            this.pnlTop.Controls.Add(this.btnRefresh);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.FillColor = System.Drawing.Color.White;
            this.pnlTop.FillColor2 = System.Drawing.Color.White;
            this.pnlTop.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
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
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.lblTitle.Location = new System.Drawing.Point(20, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(515, 40);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "🏭 QUẢN LÝ NHÀ CUNG CẤP";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.btnRefresh.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.btnRefresh.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.btnRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
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
            this.pnlForm.Controls.Add(this.lblAddress);
            this.pnlForm.Controls.Add(this.txtAddress);
            this.pnlForm.Controls.Add(this.lblPhone);
            this.pnlForm.Controls.Add(this.txtPhone);
            this.pnlForm.Controls.Add(this.lblEmail);
            this.pnlForm.Controls.Add(this.txtEmail);
            this.pnlForm.Controls.Add(this.lblTaxCode);
            this.pnlForm.Controls.Add(this.txtTaxCode);
            this.pnlForm.Controls.Add(this.lblContact);
            this.pnlForm.Controls.Add(this.txtContact);
            this.pnlForm.Controls.Add(this.btnAdd);
            this.pnlForm.Controls.Add(this.btnUpdate);
            this.pnlForm.Controls.Add(this.btnDelete);
            this.pnlForm.Controls.Add(this.btnClear);
            this.pnlForm.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlForm.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.pnlForm.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.pnlForm.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
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
            this.lblName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblName.Location = new System.Drawing.Point(20, 15);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(150, 25);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Tên NCC *";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtName
            // 
            this.txtName.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtName.Location = new System.Drawing.Point(20, 45);
            this.txtName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtName.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtName.Name = "txtName";
            this.txtName.Padding = new System.Windows.Forms.Padding(5);
            this.txtName.ShowText = false;
            this.txtName.Size = new System.Drawing.Size(250, 35);
            this.txtName.TabIndex = 1;
            this.txtName.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtName.Watermark = "Nhập tên nhà cung cấp...";
            // 
            // lblAddress
            // 
            this.lblAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblAddress.Location = new System.Drawing.Point(290, 15);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(100, 25);
            this.lblAddress.TabIndex = 2;
            this.lblAddress.Text = "Địa chỉ";
            this.lblAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtAddress
            // 
            this.txtAddress.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtAddress.Location = new System.Drawing.Point(290, 45);
            this.txtAddress.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtAddress.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Padding = new System.Windows.Forms.Padding(5);
            this.txtAddress.ShowText = false;
            this.txtAddress.Size = new System.Drawing.Size(300, 35);
            this.txtAddress.TabIndex = 3;
            this.txtAddress.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtAddress.Watermark = "Nhập địa chỉ...";
            // 
            // lblPhone
            // 
            this.lblPhone.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblPhone.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblPhone.Location = new System.Drawing.Point(610, 15);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(100, 25);
            this.lblPhone.TabIndex = 4;
            this.lblPhone.Text = "SĐT";
            this.lblPhone.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtPhone
            // 
            this.txtPhone.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtPhone.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtPhone.Location = new System.Drawing.Point(610, 45);
            this.txtPhone.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtPhone.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Padding = new System.Windows.Forms.Padding(5);
            this.txtPhone.ShowText = false;
            this.txtPhone.Size = new System.Drawing.Size(150, 35);
            this.txtPhone.TabIndex = 5;
            this.txtPhone.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtPhone.Watermark = "SĐT...";
            // 
            // lblEmail
            // 
            this.lblEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblEmail.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblEmail.Location = new System.Drawing.Point(780, 15);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(100, 25);
            this.lblEmail.TabIndex = 6;
            this.lblEmail.Text = "Email";
            this.lblEmail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtEmail
            // 
            this.txtEmail.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtEmail.Location = new System.Drawing.Point(780, 45);
            this.txtEmail.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtEmail.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Padding = new System.Windows.Forms.Padding(5);
            this.txtEmail.ShowText = false;
            this.txtEmail.Size = new System.Drawing.Size(200, 35);
            this.txtEmail.TabIndex = 7;
            this.txtEmail.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtEmail.Watermark = "Email...";
            // 
            // lblTaxCode
            // 
            this.lblTaxCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblTaxCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblTaxCode.Location = new System.Drawing.Point(1000, 15);
            this.lblTaxCode.Name = "lblTaxCode";
            this.lblTaxCode.Size = new System.Drawing.Size(120, 25);
            this.lblTaxCode.TabIndex = 8;
            this.lblTaxCode.Text = "Mã số thuế";
            this.lblTaxCode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtTaxCode
            // 
            this.txtTaxCode.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtTaxCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtTaxCode.Location = new System.Drawing.Point(1000, 45);
            this.txtTaxCode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtTaxCode.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtTaxCode.Name = "txtTaxCode";
            this.txtTaxCode.Padding = new System.Windows.Forms.Padding(5);
            this.txtTaxCode.ShowText = false;
            this.txtTaxCode.Size = new System.Drawing.Size(180, 35);
            this.txtTaxCode.TabIndex = 9;
            this.txtTaxCode.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtTaxCode.Watermark = "MST...";
            // 
            // lblContact
            // 
            this.lblContact.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblContact.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblContact.Location = new System.Drawing.Point(42, 93);
            this.lblContact.Name = "lblContact";
            this.lblContact.Size = new System.Drawing.Size(150, 35);
            this.lblContact.TabIndex = 10;
            this.lblContact.Text = "Người liên hệ";
            this.lblContact.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtContact
            // 
            this.txtContact.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtContact.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtContact.Location = new System.Drawing.Point(213, 93);
            this.txtContact.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtContact.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtContact.Name = "txtContact";
            this.txtContact.Padding = new System.Windows.Forms.Padding(5);
            this.txtContact.ShowText = false;
            this.txtContact.Size = new System.Drawing.Size(250, 35);
            this.txtContact.TabIndex = 11;
            this.txtContact.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtContact.Watermark = "Tên người liên hệ...";
            // 
            // btnAdd
            // 
            this.btnAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAdd.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.btnAdd.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            this.btnAdd.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            this.btnAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.btnAdd.Location = new System.Drawing.Point(517, 88);
            this.btnAdd.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(110, 38);
            this.btnAdd.TabIndex = 12;
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
            this.btnUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.btnUpdate.Location = new System.Drawing.Point(650, 88);
            this.btnUpdate.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(110, 38);
            this.btnUpdate.TabIndex = 13;
            this.btnUpdate.Text = "✏️ Sửa";
            this.btnUpdate.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnUpdate.Click += new System.EventHandler(this.BtnUpdate_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDelete.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnDelete.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.btnDelete.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.btnDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.btnDelete.Location = new System.Drawing.Point(784, 88);
            this.btnDelete.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(110, 38);
            this.btnDelete.TabIndex = 14;
            this.btnDelete.Text = "🗑️ Xóa";
            this.btnDelete.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // btnClear
            // 
            this.btnClear.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClear.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.btnClear.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.btnClear.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnClear.Location = new System.Drawing.Point(914, 88);
            this.btnClear.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(120, 38);
            this.btnClear.TabIndex = 15;
            this.btnClear.Text = "🔄 Làm mới";
            this.btnClear.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnClear.Click += new System.EventHandler(this.BtnClear_Click);
            // 
            // dgvSuppliers
            // 
            this.dgvSuppliers.AllowUserToAddRows = false;
            this.dgvSuppliers.AllowUserToDeleteRows = false;
            this.dgvSuppliers.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(249)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.dgvSuppliers.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvSuppliers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSuppliers.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(249)))), ((int)(((byte)(255)))));
            this.dgvSuppliers.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSuppliers.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvSuppliers.ColumnHeadersHeight = 38;
            this.dgvSuppliers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(254)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvSuppliers.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvSuppliers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSuppliers.EnableHeadersVisualStyles = false;
            this.dgvSuppliers.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.dgvSuppliers.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(254)))));
            this.dgvSuppliers.Location = new System.Drawing.Point(0, 220);
            this.dgvSuppliers.MultiSelect = false;
            this.dgvSuppliers.Name = "dgvSuppliers";
            this.dgvSuppliers.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(249)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSuppliers.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvSuppliers.RowHeadersVisible = false;
            this.dgvSuppliers.RowHeadersWidth = 51;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.dgvSuppliers.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvSuppliers.RowTemplate.Height = 32;
            this.dgvSuppliers.SelectedIndex = -1;
            this.dgvSuppliers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSuppliers.Size = new System.Drawing.Size(1200, 480);
            this.dgvSuppliers.TabIndex = 2;
            this.dgvSuppliers.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvSuppliers_CellClick);
            // 
            // SupplierForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.dgvSuppliers);
            this.Controls.Add(this.pnlForm);
            this.Controls.Add(this.pnlTop);
            this.Name = "SupplierForm";
            this.Text = "Quản lý nhà cung cấp";
            this.pnlTop.ResumeLayout(false);
            this.pnlForm.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSuppliers)).EndInit();
            this.ResumeLayout(false);

        }
    }
}

