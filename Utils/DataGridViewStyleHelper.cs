using System;
using System.Drawing;
using System.Windows.Forms;
using Sunny.UI;

namespace SupermarketApp.Utils
{
    public static class DataGridViewStyleHelper
    {
        // Chuẩn font chung cho toàn bộ ứng dụng
        private static readonly Font HeaderFont = FontHelper.GetDGVHeaderFont();
        private static readonly Font CellFont = FontHelper.GetDVGCellFont();
        private static readonly Font RowHeaderFont = FontHelper.GetDVGCellFont();

        /// <summary>
        /// Áp dụng style chuẩn cho UIDataGridView
        /// </summary>
        public static void ApplyStandardStyle(UIDataGridView dgv, Color primaryColor)
        {
            if (dgv == null) return;

            // Cấu hình chung
            dgv.Font = CellFont;
            dgv.RowTemplate.Height = 35;
            dgv.ColumnHeadersHeight = 40;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.EnableHeadersVisualStyles = false;
            dgv.BackgroundColor = Color.White;
            dgv.GridColor = Color.FromArgb(230, 230, 230);
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            // Header style
            var headerStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                BackColor = primaryColor,
                Font = HeaderFont,
                ForeColor = Color.White,
                SelectionBackColor = primaryColor,
                SelectionForeColor = Color.White,
                WrapMode = DataGridViewTriState.True
            };
            dgv.ColumnHeadersDefaultCellStyle = headerStyle;

            // Cell style
            var cellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                BackColor = Color.White,
                Font = CellFont,
                ForeColor = Color.FromArgb(48, 48, 48),
                SelectionBackColor = Color.FromArgb(240, 248, 255),
                SelectionForeColor = Color.FromArgb(48, 48, 48),
                WrapMode = DataGridViewTriState.False,
                Padding = new Padding(5, 0, 5, 0)
            };
            dgv.DefaultCellStyle = cellStyle;

            // Alternating row style
            var alternatingStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                BackColor = Color.FromArgb(248, 249, 250),
                Font = CellFont,
                ForeColor = Color.FromArgb(48, 48, 48),
                SelectionBackColor = Color.FromArgb(240, 248, 255),
                SelectionForeColor = Color.FromArgb(48, 48, 48),
                WrapMode = DataGridViewTriState.False,
                Padding = new Padding(5, 0, 5, 0)
            };
            dgv.AlternatingRowsDefaultCellStyle = alternatingStyle;

            // Row header style
            var rowHeaderStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                BackColor = Color.FromArgb(248, 249, 250),
                Font = RowHeaderFont,
                ForeColor = Color.FromArgb(48, 48, 48),
                SelectionBackColor = primaryColor,
                SelectionForeColor = Color.White,
                WrapMode = DataGridViewTriState.True
            };
            dgv.RowHeadersDefaultCellStyle = rowHeaderStyle;
            dgv.RowHeadersWidth = 50;

            // Rows default style
            var rowsStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                Font = CellFont,
                ForeColor = Color.FromArgb(48, 48, 48)
            };
            dgv.RowsDefaultCellStyle = rowsStyle;
        }

        /// <summary>
        /// Cấu hình cột với font và alignment chuẩn
        /// </summary>
        public static void ConfigureColumn(DataGridViewColumn column, string headerText, int width, 
            DataGridViewContentAlignment alignment = DataGridViewContentAlignment.MiddleLeft, 
            string format = "")
        {
            if (column == null) return;

            column.HeaderText = headerText;
            column.Width = width;
            column.DefaultCellStyle.Alignment = alignment;
            column.DefaultCellStyle.Font = CellFont;

            if (!string.IsNullOrEmpty(format))
            {
                column.DefaultCellStyle.Format = format;
            }
        }

        /// <summary>
        /// Cấu hình cột số với format và alignment phải
        /// </summary>
        public static void ConfigureNumericColumn(DataGridViewColumn column, string headerText, int width, string format = "N0")
        {
            ConfigureColumn(column, headerText, width, DataGridViewContentAlignment.MiddleRight, format);
        }

        /// <summary>
        /// Cấu hình cột text căn giữa
        /// </summary>
        public static void ConfigureCenterColumn(DataGridViewColumn column, string headerText, int width)
        {
            ConfigureColumn(column, headerText, width, DataGridViewContentAlignment.MiddleCenter);
        }

        /// <summary>
        /// Auto-resize columns với min width
        /// </summary>
        public static void AutoResizeColumns(UIDataGridView dgv, int minWidth = 80)
        {
            if (dgv == null || dgv.Columns.Count == 0) return;

            foreach (DataGridViewColumn column in dgv.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                
                // Tính width dựa trên header và content
                int headerWidth = TextRenderer.MeasureText(column.HeaderText, HeaderFont).Width + 40;
                int width = headerWidth;

                if (dgv.Rows.Count > 0)
                {
                    for (int i = 0; i < Math.Min(10, dgv.Rows.Count); i++)
                    {
                        var cellValue = dgv.Rows[i].Cells[column.Index].Value?.ToString() ?? "";
                        int cellWidth = TextRenderer.MeasureText(cellValue, CellFont).Width + 50;
                        width = Math.Max(width, cellWidth);
                    }
                }

                column.Width = Math.Max(Math.Min(width, 300), minWidth);
            }
        }
    }
}

