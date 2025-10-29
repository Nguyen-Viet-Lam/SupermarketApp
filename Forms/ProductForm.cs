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
    public partial class ProductForm : Form
    {
        private UIPanel pnlTop;
        private UILabel lblTitle;
        private UITextBox txtSearch;
        private UIButton btnSearch;
        private UIButton btnAdd;
        private UIButton btnEdit;
        private UIButton btnDelete;
        private UIButton btnRefresh;
        private UIDataGridView dgvProducts;
        private int? selectedId = null;

        public ProductForm()
        {
            InitializeComponent();
            this.Load += async (s,e)=> await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            using (var db = new SupermarketContext())
            {
                var data = await Task.Run(()=> db.SanPham
                    .OrderBy(x=>x.MaSP)
                    .Select(x => new {
                        x.MaSP,
                        x.TenSP,
                        x.DonGia,
                        x.SoLuong,
                        x.DonVi,
                        x.MoTa
                    }).ToList());
                
                dgvProducts.DataSource = data;
                
                // Apply standard style
                DataGridViewStyleHelper.ApplyStandardStyle(dgvProducts, Color.FromArgb(236, 72, 153));
                
                if (dgvProducts.Columns.Count > 0)
                {
                    // Mã SP - căn giữa
                    DataGridViewStyleHelper.ConfigureCenterColumn(dgvProducts.Columns["MaSP"], "Mã SP", 80);
                    
                    // Tên sản phẩm - căn trái
                    DataGridViewStyleHelper.ConfigureColumn(dgvProducts.Columns["TenSP"], "Tên sản phẩm", 280);
                    
                    // Đơn giá - số, căn phải
                    DataGridViewStyleHelper.ConfigureNumericColumn(dgvProducts.Columns["DonGia"], "Đơn giá", 130, "N0");
                    
                    // Tồn kho - số, căn phải
                    dgvProducts.Columns["SoLuong"].HeaderText = "Tồn kho";
                    dgvProducts.Columns["SoLuong"].Width = 100;
                    dgvProducts.Columns["SoLuong"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvProducts.Columns["SoLuong"].DefaultCellStyle.Font = FontHelper.GetDVGCellFont();
                    
                    // Đơn vị - text ngắn, căn giữa
                    DataGridViewStyleHelper.ConfigureCenterColumn(dgvProducts.Columns["DonVi"], "Đơn vị", 90);
                    
                    // Mô tả - căn trái
                    DataGridViewStyleHelper.ConfigureColumn(dgvProducts.Columns["MoTa"], "Mô tả", 250);
                    dgvProducts.Columns["MoTa"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
        }

        private void DgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvProducts.Rows[e.RowIndex].Cells["MaSP"].Value != null)
            {
                selectedId = Convert.ToInt32(dgvProducts.Rows[e.RowIndex].Cells["MaSP"].Value);
            }
        }

        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            await LoadDataAsync();
            MessageHelper.ShowTipSuccess("Đã tải lại dữ liệu!");
        }

        private async void BtnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                await LoadDataAsync();
                return;
            }

            using (var db = new SupermarketContext())
            {
                var keyword = txtSearch.Text.ToLower().Trim();
                var data = await Task.Run(()=> db.SanPham
                    .Where(x => x.TenSP.ToLower().Contains(keyword) 
                           || (x.MoTa != null && x.MoTa.ToLower().Contains(keyword)))
                    .Select(x => new {
                        x.MaSP,
                        x.TenSP,
                        x.DonGia,
                        x.SoLuong,
                        x.DonVi,
                        x.MoTa
                    }).ToList());
                
                dgvProducts.DataSource = data;
            }
        }

        private async void BtnAdd_Click(object sender, EventArgs e)
        {
            var dialog = new ProductDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (var db = new SupermarketContext())
                    {
                        db.SanPham.Add(dialog.Product);
                        await db.SaveChangesAsync();
                    }
                    MessageHelper.ShowSuccess("Thêm sản phẩm thành công!");
                    await LoadDataAsync();
                    selectedId = null;
                }
                catch (Exception ex)
                {
                    MessageHelper.ShowError("Lỗi: " + ex.Message);
                }
            }
        }

        private async void BtnEdit_Click(object sender, EventArgs e)
        {
            if (selectedId == null)
            {
                MessageHelper.ShowWarning("Vui lòng chọn sản phẩm cần sửa!");
                return;
            }

            try
            {
                using (var db = new SupermarketContext())
                {
                    var product = await db.SanPham.FindAsync(selectedId.Value);
                    if (product == null)
                    {
                        MessageHelper.ShowWarning("Không tìm thấy sản phẩm!");
                        return;
                    }

                    var dialog = new ProductDialog(product);
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        db.SanPham.Update(dialog.Product);
                        await db.SaveChangesAsync();
                        MessageHelper.ShowSuccess("Cập nhật sản phẩm thành công!");
                        await LoadDataAsync();
                        selectedId = null;
                    }
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
                MessageHelper.ShowWarning("Vui lòng chọn sản phẩm cần xóa!");
                return;
            }

            if (!MessageHelper.ShowAsk("Bạn có chắc muốn xóa sản phẩm này?"))
            {
                return;
            }

            try
            {
                using (var db = new SupermarketContext())
                {
                    var product = await db.SanPham.FindAsync(selectedId.Value);
                    if (product == null)
                    {
                        MessageHelper.ShowWarning("Không tìm thấy sản phẩm!");
                        return;
                    }

                    // Kiểm tra xem sản phẩm đã có trong hóa đơn chưa
                    var existsInInvoice = await Task.Run(() => db.CTHoaDon.Any(x => x.MaSP == selectedId.Value));
                    if (existsInInvoice)
                    {
                        MessageHelper.ShowWarning("Không thể xóa! Sản phẩm đã có trong hóa đơn.\n\nBạn có thể vô hiệu hóa sản phẩm thay vì xóa.");
                        return;
                    }

                    db.SanPham.Remove(product);
                    await db.SaveChangesAsync();
                    MessageHelper.ShowSuccess("Đã xóa sản phẩm thành công!");
                    await LoadDataAsync();
                    selectedId = null;
                }
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("Lỗi: " + ex.Message);
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
            this.btnRefresh = new Sunny.UI.UIButton();
            this.btnDelete = new Sunny.UI.UIButton();
            this.btnEdit = new Sunny.UI.UIButton();
            this.btnAdd = new Sunny.UI.UIButton();
            this.btnSearch = new Sunny.UI.UIButton();
            this.txtSearch = new Sunny.UI.UITextBox();
            this.lblTitle = new Sunny.UI.UILabel();
            this.dgvProducts = new Sunny.UI.UIDataGridView();
            this.pnlTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProducts)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.btnRefresh);
            this.pnlTop.Controls.Add(this.btnDelete);
            this.pnlTop.Controls.Add(this.btnEdit);
            this.pnlTop.Controls.Add(this.btnAdd);
            this.pnlTop.Controls.Add(this.btnSearch);
            this.pnlTop.Controls.Add(this.txtSearch);
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
            this.pnlTop.Size = new System.Drawing.Size(1000, 120);
            this.pnlTop.TabIndex = 0;
            this.pnlTop.Text = null;
            this.pnlTop.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.btnRefresh.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.btnRefresh.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.btnRefresh.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            this.btnRefresh.Font = FontHelper.GetButtonFont();
            this.btnRefresh.Location = new System.Drawing.Point(855, 65);
            this.btnRefresh.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(120, 40);
            this.btnRefresh.TabIndex = 6;
            this.btnRefresh.Text = "🔄 Tải lại";
            this.btnRefresh.TipsFont = FontHelper.GetStandardFont();
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDelete.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnDelete.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.btnDelete.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.btnDelete.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.btnDelete.Font = FontHelper.GetButtonFont();
            this.btnDelete.Location = new System.Drawing.Point(735, 65);
            this.btnDelete.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(110, 40);
            this.btnDelete.TabIndex = 5;
            this.btnDelete.Text = "🗑️ Xóa";
            this.btnDelete.TipsFont = FontHelper.GetStandardFont();
            this.btnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEdit.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnEdit.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnEdit.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnEdit.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(78)))), ((int)(((byte)(216)))));
            this.btnEdit.Font = FontHelper.GetButtonFont();
            this.btnEdit.Location = new System.Drawing.Point(615, 65);
            this.btnEdit.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(110, 40);
            this.btnEdit.TabIndex = 4;
            this.btnEdit.Text = "✏️ Sửa";
            this.btnEdit.TipsFont = FontHelper.GetStandardFont();
            this.btnEdit.Click += new System.EventHandler(this.BtnEdit_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAdd.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(197)))), ((int)(((byte)(94)))));
            this.btnAdd.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(74)))));
            this.btnAdd.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(74)))));
            this.btnAdd.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(128)))), ((int)(((byte)(61)))));
            this.btnAdd.Font = FontHelper.GetButtonFont();
            this.btnAdd.Location = new System.Drawing.Point(495, 65);
            this.btnAdd.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(110, 40);
            this.btnAdd.TabIndex = 3;
            this.btnAdd.Text = "➕ Thêm";
            this.btnAdd.TipsFont = FontHelper.GetStandardFont();
            this.btnAdd.Click += new System.EventHandler(this.BtnAdd_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearch.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(72)))), ((int)(((byte)(153)))));
            this.btnSearch.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(39)))), ((int)(((byte)(119)))));
            this.btnSearch.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(39)))), ((int)(((byte)(119)))));
            this.btnSearch.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(24)))), ((int)(((byte)(93)))));
            this.btnSearch.Font = FontHelper.GetButtonFont();
            this.btnSearch.Location = new System.Drawing.Point(375, 65);
            this.btnSearch.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(100, 40);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "🔍 Tìm";
            this.btnSearch.TipsFont = FontHelper.GetStandardFont();
            this.btnSearch.Click += new System.EventHandler(this.BtnSearch_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtSearch.Font = FontHelper.GetTextBoxFont();
            this.txtSearch.Location = new System.Drawing.Point(15, 65);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtSearch.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Padding = new System.Windows.Forms.Padding(5);
            this.txtSearch.ShowText = false;
            this.txtSearch.Size = new System.Drawing.Size(350, 40);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtSearch.Watermark = "Tìm kiếm sản phẩm...";
            // 
            // lblTitle
            // 
            this.lblTitle.Font = FontHelper.GetTitleFont();
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(72)))), ((int)(((byte)(153)))));
            this.lblTitle.Location = new System.Drawing.Point(15, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(409, 35);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "📦 QUẢN LÝ SẢN PHẨM";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dgvProducts
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(242)))), ((int)(((byte)(248)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvProducts.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvProducts.BackgroundColor = System.Drawing.Color.White;
            this.dgvProducts.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(72)))), ((int)(((byte)(153)))));
            dataGridViewCellStyle2.Font = FontHelper.GetDGVHeaderFont();
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(72)))), ((int)(((byte)(153)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvProducts.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvProducts.ColumnHeadersHeight = 40;
            this.dgvProducts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = FontHelper.GetDVGCellFont();
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(207)))), ((int)(((byte)(232)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvProducts.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvProducts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProducts.EnableHeadersVisualStyles = false;
            this.dgvProducts.Font = FontHelper.GetDVGCellFont();
            this.dgvProducts.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(207)))), ((int)(((byte)(232)))));
            this.dgvProducts.Location = new System.Drawing.Point(0, 120);
            this.dgvProducts.Name = "dgvProducts";
            this.dgvProducts.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(242)))), ((int)(((byte)(248)))));
            dataGridViewCellStyle4.Font = FontHelper.GetDVGCellFont();
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(72)))), ((int)(((byte)(153)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvProducts.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvProducts.RowHeadersWidth = 51;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = FontHelper.GetDVGCellFont();
            this.dgvProducts.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvProducts.RowTemplate.Height = 35;
            this.dgvProducts.SelectedIndex = -1;
            this.dgvProducts.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProducts.Size = new System.Drawing.Size(1000, 480);
            this.dgvProducts.StripeOddColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(242)))), ((int)(((byte)(248)))));
            this.dgvProducts.TabIndex = 1;
            this.dgvProducts.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvProducts_CellClick);
            // 
            // ProductForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.Controls.Add(this.dgvProducts);
            this.Controls.Add(this.pnlTop);
            this.Name = "ProductForm";
            this.Text = "Quản lý sản phẩm";
            this.pnlTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProducts)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
