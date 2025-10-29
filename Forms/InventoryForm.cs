using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sunny.UI;
using SupermarketApp.Utils;
using SupermarketApp.Data;
using SupermarketApp.Data.Models;

namespace SupermarketApp.Forms
{
    public partial class InventoryForm : Form
    {
        private UIPanel pnlTop;
        private UILabel lblTitle;
        private UILabel lblSearch;
        private UITextBox txtSearch;
        private UIButton btnSearch;
        private UIButton btnRefresh;
        private UIButton btnExport;
        private UIDataGridView dgvInventory;
        private UIPanel pnlStats;
        private UILabel lblTotalProducts;
        private UILabel lblLowStock;
        private UILabel lblOutOfStock;
        private UILabel lblTotalValue;

        public InventoryForm()
        {
            InitializeComponent();
            this.Load += async (s, e) => await LoadDataAsync();
        }

        private async Task LoadDataAsync(string searchTerm = "")
        {
            using (var db = new SupermarketContext())
            {
                var query = db.SanPham.AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(x => x.TenSP.Contains(searchTerm) || 
                                           x.LoaiSP.Contains(searchTerm) ||
                                           x.Barcode.Contains(searchTerm));
                }

                var data = await Task.Run(() => query
                    .OrderBy(x => x.TenSP)
                    .Select(x => new
                    {
                        x.MaSP,
                        x.TenSP,
                        x.LoaiSP,
                        x.DonVi,
                        x.SoLuong,
                        x.DonGia,
                        ThanhTien = x.SoLuong * x.DonGia,
                        TrangThai = x.TrangThai ? "Hoạt động" : "Ngừng",
                        TrangThaiKho = x.SoLuong == 0 ? "Hết hàng" : 
                                      x.SoLuong <= 10 ? "Sắp hết" : "Đủ hàng"
                    })
                    .ToList());

                dgvInventory.DataSource = data;

                if (dgvInventory.Columns.Count > 0)
                {
                    dgvInventory.Columns["MaSP"].HeaderText = "Mã SP";
                    dgvInventory.Columns["MaSP"].Width = 80;
                    dgvInventory.Columns["TenSP"].HeaderText = "Tên sản phẩm";
                    dgvInventory.Columns["TenSP"].Width = 250;
                    dgvInventory.Columns["LoaiSP"].HeaderText = "Loại";
                    dgvInventory.Columns["LoaiSP"].Width = 120;
                    dgvInventory.Columns["DonVi"].HeaderText = "Đơn vị";
                    dgvInventory.Columns["DonVi"].Width = 80;
                    dgvInventory.Columns["SoLuong"].HeaderText = "Tồn kho";
                    dgvInventory.Columns["SoLuong"].Width = 100;
                    dgvInventory.Columns["DonGia"].HeaderText = "Đơn giá";
                    dgvInventory.Columns["DonGia"].Width = 120;
                    dgvInventory.Columns["DonGia"].DefaultCellStyle.Format = "N0";
                    dgvInventory.Columns["ThanhTien"].HeaderText = "Giá trị";
                    dgvInventory.Columns["ThanhTien"].Width = 150;
                    dgvInventory.Columns["ThanhTien"].DefaultCellStyle.Format = "N0";
                    dgvInventory.Columns["TrangThai"].HeaderText = "Trạng thái";
                    dgvInventory.Columns["TrangThai"].Width = 100;
                    dgvInventory.Columns["TrangThaiKho"].HeaderText = "Tình trạng";
                    dgvInventory.Columns["TrangThaiKho"].Width = 100;

                    // Tô màu theo tình trạng kho
                    foreach (DataGridViewRow row in dgvInventory.Rows)
                    {
                        if (row.Cells["TrangThaiKho"].Value?.ToString() == "Hết hàng")
                        {
                            row.DefaultCellStyle.BackColor = Color.FromArgb(254, 226, 226); // Đỏ nhạt
                        }
                        else if (row.Cells["TrangThaiKho"].Value?.ToString() == "Sắp hết")
                        {
                            row.DefaultCellStyle.BackColor = Color.FromArgb(254, 243, 199); // Vàng nhạt
                        }
                    }
                }

                // Cập nhật thống kê
                await UpdateStatsAsync();
            }
        }

