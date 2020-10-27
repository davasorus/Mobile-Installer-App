using System.Windows.Forms;

namespace Mobile_App
{
    partial class NWPSPreReqInstaller
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NWPSPreReqInstaller));
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.CustomRun = new System.Windows.Forms.Button();
            this.label20 = new System.Windows.Forms.Label();
            this.MobileTriage = new System.Windows.Forms.CheckedListBox();
            this.CustomInstallOption = new System.Windows.Forms.CheckedListBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.CustomUninstallOptions = new System.Windows.Forms.CheckedListBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.UpdaterAppend = new System.Windows.Forms.Button();
            this.MergeClient = new System.Windows.Forms.CheckBox();
            this.FireClient = new System.Windows.Forms.CheckBox();
            this.PoliceClient = new System.Windows.Forms.CheckBox();
            this.MobileServer = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.GenerateNumber = new System.Windows.Forms.TextBox();
            this.FieldGenerateButton = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.statusStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
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
            this.UninstallMobile.Location = new System.Drawing.Point(6, 60);
            this.UninstallMobile.Name = "UninstallMobile";
            this.UninstallMobile.Size = new System.Drawing.Size(129, 17);
            this.UninstallMobile.TabIndex = 1;
            this.UninstallMobile.Text = "Uninstall Mobile Client";
            this.UninstallMobile.UseVisualStyleBackColor = true;
            // 
            // InstallMobile
            // 
            this.InstallMobile.AutoSize = true;
            this.InstallMobile.Location = new System.Drawing.Point(6, 83);
            this.InstallMobile.Name = "InstallMobile";
            this.InstallMobile.Size = new System.Drawing.Size(116, 17);
            this.InstallMobile.TabIndex = 3;
            this.InstallMobile.Text = "Install Mobile Client";
            this.InstallMobile.UseVisualStyleBackColor = true;
            // 
            // Run
            // 
            this.Run.Location = new System.Drawing.Point(324, 202);
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
            this.NwsHoldPath.Location = new System.Drawing.Point(6, 162);
            this.NwsHoldPath.Name = "NwsHoldPath";
            this.NwsHoldPath.Size = new System.Drawing.Size(150, 20);
            this.NwsHoldPath.TabIndex = 6;
            this.NwsHoldPath.Text = "THIS HAS BEEN DEPRECIATED";
            this.NwsHoldPath.Visible = false;
            // 
            // FilePath_lbl
            // 
            this.FilePath_lbl.AutoSize = true;
            this.FilePath_lbl.Location = new System.Drawing.Point(3, 147);
            this.FilePath_lbl.Name = "FilePath_lbl";
            this.FilePath_lbl.Size = new System.Drawing.Size(164, 13);
            this.FilePath_lbl.TabIndex = 7;
            this.FilePath_lbl.Text = "Network Path to the MMS Server";
            this.FilePath_lbl.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(161, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Mobile Client Pre-Req Install App";
            // 
            // Is64Bit
            // 
            this.Is64Bit.AutoSize = true;
            this.Is64Bit.Location = new System.Drawing.Point(6, 29);
            this.Is64Bit.Name = "Is64Bit";
            this.Is64Bit.Size = new System.Drawing.Size(97, 17);
            this.Is64Bit.TabIndex = 9;
            this.Is64Bit.Text = "64 Bit Machine";
            this.Is64Bit.UseVisualStyleBackColor = true;
            // 
            // Help_BTN
            // 
            this.Help_BTN.Location = new System.Drawing.Point(332, 6);
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
            this.Is32bit.Location = new System.Drawing.Point(126, 29);
            this.Is32bit.Name = "Is32bit";
            this.Is32bit.Size = new System.Drawing.Size(97, 17);
            this.Is32bit.TabIndex = 11;
            this.Is32bit.Text = "32 Bit Machine\r\n";
            this.Is32bit.UseVisualStyleBackColor = true;
            // 
            // CopyButton
            // 
            this.CopyButton.Location = new System.Drawing.Point(324, 162);
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
            this.Combo.Location = new System.Drawing.Point(6, 106);
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
            this.statusStrip1.Location = new System.Drawing.Point(0, 274);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(427, 22);
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
            this.label5.Location = new System.Drawing.Point(3, 185);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(272, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "File Path to Pre Req or Network Path to the MSP Server";
            // 
            // MSPServerPath
            // 
            this.MSPServerPath.Location = new System.Drawing.Point(6, 202);
            this.MSPServerPath.Multiline = true;
            this.MSPServerPath.Name = "MSPServerPath";
            this.MSPServerPath.Size = new System.Drawing.Size(269, 36);
            this.MSPServerPath.TabIndex = 16;
            this.MSPServerPath.Text = "\\\\MSPServerName\\";
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(3, 1);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(424, 273);
            this.tabControl1.TabIndex = 17;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabPage1.Controls.Add(this.InstallMobile);
            this.tabPage1.Controls.Add(this.MSPServerPath);
            this.tabPage1.Controls.Add(this.UninstallMobile);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.Run);
            this.tabPage1.Controls.Add(this.NwsHoldPath);
            this.tabPage1.Controls.Add(this.Combo);
            this.tabPage1.Controls.Add(this.FilePath_lbl);
            this.tabPage1.Controls.Add(this.CopyButton);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.Is32bit);
            this.tabPage1.Controls.Add(this.Is64Bit);
            this.tabPage1.Controls.Add(this.Help_BTN);
            this.tabPage1.ForeColor = System.Drawing.SystemColors.Desktop;
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(416, 244);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Install/Uninstall Options";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.LightGray;
            this.tabPage2.Controls.Add(this.CustomRun);
            this.tabPage2.Controls.Add(this.label20);
            this.tabPage2.Controls.Add(this.MobileTriage);
            this.tabPage2.Controls.Add(this.CustomInstallOption);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.CustomUninstallOptions);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(416, 244);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Install/Uninstall Custom Options";
            // 
            // CustomRun
            // 
            this.CustomRun.Location = new System.Drawing.Point(327, 213);
            this.CustomRun.Name = "CustomRun";
            this.CustomRun.Size = new System.Drawing.Size(75, 23);
            this.CustomRun.TabIndex = 6;
            this.CustomRun.Text = "Run";
            this.CustomRun.UseVisualStyleBackColor = true;
            this.CustomRun.Click += new System.EventHandler(this.CustomRun_Click);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(7, 153);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(137, 13);
            this.label20.TabIndex = 5;
            this.label20.Text = "Typical Mobile Client Triage";
            // 
            // MobileTriage
            // 
            this.MobileTriage.FormattingEnabled = true;
            this.MobileTriage.Items.AddRange(new object[] {
            "Set Folder Permissions",
            "Restart Updater Service",
            "Force Re-download of Updater Files",
            "Wipe out mobile client/Updater Files/Client Folders",
            "Run Mobile Interface Tester Utility",
            "Run Mobile Device Tester Utility",
            "Run Mobile GPS Tester Utility",
            "Run UBlox Virtual GPS  Work Around"});
            this.MobileTriage.Location = new System.Drawing.Point(9, 172);
            this.MobileTriage.Name = "MobileTriage";
            this.MobileTriage.Size = new System.Drawing.Size(290, 64);
            this.MobileTriage.TabIndex = 4;
            // 
            // CustomInstallOption
            // 
            this.CustomInstallOption.FormattingEnabled = true;
            this.CustomInstallOption.Items.AddRange(new object[] {
            "Install .Net",
            "Install SQL Compact 3.5",
            "Install GIS Components",
            "Install DB Providers",
            "Install New World Updater",
            "Run Updater Config Utility",
            "Set Folder Permissions",
            "Install CAD/MSP",
            "Install Incident Observer",
            "Install CAD Management Client",
            "Install ScenePD",
            "Restart Machine"});
            this.CustomInstallOption.Location = new System.Drawing.Point(215, 24);
            this.CustomInstallOption.Name = "CustomInstallOption";
            this.CustomInstallOption.Size = new System.Drawing.Size(188, 124);
            this.CustomInstallOption.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(215, 7);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(73, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Install Options";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 7);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(86, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Uninstall Options";
            // 
            // CustomUninstallOptions
            // 
            this.CustomUninstallOptions.FormattingEnabled = true;
            this.CustomUninstallOptions.Items.AddRange(new object[] {
            "Uninstall the Fire Client",
            "Uninstall the Police Client",
            "Uninstall the Merge Client",
            "Remove novaPDF",
            "Remove GIS Components",
            "Remove SQL Compact 3.5",
            "Remove Updater",
            "Remove Client Folders",
            "Remove Mobile Client Updater entries",
            "Uninstall MSP/CAD",
            "Restart Machine"});
            this.CustomUninstallOptions.Location = new System.Drawing.Point(7, 24);
            this.CustomUninstallOptions.Name = "CustomUninstallOptions";
            this.CustomUninstallOptions.Size = new System.Drawing.Size(202, 124);
            this.CustomUninstallOptions.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.AllowDrop = true;
            this.tabPage3.AutoScroll = true;
            this.tabPage3.BackColor = System.Drawing.Color.LightGray;
            this.tabPage3.Controls.Add(this.UpdaterAppend);
            this.tabPage3.Controls.Add(this.MergeClient);
            this.tabPage3.Controls.Add(this.FireClient);
            this.tabPage3.Controls.Add(this.PoliceClient);
            this.tabPage3.Controls.Add(this.MobileServer);
            this.tabPage3.Controls.Add(this.label11);
            this.tabPage3.Controls.Add(this.label9);
            this.tabPage3.Controls.Add(this.label8);
            this.tabPage3.Controls.Add(this.GenerateNumber);
            this.tabPage3.Controls.Add(this.FieldGenerateButton);
            this.tabPage3.Controls.Add(this.textBox1);
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(416, 244);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Configure Updater Util";
            // 
            // UpdaterAppend
            // 
            this.UpdaterAppend.Location = new System.Drawing.Point(7, 202);
            this.UpdaterAppend.Name = "UpdaterAppend";
            this.UpdaterAppend.Size = new System.Drawing.Size(84, 42);
            this.UpdaterAppend.TabIndex = 10;
            this.UpdaterAppend.Text = "Append";
            this.UpdaterAppend.UseVisualStyleBackColor = true;
            this.UpdaterAppend.Click += new System.EventHandler(this.UpdaterAppend_Click);
            // 
            // MergeClient
            // 
            this.MergeClient.AutoSize = true;
            this.MergeClient.Location = new System.Drawing.Point(9, 184);
            this.MergeClient.Name = "MergeClient";
            this.MergeClient.Size = new System.Drawing.Size(85, 17);
            this.MergeClient.TabIndex = 9;
            this.MergeClient.Text = "Merge Client";
            this.MergeClient.UseVisualStyleBackColor = true;
            // 
            // FireClient
            // 
            this.FireClient.AutoSize = true;
            this.FireClient.Location = new System.Drawing.Point(9, 160);
            this.FireClient.Name = "FireClient";
            this.FireClient.Size = new System.Drawing.Size(72, 17);
            this.FireClient.TabIndex = 8;
            this.FireClient.Text = "Fire Client";
            this.FireClient.UseVisualStyleBackColor = true;
            // 
            // PoliceClient
            // 
            this.PoliceClient.AutoSize = true;
            this.PoliceClient.Location = new System.Drawing.Point(10, 137);
            this.PoliceClient.Name = "PoliceClient";
            this.PoliceClient.Size = new System.Drawing.Size(84, 17);
            this.PoliceClient.TabIndex = 7;
            this.PoliceClient.Text = "Police Client";
            this.PoliceClient.UseVisualStyleBackColor = true;
            // 
            // MobileServer
            // 
            this.MobileServer.Location = new System.Drawing.Point(7, 111);
            this.MobileServer.Name = "MobileServer";
            this.MobileServer.Size = new System.Drawing.Size(146, 20);
            this.MobileServer.TabIndex = 6;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(4, 95);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(72, 13);
            this.label11.TabIndex = 5;
            this.label11.Text = "Mobile Server";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(328, 7);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 13);
            this.label9.TabIndex = 4;
            this.label9.Text = "FDIDs";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(221, 7);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(31, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "ORIs";
            // 
            // GenerateNumber
            // 
            this.GenerateNumber.Location = new System.Drawing.Point(89, 69);
            this.GenerateNumber.Name = "GenerateNumber";
            this.GenerateNumber.Size = new System.Drawing.Size(64, 20);
            this.GenerateNumber.TabIndex = 2;
            this.GenerateNumber.Text = "0";
            // 
            // FieldGenerateButton
            // 
            this.FieldGenerateButton.Location = new System.Drawing.Point(7, 69);
            this.FieldGenerateButton.Name = "FieldGenerateButton";
            this.FieldGenerateButton.Size = new System.Drawing.Size(75, 23);
            this.FieldGenerateButton.TabIndex = 1;
            this.FieldGenerateButton.Text = "Generate";
            this.FieldGenerateButton.UseVisualStyleBackColor = true;
            this.FieldGenerateButton.Click += new System.EventHandler(this.ORIGenerate_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(7, 4);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(172, 61);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = "Please enter the desired amount of ORI/FDID Fields and press the generate button." +
    "";
            // 
            // NWPSPreReqInstaller
            // 
            this.AcceptButton = this.Run;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(427, 296);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NWPSPreReqInstaller";
            this.Text = "Mobile Installer App";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
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

        public NWPSPreReqInstaller(Label label2)
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
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Label label20;
        private CheckedListBox MobileTriage;
        private CheckedListBox CustomInstallOption;
        private Label label7;
        private Label label6;
        private CheckedListBox CustomUninstallOptions;
        private Button CustomRun;
        private TabPage tabPage3;
        private TextBox textBox1;
        private Button FieldGenerateButton;
        private Label label9;
        private Label label8;
        private TextBox GenerateNumber;
        private TextBox MobileServer;
        private Label label11;
        private CheckBox FireClient;
        private CheckBox PoliceClient;
        private CheckBox MergeClient;
        private Button UpdaterAppend;

        public Label Label1 { get => label1; set => label1 = value; }
        public Button Button1 { get => button1; set => button1 = value; }
        public CheckBox CheckBox1 { get => checkBox1; set => checkBox1 = value; }
        public CheckBox CheckBox2 { get => checkBox2; set => checkBox2 = value; }
    }
}

