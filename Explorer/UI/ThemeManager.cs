using System.Drawing.Drawing2D;

namespace GodotPCKExplorer.UI
{
    internal static class ThemeManager
    {
        internal static readonly Color DarkBackColor = Color.FromArgb(32, 33, 36);
        internal static readonly Color DarkSurfaceColor = Color.FromArgb(45, 47, 51);
        internal static readonly Color DarkTextColor = Color.FromArgb(235, 235, 235);
        internal static readonly Color DarkMutedTextColor = Color.FromArgb(190, 190, 190);
        internal static readonly Color DarkGridLineColor = Color.FromArgb(70, 72, 78);

        public static bool IsDark => GUIConfig.Instance.DarkTheme;

        public static void Toggle()
        {
            GUIConfig.Instance.DarkTheme = !GUIConfig.Instance.DarkTheme;
            GUIConfig.Instance.Save();

            foreach (Form form in Application.OpenForms)
                Apply(form);
        }

        public static void Apply(Form form)
        {
            form.BackColor = IsDark ? DarkBackColor : SystemColors.Control;
            form.ForeColor = IsDark ? DarkTextColor : SystemColors.ControlText;

            ApplyControl(form);
        }

        public static Bitmap CreateToggleIcon()
        {
            var icon = new Bitmap(16, 16);
            using var graphics = Graphics.FromImage(icon);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.Clear(Color.Transparent);

            if (IsDark)
            {
                using var brush = new SolidBrush(Color.FromArgb(255, 214, 94));
                using var pen = new Pen(Color.FromArgb(255, 214, 94), 1.4f);
                graphics.FillEllipse(brush, 4, 4, 8, 8);
                for (var angle = 0; angle < 360; angle += 45)
                {
                    var radians = angle * Math.PI / 180.0;
                    var x1 = 8 + (float)Math.Cos(radians) * 6;
                    var y1 = 8 + (float)Math.Sin(radians) * 6;
                    var x2 = 8 + (float)Math.Cos(radians) * 7.5f;
                    var y2 = 8 + (float)Math.Sin(radians) * 7.5f;
                    graphics.DrawLine(pen, x1, y1, x2, y2);
                }
            }
            else
            {
                using var brush = new SolidBrush(Color.FromArgb(55, 58, 64));
                graphics.FillEllipse(brush, 3, 2, 11, 11);
                using var cutout = new SolidBrush(Color.FromArgb(240, 240, 240));
                graphics.FillEllipse(cutout, 6, 1, 9, 9);
            }

            return icon;
        }

        static void ApplyControl(Control control)
        {
            if (control is DataGridView grid)
            {
                grid.BackgroundColor = IsDark ? DarkBackColor : SystemColors.Window;
                grid.GridColor = IsDark ? DarkGridLineColor : SystemColors.ControlDark;
                grid.EnableHeadersVisualStyles = false;
                grid.ColumnHeadersDefaultCellStyle.BackColor = IsDark ? DarkSurfaceColor : SystemColors.Control;
                grid.ColumnHeadersDefaultCellStyle.ForeColor = IsDark ? DarkTextColor : SystemColors.ControlText;
                grid.DefaultCellStyle.BackColor = IsDark ? DarkSurfaceColor : SystemColors.Window;
                grid.DefaultCellStyle.ForeColor = IsDark ? DarkTextColor : SystemColors.ControlText;
                grid.DefaultCellStyle.SelectionBackColor = IsDark ? Color.FromArgb(70, 93, 125) : SystemColors.Highlight;
                grid.DefaultCellStyle.SelectionForeColor = IsDark ? Color.White : SystemColors.HighlightText;
            }
            else if (control is ToolStrip strip)
            {
                strip.BackColor = IsDark ? DarkSurfaceColor : SystemColors.Control;
                strip.ForeColor = IsDark ? DarkTextColor : SystemColors.ControlText;
                strip.Renderer = IsDark
                    ? new ToolStripProfessionalRenderer(new DarkColorTable())
                    : new ToolStripProfessionalRenderer();

                foreach (ToolStripItem item in strip.Items)
                    ApplyToolStripItem(item);
            }
            else
            {
                var isInput = control is TextBoxBase || control is ListControl || control is NumericUpDown || control is ButtonBase;
                control.BackColor = IsDark ? DarkSurfaceColor : isInput ? SystemColors.Window : SystemColors.Control;
                control.ForeColor = IsDark ? DarkTextColor : SystemColors.ControlText;
            }

            if (control is LinkLabel link)
            {
                link.LinkColor = IsDark ? Color.FromArgb(120, 190, 255) : Color.Blue;
                link.ActiveLinkColor = IsDark ? Color.FromArgb(170, 215, 255) : Color.Red;
                link.VisitedLinkColor = IsDark ? Color.FromArgb(175, 145, 235) : Color.Purple;
                link.DisabledLinkColor = IsDark ? DarkMutedTextColor : SystemColors.GrayText;
            }

            foreach (Control child in control.Controls)
                ApplyControl(child);
        }

        static void ApplyToolStripItem(ToolStripItem item)
        {
            item.BackColor = IsDark ? DarkSurfaceColor : SystemColors.Control;
            item.ForeColor = IsDark ? DarkTextColor : SystemColors.ControlText;

            if (item is ToolStripControlHost host)
            {
                host.Control.BackColor = IsDark ? DarkSurfaceColor : SystemColors.Window;
                host.Control.ForeColor = IsDark ? DarkTextColor : SystemColors.ControlText;
            }

            if (item is ToolStripDropDownItem dropDown)
            {
                foreach (ToolStripItem child in dropDown.DropDownItems)
                    ApplyToolStripItem(child);
            }
        }

        sealed class DarkColorTable : ProfessionalColorTable
        {
            public override Color ImageMarginGradientBegin => DarkSurfaceColor;
            public override Color ImageMarginGradientMiddle => DarkSurfaceColor;
            public override Color ImageMarginGradientEnd => DarkSurfaceColor;
            public override Color MenuBorder => DarkGridLineColor;
            public override Color MenuItemBorder => DarkGridLineColor;
            public override Color MenuItemSelected => Color.FromArgb(70, 93, 125);
            public override Color MenuItemSelectedGradientBegin => Color.FromArgb(70, 93, 125);
            public override Color MenuItemSelectedGradientEnd => Color.FromArgb(70, 93, 125);
            public override Color MenuStripGradientBegin => DarkSurfaceColor;
            public override Color MenuStripGradientEnd => DarkSurfaceColor;
            public override Color ToolStripDropDownBackground => DarkSurfaceColor;
            public override Color ToolStripGradientBegin => DarkSurfaceColor;
            public override Color ToolStripGradientEnd => DarkSurfaceColor;
            public override Color ToolStripContentPanelGradientBegin => DarkBackColor;
            public override Color ToolStripContentPanelGradientEnd => DarkBackColor;
            public override Color ToolStripBorder => DarkGridLineColor;
            public override Color SeparatorDark => DarkGridLineColor;
            public override Color SeparatorLight => DarkGridLineColor;
        }
    }
}
