namespace ObtainPCKEncryptionKey
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            ofd_exe = new OpenFileDialog();
            btn_start = new Button();
            tb_pck = new TextBox();
            label2 = new Label();
            tb_exe = new TextBox();
            label1 = new Label();
            btn_exe = new Button();
            btn_pck = new Button();
            ofd_pck = new OpenFileDialog();
            tb_result = new TextBox();
            nud_threads = new NumericUpDown();
            toolTip1 = new ToolTip(components);
            cb_inMemory = new CheckBox();
            nud_from = new NumericUpDown();
            nud_to = new NumericUpDown();
            tlp_progressTable = new TableLayoutPanel();
            l_percents = new Label();
            l_elapsedTime = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            l_estimatedTime = new Label();
            label3 = new Label();
            tableLayoutPanel2 = new TableLayoutPanel();
            tlp_output = new TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)nud_threads).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nud_from).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nud_to).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tlp_output.SuspendLayout();
            SuspendLayout();
            // 
            // ofd_exe
            // 
            ofd_exe.Filter = "Executable|*.exe|All files|*.*";
            ofd_exe.Title = "Select .exe file";
            // 
            // btn_start
            // 
            btn_start.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btn_start.Location = new Point(12, 129);
            btn_start.Name = "btn_start";
            btn_start.Size = new Size(529, 25);
            btn_start.TabIndex = 4;
            btn_start.Text = "Try to get the key";
            btn_start.UseVisualStyleBackColor = true;
            btn_start.Click += btn_start_Click;
            // 
            // tb_pck
            // 
            tb_pck.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tb_pck.Location = new Point(12, 71);
            tb_pck.Name = "tb_pck";
            tb_pck.Size = new Size(435, 23);
            tb_pck.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 53);
            label2.Name = "label2";
            label2.Size = new Size(48, 15);
            label2.TabIndex = 3;
            label2.Text = "PCK file";
            // 
            // tb_exe
            // 
            tb_exe.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tb_exe.Location = new Point(12, 27);
            tb_exe.Name = "tb_exe";
            tb_exe.Size = new Size(435, 23);
            tb_exe.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(45, 15);
            label1.TabIndex = 2;
            label1.Text = "EXE file";
            // 
            // btn_exe
            // 
            btn_exe.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_exe.Location = new Point(453, 27);
            btn_exe.Name = "btn_exe";
            btn_exe.Size = new Size(88, 23);
            btn_exe.TabIndex = 5;
            btn_exe.Text = "Browse...";
            btn_exe.UseVisualStyleBackColor = true;
            btn_exe.Click += btn_exe_Click;
            // 
            // btn_pck
            // 
            btn_pck.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_pck.Location = new Point(453, 71);
            btn_pck.Name = "btn_pck";
            btn_pck.Size = new Size(88, 23);
            btn_pck.TabIndex = 6;
            btn_pck.Text = "Browse...";
            btn_pck.UseVisualStyleBackColor = true;
            btn_pck.Click += btn_pck_Click;
            // 
            // ofd_pck
            // 
            ofd_pck.Filter = "PCK container|*.exe;*.pck|All files|*.*";
            ofd_pck.Title = "Select .pck file";
            // 
            // tb_result
            // 
            tb_result.Dock = DockStyle.Fill;
            tb_result.Location = new Point(0, 26);
            tb_result.Margin = new Padding(0, 1, 0, 1);
            tb_result.Multiline = true;
            tb_result.Name = "tb_result";
            tb_result.ReadOnly = true;
            tb_result.ScrollBars = ScrollBars.Vertical;
            tb_result.Size = new Size(529, 53);
            tb_result.TabIndex = 8;
            // 
            // nud_threads
            // 
            nud_threads.Location = new Point(12, 100);
            nud_threads.Maximum = new decimal(new int[] { 2048, 0, 0, 0 });
            nud_threads.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nud_threads.Name = "nud_threads";
            nud_threads.Size = new Size(51, 23);
            nud_threads.TabIndex = 9;
            toolTip1.SetToolTip(nud_threads, "Threads");
            nud_threads.Value = new decimal(new int[] { 4, 0, 0, 0 });
            // 
            // cb_inMemory
            // 
            cb_inMemory.AutoSize = true;
            cb_inMemory.Checked = true;
            cb_inMemory.CheckState = CheckState.Checked;
            cb_inMemory.Location = new Point(69, 102);
            cb_inMemory.Name = "cb_inMemory";
            cb_inMemory.Size = new Size(84, 19);
            cb_inMemory.TabIndex = 12;
            cb_inMemory.Text = "In memory";
            toolTip1.SetToolTip(cb_inMemory, "Load PCK parts into RAM");
            cb_inMemory.UseVisualStyleBackColor = true;
            // 
            // nud_from
            // 
            nud_from.Dock = DockStyle.Fill;
            nud_from.Location = new Point(0, 0);
            nud_from.Margin = new Padding(0);
            nud_from.Name = "nud_from";
            nud_from.Size = new Size(125, 23);
            nud_from.TabIndex = 9;
            toolTip1.SetToolTip(nud_from, "Start address");
            // 
            // nud_to
            // 
            nud_to.Dock = DockStyle.Fill;
            nud_to.Location = new Point(146, 0);
            nud_to.Margin = new Padding(0);
            nud_to.Name = "nud_to";
            nud_to.Size = new Size(125, 23);
            nud_to.TabIndex = 9;
            toolTip1.SetToolTip(nud_to, "End address");
            // 
            // tlp_progressTable
            // 
            tlp_progressTable.ColumnCount = 1;
            tlp_progressTable.ColumnStyles.Add(new ColumnStyle());
            tlp_progressTable.Dock = DockStyle.Fill;
            tlp_progressTable.Location = new Point(0, 81);
            tlp_progressTable.Margin = new Padding(0, 1, 0, 1);
            tlp_progressTable.Name = "tlp_progressTable";
            tlp_progressTable.RowCount = 1;
            tlp_progressTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlp_progressTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tlp_progressTable.Size = new Size(529, 31);
            tlp_progressTable.TabIndex = 11;
            // 
            // l_percents
            // 
            l_percents.Dock = DockStyle.Fill;
            l_percents.Location = new Point(4, 0);
            l_percents.Margin = new Padding(4, 0, 4, 0);
            l_percents.Name = "l_percents";
            l_percents.Size = new Size(131, 23);
            l_percents.TabIndex = 13;
            l_percents.Text = "0.00%";
            l_percents.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // l_elapsedTime
            // 
            l_elapsedTime.Dock = DockStyle.Fill;
            l_elapsedTime.Location = new Point(337, 0);
            l_elapsedTime.Margin = new Padding(4, 0, 4, 0);
            l_elapsedTime.Name = "l_elapsedTime";
            l_elapsedTime.Size = new Size(188, 23);
            l_elapsedTime.TabIndex = 14;
            l_elapsedTime.Text = "Elapsed time: 00:00:00";
            l_elapsedTime.TextAlign = ContentAlignment.MiddleRight;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.Controls.Add(l_percents, 0, 0);
            tableLayoutPanel1.Controls.Add(l_elapsedTime, 2, 0);
            tableLayoutPanel1.Controls.Add(l_estimatedTime, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 1);
            tableLayoutPanel1.Margin = new Padding(0, 1, 0, 1);
            tableLayoutPanel1.MaximumSize = new Size(0, 23);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 23F));
            tableLayoutPanel1.Size = new Size(529, 23);
            tableLayoutPanel1.TabIndex = 15;
            // 
            // l_estimatedTime
            // 
            l_estimatedTime.Dock = DockStyle.Fill;
            l_estimatedTime.Location = new Point(143, 0);
            l_estimatedTime.Margin = new Padding(4, 0, 4, 0);
            l_estimatedTime.Name = "l_estimatedTime";
            l_estimatedTime.Size = new Size(186, 23);
            l_estimatedTime.TabIndex = 15;
            l_estimatedTime.Text = "Estimated time: 00:00:00";
            l_estimatedTime.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Dock = DockStyle.Fill;
            label3.Location = new Point(129, 0);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(13, 23);
            label3.TabIndex = 3;
            label3.Text = "–";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            tableLayoutPanel2.ColumnCount = 3;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Controls.Add(nud_to, 2, 0);
            tableLayoutPanel2.Controls.Add(nud_from, 0, 0);
            tableLayoutPanel2.Controls.Add(label3, 1, 0);
            tableLayoutPanel2.Location = new Point(270, 100);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(271, 23);
            tableLayoutPanel2.TabIndex = 17;
            // 
            // tlp_output
            // 
            tlp_output.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tlp_output.AutoScroll = true;
            tlp_output.AutoScrollMinSize = new Size(5, 0);
            tlp_output.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tlp_output.ColumnCount = 1;
            tlp_output.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlp_output.Controls.Add(tableLayoutPanel1, 0, 0);
            tlp_output.Controls.Add(tb_result, 0, 1);
            tlp_output.Controls.Add(tlp_progressTable, 0, 2);
            tlp_output.Location = new Point(12, 160);
            tlp_output.Name = "tlp_output";
            tlp_output.RowCount = 3;
            tlp_output.RowStyles.Add(new RowStyle());
            tlp_output.RowStyles.Add(new RowStyle(SizeType.Absolute, 55F));
            tlp_output.RowStyles.Add(new RowStyle());
            tlp_output.Size = new Size(529, 113);
            tlp_output.TabIndex = 16;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(553, 278);
            Controls.Add(tlp_output);
            Controls.Add(tableLayoutPanel2);
            Controls.Add(cb_inMemory);
            Controls.Add(nud_threads);
            Controls.Add(btn_pck);
            Controls.Add(btn_exe);
            Controls.Add(label1);
            Controls.Add(tb_exe);
            Controls.Add(label2);
            Controls.Add(btn_start);
            Controls.Add(tb_pck);
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MaximumSize = new Size(1491, 830);
            MinimumSize = new Size(569, 317);
            Name = "MainForm";
            Text = "PCK bruteforcer";
            FormClosing += MainForm_FormClosing;
            ((System.ComponentModel.ISupportInitialize)nud_threads).EndInit();
            ((System.ComponentModel.ISupportInitialize)nud_from).EndInit();
            ((System.ComponentModel.ISupportInitialize)nud_to).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            tlp_output.ResumeLayout(false);
            tlp_output.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private OpenFileDialog ofd_exe;
        private Button btn_start;
        private TextBox tb_pck;
        private Label label2;
        private TextBox tb_exe;
        private Label label1;
        private Button btn_exe;
        private Button btn_pck;
        private OpenFileDialog ofd_pck;
        private TextBox tb_result;
        private NumericUpDown nud_threads;
        private ToolTip toolTip1;
        private TableLayoutPanel tlp_progressTable;
        private CheckBox cb_inMemory;
        private Label l_percents;
        private Label l_elapsedTime;
        private TableLayoutPanel tableLayoutPanel1;
        private Label l_estimatedTime;
        private NumericUpDown nud_from;
        private Label label3;
        private NumericUpDown nud_to;
        private TableLayoutPanel tableLayoutPanel2;
        private TableLayoutPanel tlp_output;
    }
}

