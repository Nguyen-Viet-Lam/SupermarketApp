using System;
using System.Drawing;
using System.Windows.Forms;
using Sunny.UI;
using SupermarketApp.Utils;
using SupermarketApp.Data.Models;

namespace SupermarketApp.Forms
{
    public partial class ProductDialog : UIForm
    {
        private UILabel lblTitle;
        private UILabel lblTenSP;
        private UILabel lblDonGia;
        private UILabel lblSoLuong;
        private UILabel lblDonVi;
        private UILabel lblLoaiSP;
        private UILabel lblBarcode;
        private UILabel lblMoTa;
        private UITextBox txtTenSP;
        private UITextBox txtDonGia;
        private UIIntegerUpDown numSoLuong;
        private UIComboBox cbDonVi;
        private UIComboBox cbLoaiSP;
        private UITextBox txtBarcode;
        private UITextBox txtMoTa;
        private UIButton btnSave;
        private UIButton btnCancel;
        
        public SanPham Product { get; private set; }
        public bool IsEditMode { get; private set; }

        public ProductDialog(SanPham product = null)
        {
            IsEditMode = product != null;
            Product = product ?? new SanPham 
            { 
                TrangThai = true, 
                NgayTao = DateTime.Now,
                SoLuong = 0
            };
            
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            if (IsEditMode)
            {
                lblTitle.Text = "✏️ SỬA SẢN PHẨM";
                txtTenSP.Text = Product.TenSP;
                txtDonGia.Text = Product.DonGia.ToString();
                numSoLuong.Value = Product.SoLuong;
                cbDonVi.Text = Product.DonVi ?? "";
                cbLoaiSP.Text = Product.LoaiSP ?? "";
                txtBarcode.Text = Product.Barcode;
                txtMoTa.Text = Product.MoTa;
            }
            else
            {
                lblTitle.Text = "➕ THÊM SẢN PHẨM MỚI";
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtTenSP.Text))
            {
                MessageHelper.ShowWarning("Vui lòng nhập tên sản phẩm!");
                txtTenSP.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDonGia.Text) || !decimal.TryParse(txtDonGia.Text, out decimal donGia) || donGia <= 0)
            {
                MessageHelper.ShowWarning("Đơn giá phải là số dương!");
                txtDonGia.Focus();
                return;
            }

            if (numSoLuong.Value < 0)
            {
                MessageHelper.ShowWarning("Số lượng không được âm!");
                return;
            }

