using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sunny.UI;
using SupermarketApp.Utils;
using SupermarketApp.Data;
using SupermarketApp.Data.Models;

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
        private UILabel lblCustomerType;
        private UITextBox txtName;
        private UITextBox txtPhone;
        private UITextBox txtEmail;
        private UITextBox txtAddress;
        private UIComboBox cmbCustomerType;
        private UIButton btnAdd;
        private UIButton btnUpdate;
        private UIButton btnDelete;
        private UIButton btnClear;
        private UIButton btnRefresh;
        private UIButton btnDeleteFromId10;
        private UIDataGridView dgvCustomers;
        private int? selectedId = null;

        public CustomerForm()
        {
            InitializeComponent();
            this.Load += async (s,e)=> await LoadDataAsync();
            
            // Load customer type combo box
            cmbCustomerType.Items.Add("Vãng lai");
            cmbCustomerType.Items.Add("Thân quen");
            cmbCustomerType.SelectedIndex = 0;
        }

        private async Task LoadDataAsync()
        {
            using (var db = new SupermarketContext())
            {
                var query = db.KhachHang.AsQueryable();
                
                var data = await Task.Run(()=> query
                    .OrderByDescending(x=>x.MaKH)
                    .Select(x=> new { x.MaKH, x.TenKH, x.SDT, x.Email, x.DiaChi, x.DiemTichLuy, x.LoaiKH })
                    .ToList());
                
                dgvCustomers.DataSource = data;
                
                // Apply standard style
                DataGridViewStyleHelper.ApplyStandardStyle(dgvCustomers, Color.FromArgb(139, 92, 246));
                
                if (dgvCustomers.Columns.Count > 0)
                {
                    DataGridViewStyleHelper.ConfigureCenterColumn(dgvCustomers.Columns["MaKH"], "Mã KH", 80);
                    DataGridViewStyleHelper.ConfigureColumn(dgvCustomers.Columns["TenKH"], "Tên khách hàng", 220);
                    DataGridViewStyleHelper.ConfigureCenterColumn(dgvCustomers.Columns["SDT"], "Số điện thoại", 140);
                    DataGridViewStyleHelper.ConfigureColumn(dgvCustomers.Columns["Email"], "Email", 200);
                    DataGridViewStyleHelper.ConfigureColumn(dgvCustomers.Columns["DiaChi"], "Địa chỉ", 250);
                    dgvCustomers.Columns["DiaChi"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    DataGridViewStyleHelper.ConfigureNumericColumn(dgvCustomers.Columns["DiemTichLuy"], "Điểm", 80);
                    DataGridViewStyleHelper.ConfigureCenterColumn(dgvCustomers.Columns["LoaiKH"], "Loại KH", 110);
                    
                    // Color coding for customer types
                    foreach (DataGridViewRow row in dgvCustomers.Rows)
                    {
                        if (row.Cells["LoaiKH"].Value != null)
                        {
                            string loai = row.Cells["LoaiKH"].Value.ToString();
                            Color cellBackColor = Color.White;
                            Color cellForeColor = Color.FromArgb(48, 48, 48);
                            
                            if (loai == "VIP")
                            {
                                cellBackColor = Color.FromArgb(255, 250, 205); // Vàng nhạt
                                row.Cells["LoaiKH"].Style.ForeColor = Color.FromArgb(245, 158, 11);
                                row.Cells["LoaiKH"].Style.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                            }
                            else if (loai == "Thân quen")
                            {
                                cellBackColor = Color.FromArgb(236, 253, 245); // Xanh nhạt
                                row.Cells["LoaiKH"].Style.ForeColor = Color.FromArgb(16, 185, 129);
                                row.Cells["LoaiKH"].Style.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                            }
                            else
                            {
                                cellBackColor = Color.White;
                                row.Cells["LoaiKH"].Style.ForeColor = Color.FromArgb(107, 114, 128);
                            }
                            
                            row.DefaultCellStyle.BackColor = cellBackColor;
                        }
                    }
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
                cmbCustomerType.Text = dgvCustomers.Rows[e.RowIndex].Cells["LoaiKH"].Value?.ToString();
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
                        var exists = await Task.Run(() => db.KhachHang.Any(x => x.SDT == phone));
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
                        NgayTao = DateTime.Now,
                        DiemTichLuy = 0,
                        LoaiKH = cmbCustomerType?.SelectedItem?.ToString() ?? "Vãng lai"
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
                        var exists = await Task.Run(() => db.KhachHang.Any(x => x.SDT == phone && x.MaKH != selectedId.Value));
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
                    kh.LoaiKH = cmbCustomerType?.SelectedItem?.ToString() ?? "Vãng lai";
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

        private async void BtnDeleteFromId10_Click(object sender, EventArgs e)
        {
            if (!MessageHelper.ShowAsk("⚠️ CẢNH BÁO NGHIÊM TRỌNG! ⚠️\n\nBạn muốn xóa:\n• TẤT CẢ khách hàng từ ID 10 trở đi\n• Tất cả hóa đơn và chi tiết liên quan\n\nHành động này KHÔNG THỂ HOÀN TÁC!\n\nXác nhận xóa?"))
            {
                return;
            }

            try
            {
                using (var db = new SupermarketContext())
                {
                    // Xóa chi tiết hóa đơn
                    var hoaDonIds = await Task.Run(() => db.HoaDon
                        .Where(x => x.MaKH.HasValue && x.MaKH.Value > 9)
                        .Select(x => x.MaHD)
                        .ToList());

                    if (hoaDonIds.Any())
                    {
                        foreach (var maHD in hoaDonIds)
                        {
                            db.CTHoaDon.RemoveRange(db.CTHoaDon.Where(x => x.MaHD == maHD));
                        }
                        await db.SaveChangesAsync();
                    }

                    // Xóa hóa đơn
                    var hoaDons = await Task.Run(() => db.HoaDon
                        .Where(x => x.MaKH.HasValue && x.MaKH.Value > 9)
                        .ToList());
                    if (hoaDons.Any())
                    {
                        db.HoaDon.RemoveRange(hoaDons);
                        await db.SaveChangesAsync();
                    }

                    // Xóa khách hàng từ ID 10 trở đi
                    var customers = await Task.Run(() => db.KhachHang
                        .Where(x => x.MaKH > 9)
                        .ToList());
                    
                    int count = customers.Count;
                    if (customers.Any())
                    {
                        db.KhachHang.RemoveRange(customers);
                        await db.SaveChangesAsync();
                    }

                    MessageHelper.ShowSuccess($"✅ Đã xóa {count} khách hàng từ ID 10 trở đi!\n✅ Đã xóa {hoaDons.Count} hóa đơn liên quan");
                    await LoadDataAsync();
                }
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("❌ Lỗi khi xóa: " + ex.Message);
            }
        }

        private void ClearForm()
        {
            txtName.Text = txtPhone.Text = txtEmail.Text = txtAddress.Text = "";
            cmbCustomerType.SelectedIndex = 0;
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
            this.pnlForm = new Sunny.UI.UIPanel();
            this.btnDeleteFromId10 = new Sunny.UI.UIButton();
            this.btnRefresh = new Sunny.UI.UIButton();
            this.btnClear = new Sunny.UI.UIButton();
            this.btnDelete = new Sunny.UI.UIButton();
            this.btnUpdate = new Sunny.UI.UIButton();
            this.btnAdd = new Sunny.UI.UIButton();
            this.cmbCustomerType = new Sunny.UI.UIComboBox();
            this.lblCustomerType = new Sunny.UI.UILabel();
            this.txtAddress = new Sunny.UI.UITextBox();
            this.lblAddress = new Sunny.UI.UILabel();
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
            this.pnlTop.Font = FontHelper.GetStandardFont();
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
            this.lblTitle.Font = FontHelper.GetTitleFont();
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
            this.pnlForm.Controls.Add(this.btnDeleteFromId10);
            this.pnlForm.Controls.Add(this.btnRefresh);
            this.pnlForm.Controls.Add(this.btnClear);
            this.pnlForm.Controls.Add(this.btnDelete);
            this.pnlForm.Controls.Add(this.btnUpdate);
            this.pnlForm.Controls.Add(this.btnAdd);
            this.pnlForm.Controls.Add(this.cmbCustomerType);
            this.pnlForm.Controls.Add(this.lblCustomerType);
            this.pnlForm.Controls.Add(this.txtAddress);
            this.pnlForm.Controls.Add(this.lblAddress);
            this.pnlForm.Controls.Add(this.txtEmail);
            this.pnlForm.Controls.Add(this.lblEmail);
            this.pnlForm.Controls.Add(this.txtPhone);
            this.pnlForm.Controls.Add(this.lblPhone);
            this.pnlForm.Controls.Add(this.txtName);
            this.pnlForm.Controls.Add(this.lblName);
            this.pnlForm.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlForm.FillColor = System.Drawing.Color.White;
            this.pnlForm.Font = FontHelper.GetStandardFont();
            this.pnlForm.Location = new System.Drawing.Point(0, 60);
            this.pnlForm.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlForm.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlForm.Name = "pnlForm";
            this.pnlForm.Padding = new System.Windows.Forms.Padding(15);
            this.pnlForm.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.pnlForm.Size = new System.Drawing.Size(1000, 230);
            this.pnlForm.TabIndex = 1;
            this.pnlForm.Text = null;
            this.pnlForm.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnDeleteFromId10
            // 
            this.btnDeleteFromId10.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDeleteFromId10.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnDeleteFromId10.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.btnDeleteFromId10.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.btnDeleteFromId10.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.btnDeleteFromId10.Font = FontHelper.GetHeaderFont();
            this.btnDeleteFromId10.Location = new System.Drawing.Point(731, 100);
            this.btnDeleteFromId10.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnDeleteFromId10.Name = "btnDeleteFromId10";
            this.btnDeleteFromId10.Size = new System.Drawing.Size(200, 40);
            this.btnDeleteFromId10.TabIndex = 13;
            this.btnDeleteFromId10.Text = "🗑️ Xóa hết (ID ≥10)";
            this.btnDeleteFromId10.TipsFont = FontHelper.GetStandardFont();
            this.btnDeleteFromId10.Click += new System.EventHandler(this.BtnDeleteFromId10_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.btnRefresh.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.btnRefresh.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.btnRefresh.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            this.btnRefresh.Font = FontHelper.GetButtonFont();
            this.btnRefresh.Location = new System.Drawing.Point(580, 100);
            this.btnRefresh.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(130, 40);
            this.btnRefresh.TabIndex = 12;
            this.btnRefresh.Text = "🔃 Tải lại";
            this.btnRefresh.TipsFont = FontHelper.GetStandardFont();
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // btnClear
            // 
            this.btnClear.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClear.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnClear.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnClear.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnClear.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(78)))), ((int)(((byte)(216)))));
            this.btnClear.Font = FontHelper.GetButtonFont();
            this.btnClear.Location = new System.Drawing.Point(437, 100);
            this.btnClear.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(120, 40);
            this.btnClear.TabIndex = 11;
            this.btnClear.Text = "🔄 Làm mới";
            this.btnClear.TipsFont = FontHelper.GetStandardFont();
            this.btnClear.Click += new System.EventHandler(this.BtnClear_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDelete.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnDelete.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.btnDelete.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.btnDelete.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.btnDelete.Font = FontHelper.GetButtonFont();
            this.btnDelete.Location = new System.Drawing.Point(299, 100);
            this.btnDelete.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(120, 40);
            this.btnDelete.TabIndex = 10;
            this.btnDelete.Text = "🗑️ Xóa";
            this.btnDelete.TipsFont = FontHelper.GetStandardFont();
            this.btnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnUpdate.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnUpdate.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnUpdate.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnUpdate.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(78)))), ((int)(((byte)(216)))));
            this.btnUpdate.Font = FontHelper.GetButtonFont();
            this.btnUpdate.Location = new System.Drawing.Point(156, 100);
            this.btnUpdate.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(120, 40);
            this.btnUpdate.TabIndex = 9;
            this.btnUpdate.Text = "✏️ Sửa";
            this.btnUpdate.TipsFont = FontHelper.GetStandardFont();
            this.btnUpdate.Click += new System.EventHandler(this.BtnUpdate_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAdd.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(197)))), ((int)(((byte)(94)))));
            this.btnAdd.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(74)))));
            this.btnAdd.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(74)))));
            this.btnAdd.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(128)))), ((int)(((byte)(61)))));
            this.btnAdd.Font = FontHelper.GetButtonFont();
            this.btnAdd.Location = new System.Drawing.Point(15, 100);
            this.btnAdd.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(120, 40);
            this.btnAdd.TabIndex = 8;
            this.btnAdd.Text = "➕ Thêm";
            this.btnAdd.TipsFont = FontHelper.GetStandardFont();
            this.btnAdd.Click += new System.EventHandler(this.BtnAdd_Click);
            // 
            // cmbCustomerType
            // 
            this.cmbCustomerType.DataSource = null;
            this.cmbCustomerType.DropDownStyle = Sunny.UI.UIDropDownStyle.DropDownList;
            this.cmbCustomerType.FillColor = System.Drawing.Color.White;
            this.cmbCustomerType.Font = FontHelper.GetComboBoxFont();
            this.cmbCustomerType.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.cmbCustomerType.Items.AddRange(new object[] {
            "Vãng lai",
            "Thân quen"});
            this.cmbCustomerType.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.cmbCustomerType.Location = new System.Drawing.Point(807, 35);
            this.cmbCustomerType.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbCustomerType.MinimumSize = new System.Drawing.Size(63, 0);
            this.cmbCustomerType.Name = "cmbCustomerType";
            this.cmbCustomerType.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.cmbCustomerType.Size = new System.Drawing.Size(180, 35);
            this.cmbCustomerType.SymbolDropDown = 61555;
            this.cmbCustomerType.SymbolNormal = 61555;
            this.cmbCustomerType.SymbolSize = 24;
            this.cmbCustomerType.TabIndex = 9;
            this.cmbCustomerType.Text = "Vãng lai";
            this.cmbCustomerType.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmbCustomerType.Watermark = "";
            // 
            // lblCustomerType
            // 
            this.lblCustomerType.Font = FontHelper.GetLabelFont();
            this.lblCustomerType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblCustomerType.Location = new System.Drawing.Point(811, 5);
            this.lblCustomerType.Name = "lblCustomerType";
            this.lblCustomerType.Size = new System.Drawing.Size(120, 25);
            this.lblCustomerType.TabIndex = 8;
            this.lblCustomerType.Text = "🏷️ Loại KH";
            this.lblCustomerType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtAddress
            // 
            this.txtAddress.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAddress.Font = FontHelper.GetTextBoxFont();
            this.txtAddress.Location = new System.Drawing.Point(596, 35);
            this.txtAddress.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtAddress.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Padding = new System.Windows.Forms.Padding(5);
            this.txtAddress.ShowText = false;
            this.txtAddress.Size = new System.Drawing.Size(200, 35);
            this.txtAddress.TabIndex = 7;
            this.txtAddress.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtAddress.Watermark = "Địa chỉ khách hàng...";
            // 
            // lblAddress
            // 
            this.lblAddress.Font = FontHelper.GetLabelFont();
            this.lblAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblAddress.Location = new System.Drawing.Point(603, 0);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(120, 25);
            this.lblAddress.TabIndex = 6;
            this.lblAddress.Text = "Địa chỉ ";
            this.lblAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtEmail
            // 
            this.txtEmail.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtEmail.Font = FontHelper.GetTextBoxFont();
            this.txtEmail.Location = new System.Drawing.Point(388, 35);
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
            this.lblEmail.Font = FontHelper.GetLabelFont();
            this.lblEmail.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblEmail.Location = new System.Drawing.Point(401, 5);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(120, 25);
            this.lblEmail.TabIndex = 4;
            this.lblEmail.Text = "📧 Email";
            this.lblEmail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtPhone
            // 
            this.txtPhone.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtPhone.Font = FontHelper.GetTextBoxFont();
            this.txtPhone.Location = new System.Drawing.Point(228, 35);
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
            this.lblPhone.Font = FontHelper.GetLabelFont();
            this.lblPhone.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblPhone.Location = new System.Drawing.Point(230, 5);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(120, 25);
            this.lblPhone.TabIndex = 2;
            this.lblPhone.Text = "📞 Số điện thoại";
            this.lblPhone.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtName
            // 
            this.txtName.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtName.Font = FontHelper.GetTextBoxFont();
            this.txtName.Location = new System.Drawing.Point(13, 35);
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
            this.lblName.Font = FontHelper.GetLabelFont();
            this.lblName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblName.Location = new System.Drawing.Point(12, 5);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(150, 25);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "👤 Tên khách hàng";
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
            dataGridViewCellStyle2.Font = FontHelper.GetDGVHeaderFont();
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCustomers.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvCustomers.ColumnHeadersHeight = 40;
            this.dgvCustomers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = FontHelper.GetDVGCellFont();
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(254)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(64)))), ((int)(((byte)(175)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvCustomers.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvCustomers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCustomers.EnableHeadersVisualStyles = false;
            this.dgvCustomers.Font = FontHelper.GetDVGCellFont();
            this.dgvCustomers.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(254)))));
            this.dgvCustomers.Location = new System.Drawing.Point(0, 290);
            this.dgvCustomers.Name = "dgvCustomers";
            this.dgvCustomers.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(246)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.Font = FontHelper.GetDVGCellFont();
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCustomers.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvCustomers.RowHeadersWidth = 51;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = FontHelper.GetDVGCellFont();
            this.dgvCustomers.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvCustomers.RowTemplate.Height = 35;
            this.dgvCustomers.SelectedIndex = -1;
            this.dgvCustomers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCustomers.Size = new System.Drawing.Size(1000, 310);
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

        
    }
}
