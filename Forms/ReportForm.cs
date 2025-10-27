using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using Sunny.UI;
using SupermarketApp.Utils;
using SupermarketApp.Data;
using SupermarketApp.Data.Models;
using OfficeOpenXml;
using SupermarketApp.Services;

namespace SupermarketApp.Forms
{
    public partial class ReportForm : Form
    {
        private UIPanel pnlTop;
        private UIPanel pnlStats;
        private UILabel lblTitle;
        private UILabel lblFromDate;
        private UILabel lblToDate;
        private System.Windows.Forms.DateTimePicker dtFrom;
        private System.Windows.Forms.DateTimePicker dtTo;
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
            this.Load += (s,e)=> LoadData();
        }

        private class TopCustomerDto
        {
            public int XepHang { get; set; }
            public string TenKH { get; set; }
            public string Loai { get; set; } // Loại khách hàng: vãng lai, thân quen, VIP...
            public int SoHoaDon { get; set; }
            public decimal TongTien { get; set; }
            public int DiemTichLuy { get; set; }
        }

        private void LoadData()
        {
            try
            {
                DateTime s = dtFrom.Value.Date;
                DateTime e = dtTo.Value.Date.AddDays(1);
                
            using (var db = new SupermarketContext())
            {
                    var q = db.HoaDon
                        .AsNoTracking()
                        .Where(x => x.NgayLap >= s.ToUniversalTime() && x.NgayLap < e.ToUniversalTime())
                        .GroupBy(x => x.NgayLap.Date)
                        .Select(g => new
                        {
                            Ngay = g.Key,
                            TongDoanhThu = g.Sum(x => x.TongTien),
                            SoHoaDon = g.Count()
                        })
                                 .OrderByDescending(x => x.Ngay)
                                 .ToList();
                    
                    dgvReport.DataSource = q.Select(x => new
                    {
                        Ngày = x.Ngay.ToString("dd/MM/yyyy"),
                        DoanhThu = x.TongDoanhThu,
                        SốHoáĐơn = x.SoHoaDon,
                        TrungBình = x.SoHoaDon > 0 ? x.TongDoanhThu / x.SoHoaDon : 0
                    }).ToList(); dgvReport.ClearSelection();
                    
                    if (dgvReport.Columns.Count > 0)
                    {
                        dgvReport.Columns["Ngày"].HeaderText = "Ngày";
                        dgvReport.Columns["Ngày"].Width = 120;
                        dgvReport.Columns["DoanhThu"].HeaderText = "Doanh thu (VNĐ)";
                        dgvReport.Columns["DoanhThu"].Width = 180;
                        dgvReport.Columns["DoanhThu"].DefaultCellStyle.Format = "N0";
                        dgvReport.Columns["SốHoáĐơn"].HeaderText = "Số hóa đơn";
                        dgvReport.Columns["SốHoáĐơn"].Width = 120;
                        dgvReport.Columns["TrungBình"].HeaderText = "TB/hóa đơn (VNĐ)";
                        dgvReport.Columns["TrungBình"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        dgvReport.Columns["TrungBình"].DefaultCellStyle.Format = "N0";
                    }
                    
                    decimal totalRevenue = q.Sum(x => x.TongDoanhThu);
                    int totalOrders = q.Sum(x => x.SoHoaDon);
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
            if (dtFrom.Value > dtTo.Value)
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
                    MessageHelper.ShowWarning("Không có dữ liệu để xuất!");
                    return;
                }

                using (var sfd = new SaveFileDialog
                {
                    Title = "Xuất báo cáo",
                    Filter = "Excel file (*.xlsx)|*.xlsx|CSV file (*.csv)|*.csv",
                    FileName = $"TopKH_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
                })
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        if (sfd.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                        {
                            ExportGridToExcel(sfd.FileName);
                        }
                        else if (sfd.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                        {
                            ExportGridToCsv(sfd.FileName);
                        }
                        MessageHelper.ShowSuccess("Đã xuất báo cáo thành công!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("Lỗi: " + ex.Message);
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

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlTop = new Sunny.UI.UIPanel();
            this.btnExport = new Sunny.UI.UIButton();
            this.btnLoad = new Sunny.UI.UIButton();
            this.dtTo = new System.Windows.Forms.DateTimePicker();
            this.lblToDate = new Sunny.UI.UILabel();
            this.dtFrom = new System.Windows.Forms.DateTimePicker();
            this.lblFromDate = new Sunny.UI.UILabel();
            this.lblTitle = new Sunny.UI.UILabel();
            this.btnCustomerStats = new Sunny.UI.UIButton();
            this.btnTopProducts = new Sunny.UI.UIButton();
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
            this.pnlTop.Controls.Add(this.btnExport);
            this.pnlTop.Controls.Add(this.btnLoad);
            this.pnlTop.Controls.Add(this.dtTo);
            this.pnlTop.Controls.Add(this.lblToDate);
            this.pnlTop.Controls.Add(this.dtFrom);
            this.pnlTop.Controls.Add(this.lblFromDate);
            this.pnlTop.Controls.Add(this.lblTitle);
            this.pnlTop.Controls.Add(this.btnCustomerStats);
            this.pnlTop.Controls.Add(this.btnTopProducts);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.FillColor = System.Drawing.Color.White;
            this.pnlTop.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlTop.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Padding = new System.Windows.Forms.Padding(15);
            this.pnlTop.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.pnlTop.Size = new System.Drawing.Size(1169, 110);
            this.pnlTop.TabIndex = 0;
            this.pnlTop.Text = null;
            this.pnlTop.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnExport
            // 
            this.btnExport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExport.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(197)))), ((int)(((byte)(94)))));
            this.btnExport.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(74)))));
            this.btnExport.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(74)))));
            this.btnExport.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(128)))), ((int)(((byte)(61)))));
            this.btnExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnExport.Location = new System.Drawing.Point(960, 15);
            this.btnExport.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(171, 35);
            this.btnExport.TabIndex = 6;
            this.btnExport.Text = "📊 Xuất Excel/CSV";
            this.btnExport.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnExport.Click += new System.EventHandler(this.BtnExport_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLoad.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(72)))), ((int)(((byte)(153)))));
            this.btnLoad.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(39)))), ((int)(((byte)(119)))));
            this.btnLoad.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(39)))), ((int)(((byte)(119)))));
            this.btnLoad.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(24)))), ((int)(((byte)(93)))));
            this.btnLoad.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnLoad.Location = new System.Drawing.Point(605, 52);
            this.btnLoad.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(164, 35);
            this.btnLoad.TabIndex = 5;
            this.btnLoad.Text = "🔍 Xem báo cáo";
            this.btnLoad.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnLoad.Click += new System.EventHandler(this.BtnLoad_Click);
            // 
            // dtTo
            // 
            this.dtTo.CustomFormat = "dd/MM/yyyy";
            this.dtTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.dtTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtTo.Location = new System.Drawing.Point(409, 55);
            this.dtTo.Name = "dtTo";
            this.dtTo.Size = new System.Drawing.Size(180, 30);
            this.dtTo.TabIndex = 4;
            // 
            // lblToDate
            // 
            this.lblToDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblToDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblToDate.Location = new System.Drawing.Point(302, 62);
            this.lblToDate.Name = "lblToDate";
            this.lblToDate.Size = new System.Drawing.Size(100, 25);
            this.lblToDate.TabIndex = 3;
            this.lblToDate.Text = "Đến ngày";
            this.lblToDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dtFrom
            // 
            this.dtFrom.CustomFormat = "dd/MM/yyyy";
            this.dtFrom.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.dtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtFrom.Location = new System.Drawing.Point(115, 52);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Size = new System.Drawing.Size(180, 30);
            this.dtFrom.TabIndex = 2;
            this.dtFrom.Value = new System.DateTime(2025, 9, 21, 0, 0, 0, 0);
            // 
            // lblFromDate
            // 
            this.lblFromDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblFromDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblFromDate.Location = new System.Drawing.Point(8, 62);
            this.lblFromDate.Name = "lblFromDate";
            this.lblFromDate.Size = new System.Drawing.Size(100, 25);
            this.lblFromDate.TabIndex = 1;
            this.lblFromDate.Text = "Từ ngày";
            this.lblFromDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblFromDate.Click += new System.EventHandler(this.lblFromDate_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(72)))), ((int)(((byte)(153)))));
            this.lblTitle.Location = new System.Drawing.Point(15, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(404, 35);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "📊 BÁO CÁO DOANH THU";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnCustomerStats
            // 
            this.btnCustomerStats.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCustomerStats.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.btnCustomerStats.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(58)))), ((int)(((byte)(237)))));
            this.btnCustomerStats.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(58)))), ((int)(((byte)(237)))));
            this.btnCustomerStats.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.btnCustomerStats.Location = new System.Drawing.Point(775, 52);
            this.btnCustomerStats.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnCustomerStats.Name = "btnCustomerStats";
            this.btnCustomerStats.Size = new System.Drawing.Size(150, 35);
            this.btnCustomerStats.TabIndex = 5;
            this.btnCustomerStats.Text = "👥 Top KH";
            this.btnCustomerStats.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnCustomerStats.Click += new System.EventHandler(this.BtnCustomerStats_Click);
            // 
            // btnTopProducts
            // 
            this.btnTopProducts.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTopProducts.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.btnTopProducts.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            this.btnTopProducts.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            this.btnTopProducts.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.btnTopProducts.Location = new System.Drawing.Point(950, 52);
            this.btnTopProducts.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnTopProducts.Name = "btnTopProducts";
            this.btnTopProducts.Size = new System.Drawing.Size(150, 35);
            this.btnTopProducts.TabIndex = 4;
            this.btnTopProducts.Text = "🏆 Top SP";
            this.btnTopProducts.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnTopProducts.Click += new System.EventHandler(this.BtnTopProducts_Click);
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
            this.pnlStats.Padding = new System.Windows.Forms.Padding(15);
            this.pnlStats.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.pnlStats.Size = new System.Drawing.Size(1169, 90);
            this.pnlStats.TabIndex = 1;
            this.pnlStats.Text = null;
            this.pnlStats.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAvgOrderValue
            // 
            this.lblAvgOrderValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold);
            this.lblAvgOrderValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(197)))), ((int)(((byte)(94)))));
            this.lblAvgOrderValue.Location = new System.Drawing.Point(625, 40);
            this.lblAvgOrderValue.Name = "lblAvgOrderValue";
            this.lblAvgOrderValue.Size = new System.Drawing.Size(300, 35);
            this.lblAvgOrderValue.TabIndex = 5;
            this.lblAvgOrderValue.Text = "0 VNĐ";
            this.lblAvgOrderValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAvgOrder
            // 
            this.lblAvgOrder.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblAvgOrder.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblAvgOrder.Location = new System.Drawing.Point(625, 15);
            this.lblAvgOrder.Name = "lblAvgOrder";
            this.lblAvgOrder.Size = new System.Drawing.Size(300, 25);
            this.lblAvgOrder.TabIndex = 4;
            this.lblAvgOrder.Text = "📈 TRUNG BÌNH MỖI HÓA ĐƠN";
            this.lblAvgOrder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTotalOrdersValue
            // 
            this.lblTotalOrdersValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold);
            this.lblTotalOrdersValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.lblTotalOrdersValue.Location = new System.Drawing.Point(355, 40);
            this.lblTotalOrdersValue.Name = "lblTotalOrdersValue";
            this.lblTotalOrdersValue.Size = new System.Drawing.Size(250, 35);
            this.lblTotalOrdersValue.TabIndex = 3;
            this.lblTotalOrdersValue.Text = "0";
            this.lblTotalOrdersValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTotalOrders
            // 
            this.lblTotalOrders.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblTotalOrders.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblTotalOrders.Location = new System.Drawing.Point(355, 15);
            this.lblTotalOrders.Name = "lblTotalOrders";
            this.lblTotalOrders.Size = new System.Drawing.Size(250, 25);
            this.lblTotalOrders.TabIndex = 2;
            this.lblTotalOrders.Text = "🧾 TỔNG SỐ HÓA ĐƠN";
            this.lblTotalOrders.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTotalRevenueValue
            // 
            this.lblTotalRevenueValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold);
            this.lblTotalRevenueValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(72)))), ((int)(((byte)(153)))));
            this.lblTotalRevenueValue.Location = new System.Drawing.Point(15, 40);
            this.lblTotalRevenueValue.Name = "lblTotalRevenueValue";
            this.lblTotalRevenueValue.Size = new System.Drawing.Size(320, 35);
            this.lblTotalRevenueValue.TabIndex = 1;
            this.lblTotalRevenueValue.Text = "0 VNĐ";
            this.lblTotalRevenueValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTotalRevenue
            // 
            this.lblTotalRevenue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblTotalRevenue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblTotalRevenue.Location = new System.Drawing.Point(15, 15);
            this.lblTotalRevenue.Name = "lblTotalRevenue";
            this.lblTotalRevenue.Size = new System.Drawing.Size(320, 25);
            this.lblTotalRevenue.TabIndex = 0;
            this.lblTotalRevenue.Text = "💰 TỔNG DOANH THU";
            this.lblTotalRevenue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dgvReport
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(242)))), ((int)(((byte)(248)))));
            this.dgvReport.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvReport.BackgroundColor = System.Drawing.Color.White;
            this.dgvReport.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(72)))), ((int)(((byte)(153)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(72)))), ((int)(((byte)(153)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvReport.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvReport.ColumnHeadersHeight = 40;
            this.dgvReport.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(207)))), ((int)(((byte)(232)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvReport.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvReport.EnableHeadersVisualStyles = false;
            this.dgvReport.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.dgvReport.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(207)))), ((int)(((byte)(232)))));
            this.dgvReport.Location = new System.Drawing.Point(0, 200);
            this.dgvReport.Name = "dgvReport";
            this.dgvReport.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(242)))), ((int)(((byte)(248)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(72)))), ((int)(((byte)(153)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvReport.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvReport.RowHeadersWidth = 51;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.dgvReport.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvReport.RowTemplate.Height = 35;
            this.dgvReport.SelectedIndex = -1;
            this.dgvReport.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvReport.Size = new System.Drawing.Size(1169, 400);
            this.dgvReport.StripeOddColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(242)))), ((int)(((byte)(248)))));
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
            this.Text = "Báo cáo doanh thu";
            this.pnlTop.ResumeLayout(false);
            this.pnlStats.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvReport)).EndInit();
            this.ResumeLayout(false);

        }

        private void LoadTopProducts()
        {
            try
            {
                DateTime s = dtFrom.Value.Date;
                DateTime e = dtTo.Value.Date.AddDays(1);
                
                using (var db = new SupermarketContext())
                {
                    // B1: Group theo MaSP (ID) để EF translate tốt
                    var stats = db.CTHoaDon
                        .AsNoTracking()
                        .Where(x => x.HoaDon.NgayLap >= s.ToUniversalTime() && x.HoaDon.NgayLap < e.ToUniversalTime())
                        .GroupBy(x => x.MaSP)
                        .Select(g => new
                        {
                            MaSP = g.Key,
                            SoLuongBan = g.Sum(x => x.SoLuong),
                            DoanhThu = g.Sum(x => x.SoLuong * x.DonGiaBan)
                        })
                        .OrderByDescending(x => x.SoLuongBan)
                        .Take(20)
                        .ToList();

                    var ids = stats.Select(x => x.MaSP).ToList();
                    var products = db.SanPham
                        .AsNoTracking()
                        .Where(p => ids.Contains(p.MaSP))
                        .Select(p => new { p.MaSP, p.TenSP })
                        .ToList();

                    var topProducts = (from s1 in stats
                                       join p in products on s1.MaSP equals p.MaSP
                                       select new
                                       {
                                           TenSP = p.TenSP,
                                           SoLuongBan = s1.SoLuongBan,
                                           DoanhThu = s1.DoanhThu
                                       })
                                       .OrderByDescending(x => x.SoLuongBan)
                                       .ToList();

                    dgvReport.DataSource = topProducts; dgvReport.ClearSelection();

                    if (dgvReport.Columns.Count > 0)
                    {
                        dgvReport.Columns["TenSP"].HeaderText = "Tên sản phẩm";
                        dgvReport.Columns["TenSP"].Width = 300;
                        dgvReport.Columns["SoLuongBan"].HeaderText = "SL bán";
                        dgvReport.Columns["SoLuongBan"].Width = 100;
                        dgvReport.Columns["DoanhThu"].HeaderText = "Doanh thu";
                        dgvReport.Columns["DoanhThu"].Width = 150;
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
                DateTime s = dtFrom.Value.Date;
                DateTime e = dtTo.Value.Date.AddDays(1);
                
                using (var db = new SupermarketContext())
                {
                    var results = new System.Collections.Generic.List<TopCustomerDto>();
                    try
                    {
                        //  Ưu tiên dùng SQL thuần (bao gồm hóa đơn không có MaKH gộp thành 'Khách vãng lai')
                        var sql = @"
SELECT TOP 20
       COALESCE(k.TenKH, N'Khách vãng lai') AS TenKH,
       COALESCE(k.LoaiKhachHang, N'Khách vãng lai') AS Loai,
       COUNT(*) AS SoHoaDon,
       SUM(h.TongTien) AS TongTien,
       COALESCE(MAX(k.DiemTichLuy), 0) AS DiemTichLuy
FROM HOADON h
LEFT JOIN KHACHHANG k ON k.MaKH = h.MaKH
WHERE h.NgayLap >= @s AND h.NgayLap < @e
GROUP BY COALESCE(k.TenKH, N'Khách vãng lai'), COALESCE(k.LoaiKhachHang, N'Khách vãng lai')
ORDER BY TongTien DESC";

                        var conn = db.Database.GetDbConnection();
                        if (conn.State != System.Data.ConnectionState.Open) conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = sql;

                            var p1 = cmd.CreateParameter();
                            p1.ParameterName = "@s";
                            p1.Value = s.ToUniversalTime();
                            cmd.Parameters.Add(p1);

                            var p2 = cmd.CreateParameter();
                            p2.ParameterName = "@e";
                            p2.Value = e.ToUniversalTime();
                            cmd.Parameters.Add(p2);

                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    results.Add(new TopCustomerDto
                                    {
                                        TenKH = reader.IsDBNull(0) ? null : reader.GetString(0),
                                        Loai = reader.IsDBNull(1) ? null : reader.GetString(1),
                                        SoHoaDon = reader.IsDBNull(2) ? 0 : Convert.ToInt32(reader.GetValue(2)),
                                        TongTien = reader.IsDBNull(3) ? 0 : Convert.ToDecimal(reader.GetValue(3)),
                                        DiemTichLuy = reader.IsDBNull(4) ? 0 : Convert.ToInt32(reader.GetValue(4))
                                    });
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Fallback: EF + group phía client (giới hạn theo khoảng ngày)
                        var sUtc = s.ToUniversalTime();
                        var eUtc = e.ToUniversalTime();
                        
                        var baseRows = db.HoaDon
                            .AsNoTracking()
                            .Where(x => x.NgayLap >= sUtc && x.NgayLap < eUtc)
                            .Select(x => new { MaKH = x.MaKH, TenKH = x.KhachHang != null ? x.KhachHang.TenKH : null, Loai = x.KhachHang != null ? x.KhachHang.LoaiKhachHang : null, Diem = x.KhachHang != null ? x.KhachHang.DiemTichLuy : 0, TongTien = x.TongTien })
                            .ToList();
                        
                        var grouped = baseRows
                            .GroupBy(r => new { MaKHKey = r.MaKH ?? 0, TenKey = r.TenKH ?? "Khách vãng lai" })
                            .Select(g => new TopCustomerDto
                            {
                                TenKH = g.Key.TenKey,
                                Loai = g.Select(x => x.Loai ?? "Khách vãng lai").FirstOrDefault(),
                                SoHoaDon = g.Count(),
                                TongTien = g.Sum(x => x.TongTien),
                                DiemTichLuy = g.Max(x => x.Diem)
                            })
                            .OrderByDescending(x => x.TongTien)
                            .Take(20)
                            .ToList();

                        results = grouped;
                    }

                    // Thêm xếp hạng cho kết quả
                    var rankedResults = results.Select((r, index) => new TopCustomerDto
                    {
                        XepHang = index + 1,
                        TenKH = r.TenKH,
                        Loai = r.Loai,
                        SoHoaDon = r.SoHoaDon,
                        TongTien = r.TongTien,
                        DiemTichLuy = r.DiemTichLuy
                    }).ToList();

                    dgvReport.DataSource = rankedResults;
                    dgvReport.ClearSelection();

                    if (dgvReport.Columns.Count > 0)
                    {
                        dgvReport.Columns["XepHang"].HeaderText = "Xếp hạng";
                        dgvReport.Columns["XepHang"].Width = 80;
                        dgvReport.Columns["XepHang"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        
                        dgvReport.Columns["TenKH"].HeaderText = "Tên khách hàng";
                        dgvReport.Columns["TenKH"].Width = 200;

                        if (dgvReport.Columns.Contains("Loai"))
                        {
                            dgvReport.Columns["Loai"].HeaderText = "Loại KH";
                            dgvReport.Columns["Loai"].Width = 120;
                        }

                        dgvReport.Columns["SoHoaDon"].HeaderText = "Số HĐ";
                        dgvReport.Columns["SoHoaDon"].Width = 80;
                        dgvReport.Columns["SoHoaDon"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        
                        dgvReport.Columns["TongTien"].HeaderText = "Tổng tiền (VNĐ)";
                        dgvReport.Columns["TongTien"].Width = 150;
                        dgvReport.Columns["TongTien"].DefaultCellStyle.Format = "N0";
                        dgvReport.Columns["TongTien"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        
                        if (dgvReport.Columns.Contains("DiemTichLuy"))
                        {
                            dgvReport.Columns["DiemTichLuy"].HeaderText = "Điểm tích lũy";
                            dgvReport.Columns["DiemTichLuy"].Width = 120;
                            dgvReport.Columns["DiemTichLuy"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }
                    }

                    // Cập nhật thống kê
                    decimal totalRevenue = results.Sum(x => x.TongTien);
                    int totalOrders = results.Sum(x => x.SoHoaDon);
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

        private void lblFromDate_Click(object sender, EventArgs e)
        {

        }

        private void ExportGridToCsv(string filePath)
        {
            var encoding = new UTF8Encoding(true); // BOM for Excel UTF-8
            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new StreamWriter(stream, encoding))
            {
                // Header
                var header = string.Join(",",
                    dgvReport.Columns
                        .Cast<DataGridViewColumn>()
                        .Select(c => EscapeCsv(c.HeaderText)));
                writer.WriteLine(header);

                // Rows
                foreach (DataGridViewRow row in dgvReport.Rows)
                {
                    if (row.IsNewRow) continue;
                    var line = string.Join(",",
                        row.Cells
                           .Cast<DataGridViewCell>()
                           .Select(cell => EscapeCsv(Convert.ToString(cell.Value))));
                    writer.WriteLine(line);
                }
            }
        }

        private void ExportGridToExcel(string filePath)
        {
            // EPPlus 4.x: LicenseContext API không tồn tại; bỏ thiết lập license.
            
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Báo cáo");
                
                // Header
                for (int i = 0; i < dgvReport.Columns.Count; i++)
                {
                    worksheet.Cells[1, i + 1].Value = dgvReport.Columns[i].HeaderText;
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                    worksheet.Cells[1, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                }
                
                // Data rows
                for (int i = 0; i < dgvReport.Rows.Count; i++)
                {
                    var row = dgvReport.Rows[i];
                    if (row.IsNewRow) continue;
                    
                    for (int j = 0; j < dgvReport.Columns.Count; j++)
                    {
                        var cellValue = row.Cells[j].Value;
                        if (cellValue != null)
                        {
                            // Handle numeric values
                            if (cellValue is decimal || cellValue is double || cellValue is int || cellValue is long || cellValue is float)
                            {
                                worksheet.Cells[i + 2, j + 1].Value = Convert.ToDouble(cellValue);
                            }
                            else
                            {
                                var str = cellValue.ToString();
                                if (double.TryParse(str, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CurrentCulture, out var num))
                                {
                                    worksheet.Cells[i + 2, j + 1].Value = num;
                                }
                                else
                                {
                                    worksheet.Cells[i + 2, j + 1].Value = str;
                                }
                            }
                        }
                    }
                }
                
                // Auto-fit columns
                worksheet.Cells.AutoFitColumns();
                
                // Add border to all cells
                var range = worksheet.Cells[1, 1, dgvReport.Rows.Count + 1, dgvReport.Columns.Count];
                range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                
                // Save file
                package.SaveAs(new FileInfo(filePath));
            }
        }

        private string EscapeCsv(string value)
        {
            if (value == null) return "";
            var needsQuote = value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r");
            value = value.Replace("\"", "\"\"");
            return needsQuote ? $"\"{value}\"" : value;
        }

    }
}
