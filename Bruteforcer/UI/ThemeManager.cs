using GodotPCKExplorer.GlobalShared;
using System.Drawing.Drawing2D;
using System.Text.Json;

namespace PCKBruteforcer.UI
{
    internal static class ThemeManager
    {
        internal static readonly Color DarkBackColor = Color.FromArgb(32, 33, 36);
        internal static readonly Color DarkSurfaceColor = Color.FromArgb(45, 47, 51);
        internal static readonly Color DarkDisabledSurfaceColor = Color.FromArgb(57, 59, 64);
        internal static readonly Color DarkTextColor = Color.FromArgb(235, 235, 235);
        internal static readonly Color DarkMutedTextColor = Color.FromArgb(190, 190, 190);
        internal static readonly Color DarkDisabledTextColor = Color.FromArgb(155, 158, 164);
        static readonly string SettingsFile = Path.Combine(GlobalConstants.AppDataPath, "bruteforcer-ui-settings.json");
        public static bool IsDark { get; private set; } = Load();

        static bool Load()
        {
            try
            {
                if (File.Exists(SettingsFile))
                    return JsonSerializer.Deserialize<bool>(File.ReadAllText(SettingsFile));
            }
            catch (Exception ex) { Program.Log(ex); }
            return false;
        }

        public static void Toggle()
        {
            IsDark = !IsDark;
            try { File.WriteAllText(SettingsFile, JsonSerializer.Serialize(IsDark)); }
            catch (Exception ex) { Program.Log(ex); }

            foreach (Form form in Application.OpenForms)
                Apply(form);
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
                    graphics.DrawLine(pen, 8 + (float)Math.Cos(radians) * 6, 8 + (float)Math.Sin(radians) * 6,
                        8 + (float)Math.Cos(radians) * 7.5f, 8 + (float)Math.Sin(radians) * 7.5f);
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

        public static void Apply(Control control)
        {
            if (control is ProgressBarEx progressBar)
            {
                progressBar.BackColor = IsDark ? DarkBackColor : SystemColors.Control;
                progressBar.TextColor = IsDark ? DarkTextColor : SystemColors.ControlText;
            }
            else
            {
                var isInput = control is TextBoxBase || control is NumericUpDown || control is ButtonBase;
                var disabled = !control.Enabled;
                control.BackColor = IsDark
                    ? disabled ? DarkDisabledSurfaceColor : DarkSurfaceColor
                    : isInput ? SystemColors.Window : SystemColors.Control;
                control.ForeColor = IsDark
                    ? disabled ? DarkDisabledTextColor : DarkTextColor
                    : SystemColors.ControlText;

                if (control is Button button)
                {
                    button.UseVisualStyleBackColor = !IsDark;
                    button.FlatStyle = IsDark ? FlatStyle.Flat : FlatStyle.Standard;
                    if (IsDark)
                    {
                        button.FlatAppearance.BorderColor = disabled
                            ? Color.FromArgb(90, 93, 99)
                            : Color.FromArgb(220, 220, 220);
                        button.FlatAppearance.MouseOverBackColor = Color.FromArgb(63, 66, 72);
                        button.FlatAppearance.MouseDownBackColor = Color.FromArgb(75, 78, 84);
                    }
                }

                if (control is CheckBox checkBox)
                {
                    checkBox.UseVisualStyleBackColor = !IsDark;
                    checkBox.FlatStyle = IsDark ? FlatStyle.Flat : FlatStyle.Standard;
                }
            }

            foreach (Control child in control.Controls)
                Apply(child);
        }
    }
}
