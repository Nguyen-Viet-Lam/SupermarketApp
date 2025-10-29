using System.Drawing;
using System.Windows.Forms;

namespace SupermarketApp.Utils
{
    /// <summary>
    /// Helper class để set font chuẩn Unicode Segoe UI cho toàn bộ controls
    /// </summary>
    public static class FontHelper
    {
        // Font chuẩn Unicode tiếng Việt - đồng đều cho toàn bộ app
        public static readonly Font StandardFont = new Font("Segoe UI", 11F, FontStyle.Regular);
        public static readonly Font LabelFont = new Font("Segoe UI", 11F, FontStyle.Regular);
        public static readonly Font TextBoxFont = new Font("Segoe UI", 11F, FontStyle.Regular);
        public static readonly Font ComboBoxFont = new Font("Segoe UI", 11F, FontStyle.Regular);
        public static readonly Font ButtonFont = new Font("Segoe UI", 11F, FontStyle.Regular);
        public static readonly Font HeaderFont = new Font("Segoe UI", 11F, FontStyle.Bold);
        public static readonly Font LargeFont = new Font("Segoe UI", 12F, FontStyle.Regular);
        public static readonly Font TitleFont = new Font("Segoe UI", 16F, FontStyle.Bold);
        
        // Font cho DataGridView
        public static readonly Font DGVHeaderFont = new Font("Segoe UI", 11F, FontStyle.Bold);
        public static readonly Font DVGCellFont = new Font("Segoe UI", 10.5F, FontStyle.Regular);

        /// <summary>
        /// Áp dụng font chuẩn cho tất cả controls trong form
        /// </summary>
        public static void ApplyStandardFont(Control control)
        {
            if (control == null) return;

            // Áp dụng font cho control chính
            if (control.Font != null)
            {
                control.Font = StandardFont;
            }

            // Áp dụng cho tất cả controls con
            foreach (Control child in control.Controls)
            {
                ApplyStandardFont(child);

                // Special handling for specific controls
                if (child is DataGridView dgv)
                {
                    dgv.DefaultCellStyle.Font = DVGCellFont;
                    dgv.ColumnHeadersDefaultCellStyle.Font = DGVHeaderFont;
                }
                else if (child is TextBox txt)
                {
                    txt.Font = TextBoxFont;
                }
                else if (child is ComboBox cmb)
                {
                    cmb.Font = ComboBoxFont;
                }
                else if (child is Button btn && btn.Tag?.ToString() == "Header")
                {
                    btn.Font = HeaderFont;
                }
                else if (child is Label lbl && lbl.Font.Size >= 12)
                {
                    lbl.Font = TitleFont;
                }
            }
        }
        
        /// <summary>
        /// Cấu hình TextAlignment căn trái cho control
        /// </summary>
        public static void SetLeftAlignment(Control control)
        {
            if (control == null) return;
            
            if (control is TextBox txt)
            {
                txt.TextAlign = HorizontalAlignment.Left;
            }
            else if (control is Label lbl)
            {
                lbl.TextAlign = ContentAlignment.MiddleLeft;
            }
        }

        /// <summary>
        /// Áp dụng font chuẩn cho form
        /// </summary>
        public static void SetFormFont(Form form)
        {
            if (form != null)
            {
                form.Font = StandardFont;
                ApplyStandardFont(form);
            }
        }

        /// <summary>
        /// Lấy font chuẩn
        /// </summary>
        public static Font GetStandardFont() => StandardFont;
        
        /// <summary>
        /// Lấy font Label
        /// </summary>
        public static Font GetLabelFont() => LabelFont;
        
        /// <summary>
        /// Lấy font TextBox
        /// </summary>
        public static Font GetTextBoxFont() => TextBoxFont;
        
        /// <summary>
        /// Lấy font ComboBox
        /// </summary>
        public static Font GetComboBoxFont() => ComboBoxFont;
        
        /// <summary>
        /// Lấy font Button
        /// </summary>
        public static Font GetButtonFont() => ButtonFont;
        
        /// <summary>
        /// Lấy font header (Bold)
        /// </summary>
        public static Font GetHeaderFont() => HeaderFont;
        
        /// <summary>
        /// Lấy font lớn
        /// </summary>
        public static Font GetLargeFont() => LargeFont;
        
        /// <summary>
        /// Lấy font title
        /// </summary>
        public static Font GetTitleFont() => TitleFont;
        
        /// <summary>
        /// Lấy font DataGridView header
        /// </summary>
        public static Font GetDGVHeaderFont() => DGVHeaderFont;
        
        /// <summary>
        /// Lấy font DataGridView cell
        /// </summary>
        public static Font GetDVGCellFont() => DVGCellFont;
    }
}








