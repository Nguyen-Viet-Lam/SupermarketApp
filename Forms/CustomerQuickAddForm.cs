using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Sunny.UI;
using Microsoft.EntityFrameworkCore;
using SupermarketApp.Data;
using SupermarketApp.Data.Models;
using SupermarketApp.Utils;

namespace SupermarketApp.Forms
{
    public partial class CustomerQuickAddForm : Form
    {
        private UIPanel pnlTop;
        private UIPanel pnlForm;
        private UILabel lblTitle;
        private UILabel lblName;
        private UILabel lblPhone;
        private UILabel lblEmail;
        private UILabel lblAddress;
        private UILabel lblCategory;
        private UITextBox txtName;
        private UITextBox txtPhone;
        private UITextBox txtEmail;
        private UITextBox txtAddress;
        private UIComboBox cbCategory;
        private UIButton btnOk;
        private UIButton btnCancel;

        public KhachHang CreatedCustomer { get; private set; }

        public CustomerQuickAddForm()
        {
            InitializeComponent();
            this.Load += (s, e) => LoadCategoryOptions();
        }

        private void InitializeComponent()
        {
            this.pnlTop = new Sunny.UI.UIPanel();
            this.lblTitle = new Sunny.UI.UILabel();
            this.pnlForm = new Sunny.UI.UIPanel();
            this.lblName = new Sunny.UI.UILabel();
            this.txtName = new Sunny.UI.UITextBox();
            this.lblPhone = new Sunny.UI.UILabel();
            this.txtPhone = new Sunny.UI.UITextBox();
            this.lblEmail = new Sunny.UI.UILabel();
            this.txtEmail = new Sunny.UI.UITextBox();
            this.lblAddress = new Sunny.UI.UILabel();
            this.txtAddress = new Sunny.UI.UITextBox();
            this.lblCategory = new Sunny.UI.UILabel();
            this.cbCategory = new Sunny.UI.UIComboBox();
            this.btnOk = new Sunny.UI.UIButton();
            this.btnCancel = new Sunny.UI.UIButton();
            this.pnlTop.SuspendLayout();
            this.pnlForm.SuspendLayout();
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
            this.pnlTop.Size = new System.Drawing.Size(576, 60);
            this.pnlTop.TabIndex = 1;
            this.pnlTop.Text = null;
            this.pnlTop.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(197)))), ((int)(((byte)(94)))));
            this.lblTitle.Location = new System.Drawing.Point(15, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(380, 35);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "➕ THÊM KHÁCH HÀNG";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlForm
            // 
            this.pnlForm.Controls.Add(this.lblName);
            this.pnlForm.Controls.Add(this.txtName);
            this.pnlForm.Controls.Add(this.lblPhone);
            this.pnlForm.Controls.Add(this.txtPhone);
            this.pnlForm.Controls.Add(this.lblEmail);
            this.pnlForm.Controls.Add(this.txtEmail);
            this.pnlForm.Controls.Add(this.lblAddress);
            this.pnlForm.Controls.Add(this.txtAddress);
            this.pnlForm.Controls.Add(this.lblCategory);
            this.pnlForm.Controls.Add(this.cbCategory);
            this.pnlForm.Controls.Add(this.btnOk);
            this.pnlForm.Controls.Add(this.btnCancel);
            this.pnlForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlForm.FillColor = System.Drawing.Color.White;
            this.pnlForm.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pnlForm.Location = new System.Drawing.Point(0, 60);
            this.pnlForm.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlForm.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlForm.Name = "pnlForm";
            this.pnlForm.Padding = new System.Windows.Forms.Padding(15);
            this.pnlForm.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.pnlForm.Size = new System.Drawing.Size(576, 364);
            this.pnlForm.TabIndex = 0;
            this.pnlForm.Text = null;
            this.pnlForm.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblName
            // 
            this.lblName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblName.Location = new System.Drawing.Point(20, 20);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(140, 25);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Tên KH";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtName
            // 
            this.txtName.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.txtName.Location = new System.Drawing.Point(180, 15);
            this.txtName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtName.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtName.Name = "txtName";
            this.txtName.Padding = new System.Windows.Forms.Padding(5);
            this.txtName.ShowText = false;
            this.txtName.Size = new System.Drawing.Size(300, 35);
            this.txtName.TabIndex = 1;
            this.txtName.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtName.Watermark = "Nhập tên khách hàng...";
            // 
            // lblPhone
            // 
            this.lblPhone.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblPhone.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblPhone.Location = new System.Drawing.Point(20, 65);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(140, 25);
            this.lblPhone.TabIndex = 2;
            this.lblPhone.Text = "Số điện thoại";
            this.lblPhone.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtPhone
            // 
            this.txtPhone.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtPhone.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.txtPhone.Location = new System.Drawing.Point(180, 60);
            this.txtPhone.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtPhone.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Padding = new System.Windows.Forms.Padding(5);
            this.txtPhone.ShowText = false;
            this.txtPhone.Size = new System.Drawing.Size(300, 35);
            this.txtPhone.TabIndex = 3;
            this.txtPhone.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtPhone.Watermark = "0xxxxxxxxx";
            // 
            // lblEmail
            // 
            this.lblEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblEmail.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblEmail.Location = new System.Drawing.Point(20, 110);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(140, 25);
            this.lblEmail.TabIndex = 4;
            this.lblEmail.Text = "Email";
            this.lblEmail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtEmail
            // 
            this.txtEmail.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.txtEmail.Location = new System.Drawing.Point(180, 105);
            this.txtEmail.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtEmail.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Padding = new System.Windows.Forms.Padding(5);
            this.txtEmail.ShowText = false;
            this.txtEmail.Size = new System.Drawing.Size(300, 35);
            this.txtEmail.TabIndex = 5;
            this.txtEmail.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtEmail.Watermark = "example@email.com";
            // 
            // lblAddress
            // 
            this.lblAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblAddress.Location = new System.Drawing.Point(20, 155);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(140, 25);
            this.lblAddress.TabIndex = 6;
            this.lblAddress.Text = "Địa chỉ";
            this.lblAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtAddress
            // 
            this.txtAddress.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.txtAddress.Location = new System.Drawing.Point(180, 150);
            this.txtAddress.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtAddress.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Padding = new System.Windows.Forms.Padding(5);
            this.txtAddress.ShowText = false;
            this.txtAddress.Size = new System.Drawing.Size(300, 35);
            this.txtAddress.TabIndex = 7;
            this.txtAddress.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtAddress.Watermark = "Địa chỉ khách hàng...";
            // 
            // lblCategory
            // 
            this.lblCategory.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblCategory.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblCategory.Location = new System.Drawing.Point(20, 200);
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Size = new System.Drawing.Size(140, 25);
            this.lblCategory.TabIndex = 8;
            this.lblCategory.Text = "Loại KH";
            this.lblCategory.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbCategory
            // 
            this.cbCategory.DataSource = null;
            this.cbCategory.FillColor = System.Drawing.Color.White;
            this.cbCategory.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.cbCategory.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.cbCategory.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.cbCategory.Location = new System.Drawing.Point(180, 195);
            this.cbCategory.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbCategory.MinimumSize = new System.Drawing.Size(1, 16);
            this.cbCategory.Name = "cbCategory";
            this.cbCategory.Padding = new System.Windows.Forms.Padding(5, 5, 30, 5);
            this.cbCategory.Size = new System.Drawing.Size(300, 35);
            this.cbCategory.SymbolSize = 24;
            this.cbCategory.TabIndex = 14;
            this.cbCategory.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.cbCategory.Watermark = "Chọn loại";
            // 
            // btnOk
            // 
            this.btnOk.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOk.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(197)))), ((int)(((byte)(94)))));
            this.btnOk.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(74)))));
            this.btnOk.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(74)))));
            this.btnOk.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(128)))), ((int)(((byte)(61)))));
            this.btnOk.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnOk.Location = new System.Drawing.Point(280, 280);
            this.btnOk.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(110, 40);
            this.btnOk.TabIndex = 20;
            this.btnOk.Text = "Lưu";
            this.btnOk.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnOk.Click += new System.EventHandler(this.BtnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnCancel.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.btnCancel.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.btnCancel.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnCancel.Location = new System.Drawing.Point(400, 280);
            this.btnCancel.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 40);
            this.btnCancel.TabIndex = 21;
            this.btnCancel.Text = "Hủy";
            this.btnCancel.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // CustomerQuickAddForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.ClientSize = new System.Drawing.Size(576, 424);
            this.Controls.Add(this.pnlForm);
            this.Controls.Add(this.pnlTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CustomerQuickAddForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Thêm khách hàng";
            this.pnlTop.ResumeLayout(false);
            this.pnlForm.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void LoadCategoryOptions()
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;
            try
            {
                using (var db = new SupermarketContext())
                {
                    var val = db.CaiDat
                        .AsNoTracking()
                        .Where(x => x.TenCaiDat == "CustomerCategories")
                        .Select(x => x.GiaTri)
                        .FirstOrDefault();

                    var options = (val ?? "").Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim())
                        .Where(s => s.Length > 0)
                        .Distinct(StringComparer.CurrentCultureIgnoreCase)
                        .ToList();

                    if (options.Count == 0)
                    {
                        options = new System.Collections.Generic.List<string> { "Khách vãng lai", "Khách thân quen", "VIP" };
                    }

                    cbCategory.Items.Clear();
                    foreach (var opt in options)
                        cbCategory.Items.Add(opt);
                    if (cbCategory.Items.Count > 0)
                        cbCategory.SelectedIndex = 0;
                }
            }
            catch
            {
                cbCategory.Items.Clear();
                cbCategory.Items.Add("Khách vãng lai");
                cbCategory.Items.Add("Khách thân quen");
                cbCategory.Items.Add("VIP");
                cbCategory.SelectedIndex = 0;
            }
        }

        private void BtnOk_Click(object sender, EventArgs e)
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
            if (!string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                var phone = txtPhone.Text.Trim();
                if (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^[0-9]{10,11}$"))
                {
                    MessageHelper.ShowWarning("Số điện thoại không hợp lệ! (10-11 số)");
                    txtPhone.Focus();
                    return;
                }
            }
            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                var email = txtEmail.Text.Trim();
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
                    // Duplicate phone check
                    if (!string.IsNullOrWhiteSpace(txtPhone.Text))
                    {
                        var phone = txtPhone.Text.Trim();
                        var exists = db.KhachHang.AsNoTracking().Any(x => x.SDT == phone);
                        if (exists)
                        {
                            MessageHelper.ShowWarning("Số điện thoại đã tồn tại!");
                            txtPhone.Focus();
                            return;
                        }
                    }

                    var entity = new KhachHang
                    {
                        TenKH = txtName.Text.Trim(),
                        SDT = string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text.Trim(),
                        Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                        DiaChi = string.IsNullOrWhiteSpace(txtAddress.Text) ? null : txtAddress.Text.Trim(),
                        LoaiKhachHang = string.IsNullOrWhiteSpace(cbCategory.Text) ? "Khách vãng lai" : cbCategory.Text.Trim(),
                        NgayTao = DateTime.Now,
                        DiemTichLuy = 0
                    };
                    db.KhachHang.Add(entity);
                    db.SaveChanges();

                    this.CreatedCustomer = entity;
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("Không thể lưu khách hàng: " + ex.Message);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}