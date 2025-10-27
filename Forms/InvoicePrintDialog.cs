using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using Sunny.UI;
using SupermarketApp.Utils;
using SupermarketApp.Data;
using SupermarketApp.Data.Models;

namespace SupermarketApp.Forms
{
    public partial class InvoicePrintDialog : UIForm
    {
        private int invoiceId;
        private UITextBox txtContent;
        private UIButton btnPrint;
        private UIButton btnClose;
        private string invoiceContent;

        public InvoicePrintDialog(int maHD)
        {
            invoiceId = maHD;
            InitializeComponent();
            LoadInvoiceData();
        }

        private void LoadInvoiceData()
        {
            try
            {
                using (var db = new SupermarketContext())
                {
                    var hd = db.HoaDon.Find(invoiceId);
                    if (hd == null)
                    {
                        MessageHelper.ShowError("Không tìm thấy hóa đơn!");
                        this.Close();
                        return;
                    }

                    var chiTiets = db.CTHoaDon
                        .Where(x => x.MaHD == invoiceId)
                        .Select(x => new {
                            x.SanPham.TenSP,
                            x.SoLuong,
                            x.DonGiaBan,
                            ThanhTien = x.SoLuong * x.DonGiaBan
                        })
                        .ToList();

                    var nhanVien = db.NhanVien.Find(hd.MaNV);
                    var khachHang = hd.MaKH.HasValue ? db.KhachHang.Find(hd.MaKH.Value) : null;

                    // Tạo nội dung hóa đơn
                    var content = new System.Text.StringBuilder();
                    content.AppendLine("╔════════════════════════════════════════════════╗");
                    content.AppendLine("║          SIÊU THỊ MINIMART                     ║");
                    content.AppendLine("║      Địa chỉ: 123 Đường ABC, TP.HCM            ║");
                    content.AppendLine("║      Hotline: 0123 456 789                     ║");
                    content.AppendLine("╠════════════════════════════════════════════════╣");
                    content.AppendLine("║              HÓA ĐƠN BÁN HÀNG                  ║");
                    content.AppendLine("╠════════════════════════════════════════════════╣");
                    content.AppendLine($"  Mã HĐ: {hd.MaHD.ToString().PadRight(20)} Ngày: {hd.NgayLap:dd/MM/yyyy HH:mm}");
                    content.AppendLine($"  Thu ngân: {nhanVien?.TenNV}");
                    if (khachHang != null)
                    {
                        content.AppendLine($"  Khách hàng: {khachHang.TenKH}");
                        content.AppendLine($"  SĐT: {khachHang.SDT}");
                        content.AppendLine($"  Điểm tích lũy: {khachHang.DiemTichLuy}");
                    }
                    content.AppendLine("────────────────────────────────────────────────");
                    content.AppendLine("  STT | Tên SP              | SL  | Đ.Giá    | T.Tiền");
                    content.AppendLine("────────────────────────────────────────────────");

                    int stt = 1;
                    foreach (var item in chiTiets)
                    {
                        string tenSP = item.TenSP.Length > 20 ? item.TenSP.Substring(0, 17) + "..." : item.TenSP.PadRight(20);
                        content.AppendLine($"  {stt.ToString().PadLeft(2)}  | {tenSP} | {item.SoLuong.ToString().PadLeft(3)} | {item.DonGiaBan.ToString("N0").PadLeft(8)} | {item.ThanhTien.ToString("N0").PadLeft(10)}");
                        stt++;
                    }

                    content.AppendLine("────────────────────────────────────────────────");
                    content.AppendLine($"                        TỔNG CỘNG: {hd.TongTien.ToString("N0").PadLeft(15)} VNĐ");
                    content.AppendLine("────────────────────────────────────────────────");
                    content.AppendLine();
                    content.AppendLine("        ★ CẢM ƠN QUÝ KHÁCH - HẸN GẶP LẠI! ★");
                    content.AppendLine();
                    content.AppendLine("╚════════════════════════════════════════════════╝");

                    invoiceContent = content.ToString();
                    txtContent.Text = invoiceContent;
                }
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("Lỗi: " + ex.Message);
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                PrintDocument printDoc = new PrintDocument();
                printDoc.PrintPage += PrintDoc_PrintPage;
                
                PrintDialog printDialog = new PrintDialog();
                printDialog.Document = printDoc;

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    printDoc.Print();
                    MessageHelper.ShowSuccess("In hóa đơn thành công!");
                }
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("Lỗi in: " + ex.Message);
            }
        }

        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            Font font = new Font("Courier New", 10);
            float yPos = 50;
            float leftMargin = 50;
            
            foreach (string line in invoiceContent.Split('\n'))
            {
                e.Graphics.DrawString(line, font, Brushes.Black, leftMargin, yPos);
                yPos += font.GetHeight(e.Graphics);
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void InitializeComponent()
        {
            this.txtContent = new Sunny.UI.UITextBox();
            this.btnPrint = new Sunny.UI.UIButton();
            this.btnClose = new Sunny.UI.UIButton();
            this.SuspendLayout();
            // 
            // txtContent
            // 
            this.txtContent.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtContent.Font = new System.Drawing.Font("Courier New", 10F);
            this.txtContent.Location = new System.Drawing.Point(20, 50);
            this.txtContent.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtContent.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtContent.Multiline = true;
            this.txtContent.Name = "txtContent";
            this.txtContent.Padding = new System.Windows.Forms.Padding(10);
            this.txtContent.ReadOnly = true;
            this.txtContent.ShowText = false;
            this.txtContent.Size = new System.Drawing.Size(874, 500);
            this.txtContent.TabIndex = 0;
            this.txtContent.TextAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.txtContent.Watermark = "";
            // 
            // btnPrint
            // 
            this.btnPrint.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPrint.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnPrint.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnPrint.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnPrint.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnPrint.Location = new System.Drawing.Point(653, 570);
            this.btnPrint.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(110, 40);
            this.btnPrint.TabIndex = 1;
            this.btnPrint.Text = "🖨️ In";
            this.btnPrint.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnPrint.Click += new System.EventHandler(this.BtnPrint_Click);
            // 
            // btnClose
            // 
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.btnClose.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.btnClose.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnClose.Location = new System.Drawing.Point(784, 570);
            this.btnClose.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(110, 40);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "❌ Đóng";
            this.btnClose.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // InvoicePrintDialog
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(911, 630);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.txtContent);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InvoicePrintDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Xem & In hóa đơn";
            this.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.ZoomScaleRect = new System.Drawing.Rectangle(19, 19, 700, 630);
            this.ResumeLayout(false);

        }
    }
}

