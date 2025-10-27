using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sunny.UI;
using Microsoft.EntityFrameworkCore;
using SupermarketApp.Utils;
using SupermarketApp.Data;
using SupermarketApp.Data.Models;
using SupermarketApp.Services;

namespace SupermarketApp.Forms
{
    public partial class InvoiceForm : Form
    {
        private UIPanel pnlTop;
        private UIPanel pnlBottom;
        private UILabel lblTitle;
        private UILabel lblProduct;
        private UILabel lblQuantity;
        private UIComboBox cbProduct;
        private UIComboBox cbCustomer;
        private UIButton btnAddCustomer;
        private UILabel lblCustomer;
        private UIIntegerUpDown numQty;
        private UIButton btnAddToCart;
        private UIButton btnRemove;
        private UIButton btnClear;
        private UIDataGridView dgvCart;
        private UILabel lblTotal;
        private UILabel lblTotalValue;
        private UILabel lblCustomerPay;
        private UITextBox txtCustomerPay;
        private UILabel lblChange;
        private UILabel lblChangeValue;
        private UIButton btnSave;
        private readonly List<InvoiceService.InvoiceItemDto> cart = new List<InvoiceService.InvoiceItemDto>();
        private List<SanPham> products;
        private List<KhachHang> customers;

        public int MaNVCurrent { get; set; } = 1; 
        public int? MaKHCurrent { get; set; } = null; 

        public InvoiceForm()
        {
            InitializeComponent();
            this.Load += async (s,e)=> await InitAsync();
        }

        private async Task InitAsync()
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;
            using (var db = new SupermarketContext())
            {
                products = await db.SanPham
                    .AsNoTracking()
                    .Where(x => x.SoLuong > 0)
                    .OrderBy(x => x.TenSP)
                    .ToListAsync();

                customers = await db.KhachHang
                    .AsNoTracking()
                    .OrderBy(x => x.TenKH)
                    .ToListAsync();
            }
            
            // Bind sản phẩm
            cbProduct.Items.Clear();
            cbProduct.DataSource = null;
            var displayList = products.Select(p =>
                $"{p.TenSP} - Giá: {p.DonGia:N0} - Tồn: {p.SoLuong}"
            ).ToList();
            cbProduct.DataSource = displayList;
            cbProduct.SelectedIndex = products.Count > 0 ? 0 : -1;

            // Bind khách hàng
            cbCustomer.Items.Clear();
            cbCustomer.DataSource = null;
            var customerDisplay = customers.Select(c =>
                string.IsNullOrWhiteSpace(c.SDT) ? $"{c.TenKH}" : $"{c.TenKH} - {c.SDT}"
            ).ToList();
            cbCustomer.DataSource = customerDisplay;
            cbCustomer.SelectedIndex = -1; // mặc định không chọn khách
            MaKHCurrent = null;

            // Gán sự kiện chọn khách để cập nhật MaKHCurrent
            cbCustomer.SelectedIndexChanged += (s2, e2) =>
            {
                if (cbCustomer.SelectedIndex >= 0 && customers != null && cbCustomer.SelectedIndex < customers.Count)
                    MaKHCurrent = customers[cbCustomer.SelectedIndex].MaKH;
                else
                    MaKHCurrent = null;
            };

            BindCart();
        }

