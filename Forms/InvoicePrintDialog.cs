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

                    // Tạo nội dung hóa đơn - đơn giản, dễ đọc
                    var content = new System.Text.StringBuilder();
                    content.AppendLine("════════════════════════════════════════════════");
                    content.AppendLine("         SIÊU THI MINIMART");
                    content.AppendLine("    Dia chi: 123 Duong ABC, TP.HCM");
                    content.AppendLine("    Hotline: 0123 456 789");
                    content.AppendLine("════════════════════════════════════════════════");
                    content.AppendLine("           HOA DON BAN HANG");
                    content.AppendLine("════════════════════════════════════════════════");
                    content.AppendLine();
                    content.AppendLine($"Ma HD: {hd.MaHD}");
                    content.AppendLine($"Ngay: {hd.NgayLap:dd/MM/yyyy HH:mm}");
                    content.AppendLine($"Thu ngan: {nhanVien?.TenNV ?? "N/A"}");
                    if (khachHang != null)
                    {
                        content.AppendLine($"Khach hang: {khachHang.TenKH}");
                        content.AppendLine($"SDT: {khachHang.SDT}");
                        content.AppendLine($"Diem tich luy: {khachHang.DiemTichLuy}");
                    }
                    content.AppendLine();
                    content.AppendLine("────────────────────────────────────────────────");
                    content.AppendLine("STT | Ten san pham       | SL  | Don gia | T.Tien");
                    content.AppendLine("────────────────────────────────────────────────");

                    int stt = 1;
                    foreach (var item in chiTiets)
                    {
                        string tenSP = item.TenSP.Length > 20 ? item.TenSP.Substring(0, 17) + "..." : item.TenSP.PadRight(20);
                        content.AppendLine($"{stt.ToString().PadLeft(3)} | {tenSP} | {item.SoLuong.ToString().PadLeft(3)} | {item.DonGiaBan.ToString("N0").PadLeft(7)} | {item.ThanhTien.ToString("N0")}");
                        stt++;
                    }

                    content.AppendLine("────────────────────────────────────────────────");
                    content.AppendLine($"                TONG CONG: {hd.TongTien.ToString("N0")} VND");
                    content.AppendLine("────────────────────────────────────────────────");
                    content.AppendLine();
                    content.AppendLine("    ★ CAM ON QUY KHACH - HEN GAP LAI! ★");
                    content.AppendLine();
                    content.AppendLine("════════════════════════════════════════════════");

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
            // Sử dụng font hỗ trợ tiếng Việt tốt
            Font titleFont = new Font("Arial", 16, FontStyle.Bold);
            Font headerFont = new Font("Arial", 10, FontStyle.Bold);
            Font contentFont = new Font("Arial", 9, FontStyle.Regular);
            
            float yPos = 50;
            float leftMargin = 50;
            float maxWidth = e.PageBounds.Width - 100;
            
            string[] lines = invoiceContent.Split('\n');
            
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                Font currentFont = contentFont;
                
                // Xác định font phù hợp theo nội dung
                if (line.Contains("SIÊU THỊ") || line.Contains("HÓA ĐƠN"))
                {
                    currentFont = titleFont;
                }
                else if (line.Contains("STT") || line.Contains("═══") || line.Contains("════"))
                {
                    currentFont = headerFont;
                }
                
                // Vẽ text với TextRenderer để hỗ trợ Unicode tốt hơn
                TextRenderer.DrawText(e.Graphics, line, currentFont, new Point((int)leftMargin, (int)yPos), Color.Black);
                
                yPos += currentFont.GetHeight(e.Graphics);
                
                // Kiểm tra nếu hết trang
                if (yPos > e.PageBounds.Height - 50)
                {
                    e.HasMorePages = true;
                    return;
                }
            }
            
            e.HasMorePages = false;
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
            this.txtContent.Font = new System.Drawing.Font("Arial", 10F);
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