            // Cập nhật dữ liệu
            Product.TenSP = txtTenSP.Text.Trim();
            Product.DonGia = donGia;
            Product.SoLuong = numSoLuong.Value;
            Product.DonVi = cbDonVi.Text?.Trim();
            Product.LoaiSP = cbLoaiSP.Text?.Trim();
            Product.Barcode = txtBarcode.Text?.Trim();
            Product.MoTa = txtMoTa.Text?.Trim();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Sunny.UI.UILabel();
            this.lblTenSP = new Sunny.UI.UILabel();
            this.txtTenSP = new Sunny.UI.UITextBox();
            this.lblDonGia = new Sunny.UI.UILabel();
            this.txtDonGia = new Sunny.UI.UITextBox();
            this.lblSoLuong = new Sunny.UI.UILabel();
            this.numSoLuong = new Sunny.UI.UIIntegerUpDown();
            this.lblDonVi = new Sunny.UI.UILabel();
            this.cbDonVi = new Sunny.UI.UIComboBox();
            this.lblLoaiSP = new Sunny.UI.UILabel();
            this.cbLoaiSP = new Sunny.UI.UIComboBox();
            this.lblBarcode = new Sunny.UI.UILabel();
            this.txtBarcode = new Sunny.UI.UITextBox();
            this.lblMoTa = new Sunny.UI.UILabel();
            this.txtMoTa = new Sunny.UI.UITextBox();
            this.btnSave = new Sunny.UI.UIButton();
            this.btnCancel = new Sunny.UI.UIButton();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.lblTitle.Location = new System.Drawing.Point(20, 45);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(560, 35);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "➕ THÊM SẢN PHẨM MỚI";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTenSP
            // 
            this.lblTenSP.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblTenSP.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblTenSP.Location = new System.Drawing.Point(30, 95);
            this.lblTenSP.Name = "lblTenSP";
            this.lblTenSP.Size = new System.Drawing.Size(150, 25);
            this.lblTenSP.TabIndex = 1;
            this.lblTenSP.Text = "Tên sản phẩm *";
            this.lblTenSP.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtTenSP
            // 
            this.txtTenSP.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtTenSP.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.txtTenSP.Location = new System.Drawing.Point(30, 125);
            this.txtTenSP.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtTenSP.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtTenSP.Name = "txtTenSP";
            this.txtTenSP.Padding = new System.Windows.Forms.Padding(5);
            this.txtTenSP.ShowText = false;
            this.txtTenSP.Size = new System.Drawing.Size(540, 35);
            this.txtTenSP.TabIndex = 2;
            this.txtTenSP.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtTenSP.Watermark = "Nhập tên sản phẩm...";
            // 
            // lblDonGia
            // 
            this.lblDonGia.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblDonGia.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblDonGia.Location = new System.Drawing.Point(30, 175);
            this.lblDonGia.Name = "lblDonGia";
            this.lblDonGia.Size = new System.Drawing.Size(150, 25);
            this.lblDonGia.TabIndex = 3;
            this.lblDonGia.Text = "Đơn giá *";
            this.lblDonGia.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtDonGia
            // 
            this.txtDonGia.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtDonGia.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.txtDonGia.Location = new System.Drawing.Point(30, 205);
            this.txtDonGia.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtDonGia.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtDonGia.Name = "txtDonGia";
            this.txtDonGia.Padding = new System.Windows.Forms.Padding(5);
            this.txtDonGia.ShowText = false;
            this.txtDonGia.Size = new System.Drawing.Size(170, 35);
            this.txtDonGia.TabIndex = 4;
            this.txtDonGia.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtDonGia.Watermark = "0";
            // 
            // lblSoLuong
            // 
            this.lblSoLuong.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblSoLuong.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblSoLuong.Location = new System.Drawing.Point(220, 175);
            this.lblSoLuong.Name = "lblSoLuong";
            this.lblSoLuong.Size = new System.Drawing.Size(100, 25);
            this.lblSoLuong.TabIndex = 5;
            this.lblSoLuong.Text = "Số lượng";
            this.lblSoLuong.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // numSoLuong
            // 
            this.numSoLuong.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.numSoLuong.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.numSoLuong.Location = new System.Drawing.Point(220, 205);
            this.numSoLuong.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numSoLuong.Maximum = 100000D;
            this.numSoLuong.Minimum = 0D;
            this.numSoLuong.MinimumSize = new System.Drawing.Size(100, 0);
            this.numSoLuong.Name = "numSoLuong";
            this.numSoLuong.Padding = new System.Windows.Forms.Padding(5);
            this.numSoLuong.ShowText = false;
            this.numSoLuong.Size = new System.Drawing.Size(130, 35);
            this.numSoLuong.TabIndex = 6;
            this.numSoLuong.Text = "0";
            this.numSoLuong.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDonVi
            // 
            this.lblDonVi.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblDonVi.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblDonVi.Location = new System.Drawing.Point(370, 175);
            this.lblDonVi.Name = "lblDonVi";
            this.lblDonVi.Size = new System.Drawing.Size(100, 25);
            this.lblDonVi.TabIndex = 7;
            this.lblDonVi.Text = "Đơn vị";
            this.lblDonVi.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbDonVi
            // 
            this.cbDonVi.DataSource = null;
            this.cbDonVi.FillColor = System.Drawing.Color.White;
            this.cbDonVi.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.cbDonVi.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(254)))));
            this.cbDonVi.Items.AddRange(new object[] {
            "Cái",
            "Hộp",
            "Thùng",
            "Kg",
            "Lít",
            "Gói",
            "Chai",
            "Lon"});
            this.cbDonVi.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.cbDonVi.Location = new System.Drawing.Point(370, 205);
            this.cbDonVi.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbDonVi.MinimumSize = new System.Drawing.Size(63, 0);
            this.cbDonVi.Name = "cbDonVi";
            this.cbDonVi.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.cbDonVi.Size = new System.Drawing.Size(200, 35);
            this.cbDonVi.SymbolSize = 24;
            this.cbDonVi.TabIndex = 8;
            this.cbDonVi.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.cbDonVi.Watermark = "Chọn đơn vị...";
            // 
            // lblLoaiSP
            // 
            this.lblLoaiSP.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblLoaiSP.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblLoaiSP.Location = new System.Drawing.Point(30, 255);
            this.lblLoaiSP.Name = "lblLoaiSP";
            this.lblLoaiSP.Size = new System.Drawing.Size(150, 25);
            this.lblLoaiSP.TabIndex = 9;
            this.lblLoaiSP.Text = "Loại sản phẩm";
            this.lblLoaiSP.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbLoaiSP
            // 
            this.cbLoaiSP.DataSource = null;
            this.cbLoaiSP.FillColor = System.Drawing.Color.White;
            this.cbLoaiSP.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.cbLoaiSP.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(254)))));
            this.cbLoaiSP.Items.AddRange(new object[] {
            "Thực phẩm tươi sống",
            "Đồ uống",
            "Bánh kẹo",
            "Gia vị",
            "Đồ dùng gia đình",
            "Sản phẩm chăm sóc cá nhân",
            "Đồ gia dụng",
            "Khác"});
            this.cbLoaiSP.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.cbLoaiSP.Location = new System.Drawing.Point(30, 285);
            this.cbLoaiSP.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbLoaiSP.MinimumSize = new System.Drawing.Size(63, 0);
            this.cbLoaiSP.Name = "cbLoaiSP";
            this.cbLoaiSP.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.cbLoaiSP.Size = new System.Drawing.Size(260, 35);
            this.cbLoaiSP.SymbolSize = 24;
            this.cbLoaiSP.TabIndex = 10;
            this.cbLoaiSP.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.cbLoaiSP.Watermark = "Chọn loại...";
            // 
            // lblBarcode
            // 
            this.lblBarcode.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblBarcode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblBarcode.Location = new System.Drawing.Point(310, 255);
            this.lblBarcode.Name = "lblBarcode";
            this.lblBarcode.Size = new System.Drawing.Size(100, 25);
            this.lblBarcode.TabIndex = 11;
            this.lblBarcode.Text = "Barcode";
            this.lblBarcode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtBarcode
            // 
            this.txtBarcode.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtBarcode.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.txtBarcode.Location = new System.Drawing.Point(310, 285);
            this.txtBarcode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtBarcode.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtBarcode.Name = "txtBarcode";
            this.txtBarcode.Padding = new System.Windows.Forms.Padding(5);
            this.txtBarcode.ShowText = false;
            this.txtBarcode.Size = new System.Drawing.Size(260, 35);
            this.txtBarcode.TabIndex = 12;
            this.txtBarcode.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtBarcode.Watermark = "Mã vạch...";
            // 
            // lblMoTa
            // 
            this.lblMoTa.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblMoTa.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblMoTa.Location = new System.Drawing.Point(30, 335);
            this.lblMoTa.Name = "lblMoTa";
            this.lblMoTa.Size = new System.Drawing.Size(100, 25);
            this.lblMoTa.TabIndex = 13;
            this.lblMoTa.Text = "Mô tả";
            this.lblMoTa.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtMoTa
            // 
            this.txtMoTa.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtMoTa.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.txtMoTa.Location = new System.Drawing.Point(30, 365);
            this.txtMoTa.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtMoTa.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtMoTa.Multiline = true;
            this.txtMoTa.Name = "txtMoTa";
            this.txtMoTa.Padding = new System.Windows.Forms.Padding(5);
            this.txtMoTa.ShowText = false;
            this.txtMoTa.Size = new System.Drawing.Size(540, 80);
            this.txtMoTa.TabIndex = 14;
            this.txtMoTa.TextAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.txtMoTa.Watermark = "Nhập mô tả sản phẩm...";
            // 
            // btnSave
            // 
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnSave.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnSave.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnSave.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(78)))), ((int)(((byte)(216)))));
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnSave.Location = new System.Drawing.Point(330, 470);
            this.btnSave.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(120, 40);
            this.btnSave.TabIndex = 15;
            this.btnSave.Text = "💾 Lưu";
            this.btnSave.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.btnCancel.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.btnCancel.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.btnCancel.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnCancel.Location = new System.Drawing.Point(460, 470);
            this.btnCancel.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(110, 40);
            this.btnCancel.TabIndex = 16;
            this.btnCancel.Text = "❌ Hủy";
            this.btnCancel.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // ProductDialog
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(753, 540);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtMoTa);
            this.Controls.Add(this.lblMoTa);
            this.Controls.Add(this.txtBarcode);
            this.Controls.Add(this.lblBarcode);
            this.Controls.Add(this.cbLoaiSP);
            this.Controls.Add(this.lblLoaiSP);
            this.Controls.Add(this.cbDonVi);
            this.Controls.Add(this.lblDonVi);
            this.Controls.Add(this.numSoLuong);
            this.Controls.Add(this.lblSoLuong);
            this.Controls.Add(this.txtDonGia);
            this.Controls.Add(this.lblDonGia);
            this.Controls.Add(this.txtTenSP);
            this.Controls.Add(this.lblTenSP);
            this.Controls.Add(this.lblTitle);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProductDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Sản phẩm";
            this.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.ZoomScaleRect = new System.Drawing.Rectangle(19, 19, 600, 540);
            this.ResumeLayout(false);

        }
    }
}

