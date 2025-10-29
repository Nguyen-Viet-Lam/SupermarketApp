
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Sunny.UI;
using SupermarketApp.Utils;
using SupermarketApp.Data;
using SupermarketApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace SupermarketApp.Forms
{
    public partial class ReportForm : Form
    {
        private UIPanel pnlTop;
        private UIPanel pnlStats;
        private UILabel lblTitle;
        private UILabel lblFromDate;
        private UILabel lblToDate;
        private DateTimePicker dtpFromDate;
        private DateTimePicker dtpToDate;
        private UIButton btnLoad;
        private UIButton btnExport;
        private UIButton btnTopProducts;
        private UIButton btnCustomerStats;
        private UIDataGridView dgvReport;
        private UILabel lblTotalRevenue;
        private UILabel lblTotalRevenueValue;
        private UILabel lblTotalOrders;
        private UILabel lblTotalOrdersValue;
        private UILabel lblAvgOrder;
        private UILabel lblAvgOrderValue;

        public ReportForm()
        {
            InitializeComponent();
            this.Load += (s, e) => LoadData();
        }

        private void LoadData()
        {
            try
            {
                DateTime startDate = dtpFromDate.Value.Date;
                DateTime endDate = dtpToDate.Value.Date.AddDays(1);
                
                using (var db = new SupermarketContext())
                {
                    var query = db.HoaDon
                        .Where(x => x.NgayLap >= startDate && x.NgayLap < endDate)
                        .GroupBy(x => x.NgayLap.Date)
                        .Select(g => new 
                        { 
                            Ngay = g.Key, 
                            TongDoanhThu = g.Sum(x => x.TongTien),
                            SoHoaDon = g.Count() 
                        })
                        .OrderByDescending(x => x.Ngay)
                        .ToList();
                    
                    dgvReport.DataSource = query.Select(x => new 
                    { 
                        Ngay = x.Ngay.ToString("dd/MM/yyyy"),
                        DoanhThu = x.TongDoanhThu,
                        SoHoaDon = x.SoHoaDon,
                        TrungBinh = x.SoHoaDon > 0 ? x.TongDoanhThu / x.SoHoaDon : 0
                    }).ToList();
                    
                    if (dgvReport.Columns.Count > 0)
                    {
                        dgvReport.Columns["Ngay"].HeaderText = "Ngày";
                        dgvReport.Columns["DoanhThu"].HeaderText = "Doanh thu (VNĐ)";
                        dgvReport.Columns["DoanhThu"].Width = 180;
                        dgvReport.Columns["DoanhThu"].DefaultCellStyle.Format = "N0";
                        dgvReport.Columns["SoHoaDon"].HeaderText = "Số hóa đơn";
                        dgvReport.Columns["SoHoaDon"].Width = 120;
                        dgvReport.Columns["TrungBinh"].HeaderText = "TB/hóa đơn (VNĐ)";
                        dgvReport.Columns["TrungBinh"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        dgvReport.Columns["TrungBinh"].DefaultCellStyle.Format = "N0";
                    }
                    
                    decimal totalRevenue = query.Sum(x => x.TongDoanhThu);
                    int totalOrders = query.Sum(x => x.SoHoaDon);
                    decimal avgOrder = totalOrders > 0 ? totalRevenue / totalOrders : 0;
                    
                    lblTotalRevenueValue.Text = $"{totalRevenue:N0} VNĐ";
                    lblTotalOrdersValue.Text = totalOrders.ToString();
                    lblAvgOrderValue.Text = $"{avgOrder:N0} VNĐ";
                }
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("Lỗi: " + ex.Message);
            }
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            if (dtpFromDate.Value > dtpToDate.Value)
            {
                MessageHelper.ShowWarning("Ngày bắt đầu phải nhỏ hơn ngày kết thúc!");
                return;
            }
            LoadData();
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvReport.DataSource == null || dgvReport.Rows.Count == 0)
                {
                    MessageHelper.ShowWarning("Không có dữ liệu để xuất CSV!");
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                    Title = "Xuất báo cáo CSV",
                    FileName = $"BaoCao_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
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
                writer.WriteLine("BÁO CÁO THỐNG KÊ SIÊU THỊ");
                writer.WriteLine($"Từ ngày: {dtpFromDate.Value:dd/MM/yyyy};Đến ngày: {dtpToDate.Value:dd/MM/yyyy};Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm}");
                writer.WriteLine(); // Dòng trống

                // Header
                var headers = new List<string>();
                for (int i = 0; i < dgvReport.Columns.Count; i++)
                {
                    headers.Add(dgvReport.Columns[i].HeaderText);
                }
                writer.WriteLine(string.Join(";", headers));

                // Data
                for (int r = 0; r < dgvReport.Rows.Count; r++)
                {
                    var rowValues = new List<string>();
                    for (int c = 0; c < dgvReport.Columns.Count; c++)
                    {
                        var val = dgvReport.Rows[r].Cells[c].Value;
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
                writer.WriteLine($"Tổng doanh thu;{lblTotalRevenueValue.Text}");
                writer.WriteLine($"Tổng số đơn hàng;{lblTotalOrdersValue.Text}");
                writer.WriteLine($"Giá trị trung bình;{lblAvgOrderValue.Text}");
            }
        }

        private void BtnTopProducts_Click(object sender, EventArgs e)
        {
            LoadTopProducts();
        }

        private void BtnCustomerStats_Click(object sender, EventArgs e)
        {
            LoadCustomerStats();
        }

        private void LoadTopProducts()
        {
            try
            {
                DateTime startDate = dtpFromDate.Value.Date;
                DateTime endDate = dtpToDate.Value.Date.AddDays(1);
                
                using (var db = new SupermarketContext())
                {
                    var topProducts = db.CTHoaDon
                        .Where(x => x.HoaDon.NgayLap >= startDate && x.HoaDon.NgayLap < endDate)
                        .GroupBy(x => new { x.MaSP, x.SanPham.TenSP })
                        .Select(g => new
                        {
                            TenSP = g.Key.TenSP,
                            SoLuongBan = g.Sum(x => x.SoLuong),
                            DoanhThu = g.Sum(x => x.SoLuong * x.DonGiaBan)
                        })
                        .OrderByDescending(x => x.SoLuongBan)
                        .Take(20)
                        .ToList();

                    dgvReport.DataSource = topProducts;

                    if (dgvReport.Columns.Count > 0)
                    {
                        dgvReport.Columns["TenSP"].HeaderText = "Tên sản phẩm";
                        dgvReport.Columns["SoLuongBan"].HeaderText = "SL bán";
                        dgvReport.Columns["DoanhThu"].HeaderText = "Doanh thu";
                        dgvReport.Columns["DoanhThu"].DefaultCellStyle.Format = "N0";
                    }

                    // Cập nhật thống kê
                    decimal totalRevenue = topProducts.Sum(x => x.DoanhThu);
                    int totalOrders = topProducts.Sum(x => x.SoLuongBan);
                    decimal avgOrder = totalOrders > 0 ? totalRevenue / totalOrders : 0;

                    lblTotalRevenueValue.Text = $"{totalRevenue:N0} VNĐ";
                    lblTotalOrdersValue.Text = totalOrders.ToString();
                    lblAvgOrderValue.Text = $"{avgOrder:N0} VNĐ";
                }
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("Lỗi: " + ex.Message);
            }
        }

        private void LoadCustomerStats()
        {
            try
            {
                DateTime startDate = dtpFromDate.Value.Date;
                DateTime endDate = dtpToDate.Value.Date.AddDays(1);
                
                using (var db = new SupermarketContext())
                {
                    // Load data first to avoid LINQ translation issues
                    var hoaDons = db.HoaDon
                        .Include(x => x.KhachHang)
                        .Where(x => x.NgayLap >= startDate && x.NgayLap < endDate && x.MaKH.HasValue)
                        .ToList();
                    
                    var customerStats = hoaDons
                        .GroupBy(x => new { x.MaKH, TenKH = x.KhachHang?.TenKH, LoaiKH = x.KhachHang?.LoaiKH })
                        .Select(g => new
                        {
                            TenKH = g.Key.TenKH,
                            LoaiKH = g.Key.LoaiKH ?? "Vãng lai",
                            SoHoaDon = g.Count(),
                            TongTien = g.Sum(x => x.TongTien),
                            DiemTichLuy = g.First().KhachHang?.DiemTichLuy ?? 0,
                            GiaTriTrungBinh = g.Average(x => x.TongTien)
                        })
                        .OrderByDescending(x => x.TongTien)
                        .Take(20)
                        .ToList();

                    dgvReport.DataSource = customerStats;

                    if (dgvReport.Columns.Count > 0)
                    {
                        dgvReport.Columns["TenKH"].HeaderText = "Tên khách hàng";
                        dgvReport.Columns["LoaiKH"].HeaderText = "Loại KH";
                        dgvReport.Columns["SoHoaDon"].HeaderText = "Số HĐ";
                        dgvReport.Columns["TongTien"].HeaderText = "Tổng tiền";
                        dgvReport.Columns["TongTien"].DefaultCellStyle.Format = "N0";
                        dgvReport.Columns["DiemTichLuy"].HeaderText = "Điểm tích lũy";
                        dgvReport.Columns["GiaTriTrungBinh"].HeaderText = "Giá trị TB/HĐ";
                        dgvReport.Columns["GiaTriTrungBinh"].DefaultCellStyle.Format = "N0";
                    }

                    // Cập nhật thống kê
                    decimal totalRevenue = customerStats.Sum(x => x.TongTien);
                    int totalOrders = customerStats.Sum(x => x.SoHoaDon);
                    decimal avgOrder = totalOrders > 0 ? totalRevenue / totalOrders : 0;

                    lblTotalRevenueValue.Text = $"{totalRevenue:N0} VNĐ";
                    lblTotalOrdersValue.Text = totalOrders.ToString();
                    lblAvgOrderValue.Text = $"{avgOrder:N0} VNĐ";
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
            this.pnlTop = new Sunny.UI.UIPanel();
            this.btnCustomerStats = new Sunny.UI.UIButton();
            this.btnTopProducts = new Sunny.UI.UIButton();
            this.btnExport = new Sunny.UI.UIButton();
            this.btnLoad = new Sunny.UI.UIButton();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.lblToDate = new Sunny.UI.UILabel();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.lblFromDate = new Sunny.UI.UILabel();
            this.lblTitle = new Sunny.UI.UILabel();
            this.pnlStats = new Sunny.UI.UIPanel();
            this.lblAvgOrderValue = new Sunny.UI.UILabel();
            this.lblAvgOrder = new Sunny.UI.UILabel();
            this.lblTotalOrdersValue = new Sunny.UI.UILabel();
            this.lblTotalOrders = new Sunny.UI.UILabel();
            this.lblTotalRevenueValue = new Sunny.UI.UILabel();
            this.lblTotalRevenue = new Sunny.UI.UILabel();
            this.dgvReport = new Sunny.UI.UIDataGridView();
            this.pnlTop.SuspendLayout();
            this.pnlStats.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReport)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.btnCustomerStats);
            this.pnlTop.Controls.Add(this.btnTopProducts);
            this.pnlTop.Controls.Add(this.btnExport);
            this.pnlTop.Controls.Add(this.btnLoad);
            this.pnlTop.Controls.Add(this.dtpToDate);
            this.pnlTop.Controls.Add(this.lblToDate);
            this.pnlTop.Controls.Add(this.dtpFromDate);
            this.pnlTop.Controls.Add(this.lblFromDate);
            this.pnlTop.Controls.Add(this.lblTitle);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.FillColor = System.Drawing.Color.White;
            this.pnlTop.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlTop.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1169, 110);
            this.pnlTop.TabIndex = 0;
            this.pnlTop.Text = null;
            this.pnlTop.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnCustomerStats
            // 
            this.btnCustomerStats.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCustomerStats.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnCustomerStats.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnCustomerStats.Location = new System.Drawing.Point(796, 62);
            this.btnCustomerStats.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnCustomerStats.Name = "btnCustomerStats";
            this.btnCustomerStats.Size = new System.Drawing.Size(158, 35);
            this.btnCustomerStats.TabIndex = 7;
            this.btnCustomerStats.Text = "👥 KH thân thiết";
            this.btnCustomerStats.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnCustomerStats.Click += new System.EventHandler(this.BtnCustomerStats_Click);
            // 
            // btnTopProducts
            // 
            this.btnTopProducts.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTopProducts.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(168)))), ((int)(((byte)(85)))), ((int)(((byte)(247)))));
            this.btnTopProducts.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnTopProducts.Location = new System.Drawing.Point(646, 62);
            this.btnTopProducts.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnTopProducts.Name = "btnTopProducts";
            this.btnTopProducts.Size = new System.Drawing.Size(144, 35);
            this.btnTopProducts.TabIndex = 6;
            this.btnTopProducts.Text = "🏆 SP bán chạy";
            this.btnTopProducts.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnTopProducts.Click += new System.EventHandler(this.BtnTopProducts_Click);
            // 
            // btnExport
            // 
            this.btnExport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExport.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(197)))), ((int)(((byte)(94)))));
            this.btnExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnExport.Location = new System.Drawing.Point(960, 62);
            this.btnExport.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(147, 35);
            this.btnExport.TabIndex = 5;
            this.btnExport.Text = "📄 Xuất CSV";
            this.btnExport.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnExport.Click += new System.EventHandler(this.BtnExport_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLoad.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(72)))), ((int)(((byte)(153)))));
            this.btnLoad.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnLoad.Location = new System.Drawing.Point(476, 62);
            this.btnLoad.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(164, 35);
            this.btnLoad.TabIndex = 4;
            this.btnLoad.Text = "🔍 Xem báo cáo";
            this.btnLoad.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnLoad.Click += new System.EventHandler(this.BtnLoad_Click);
            // 
            // dtpToDate
            // 
            this.dtpToDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpToDate.Location = new System.Drawing.Point(320, 65);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(150, 28);
            this.dtpToDate.TabIndex = 3;
            // 
            // lblToDate
            // 
            this.lblToDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblToDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblToDate.Location = new System.Drawing.Point(260, 65);
            this.lblToDate.Name = "lblToDate";
            this.lblToDate.Size = new System.Drawing.Size(50, 28);
            this.lblToDate.TabIndex = 2;
            this.lblToDate.Text = "Đến:";
            this.lblToDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFromDate.Location = new System.Drawing.Point(90, 65);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(150, 28);
            this.dtpFromDate.TabIndex = 1;
            this.dtpFromDate.Value = new System.DateTime(2025, 9, 28, 23, 28, 38, 991);
            // 
            // lblFromDate
            // 
            this.lblFromDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblFromDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblFromDate.Location = new System.Drawing.Point(20, 65);
            this.lblFromDate.Name = "lblFromDate";
            this.lblFromDate.Size = new System.Drawing.Size(60, 28);
            this.lblFromDate.TabIndex = 0;
            this.lblFromDate.Text = "Từ:";
            this.lblFromDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblTitle.Location = new System.Drawing.Point(15, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(378, 35);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "📊 BÁO CÁO DOANH THU";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlStats
            // 
            this.pnlStats.Controls.Add(this.lblAvgOrderValue);
            this.pnlStats.Controls.Add(this.lblAvgOrder);
            this.pnlStats.Controls.Add(this.lblTotalOrdersValue);
            this.pnlStats.Controls.Add(this.lblTotalOrders);
            this.pnlStats.Controls.Add(this.lblTotalRevenueValue);
            this.pnlStats.Controls.Add(this.lblTotalRevenue);
            this.pnlStats.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlStats.FillColor = System.Drawing.Color.White;
            this.pnlStats.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pnlStats.Location = new System.Drawing.Point(0, 110);
            this.pnlStats.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlStats.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlStats.Name = "pnlStats";
            this.pnlStats.Size = new System.Drawing.Size(1169, 80);
            this.pnlStats.TabIndex = 1;
            this.pnlStats.Text = null;
            this.pnlStats.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAvgOrderValue
            // 
            this.lblAvgOrderValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.lblAvgOrderValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.lblAvgOrderValue.Location = new System.Drawing.Point(600, 45);
            this.lblAvgOrderValue.Name = "lblAvgOrderValue";
            this.lblAvgOrderValue.Size = new System.Drawing.Size(250, 25);
            this.lblAvgOrderValue.TabIndex = 5;
            this.lblAvgOrderValue.Text = "0 VNĐ";
            this.lblAvgOrderValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAvgOrder
            // 
            this.lblAvgOrder.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.lblAvgOrder.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblAvgOrder.Location = new System.Drawing.Point(600, 15);
            this.lblAvgOrder.Name = "lblAvgOrder";
            this.lblAvgOrder.Size = new System.Drawing.Size(190, 25);
            this.lblAvgOrder.TabIndex = 4;
            this.lblAvgOrder.Text = "📈 Trung bình/HĐ:";
            this.lblAvgOrder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTotalOrdersValue
            // 
            this.lblTotalOrdersValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.lblTotalOrdersValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.lblTotalOrdersValue.Location = new System.Drawing.Point(365, 45);
            this.lblTotalOrdersValue.Name = "lblTotalOrdersValue";
            this.lblTotalOrdersValue.Size = new System.Drawing.Size(150, 25);
            this.lblTotalOrdersValue.TabIndex = 3;
            this.lblTotalOrdersValue.Text = "0";
            this.lblTotalOrdersValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTotalOrders
            // 
            this.lblTotalOrders.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.lblTotalOrders.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblTotalOrders.Location = new System.Drawing.Point(350, 15);
            this.lblTotalOrders.Name = "lblTotalOrders";
            this.lblTotalOrders.Size = new System.Drawing.Size(188, 25);
            this.lblTotalOrders.TabIndex = 2;
            this.lblTotalOrders.Text = "🧾 Tổng hóa đơn:";
            this.lblTotalOrders.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTotalRevenueValue
            // 
            this.lblTotalRevenueValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.lblTotalRevenueValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.lblTotalRevenueValue.Location = new System.Drawing.Point(20, 45);
            this.lblTotalRevenueValue.Name = "lblTotalRevenueValue";
            this.lblTotalRevenueValue.Size = new System.Drawing.Size(250, 25);
            this.lblTotalRevenueValue.TabIndex = 1;
            this.lblTotalRevenueValue.Text = "0 VNĐ";
            this.lblTotalRevenueValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTotalRevenue
            // 
            this.lblTotalRevenue.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.lblTotalRevenue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblTotalRevenue.Location = new System.Drawing.Point(20, 15);
            this.lblTotalRevenue.Name = "lblTotalRevenue";
            this.lblTotalRevenue.Size = new System.Drawing.Size(150, 25);
            this.lblTotalRevenue.TabIndex = 0;
            this.lblTotalRevenue.Text = "💰 Tổng doanh thu:";
            this.lblTotalRevenue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dgvReport
            // 
            this.dgvReport.AllowUserToAddRows = false;
            this.dgvReport.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvReport.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvReport.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvReport.BackgroundColor = System.Drawing.Color.White;
            this.dgvReport.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvReport.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvReport.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvReport.EnableHeadersVisualStyles = false;
            this.dgvReport.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.dgvReport.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.dgvReport.Location = new System.Drawing.Point(0, 190);
            this.dgvReport.Name = "dgvReport";
            this.dgvReport.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvReport.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvReport.RowHeadersWidth = 51;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.dgvReport.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvReport.RowTemplate.Height = 35;
            this.dgvReport.SelectedIndex = -1;
            this.dgvReport.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvReport.Size = new System.Drawing.Size(1169, 410);
            this.dgvReport.StripeOddColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.dgvReport.TabIndex = 2;
            // 
            // ReportForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.ClientSize = new System.Drawing.Size(1169, 600);
            this.Controls.Add(this.dgvReport);
            this.Controls.Add(this.pnlStats);
            this.Controls.Add(this.pnlTop);
            this.Name = "ReportForm";
            this.Text = "Báo cáo thống kê";
            this.pnlTop.ResumeLayout(false);
            this.pnlStats.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvReport)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
