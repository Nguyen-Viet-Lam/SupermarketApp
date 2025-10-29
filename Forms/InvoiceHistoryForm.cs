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
    public partial class InvoiceHistoryForm : Form
    {
        private UIPanel pnlTop;
        private UILabel lblTitle;
        private UILabel lblFromDate;
        private UILabel lblToDate;
        private DateTimePicker dtpFromDate;
        private DateTimePicker dtpToDate;
        private UIButton btnSearch;
        private UIButton btnRefresh;
        private UIButton btnViewDetail;
        private UIButton btnPrint;
        private UIButton btnDelete;
        private UIDataGridView dgvInvoices;
        private UIPanel pnlStats;
        private UILabel lblTotalInvoices;
        private UILabel lblTotalRevenue;
        private int? selectedInvoiceId = null;

        public InvoiceHistoryForm()
        {
            InitializeComponent();
            this.Load += async (s, e) => await LoadDataAsync();
        }

        private async Task LoadDataAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            using (var db = new SupermarketContext())
            {
                var query = db.HoaDon.AsQueryable();

                if (fromDate.HasValue)
                {
                    query = query.Where(x => x.NgayLap >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    var endDate = toDate.Value.Date.AddDays(1);
                    query = query.Where(x => x.NgayLap < endDate);
                }

                var data = await Task.Run(() => query
                    .OrderByDescending(x => x.NgayLap)
                    .Select(x => new
                    {
                        x.MaHD,
                        x.NgayLap,
                        NhanVien = x.NhanVien.TenNV,
                        KhachHang = x.MaKH.HasValue ? x.KhachHang.TenKH : "Khách lẻ",
                        x.TongTien,
                        SoSanPham = x.ChiTiets.Count
                    })
                    .ToList());

                dgvInvoices.DataSource = data;

                if (dgvInvoices.Columns.Count > 0)
                {
                    dgvInvoices.Columns["MaHD"].HeaderText = "Mã HĐ";
                    dgvInvoices.Columns["MaHD"].Width = 80;
                    dgvInvoices.Columns["NgayLap"].HeaderText = "Ngày lập";
                    dgvInvoices.Columns["NgayLap"].Width = 150;
                    dgvInvoices.Columns["NgayLap"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                    dgvInvoices.Columns["NhanVien"].HeaderText = "Nhân viên";
                    dgvInvoices.Columns["NhanVien"].Width = 150;
                    dgvInvoices.Columns["KhachHang"].HeaderText = "Khách hàng";
                    dgvInvoices.Columns["KhachHang"].Width = 180;
                    dgvInvoices.Columns["TongTien"].HeaderText = "Tổng tiền";
                    dgvInvoices.Columns["TongTien"].Width = 130;
                    dgvInvoices.Columns["TongTien"].DefaultCellStyle.Format = "N0";
                    dgvInvoices.Columns["SoSanPham"].HeaderText = "Số SP";
                    dgvInvoices.Columns["SoSanPham"].Width = 80;
                }

                // Update stats
                int totalCount = data.Count;
                decimal totalRevenue = data.Sum(x => x.TongTien);
                lblTotalInvoices.Text = $"Tổng số HĐ: {totalCount}";
                lblTotalRevenue.Text = $"Tổng doanh thu: {totalRevenue:N0} VNĐ";
            }
        }

        private void DgvInvoices_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvInvoices.Rows[e.RowIndex].Cells["MaHD"].Value != null)
            {
                selectedInvoiceId = Convert.ToInt32(dgvInvoices.Rows[e.RowIndex].Cells["MaHD"].Value);
            }
        }

        private async void BtnSearch_Click(object sender, EventArgs e)
        {
            DateTime fromDate = dtpFromDate.Value.Date;
            DateTime toDate = dtpToDate.Value.Date;

            if (fromDate > toDate)
            {
                MessageHelper.ShowWarning("Ngày bắt đầu phải nhỏ hơn ngày kết thúc!");
                return;
            }

            await LoadDataAsync(fromDate, toDate);
        }

        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            await LoadDataAsync();
            MessageHelper.ShowTipSuccess("Đã tải lại dữ liệu!");
        }

        private void BtnViewDetail_Click(object sender, EventArgs e)
        {
            if (selectedInvoiceId == null)
            {
                MessageHelper.ShowWarning("Vui lòng chọn hóa đơn cần xem!");
                return;
            }

            try
            {
                using (var db = new SupermarketContext())
                {
                    var invoice = db.HoaDon.Find(selectedInvoiceId.Value);
                if (invoice == null)
                {
                    MessageHelper.ShowWarning("Không tìm thấy hóa đơn!");
                    return;
                }

                    var chiTiets = db.CTHoaDon
                        .Where(x => x.MaHD == selectedInvoiceId.Value)
                        .Select(x => new
                        {
                            x.SanPham.TenSP,
                            x.SoLuong,
                            x.DonGiaBan,
                            ThanhTien = x.SoLuong * x.DonGiaBan
                        })
                        .ToList();

                    var nhanVien = db.NhanVien.Find(invoice.MaNV);
                    var khachHang = invoice.MaKH.HasValue ? db.KhachHang.Find(invoice.MaKH.Value) : null;

                    var message = $"HÓA ĐƠN #{invoice.MaHD}\n" +
                                $"════════════════════════════\n" +
                                $"Ngày: {invoice.NgayLap:dd/MM/yyyy HH:mm}\n" +
                                $"Nhân viên: {nhanVien?.TenNV}\n" +
                                $"Khách hàng: {(khachHang != null ? khachHang.TenKH : "Khách lẻ")}\n" +
                                $"════════════════════════════\n\n" +
                                $"CHI TIẾT SẢN PHẨM:\n";

                    foreach (var item in chiTiets)
                    {
                        message += $"• {item.TenSP}\n" +
                                  $"  SL: {item.SoLuong} x {item.DonGiaBan:N0} = {item.ThanhTien:N0} VNĐ\n";
                    }

                    message += $"\n════════════════════════════\n" +
                              $"TỔNG CỘNG: {invoice.TongTien:N0} VNĐ";

                    MessageHelper.Show(message, "Chi tiết hóa đơn");
                }
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("Lỗi: " + ex.Message);
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            if (selectedInvoiceId == null)
            {
                MessageHelper.ShowWarning("Vui lòng chọn hóa đơn cần in!");
                return;
            }

            var printDialog = new InvoicePrintDialog(selectedInvoiceId.Value);
            printDialog.ShowDialog();
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            if (selectedInvoiceId == null)
            {
                MessageHelper.ShowWarning("Vui lòng chọn hóa đơn cần xóa!");
                return;
            }

            if (!MessageHelper.ShowAsk("⚠️ CẢNH BÁO! ⚠️\n\nBạn có chắc muốn xóa hóa đơn này?\nHành động này không thể hoàn tác!"))
            {
                return;
            }

            try
            {
                using (var db = new SupermarketContext())
                {
                    // Xóa chi tiết hóa đơn trước
                    var chiTiets = db.CTHoaDon.Where(x => x.MaHD == selectedInvoiceId.Value).ToList();
                    db.CTHoaDon.RemoveRange(chiTiets);
                    await db.SaveChangesAsync();

                    // Xóa hóa đơn
                    var hoaDon = await db.HoaDon.FindAsync(selectedInvoiceId.Value);
                    if (hoaDon != null)
                    {
                        db.HoaDon.Remove(hoaDon);
                        await db.SaveChangesAsync();
                    }

                    MessageHelper.ShowSuccess("✅ Đã xóa hóa đơn thành công!");
                    await LoadDataAsync();
                }
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("❌ Lỗi khi xóa: " + ex.Message);
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
            this.lblFromDate = new Sunny.UI.UILabel();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.lblToDate = new Sunny.UI.UILabel();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.btnSearch = new Sunny.UI.UIButton();
            this.btnRefresh = new Sunny.UI.UIButton();
            this.btnViewDetail = new Sunny.UI.UIButton();
            this.btnPrint = new Sunny.UI.UIButton();
            this.btnDelete = new Sunny.UI.UIButton();
            this.dgvInvoices = new Sunny.UI.UIDataGridView();
            this.pnlStats = new Sunny.UI.UIPanel();
            this.lblTotalInvoices = new Sunny.UI.UILabel();
            this.lblTotalRevenue = new Sunny.UI.UILabel();
            this.pnlTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInvoices)).BeginInit();
            this.pnlStats.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.lblTitle);
            this.pnlTop.Controls.Add(this.lblFromDate);
            this.pnlTop.Controls.Add(this.dtpFromDate);
            this.pnlTop.Controls.Add(this.lblToDate);
            this.pnlTop.Controls.Add(this.dtpToDate);
            this.pnlTop.Controls.Add(this.btnSearch);
            this.pnlTop.Controls.Add(this.btnRefresh);
            this.pnlTop.Controls.Add(this.btnViewDetail);
            this.pnlTop.Controls.Add(this.btnPrint);
            this.pnlTop.Controls.Add(this.btnDelete);
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
            this.pnlTop.Size = new System.Drawing.Size(1227, 120);
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
            this.lblTitle.Size = new System.Drawing.Size(350, 35);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "📜 LỊCH SỬ HÓA ĐƠN";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblFromDate
            // 
            this.lblFromDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblFromDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblFromDate.Location = new System.Drawing.Point(20, 60);
            this.lblFromDate.Name = "lblFromDate";
            this.lblFromDate.Size = new System.Drawing.Size(100, 25);
            this.lblFromDate.TabIndex = 1;
            this.lblFromDate.Text = "Từ ngày:";
            this.lblFromDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFromDate.Location = new System.Drawing.Point(120, 60);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(180, 28);
            this.dtpFromDate.TabIndex = 2;
            this.dtpFromDate.Value = new System.DateTime(2025, 9, 21, 14, 49, 0, 530);
            // 
            // lblToDate
            // 
            this.lblToDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblToDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblToDate.Location = new System.Drawing.Point(320, 60);
            this.lblToDate.Name = "lblToDate";
            this.lblToDate.Size = new System.Drawing.Size(100, 25);
            this.lblToDate.TabIndex = 3;
            this.lblToDate.Text = "Đến ngày:";
            this.lblToDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dtpToDate
            // 
            this.dtpToDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpToDate.Location = new System.Drawing.Point(420, 60);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(180, 28);
            this.dtpToDate.TabIndex = 4;
            this.dtpToDate.Value = new System.DateTime(2025, 10, 21, 14, 49, 0, 530);
            // 
            // btnSearch
            // 
            this.btnSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearch.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnSearch.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnSearch.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.btnSearch.Location = new System.Drawing.Point(606, 60);
            this.btnSearch.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(120, 35);
            this.btnSearch.TabIndex = 5;
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
            this.btnRefresh.Location = new System.Drawing.Point(732, 60);
            this.btnRefresh.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(130, 35);
            this.btnRefresh.TabIndex = 6;
            this.btnRefresh.Text = "🔄 Làm mới";
            this.btnRefresh.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // btnViewDetail
            // 
            this.btnViewDetail.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnViewDetail.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.btnViewDetail.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            this.btnViewDetail.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            this.btnViewDetail.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.btnViewDetail.Location = new System.Drawing.Point(868, 60);
            this.btnViewDetail.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnViewDetail.Name = "btnViewDetail";
            this.btnViewDetail.Size = new System.Drawing.Size(130, 35);
            this.btnViewDetail.TabIndex = 7;
            this.btnViewDetail.Text = "👁️ Xem";
            this.btnViewDetail.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnViewDetail.Click += new System.EventHandler(this.BtnViewDetail_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPrint.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.btnPrint.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(58)))), ((int)(((byte)(237)))));
            this.btnPrint.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(58)))), ((int)(((byte)(237)))));
            this.btnPrint.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.btnPrint.Location = new System.Drawing.Point(1004, 60);
            this.btnPrint.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(130, 35);
            this.btnPrint.TabIndex = 8;
            this.btnPrint.Text = "🖨️ In";
            this.btnPrint.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnPrint.Click += new System.EventHandler(this.BtnPrint_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDelete.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnDelete.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.btnDelete.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.btnDelete.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.btnDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.btnDelete.Location = new System.Drawing.Point(1140, 60);
            this.btnDelete.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(87, 35);
            this.btnDelete.TabIndex = 9;
            this.btnDelete.Text = "🗑️ Xóa";
            this.btnDelete.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // dgvInvoices
            // 
            this.dgvInvoices.AllowUserToAddRows = false;
            this.dgvInvoices.AllowUserToDeleteRows = false;
            this.dgvInvoices.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(249)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.dgvInvoices.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvInvoices.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvInvoices.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(249)))), ((int)(((byte)(255)))));
            this.dgvInvoices.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvInvoices.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvInvoices.ColumnHeadersHeight = 38;
            this.dgvInvoices.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(254)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvInvoices.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvInvoices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvInvoices.EnableHeadersVisualStyles = false;
            this.dgvInvoices.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.dgvInvoices.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(254)))));
            this.dgvInvoices.Location = new System.Drawing.Point(0, 120);
            this.dgvInvoices.MultiSelect = false;
            this.dgvInvoices.Name = "dgvInvoices";
            this.dgvInvoices.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(249)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvInvoices.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvInvoices.RowHeadersVisible = false;
            this.dgvInvoices.RowHeadersWidth = 51;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.dgvInvoices.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvInvoices.RowTemplate.Height = 32;
            this.dgvInvoices.SelectedIndex = -1;
            this.dgvInvoices.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvInvoices.Size = new System.Drawing.Size(1227, 480);
            this.dgvInvoices.TabIndex = 1;
            this.dgvInvoices.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvInvoices_CellClick);
            // 
            // pnlStats
            // 
            this.pnlStats.Controls.Add(this.lblTotalInvoices);
            this.pnlStats.Controls.Add(this.lblTotalRevenue);
            this.pnlStats.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlStats.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.pnlStats.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.pnlStats.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pnlStats.Location = new System.Drawing.Point(0, 600);
            this.pnlStats.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlStats.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlStats.Name = "pnlStats";
            this.pnlStats.Padding = new System.Windows.Forms.Padding(15, 10, 15, 10);
            this.pnlStats.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.pnlStats.Size = new System.Drawing.Size(1227, 50);
            this.pnlStats.TabIndex = 2;
            this.pnlStats.Text = null;
            this.pnlStats.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTotalInvoices
            // 
            this.lblTotalInvoices.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTotalInvoices.ForeColor = System.Drawing.Color.Black;
            this.lblTotalInvoices.Location = new System.Drawing.Point(20, 12);
            this.lblTotalInvoices.Name = "lblTotalInvoices";
            this.lblTotalInvoices.Size = new System.Drawing.Size(300, 26);
            this.lblTotalInvoices.TabIndex = 0;
            this.lblTotalInvoices.Text = "Tổng số HĐ: 0";
            this.lblTotalInvoices.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTotalRevenue
            // 
            this.lblTotalRevenue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTotalRevenue.ForeColor = System.Drawing.Color.Black;
            this.lblTotalRevenue.Location = new System.Drawing.Point(700, 12);
            this.lblTotalRevenue.Name = "lblTotalRevenue";
            this.lblTotalRevenue.Size = new System.Drawing.Size(480, 26);
            this.lblTotalRevenue.TabIndex = 1;
            this.lblTotalRevenue.Text = "Tổng doanh thu: 0 VNĐ";
            this.lblTotalRevenue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // InvoiceHistoryForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1227, 650);
            this.Controls.Add(this.dgvInvoices);
            this.Controls.Add(this.pnlStats);
            this.Controls.Add(this.pnlTop);
            this.Name = "InvoiceHistoryForm";
            this.Text = "Lịch sử hóa đơn";
            this.pnlTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInvoices)).EndInit();
            this.pnlStats.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}

