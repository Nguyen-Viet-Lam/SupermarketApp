using System;
using System.Collections.Generic;
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
    public partial class PurchaseOrderForm : Form
    {
        private UIPanel pnlTop;
        private UILabel lblTitle;
        private UILabel lblSupplier;
        private UILabel lblProduct;
        private UILabel lblQuantity;
        private UILabel lblPrice;
        private UIComboBox cbSupplier;
        private UIComboBox cbProduct;
        private UIIntegerUpDown numQuantity;
        private UITextBox txtPrice;
        private UIButton btnAddToCart;
        private UIButton btnClear;
        private UIDataGridView dgvCart;
        private UIPanel pnlBottom;
        private UILabel lblTotal;
        private UILabel lblTotalValue;
        private UIButton btnSave;
        private UIButton btnRemove;

        private List<PurchaseItem> cart = new List<PurchaseItem>();
        private int currentEmployeeId = 1; // Sẽ lấy từ session thực tế

        public class PurchaseItem
        {
            public int MaSP { get; set; }
            public string TenSP { get; set; }
            public int SoLuong { get; set; }
            public decimal DonGiaNhap { get; set; }
            public decimal ThanhTien => SoLuong * DonGiaNhap;
        }

        public PurchaseOrderForm()
        {
            InitializeComponent();
            this.Load += async (s, e) => await InitAsync();
        }

        private async Task InitAsync()
        {
            await LoadSuppliersAsync();
            await LoadProductsAsync();
        }

        private async Task LoadSuppliersAsync()
        {
            using (var db = new SupermarketContext())
            {
                var suppliers = await db.NhaCungCap
                    .AsNoTracking()
                    .Where(x => x.TrangThai)
                    .Select(x => new { x.MaNCC, x.TenNCC })
                    .ToListAsync();

                cbSupplier.DataSource = suppliers;
                cbSupplier.DisplayMember = "TenNCC";
                cbSupplier.ValueMember = "MaNCC";
            }
        }

        private async Task LoadProductsAsync()
        {
            using (var db = new SupermarketContext())
            {
                var products = await db.SanPham
                    .AsNoTracking()
                    .Where(x => x.TrangThai)
                    .Select(x => new { x.MaSP, Display = x.TenSP + " - " + x.DonVi })
                    .ToListAsync();

                cbProduct.DataSource = products;
                cbProduct.DisplayMember = "Display";
                cbProduct.ValueMember = "MaSP";
            }
        }

        private void BtnAddToCart_Click(object sender, EventArgs e)
        {
            if (cbProduct.SelectedValue == null)
            {
                MessageHelper.ShowWarning("Vui lòng chọn sản phẩm!");
                return;
            }

            if (numQuantity.Value <= 0)
            {
                MessageHelper.ShowWarning("Số lượng phải lớn hơn 0!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPrice.Text) || !decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
            {
                MessageHelper.ShowWarning("Đơn giá nhập phải là số dương!");
                txtPrice.Focus();
                return;
            }

            int maSP = Convert.ToInt32(cbProduct.SelectedValue);
            string tenSP = cbProduct.Text;
            int soLuong = numQuantity.Value;

            // Kiểm tra đã có trong giỏ chưa
            var existing = cart.FirstOrDefault(x => x.MaSP == maSP);
            if (existing != null)
            {
                existing.SoLuong += soLuong;
                existing.DonGiaNhap = price; // Cập nhật giá mới nhất
            }
            else
            {
                cart.Add(new PurchaseItem
                {
                    MaSP = maSP,
                    TenSP = tenSP,
                    SoLuong = soLuong,
                    DonGiaNhap = price
                });
            }

            BindCart();
            
            // Reset form
            numQuantity.Value = 1;
            txtPrice.Clear();
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (dgvCart.SelectedRows.Count == 0)
            {
                MessageHelper.ShowWarning("Vui lòng chọn sản phẩm cần xóa!");
                return;
            }

            int maSP = Convert.ToInt32(dgvCart.SelectedRows[0].Cells["MaSP"].Value);
            cart.RemoveAll(x => x.MaSP == maSP);
            BindCart();
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            if (cart.Count > 0 && MessageHelper.ShowAsk("Bạn có chắc muốn xóa toàn bộ giỏ hàng?"))
            {
                cart.Clear();
                BindCart();
            }
        }

        private void BindCart()
        {
            dgvCart.DataSource = null;
            dgvCart.DataSource = cart.Select(x => new
            {
                x.MaSP,
                x.TenSP,
                x.SoLuong,
                DonGiaNhap = x.DonGiaNhap,
                ThanhTien = x.ThanhTien
            }).ToList();

            if (dgvCart.Columns.Count > 0)
            {
                dgvCart.Columns["MaSP"].HeaderText = "Mã SP";
                dgvCart.Columns["MaSP"].Width = 80;
                dgvCart.Columns["TenSP"].HeaderText = "Tên sản phẩm";
                dgvCart.Columns["TenSP"].Width = 250;
                dgvCart.Columns["SoLuong"].HeaderText = "SL";
                dgvCart.Columns["SoLuong"].Width = 70;
                dgvCart.Columns["DonGiaNhap"].HeaderText = "Đơn giá";
                dgvCart.Columns["DonGiaNhap"].Width = 120;
                dgvCart.Columns["DonGiaNhap"].DefaultCellStyle.Format = "N0";
                dgvCart.Columns["ThanhTien"].HeaderText = "Thành tiền";
                dgvCart.Columns["ThanhTien"].Width = 150;
                dgvCart.Columns["ThanhTien"].DefaultCellStyle.Format = "N0";
            }

            decimal total = cart.Sum(x => x.ThanhTien);
            lblTotalValue.Text = $"{total:N0} VNĐ";
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            // Enhanced validation
            if (cbSupplier.SelectedValue == null)
            {
                MessageHelper.ShowWarning("Vui lòng chọn nhà cung cấp!");
                cbSupplier.Focus();
                return;
            }

            if (cart.Count == 0)
            {
                MessageHelper.ShowWarning("Chưa có sản phẩm trong phiếu nhập!");
                return;
            }

            // Validate all quantities and prices > 0
            if (cart.Any(x => x.SoLuong <= 0))
            {
                MessageHelper.ShowWarning("Số lượng phải lớn hơn 0 cho tất cả sản phẩm!");
                return;
            }

            if (cart.Any(x => x.DonGiaNhap <= 0))
            {
                MessageHelper.ShowWarning("Đơn giá nhập phải lớn hơn 0 cho tất cả sản phẩm!");
                return;
            }

            if (!MessageHelper.ShowAsk("Xác nhận lưu phiếu nhập hàng?"))
            {
                return;
            }

            try
            {
                btnSave.Enabled = false;
                btnSave.Text = "Đang lưu...";

                using (var db = new SupermarketContext())
                {
                    using (var transaction = await db.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            // Tạo phiếu nhập với trạng thái "Chờ duyệt"
                            var phieuNhap = new PhieuNhap
                            {
                                NgayNhap = DateTime.Now,
                                MaNCC = Convert.ToInt32(cbSupplier.SelectedValue),
                                MaNV = currentEmployeeId,
                                TongTien = cart.Sum(x => x.ThanhTien),
                                TrangThai = "Chờ duyệt"  // Changed from "Đã duyệt" for approval workflow
                            };
                            db.PhieuNhap.Add(phieuNhap);
                            await db.SaveChangesAsync();

                            // Tạo chi tiết phiếu nhập
                            foreach (var item in cart)
                            {
                                db.CTPhieuNhap.Add(new CTPhieuNhap
                                {
                                    MaPN = phieuNhap.MaPN,
                                    MaSP = item.MaSP,
                                    SoLuong = item.SoLuong,
                                    DonGiaNhap = item.DonGiaNhap
                                });

                                // KHÔNG cộng tồn kho ngay - chỉ cộng khi Admin duyệt
                                // Stock will be updated when status changes to "Đã duyệt"
                            }
                            await db.SaveChangesAsync();

                            await transaction.CommitAsync();

                            string statusMsg = phieuNhap.TrangThai == "Chờ duyệt" ? "\n\n⏳ Trạng thái: Chờ duyệt\nTồn kho sẽ được cập nhật sau khi Admin duyệt." : "";
                            MessageHelper.ShowSuccess($"Đã lưu phiếu nhập #{phieuNhap.MaPN} thành công!\nTổng tiền: {phieuNhap.TongTien:N0} VNĐ{statusMsg}");

                            cart.Clear();
                            BindCart();
                            await InitAsync();
                        }
                        catch
                        {
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("Lỗi: " + ex.Message);
            }
            finally
            {
                btnSave.Enabled = true;
                btnSave.Text = "💾 Lưu phiếu nhập";
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
            this.lblSupplier = new Sunny.UI.UILabel();
            this.cbSupplier = new Sunny.UI.UIComboBox();
            this.lblProduct = new Sunny.UI.UILabel();
            this.cbProduct = new Sunny.UI.UIComboBox();
            this.lblQuantity = new Sunny.UI.UILabel();
            this.numQuantity = new Sunny.UI.UIIntegerUpDown();
            this.lblPrice = new Sunny.UI.UILabel();
            this.txtPrice = new Sunny.UI.UITextBox();
            this.btnAddToCart = new Sunny.UI.UIButton();
            this.btnRemove = new Sunny.UI.UIButton();
            this.btnClear = new Sunny.UI.UIButton();
            this.dgvCart = new Sunny.UI.UIDataGridView();
            this.pnlBottom = new Sunny.UI.UIPanel();
            this.lblTotal = new Sunny.UI.UILabel();
            this.lblTotalValue = new Sunny.UI.UILabel();
            this.btnSave = new Sunny.UI.UIButton();
            this.pnlTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCart)).BeginInit();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.lblTitle);
            this.pnlTop.Controls.Add(this.lblSupplier);
            this.pnlTop.Controls.Add(this.cbSupplier);
            this.pnlTop.Controls.Add(this.lblProduct);
            this.pnlTop.Controls.Add(this.cbProduct);
            this.pnlTop.Controls.Add(this.lblQuantity);
            this.pnlTop.Controls.Add(this.numQuantity);
            this.pnlTop.Controls.Add(this.lblPrice);
            this.pnlTop.Controls.Add(this.txtPrice);
            this.pnlTop.Controls.Add(this.btnAddToCart);
            this.pnlTop.Controls.Add(this.btnRemove);
            this.pnlTop.Controls.Add(this.btnClear);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.FillColor = System.Drawing.Color.White;
            this.pnlTop.FillColor2 = System.Drawing.Color.White;
            this.pnlTop.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlTop.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Padding = new System.Windows.Forms.Padding(10);
            this.pnlTop.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.pnlTop.Size = new System.Drawing.Size(1000, 180);
            this.pnlTop.TabIndex = 0;
            this.pnlTop.Text = null;
            this.pnlTop.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.lblTitle.Location = new System.Drawing.Point(20, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(400, 35);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "📦 LẬP PHIẾU NHẬP HÀNG";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSupplier
            // 
            this.lblSupplier.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblSupplier.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblSupplier.Location = new System.Drawing.Point(20, 60);
            this.lblSupplier.Name = "lblSupplier";
            this.lblSupplier.Size = new System.Drawing.Size(150, 25);
            this.lblSupplier.TabIndex = 1;
            this.lblSupplier.Text = "Nhà cung cấp *";
            this.lblSupplier.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbSupplier
            // 
            this.cbSupplier.DataSource = null;
            this.cbSupplier.FillColor = System.Drawing.Color.White;
            this.cbSupplier.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.cbSupplier.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(254)))));
            this.cbSupplier.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.cbSupplier.Location = new System.Drawing.Point(20, 90);
            this.cbSupplier.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbSupplier.MinimumSize = new System.Drawing.Size(63, 0);
            this.cbSupplier.Name = "cbSupplier";
            this.cbSupplier.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.cbSupplier.Size = new System.Drawing.Size(300, 35);
            this.cbSupplier.SymbolSize = 24;
            this.cbSupplier.TabIndex = 2;
            this.cbSupplier.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.cbSupplier.Watermark = "Chọn nhà cung cấp...";
            // 
            // lblProduct
            // 
            this.lblProduct.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblProduct.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblProduct.Location = new System.Drawing.Point(340, 60);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(150, 25);
            this.lblProduct.TabIndex = 3;
            this.lblProduct.Text = "Sản phẩm *";
            this.lblProduct.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbProduct
            // 
            this.cbProduct.DataSource = null;
            this.cbProduct.FillColor = System.Drawing.Color.White;
            this.cbProduct.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.cbProduct.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(254)))));
            this.cbProduct.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.cbProduct.Location = new System.Drawing.Point(340, 90);
            this.cbProduct.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbProduct.MinimumSize = new System.Drawing.Size(63, 0);
            this.cbProduct.Name = "cbProduct";
            this.cbProduct.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.cbProduct.Size = new System.Drawing.Size(300, 35);
            this.cbProduct.SymbolSize = 24;
            this.cbProduct.TabIndex = 4;
            this.cbProduct.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.cbProduct.Watermark = "Chọn sản phẩm...";
            // 
            // lblQuantity
            // 
            this.lblQuantity.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblQuantity.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblQuantity.Location = new System.Drawing.Point(660, 60);
            this.lblQuantity.Name = "lblQuantity";
            this.lblQuantity.Size = new System.Drawing.Size(100, 25);
            this.lblQuantity.TabIndex = 5;
            this.lblQuantity.Text = "Số lượng *";
            this.lblQuantity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // numQuantity
            // 
            this.numQuantity.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.numQuantity.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.numQuantity.Location = new System.Drawing.Point(660, 90);
            this.numQuantity.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numQuantity.Maximum = 100000D;
            this.numQuantity.Minimum = 1D;
            this.numQuantity.MinimumSize = new System.Drawing.Size(100, 0);
            this.numQuantity.Name = "numQuantity";
            this.numQuantity.Padding = new System.Windows.Forms.Padding(5);
            this.numQuantity.ShowText = false;
            this.numQuantity.Size = new System.Drawing.Size(120, 35);
            this.numQuantity.TabIndex = 6;
            this.numQuantity.Text = "1";
            this.numQuantity.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.numQuantity.Value = 1;
            // 
            // lblPrice
            // 
            this.lblPrice.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblPrice.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblPrice.Location = new System.Drawing.Point(800, 60);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(81, 25);
            this.lblPrice.TabIndex = 7;
            this.lblPrice.Text = "Đơn giá nhập *";
            this.lblPrice.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtPrice
            // 
            this.txtPrice.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtPrice.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.txtPrice.Location = new System.Drawing.Point(800, 90);
            this.txtPrice.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtPrice.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtPrice.Name = "txtPrice";
            this.txtPrice.Padding = new System.Windows.Forms.Padding(5);
            this.txtPrice.ShowText = false;
            this.txtPrice.Size = new System.Drawing.Size(170, 35);
            this.txtPrice.TabIndex = 8;
            this.txtPrice.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtPrice.Watermark = "Nhập đơn giá...";
            // 
            // btnAddToCart
            // 
            this.btnAddToCart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddToCart.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.btnAddToCart.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            this.btnAddToCart.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            this.btnAddToCart.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.btnAddToCart.Location = new System.Drawing.Point(20, 135);
            this.btnAddToCart.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnAddToCart.Name = "btnAddToCart";
            this.btnAddToCart.Size = new System.Drawing.Size(150, 36);
            this.btnAddToCart.TabIndex = 9;
            this.btnAddToCart.Text = "➕ Thêm vào";
            this.btnAddToCart.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnAddToCart.Click += new System.EventHandler(this.BtnAddToCart_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRemove.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnRemove.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.btnRemove.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.btnRemove.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.btnRemove.Location = new System.Drawing.Point(185, 135);
            this.btnRemove.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(130, 36);
            this.btnRemove.TabIndex = 10;
            this.btnRemove.Text = "🗑️ Xóa SP";
            this.btnRemove.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnRemove.Click += new System.EventHandler(this.BtnRemove_Click);
            // 
            // btnClear
            // 
            this.btnClear.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClear.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.btnClear.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.btnClear.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.btnClear.Location = new System.Drawing.Point(330, 135);
            this.btnClear.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(130, 36);
            this.btnClear.TabIndex = 11;
            this.btnClear.Text = "🔄 Xóa tất cả";
            this.btnClear.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnClear.Click += new System.EventHandler(this.BtnClear_Click);
            // 
            // dgvCart
            // 
            this.dgvCart.AllowUserToAddRows = false;
            this.dgvCart.AllowUserToDeleteRows = false;
            this.dgvCart.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(249)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.dgvCart.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvCart.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCart.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(249)))), ((int)(((byte)(255)))));
            this.dgvCart.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCart.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvCart.ColumnHeadersHeight = 38;
            this.dgvCart.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(254)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvCart.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvCart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCart.EnableHeadersVisualStyles = false;
            this.dgvCart.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.dgvCart.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(254)))));
            this.dgvCart.Location = new System.Drawing.Point(0, 180);
            this.dgvCart.MultiSelect = false;
            this.dgvCart.Name = "dgvCart";
            this.dgvCart.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(249)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCart.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvCart.RowHeadersVisible = false;
            this.dgvCart.RowHeadersWidth = 51;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.dgvCart.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvCart.RowTemplate.Height = 32;
            this.dgvCart.SelectedIndex = -1;
            this.dgvCart.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCart.Size = new System.Drawing.Size(1000, 340);
            this.dgvCart.TabIndex = 1;
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.lblTotal);
            this.pnlBottom.Controls.Add(this.lblTotalValue);
            this.pnlBottom.Controls.Add(this.btnSave);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.FillColor = System.Drawing.Color.White;
            this.pnlBottom.FillColor2 = System.Drawing.Color.White;
            this.pnlBottom.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pnlBottom.Location = new System.Drawing.Point(0, 520);
            this.pnlBottom.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlBottom.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Padding = new System.Windows.Forms.Padding(10);
            this.pnlBottom.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.pnlBottom.Size = new System.Drawing.Size(1000, 80);
            this.pnlBottom.TabIndex = 2;
            this.pnlBottom.Text = null;
            this.pnlBottom.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTotal
            // 
            this.lblTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.lblTotal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblTotal.Location = new System.Drawing.Point(20, 20);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(150, 40);
            this.lblTotal.TabIndex = 0;
            this.lblTotal.Text = "TỔNG TIỀN:";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTotalValue
            // 
            this.lblTotalValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.lblTotalValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.lblTotalValue.Location = new System.Drawing.Point(170, 20);
            this.lblTotalValue.Name = "lblTotalValue";
            this.lblTotalValue.Size = new System.Drawing.Size(300, 40);
            this.lblTotalValue.TabIndex = 1;
            this.lblTotalValue.Text = "0 VNĐ";
            this.lblTotalValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnSave
            // 
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnSave.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnSave.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold);
            this.btnSave.Location = new System.Drawing.Point(762, 20);
            this.btnSave.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(218, 45);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "💾 Lưu phiếu nhập";
            this.btnSave.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // PurchaseOrderForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.Controls.Add(this.dgvCart);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlTop);
            this.Name = "PurchaseOrderForm";
            this.Text = "Lập phiếu nhập hàng";
            this.pnlTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCart)).EndInit();
            this.pnlBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}