        private void BtnAddToCart_Click(object sender, EventArgs e)
        {
            if (cbProduct.SelectedIndex < 0 || products.Count == 0)
            {
                MessageHelper.ShowWarning("Vui lòng chọn sản phẩm!");
                return;
            }
            
            var p = products[cbProduct.SelectedIndex];
            int sl = numQty.Value;
            
            if (sl <= 0)
            {
                MessageHelper.ShowWarning("Số lượng phải lớn hơn 0!");
                return;
            }
            
            // Check total quantity in cart vs stock
            var existing = cart.FirstOrDefault(x => x.MaSP == p.MaSP);
            int totalInCart = (existing?.SoLuong ?? 0) + sl;
            
            if (totalInCart > p.SoLuong)
            {
                MessageHelper.ShowWarning($"Không đủ tồn kho!\nTồn kho: {p.SoLuong}\nĐã có trong giỏ: {existing?.SoLuong ?? 0}\nYêu cầu thêm: {sl}\n\nChỉ có thể thêm tối đa: {p.SoLuong - (existing?.SoLuong ?? 0)}");
                return;
            }
            
            if (existing == null)
            {
                cart.Add(new InvoiceService.InvoiceItemDto 
                { 
                    MaSP = p.MaSP, 
                    TenSP = p.TenSP, 
                    SoLuong = sl, 
                    DonGiaBan = p.DonGia 
                });
                MessageHelper.ShowTipSuccess($"Đã thêm {p.TenSP} vào giỏ hàng!");
            }
            else
            {
                existing.SoLuong += sl;
                MessageHelper.ShowTipSuccess($"Đã cập nhật số lượng!");
            }
            
            BindCart();
            numQty.Value = 1;
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (dgvCart.SelectedRows.Count == 0 || cart.Count == 0)
            {
                MessageHelper.ShowWarning("Vui lòng chọn sản phẩm cần xóa!");
                return;
            }
            
            var maSP = Convert.ToInt32(dgvCart.SelectedRows[0].Cells["MaSP"].Value);
            var item = cart.FirstOrDefault(x => x.MaSP == maSP);
            
            if (item != null && MessageHelper.ShowAsk($"Xóa {item.TenSP} khỏi giỏ hàng?"))
            {
                cart.Remove(item);
                BindCart();
                MessageHelper.ShowTipSuccess("Đã xóa khỏi giỏ hàng!");
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            if (cart.Count == 0) return;
            
            if (MessageHelper.ShowAsk("Xóa tất cả sản phẩm trong giỏ hàng?"))
            {
                cart.Clear();
                BindCart();
                MessageHelper.ShowTipSuccess("Đã xóa tất cả!");
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
                DonGia = x.DonGiaBan, 
                x.ThanhTien 
            }).ToList();
            
            if (dgvCart.Columns.Count > 0)
            {
                dgvCart.Columns["MaSP"].HeaderText = "Mã SP";
                dgvCart.Columns["MaSP"].Width = 80;
                dgvCart.Columns["TenSP"].HeaderText = "Tên sản phẩm";
                dgvCart.Columns["TenSP"].Width = 280;
                dgvCart.Columns["SoLuong"].HeaderText = "SL";
                dgvCart.Columns["SoLuong"].Width = 80;
                dgvCart.Columns["DonGia"].HeaderText = "Đơn giá";
                dgvCart.Columns["DonGia"].Width = 120;
                dgvCart.Columns["DonGia"].DefaultCellStyle.Format = "N0";
                dgvCart.Columns["ThanhTien"].HeaderText = "Thành tiền";
                dgvCart.Columns["ThanhTien"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvCart.Columns["ThanhTien"].DefaultCellStyle.Format = "N0";
            }
            
            decimal tong = cart.Sum(x => x.ThanhTien);
            lblTotalValue.Text = $"{tong:N0} VNĐ";
            CalculateChange();
        }
        
        private void TxtCustomerPay_TextChanged(object sender, EventArgs e)
        {
            CalculateChange();
        }

        private void CalculateChange()
        {
            try
            {
                decimal total = cart.Sum(x => x.ThanhTien);
                if (decimal.TryParse(txtCustomerPay.Text, out decimal customerPay))
                {
                    decimal change = customerPay - total;
                    lblChangeValue.Text = $"{change:N0} VNĐ";
                    lblChangeValue.ForeColor = change >= 0 ? Color.FromArgb(16, 185, 129) : Color.FromArgb(239, 68, 68);
                }
                else
                {
                    lblChangeValue.Text = "0 VNĐ";
                    lblChangeValue.ForeColor = Color.FromArgb(107, 114, 128);
                }
            }
            catch
            {
                lblChangeValue.Text = "0 VNĐ";
            }
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            // Enhanced validation
            if (cart.Count == 0)
            {
                MessageHelper.ShowWarning("Chưa có sản phẩm trong giỏ hàng!");
                return;
            }
            
            // Validate all quantities > 0
            if (cart.Any(x => x.SoLuong <= 0))
            {
                MessageHelper.ShowWarning("Số lượng phải lớn hơn 0 cho tất cả sản phẩm!");
                return;
            }
            
            // Validate payment amount
            decimal total = cart.Sum(x => x.ThanhTien);
            if (!string.IsNullOrEmpty(txtCustomerPay.Text))
            {
                if (decimal.TryParse(txtCustomerPay.Text, out decimal customerPay))
                {
                    if (customerPay < total)
                    {
                        MessageHelper.ShowWarning($"Số tiền khách trả ({customerPay:N0} VNĐ) không đủ!\nTổng tiền: {total:N0} VNĐ");
                        txtCustomerPay.Focus();
                        return;
                    }
                }
            }
            
            if (!MessageHelper.ShowAsk("Xác nhận lưu hóa đơn?"))
            {
                return;
            }
            
            try
            {
                btnSave.Enabled = false;
                btnSave.Text = "Đang lưu...";
                
                var service = new SupermarketApp.Services.InvoiceService();
                int maHD = await service.CreateInvoiceAsync(MaNVCurrent, MaKHCurrent, cart);
                
                decimal change = 0;
                if (!string.IsNullOrEmpty(txtCustomerPay.Text) && decimal.TryParse(txtCustomerPay.Text, out decimal customerPay))
                {
                    change = customerPay - total;
                }
                
                string successMsg = $"Đã lưu hóa đơn #{maHD} thành công!\n\nTổng tiền: {total:N0} VNĐ";
                if (change > 0 && !string.IsNullOrEmpty(txtCustomerPay.Text) && decimal.TryParse(txtCustomerPay.Text, out decimal customerPayDisplay))
                {
                    successMsg += $"\nKhách trả: {customerPayDisplay:N0} VNĐ\nTiền thối: {change:N0} VNĐ";
                }
                
                // Add customer points info if applicable
                if (MaKHCurrent.HasValue)
                {
                    int points = (int)(total / 10000m);
                    if (points > 0)
                    {
                        successMsg += $"\n\n+{points} điểm tích lũy cho khách hàng!";
                    }
                }
                
                MessageHelper.ShowSuccess(successMsg);
                
                // Hỏi có muốn in hóa đơn không
                if (MessageHelper.ShowAsk("Bạn có muốn in hóa đơn không?"))
                {
                    var printDialog = new InvoicePrintDialog(maHD);
                    printDialog.ShowDialog();
                }
                
                cart.Clear();
                txtCustomerPay.Clear();
                BindCart();
                await InitAsync();
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("Lỗi: " + ex.Message);
            }
            finally
            {
                btnSave.Enabled = true;
                btnSave.Text = "💾 Lưu hóa đơn";
            }
        }

        private void BtnAddCustomer_Click(object sender, EventArgs e)
        {
            using (var dlg = new CustomerQuickAddForm())
            {
                if (dlg.ShowDialog(this) == DialogResult.OK && dlg.CreatedCustomer != null)
                {
                    try
                    {
                        using (var db = new SupermarketContext())
                        {
                            customers = db.KhachHang
                                .AsNoTracking()
                                .OrderBy(x => x.TenKH)
                                .ToList();
                        }

                        var customerDisplay2 = customers.Select(c =>
                            string.IsNullOrWhiteSpace(c.SDT) ? $"{c.TenKH}" : $"{c.TenKH} - {c.SDT}"
                        ).ToList();

                        cbCustomer.DataSource = null;
                        cbCustomer.Items.Clear();
                        cbCustomer.DataSource = customerDisplay2;

                        int idx = customers.FindIndex(c => c.MaKH == dlg.CreatedCustomer.MaKH);
                        if (idx >= 0)
                        {
                            cbCustomer.SelectedIndex = idx;
                            MaKHCurrent = customers[idx].MaKH;
                        }
                        else
                        {
                            cbCustomer.SelectedIndex = -1;
                            MaKHCurrent = null;
                        }

                        MessageHelper.ShowTipSuccess("Đã thêm khách hàng mới!");
                    }
                    catch (Exception ex2)
                    {
                        MessageHelper.ShowError("Không thể tải lại danh sách khách hàng: " + ex2.Message);
                    }
                }
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
            this.btnClear = new Sunny.UI.UIButton();
            this.btnRemove = new Sunny.UI.UIButton();
            this.btnAddToCart = new Sunny.UI.UIButton();
            this.numQty = new Sunny.UI.UIIntegerUpDown();
            this.lblQuantity = new Sunny.UI.UILabel();
            this.cbProduct = new Sunny.UI.UIComboBox();
            this.lblProduct = new Sunny.UI.UILabel();
            this.cbCustomer = new Sunny.UI.UIComboBox();
            this.lblCustomer = new Sunny.UI.UILabel();
            this.lblTitle = new Sunny.UI.UILabel();
            this.btnAddCustomer = new Sunny.UI.UIButton();
            this.dgvCart = new Sunny.UI.UIDataGridView();
            this.pnlBottom = new Sunny.UI.UIPanel();
            this.btnSave = new Sunny.UI.UIButton();
            this.lblChangeValue = new Sunny.UI.UILabel();
            this.lblChange = new Sunny.UI.UILabel();
            this.txtCustomerPay = new Sunny.UI.UITextBox();
            this.lblCustomerPay = new Sunny.UI.UILabel();
            this.lblTotalValue = new Sunny.UI.UILabel();
            this.lblTotal = new Sunny.UI.UILabel();
            this.pnlTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCart)).BeginInit();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.btnClear);
            this.pnlTop.Controls.Add(this.btnRemove);
            this.pnlTop.Controls.Add(this.btnAddToCart);
            this.pnlTop.Controls.Add(this.numQty);
            this.pnlTop.Controls.Add(this.lblQuantity);
            this.pnlTop.Controls.Add(this.cbProduct);
            this.pnlTop.Controls.Add(this.lblProduct);
            this.pnlTop.Controls.Add(this.cbCustomer);
            this.pnlTop.Controls.Add(this.lblCustomer);
            this.pnlTop.Controls.Add(this.lblTitle);
            this.pnlTop.Controls.Add(this.btnAddCustomer);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.FillColor = System.Drawing.Color.White;
            this.pnlTop.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlTop.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Padding = new System.Windows.Forms.Padding(15);
            this.pnlTop.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.pnlTop.Size = new System.Drawing.Size(1000, 130);
            this.pnlTop.TabIndex = 0;
            this.pnlTop.Text = null;
            this.pnlTop.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnClear
            // 
            this.btnClear.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClear.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.btnClear.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.btnClear.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.btnClear.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnClear.Location = new System.Drawing.Point(855, 85);
            this.btnClear.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(130, 35);
            this.btnClear.TabIndex = 7;
            this.btnClear.Text = "🔄 Xóa tất cả";
            this.btnClear.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnClear.Click += new System.EventHandler(this.BtnClear_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRemove.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnRemove.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.btnRemove.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.btnRemove.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.btnRemove.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnRemove.Location = new System.Drawing.Point(725, 85);
            this.btnRemove.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(120, 35);
            this.btnRemove.TabIndex = 6;
            this.btnRemove.Text = "🗑️ Xóa";
            this.btnRemove.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnRemove.Click += new System.EventHandler(this.BtnRemove_Click);
            // 
            // btnAddToCart
            // 
            this.btnAddToCart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddToCart.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(72)))), ((int)(((byte)(153)))));
            this.btnAddToCart.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(39)))), ((int)(((byte)(119)))));
            this.btnAddToCart.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(39)))), ((int)(((byte)(119)))));
            this.btnAddToCart.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(24)))), ((int)(((byte)(93)))));
            this.btnAddToCart.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnAddToCart.Location = new System.Drawing.Point(565, 85);
            this.btnAddToCart.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnAddToCart.Name = "btnAddToCart";
            this.btnAddToCart.Size = new System.Drawing.Size(154, 35);
            this.btnAddToCart.TabIndex = 5;
            this.btnAddToCart.Text = "➕ Thêm vào giỏ";
            this.btnAddToCart.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnAddToCart.Click += new System.EventHandler(this.BtnAddToCart_Click);
            // 
            // numQty
            // 
            this.numQty.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.numQty.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.numQty.Location = new System.Drawing.Point(441, 85);
            this.numQty.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numQty.Maximum = 1000D;
            this.numQty.Minimum = 1D;
            this.numQty.MinimumSize = new System.Drawing.Size(100, 0);
            this.numQty.Name = "numQty";
            this.numQty.Padding = new System.Windows.Forms.Padding(5);
            this.numQty.ShowText = false;
            this.numQty.Size = new System.Drawing.Size(117, 35);
            this.numQty.TabIndex = 4;
            this.numQty.Text = "1";
            this.numQty.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.numQty.Value = 1;
            // 
            // lblQuantity
            // 
            this.lblQuantity.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblQuantity.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblQuantity.Location = new System.Drawing.Point(448, 55);
            this.lblQuantity.Name = "lblQuantity";
            this.lblQuantity.Size = new System.Drawing.Size(110, 25);
            this.lblQuantity.TabIndex = 3;
            this.lblQuantity.Text = "Số lượng";
            this.lblQuantity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbProduct
            // 
            this.cbProduct.DataSource = null;
            this.cbProduct.FillColor = System.Drawing.Color.White;
            this.cbProduct.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.cbProduct.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(207)))), ((int)(((byte)(232)))));
            this.cbProduct.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(72)))), ((int)(((byte)(153)))));
            this.cbProduct.Location = new System.Drawing.Point(13, 85);
            this.cbProduct.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbProduct.MinimumSize = new System.Drawing.Size(63, 0);
            this.cbProduct.Name = "cbProduct";
            this.cbProduct.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.cbProduct.Size = new System.Drawing.Size(420, 35);
            this.cbProduct.SymbolSize = 24;
            this.cbProduct.TabIndex = 2;
            this.cbProduct.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.cbProduct.Watermark = "Chọn sản phẩm...";
            // 
            // lblProduct
            // 
            this.lblProduct.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblProduct.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblProduct.Location = new System.Drawing.Point(15, 55);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(107, 25);
            this.lblProduct.TabIndex = 1;
            this.lblProduct.Text = "Sản phẩm";
            this.lblProduct.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbCustomer
            // 
            this.cbCustomer.DataSource = null;
            this.cbCustomer.FillColor = System.Drawing.Color.White;
            this.cbCustomer.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.cbCustomer.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(207)))), ((int)(((byte)(232)))));
            this.cbCustomer.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(72)))), ((int)(((byte)(153)))));
            this.cbCustomer.Location = new System.Drawing.Point(640, 45);
            this.cbCustomer.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbCustomer.MinimumSize = new System.Drawing.Size(63, 0);
            this.cbCustomer.Name = "cbCustomer";
            this.cbCustomer.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.cbCustomer.Size = new System.Drawing.Size(300, 35);
            this.cbCustomer.SymbolSize = 24;
            this.cbCustomer.TabIndex = 2;
            this.cbCustomer.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.cbCustomer.Watermark = "Chọn khách hàng...";
            // 
            // lblCustomer
            // 
            this.lblCustomer.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblCustomer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblCustomer.Location = new System.Drawing.Point(636, 15);
            this.lblCustomer.Name = "lblCustomer";
            this.lblCustomer.Size = new System.Drawing.Size(120, 25);
            this.lblCustomer.TabIndex = 0;
            this.lblCustomer.Text = "Khách hàng";
            this.lblCustomer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(72)))), ((int)(((byte)(153)))));
            this.lblTitle.Location = new System.Drawing.Point(15, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(285, 35);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "🛒 LẬP HÓA ĐƠN";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnAddCustomer
            // 
            this.btnAddCustomer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddCustomer.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(197)))), ((int)(((byte)(94)))));
            this.btnAddCustomer.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(74)))));
            this.btnAddCustomer.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(74)))));
            this.btnAddCustomer.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(128)))), ((int)(((byte)(61)))));
            this.btnAddCustomer.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.btnAddCustomer.Location = new System.Drawing.Point(945, 45);
            this.btnAddCustomer.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnAddCustomer.Name = "btnAddCustomer";
            this.btnAddCustomer.Size = new System.Drawing.Size(40, 35);
            this.btnAddCustomer.TabIndex = 15;
            this.btnAddCustomer.Text = "➕";
            this.btnAddCustomer.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnAddCustomer.Click += new System.EventHandler(this.BtnAddCustomer_Click);
            // 
            // dgvCart
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(242)))), ((int)(((byte)(248)))));
            this.dgvCart.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvCart.BackgroundColor = System.Drawing.Color.White;
            this.dgvCart.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(72)))), ((int)(((byte)(153)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(72)))), ((int)(((byte)(153)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCart.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvCart.ColumnHeadersHeight = 40;
            this.dgvCart.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(207)))), ((int)(((byte)(232)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvCart.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvCart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCart.EnableHeadersVisualStyles = false;
            this.dgvCart.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.dgvCart.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(207)))), ((int)(((byte)(232)))));
            this.dgvCart.Location = new System.Drawing.Point(0, 130);
            this.dgvCart.Name = "dgvCart";
            this.dgvCart.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(242)))), ((int)(((byte)(248)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(72)))), ((int)(((byte)(153)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCart.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvCart.RowHeadersWidth = 51;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.dgvCart.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvCart.RowTemplate.Height = 35;
            this.dgvCart.SelectedIndex = -1;
            this.dgvCart.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCart.Size = new System.Drawing.Size(1000, 340);
            this.dgvCart.StripeOddColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(242)))), ((int)(((byte)(248)))));
            this.dgvCart.TabIndex = 1;
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.btnSave);
            this.pnlBottom.Controls.Add(this.lblChangeValue);
            this.pnlBottom.Controls.Add(this.lblChange);
            this.pnlBottom.Controls.Add(this.txtCustomerPay);
            this.pnlBottom.Controls.Add(this.lblCustomerPay);
            this.pnlBottom.Controls.Add(this.lblTotalValue);
            this.pnlBottom.Controls.Add(this.lblTotal);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.FillColor = System.Drawing.Color.White;
            this.pnlBottom.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pnlBottom.Location = new System.Drawing.Point(0, 470);
            this.pnlBottom.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlBottom.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Padding = new System.Windows.Forms.Padding(15);
            this.pnlBottom.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.pnlBottom.Size = new System.Drawing.Size(1000, 130);
            this.pnlBottom.TabIndex = 2;
            this.pnlBottom.Text = null;
            this.pnlBottom.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnSave
            // 
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(197)))), ((int)(((byte)(94)))));
            this.btnSave.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(74)))));
            this.btnSave.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(74)))));
            this.btnSave.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(128)))), ((int)(((byte)(61)))));
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.btnSave.Location = new System.Drawing.Point(640, 65);
            this.btnSave.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnSave.Name = "btnSave";
            this.btnSave.Radius = 8;
            this.btnSave.Size = new System.Drawing.Size(345, 42);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "💾 Lưu hóa đơn";
            this.btnSave.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // lblChangeValue
            // 
            this.lblChangeValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold);
            this.lblChangeValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.lblChangeValue.Location = new System.Drawing.Point(425, 68);
            this.lblChangeValue.Name = "lblChangeValue";
            this.lblChangeValue.Size = new System.Drawing.Size(200, 32);
            this.lblChangeValue.TabIndex = 6;
            this.lblChangeValue.Text = "0 VNĐ";
            this.lblChangeValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblChange
            // 
            this.lblChange.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblChange.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblChange.Location = new System.Drawing.Point(320, 70);
            this.lblChange.Name = "lblChange";
            this.lblChange.Size = new System.Drawing.Size(100, 25);
            this.lblChange.TabIndex = 5;
            this.lblChange.Text = "Tiền thối:";
            this.lblChange.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtCustomerPay
            // 
            this.txtCustomerPay.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtCustomerPay.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtCustomerPay.Location = new System.Drawing.Point(120, 68);
            this.txtCustomerPay.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtCustomerPay.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtCustomerPay.Name = "txtCustomerPay";
            this.txtCustomerPay.Padding = new System.Windows.Forms.Padding(5);
            this.txtCustomerPay.ShowText = false;
            this.txtCustomerPay.Size = new System.Drawing.Size(180, 32);
            this.txtCustomerPay.TabIndex = 4;
            this.txtCustomerPay.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
            this.txtCustomerPay.Watermark = "Nhập số tiền...";
            this.txtCustomerPay.TextChanged += new System.EventHandler(this.TxtCustomerPay_TextChanged);
            // 
            // lblCustomerPay
            // 
            this.lblCustomerPay.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblCustomerPay.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblCustomerPay.Location = new System.Drawing.Point(15, 70);
            this.lblCustomerPay.Name = "lblCustomerPay";
            this.lblCustomerPay.Size = new System.Drawing.Size(100, 25);
            this.lblCustomerPay.TabIndex = 3;
            this.lblCustomerPay.Text = "Khách trả:";
            this.lblCustomerPay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTotalValue
            // 
            this.lblTotalValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold);
            this.lblTotalValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(72)))), ((int)(((byte)(153)))));
            this.lblTotalValue.Location = new System.Drawing.Point(171, 20);
            this.lblTotalValue.Name = "lblTotalValue";
            this.lblTotalValue.Size = new System.Drawing.Size(298, 40);
            this.lblTotalValue.TabIndex = 1;
            this.lblTotalValue.Text = "0 VNĐ";
            this.lblTotalValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTotal
            // 
            this.lblTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.lblTotal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblTotal.Location = new System.Drawing.Point(15, 20);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(150, 40);
            this.lblTotal.TabIndex = 0;
            this.lblTotal.Text = "TỔNG TIỀN:";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // InvoiceForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.Controls.Add(this.dgvCart);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlTop);
            this.Name = "InvoiceForm";
            this.Text = "Lập hóa đơn";
            this.pnlTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCart)).EndInit();
            this.pnlBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
