namespace GodotPCKExplorer.UI
{
    partial class ChangePCKVersion
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
            label1 = new Label();
            l_path = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            l_version = new Label();
            btn_ok = new Button();
            label5 = new Label();
            panel1 = new Panel();
            tableLayoutPanel2 = new TableLayoutPanel();
            pckVersionSelector1 = new GodotPCKExplorer.UI.UIComponents.PCKVersionSelector();
            tableLayoutPanel1 = new TableLayoutPanel();
            panel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Location = new Point(4, 0);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(77, 22);
            label1.TabIndex = 13;
            label1.Text = "Pack Version:";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // l_path
            // 
            l_path.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            l_path.AutoSize = true;
            l_path.Location = new Point(13, 48);
            l_path.Margin = new Padding(4, 0, 4, 0);
            l_path.Name = "l_path";
            l_path.Size = new Size(378, 30);
            l_path.TabIndex = 18;
            l_path.Text = "File path:\r\nddddddddddddddddddddddddddddddddddddddddddddddddddddd";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Location = new Point(89, 0);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(77, 22);
            label2.TabIndex = 19;
            label2.Text = "Major:";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Dock = DockStyle.Fill;
            label3.Location = new Point(174, 0);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(77, 22);
            label3.TabIndex = 20;
            label3.Text = "Minor:";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Dock = DockStyle.Fill;
            label4.Location = new Point(259, 0);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(80, 22);
            label4.TabIndex = 21;
            label4.Text = "Patch:";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // l_version
            // 
            l_version.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            l_version.AutoSize = true;
            l_version.Location = new Point(13, 78);
            l_version.Margin = new Padding(4, 0, 4, 0);
            l_version.Name = "l_version";
            l_version.Size = new Size(378, 30);
            l_version.TabIndex = 22;
            l_version.Text = "Original Version:\r\nddddddddddddddddddddddddddddddddddddddddddddddddddddd";
            // 
            // btn_ok
            // 
            btn_ok.Location = new Point(110, 56);
            btn_ok.Margin = new Padding(4, 3, 4, 3);
            btn_ok.Name = "btn_ok";
            btn_ok.Size = new Size(128, 27);
            btn_ok.TabIndex = 23;
            btn_ok.Text = "Change Version";
            btn_ok.UseVisualStyleBackColor = true;
            btn_ok.Click += btn_ok_Click;
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 204);
            label5.Location = new Point(13, 9);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(491, 30);
            label5.TabIndex = 24;
            label5.Text = "Important note: Changing the version only affects the initial PCK check in Godot.\r\nAll the content remains unchanged, which can lead to errors in other versions of the engine.";
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel1.Controls.Add(tableLayoutPanel2);
            panel1.Controls.Add(pckVersionSelector1);
            panel1.Controls.Add(btn_ok);
            panel1.Location = new Point(71, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(349, 83);
            panel1.TabIndex = 25;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 4;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel2.Controls.Add(label4, 3, 0);
            tableLayoutPanel2.Controls.Add(label1, 0, 0);
            tableLayoutPanel2.Controls.Add(label3, 2, 0);
            tableLayoutPanel2.Controls.Add(label2, 1, 0);
            tableLayoutPanel2.Location = new Point(3, 3);
            tableLayoutPanel2.Margin = new Padding(3, 3, 3, 0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(343, 22);
            tableLayoutPanel2.TabIndex = 25;
            // 
            // pckVersionSelector1
            // 
            pckVersionSelector1.Location = new Point(3, 27);
            pckVersionSelector1.Margin = new Padding(3, 1, 3, 3);
            pckVersionSelector1.Name = "pckVersionSelector1";
            pckVersionSelector1.Size = new Size(343, 25);
            pckVersionSelector1.TabIndex = 24;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(panel1, 1, 0);
            tableLayoutPanel1.Location = new Point(12, 114);
            tableLayoutPanel1.Margin = new Padding(3, 0, 3, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(491, 89);
            tableLayoutPanel1.TabIndex = 26;
            // 
            // ChangePCKVersion
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(515, 212);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(label5);
            Controls.Add(l_version);
            Controls.Add(l_path);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Margin = new Padding(4, 3, 4, 3);
            MaximumSize = new Size(531, 251);
            MinimumSize = new Size(531, 251);
            Name = "ChangePCKVersion";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Change PCK Version";
            panel1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label label1;
        private Label l_path;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label l_version;
        private Button btn_ok;
        private Label label5;
        private Panel panel1;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private UIComponents.PCKVersionSelector pckVersionSelector1;
    }
}