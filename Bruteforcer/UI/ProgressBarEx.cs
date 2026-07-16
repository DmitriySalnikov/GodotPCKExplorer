namespace PCKBruteforcer.UI
{
    // https://stackoverflow.com/a/2498036
    internal class ProgressBarEx : ProgressBar
    {
        private SolidBrush ProgressBrush = new(SystemColors.HotTrack);
        private SolidBrush TextBrush = new(SystemColors.ControlText);

        public Color TextColor = SystemColors.ControlText;

        Color progressColor = SystemColors.HotTrack;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color ProgressColor
        {
            get => progressColor;
            set
            {
                if (progressColor != value)
                {
                    progressColor = value;
                    Update();
                }
            }
        }

        string progressText = "";
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ProgressText
        {
            get => progressText;
            set
            {
                if (progressText != value)
                {
                    progressText = value;
                    Update();
                }
            }
        }

        readonly StringFormat progressFormat = new(StringFormatFlags.NoWrap) { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

        public ProgressBarEx()
        {
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (ProgressBrush == null || ProgressBrush.Color != ProgressColor)
                ProgressBrush = new SolidBrush(ProgressColor);

            if (TextBrush == null || TextBrush.Color != TextColor)
                TextBrush = new SolidBrush(TextColor);

            var rec = new Rectangle(0, 0, Width, Height);
            if (BackColor == ThemeManager.DarkBackColor)
            {
                using var backgroundBrush = new SolidBrush(BackColor);
                using var borderPen = new Pen(Color.FromArgb(100, 103, 110));
                e.Graphics.FillRectangle(backgroundBrush, rec);
                e.Graphics.DrawRectangle(borderPen, 0, 0, rec.Width - 1, rec.Height - 1);
            }
            else if (ProgressBarRenderer.IsSupported)
                ProgressBarRenderer.DrawHorizontalBar(e.Graphics, rec);

            Rectangle barRec = rec;
            barRec.Width = (int)((barRec.Width - 2) * ((double)Value / Maximum));
            barRec.Height -= 2;
            e.Graphics.FillRectangle(ProgressBrush, 1, 1, barRec.Width, barRec.Height);
            e.Graphics.DrawString(ProgressText, DefaultFont, TextBrush, rec, progressFormat);
        }
    }
}