        private async Task UpdateStatsAsync()
        {
            using (var db = new SupermarketContext())
            {
                var stats = await Task.Run(() => new
                {
                    TotalProducts = db.SanPham.Count(),
                    LowStock = db.SanPham.Count(x => x.SoLuong > 0 && x.SoLuong <= 10),
                    OutOfStock = db.SanPham.Count(x => x.SoLuong == 0),
                    TotalValue = db.SanPham.Sum(x => x.SoLuong * x.DonGia)
                });

                lblTotalProducts.Text = $"Tổng SP: {stats.TotalProducts}";
                lblLowStock.Text = $"Sắp hết: {stats.LowStock}";
                lblOutOfStock.Text = $"Hết hàng: {stats.OutOfStock}";
                lblTotalValue.Text = $"Giá trị: {stats.TotalValue:N0} VNĐ";
            }
        }

        private async void BtnSearch_Click(object sender, EventArgs e)
        {
            await LoadDataAsync(txtSearch.Text.Trim());
        }

        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            await LoadDataAsync();
            MessageHelper.ShowTipSuccess("Đã tải lại dữ liệu!");
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvInventory.DataSource == null || dgvInventory.Rows.Count == 0)
                {
                    MessageHelper.ShowWarning("Không có dữ liệu để xuất CSV!");
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                    Title = "Xuất báo cáo tồn kho CSV",
                    FileName = $"BaoCaoTonKho_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToCSV(saveFileDialog.FileName);
                    MessageHelper.ShowSuccess($"✅ Xuất CSV thành công!\nFile: {saveFileDialog.FileName}");
                }
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError($"Lỗi xuất CSV: {ex.Message}");
            }
        }

        private void ExportToCSV(string filePath)
        {
            using (var writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
            {
                // UTF-8 BOM để Excel nhận diện tiếng Việt
                writer.Write('\uFEFF');

                // Tiêu đề báo cáo
                writer.WriteLine("BÁO CÁO TỒN KHO SIÊU THỊ");
                writer.WriteLine($"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm}");
                writer.WriteLine(); // Dòng trống

                // Header
                var headers = new System.Collections.Generic.List<string>();
                for (int i = 0; i < dgvInventory.Columns.Count; i++)
                {
                    headers.Add(dgvInventory.Columns[i].HeaderText);
                }
                writer.WriteLine(string.Join(";", headers));

                // Data
                for (int r = 0; r < dgvInventory.Rows.Count; r++)
                {
                    var rowValues = new System.Collections.Generic.List<string>();
                    for (int c = 0; c < dgvInventory.Columns.Count; c++)
                    {
                        var val = dgvInventory.Rows[r].Cells[c].Value;
                        string valueStr = val?.ToString() ?? "";
                        
                        // Escape giá trị nếu chứa dấu chấm phẩy hoặc dấu ngoặc kép
                        if (valueStr.Contains(";") || valueStr.Contains("\"") || valueStr.Contains("\n"))
                        {
                            valueStr = "\"" + valueStr.Replace("\"", "\"\"") + "\"";
                        }
                        
                        rowValues.Add(valueStr);
                    }
                    writer.WriteLine(string.Join(";", rowValues));
                }

                // Summary
                writer.WriteLine();
                writer.WriteLine("THỐNG KÊ TỔNG QUAN:");
                writer.WriteLine($"Tổng sản phẩm;{lblTotalProducts.Text.Replace("Tổng SP: ", "")}");
                writer.WriteLine($"Sản phẩm sắp hết;{lblLowStock.Text.Replace("Sắp hết: ", "")}");
                writer.WriteLine($"Sản phẩm hết hàng;{lblOutOfStock.Text.Replace("Hết hàng: ", "")}");
                writer.WriteLine($"Tổng giá trị tồn kho;{lblTotalValue.Text.Replace("Giá trị: ", "")}");
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
            this.lblSearch = new Sunny.UI.UILabel();
            this.txtSearch = new Sunny.UI.UITextBox();
            this.btnSearch = new Sunny.UI.UIButton();
            this.btnRefresh = new Sunny.UI.UIButton();
            this.btnExport = new Sunny.UI.UIButton();
            this.dgvInventory = new Sunny.UI.UIDataGridView();
            this.pnlStats = new Sunny.UI.UIPanel();
            this.lblTotalProducts = new Sunny.UI.UILabel();
            this.lblLowStock = new Sunny.UI.UILabel();
            this.lblOutOfStock = new Sunny.UI.UILabel();
            this.lblTotalValue = new Sunny.UI.UILabel();
            this.pnlTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInventory)).BeginInit();
            this.pnlStats.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.lblTitle);
            this.pnlTop.Controls.Add(this.lblSearch);
            this.pnlTop.Controls.Add(this.txtSearch);
            this.pnlTop.Controls.Add(this.btnSearch);
            this.pnlTop.Controls.Add(this.btnRefresh);
            this.pnlTop.Controls.Add(this.btnExport);
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
            this.pnlTop.Size = new System.Drawing.Size(1200, 100);
            this.pnlTop.TabIndex = 0;
            this.pnlTop.Text = null;
            this.pnlTop.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.lblTitle.Location = new System.Drawing.Point(12, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(358, 35);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "📦 QUẢN LÝ KHO HÀNG";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSearch
            // 
            this.lblSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblSearch.Location = new System.Drawing.Point(13, 55);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(100, 35);
            this.lblSearch.TabIndex = 1;
            this.lblSearch.Text = "Tìm kiếm:";
            this.lblSearch.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtSearch
            // 
            this.txtSearch.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.txtSearch.Location = new System.Drawing.Point(120, 55);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtSearch.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Padding = new System.Windows.Forms.Padding(5);
            this.txtSearch.ShowText = false;
            this.txtSearch.Size = new System.Drawing.Size(347, 35);
            this.txtSearch.TabIndex = 2;
            this.txtSearch.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtSearch.Watermark = "Nhập tên SP, loại SP hoặc barcode...";
            // 
            // btnSearch
            // 
            this.btnSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearch.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnSearch.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnSearch.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.btnSearch.Location = new System.Drawing.Point(474, 55);
            this.btnSearch.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(100, 35);
            this.btnSearch.TabIndex = 3;
            this.btnSearch.Text = "🔍 Tìm";
            this.btnSearch.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnSearch.Click += new System.EventHandler(this.BtnSearch_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.btnRefresh.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.btnRefresh.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.btnRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.btnRefresh.Location = new System.Drawing.Point(589, 55);
            this.btnRefresh.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(120, 35);
            this.btnRefresh.TabIndex = 4;
            this.btnRefresh.Text = "🔄 Làm mới";
            this.btnRefresh.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // btnExport
            // 
            this.btnExport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExport.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.btnExport.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            this.btnExport.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            this.btnExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.btnExport.Location = new System.Drawing.Point(724, 55);
            this.btnExport.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(162, 35);
            this.btnExport.TabIndex = 5;
            this.btnExport.Text = "📊 Xuất CSV";
            this.btnExport.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnExport.Click += new System.EventHandler(this.BtnExport_Click);
            // 
            // dgvInventory
            // 
            this.dgvInventory.AllowUserToAddRows = false;
            this.dgvInventory.AllowUserToDeleteRows = false;
            this.dgvInventory.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(249)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvInventory.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvInventory.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvInventory.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(249)))), ((int)(((byte)(255)))));
            this.dgvInventory.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvInventory.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvInventory.ColumnHeadersHeight = 38;
            this.dgvInventory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(254)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvInventory.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvInventory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvInventory.EnableHeadersVisualStyles = false;
            this.dgvInventory.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.dgvInventory.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(254)))));
            this.dgvInventory.Location = new System.Drawing.Point(0, 100);
            this.dgvInventory.MultiSelect = false;
            this.dgvInventory.Name = "dgvInventory";
            this.dgvInventory.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(249)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvInventory.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvInventory.RowHeadersVisible = false;
            this.dgvInventory.RowHeadersWidth = 51;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.dgvInventory.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvInventory.RowTemplate.Height = 32;
            this.dgvInventory.SelectedIndex = -1;
            this.dgvInventory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvInventory.Size = new System.Drawing.Size(1200, 423);
            this.dgvInventory.TabIndex = 1;
            // 
            // pnlStats
            // 
            this.pnlStats.Controls.Add(this.lblTotalProducts);
            this.pnlStats.Controls.Add(this.lblLowStock);
            this.pnlStats.Controls.Add(this.lblOutOfStock);
            this.pnlStats.Controls.Add(this.lblTotalValue);
            this.pnlStats.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlStats.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.pnlStats.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.pnlStats.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pnlStats.Location = new System.Drawing.Point(0, 523);
            this.pnlStats.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlStats.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlStats.Name = "pnlStats";
            this.pnlStats.Padding = new System.Windows.Forms.Padding(15, 10, 15, 10);
            this.pnlStats.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.pnlStats.Size = new System.Drawing.Size(1200, 50);
            this.pnlStats.TabIndex = 2;
            this.pnlStats.Text = null;
            this.pnlStats.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTotalProducts
            // 
            this.lblTotalProducts.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblTotalProducts.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTotalProducts.ForeColor = System.Drawing.Color.Black;
            this.lblTotalProducts.Location = new System.Drawing.Point(20, 10);
            this.lblTotalProducts.Name = "lblTotalProducts";
            this.lblTotalProducts.Size = new System.Drawing.Size(204, 28);
            this.lblTotalProducts.TabIndex = 0;
            this.lblTotalProducts.Text = "Tổng SP: 0";
            this.lblTotalProducts.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblLowStock
            // 
            this.lblLowStock.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblLowStock.ForeColor = System.Drawing.Color.Black;
            this.lblLowStock.Location = new System.Drawing.Point(275, 7);
            this.lblLowStock.Name = "lblLowStock";
            this.lblLowStock.Size = new System.Drawing.Size(209, 31);
            this.lblLowStock.TabIndex = 1;
            this.lblLowStock.Text = "Sắp hết: 0";
            this.lblLowStock.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblOutOfStock
            // 
            this.lblOutOfStock.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblOutOfStock.ForeColor = System.Drawing.Color.Black;
            this.lblOutOfStock.Location = new System.Drawing.Point(558, 9);
            this.lblOutOfStock.Name = "lblOutOfStock";
            this.lblOutOfStock.Size = new System.Drawing.Size(217, 29);
            this.lblOutOfStock.TabIndex = 2;
            this.lblOutOfStock.Text = "Hết hàng: 0";
            this.lblOutOfStock.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTotalValue
            // 
            this.lblTotalValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTotalValue.ForeColor = System.Drawing.Color.Black;
            this.lblTotalValue.Location = new System.Drawing.Point(816, 7);
            this.lblTotalValue.Name = "lblTotalValue";
            this.lblTotalValue.Size = new System.Drawing.Size(345, 33);
            this.lblTotalValue.TabIndex = 3;
            this.lblTotalValue.Text = "Giá trị: 0 VNĐ";
            this.lblTotalValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // InventoryForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1200, 573);
            this.Controls.Add(this.dgvInventory);
            this.Controls.Add(this.pnlStats);
            this.Controls.Add(this.pnlTop);
            this.Name = "InventoryForm";
            this.Text = "Quản lý kho hàng";
            this.pnlTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInventory)).EndInit();
            this.pnlStats.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}

