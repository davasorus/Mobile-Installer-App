using System.Windows.Forms;

namespace Mobile_App
{
    partial class Form1
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
            this.label3 = new System.Windows.Forms.Label();
            this.UninstallMobile = new System.Windows.Forms.CheckBox();
            this.InstallMobile = new System.Windows.Forms.CheckBox();
            this.Run = new System.Windows.Forms.Button();
            this.NwsHoldPath = new System.Windows.Forms.TextBox();
            this.FilePath_lbl = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Is64Bit = new System.Windows.Forms.CheckBox();
            this.Help_BTN = new System.Windows.Forms.Button();
            this.Is32bit = new System.Windows.Forms.CheckBox();
            this.CopyButton = new System.Windows.Forms.Button();
            this.Combo = new System.Windows.Forms.CheckBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.ProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.label5 = new System.Windows.Forms.Label();
            this.MSPServerPath = new System.Windows.Forms.TextBox();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 13);
            this.label3.TabIndex = 0;
            // 
            // UninstallMobile
            // 
            this.UninstallMobile.AutoSize = true;
            this.UninstallMobile.Location = new System.Drawing.Point(3, 65);
            this.UninstallMobile.Name = "UninstallMobile";
            this.UninstallMobile.Size = new System.Drawing.Size(129, 17);
            this.UninstallMobile.TabIndex = 1;
            this.UninstallMobile.Text = "Uninstall Mobile Client";
            this.UninstallMobile.UseVisualStyleBackColor = true;
            // 
            // InstallMobile
            // 
            this.InstallMobile.AutoSize = true;
            this.InstallMobile.Location = new System.Drawing.Point(3, 98);
            this.InstallMobile.Name = "InstallMobile";
            this.InstallMobile.Size = new System.Drawing.Size(116, 17);
            this.InstallMobile.TabIndex = 3;
            this.InstallMobile.Text = "Install Mobile Client";
            this.InstallMobile.UseVisualStyleBackColor = true;
            // 
            // Run
            // 
            this.Run.Location = new System.Drawing.Point(230, 165);
            this.Run.Name = "Run";
            this.Run.Size = new System.Drawing.Size(75, 23);
            this.Run.TabIndex = 4;
            this.Run.Text = "Run";
            this.Run.UseVisualStyleBackColor = true;
            this.Run.Click += new System.EventHandler(this.Button2_Click);
            this.Run.MouseCaptureChanged += new System.EventHandler(this.Button2_Click);
            // 
            // NwsHoldPath
            // 
            this.NwsHoldPath.AccessibleDescription = "File path to the mobile installer file";
            this.NwsHoldPath.Location = new System.Drawing.Point(3, 167);
            this.NwsHoldPath.Name = "NwsHoldPath";
            this.NwsHoldPath.Size = new System.Drawing.Size(150, 20);
            this.NwsHoldPath.TabIndex = 6;
            this.NwsHoldPath.Text = "\\\\MobileServerName\\C$\\";
            // 
            // FilePath_lbl
            // 
            this.FilePath_lbl.AutoSize = true;
            this.FilePath_lbl.Location = new System.Drawing.Point(0, 152);
            this.FilePath_lbl.Name = "FilePath_lbl";
            this.FilePath_lbl.Size = new System.Drawing.Size(164, 13);
            this.FilePath_lbl.TabIndex = 7;
            this.FilePath_lbl.Text = "Network Path to the MMS Server";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(-3, 4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(161, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Mobile Client Pre-Req Install App";
            // 
            // Is64Bit
            // 
            this.Is64Bit.AutoSize = true;
            this.Is64Bit.Location = new System.Drawing.Point(3, 34);
            this.Is64Bit.Name = "Is64Bit";
            this.Is64Bit.Size = new System.Drawing.Size(97, 17);
            this.Is64Bit.TabIndex = 9;
            this.Is64Bit.Text = "64 Bit Machine";
            this.Is64Bit.UseVisualStyleBackColor = true;
            // 
            // Help_BTN
            // 
            this.Help_BTN.Location = new System.Drawing.Point(240, 4);
            this.Help_BTN.Name = "Help_BTN";
            this.Help_BTN.Size = new System.Drawing.Size(65, 23);
            this.Help_BTN.TabIndex = 10;
            this.Help_BTN.Text = "Help";
            this.Help_BTN.UseVisualStyleBackColor = true;
            this.Help_BTN.Click += new System.EventHandler(this.Button3_Click);
            // 
            // Is32bit
            // 
            this.Is32bit.AutoSize = true;
            this.Is32bit.Location = new System.Drawing.Point(123, 34);
            this.Is32bit.Name = "Is32bit";
            this.Is32bit.Size = new System.Drawing.Size(97, 17);
            this.Is32bit.TabIndex = 11;
            this.Is32bit.Text = "32 Bit Machine\r\n";
            this.Is32bit.UseVisualStyleBackColor = true;
            // 
            // CopyButton
            // 
            this.CopyButton.Location = new System.Drawing.Point(230, 132);
            this.CopyButton.Name = "CopyButton";
            this.CopyButton.Size = new System.Drawing.Size(75, 27);
            this.CopyButton.TabIndex = 12;
            this.CopyButton.Text = "Copy";
            this.CopyButton.UseVisualStyleBackColor = true;
            this.CopyButton.Click += new System.EventHandler(this.CopyFiles_Click);
            // 
            // Combo
            // 
            this.Combo.AutoSize = true;
            this.Combo.Location = new System.Drawing.Point(3, 132);
            this.Combo.Name = "Combo";
            this.Combo.Size = new System.Drawing.Size(191, 17);
            this.Combo.TabIndex = 13;
            this.Combo.Text = "Uninstall/Re-Install Mobile Pre Req";
            this.Combo.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ProgressBar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 227);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(309, 22);
            this.statusStrip1.TabIndex = 14;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // ProgressBar
            // 
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(0, 190);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(162, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Network Path to the MSP Server";
            // 
            // MSPServerPath
            // 
            this.MSPServerPath.Location = new System.Drawing.Point(3, 207);
            this.MSPServerPath.Name = "MSPServerPath";
            this.MSPServerPath.Size = new System.Drawing.Size(150, 20);
            this.MSPServerPath.TabIndex = 16;
            this.MSPServerPath.Text = "\\\\MSPServerName\\";
            // 
            // Form1
            // 
            this.AcceptButton = this.Run;
            this.ClientSize = new System.Drawing.Size(309, 249);
            this.Controls.Add(this.MSPServerPath);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.Combo);
            this.Controls.Add(this.CopyButton);
            this.Controls.Add(this.Is32bit);
            this.Controls.Add(this.Help_BTN);
            this.Controls.Add(this.Is64Bit);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.FilePath_lbl);
            this.Controls.Add(this.NwsHoldPath);
            this.Controls.Add(this.Run);
            this.Controls.Add(this.InstallMobile);
            this.Controls.Add(this.UninstallMobile);
            this.Controls.Add(this.label3);
            this.Name = "Form1";
            this.Text = "Mobile Installer App";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox UninstallMobile;
        private System.Windows.Forms.CheckBox InstallMobile;
        private System.Windows.Forms.Button Run;
        private System.Windows.Forms.TextBox NwsHoldPath;
        private System.Windows.Forms.Label FilePath_lbl;
        private System.Windows.Forms.Label label4;

        public Form1(Label label2)
        {
            this.label2 = label2 ?? throw new System.ArgumentNullException(nameof(label2));
        }

        private CheckBox Is64Bit;
        private Button Help_BTN;
        private CheckBox Is32bit;
        private Button CopyButton;
        private CheckBox Combo;
        private StatusStrip statusStrip1;
        private Label label5;
        private TextBox MSPServerPath;
        private ToolStripProgressBar ProgressBar;

        public Label Label1 { get => label1; set => label1 = value; }
        public Button Button1 { get => button1; set => button1 = value; }
        public CheckBox CheckBox1 { get => checkBox1; set => checkBox1 = value; }
        public CheckBox CheckBox2 { get => checkBox2; set => checkBox2 = value; }
    }
}

