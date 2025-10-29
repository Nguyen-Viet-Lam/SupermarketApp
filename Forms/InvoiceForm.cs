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
        private UIIntegerUpDown numQty;
        private UIButton btnAddToCart;
        private UIButton btnRemove;
        private UIButton btnClear;
        private UILabel lblCustomerType;
        private UIButton btnVangLai;
        private UIButton btnThanThiet;
        private UIButton btnAddCustomer;
        private UILabel lblSelectedCustomer;
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

        public int MaNVCurrent { get; set; } = 1; 
        public int? MaKHCurrent { get; set; } = null;
        private string selectedCustomerType = "Vãng lai";
        private string selectedCustomerName = ""; 

        public InvoiceForm()
        {
            InitializeComponent();
            this.Load += async (s,e)=> await InitAsync();
            
            // Set default selection
            UpdateCustomerTypeSelection("Vãng lai");
        }

        private async Task InitAsync()
        {
            using (var db = new SupermarketContext())
            {
                products = await Task.Run(() => db.SanPham
                    .Where(x => x.SoLuong > 0)
                    .OrderBy(x => x.TenSP)
                    .ToList());
            }
            
            cbProduct.Items.Clear();
            cbProduct.DataSource = null;
            
            var displayList = products.Select(p => 
                $"{p.TenSP} - Giá: {p.DonGia:N0} - Tồn: {p.SoLuong}"
            ).ToList();
            
            cbProduct.DataSource = displayList;
            cbProduct.SelectedIndex = products.Count > 0 ? 0 : -1;
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

        private void BtnVangLai_Click(object sender, EventArgs e)
        {
            UpdateCustomerTypeSelection("Vãng lai", null, "");
            MessageHelper.ShowTipSuccess("Đã chọn: Khách vãng lai");
        }

        private void BtnThanThiet_Click(object sender, EventArgs e)
        {
            UpdateCustomerTypeSelection("Thân quen", null, "");
            MessageHelper.ShowTipSuccess("Đã chọn: Khách thân thiết");
        }

        private void UpdateCustomerTypeSelection(string type, int? maKH = null, string tenKH = "")
        {
            selectedCustomerType = type;
            MaKHCurrent = maKH;
            selectedCustomerName = tenKH;
            
            // Update customer info label
            if (lblSelectedCustomer != null)
            {
                if (maKH.HasValue && !string.IsNullOrWhiteSpace(tenKH))
                {
                    lblSelectedCustomer.Text = $"KH: {tenKH} ({type})";
                    lblSelectedCustomer.ForeColor = Color.FromArgb(34, 197, 94);
                }
                else
                {
                    lblSelectedCustomer.Text = $"Loại: {type}";
                    lblSelectedCustomer.ForeColor = Color.FromArgb(107, 114, 128);
                }
            }
            
            // Reset all buttons to default style
            if (btnVangLai != null)
            {
                btnVangLai.FillColor = Color.FromArgb(229, 231, 235);
                btnVangLai.ForeColor = Color.FromArgb(75, 85, 99);
            }
            if (btnThanThiet != null)
            {
                btnThanThiet.FillColor = Color.FromArgb(229, 231, 235);
                btnThanThiet.ForeColor = Color.FromArgb(75, 85, 99);
            }
            
            // Highlight selected button
            UIButton selectedBtn = null;
            Color activeColor = Color.FromArgb(59, 130, 246);
            
            if (type == "Vãng lai" && btnVangLai != null)
            {
                selectedBtn = btnVangLai;
            }
            else if (type == "Thân quen" && btnThanThiet != null)
            {
                selectedBtn = btnThanThiet;
                activeColor = Color.FromArgb(139, 92, 246);
            }
            
            if (selectedBtn != null)
            {
                selectedBtn.FillColor = activeColor;
                selectedBtn.ForeColor = Color.White;
            }
        }

        private async void BtnAddCustomer_Click(object sender, EventArgs e)
        {
            var inputForm = new Form
            {
                Text = "Thêm khách hàng mới",
                Size = new Size(450, 350),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.White
            };
            
            var lblName = new Label { Text = "Tên khách hàng:", Left = 20, Top = 20, Width = 120 };
            var txtName = new TextBox { Left = 150, Top = 18, Width = 250 };
            
            var lblPhone = new Label { Text = "Số điện thoại:", Left = 20, Top = 60, Width = 120 };
            var txtPhone = new TextBox { Left = 150, Top = 58, Width = 250 };
            
            var lblEmail = new Label { Text = "Email:", Left = 20, Top = 100, Width = 120 };
            var txtEmail = new TextBox { Left = 150, Top = 98, Width = 250 };
            
            var lblAddress = new Label { Text = "Địa chỉ:", Left = 20, Top = 140, Width = 120 };
            var txtAddress = new TextBox { Left = 150, Top = 138, Width = 250, Height = 60, Multiline = true };
            
            var lblType = new Label { Text = "Loại khách hàng:", Left = 20, Top = 210, Width = 120 };
            var cmbType = new ComboBox 
            { 
                Left = 150, 
                Top = 208, 
                Width = 250,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbType.Items.AddRange(new object[] { "Vãng lai", "Thân quen" });
            cmbType.SelectedIndex = 0;
            
            var btnOK = new Button { Text = "Thêm", Left = 150, Top = 250, Width = 120, DialogResult = DialogResult.OK };
            var btnCancel = new Button { Text = "Hủy", Left = 280, Top = 250, Width = 120, DialogResult = DialogResult.Cancel };
            
            btnOK.BackColor = Color.FromArgb(34, 197, 94);
            btnOK.ForeColor = Color.White;
            btnOK.FlatStyle = FlatStyle.Flat;
            btnOK.FlatAppearance.BorderSize = 0;
            
            btnCancel.BackColor = Color.FromArgb(107, 114, 128);
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 0;
            
            inputForm.Controls.AddRange(new Control[] { lblName, txtName, lblPhone, txtPhone, lblEmail, txtEmail, lblAddress, txtAddress, lblType, cmbType, btnOK, btnCancel });
            inputForm.AcceptButton = btnOK;
            inputForm.CancelButton = btnCancel;
            
            if (inputForm.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageHelper.ShowWarning("Vui lòng nhập tên khách hàng!");
                    return;
                }
                
                try
                {
                    using (var db = new SupermarketContext())
                    {
                        string phone = txtPhone.Text.Trim();
                        if (!string.IsNullOrWhiteSpace(phone))
                        {
                            var existingCustomer = db.KhachHang.FirstOrDefault(x => x.SDT == phone);
                            if (existingCustomer != null)
                            {
                                if (MessageHelper.ShowAsk($"Số điện thoại này đã tồn tại cho khách hàng: {existingCustomer.TenKH}\n\nBạn có muốn chọn khách hàng này không?"))
                                {
                                    UpdateCustomerTypeSelection(existingCustomer.LoaiKH, existingCustomer.MaKH, existingCustomer.TenKH);
                                    MessageHelper.ShowSuccess($"Đã chọn khách hàng: {existingCustomer.TenKH}");
                                    return;
                                }
                                else
                                {
                                    MessageHelper.ShowWarning("Vui lòng sử dụng số điện thoại khác hoặc chọn khách hàng đã tồn tại!");
                                    return;
                                }
                            }
                        }
                        
                        var newCustomer = new KhachHang
                        {
                            TenKH = txtName.Text.Trim(),
                            SDT = string.IsNullOrWhiteSpace(phone) ? null : phone,
                            Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                            DiaChi = string.IsNullOrWhiteSpace(txtAddress.Text) ? null : txtAddress.Text.Trim(),
                            LoaiKH = cmbType.SelectedItem.ToString(),
                            DiemTichLuy = 0,
                            NgayTao = DateTime.Now
                        };
                        
                        db.KhachHang.Add(newCustomer);
                        await db.SaveChangesAsync();
                        
                        UpdateCustomerTypeSelection(newCustomer.LoaiKH, newCustomer.MaKH, newCustomer.TenKH);
                        
                        MessageHelper.ShowSuccess($"Đã thêm khách hàng: {newCustomer.TenKH}\nLoại: {newCustomer.LoaiKH}");
                    }
                }
                catch (Exception ex)
                {
                    MessageHelper.ShowError("Lỗi khi thêm khách hàng: " + ex.Message);
                }
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
                
                // Determine customer based on selection
                string selectedLoai = selectedCustomerType;
                int? maKHForInvoice = MaKHCurrent;

                using (var db = new SupermarketContext())
                {
                    if (maKHForInvoice.HasValue)
                    {
                        var kh = await db.KhachHang.FindAsync(maKHForInvoice.Value);
                        if (kh != null)
                        {
                            kh.LoaiKH = selectedLoai;
                            await db.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        // Ensure a default customer exists for this type, then use it
                        var tenMacDinh = selectedLoai == "Thân quen" ? "Khách Thân quen" : "Khách Vãng lai";
                        var kh = db.KhachHang.FirstOrDefault(x => x.TenKH == tenMacDinh && x.LoaiKH == selectedLoai);
                        if (kh == null)
                        {
                            kh = new KhachHang
                            {
                                TenKH = tenMacDinh,
                                SDT = null,
                                Email = null,
                                DiaChi = null,
                                DiemTichLuy = 0,
                                LoaiKH = selectedLoai,
                                NgayTao = DateTime.Now
                            };
                            db.KhachHang.Add(kh);
                            await db.SaveChangesAsync();
                        }
                        maKHForInvoice = kh.MaKH;
                    }
                }

                var service = new SupermarketApp.Services.InvoiceService();
                int maHD = await service.CreateInvoiceAsync(MaNVCurrent, maKHForInvoice, cart);
                
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

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlTop = new Sunny.UI.UIPanel();
            this.btnClear = new Sunny.UI.UIButton();
            this.btnAddCustomer = new Sunny.UI.UIButton();
            this.btnThanThiet = new Sunny.UI.UIButton();
            this.btnVangLai = new Sunny.UI.UIButton();
            this.lblCustomerType = new Sunny.UI.UILabel();
            this.btnRemove = new Sunny.UI.UIButton();
            this.btnAddToCart = new Sunny.UI.UIButton();
            this.numQty = new Sunny.UI.UIIntegerUpDown();
            this.lblQuantity = new Sunny.UI.UILabel();
            this.cbProduct = new Sunny.UI.UIComboBox();
            this.lblProduct = new Sunny.UI.UILabel();
            this.lblTitle = new Sunny.UI.UILabel();
            this.lblSelectedCustomer = new Sunny.UI.UILabel();
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
            this.pnlTop.Controls.Add(this.btnAddCustomer);
            this.pnlTop.Controls.Add(this.btnThanThiet);
            this.pnlTop.Controls.Add(this.btnVangLai);
            this.pnlTop.Controls.Add(this.lblCustomerType);
            this.pnlTop.Controls.Add(this.btnRemove);
            this.pnlTop.Controls.Add(this.btnAddToCart);
            this.pnlTop.Controls.Add(this.numQty);
            this.pnlTop.Controls.Add(this.lblQuantity);
            this.pnlTop.Controls.Add(this.cbProduct);
            this.pnlTop.Controls.Add(this.lblProduct);
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
            // btnAddCustomer
            // 
            this.btnAddCustomer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddCustomer.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.btnAddCustomer.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            this.btnAddCustomer.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            this.btnAddCustomer.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(4)))), ((int)(((byte)(120)))), ((int)(((byte)(87)))));
            this.btnAddCustomer.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.btnAddCustomer.Location = new System.Drawing.Point(880, 45);
            this.btnAddCustomer.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnAddCustomer.Name = "btnAddCustomer";
            this.btnAddCustomer.Size = new System.Drawing.Size(120, 35);
            this.btnAddCustomer.TabIndex = 12;
            this.btnAddCustomer.Text = "➕ Thêm KH";
            this.btnAddCustomer.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnAddCustomer.Click += new System.EventHandler(this.BtnAddCustomer_Click);
            // 
            // btnThanThiet
            // 
            this.btnThanThiet.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnThanThiet.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.btnThanThiet.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.btnThanThiet.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.btnThanThiet.Location = new System.Drawing.Point(745, 45);
            this.btnThanThiet.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnThanThiet.Name = "btnThanThiet";
            this.btnThanThiet.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.btnThanThiet.Size = new System.Drawing.Size(129, 35);
            this.btnThanThiet.TabIndex = 10;
            this.btnThanThiet.Text = "💎 Thân thiết";
            this.btnThanThiet.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnThanThiet.Click += new System.EventHandler(this.BtnThanThiet_Click);
            // 
            // btnVangLai
            // 
            this.btnVangLai.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVangLai.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnVangLai.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.btnVangLai.Location = new System.Drawing.Point(624, 44);
            this.btnVangLai.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnVangLai.Name = "btnVangLai";
            this.btnVangLai.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.btnVangLai.Size = new System.Drawing.Size(115, 35);
            this.btnVangLai.TabIndex = 9;
            this.btnVangLai.Text = "👤 Vãng lai";
            this.btnVangLai.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnVangLai.Click += new System.EventHandler(this.BtnVangLai_Click);
            // 
            // lblCustomerType
            // 
            this.lblCustomerType.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblCustomerType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblCustomerType.Location = new System.Drawing.Point(515, 44);
            this.lblCustomerType.Name = "lblCustomerType";
            this.lblCustomerType.Size = new System.Drawing.Size(103, 35);
            this.lblCustomerType.TabIndex = 8;
            this.lblCustomerType.Text = "Loại KH:";
            this.lblCustomerType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnRemove
            // 
            this.btnRemove.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRemove.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnRemove.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.btnRemove.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.btnRemove.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.btnRemove.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnRemove.Location = new System.Drawing.Point(718, 85);
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
            this.btnAddToCart.Location = new System.Drawing.Point(547, 85);
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
            this.numQty.Location = new System.Drawing.Point(413, 84);
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
            this.lblQuantity.Location = new System.Drawing.Point(409, 45);
            this.lblQuantity.Name = "lblQuantity";
            this.lblQuantity.Size = new System.Drawing.Size(100, 35);
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
            this.cbProduct.Size = new System.Drawing.Size(392, 35);
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
            // lblSelectedCustomer
            // 
            this.lblSelectedCustomer.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Italic);
            this.lblSelectedCustomer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblSelectedCustomer.Location = new System.Drawing.Point(376, 146);
            this.lblSelectedCustomer.Name = "lblSelectedCustomer";
            this.lblSelectedCustomer.Size = new System.Drawing.Size(420, 25);
            this.lblSelectedCustomer.TabIndex = 13;
            this.lblSelectedCustomer.Text = "Loại: Vãng lai";
            this.lblSelectedCustomer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            this.lblTotalValue.Location = new System.Drawing.Point(175, 20);
            this.lblTotalValue.Name = "lblTotalValue";
            this.lblTotalValue.Size = new System.Drawing.Size(300, 40);
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
            this.Controls.Add(this.lblSelectedCustomer);
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
