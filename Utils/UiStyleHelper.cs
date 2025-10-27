using System;
using System.Drawing;
using System.Windows.Forms;
using Sunny.UI;

namespace SupermarketApp.Utils
{
    public static class UiStyleHelper
    {
        private static readonly Font TitleFont = new Font("Microsoft Sans Serif", 18F, FontStyle.Bold);
        private static readonly Font LabelFont = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular);
        private static readonly Font InputFont = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular);
        private static readonly Font ButtonFont = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
        private static readonly Font GridHeaderFont = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
        private static readonly Font GridCellFont = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular);
        private static readonly Font TipsFont = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular);

        public static void ApplyFormFonts(Form form)
        {
            if (form == null) return;
            foreach (Control c in form.Controls)
            {
                ApplyFontsRecursive(c);
            }
        }

        private static void ApplyFontsRecursive(Control c)
        {
            if (c == null) return;

            switch (c)
            {
                case UILabel _:
                    c.Font = LabelFont;
                    break;
                case UITextBox _:
                    c.Font = InputFont;
                    break;
                case UIComboBox _:
                    c.Font = InputFont;
                    break;
                case UIIntegerUpDown _:
                    c.Font = InputFont;
                    break;
                case UIDataGridView uiGrid:
                    ApplyGridFonts(uiGrid);
                    break;
                case DataGridView winGrid:
                    ApplyWinGridFonts(winGrid);
                    break;
                case UIButton uiBtn:
                    uiBtn.Font = ButtonFont;
                    uiBtn.TipsFont = TipsFont;
                    break;
                case DateTimePicker dtp:
                    dtp.Font = InputFont;
                    break;
                default:
                    if (c is UIPanel)
                    {
                        c.Font = InputFont;
                    }
                    break;
            }

            if (c is UILabel label && (label.Name?.Contains("lblTitle") == true))
            {
                label.Font = TitleFont;
            }

            foreach (Control child in c.Controls)
            {
                ApplyFontsRecursive(child);
            }
        }

        private static void ApplyGridFonts(DataGridView grid)
        {
            if (grid == null) return;

            grid.Font = GridCellFont;

            var alt = grid.AlternatingRowsDefaultCellStyle.Clone();
            alt.Font = GridCellFont;
            grid.AlternatingRowsDefaultCellStyle = alt;

            var def = grid.DefaultCellStyle.Clone();
            def.Font = GridCellFont;
            grid.DefaultCellStyle = def;

            var rowHdr = grid.RowHeadersDefaultCellStyle.Clone();
            rowHdr.Font = GridCellFont;
            grid.RowHeadersDefaultCellStyle = rowHdr;

            var colHdr = grid.ColumnHeadersDefaultCellStyle.Clone();
            colHdr.Font = GridHeaderFont;
            grid.ColumnHeadersDefaultCellStyle = colHdr;

            var rowsDef = grid.RowsDefaultCellStyle.Clone();
            rowsDef.Font = GridCellFont;
            grid.RowsDefaultCellStyle = rowsDef;
        }

        private static void ApplyGridFonts(UIDataGridView grid)
        {
            if (grid == null) return;
            ApplyGridFonts(grid as DataGridView);
            grid.Font = GridCellFont;
            grid.TipsFont = TipsFont;
        }

        private static void ApplyWinGridFonts(DataGridView grid)
        {
            ApplyGridFonts(grid);
        }
    }
}