using System.Windows.Forms;

namespace Mobile_App
{
    partial class NWPSClientAdminTool
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NWPSClientAdminTool));
            this.label3 = new System.Windows.Forms.Label();
            this.UninstallMobile = new System.Windows.Forms.CheckBox();
            this.InstallMobile = new System.Windows.Forms.CheckBox();
            this.Run = new System.Windows.Forms.Button();
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
            this.InstallCAD = new System.Windows.Forms.CheckBox();
            this.InstallMSP = new System.Windows.Forms.CheckBox();
            this.UninstallCAD = new System.Windows.Forms.CheckBox();
            this.UninstallMSP = new System.Windows.Forms.CheckBox();
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
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.label39 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.SPD = new System.Windows.Forms.Label();
            this.SQLCP364 = new System.Windows.Forms.Label();
            this.SQLCP332 = new System.Windows.Forms.Label();
            this.CLR64 = new System.Windows.Forms.Label();
            this.CLR32 = new System.Windows.Forms.Label();
            this.GIS64 = new System.Windows.Forms.Label();
            this.GIS32 = new System.Windows.Forms.Label();
            this.Updater = new System.Windows.Forms.Label();
            this.SQLCP4 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.PreReqCheck = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
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
            this.UninstallMobile.Location = new System.Drawing.Point(5, 79);
            this.UninstallMobile.Name = "UninstallMobile";
            this.UninstallMobile.Size = new System.Drawing.Size(197, 17);
            this.UninstallMobile.TabIndex = 1;
            this.UninstallMobile.Text = "Uninstall Mobile Client and Pre Reqs";
            this.UninstallMobile.UseVisualStyleBackColor = true;
            // 
            // InstallMobile
            // 
            this.InstallMobile.AutoSize = true;
            this.InstallMobile.Location = new System.Drawing.Point(5, 102);
            this.InstallMobile.Name = "InstallMobile";
            this.InstallMobile.Size = new System.Drawing.Size(163, 17);
            this.InstallMobile.TabIndex = 3;
            this.InstallMobile.Text = "Install Mobile Client Pre Reqs";
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
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(86, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "OS Version";
            // 
            // Is64Bit
            // 
            this.Is64Bit.AutoSize = true;
            this.Is64Bit.Location = new System.Drawing.Point(6, 19);
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
            this.Is32bit.Location = new System.Drawing.Point(126, 19);
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
            this.Combo.Location = new System.Drawing.Point(6, 125);
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
            this.label5.Size = new System.Drawing.Size(290, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "File Path to Pre Req or Network Path to IMS Pre Req Folder";
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
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(3, 1);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(424, 273);
            this.tabControl1.TabIndex = 17;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.LightGray;
            this.tabPage1.Controls.Add(this.label12);
            this.tabPage1.Controls.Add(this.InstallCAD);
            this.tabPage1.Controls.Add(this.InstallMSP);
            this.tabPage1.Controls.Add(this.UninstallCAD);
            this.tabPage1.Controls.Add(this.UninstallMSP);
            this.tabPage1.Controls.Add(this.InstallMobile);
            this.tabPage1.Controls.Add(this.MSPServerPath);
            this.tabPage1.Controls.Add(this.UninstallMobile);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.Run);
            this.tabPage1.Controls.Add(this.Combo);
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
            // InstallCAD
            // 
            this.InstallCAD.AutoSize = true;
            this.InstallCAD.Location = new System.Drawing.Point(319, 125);
            this.InstallCAD.Name = "InstallCAD";
            this.InstallCAD.Size = new System.Drawing.Size(78, 17);
            this.InstallCAD.TabIndex = 20;
            this.InstallCAD.Text = "Install CAD";
            this.InstallCAD.UseVisualStyleBackColor = true;
            // 
            // InstallMSP
            // 
            this.InstallMSP.AutoSize = true;
            this.InstallMSP.Location = new System.Drawing.Point(214, 125);
            this.InstallMSP.Name = "InstallMSP";
            this.InstallMSP.Size = new System.Drawing.Size(79, 17);
            this.InstallMSP.TabIndex = 19;
            this.InstallMSP.Text = "Install MSP";
            this.InstallMSP.UseVisualStyleBackColor = true;
            // 
            // UninstallCAD
            // 
            this.UninstallCAD.AutoSize = true;
            this.UninstallCAD.Location = new System.Drawing.Point(319, 102);
            this.UninstallCAD.Name = "UninstallCAD";
            this.UninstallCAD.Size = new System.Drawing.Size(91, 17);
            this.UninstallCAD.TabIndex = 18;
            this.UninstallCAD.Text = "Uninstall CAD";
            this.UninstallCAD.UseVisualStyleBackColor = true;
            // 
            // UninstallMSP
            // 
            this.UninstallMSP.AutoSize = true;
            this.UninstallMSP.Location = new System.Drawing.Point(214, 102);
            this.UninstallMSP.Name = "UninstallMSP";
            this.UninstallMSP.Size = new System.Drawing.Size(92, 17);
            this.UninstallMSP.TabIndex = 17;
            this.UninstallMSP.Text = "Uninstall MSP";
            this.UninstallMSP.UseVisualStyleBackColor = true;
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
            "Update Host File",
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
            "Install ScenePD",
            "Install SQL Compact 4.0",
            "Install 2010 Visual Studio Tools",
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
            "Uninstall novaPDF",
            "Uninstall GIS Components",
            "Uninstall SQL Compact 3.5",
            "Uninstall Updater",
            "Delete Client Folders",
            "Remove Mobile Client Updater entries",
            "Uninstall MSP/CAD",
            "Uninstall SQL Compact 4.0",
            "Uninstall Scene PD",
            "Uninstall CAD Incident Observer",
            "Uninstall SQL CLR Types",
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
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.Color.LightGray;
            this.tabPage4.Controls.Add(this.label39);
            this.tabPage4.Controls.Add(this.label38);
            this.tabPage4.Controls.Add(this.label37);
            this.tabPage4.Controls.Add(this.label36);
            this.tabPage4.Controls.Add(this.label35);
            this.tabPage4.Controls.Add(this.label34);
            this.tabPage4.Controls.Add(this.label33);
            this.tabPage4.Controls.Add(this.label32);
            this.tabPage4.Controls.Add(this.label31);
            this.tabPage4.Controls.Add(this.label30);
            this.tabPage4.Controls.Add(this.label29);
            this.tabPage4.Controls.Add(this.label28);
            this.tabPage4.Controls.Add(this.label27);
            this.tabPage4.Controls.Add(this.label26);
            this.tabPage4.Controls.Add(this.SPD);
            this.tabPage4.Controls.Add(this.SQLCP364);
            this.tabPage4.Controls.Add(this.SQLCP332);
            this.tabPage4.Controls.Add(this.CLR64);
            this.tabPage4.Controls.Add(this.CLR32);
            this.tabPage4.Controls.Add(this.GIS64);
            this.tabPage4.Controls.Add(this.GIS32);
            this.tabPage4.Controls.Add(this.Updater);
            this.tabPage4.Controls.Add(this.SQLCP4);
            this.tabPage4.Controls.Add(this.label10);
            this.tabPage4.Controls.Add(this.PreReqCheck);
            this.tabPage4.Location = new System.Drawing.Point(4, 25);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(416, 244);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Pre Req Checker";
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(170, 224);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(100, 13);
            this.label39.TabIndex = 28;
            this.label39.Text = "Waiting Initialization";
            this.label39.Visible = false;
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(170, 211);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(100, 13);
            this.label38.TabIndex = 27;
            this.label38.Text = "Waiting Initialization";
            this.label38.Visible = false;
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(170, 198);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(100, 13);
            this.label37.TabIndex = 26;
            this.label37.Text = "Waiting Initialization";
            this.label37.Visible = false;
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(170, 185);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(100, 13);
            this.label36.TabIndex = 25;
            this.label36.Text = "Waiting Initialization";
            this.label36.Visible = false;
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(170, 126);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(100, 13);
            this.label35.TabIndex = 24;
            this.label35.Text = "Waiting Initialization";
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(170, 113);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(100, 13);
            this.label34.TabIndex = 23;
            this.label34.Text = "Waiting Initialization";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(170, 100);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(100, 13);
            this.label33.TabIndex = 22;
            this.label33.Text = "Waiting Initialization";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(170, 87);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(100, 13);
            this.label32.TabIndex = 21;
            this.label32.Text = "Waiting Initialization";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(170, 74);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(100, 13);
            this.label31.TabIndex = 20;
            this.label31.Text = "Waiting Initialization";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(170, 61);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(100, 13);
            this.label30.TabIndex = 19;
            this.label30.Text = "Waiting Initialization";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(170, 48);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(100, 13);
            this.label29.TabIndex = 18;
            this.label29.Text = "Waiting Initialization";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(170, 35);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(100, 13);
            this.label28.TabIndex = 17;
            this.label28.Text = "Waiting Initialization";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(170, 22);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(100, 13);
            this.label27.TabIndex = 16;
            this.label27.Text = "Waiting Initialization";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(170, 3);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(37, 13);
            this.label26.TabIndex = 15;
            this.label26.Text = "Status";
            // 
            // SPD
            // 
            this.SPD.AutoSize = true;
            this.SPD.Location = new System.Drawing.Point(5, 126);
            this.SPD.Name = "SPD";
            this.SPD.Size = new System.Drawing.Size(56, 13);
            this.SPD.TabIndex = 14;
            this.SPD.Text = "Scene PD";
            // 
            // SQLCP364
            // 
            this.SQLCP364.AutoSize = true;
            this.SQLCP364.Location = new System.Drawing.Point(5, 113);
            this.SQLCP364.Name = "SQLCP364";
            this.SQLCP364.Size = new System.Drawing.Size(141, 13);
            this.SQLCP364.TabIndex = 10;
            this.SQLCP364.Text = "64Bit SQL Compact 3.5 SP2";
            // 
            // SQLCP332
            // 
            this.SQLCP332.AutoSize = true;
            this.SQLCP332.Location = new System.Drawing.Point(5, 100);
            this.SQLCP332.Name = "SQLCP332";
            this.SQLCP332.Size = new System.Drawing.Size(144, 13);
            this.SQLCP332.TabIndex = 9;
            this.SQLCP332.Text = "32 Bit SQL Compact 3.5 SP2";
            // 
            // CLR64
            // 
            this.CLR64.AutoSize = true;
            this.CLR64.Location = new System.Drawing.Point(5, 87);
            this.CLR64.Name = "CLR64";
            this.CLR64.Size = new System.Drawing.Size(114, 13);
            this.CLR64.TabIndex = 8;
            this.CLR64.Text = "64 Bit SQL CLR Types";
            // 
            // CLR32
            // 
            this.CLR32.AutoSize = true;
            this.CLR32.Location = new System.Drawing.Point(6, 74);
            this.CLR32.Name = "CLR32";
            this.CLR32.Size = new System.Drawing.Size(114, 13);
            this.CLR32.TabIndex = 7;
            this.CLR32.Text = "32 Bit SQL CLR Types";
            // 
            // GIS64
            // 
            this.GIS64.AutoSize = true;
            this.GIS64.Location = new System.Drawing.Point(6, 61);
            this.GIS64.Name = "GIS64";
            this.GIS64.Size = new System.Drawing.Size(117, 13);
            this.GIS64.TabIndex = 6;
            this.GIS64.Text = "64 Bit GIS Components";
            // 
            // GIS32
            // 
            this.GIS32.AutoSize = true;
            this.GIS32.Location = new System.Drawing.Point(6, 48);
            this.GIS32.Name = "GIS32";
            this.GIS32.Size = new System.Drawing.Size(117, 13);
            this.GIS32.TabIndex = 5;
            this.GIS32.Text = "32 Bit GIS Components";
            // 
            // Updater
            // 
            this.Updater.AutoSize = true;
            this.Updater.Location = new System.Drawing.Point(5, 22);
            this.Updater.Name = "Updater";
            this.Updater.Size = new System.Drawing.Size(45, 13);
            this.Updater.TabIndex = 4;
            this.Updater.Text = "Updater";
            // 
            // SQLCP4
            // 
            this.SQLCP4.AutoSize = true;
            this.SQLCP4.Location = new System.Drawing.Point(5, 35);
            this.SQLCP4.Name = "SQLCP4";
            this.SQLCP4.Size = new System.Drawing.Size(126, 13);
            this.SQLCP4.TabIndex = 3;
            this.SQLCP4.Text = "SQL Compact Edition 4.0";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 3);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(51, 13);
            this.label10.TabIndex = 1;
            this.label10.Text = "Pre Reqs";
            // 
            // PreReqCheck
            // 
            this.PreReqCheck.Location = new System.Drawing.Point(338, 221);
            this.PreReqCheck.Name = "PreReqCheck";
            this.PreReqCheck.Size = new System.Drawing.Size(75, 23);
            this.PreReqCheck.TabIndex = 0;
            this.PreReqCheck.Text = "Check";
            this.PreReqCheck.UseVisualStyleBackColor = true;
            this.PreReqCheck.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(89, 53);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(118, 13);
            this.label12.TabIndex = 21;
            this.label12.Text = "Install/Uninstall Options";
            // 
            // NWPSClientAdminTool
            // 
            this.AcceptButton = this.Run;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(427, 296);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NWPSClientAdminTool";
            this.Text = "NWPS Client Admin Tool";
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
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
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
        private System.Windows.Forms.Label label4;

        public NWPSClientAdminTool(Label label2)
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
        private TabPage tabPage4;
        private Button PreReqCheck;
        private Label GIS32;
        private Label Updater;
        private Label SQLCP4;
        private Label label10;
        private Label label39;
        private Label label38;
        private Label label37;
        private Label label36;
        private Label label35;
        private Label label34;
        private Label label33;
        private Label label32;
        private Label label31;
        private Label label30;
        private Label label29;
        private Label label28;
        private Label label27;
        private Label label26;
        private Label SPD;
        private Label SQLCP364;
        private Label SQLCP332;
        private Label CLR64;
        private Label CLR32;
        private Label GIS64;
        private CheckBox InstallCAD;
        private CheckBox InstallMSP;
        private CheckBox UninstallCAD;
        private CheckBox UninstallMSP;
        private Label label12;

        public Label Label1 { get => label1; set => label1 = value; }
        public Button Button1 { get => button1; set => button1 = value; }
        public CheckBox CheckBox1 { get => checkBox1; set => checkBox1 = value; }
        public CheckBox CheckBox2 { get => checkBox2; set => checkBox2 = value; }
    }
}

