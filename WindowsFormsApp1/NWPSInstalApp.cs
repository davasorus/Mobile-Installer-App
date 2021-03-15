﻿using MobileInstallApp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.AccessControl;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

//Created 12/23/2017
//Created by Sean Davitt, Tyler Tech NWPS Mobile Implementation Specialist
//Designed for 64bit and 32 bit Windows

//Changed .net requirement from 4.6.1 to 4.5 02/05/2018

namespace Mobile_App
{
    partial class NWPSClientAdminTool : Form
    {
        private XmlDocument UpdaterConfig = new XmlDocument();
        private XmlDocument StartupSettings = new XmlDocument();

        private BackgroundWorker Tab1bg;
        private BackgroundWorker Tab2bg;
        private BackgroundWorker Tab3bg;

        private string DotNet47 = "dotNetFx471_Full_setup_Offline.exe";
        private string DotNet48 = "ndp48-x86-x64-allos-enu.exe";
        private string SQLCE3532 = "SSCERuntime_x86-ENU.msi";
        private string SQLCE3564 = "SSCERuntime_x64-ENU.msi";
        private string SQLCE4032 = "SSCERuntime_x86-ENU-4.0.exe";
        private string SQLCE4064 = "SSCERuntime_x64-ENU-4.0.exe";
        private string NWPSGIS32 = "NewWorld.Gis.Components.x86.msi";
        private string NWPSGIS64 = "NewWorld.Gis.Components.x64.msi";
        private string MSSYNC64 = "Synchronization-v2.1-x64-ENU.msi";
        private string MSPROSERV64 = "ProviderServices-v2.1-x64-ENU.msi";
        private string MSDBPRO64 = "DatabaseProviders-v3.1-x64-ENU.msi";
        private string MSSYNC32 = "Synchronization-v2.1-x86-ENU.msi";
        private string MSPROSERV32 = "ProviderServices-v2.1-x86-ENU.msi";
        private string MSDBPRO32 = "DatabaseProviders-v3.1-x86-ENU.msi";
        private string NWPSUPDATE = "NewWorld.Management.Updater.msi";
        private string SQLCLR32 = "SQLSysClrTypesx86.msi";
        private string SQLCLR64 = "SQLSysClrTypesx64.msi";
        private string SCPD6 = "SPD6-4-8993.exe";
        private string SCPD6AX = "SPDX6-4-3083.exe";
        private string SCPD4 = "SPD4-0-92.exe";
        private string MSPClient = "NewWorldMSPClient.msi";
        private string CADClient64 = "NewWorld.Enterprise.CAD.Client.x64.msi";
        private string CADClient32 = "NewWorld.Enterprise.CAD.Client.x86.msi";
        private string CADIncObs64 = "NewWorld.Enterprise.CAD.IncidentObserver.x64.msi";

        private string LocalRun = @"C:\Temp\MobileInstaller";
        private bool flag = false;

        public string MSPServerName { get; private set; }
        private bool PoliceClientExists = false;
        private bool FireClientExists = false;
        private bool MergeClientExists = false;
        private int j = 0;
        private string SourcePath = @"";
        private string TargetPath = @"";
        private bool is64bit = false;
        private bool installmobile = false;
        private bool uninstallmobile = false;
        private List<string> serverlocation_text = new List<string>();
        public bool Is64bit { get => is64bit; set => is64bit = value; }
        public bool Installmobile { get => installmobile; set => installmobile = value; }
        public bool Uninstallmobile { get => uninstallmobile; set => uninstallmobile = value; }
        public string ClientPath1 { get => TargetPath; set => TargetPath = value; }
        public string SourcePath1 { get => SourcePath; set => SourcePath = value; }
        public object AddonCopyText { get; private set; }

        private NWPSADDONDOWNLOAD secondForm = new NWPSADDONDOWNLOAD();

        private ToolStripLabel ts = new ToolStripLabel();

        //what the form does when it initializes every time
        private void Form1_Load(object sender, EventArgs e)
        {
            string LogEntry = @"----------- Start of log file" + " " + DateTime.Now + "-----------";

            //this will put the current date of application start per run. This allows for easier readability.
            if (File.Exists("NWPSAdminLog.txt"))
            {
                using (StreamWriter file = new StreamWriter(("NWPSAdminLog.txt"), true))
                {
                    file.WriteLine(LogEntry);
                }
            }
            else
            {
                using (StreamWriter file = new StreamWriter(("NWPSAdminLog.txt"), true))
                {
                    file.WriteLine(LogEntry);
                }
            }

            InitialLoadofXML();

            statusStrip1.Items.AddRange(new ToolStripItem[] { ts });
            BeginInvoke((Action)(() => ts.Text = "Ready"));
            BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

            //checks if a directory exists to determine a 64 or 32 bit machine and configures the check boxes accordingly.
            if (Directory.Exists("C:\\Program files (x86)"))
            {
                Is64Bit.Checked = true;
                Is32bit.Checked = false;
            }
            else
            {
                Is64Bit.Checked = false;
                Is32bit.Checked = true;
            }

            // will utilize prior configuration for mobile updater form if it exists
            //if there is a non 0 number in the configuration XML there will that many ORI and FDID fields created on startup
            try
            {
                if (GenerateNumber.Text != " ")
                {
                    if (GenerateNumber.Text != "")
                    {
                        int num = int.Parse(GenerateNumber.Text);
                        if (num > 0)
                        {
                            ORIGenerate_Click(sender, e);
                        }
                        else
                        {
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry1 = DateTime.Now + " " + ex.ToString();

                LogEntryWriter(LogEntry1);
            }
        }

        //background worker and component initialize
        public NWPSClientAdminTool()
        {
            InitializeComponent();

            //copy background worker
            Tab1bg = new BackgroundWorker();
            Tab1bg.DoWork += Tab1bg_DoWork;
            Tab1bg.ProgressChanged += Tab1bg_ProgressChanged;
            Tab1bg.RunWorkerCompleted += Tab1bg_RunWorkerCompleted;
            Tab1bg.WorkerReportsProgress = true;

            Tab3bg = new BackgroundWorker();
            Tab3bg.DoWork += Tab3bg_DoWork;

            Tab2bg = new BackgroundWorker();
            Tab2bg.DoWork += Tab2bg_DoWork;
        }

        //---------------------------Button Click events----------------------

        //run button for TabPage1
        //used when doing mobile client upgrade/removal/install
        private void Button2_Click(object sender, EventArgs e)
        {
            if (!Tab3bg.IsBusy)
            {
                Tab3bg.RunWorkerAsync();
            }
            else
            {
                //I hate that this works...
                //so this empty else statement allows the above async worker to actually work as designed.
                //I am unsure why THIS async button event needs to iterate twice before actually running the specified code.
                //the IF background worker is not busy run to an empty else is the only way I was able to get this work "properly"
                //any assistance would be appreciated
            }
        }

        //Copy Button
        private void CopyFiles_Click(object sender, EventArgs e)
        {
            ProgressBar.Visible = true;
            ProgressBar.Enabled = true;

            BeginInvoke((Action)(() => ts.Text = "prepping Temp Folder"));
            Temp();

            SaveStartupSettings();

            BeginInvoke((Action)(() => ts.ForeColor = Color.DarkSlateBlue));
            BeginInvoke((Action)(() => ts.Text = "Copying required files locally"));

            string LogEntry1 = DateTime.Now + " File Copy Initiated";

            LogEntryWriter(LogEntry1);

            ProgressBar.Minimum = 0;
            ProgressBar.Maximum = 50;
            Tab1bg.RunWorkerAsync();
        }

        //Help button logic on button click - WIP
        private void Button3_Click(object sender, EventArgs e)
        {
            byte[] resourceFile = NWPSAdminApp.Properties.Resources.NWPS_Installer_App_User_Guide;

            string destination = Path.Combine(Path.GetTempPath(), "NWPS_Installer_App_User_Guide.docx");
            System.IO.File.WriteAllBytes(destination, resourceFile);
            Process.Start(destination);
        }

        //Customizable Install/Uninstall/Triage Run Button
        //this is pressed when uninstalling/Installing certain portions of the software suite
        private void CustomRun_Click(object sender, EventArgs e)
        {
            ProgressBar.Visible = false;
            ProgressBar.Enabled = false;

            Tab2bg.RunWorkerAsync();
        }

        //work done when the append button is pressed
        //this will modify the updater configuration file for mobile
        //this has been enhanced from the Updater Config tool V2 to account for dynamically changing run conditions
        private void UpdaterAppend_Click(object sender, EventArgs e)
        {
            if (Is64Bit.Checked == true)
            {
                UpdaterConfig.Load(@"C:\Program Files (x86)\New World Systems\New World Automatic Updater\NewWorld.Management.Updater.Service.exe.config");
            }
            if (Is32bit.Checked == true)
            {
                UpdaterConfig.Load(@"C:\Program Files\New World Systems\New World Automatic Updater\NewWorld.Management.Updater.Service.exe.config");
            }

            SeeIfNodesExist();

            UpdateXMLORI();

            UpdateXMLFDID();

            //this creates the ORI text boxes
            //this also modifies the pre req installer config XML for each ORI
            foreach (Control c in tabPage3.Controls)
            {
                if (c.Name.Contains("ORI"))
                {
                    if (c.Text != "")
                    {
                        BeginInvoke((Action)(() => ts.Text = "ORIs Added"));
                        string ORI = c.Text;
                        string Name = c.Name;

                        CreateXMLORI(ORI, Name);

                        ORISub(ORI);
                    }
                }
            }

            //this creates the ORI text boxes
            //this also modifies the pre req installer config XML for each FDID
            foreach (Control c in tabPage3.Controls)
            {
                if (c.Name.Contains("FDID"))
                {
                    if (c.Text != "")
                    {
                        BeginInvoke((Action)(() => ts.Text = "FDIDs Added"));
                        string FDID = c.Text;
                        string Name = c.Name;

                        CreateXMLFDID(FDID, Name);

                        FDIDSub(FDID);
                    }
                }
            }

            if (PoliceClient.Checked && PoliceClientExists == false)
            {
                PoliceClientSub();
            }

            if (FireClient.Checked && FireClientExists == false)
            {
                FireClientSub();
            }

            if (MergeClient.Checked && MergeClientExists == false)
            {
                MergeClientSub();
            }

            NewWorldUpdaterSub();

            if (Is64Bit.Checked == true)
            {
                UpdaterConfig.Save(@"C:\Program Files (x86)\New World Systems\New World Automatic Updater\NewWorld.Management.Updater.Service.exe.config");
            }
            if (Is32bit.Checked == true)
            {
                UpdaterConfig.Save(@"C:\Program Files\New World Systems\New World Automatic Updater\NewWorld.Management.Updater.Service.exe.config");
            }

            ServiceController myService = new ServiceController
            {
                ServiceName = "NewWorldUpdaterService"
            };

            string svcStatus = myService.Status.ToString();

            if (svcStatus == "Running")
            {
                myService.Stop();
                myService.WaitForStatus(ServiceControllerStatus.Stopped);
                myService.Start();
            }
            else if (svcStatus == "Stopped")
            {
                myService.Start();
            }
            else
            {
                myService.Stop();
                myService.WaitForStatus(ServiceControllerStatus.Stopped);
                myService.Start();
            }

            SaveStartupSettings();
        }

        //work done when the generate button is pressed
        //the button that actually does the drawing and naming of text boxes used to append the updater config file
        private void ORIGenerate_Click(object sender, EventArgs e)
        {
            label20.Visible = true;
            label8.Visible = true;

            RemoveFormEntries();

            FieldGenerateButton.Visible = true;
            GenerateNumber.Visible = true;
            textBox1.Visible = true;
            //creates ORIs
            try
            {
                int txtno = int.Parse(GenerateNumber.Text);
                int pointX = 190;
                int pointY = 30;

                for (int i = 0; i < txtno; i++)
                {
                    TextBox a = new TextBox
                    {
                        Name = "ORI" + (i + 1)
                    };
                    a.Tag = a.Name;

                    a.Location = new Point(pointX, pointY);

                    tabPage3.Controls.Add(a);
                    tabPage3.Show();
                    pointY += 20;

                    string logEntry = DateTime.Now + " ORI/FDID" + (i + 1) + " added ";

                    LogEntryWriter(logEntry);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

                string LogEntry = DateTime.Now + " " + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            LoadORIXML();

            //Creates FDIDs
            try
            {
                int txtno = int.Parse(GenerateNumber.Text);
                int pointX = 295;
                int pointY = 30;

                for (int i = 0; i < txtno; i++)
                {
                    TextBox a = new TextBox
                    {
                        Name = "FDID" + (i + 1)
                    };

                    a.Location = new Point(pointX, pointY);

                    tabPage3.Controls.Add(a);
                    tabPage3.Show();
                    pointY += 20;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

                string LogEntry = DateTime.Now + " " + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            LoadFDIDXML();
        }

        //PreReqChecker code
        //this is a dedicated tab to check for prior installed/uninstalled
        //if someone presses the button it will check, if people use the normal install/uninstall process it will update as they run.
        private void button2_Click_1(object sender, EventArgs e)
        {
            ProgressBar.Visible = false;
            ProgressBar.Enabled = false;
            PreReqCheck.Visible = false;

            PreStatusChecker();

            ts.ForeColor = System.Drawing.Color.DarkSlateBlue;
            ts.Text = "Running Pre Req Checker";

            //
            //The below code block does the actual checking and updating of the status on button click
            //

            //updater check
            try
            {
                if (label27.Text == "Pending")
                {
                    ts.Text = "Checking for Updater";
                    ts.ForeColor = Color.DarkSlateBlue;

                    if (PreReqChecker("New World Automatic Updater") == true)
                    {
                        label27.Text = "Installed";
                        label27.ForeColor = System.Drawing.Color.ForestGreen;
                    }
                    else
                    {
                        label27.Text = "Uninstalled";
                        label27.ForeColor = System.Drawing.Color.OrangeRed;
                    }
                }
                else
                {
                    ts.Text = "Updater is already installed";
                    ts.ForeColor = System.Drawing.Color.ForestGreen;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + " " + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            //SQL Server Compact 4.0 Check
            try
            {
                if (label28.Text == "Pending")
                {
                    ts.Text = "Checking for SQL Server Compact 4.0";
                    ts.ForeColor = Color.DarkSlateBlue;

                    if (Is64Bit.Checked == true)
                    {
                        if (PreReqChecker("Microsoft SQL Server Compact 4.0 x64 ENU") == true)
                        {
                            label28.Text = "Installed";
                            label28.ForeColor = System.Drawing.Color.ForestGreen;
                        }
                        else
                        {
                            label28.Text = "Uninstalled";
                            label28.ForeColor = System.Drawing.Color.OrangeRed;
                        }
                    }
                    else
                    {
                        if (PreReqChecker("Microsoft SQL Server Compact 4.0 x86 ENU") == true)
                        {
                            label28.Text = "Installed";
                            label28.ForeColor = System.Drawing.Color.ForestGreen;
                        }
                        else
                        {
                            label28.Text = "Uninstalled";
                            label28.ForeColor = System.Drawing.Color.OrangeRed;
                        }
                    }
                }
                else
                {
                    ts.Text = "SQL Compact 4.0 already installed";
                    ts.ForeColor = System.Drawing.Color.ForestGreen;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + " " + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            //32 bit GIS check
            try
            {
                if (label29.Text == "Pending")
                {
                    ts.Text = "Checking for 32bit GIS Components";
                    ts.ForeColor = Color.DarkSlateBlue;

                    if (PreReqChecker("New World GIS Components x86") == true)
                    {
                        label29.Text = "Installed";
                        label29.ForeColor = System.Drawing.Color.ForestGreen;
                    }
                    else
                    {
                        label29.Text = "Uninstalled";
                        label29.ForeColor = System.Drawing.Color.OrangeRed;
                    }
                }
                else
                {
                    ts.Text = "32bit GIS is already installed";
                    ts.ForeColor = System.Drawing.Color.ForestGreen;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + " " + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            //64bit GIS Check
            try
            {
                if (label30.Text == "Pending")
                {
                    ts.Text = "Checking for 32bit GIS Components";
                    ts.ForeColor = Color.DarkSlateBlue;

                    if (PreReqChecker("New World GIS Components x64") == true)
                    {
                        label30.Text = "Installed";
                        label30.ForeColor = System.Drawing.Color.ForestGreen;
                    }
                    else
                    {
                        label30.Text = "Uninstalled";
                        label30.ForeColor = System.Drawing.Color.OrangeRed;
                    }
                }
                else
                {
                    ts.Text = "64bit GIS is already installed";
                    ts.ForeColor = System.Drawing.Color.ForestGreen;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + " " + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            //32bit SQL Server CLR Types check
            try
            {
                if (label31.Text == "Pending")
                {
                    ts.Text = "Checking for 32bit  SQL Server CLR Types";
                    ts.ForeColor = Color.DarkSlateBlue;

                    if (PreReqChecker("Microsoft SQL Server System CLR Types") == true)
                    {
                        label31.Text = "Installed";
                        label31.ForeColor = System.Drawing.Color.ForestGreen;
                    }
                    else
                    {
                        label31.Text = "Uninstalled";
                        label31.ForeColor = System.Drawing.Color.OrangeRed;
                    }
                }
                else
                {
                    ts.Text = "32bit SQL CLR Types is already installed";
                    ts.ForeColor = System.Drawing.Color.ForestGreen;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + " " + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            //64bit SQL ServerCLR Types check
            try
            {
                if (label32.Text == "Pending")
                {
                    ts.Text = "Checking for 64bit  SQL Server CLR Types";
                    ts.ForeColor = Color.DarkSlateBlue;

                    if (PreReqChecker("Microsoft SQL Server System CLR Types (x64)") == true)
                    {
                        label32.Text = "Installed";
                        label32.ForeColor = System.Drawing.Color.ForestGreen;
                    }
                    else
                    {
                        label32.Text = "Uninstalled";
                        label32.ForeColor = System.Drawing.Color.OrangeRed;
                    }
                }
                else
                {
                    ts.Text = "64 bit SQL CLR Types is already installed";
                    ts.ForeColor = System.Drawing.Color.ForestGreen;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + " " + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            //32bit SQL Server 3.5 SP2 check
            try
            {
                if (label33.Text == "Pending")
                {
                    ts.Text = "Checking for 32bit SQL Server Compact 3.5 SP2";
                    ts.ForeColor = Color.DarkSlateBlue;

                    if (PreReqChecker("Microsoft SQL Server Compact 3.5 SP2 ENU") == true)
                    {
                        label33.Text = "Installed";
                        label33.ForeColor = System.Drawing.Color.ForestGreen;
                    }
                    else
                    {
                        label33.Text = "Uninstalled";
                        label33.ForeColor = System.Drawing.Color.OrangeRed;
                    }
                }
                else
                {
                    ts.Text = "32 bit SQL Server 3.5 SP2 is already installed";
                    ts.ForeColor = System.Drawing.Color.ForestGreen;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + " " + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            //64 SQL server 3.5 SP2 check
            try
            {
                if (label34.Text == "Pending")
                {
                    ts.Text = "Checking for 64bit SQL Server Compact 3.5 SP2";
                    ts.ForeColor = Color.DarkSlateBlue;

                    if (PreReqChecker("Microsoft SQL Server Compact 3.5 SP2 x64 ENU") == true)
                    {
                        label34.Text = "Installed";
                        label34.ForeColor = System.Drawing.Color.ForestGreen;
                    }
                    else
                    {
                        label34.Text = "Uninstalled";
                        label34.ForeColor = System.Drawing.Color.OrangeRed;
                    }
                }
                else
                {
                    ts.Text = "64 bit SQL Server 3.5 SP2 is already installed";
                    ts.ForeColor = System.Drawing.Color.ForestGreen;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + " " + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            //scene PD check
            try
            {
                if (label35.Text == "Pending")
                {
                    ts.Text = "Checking for ScenePD";
                    ts.ForeColor = Color.DarkSlateBlue;

                    if (PreReqChecker("ScenePD 6 Desktop Edition") == true)
                    {
                        label35.Text = "Scene PD 6 Installed";
                        label35.ForeColor = System.Drawing.Color.ForestGreen;

                        if (PreReqChecker("ScenePD 6 ActiveX Control") == true)
                        {
                            label35.Text = "Scene PD 6, ActiveX Installed";
                            label35.ForeColor = System.Drawing.Color.ForestGreen;
                        }
                        else
                        {
                            label35.Text = "Scene PD 6 Installed, ActiveX not";
                            label35.ForeColor = System.Drawing.Color.ForestGreen;
                        }
                    }
                    else if (PreReqChecker("ScenePD 4") == true)
                    {
                        label35.Text = "ScenePD 4 Installed";
                        label35.ForeColor = System.Drawing.Color.ForestGreen;
                    }
                    else
                    {
                        label35.Text = "Uninstalled";
                        label35.ForeColor = System.Drawing.Color.OrangeRed;
                    }
                }
                else
                {
                    ts.Text = "ScenePD is already installed";
                    ts.ForeColor = System.Drawing.Color.ForestGreen;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + " " + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            ts.Text = "Pre Req Checker is Complete";
            ts.ForeColor = System.Drawing.Color.ForestGreen;
            PreReqCheck.Visible = true;
        }

        //----------------------pre req install/uninstall methods---------------------

        //Mobile 64bit uninstaller
        //this will uninstall NWPS Mobile, NWS Mobile, Pre Reqs for 64bit mobile
        private void Mobile64Uninstall()
        {
            BeginInvoke((Action)(() => ts.Text = "Checking to uninstall Police Mobile"));
            UninstallProgram("Aegis Mobile");

            UninstallProgram("Law Enforcement Mobile");
            BeginInvoke((Action)(() => ts.Text = "Police Mobile is Uninstalled"));

            BeginInvoke((Action)(() => ts.Text = "Checking to uninstall Fire Mobile"));
            UninstallProgram("Aegis Fire Mobile");

            UninstallProgram("Fire Mobile");
            BeginInvoke((Action)(() => ts.Text = "Fire Mobile is Uninstalled"));

            BeginInvoke((Action)(() => ts.Text = "Checking to uninstall Mobile Merge"));
            UninstallProgram("Aegis Mobile Merge");

            UninstallProgram("Mobile Merge");
            BeginInvoke((Action)(() => ts.Text = "Mobile Merge is Uninstalled"));

            BeginInvoke((Action)(() => ts.Text = "Uninstalling DB Providers"));
            UninstallProgram("Microsoft Sync Framework 3.1 Database Providers (x64) ENU");

            BeginInvoke((Action)(() => ts.Text = "Uninstalling Provider Services"));
            UninstallProgram("Microsoft Sync Framework 2.1 Provider Services (x64) ENU");

            BeginInvoke((Action)(() => ts.Text = "Uninstalling Core Components"));
            UninstallProgram("Microsoft Sync Framework 2.1 Core Components (x64) ENU");

            BeginInvoke((Action)(() => ts.Text = "Uninstalling GIS - Old"));
            UninstallProgram("New World GIS Components");

            BeginInvoke((Action)(() => ts.Text = "Uninstalling GIS - New"));

            if (label29.Text != "Uninstalled")
            {
                UninstallProgram("New World GIS Components x64");
            }
            else
            {
                BeginInvoke((Action)(() => ts.Text = "64bit GIS Components already Uninstalled"));
                string logentry1 = DateTime.Now + " 64 bit GIS Components is already uninstalled. Pre Req Checker not modified.";
                LogEntryWriter(logentry1);
            }

            if (label28.Text != "Uninstalled")
            {
                UninstallProgram("New World GIS Components x86");
                BeginInvoke((Action)(() => ts.Text = "GIS is Uninstalled"));
            }
            else
            {
                BeginInvoke((Action)(() => ts.Text = "32bit GIS Components already Uninstalled"));
                string logentry1 = DateTime.Now + " 32 bit GIS Components is already uninstalled. Pre Req Checker not modified.";
                LogEntryWriter(logentry1);
            }

            BeginInvoke((Action)(() => ts.Text = "Uninstalling SQL Server Compact 3.5 SP2"));
            if (label34.Text != "Uninstalled")
            {
                UninstallProgram("Microsoft SQL Server Compact 3.5 SP2 x64 ENU");
            }
            else
            {
                BeginInvoke((Action)(() => ts.Text = "64 bit SQL Server Compact 3.5 SP2 is already Uninstalled"));
                string logentry1 = DateTime.Now + " 64 bit SQL Server Compact 3.5 SP2 is already uninstalled. Pre Req Checker not modified.";
                LogEntryWriter(logentry1);
            }

            if (label33.Text != "Uninstalled")
            {
                UninstallProgram("Microsoft SQL Server Compact 3.5 SP2 ENU");
                BeginInvoke((Action)(() => ts.Text = "SQL Server Compact 3.5 SP2 is Uninstalled"));
            }
            else
            {
                BeginInvoke((Action)(() => ts.Text = "32bit SQL Server Compact 3.5 SP2 is already Uninstalled"));
                string logentry1 = DateTime.Now + " 32bit SQL Server Compact 3.5 SP2 is already uninstalled. Pre Req Checker not modified.";
                LogEntryWriter(logentry1);
            }

            BeginInvoke((Action)(() => ts.Text = "Uninstalling Nova PDF"));

            UninstallProgram("NWPS Enterprise Mobile PDF Printer");

            UninstallProgram("novaPDF 8 Printer Driver");

            UninstallProgram("novaPDF 8 SDK COM (x86)");

            UninstallProgram("novaPDF 8 SDK COM (x64)");
        }

        //Mobile 64bit installer
        //this will install the Pre Reqs required for 64bit mobile, ensure folder permissions are correct, and that the updater is configured for mobile.
        private void Mobile64install()
        {
            DotNet();

            SQLCE35();

            GIS();

            DBProviderService();

            BeginInvoke((Action)(() => ts.Text = "Installing Updater"));
            BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
            UpdaterInstaller();

            BeginInvoke((Action)(() => ts.Text = "Running Mobile Updater Config form"));
            BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
            try
            {
                if (GenerateNumber.Text == "0")
                {
                    BeginInvoke((Action)(() => ts.Text = "Please Verify the Updater portion is configured and attempt again."));
                    BeginInvoke((Action)(() => Run.Visible = true));
                    throw new ArgumentException(@"ERROR: Updater Configuration section of the utility is not configured, please fill out the tab and try again.");
                }
                else
                {
                    UpdaterAppend_Click(new object(), new EventArgs());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + " " + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            BeginInvoke((Action)(() => ts.Text = "Prepping folder permissions"));
            BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
            try
            {
                SetAcl(@"C:\Program Files (x86)\New World Systems");

                string LogEntry = DateTime.Now + @" C:\Program Files (x86)\New World Systems has User permissions set.";

                LogEntryWriter(LogEntry);
                SetAcl(@"C:\ProgramData\New World Systems");

                string LogEntry1 = DateTime.Now + @" C:\ProgramData\New World Systems has User permissions set.";

                LogEntryWriter(LogEntry1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + " " + ex.ToString();

                LogEntryWriter(LogEntry);
            }
        }

        //Mobile 32bit uninstaller
        //this will uninstall NWPS Mobile, NWS Mobile, Pre Reqs for 32bit mobile
        private void Mobile32Uninstaller()
        {
            BeginInvoke((Action)(() => ts.Text = "Checking to uninstall Police Mobile"));
            UninstallProgram("Aegis Mobile");

            UninstallProgram("Law Enforcement Mobile");
            BeginInvoke((Action)(() => ts.Text = "Police Mobile is Uninstalled"));

            BeginInvoke((Action)(() => ts.Text = "Checking to uninstall Fire Mobile"));
            UninstallProgram("Aegis Fire Mobile");

            UninstallProgram("Fire Mobile");
            BeginInvoke((Action)(() => ts.Text = "Fire Mobile is Uninstalled"));

            BeginInvoke((Action)(() => ts.Text = "Checking to uninstall Mobile Merge"));
            UninstallProgram("Aegis Mobile Merge");

            UninstallProgram("Mobile Merge");
            BeginInvoke((Action)(() => ts.Text = "Mobile Merge is Uninstalled"));

            BeginInvoke((Action)(() => ts.Text = "Uninstalling DB Providers"));
            UninstallProgram("Microsoft Sync Framework 3.1 Database Providers (x86) ENU");

            BeginInvoke((Action)(() => ts.Text = "Uninstalling Provider Services"));
            UninstallProgram("Microsoft Sync Framework 2.1 Provider Services (x86) ENU");

            BeginInvoke((Action)(() => ts.Text = "Uninstalling Core Components"));
            UninstallProgram("Microsoft Sync Framework 2.1 Core Components (x86) ENU");

            BeginInvoke((Action)(() => ts.Text = "Uninstalling GIS - Old"));
            UninstallProgram("New World GIS Components");

            BeginInvoke((Action)(() => ts.Text = "Uninstalling GIS - New"));
            if (label28.Text != "Uninstalled")
            {
                UninstallProgram("New World GIS Components x86");
                BeginInvoke((Action)(() => ts.Text = "GIS is Uninstalled"));
            }
            else
            {
                BeginInvoke((Action)(() => ts.Text = "32bit GIS Components already Uninstalled"));
                string logentry1 = DateTime.Now + " 32 bit GIS Components is already uninstalled. Pre Req Checker not modified.";
                LogEntryWriter(logentry1);
            }

            BeginInvoke((Action)(() => ts.Text = "Uninstalling SQL Serer Compact 3.5 SP2"));
            if (label33.Text != "Uninstalled")
            {
                UninstallProgram("Microsoft SQL Server Compact 3.5 SP2 ENU");
                BeginInvoke((Action)(() => ts.Text = "SQL Server Compact 3.5 SP2 is Uninstalled"));
            }
            else
            {
                BeginInvoke((Action)(() => ts.Text = "32bit SQL Server Compact 3.5 SP2 is already Uninstalled"));
                string logentry1 = DateTime.Now + " 32bit SQL Server Compact 3.5 SP2 is already uninstalled. Pre Req Checker not modified.";
                LogEntryWriter(logentry1);
            }

            BeginInvoke((Action)(() => ts.Text = "Uninstalling Nova PDF"));

            UninstallProgram("NWPS Enterprise Mobile PDF Printer");

            UninstallProgram("novaPDF 8 Printer Driver");

            UninstallProgram("novaPDF 8 SDK COM (x86)");

            UninstallProgram("novaPDF 8 SDK COM (x64)");
        }

        //Mobile 32bit Installer
        //this will install the Pre Reqs required for 32bit mobile, ensure folder permissions are correct, and that the updater is configured for mobile.
        private void Mobile32install()
        {
            DotNet();

            SQLCE35();

            GIS();

            DBProviderService();

            BeginInvoke((Action)(() => ts.Text = "Installing Updater"));
            BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
            UpdaterInstaller();

            BeginInvoke((Action)(() => ts.Text = "Running Mobile Updater Config form"));
            BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
            try
            {
                if (GenerateNumber.Text == "0")
                {
                    BeginInvoke((Action)(() => ts.Text = "Please Verify the Updater portion is configured and attempt again."));
                    BeginInvoke((Action)(() => Run.Visible = true));
                    throw new ArgumentException(@"ERROR: Updater Configuration section of the utility is not configured, please fill out the tab and try again.");
                }
                else
                {
                    UpdaterAppend_Click(new object(), new EventArgs());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + " " + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            BeginInvoke((Action)(() => ts.Text = "Prepping folder permissions"));
            BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
            try
            {
                SetAcl(@"C:\Program Files\New World Systems");

                string LogEntry = DateTime.Now + @" C:\Program Files\New World Systems has User permissions set";

                LogEntryWriter(LogEntry);

                SetAcl(@"C:\ProgramData\New World Systems");

                string LogEntry1 = DateTime.Now + @" C:\ProgramData\New World Systems has User permissions set.";

                LogEntryWriter(LogEntry1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + " " + ex.ToString();

                LogEntryWriter(LogEntry);
            }
        }

        //-------------------Installation code functions----------------------

        //dot net 4.7 or 4.8 installer
        private void DotNet()
        {
            if (Directory.Exists(LocalRun))
            {
                if (File.Exists(Path.Combine(LocalRun, DotNet47)))
                {
                    BeginInvoke((Action)(() => ts.Text = "Running 4.7.1 .Net"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                    try
                    {
                        InstallProgram(DotNet47, LocalRun);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace.ToString());

                        string LogEntry = DateTime.Now + " " + ex.ToString();

                        LogEntryWriter(LogEntry);
                    }

                    BeginInvoke((Action)(() => ts.Text = ".Net 4.7.1 installed"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
                }
                //installed .net 4.8 if 4.7.1 is not present
                else
                {
                    BeginInvoke((Action)(() => ts.Text = "Running 4.8 .Net"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                    try
                    {
                        InstallProgram(DotNet48, LocalRun);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace.ToString());

                        string LogEntry = DateTime.Now + " " + ex.ToString();

                        LogEntryWriter(LogEntry);
                    }

                    BeginInvoke((Action)(() => ts.Text = ".Net 4.8 installed"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
                }
            }
            else
            {
                if (PreReqSearch(MSPServerPath.Text + @"\\_Client-Installation\", DotNet48) == 1)
                {
                    BeginInvoke((Action)(() => ts.Text = "Running 4.8 .Net"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

                    PreReqRun(MSPServerPath.Text + @"\\_Client-Installation\", DotNet48);

                    BeginInvoke((Action)(() => ts.Text = ".Net 4.8 installed"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
                }
                else
                {
                    BeginInvoke((Action)(() => ts.Text = "Running 4.7.1 .Net"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

                    PreReqRun(MSPServerPath.Text + @"\\_Client-Installation\", DotNet47);

                    BeginInvoke((Action)(() => ts.Text = ".Net 4.7.1 installed"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
                }
            }
        }

        //SQL Compact 3.5 installer
        private void SQLCE35()
        {
            if (Is64Bit.Checked == true)
            {
                BeginInvoke((Action)(() => ts.Text = "Running 32bit SQL Runtime"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                try
                {
                    if (label33.Text != "Installed")
                    {
                        if (File.Exists(Path.Combine(LocalRun, SQLCE3532)))
                        {
                            InstallProgram(SQLCE3532, LocalRun);
                        }
                        else
                        {
                            PreReqRun(MSPServerPath.Text + @"\\_Client-Installation\", SQLCE3532);
                        }
                    }
                    else
                    {
                        string logentry1 = DateTime.Now + " 32 bit SQL 3.5 SP2 Runtime is already installed. This step was skipped.";
                        LogEntryWriter(logentry1);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());

                    string LogEntry = DateTime.Now + " " + ex.ToString();

                    LogEntryWriter(LogEntry);
                }

                BeginInvoke((Action)(() => ts.Text = "Running 64 bit SQL Runtime"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                try
                {
                    if (label34.Text != "Installed")
                    {
                        if (File.Exists(Path.Combine(LocalRun, SQLCE3532)))
                        {
                            InstallProgram(SQLCE3564, LocalRun);
                        }
                        else
                        {
                            PreReqRun(MSPServerPath.Text + @"\\_Client-Installation\", SQLCE3564);
                        }
                    }
                    else
                    {
                        string logentry2 = DateTime.Now + " 64 bit SQL 3.5 SP2 Runtime is already installed. This step was skipped.";
                        LogEntryWriter(logentry2);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());

                    string LogEntry = DateTime.Now + " " + ex.ToString();

                    LogEntryWriter(LogEntry);
                }
            }
            else
            {
                BeginInvoke((Action)(() => ts.Text = "Running 32bit SQL Runtime"));
                try
                {
                    if (label33.Text != "Installed")
                    {
                        if (File.Exists(Path.Combine(LocalRun, SQLCE3532)))
                        {
                            InstallProgram(SQLCE3532, LocalRun);
                        }
                        else
                        {
                            PreReqRun(MSPServerPath.Text + @"\\_Client-Installation\", SQLCE3532);
                        }
                    }
                    else
                    {
                        string logentry1 = DateTime.Now + " 32 bit SQL 3.5 SP2 Runtime is already installed. This step was skipped.";
                        LogEntryWriter(logentry1);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());

                    string LogEntry = DateTime.Now + " " + ex.ToString();

                    LogEntryWriter(LogEntry);
                }
            }
        }

        //32bit and 64bit GIS installer
        private void GIS()
        {
            if (Is64Bit.Checked == true)
            {
                BeginInvoke((Action)(() => ts.Text = "Running 32 bit GIS Components"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                try
                {
                    if (label29.Text != "Installed")
                    {
                        if (File.Exists(Path.Combine(LocalRun, (NWPSGIS32))))
                        {
                            InstallProgram(NWPSGIS32, LocalRun);
                        }
                        else
                        {
                            PreReqRun(MSPServerPath.Text + @"\\_Client-Installation\", (NWPSGIS32));
                        }
                    }
                    else
                    {
                        string logentry1 = DateTime.Now + " 32 bit GIS Components is already installed. This step was skipped.";
                        LogEntryWriter(logentry1);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());

                    string LogEntry = DateTime.Now + " " + ex.ToString();

                    LogEntryWriter(LogEntry);
                }

                BeginInvoke((Action)(() => ts.Text = "Running 64 bit GIS Components"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                try
                {
                    if (label30.Text != "Installed")
                    {
                        if (File.Exists(Path.Combine(LocalRun, NWPSGIS64)))
                        {
                            InstallProgram(NWPSGIS64, LocalRun);
                        }
                        else
                        {
                            PreReqRun(MSPServerPath.Text + @"\\_Client-Installation\", NWPSGIS64);
                        }
                    }
                    else
                    {
                        string logentry2 = DateTime.Now + " 64 bit GIS Components is already installed. This step was skipped.";
                        LogEntryWriter(logentry2);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());

                    string LogEntry = DateTime.Now + " " + ex.ToString();

                    LogEntryWriter(LogEntry);
                }
            }
            else
            {
                BeginInvoke((Action)(() => ts.Text = "Running 32 bit GIS Components"));
                try
                {
                    if (label29.Text != "Installed")
                    {
                        if (File.Exists(Path.Combine(LocalRun, (NWPSGIS32))))
                        {
                            InstallProgram(NWPSGIS32, LocalRun);
                        }
                        else
                        {
                            PreReqRun(MSPServerPath.Text + @"\\_Client-Installation\", (NWPSGIS32));
                        }
                    }
                    else
                    {
                        string logentry1 = DateTime.Now + " 32 bit GIS Components is already installed. This step was skipped.";
                        LogEntryWriter(logentry1);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());

                    string LogEntry = DateTime.Now + " " + ex.ToString();

                    LogEntryWriter(LogEntry);
                }
            }
        }

        //MS Sync Service provider installer
        private void DBProviderService()
        {
            if (Is64Bit.Checked == true)
            {
                BeginInvoke((Action)(() => ts.Text = "Running 64 bit Synchronization"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                try
                {
                    if (File.Exists(Path.Combine(LocalRun, MSSYNC64)))
                    {
                        InstallProgram(MSSYNC64, LocalRun);
                    }
                    else
                    {
                        PreReqRun(MSPServerPath.Text + @"\\_Client-Installation\", MSSYNC64);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());

                    string LogEntry = DateTime.Now + " " + ex.ToString();

                    LogEntryWriter(LogEntry);
                }

                BeginInvoke((Action)(() => ts.Text = "Running 64 bit Provider Services"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                try
                {
                    if (File.Exists(Path.Combine(LocalRun, MSPROSERV64)))
                    {
                        InstallProgram(MSPROSERV64, LocalRun);
                    }
                    else
                    {
                        PreReqRun(MSPServerPath.Text + @"\\_Client-Installation\", MSPROSERV64);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());

                    string LogEntry = DateTime.Now + " " + ex.ToString();

                    LogEntryWriter(LogEntry);
                }

                BeginInvoke((Action)(() => ts.Text = "Running 64 bit DB Providers"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                try
                {
                    if (File.Exists(Path.Combine(LocalRun, MSDBPRO64)))
                    {
                        InstallProgram(MSDBPRO64, LocalRun);
                    }
                    else
                    {
                        PreReqRun(MSPServerPath.Text + @"\\_Client-Installation\", MSDBPRO64);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());

                    string LogEntry = DateTime.Now + " " + ex.ToString();

                    LogEntryWriter(LogEntry);
                }
            }
            else
            {
                BeginInvoke((Action)(() => ts.Text = "Running 32 bit Synchronization"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                try
                {
                    if (File.Exists(Path.Combine(LocalRun, MSSYNC32)))
                    {
                        InstallProgram(MSSYNC32, LocalRun);
                    }
                    else
                    {
                        PreReqRun(MSPServerPath.Text + @"\\_Client-Installation\", MSSYNC32);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());

                    string LogEntry = DateTime.Now + " " + ex.ToString();

                    LogEntryWriter(LogEntry);
                }

                BeginInvoke((Action)(() => ts.Text = "Running 32 bit Provider Services"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                try
                {
                    if (File.Exists(Path.Combine(LocalRun, MSPROSERV32)))
                    {
                        InstallProgram(MSPROSERV32, LocalRun);
                    }
                    else
                    {
                        PreReqRun(MSPServerPath.Text + @"\\_Client-Installation\", MSPROSERV32);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());

                    string LogEntry = DateTime.Now + " " + ex.ToString();

                    LogEntryWriter(LogEntry);
                }

                BeginInvoke((Action)(() => ts.Text = "Running 32 bit DB Providers"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                try
                {
                    if (File.Exists(Path.Combine(LocalRun, MSPROSERV32)))
                    {
                        InstallProgram(MSDBPRO32, LocalRun);
                    }
                    else
                    {
                        PreReqRun(MSPServerPath.Text + @"\\_Client-Installation\", MSPROSERV32);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());

                    string LogEntry = DateTime.Now + " " + ex.ToString();

                    LogEntryWriter(LogEntry);
                }
            }
        }

        //NWPS Updater installer
        private void UpdaterInstaller()
        {
            try
            {
                if (label27.Text != "Installed")
                {
                    BeginInvoke((Action)(() => ts.Text = "Installing Updater"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

                    if (File.Exists(Path.Combine(LocalRun, NWPSUPDATE)))
                    {
                        InstallProgram(NWPSUPDATE, LocalRun);
                    }
                    else
                    {
                        PreReqRun(MSPServerPath.Text + @"\\_Client-Installation\", NWPSUPDATE);
                    }
                }
                else
                {
                    string logentry1 = DateTime.Now + " New World Updater is already installed. This step was skipped.";
                    LogEntryWriter(logentry1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + " " + ex.ToString();

                LogEntryWriter(LogEntry);
            }
        }

        //ScenePD installer
        private void ScenePD()
        {
            //will check for scene pd 6 before displaying install prompt
            // If scenepd 6 install is denied this will check for scene pd 4 before displaying install prompt
            //if scene pd 4 is not located and that is the desired scene pd version a message is displayed with paths to move folders
            if (File.Exists(@"C:\Temp\MobileInstaller\SPD6-4-8993.exe"))
            {
                BeginInvoke((Action)(() => ts.Text = "ScenePD Install Prompt"));
                string title = "ScenePD Install Prompt";
                string message = "would you like to Install ScenePD 6?";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = MessageBox.Show(message, title, buttons);

                //scene pd 6 install
                if (result == DialogResult.Yes)
                {
                    BeginInvoke((Action)(() => ts.Text = "Installing ScenePD 6"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                    string LogEntry = DateTime.Now + @" Attempting to install ScenePD 6";

                    LogEntryWriter(LogEntry);

                    if (label35.Text != "Installed")
                    {
                        if (File.Exists(Path.Combine(LocalRun, SCPD6)))
                        {
                            RunProgram(SCPD6, LocalRun);
                        }
                        else
                        {
                            PreReqRun(MSPServerPath.Text + @"\\_Client-Installation\", SCPD6);
                        }
                        if (File.Exists(Path.Combine(LocalRun, SCPD6AX)))
                        {
                            RunProgram(SCPD6AX, LocalRun);
                        }
                        else
                        {
                            PreReqRun(MSPServerPath.Text + @"\\_Client-Installation\", SCPD6AX);
                        }
                    }
                    else
                    {
                        string logentry3 = DateTime.Now + " ScenePD6 is already installed. This step was skipped.";
                        LogEntryWriter(logentry3);
                    }

                    string LogEntry1 = DateTime.Now + @" ScenePD 6 Installed";
                    string LogEntry2 = DateTime.Now + @" ScenePD ActiveX Installed";

                    LogEntryWriter(LogEntry1);
                    LogEntryWriter(LogEntry2);

                    BeginInvoke((Action)(() => ts.Text = "ScenePD 6 Installed"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
                }

                //scene pd 4 install
                if (result == DialogResult.No)
                {
                    if (File.Exists(@"C:\Temp\MobileInstaller\SPD4-0-92.exe"))
                    {
                        string title1 = "ScenePD Install Prompt";
                        string message1 = "would you like to ScenePD 4?";
                        MessageBoxButtons buttons1 = MessageBoxButtons.YesNo;
                        DialogResult result1 = MessageBox.Show(message1, title1, buttons1);
                        if (result1 == DialogResult.Yes)
                        {
                            BeginInvoke((Action)(() => ts.Text = "Installing ScenePD 4"));
                            BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

                            string LogEntry = DateTime.Now + @" Attempting to install ScenePD 4";

                            LogEntryWriter(LogEntry);

                            if (label35.Text != "Installed")
                            {
                                if (File.Exists(Path.Combine(LocalRun, SCPD4)))
                                {
                                    RunProgram(SCPD4, LocalRun);
                                }
                                else
                                {
                                    PreReqRun(MSPServerPath.Text + @"\\_Client-Installation\", SCPD4);
                                }
                            }
                            else
                            {
                                string logentry4 = DateTime.Now + " ScenePD4 is already installed. This Step was skipped.";
                                LogEntryWriter(logentry4);
                            }

                            string LogEntry1 = DateTime.Now + @" ScenePD 4 Installed";

                            LogEntryWriter(LogEntry1);

                            BeginInvoke((Action)(() => ts.Text = "ScenePD 4 installed"));
                            BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
                        }
                    }
                    else
                    {
                        MessageBox.Show(@"Scene PD 4 was not found, please go to the \\>MobileServerName\C$\NWS Hold\
							Client Initial Setup and Installation\7  Install Scene PD, and put your preferred version in C:\Temp\MobileInstaller.");
                    }
                }
            }

            //will check for scene pd 4 before displaying install prompt
            else if (File.Exists(@"C:\Temp\MobileInstaller\SPD4-0-92.exe"))
            {
                string title1 = "ScenePD Install Prompt";
                string message1 = "would you like to ScenePD 4?";
                MessageBoxButtons buttons1 = MessageBoxButtons.YesNo;
                DialogResult result1 = MessageBox.Show(message1, title1, buttons1);
                //ScenePD 4 install
                if (result1 == DialogResult.Yes)
                {
                    BeginInvoke((Action)(() => ts.Text = "Installing ScenePD 4"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

                    string LogEntry = DateTime.Now + @" Attempting to install ScenePD 4";

                    LogEntryWriter(LogEntry);

                    if (label35.Text != "Installed")
                    {
                        if (File.Exists(Path.Combine(LocalRun, SCPD4)))
                        {
                            RunProgram(SCPD4, LocalRun);
                        }
                        else
                        {
                            PreReqRun(MSPServerPath.Text + @"\\_Client-Installation\", SCPD4);
                        }
                    }
                    else
                    {
                        string logentry4 = DateTime.Now + " ScenePD4 is already installed. This Step was skipped.";
                        LogEntryWriter(logentry4);
                    }

                    string LogEntry1 = DateTime.Now + @" ScenePD 4 Installed";

                    LogEntryWriter(LogEntry1);

                    BeginInvoke((Action)(() => ts.Text = "ScenePD 4 installed"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
                }
            }

            //this will check in the NWS Addons folder for scene pd 6
            else if (File.Exists(@"C:\Temp\MobileInstaller\NWS Addons\SPD6-4-8993.exe"))
            {
                BeginInvoke((Action)(() => ts.Text = "ScenePD Install Prompt"));
                string title = "ScenePD Install Prompt";
                string message = "would you like to Install ScenePD 6?";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = MessageBox.Show(message, title, buttons);

                //scene pd 6 install
                if (result == DialogResult.Yes)
                {
                    BeginInvoke((Action)(() => ts.Text = "Installing ScenePD 6"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                    string LogEntry = DateTime.Now + @" Attempting to install ScenePD 6";

                    LogEntryWriter(LogEntry);

                    if (label35.Text != "Installed")
                    {
                        RunProgram(SCPD6, @"C:\Temp\MobileInstaller\NWS Addons\");
                        RunProgram(SCPD6AX, @"C:\Temp\MobileInstaller\NWS Addons\");
                    }
                    else
                    {
                        string logentry3 = DateTime.Now + " ScenePD6 is already installed. This step was skipped.";
                        LogEntryWriter(logentry3);
                    }

                    string LogEntry1 = DateTime.Now + @" ScenePD 6 Installed";
                    string LogEntry2 = DateTime.Now + @" ScenePD ActiveX installed";

                    LogEntryWriter(LogEntry1);
                    LogEntryWriter(LogEntry2);

                    BeginInvoke((Action)(() => ts.Text = "ScenePD 6 Installed"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
                }

                //scene pd 4 install
                if (result == DialogResult.No)
                {
                    if (File.Exists(@"C:\Temp\MobileInstaller\NWS Addons\SPD4-0-92.exe"))
                    {
                        string title1 = "ScenePD Install Prompt";
                        string message1 = "would you like to ScenePD 4?";
                        MessageBoxButtons buttons1 = MessageBoxButtons.YesNo;
                        DialogResult result1 = MessageBox.Show(message1, title1, buttons1);
                        if (result1 == DialogResult.Yes)
                        {
                            BeginInvoke((Action)(() => ts.Text = "Installing ScenePD 4"));
                            BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

                            string LogEntry = DateTime.Now + @" Attempting to install ScenePD 4";

                            LogEntryWriter(LogEntry);
                            if (label35.Text != "Installed")
                            {
                                RunProgram(SCPD4, @"C:\Temp\MobileInstaller\NWS Addons\");
                            }
                            else
                            {
                                string logentry4 = DateTime.Now + " ScenePD4 is already installed. This Step was skipped.";
                                LogEntryWriter(logentry4);
                            }

                            string LogEntry1 = DateTime.Now + @" ScenePD 4 Installed";

                            LogEntryWriter(LogEntry1);

                            BeginInvoke((Action)(() => ts.Text = "ScenePD 4 installed"));
                            BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
                        }
                    }
                    else
                    {
                        MessageBox.Show(@"Please Select a Scene PD version to install.");
                    }
                }
            }

            //will check for scene pd 4 before displaying install prompt -- in the NWS addons folder
            else if (File.Exists(@"C:\Temp\MobileInstaller\NWS Addons\SPD4-0-92.exe"))
            {
                string title1 = "ScenePD Install Prompt";
                string message1 = "would you like to ScenePD 4?";
                MessageBoxButtons buttons1 = MessageBoxButtons.YesNo;
                DialogResult result1 = MessageBox.Show(message1, title1, buttons1);
                //ScenePD 4 install
                if (result1 == DialogResult.Yes)
                {
                    BeginInvoke((Action)(() => ts.Text = "Installing ScenePD 4"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

                    string LogEntry = DateTime.Now + @" Attempting to install ScenePD 4";

                    LogEntryWriter(LogEntry);

                    if (label35.Text != "Installed")
                    {
                        RunProgram(SCPD4, @"C:\Temp\MobileInstaller\NWS Addons");
                    }
                    else
                    {
                        string logentry4 = DateTime.Now + " ScenePD4 is already installed. This Step was skipped.";
                        LogEntryWriter(logentry4);
                    }

                    string LogEntry1 = DateTime.Now + @" ScenePD 4 Installed";

                    LogEntryWriter(LogEntry1);

                    BeginInvoke((Action)(() => ts.Text = "ScenePD 4 installed"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
                }
            }

            //if scene pd is not found at all it must be moved by the user to a displayed location
            else
            {
                string LogEntry = DateTime.Now + @" ERROR: COULD NOT LOCATE SCENE PD 4 OR 6. Attempting to Download";

                LogEntryWriter(LogEntry);

                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.OrangeRed));
                BeginInvoke((Action)(() => ts.Text = "Scene PD could not be found. Please Enter Path to the ScenePD folder."));

                string LogEntry1 = DateTime.Now + @" Addon Download Folder Displayed.";

                LogEntryWriter(LogEntry1);

                BeginInvoke((Action)(() => secondForm.Show()));
            }
        }

        //SQL Compact 4.0 installer
        private void SQLCE40()
        {
            if (Is64Bit.Checked == true)
            {
                BeginInvoke((Action)(() => ts.Text = "Running 64 bit SQL Runtime"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                try
                {
                    if (label28.Text != "Installed")
                    {
                        try
                        {
                            PreReqRename("SSCERuntime_x64-ENU.exe", SQLCE4064, "SQL Compact Edition 4.0");
                            PreReqRename("SSCERuntime_x86-ENU.exe", SQLCE4032, "SQL Compact Edition 4.0");
                        }
                        catch
                        {
                        }
                        if (File.Exists(Path.Combine(LocalRun, SQLCE4064)))
                        {
                            RunProgram(SQLCE4064, LocalRun);
                        }
                        else
                        {
                            PreReqRun(MSPServerPath.Text + @"\\_Client-Installation\", SQLCE4064);
                        }

                        BeginInvoke((Action)(() => ts.Text = "SQL Compact 4.0 Installed"));
                        BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
                    }
                    else
                    {
                        string logentry2 = DateTime.Now + " 64 bit SQL Compact 4.0 is already installed. This step was skipped";
                        LogEntryWriter(logentry2);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());

                    string LogEntry = DateTime.Now + " " + ex.ToString();

                    LogEntryWriter(LogEntry);
                }
            }
            else
            {
                BeginInvoke((Action)(() => ts.Text = "Running 32bit SQL Compact 4.0 Runtime"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                try
                {
                    if (label28.Text != "Installed")
                    {
                        if (File.Exists(Path.Combine(LocalRun, SQLCE4032)))
                        {
                            RunProgram(SQLCE4032, LocalRun);
                        }
                        else
                        {
                            PreReqRename("SSCERuntime_x86-ENU.exe", SQLCE4032, "SQL Compact Edition 4.0");

                            PreReqRun(MSPServerPath.Text + @"\\_Client-Installation\", SQLCE4064);
                        }

                        BeginInvoke((Action)(() => ts.Text = "32bit SQL Compact 4.0 Runtime Installed"));
                        BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
                    }
                    else
                    {
                        string logentry2 = DateTime.Now + " 32 bit SQL Compact 4.0 is already installed. This step was skipped";
                        LogEntryWriter(logentry2);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());

                    string LogEntry = DateTime.Now + " " + ex.ToString();

                    LogEntryWriter(LogEntry);
                }
            }
        }

        //visual studio 2010 installer
        private void VS2010()
        {
            try
            {
                if (File.Exists(Path.Combine(LocalRun, "vstor_redist.exe")))
                {
                    BeginInvoke((Action)(() => ts.Text = "Running Primary Interop Assemblies for Office"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

                    InstallProgram(@"vstor_redist.exe", LocalRun);

                    BeginInvoke((Action)(() => ts.Text = "VS 2010 Tools Installed"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
                }
                else
                {
                    BeginInvoke((Action)(() => ts.Text = "Running Primary Interop Assemblies for Office"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

                    PreReqRun(MSPServerPath.Text + @"\\_Client-Installation\", "vstor_redist.exe");

                    BeginInvoke((Action)(() => ts.Text = "VS 2010 Tools Installed"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + " " + ex.ToString();

                LogEntryWriter(LogEntry);
            }
        }

        //SQL Server CLR Types 2008 64bit and 32bit
        private void SQLCLR2008()
        {
            BeginInvoke((Action)(() => ts.Text = "SQL Server CLR Types 2008"));
            BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
            try
            {
                if (label31.Text != "Installed")
                {
                    if (File.Exists(Path.Combine(LocalRun, SQLCLR32)))
                    {
                        InstallProgram(SQLCLR32, LocalRun);
                    }
                    else
                    {
                        PreReqRun(MSPServerPath.Text + @"\\_Client-Installation\", SQLCLR32);
                    }
                }
                else
                {
                    string logentry1 = DateTime.Now + " 32 bit SQL Server CLR Types is already installed. This step was skipped.";
                    LogEntryWriter(logentry1);
                }

                if (label32.Text != "")
                {
                    if (File.Exists(Path.Combine(LocalRun, SQLCLR64)))
                    {
                        InstallProgram(SQLCLR64, LocalRun);
                    }
                    else
                    {
                        PreReqRun(MSPServerPath.Text + @"\\_Client-Installation\", SQLCLR64);
                    }
                }
                else
                {
                    string logentry2 = DateTime.Now + " 64 bit SQL Server CLR Types is already installed. This step was skipped.";
                    LogEntryWriter(logentry2);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + " " + ex.ToString();

                LogEntryWriter(LogEntry);
            }
        }

        //MSP Client Install
        private void MSP()
        {
            BeginInvoke((Action)(() => ts.Text = "Installing MSP"));
            BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
            try
            {
                if (File.Exists(Path.Combine(LocalRun, MSPClient)))
                {
                    RunProgram(MSPClient, LocalRun);
                }
                else
                {
                    PreReqRun(MSPServerPath.Text + @"\\_Client-Installation\", MSPClient);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + " " + ex.ToString();

                LogEntryWriter(LogEntry);

                BeginInvoke((Action)(() => ts.Text = "MSP is Installed"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
            }
        }

        //CAD Client Install
        private void CAD()
        {
            BeginInvoke((Action)(() => ts.Text = "Installing CAD"));
            BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
            try
            {
                if (Is64Bit.Checked == true)
                {
                    if (File.Exists(Path.Combine(LocalRun, CADClient64)))
                    {
                        RunProgram(CADClient64, LocalRun);

                        BeginInvoke((Action)(() => ts.Text = "Enterprise CAD Installed"));
                        BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
                    }
                    else
                    {
                        PreReqRun(MSPServerPath.Text + @"\\_Client-Installation\", CADClient64);

                        BeginInvoke((Action)(() => ts.Text = "Enterprise CAD Installed"));
                        BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
                    }
                }
                else
                {
                    if (File.Exists(Path.Combine(LocalRun, CADClient32)))
                    {
                        RunProgram(CADClient32, LocalRun);

                        BeginInvoke((Action)(() => ts.Text = "Enterprise CAD Installed"));
                        BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
                    }
                    else
                    {
                        PreReqRun(MSPServerPath.Text + @"\\_Client-Installation\", CADClient32);

                        BeginInvoke((Action)(() => ts.Text = "Enterprise CAD Installed"));
                        BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + " " + ex.ToString();

                LogEntryWriter(LogEntry);
            }
        }

        //CAD Incident Observer Installer
        //used to view/create AVL history packages
        private void IncidentObserver()
        {
            BeginInvoke((Action)(() => ts.Text = "Installing Incident Observer"));
            BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

            string LogEntry = DateTime.Now + @" Attempting to install Incident Observer";

            LogEntryWriter(LogEntry);

            if (File.Exists(Path.Combine(LocalRun, CADIncObs64)))
            {
                InstallProgram(CADIncObs64, LocalRun);
            }
            else
            {
                PreReqRun(MSPServerPath.Text + @"\\_Client-Installation\", CADIncObs64);
            }

            string LogEntry1 = DateTime.Now + @" Incident Observer installed";

            LogEntryWriter(LogEntry1);

            BeginInvoke((Action)(() => ts.Text = "Incident Observer is installed"));
            BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
        }

        //-----------------File related work---------------

        //will look into the updater config file and will replace any text that contains MobileUpdates with DeleteMe
        private void FileWork64Bit()
        {
            string text = File.ReadAllText(@"C:\Program Files (x86)\New World Systems\New World Automatic Updater\NewWorld.Management.Updater.Service.exe.config");
            text = text.Replace(@"MobileUpdates", "DeleteMe");
            File.WriteAllText(@"C:\Program Files (x86)\New World Systems\New World Automatic Updater\NewWorld.Management.Updater.Service.exe.config", text);

            string EntryLog = DateTime.Now + " Updater config file modified";

            LogEntryWriter(EntryLog);
        }

        //will look into the updater config file and remove any lines that contain DeleteMe
        private void UpdaterWork64Bit()
        {
            string text = @"C:\Program Files (x86)\New World Systems\New World Automatic Updater\NewWorld.Management.Updater.Service.exe.config";

            string[] Lines = File.ReadAllLines(text);
            IEnumerable<string> newLines = Lines.Where(line => !line.Contains(@"/DeleteMe/"));
            File.WriteAllLines(text, newLines);

            string EntryLog = DateTime.Now + " mobile updater file entries removed";

            LogEntryWriter(EntryLog);
        }

        //Will look into the updater config file and will replace any text that contains MobileUpdates with DeleteMe
        private void FileWork32Bit()
        {
            string text = File.ReadAllText(@"C:\Program Files\New World Systems\New World Automatic Updater\NewWorld.Management.Updater.Service.exe.config");
            text = text.Replace(@"MobileUpdates", "DeleteMe");
            File.WriteAllText(@"C:\Program Files\New World Systems\New World Automatic Updater\NewWorld.Management.Updater.Service.exe.config", text);

            string EntryLog = DateTime.Now + " Updater config file modified";

            LogEntryWriter(EntryLog);
        }

        //will look into the updater config file and remove any lines that contain DeleteMe
        private void UpdaterWork32Bit()
        {
            string text = @"C:\Program Files\New World Systems\New World Automatic Updater\NewWorld.Management.Updater.Service.exe.config";

            var Lines = File.ReadAllLines(text);
            var newLines = Lines.Where(line => !line.Contains(@"/DeleteMe/"));
            File.WriteAllLines(text, newLines);

            string EntryLog = DateTime.Now + " mobile updater file entries removed";

            LogEntryWriter(EntryLog);
        }

        //Mobile copy
        //this will copy a file from one location to another location as sent by PreReqSearch above
        private void MobileCopy(string FileNamePath)
        {
            try
            {
                string filename = Path.GetFileName(FileNamePath);

                string replace = Path.Combine(LocalRun, filename);

                File.Copy(FileNamePath, replace, true);
                File.SetAttributes(TargetPath, FileAttributes.Normal);

                Tab1bg.ReportProgress(0);

                string LogEntry = DateTime.Now + " " + filename + " has been copied.";

                LogEntryWriter(LogEntry);
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains("used by another process"))
                {
                    //CloseProcess(filename);
                }
                else
                {
                    string LogEntry = DateTime.Now + " " + ex.ToString();

                    LogEntryWriter(LogEntry);
                }
            }
        }

        //Mobile copy
        //this will copy all files located at the NWSHoldPath.txt to the MobileInstaller folder within C:\Temp
        private void MobileCopy1(string SourcePath)
        {
            string TargetPath = @"C:\Temp\MobileInstaller\NWS Addons";
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(SourcePath, "*",
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(SourcePath, TargetPath));

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(SourcePath, "*.*",
                SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(SourcePath, TargetPath), true);

            string LogEntry = DateTime.Now + "NWS Addon Folder Copied.";

            LogEntryWriter(LogEntry);
        }

        //this is designed to relabel the SQL compact 4.0 64bit and 32bit components.
        //this is so that CAD and the other applications will be able to have the correct pre reqs
        private void PreReqRename(string FileName, string NewName, string SubFOlderSearch)
        {
            try
            {
                foreach (var directory in Directory.GetDirectories(MSPServerPath.Text + @"\\_Client-Installation\"))
                {
                    foreach (var filename in Directory.GetFiles(directory))
                    {
                        string sourcepath = Path.GetDirectoryName(filename);

                        //if the directory of a found file contains the text of a searched term/s
                        //then a specific file is renamed to a desired name.
                        if (sourcepath.ToString().Contains(SubFOlderSearch))
                        {
                            File.Move(Path.Combine(sourcepath, FileName), Path.Combine(sourcepath, NewName));

                            string LogEntry = DateTime.Now + " " + FileName + " has been renamed to" + NewName;

                            LogEntryWriter(LogEntry);
                        }
                        else
                        {
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //if the exception error contains the text file not found (system.IO.FileNotFound)
                //then specific text is entered into the log file.
                if (ex.ToString().Contains("FileNotFound"))
                {
                    string LogEntry = DateTime.Now + " " + "could not find " + FileName + " to rename." + FileName + " likely is already renamed or is not present.";

                    LogEntryWriter(LogEntry);
                }
                else
                {
                    string LogEntry = DateTime.Now + " " + ex.ToString();

                    LogEntryWriter(LogEntry);
                }
            }
        }

        //this will iterate through folders to find
        private void PreReqRun(string sDir, string PreReqName)
        {
            try
            {
                foreach (var directory in Directory.GetDirectories(sDir))
                {
                    foreach (var filename in Directory.GetFiles(directory))
                    {
                        //if the file name of a found file matches what we are looking for code is run
                        if (Path.GetFileName(filename) == PreReqName)
                        {
                            //if the file we found(that matches what we are looking for) matches specific pre reqs
                            //then specific UI code is ran for the install
                            //otherwise it is silently installed
                            if (Path.GetFileName(filename) == SQLCE4064)
                            {
                                RunProgram(PreReqName, Path.GetDirectoryName(filename));
                            }
                            else if (Path.GetFileName(filename) == SQLCE4032)
                            {
                                RunProgram(PreReqName, Path.GetDirectoryName(filename));
                            }
                            else if (Path.GetFileName(filename) == MSPClient)
                            {
                                RunProgram(PreReqName, Path.GetDirectoryName(filename));
                            }
                            else if (Path.GetFileName(filename) == CADClient64)
                            {
                                RunProgram(PreReqName, Path.GetDirectoryName(filename));
                            }
                            else if (Path.GetFileName(filename) == CADClient32)
                            {
                                RunProgram(PreReqName, Path.GetDirectoryName(filename));
                            }
                            else
                            {
                                InstallProgram(PreReqName, Path.GetDirectoryName(filename));
                            }
                        }
                    }
                    PreReqRun(directory, PreReqName);
                }
            }
            catch (Exception ex)
            {
                string LogEntry = DateTime.Now + " " + ex.ToString();

                LogEntryWriter(LogEntry);
            }
        }

        //will search for different versions of applications
        //Primarily for .net
        private int PreReqSearch(string sDir, string PreReqName)
        {
            try
            {
                foreach (var directory in Directory.GetDirectories(sDir))
                {
                    foreach (var filename in Directory.GetFiles(directory))
                    {
                        if (Path.GetFileName(filename) == PreReqName)
                        {
                            //if the file name of the found file within the folders/subdirs matches the one file we are searching for
                            /// the flag is set to true and we exit this level of recursion
                            flag = true;
                            break;
                        }
                    }
                    //due to the multi leveled recursion we must check to see if the bool flag has the value of true (or doesn't have the value of false)
                    //IF the flag is false then the recursion is called again to check the subdir that returned false
                    //if the flag is set to not false then the recursion escapes to the next level
                    if (flag == false)
                    {
                        PreReqSearch(directory, PreReqName);
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                string LogEntry = DateTime.Now + " " + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            //this code block is so that a band aide escape of the multi leveled recursion
            //if the bool has a value we want to maintain that value.
            //   False = 0 and not false = 1
            if (flag == false)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        //this searches through a user entered directory/subdirectories for pre reqs
        private void PreReqSearchCopy(string sDir)
        {
            try
            {
                foreach (var directory in Directory.GetDirectories(sDir))
                {
                    foreach (var filename in Directory.GetFiles(directory))
                    {
                        MobileCopy(filename);
                    }

                    //this is so that a folder that has a subdirectory will also be searched
                    PreReqSearchCopy(directory);
                }
            }
            catch (Exception ex)
            {
                string LogEntry = DateTime.Now + " " + ex.ToString();

                LogEntryWriter(LogEntry);
            }
        }

        //---------------------Folder Related work-------------------

        //Temp file Creation, MobileInstaller Creation, Temp file cleaning on button click - Created on 02/01
        private void Temp()
        {
            //This was modified on 01/30/2018
            //This creates a temp folder if the folder does not exist.
            Directory.CreateDirectory(@"C:\Temp");

            //This was modified on 01/30/2018
            //This creates the mobile installer inside the temp
            Directory.CreateDirectory(LocalRun);
        }

        //Deletes folders by path and recursively deletes sub folders
        private void MobileDelete(string dir)
        {
            try
            {
                Directory.Delete(dir, true);

                string LogEntry = DateTime.Now + " " + dir + " has been deleted.";

                LogEntryWriter(LogEntry);
            }
            catch (IOException)
            {
                string LogEntry = DateTime.Now + " " + dir + " was not found and therefore could not deleted.";

                LogEntryWriter(LogEntry);
            }
            catch (Exception ex)
            {
                string LogEntry = DateTime.Now + " " + ex.ToString();

                LogEntryWriter(LogEntry);
            }
        }

        //This will delete all downloaded files from the MobileInstaller Folder in C:\Temp
        //this also removes the updater folder under C:\Programdata
        //this also outright removes the application folders under C:\Program files or C:\program files (x86)
        //CAUTION: THIS IS ONLY DONE WHEN UTILIZING TABPAGE1 RUN BUTTON - 03/13/2020
        private void MobileRestart()
        {
            StopService("NewWorldUpdaterService");
            Thread.Sleep(5000);
            try
            {
                BeginInvoke((Action)(() => ts.Text = "Deleting Programdata Updater"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

                MobileDelete(@"C:\Programdata\New World Systems\New World Updater");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + " " + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            try
            {
                BeginInvoke((Action)(() => ts.Text = "Deleting Fire Mobile Folder"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                if (Is64Bit.Checked == true)
                {
                    MobileDelete(@"C:\Program Files (x86)\New World Systems\Aegis Fire Mobile");
                }
                else
                {
                    MobileDelete(@"C:\Program Files\New World Systems\Aegis Fire Mobile");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + " " + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            try
            {
                BeginInvoke((Action)(() => ts.Text = "Deleting Police Mobile Folder"));
                if (Is64Bit.Checked == true)
                {
                    MobileDelete(@"C:\Program Files (x86)\New World Systems\Aegis Mobile");
                }
                else
                {
                    MobileDelete(@"C:\Program Files\New World Systems\Aegis Mobile");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + " " + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            try
            {
                BeginInvoke((Action)(() => ts.Text = "Deleting Pre Req Folder"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

                MobileDelete(@"C:\Temp\MobileInstaller");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + " " + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            StartService("NewWorldUpdaterService");

            BeginInvoke((Action)(() => ts.Text = "Shutting Down PC"));

            Process.Start("Shutdown", "/r");

            //Application.Exit();
        }

        //this will give the user role full control for folder permissions
        //this also will modify the all files and sub directories with full control to the user role
        private static bool SetAcl(string destinationDirectory)
        {
            FileSystemRights Rights = (FileSystemRights)0;
            Rights = FileSystemRights.FullControl;

            // *** Add Access Rule to the actual directory itself
            FileSystemAccessRule AccessRule = new FileSystemAccessRule("Users", Rights,
                                        InheritanceFlags.None,
                                        PropagationFlags.NoPropagateInherit,
                                        AccessControlType.Allow);

            DirectoryInfo Info = new DirectoryInfo(destinationDirectory);
            DirectorySecurity Security = Info.GetAccessControl(AccessControlSections.Access);

            Security.ModifyAccessRule(AccessControlModification.Set, AccessRule, out bool Result);

            if (!Result)
                return false;

            // *** Always allow objects to inherit on a directory
            InheritanceFlags iFlags = InheritanceFlags.ObjectInherit;
            iFlags = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;

            // *** Add Access rule for the inheritance
            AccessRule = new FileSystemAccessRule("Users", Rights,
                                        iFlags,
                                        PropagationFlags.InheritOnly,
                                        AccessControlType.Allow);
            Result = false;
            Security.ModifyAccessRule(AccessControlModification.Add, AccessRule, out Result);

            if (!Result)
                return false;

            Info.SetAccessControl(Security);

            return true;
        }

        //Program Interaction work

        //This is will run any/all programs that need user interaction by name
        private void RunProgram(string ProgramName, string Location)
        {
            Process proc = null;

            string batdir = string.Format(Location);
            proc = new Process();
            proc.StartInfo.WorkingDirectory = batdir;
            proc.StartInfo.FileName = ProgramName;
            //proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            string LogEntry1 = DateTime.Now + " " + ProgramName + " has been run";

            LogEntryWriter(LogEntry1);

            proc.Start();
            proc.WaitForExit();

            InstallChecker(ProgramName);

            if (proc.ExitCode == 0)
            {
                string LogEntry2 = DateTime.Now + " " + ProgramName + " was ran successfully.";

                LogEntryWriter(LogEntry2);
            }
            else
            {
                string errorcode = proc.ExitCode.ToString();
                string LogEntry2 = DateTime.Now + " " + ProgramName + " failed to run. Error code: " + errorcode;

                LogEntryWriter(LogEntry2);
            }
        }

        //This is the actual method to silently install pre-reqs by name
        private void InstallProgram(string PreReqName, string InstallLocation)
        {
            Process proc = null;

            string batdir = string.Format(InstallLocation);
            proc = new Process();
            proc.StartInfo.WorkingDirectory = batdir;
            proc.StartInfo.FileName = PreReqName;
            proc.StartInfo.Arguments = "/quiet";
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            string LogEntry1 = DateTime.Now + " attempting to install " + PreReqName;

            LogEntryWriter(LogEntry1);

            proc.Start();
            proc.WaitForExit();

            InstallChecker(PreReqName);

            if (proc.ExitCode == 0)
            {
                string LogEntry2 = DateTime.Now + " " + PreReqName + " has been installed successfully";

                LogEntryWriter(LogEntry2);
            }
            else if (proc.ExitCode == 3010)
            {
                string errorcode = proc.ExitCode.ToString();
                string LogEntry2 = DateTime.Now + " " + PreReqName + " has exited with code: " + errorcode +
                    " which means the machine needs to restart to finish the install process. You will need to restart the install process after the restart.";

                LogEntryWriter(LogEntry2);
            }
            else
            {
                string errorcode = proc.ExitCode.ToString();
                string LogEntry2 = DateTime.Now + " " + PreReqName + " failed to install. Error code: " + errorcode;

                LogEntryWriter(LogEntry2);
            }
        }

        //this will open files using whatever is the default program associated with the file type.
        //IF there is not a default file processor associated it will display a windows prompt to open the file type
        private void OpenProgram(string Location)
        {
            try
            {
                string LogEntry1 = DateTime.Now + " accessing file at " + Location; ;

                LogEntryWriter(LogEntry1);

                Process.Start("notepad.exe", Location);
            }
            catch (Exception ex)
            {
                string LogEntry = DateTime.Now + " " + ex.ToString();

                LogEntryWriter(LogEntry);
            }
        }

        //This is the method that will silently uninstall pre - reqs by name
        private bool UninstallProgram(string ProgramName)
        {
            ManagementObjectSearcher mos = new ManagementObjectSearcher(
                  @"SELECT * FROM Win32_Product WHERE Name = '" + ProgramName + "'");
            foreach (ManagementObject mo in mos.Get())
            {
                try
                {
                    if (mo["Name"].ToString() == ProgramName)
                    {
                        object hr = mo.InvokeMethod("Uninstall", null);

                        // //not pretty but fixes the invalid cast exception :/
                        if (hr.Equals(hr))
                        {
                            string LogEntry1 = DateTime.Now + " " + ProgramName + " has been uninstalled";

                            LogEntryWriter(LogEntry1);

                            UninstallChecker(ProgramName);
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);

                    string LogEntry2 = DateTime.Now + " " + ex.ToString();

                    LogEntryWriter(LogEntry2);
                }
            }

            string LogEntry3 = DateTime.Now + " " + ProgramName + " was not found. It was either not installed or detected.";

            LogEntryWriter(LogEntry3);

            UninstallChecker(ProgramName);

            //not found
            return false;
        }

        //----------------------XML related controls------------------------

        //XML Related information. Broken up between loading prior XML information OR creating a new XML with placeholder server location.
        private void InitialLoadofXML()
        {
            //Checking if the PreReqAppSettings.xml exists, and loading the data if it does.
            if (File.Exists("NWPSInstallApp.xml"))
            {
                StartupSettings.Load("NWPSInstallApp.xml");

                NwsHoldPath.Text = StartupSettings.GetElementsByTagName("SourcePath")[0].InnerText;

                GenerateNumber.Text = StartupSettings.GetElementsByTagName("GenerateNumber")[0].InnerText;

                MobileServer.Text = StartupSettings.GetElementsByTagName("MobileServerName")[0].InnerText;

                MSPServerPath.Text = StartupSettings.GetElementsByTagName("MSPServerPath")[0].InnerText;

                if (StartupSettings.GetElementsByTagName("PC")[0].InnerText.Equals("True"))
                {
                    PoliceClient.Checked = true;
                }

                if (StartupSettings.GetElementsByTagName("FC")[0].InnerText.Equals("True"))
                {
                    FireClient.Checked = true;
                }

                if (StartupSettings.GetElementsByTagName("MC")[0].InnerText.Equals("True"))
                {
                    MergeClient.Checked = true;
                }

                TargetPath = @"C:\Temp\MobileInstaller";

                string LogEntry = DateTime.Now + " Prior Settings Loaded";

                LogEntryWriter(LogEntry);
            }
            //Creation of a new PreReqAppSettings.xml if one does not already exist.
            else
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = ("   "),
                    CloseOutput = true,
                    OmitXmlDeclaration = true
                };
                using (XmlWriter writer = XmlWriter.Create("NWPSInstallApp.xml", settings))
                {
                    writer.WriteStartElement("root");
                    writer.WriteElementString("SourcePath", @"\\MobileServerName\C$\");
                    writer.WriteElementString("MobileServerName", "Mobile Server Name");
                    writer.WriteElementString("MSPServerPath", @"\\MSPServerName\ ");
                    writer.WriteElementString("GenerateNumber", "0");
                    writer.WriteElementString("PC", "false");
                    writer.WriteElementString("FC", "False");
                    writer.WriteElementString("MC", "False");
                    writer.WriteEndElement();
                    writer.Flush();
                    writer.Close();
                }
                string LogEntry = DateTime.Now + " a new Pre Req App Settings XML was created.";

                LogEntryWriter(LogEntry);
            }
        }

        //When the XML is modified once it is changed for all other uses with that XML.
        private void SaveStartupSettings()
        {
            StartupSettings.Load("NWPSInstallApp.xml");

            StartupSettings.GetElementsByTagName("SourcePath")[0].InnerText = NwsHoldPath.Text;

            StartupSettings.GetElementsByTagName("MSPServerPath")[0].InnerText = MSPServerPath.Text;

            StartupSettings.GetElementsByTagName("GenerateNumber")[0].InnerText = GenerateNumber.Text;

            StartupSettings.GetElementsByTagName("MobileServerName")[0].InnerText = MobileServer.Text;

            if (PoliceClient.Checked == true)
            {
                StartupSettings.GetElementsByTagName("PC")[0].InnerText = "True";
            }
            else
            {
                StartupSettings.GetElementsByTagName("PC")[0].InnerText = "False";
            }

            if (FireClient.Checked == true)
            {
                StartupSettings.GetElementsByTagName("FC")[0].InnerText = "True";
            }
            else
            {
                StartupSettings.GetElementsByTagName("FC")[0].InnerText = "False";
            }

            if (MergeClient.Checked == true)
            {
                StartupSettings.GetElementsByTagName("MC")[0].InnerText = "True";
            }
            else
            {
                StartupSettings.GetElementsByTagName("MC")[0].InnerText = "False";
            }

            //Save the start up settings
            StartupSettings.Save("NWPSInstallApp.xml");

            string LogEntry = DateTime.Now + " App Setting XML Updated";

            LogEntryWriter(LogEntry);
        }

        //this will remove ORI entries from the Mobile Install App XML
        private void UpdateXMLORI()
        {
            string text = "NWPSInstallApp.xml";

            string[] Lines = File.ReadAllLines(text);
            IEnumerable<string> newLines = Lines.Where(line => !line.Contains(@"ORI"));
            File.WriteAllLines(text, newLines);

            string LogEntry = DateTime.Now + " ORI/s Added to XML.";

            LogEntryWriter(LogEntry);
        }

        //this will remove FDID entries from the Mobile Install App XML
        private void UpdateXMLFDID()
        {
            string text = "NWPSInstallApp.xml";

            string[] Lines = File.ReadAllLines(text);
            IEnumerable<string> newLines = Lines.Where(line => !line.Contains(@"FDID"));
            File.WriteAllLines(text, newLines);

            string LogEntry = DateTime.Now + " FDID/s added to XML";

            LogEntryWriter(LogEntry);
        }

        //this saves ORI entries to the XML to be used again
        private void CreateXMLORI(string ORI, string name)
        {
            XDocument xDocument = XDocument.Load("NWPSInstallApp.xml");

            var doc = xDocument.Root.Element("root");

            xDocument.Root.Add(new XElement(name, ORI));

            xDocument.Save("NWPSInstallApp.xml");
        }

        //this saves FDID entries to the XML to be used again
        private void CreateXMLFDID(string FDID, string name)
        {
            XDocument xDocument = XDocument.Load("NWPSInstallApp.xml");

            var doc = xDocument.Root.Element("root");

            xDocument.Root.Add(new XElement(name, FDID));

            xDocument.Save("NWPSInstallApp.xml");
        }

        //will load old/prior ORI config in XML
        //this is will use the text from the XML file that corresponds to the ORI text fields in the application
        private void LoadORIXML()
        {
            StartupSettings.Load("NWPSInstallApp.xml");

            foreach (Control c in tabPage3.Controls)
            {
                if (c.Name.Contains("ORI"))
                {
                    if (StartupSettings.GetElementsByTagName(c.Name).Count > 0)
                    {
                        try
                        {
                            c.Text = StartupSettings.GetElementsByTagName(c.Name)[0].InnerText;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace.ToString());

                            string LogEntry1 = DateTime.Now + " " + ex.ToString();

                            LogEntryWriter(LogEntry1);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            string LogEntry = DateTime.Now + " ORIs Loaded from XML.";

            LogEntryWriter(LogEntry);
        }

        //will load old/prior FDID config in XML
        //this is will use the text from the XML file that corresponds to the FDID text fields in the application
        private void LoadFDIDXML()
        {
            StartupSettings.Load("NWPSInstallApp.xml");

            foreach (Control c in tabPage3.Controls)
            {
                if (c.Name.Contains("FDID"))
                {
                    if (StartupSettings.GetElementsByTagName(c.Name).Count > 0)
                    {
                        try
                        {
                            c.Text = StartupSettings.GetElementsByTagName(c.Name)[0].InnerText;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace.ToString());

                            string LogEntry1 = DateTime.Now + " " + ex.ToString();

                            LogEntryWriter(LogEntry1);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            string LogEntry = DateTime.Now + " FDIDs loaded from XML.";

            LogEntryWriter(LogEntry);
        }

        //--------------------------Background Worker code------------------

        //What to do when the Background worker is completed
        private void Tab1bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BeginInvoke((Action)(() => ts.Text = "Files Copied Locally"));
            BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));

            string LogEntry1 = DateTime.Now + " File Copy Finished";

            LogEntryWriter(LogEntry1);
        }

        //What to do when progress is made
        private void Tab1bg_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBar.Value += 1;
        }

        //--------------------The actual work done----------------------

        //Download background worker
        private void Tab1bg_DoWork(object sender, DoWorkEventArgs e)
        {
            BeginInvoke((Action)(() => CopyButton.Visible = false));

            //pre req download logic
            if (Directory.Exists(MSPServerPath.Text + @"\_Client-Installation"))
            {
                try
                {
                    PreReqRename("SSCERuntime_x64-ENU.exe", SQLCE4064, "SQL Compact Edition 4.0");
                    PreReqRename("SSCERuntime_x86-ENU.exe", SQLCE4032, "SQL Compact Edition 4.0");
                }
                catch (Exception ex)
                {
                    string LogEntry = DateTime.Now + " " + ex.ToString();

                    LogEntryWriter(LogEntry);
                }

                try
                {
                    PreReqSearchCopy(MSPServerPath.Text + @"\_Client-Installation\");
                }
                catch (Exception ex)
                {
                    string LogEntry = DateTime.Now + " " + ex.ToString();

                    LogEntryWriter(LogEntry);
                }
            }
            //nwps addon download and check
            else if (Directory.Exists(MSPServerPath.Text + @"\\DeviceTester\"))
            {
                MobileCopy1(MSPServerPath.Text);
            }
            //flash drive download
            else
            {
                MobileCopy(MSPServerPath.Text);
            }

            Tab1bg.ReportProgress(0);

            BeginInvoke((Action)(() => CopyButton.Visible = true));
        }

        //install/uninstall/combo on tab 1 background worker
        private void Tab3bg_DoWork(object sender, DoWorkEventArgs e)
        {
            BeginInvoke((Action)(() => ProgressBar.Visible = false));
            BeginInvoke((Action)(() => ProgressBar.Enabled = false));
            BeginInvoke((Action)(() => Run.Visible = false));

            SetAcl(MSPServerPath.Text + @"\\_Client-Installation\");

            string LogEntry9 = DateTime.Now + " " + MSPServerPath.Text + @"\\_Client-Installation\";

            LogEntryWriter(LogEntry9);

            //run combination mobile uninstall an mobile install
            if (Combo.Checked && Is64Bit.Checked == true)
            {
                //an exception thrown if the generate number text box is 0
                //exceptions also thrown for null and non-null/not numerical entries
                if (GenerateNumber.Text == "0")
                {
                    BeginInvoke((Action)(() => ts.Text = "Please Verify the Updater portion is configured"));
                    throw new ArgumentException(@"ERROR: Updater Configuration section of the utility is not configured, please fill out the tab and try again.");
                }
                else if (GenerateNumber.Text == " ")
                {
                    BeginInvoke((Action)(() => ts.Text = "Please Verify the Updater portion is configured"));
                    throw new ArgumentException(@"ERROR: Updater Configuration section of the utility is not configured, please fill out the tab and try again.");
                }
                else if (GenerateNumber.Text == "")
                {
                    BeginInvoke((Action)(() => ts.Text = "Please Verify the Updater portion is configured"));
                    throw new ArgumentException(@"ERROR: Updater Configuration section of the utility is not configured, please fill out the tab and try again.");
                }

                BeginInvoke((Action)(() => ts.Text = "Modifying Mobile Updater Entries"));
                FileWork64Bit();
                UpdaterWork64Bit();

                string LogEntry1 = DateTime.Now + " 64Bit Mobile Updater Entry Removal Completed";

                LogEntryWriter(LogEntry1);

                string LogEntry2 = DateTime.Now + " 64Bit Mobile Uninstall Initiated";

                LogEntryWriter(LogEntry2);

                BeginInvoke((Action)(() => ts.Text = "Uninstalling Mobile"));
                Mobile64Uninstall();

                string LogEntry3 = DateTime.Now + " 64Bit Mobile Uninstall Completed";

                LogEntryWriter(LogEntry3);

                string LogEntry4 = DateTime.Now + " 64Bit Mobile Install Initiated";

                LogEntryWriter(LogEntry4);

                BeginInvoke((Action)(() => ts.Text = "Installing Mobile"));
                Mobile64install();

                string LogEntry5 = DateTime.Now + " 64Bit Mobile Pre Req Install Completed";

                LogEntryWriter(LogEntry5);

                MessageBox.Show("Mobile Pre Reqs have been Installed");

                BeginInvoke((Action)(() => ts.Text = "Restarting PC"));
                MobileRestart();
            }

            //run 64bit installer
            if (Is64Bit.Checked && InstallMobile.Checked == true)
            {
                //this accounts for the GenerateNumber Text box being blank/Null
                if (GenerateNumber.Text == "0")
                {
                    BeginInvoke((Action)(() => ts.Text = "Please Verify the Updater portion is configured"));
                    throw new ArgumentException(@"ERROR: Updater Configuration section of the utility is not configured, please fill out the tab and try again.");
                }
                else if (GenerateNumber.Text == " ")
                {
                    BeginInvoke((Action)(() => ts.Text = "Please Verify the Updater portion is configured"));
                    throw new ArgumentException(@"ERROR: Updater Configuration section of the utility is not configured, please fill out the tab and try again.");
                }
                else if (GenerateNumber.Text == "")
                {
                    BeginInvoke((Action)(() => ts.Text = "Please Verify the Updater portion is configured"));
                    throw new ArgumentException(@"ERROR: Updater Configuration section of the utility is not configured, please fill out the tab and try again.");
                }

                string LogEntry1 = DateTime.Now + " 64Bit Mobile Install Initiated";

                LogEntryWriter(LogEntry1);

                BeginInvoke((Action)(() => ts.Text = "Going to Install mobile"));
                Mobile64install();

                string LogEntry2 = DateTime.Now + " 64Bit Mobile Pre Req Install Completed";

                LogEntryWriter(LogEntry2);

                BeginInvoke((Action)(() => ts.Text = "Restarting PC"));
                MobileRestart();
            }

            //Run 64bit uninstaller
            if (Is64Bit.Checked && UninstallMobile.Checked == true)
            {
                BeginInvoke((Action)(() => ts.Text = "Modifying Mobile Updater Entries"));
                FileWork64Bit();
                UpdaterWork64Bit();

                string LogEntry1 = DateTime.Now + " 64Bit Mobile Updater Entry Removal Completed";

                LogEntryWriter(LogEntry1);

                string LogEntry2 = DateTime.Now + " 64Bit Mobile Uninstall Initiated";

                LogEntryWriter(LogEntry2);

                BeginInvoke((Action)(() => ts.Text = "Going to Uninstall mobile"));
                Mobile64Uninstall();

                string LogEntry3 = DateTime.Now + " 64Bit Mobile Uninstall Completed";

                LogEntryWriter(LogEntry3);

                BeginInvoke((Action)(() => ts.Text = "Restarting PC"));
                MobileRestart();
            }

            //Run combination mobile uninstall and mobile install
            if (Combo.Checked && Is32bit.Checked == true)
            {
                if (GenerateNumber.Text == "0")
                {
                    BeginInvoke((Action)(() => ts.Text = "Please Verify the Updater portion is configured"));
                    throw new ArgumentException(@"ERROR: Updater Configuration section of the utility is not configured, please fill out the tab and try again.");
                }
                else if (GenerateNumber.Text == " ")
                {
                    BeginInvoke((Action)(() => ts.Text = "Please Verify the Updater portion is configured"));
                    throw new ArgumentException(@"ERROR: Updater Configuration section of the utility is not configured, please fill out the tab and try again.");
                }
                else if (GenerateNumber.Text == "")
                {
                    BeginInvoke((Action)(() => ts.Text = "Please Verify the Updater portion is configured"));
                    throw new ArgumentException(@"ERROR: Updater Configuration section of the utility is not configured, please fill out the tab and try again.");
                }

                BeginInvoke((Action)(() => ts.Text = "Modifying Mobile Updater Entries"));
                FileWork32Bit();
                UpdaterWork32Bit();

                string LogEntry1 = DateTime.Now + " 32Bit Mobile Updater Entries removal completed";

                LogEntryWriter(LogEntry1);

                string LogEntry2 = DateTime.Now + " 32Bit Mobile Uninstall Initiated";

                LogEntryWriter(LogEntry2);

                BeginInvoke((Action)(() => ts.Text = "Uninstalling Mobile"));
                Mobile32Uninstaller();

                string LogEntry3 = DateTime.Now + " 32Bit Mobile Successfully uninstalled";

                LogEntryWriter(LogEntry3);

                string LogEntry4 = DateTime.Now + " 32Bit Mobile Install Initiated";

                LogEntryWriter(LogEntry4);

                BeginInvoke((Action)(() => ts.Text = "Going to Install mobile"));
                Mobile32install();

                string LogEntry5 = DateTime.Now + " 32Bit Mobile Successfully Installed";

                LogEntryWriter(LogEntry5);

                BeginInvoke((Action)(() => ts.Text = "Restarting PC"));
                MobileRestart();
            }

            //Run 32bit installer
            if (Is32bit.Checked && InstallMobile.Checked == true)
            {
                if (GenerateNumber.Text == "0")
                {
                    BeginInvoke((Action)(() => ts.Text = "Please Verify the Updater portion is configured"));
                    throw new ArgumentException(@"ERROR: Updater Configuration section of the utility is not configured, please fill out the tab and try again.");
                }
                else if (GenerateNumber.Text == " ")
                {
                    BeginInvoke((Action)(() => ts.Text = "Please Verify the Updater portion is configured"));
                    throw new ArgumentException(@"ERROR: Updater Configuration section of the utility is not configured, please fill out the tab and try again.");
                }
                else if (GenerateNumber.Text == "")
                {
                    BeginInvoke((Action)(() => ts.Text = "Please Verify the Updater portion is configured"));
                    throw new ArgumentException(@"ERROR: Updater Configuration section of the utility is not configured, please fill out the tab and try again.");
                }

                string LogEntry2 = DateTime.Now + " 32Bit Mobile Install Initiated";

                LogEntryWriter(LogEntry2);

                BeginInvoke((Action)(() => ts.Text = "installing Mobile"));
                Mobile32install();

                string LogEntry3 = DateTime.Now + " 32Bit Mobile Successfully Installed";

                LogEntryWriter(LogEntry3);

                BeginInvoke((Action)(() => ts.Text = "Restarting PC"));
                MobileRestart();
            }

            //Run 32bit uninstaller
            if (Is32bit.Checked && UninstallMobile.Checked == true)
            {
                BeginInvoke((Action)(() => ts.Text = "Modifying Updater Files"));
                FileWork32Bit();
                UpdaterWork32Bit();

                string LogEntry1 = DateTime.Now + " 32 Bit Mobile Updater Entries removal completed";

                LogEntryWriter(LogEntry1);

                string LogEntry2 = DateTime.Now + " 32Bit Mobile Uninstall Initiated";

                LogEntryWriter(LogEntry2);

                BeginInvoke((Action)(() => ts.Text = "Uninstalling Mobile"));
                Mobile32Uninstaller();

                string LogEntry3 = DateTime.Now + " 32Bit Mobile Successfully uninstalled";

                LogEntryWriter(LogEntry3);

                BeginInvoke((Action)(() => ts.Text = "Restarting PC"));
                MobileRestart();
            }

            BeginInvoke((Action)(() => Run.Visible = true));
        }

        //Itemized Install/Uninstall/Triage Background worker
        private void Tab2bg_DoWork(object sender, DoWorkEventArgs e)
        {
            BeginInvoke((Action)(() => CustomRun.Visible = false));

            CustomUninstallRun();

            CustomInstallRun();

            MobileTriageRun();

            BeginInvoke((Action)(() => CustomRun.Visible = true));
        }

        //-------------------Local service work--------------------------------

        //will stop the service by name
        //currently only used for the Updater Service
        private void StopService(string name)
        {
            try
            {
                ServiceController sc = new ServiceController(name);
                if (sc.Status.Equals(ServiceControllerStatus.Running))
                {
                    sc.Stop();

                    string LogEntry = DateTime.Now + " " + name + " has been stopped.";

                    LogEntryWriter(LogEntry);

                    BeginInvoke((Action)(() => ts.Text = name + "Service Stopped"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
                }
                else
                {
                    BeginInvoke((Action)(() => ts.Text = name + " Service had an issue stopping"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.OrangeRed));

                    string LogEntry = DateTime.Now + " " + name + " Service is not currently Started. Cannot stop a stopped Service";

                    LogEntryWriter(LogEntry);
                }
            }
            catch (Exception ex)
            {
                BeginInvoke((Action)(() => ts.Text = name + " Service had an issue stopping"));

                string LogEntry = DateTime.Now + " " + name + " Could not be stopped. It likely is not installed, " +
                    "or could not be stopped since the Program was not run as an admin OR under an admin account. Exception: "
                    + ex.StackTrace.ToString();

                LogEntryWriter(LogEntry);
            }
        }

        //will start the service by name
        //currently only used for the Updater Service
        private void StartService(string name)
        {
            try
            {
                ServiceController sc = new ServiceController(name);
                if (sc.Status.Equals(ServiceControllerStatus.Stopped))
                {
                    sc.Start();

                    string LogEntry = DateTime.Now + " " + name + " has been started.";

                    LogEntryWriter(LogEntry);

                    BeginInvoke((Action)(() => ts.Text = name + " Service Started"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
                }
                else
                {
                    BeginInvoke((Action)(() => ts.Text = name + " Service had an issue starting"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.OrangeRed));

                    string LogEntry = DateTime.Now + " " + name + " Service is not currently stopped. Cannot Start a started Service";

                    LogEntryWriter(LogEntry);
                }
            }
            catch (Exception ex)
            {
                string LogEntry1 = DateTime.Now + ex.ToString();

                LogEntryWriter(LogEntry1);

                BeginInvoke((Action)(() => ts.Text = name + " Service had an issue starting"));

                string LogEntry = DateTime.Now + " " + name + " Could not be started. It likely is not installed, " +
                    "or could not be stopped since the Program was not run as an admin OR under an admin account. Exception: "
                    + ex.StackTrace.ToString(); ;

                LogEntryWriter(LogEntry);
            }
        }

        //---------------------Tab2 Code------------------------

        //itemized uninstall steps
        private void CustomUninstallRun()
        {
            //uninstall fire mobile
            //will uninstall fire mobile for NWPS and NWS
            if (CustomUninstallOptions.GetItemCheckState(0) == CheckState.Checked)
            {
                BeginInvoke((Action)(() => ts.Text = "Checking to uninstall Fire Mobile"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                UninstallProgram("Aegis Fire Mobile");

                UninstallProgram("Fire Mobile");
                BeginInvoke((Action)(() => ts.Text = "Fire Mobile is Uninstalled"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));

                BeginInvoke((Action)(() => ts.Text = "Fire Mobile was Uninstalled/Check Complete."));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
            }

            //uninstall police mobile
            //will uninstall police mobile for NWPS and NWS
            if (CustomUninstallOptions.GetItemCheckState(1) == CheckState.Checked)
            {
                BeginInvoke((Action)(() => ts.Text = "Checking to uninstall Police Mobile"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                UninstallProgram("Aegis Mobile");

                UninstallProgram("Law Enforcement Mobile");
                BeginInvoke((Action)(() => ts.Text = "Police Mobile is Uninstalled"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));

                BeginInvoke((Action)(() => ts.Text = "Police Mobile was Uninstalled/Check Complete"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
            }

            //uninstall merge
            //will uninstall the merge client for NWPS and NWS
            if (CustomUninstallOptions.GetItemCheckState(2) == CheckState.Checked)
            {
                BeginInvoke((Action)(() => ts.Text = "Checking to uninstall Mobile Merge"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                UninstallProgram("Aegis Mobile Merge");

                UninstallProgram("Mobile Merge");
                BeginInvoke((Action)(() => ts.Text = "Mobile Merge is Uninstalled"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));

                BeginInvoke((Action)(() => ts.Text = "Merge Client was Uninstalled/Check Complete"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
            }

            //uninstall novaPDF
            if (CustomUninstallOptions.GetItemCheckState(3) == CheckState.Checked)
            {
                BeginInvoke((Action)(() => ts.Text = "Uninstalling Nova PDF"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

                UninstallProgram("NWPS Enterprise Mobile PDF Printer");

                UninstallProgram("novaPDF 8 Printer Driver");

                UninstallProgram("novaPDF 8 SDK COM (x86)");

                UninstallProgram("novaPDF 8 SDK COM (x64)");

                BeginInvoke((Action)(() => ts.Text = "NOVA PDF was Uninstalled/Check Completed"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
            }

            //uninstall GIS
            if (CustomUninstallOptions.GetItemCheckState(4) == CheckState.Checked)
            {
                BeginInvoke((Action)(() => ts.Text = "Uninstalling GIS - Old"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                UninstallProgram("New World GIS Components");

                BeginInvoke((Action)(() => ts.Text = "Uninstalling GIS - New"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                if (label30.Text != "Uninstalled")
                {
                    UninstallProgram("New World GIS Components x64");
                }
                else
                {
                    string Logentry1 = DateTime.Now + " New World GIS Components x64 is already uninstalled. This step was skipped.";
                    LogEntryWriter(Logentry1);
                }

                if (label29.Text != "Uninstalled")
                {
                    UninstallProgram("New World GIS Components x86");
                }
                else
                {
                    string Logentry2 = DateTime.Now + " New World GIS Components x86 is already Uninstalled. This step as skipped.";
                    LogEntryWriter(Logentry2);
                }

                BeginInvoke((Action)(() => ts.Text = "GIS Components are Uninstalled/Check Complete"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
            }

            //Uninstall SQL Compact
            if (CustomUninstallOptions.GetItemCheckState(5) == CheckState.Checked)
            {
                BeginInvoke((Action)(() => ts.Text = "Uninstalling SQL Server Compact 3.5 SP2"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                if (label34.Text != "Uninstalled")
                {
                    UninstallProgram("Microsoft SQL Server Compact 3.5 SP2 x64 ENU");
                }
                else
                {
                    string Logentry1 = DateTime.Now + "64 bit SQL Compact 3.5 is already uninstalled. This step was skipped.";
                    LogEntryWriter(Logentry1);
                }

                if (label33.Text != "Uninstalled")
                {
                    UninstallProgram("Microsoft SQL Server Compact 3.5 SP2 ENU");
                }
                else
                {
                    string Logentry2 = DateTime.Now + " 32bit SQL Compact 3.5 is already uninstalled. This step was skipped.";
                    LogEntryWriter(Logentry2);
                }

                BeginInvoke((Action)(() => ts.Text = "SQL Server Compact 3.5 SP2 is Uninstalled"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
            }

            //Uninstall Updater client
            if (CustomUninstallOptions.GetItemCheckState(6) == CheckState.Checked)
            {
                BeginInvoke((Action)(() => ts.Text = "Uninstalling New World Automatic Updater"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

                if (label27.Text != "Uninstalled")
                {
                    UninstallProgram("New World Automatic Updater");
                }
                else
                {
                    string Logentry1 = DateTime.Now + " New World Automatic Updater is already uninstalled. This step was skipped.";
                    LogEntryWriter(Logentry1);
                }

                BeginInvoke((Action)(() => ts.Text = "Updater was Uninstalled/Check Complete"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
            }

            //delete client folders
            if (CustomUninstallOptions.GetItemCheckState(7) == CheckState.Checked)
            {
                StopService("NewWorldUpdaterService");

                Thread.Sleep(5000);

                //delete programdata updater
                try
                {
                    BeginInvoke((Action)(() => ts.Text = "Deleting Programdata Updater"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

                    MobileDelete(@"C:\Programdata\New World Systems\New World Updater");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());

                    string LogEntry = DateTime.Now + " " + ex.ToString();

                    LogEntryWriter(LogEntry);
                }

                //delete fire mobile folder
                try
                {
                    BeginInvoke((Action)(() => ts.Text = "Deleting Fire Mobile Folder"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                    if (Is64Bit.Checked == true)
                    {
                        MobileDelete(@"C:\Program Files (x86)\New World Systems\Aegis Fire Mobile");
                    }
                    else
                    {
                        MobileDelete(@"C:\Program Files\New World Systems\Aegis Fire Mobile");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());

                    string LogEntry = DateTime.Now + " " + ex.ToString();

                    LogEntryWriter(LogEntry);
                }

                //delete police mobile folder
                try
                {
                    BeginInvoke((Action)(() => ts.Text = "Deleting Police Mobile Folder"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                    if (Is64Bit.Checked == true)
                    {
                        MobileDelete(@"C:\Program Files (x86)\New World Systems\Aegis Mobile");
                    }
                    else
                    {
                        MobileDelete(@"C:\Program Files\New World Systems\Aegis Mobile");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());

                    string LogEntry = DateTime.Now + " " + ex.ToString();

                    LogEntryWriter(LogEntry);
                }

                StartService("NewWorldUpdaterService");
            }

            //remove mobile related updater entries
            if (CustomUninstallOptions.GetItemCheckState(8) == CheckState.Checked)
            {
                if (Is64Bit.Checked == true)
                {
                    BeginInvoke((Action)(() => ts.Text = "Modifying Mobile Updater Entries"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                    FileWork64Bit();
                    UpdaterWork64Bit();
                    BeginInvoke((Action)(() => ts.Text = "Mobile Updater Entries are removed"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                }
                else
                {
                    BeginInvoke((Action)(() => ts.Text = "Modifying Mobile Updater Entries"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                    FileWork32Bit();
                    UpdaterWork32Bit();
                    BeginInvoke((Action)(() => ts.Text = "Mobile Updater Entries are removed"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                }

                BeginInvoke((Action)(() => ts.Text = "Uninstall Complete"));
            }

            //Uninstall MSP or CAD for NWPS and NWS
            if (CustomUninstallOptions.GetItemCheckState(9) == CheckState.Checked)
            {
                //64 bit
                if (Is64Bit.Checked == true)
                {
                    string title = "MSP uninstall Dialog";
                    string message = "would you like to uninstall MSP";
                    MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                    DialogResult result = MessageBox.Show(message, title, buttons);

                    //if some wants to uninstall MSP first
                    if (result == DialogResult.Yes)
                    {
                        BeginInvoke((Action)(() => ts.Text = "uninstalling MSP"));
                        BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

                        UninstallProgram("New World MSP Client");

                        UninstallProgram("New World Aegis Client");

                        UninstallProgram("New World Aegis MSP Client");

                        BeginInvoke((Action)(() => ts.Text = "MSP has been uninstalled"));
                        BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));

                        //if someone wants to uninstall CAD second
                        string title1 = "Cad uninstall Dialog";
                        string message1 = "would you like to uninstall CAD";
                        MessageBoxButtons buttons1 = MessageBoxButtons.YesNo;
                        DialogResult result1 = MessageBox.Show(message1, title1, buttons1);
                        if (result1 == DialogResult.Yes)
                        {
                            BeginInvoke((Action)(() => ts.Text = "uninstalling CAD"));
                            BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

                            UninstallProgram("New World Enterprise CAD Client");

                            BeginInvoke((Action)(() => ts.Text = "CAD has been uninstalled"));
                            BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
                        }
                    }

                    //if someone wants to uninstall CAD but not MSP
                    else if (result == DialogResult.No)
                    {
                        string title1 = "Cad uninstall Dialog";
                        string message1 = "would you like to uninstall CAD";
                        MessageBoxButtons buttons1 = MessageBoxButtons.YesNo;
                        DialogResult result1 = MessageBox.Show(message1, title1, buttons1);
                        if (result1 == DialogResult.Yes)
                        {
                            BeginInvoke((Action)(() => ts.Text = "uninstalling CAD"));
                            BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

                            UninstallProgram("New World  Enterprise CAD Client");

                            BeginInvoke((Action)(() => ts.Text = "CAD has been uninstalled"));
                            BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
                        }
                    }
                }

                //32 bit
                else
                {
                    //if someone wants to uninstall MSP first
                    string title = "MSP uninstall Dialog";
                    string message = "would you like to uninstall MSP";
                    MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                    DialogResult result = MessageBox.Show(message, title, buttons);
                    if (result == DialogResult.Yes)
                    {
                        BeginInvoke((Action)(() => ts.Text = "uninstalling MSP"));
                        BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

                        UninstallProgram("New World MSP Client");

                        UninstallProgram("New World Aegis Client");

                        UninstallProgram("New World Aegis MSP Client");

                        BeginInvoke((Action)(() => ts.Text = "MSP has been uninstalled"));
                        BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));

                        //if someone wants to uninstall CAD second
                        string title1 = "Cad uninstall Dialog";
                        string message1 = "would you like to uninstall CAD";
                        MessageBoxButtons buttons1 = MessageBoxButtons.YesNo;
                        DialogResult result1 = MessageBox.Show(message1, title1, buttons1);
                        if (result1 == DialogResult.Yes)
                        {
                            BeginInvoke((Action)(() => ts.Text = "uninstalling CAD"));
                            BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

                            UninstallProgram("New World Enterprise CAD Client");

                            BeginInvoke((Action)(() => ts.Text = "CAD has been uninstalled"));
                            BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
                        }
                    }

                    //if someone wants to uninstall CAD
                    else if (result == DialogResult.No)
                    {
                        string title1 = "Cad uninstall Dialog";
                        string message1 = "would you like to uninstall CAD";
                        MessageBoxButtons buttons1 = MessageBoxButtons.YesNo;
                        DialogResult result1 = MessageBox.Show(message1, title1, buttons1);
                        if (result1 == DialogResult.Yes)
                        {
                            BeginInvoke((Action)(() => ts.Text = "uninstalling CAD"));
                            BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

                            UninstallProgram("New World  Enterprise CAD Client");

                            BeginInvoke((Action)(() => ts.Text = "CAD has been uninstalled"));
                            BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
                        }
                    }
                }

                BeginInvoke((Action)(() => ts.Text = "MSP and/or CAD has been Uninstalled/Check Complete"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
            }

            //remove SQL compact 4.0
            if (CustomUninstallOptions.GetItemCheckState(10) == CheckState.Checked)
            {
                //64 bit
                if (Is64Bit.Checked == true)
                {
                    BeginInvoke((Action)(() => ts.Text = "uninstalling SQL Server Compact 4.0 x64"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

                    if (label28.Text != "Uninstalled")
                    {
                        UninstallProgram("Microsoft SQL Server Compact 4.0 x64 ENU");
                    }
                    else
                    {
                        string Logentry1 = DateTime.Now + " 64 bit SQL Compact 4.0 is already uninstalled. This step was skipped.";
                        LogEntryWriter(Logentry1);
                    }

                    BeginInvoke((Action)(() => ts.Text = "SQL Compact 4.0 was Uninstalled"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
                }

                //32 bit
                else
                {
                    BeginInvoke((Action)(() => ts.Text = "uninstalling SQL Server Compact 4.0 x86"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

                    if (label28.Text != "Uninstalled")
                    {
                        UninstallProgram("Microsoft SQL Server Compact 4.0 ENU");
                    }
                    else
                    {
                        string Logentry1 = DateTime.Now + "32bit SQL Compact 4.0 is already uninstalled. This step was skipped.";
                        LogEntryWriter(Logentry1);
                    }

                    BeginInvoke((Action)(() => ts.Text = "SQL Compact 4.0 was Uninstalled"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
                }
            }

            //uninstall scene pd and active x controller
            if (CustomUninstallOptions.GetItemCheckState(11) == CheckState.Checked)
            {
                BeginInvoke((Action)(() => ts.Text = "uninstalling ScenePD"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

                if (label35.Text != "Uninstalled")
                {
                    UninstallProgram("ScenePD 6 ActiveX Control");
                    UninstallProgram("ScenePD 6 Desktop Edition");
                }

                BeginInvoke((Action)(() => ts.Text = "ScenePD has been Uninstalled"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
            }

            //Uninstall CAD Incident Observer
            if (CustomUninstallOptions.GetItemCheckState(12) == CheckState.Checked)
            {
                BeginInvoke((Action)(() => ts.Text = "uninstalling CAD Incident Observer Client"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

                UninstallProgram("New World Enterprise CAD Incident Observer Client");

                BeginInvoke((Action)(() => ts.Text = "CAD Incident Observer has been Uninstalled"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
            }

            //Uninstall SQL CLR Types
            if (CustomUninstallOptions.GetItemCheckState(13) == CheckState.Checked)
            {
                BeginInvoke((Action)(() => ts.Text = "uninstalling SQL CLR Types"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

                //64 bit
                if (Is64Bit.Checked == true)
                {
                    if (label32.Text != "Uninstalled")
                    {
                        UninstallProgram("Microsoft SQL Server System CLR Types (x64)");
                    }
                    else
                    {
                        string Logentry1 = DateTime.Now + " 64 bit SQL CLR Types is already uninstalled. This step was skipped.";
                        LogEntryWriter(Logentry1);
                    }

                    if (label31.Text != "Uninstalled")
                    {
                        UninstallProgram("Microsoft SQL Server System CLR Types");
                    }
                    string Logentry2 = DateTime.Now + " 32 bit SQL CLR Types is already uninstalled. This step was skipped.";
                    LogEntryWriter(Logentry2);

                    BeginInvoke((Action)(() => ts.Text = "SQL CLR Types has been Uninstalled"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
                }
                else
                {
                    if (label31.Text != "Uninstalled")
                    {
                        UninstallProgram("Microsoft SQL Server System CLR Types");
                    }
                    string Logentry2 = DateTime.Now + " 32 bit SQL CLR Types is already uninstalled. This step was skipped.";
                    LogEntryWriter(Logentry2);

                    BeginInvoke((Action)(() => ts.Text = "SQL CLR Types has been Uninstalled"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
                }
            }

            //Restart Machine
            if (CustomUninstallOptions.GetItemCheckState(14) == CheckState.Checked)
            {
                BeginInvoke((Action)(() => ts.Text = "Shutting Down PC"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.OrangeRed));

                Process.Start("Shutdown", "/r");

                string LogEntry = DateTime.Now + @" Restart Initiated";

                LogEntryWriter(LogEntry);
            }
        }

        //itemized install steps
        private void CustomInstallRun()
        {
            //install .net
            if (CustomInstallOption.GetItemCheckState(0) == CheckState.Checked)
            {
                DotNet();
            }

            //install SQL Runtime
            if (CustomInstallOption.GetItemCheckState(1) == CheckState.Checked)
            {
                SQLCE35();

                BeginInvoke((Action)(() => ts.Text = "SQL Runtime installed"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
            }

            //install GIS components
            if (CustomInstallOption.GetItemCheckState(2) == CheckState.Checked)
            {
                GIS();

                BeginInvoke((Action)(() => ts.Text = "GIS Components Installed"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
            }

            //install DB Provider services
            if (CustomInstallOption.GetItemCheckState(3) == CheckState.Checked)
            {
                DBProviderService();

                BeginInvoke((Action)(() => ts.Text = "DB Provider Services Installed"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
            }

            //install Updater
            if (CustomInstallOption.GetItemCheckState(4) == CheckState.Checked)
            {
                BeginInvoke((Action)(() => ts.Text = "Installing Updater"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

                UpdaterInstaller();

                BeginInvoke((Action)(() => ts.Text = "Updater Installed"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
            }

            //Running Updater Config form
            if (CustomInstallOption.GetItemCheckState(5) == CheckState.Checked)
            {
                BeginInvoke((Action)(() => ts.Text = "Running Mobile Updater Config form"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                try
                {
                    if (GenerateNumber.Text == "0")
                    {
                        BeginInvoke((Action)(() => ts.Text = "Please Verify the Updater portion is configured"));
                        throw new ArgumentException(@"ERROR: Updater Configuration section of the utility is not configured, please fill out the tab and try again.");
                    }
                    else if (GenerateNumber.Text == " ")
                    {
                        BeginInvoke((Action)(() => ts.Text = "Please Verify the Updater portion is configured"));
                        throw new ArgumentException(@"ERROR: Updater Configuration section of the utility is not configured, please fill out the tab and try again.");
                    }
                    else if (GenerateNumber.Text == "")
                    {
                        BeginInvoke((Action)(() => ts.Text = "Please Verify the Updater portion is configured"));
                        throw new ArgumentException(@"ERROR: Updater Configuration section of the utility is not configured, please fill out the tab and try again.");
                    }
                    else
                    {
                        UpdaterAppend_Click(new object(), new EventArgs());
                        BeginInvoke((Action)(() => ts.Text = "ORI/FDID Update Complete"));
                        BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());

                    string LogEntry = DateTime.Now + " " + ex.ToString();

                    LogEntryWriter(LogEntry);
                }
            }

            //Setting Folder Permissions
            if (CustomInstallOption.GetItemCheckState(6) == CheckState.Checked)
            {
                if (Is64Bit.Checked == true)
                {
                    BeginInvoke((Action)(() => ts.Text = "Prepping folder permissions"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                    try
                    {
                        SetAcl(@"C:\Program Files (x86)\New World Systems");

                        string LogEntry = DateTime.Now + @" C:\Program Files (x86)\New World Systems has User permissions set.";

                        LogEntryWriter(LogEntry);

                        SetAcl(@"C:\ProgramData\New World Systems");

                        string LogEntry1 = DateTime.Now + @" C:\ProgramData\New World Systems has User permissions set.";

                        LogEntryWriter(LogEntry1);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace.ToString());

                        string LogEntry1 = DateTime.Now + " " + ex.ToString();

                        LogEntryWriter(LogEntry1);
                    }
                }
                else
                {
                    BeginInvoke((Action)(() => ts.Text = "Prepping folder permissions"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                    try
                    {
                        SetAcl(@"C:\Program Files\New World Systems");

                        string LogEntry = DateTime.Now + @" C:\Program Files\New World Systems has User permissions set.";

                        LogEntryWriter(LogEntry);

                        SetAcl(@"C:\ProgramData\New World Systems");

                        string LogEntry1 = DateTime.Now + " " + @" C:\ProgramData\New World Systems has User permissions set.";

                        LogEntryWriter(LogEntry1);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace.ToString());

                        string LogEntry = DateTime.Now + " " + ex.ToString();

                        LogEntryWriter(LogEntry);
                    }
                }

                BeginInvoke((Action)(() => ts.Text = "Folder Permissions are Set"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
            }

            //install MSP/CAD
            if (CustomInstallOption.GetItemCheckState(7) == CheckState.Checked)
            {
                //64 bit MSP and CAD
                if (Is64Bit.Checked == true)
                {
                    string title = "MSP Install Dialog";
                    string message = "would you like to Install MSP";
                    MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                    DialogResult result = MessageBox.Show(message, title, buttons);

                    //if msp install first
                    if (result == DialogResult.Yes)
                    {
                        DotNet();

                        SQLCE35();

                        GIS();

                        UpdaterInstaller();

                        MSP();

                        BeginInvoke((Action)(() => ts.Text = "Prepping folder permissions"));
                        BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                        try
                        {
                            SetAcl(@"C:\Program Files (x86)\New World Systems");

                            string LogEntry = DateTime.Now + @" C:\Program Files (x86)\New World Systems has User permissions set.";

                            LogEntryWriter(LogEntry);

                            SetAcl(@"C:\ProgramData\New World Systems");

                            string LogEntry1 = DateTime.Now + @" C:\ProgramData\New World Systems has User permissions set.";

                            LogEntryWriter(LogEntry1);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace.ToString());

                            string LogEntry = DateTime.Now + " " + ex.ToString();

                            LogEntryWriter(LogEntry);
                        }

                        string title1 = "Cad Install Dialog";
                        string message1 = "would you like to Install CAD";
                        MessageBoxButtons buttons1 = MessageBoxButtons.YesNo;
                        DialogResult result1 = MessageBox.Show(message1, title1, buttons1);

                        //if install CAD second
                        if (result1 == DialogResult.Yes)
                        {
                            DotNet();

                            SQLCE40();

                            GIS();

                            DBProviderService();

                            UpdaterInstaller();

                            VS2010();

                            SQLCLR2008();

                            CAD();

                            BeginInvoke((Action)(() => ts.Text = "Prepping folder permissions"));
                            BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                            try
                            {
                                SetAcl(@"C:\Program Files (x86)\New World Systems");

                                string LogEntry = DateTime.Now + @" C:\Program Files (x86)\New World Systems has User permissions set.";

                                LogEntryWriter(LogEntry);

                                SetAcl(@"C:\ProgramData\New World Systems");

                                string LogEntry1 = DateTime.Now + @" C:\ProgramData\New World Systems has User permissions set.";

                                LogEntryWriter(LogEntry1);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + " " + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }
                        }
                    }

                    //if cad install and not MSP
                    else if (result == DialogResult.No)
                    {
                        string title1 = "Cad Install Dialog";
                        string message1 = "would you like to Install CAD";
                        MessageBoxButtons buttons1 = MessageBoxButtons.YesNo;
                        DialogResult result1 = MessageBox.Show(message1, title1, buttons1);
                        if (result1 == DialogResult.Yes)
                        {
                            DotNet();

                            SQLCE40();

                            GIS();

                            DBProviderService();

                            UpdaterInstaller();

                            VS2010();

                            SQLCLR2008();

                            CAD();

                            BeginInvoke((Action)(() => ts.Text = "Prepping folder permissions"));
                            BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                            try
                            {
                                SetAcl(@"C:\Program Files (x86)\New World Systems");

                                string LogEntry = DateTime.Now + @" C:\Program Files (x86)\New World Systems has User permissions set.";

                                LogEntryWriter(LogEntry);

                                SetAcl(@"C:\ProgramData\New World Systems");

                                string LogEntry1 = DateTime.Now + @" C:\ProgramData\New World Systems has User permissions set.";

                                LogEntryWriter(LogEntry1);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + " " + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }
                        }
                    }
                }

                //32 bit MSP and CAD
                else
                {
                    string title = "MSP Install Dialog";
                    string message = "would you like to Install MSP";
                    MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                    DialogResult result = MessageBox.Show(message, title, buttons);

                    //if msp install first
                    if (result == DialogResult.Yes)
                    {
                        DotNet();

                        SQLCE35();

                        GIS();

                        UpdaterInstaller();

                        MSP();

                        BeginInvoke((Action)(() => ts.Text = "Prepping folder permissions"));
                        BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                        try
                        {
                            SetAcl(@"C:\Program Files\New World Systems");

                            string LogEntry = DateTime.Now + @" C:\Program Files\New World Systems has User permissions set.";

                            LogEntryWriter(LogEntry);
                            SetAcl(@"C:\ProgramData\New World Systems");

                            string LogEntry1 = DateTime.Now + @" C:\ProgramData\New World Systems has User permissions set.";

                            LogEntryWriter(LogEntry1);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace.ToString());

                            string LogEntry = DateTime.Now + " " + ex.ToString();

                            LogEntryWriter(LogEntry);
                        }

                        string title1 = "Cad Install Dialog";
                        string message1 = "would you like to Install CAD";
                        MessageBoxButtons buttons1 = MessageBoxButtons.YesNo;
                        DialogResult result1 = MessageBox.Show(message1, title1, buttons1);

                        //if install CAD second
                        if (result1 == DialogResult.Yes)
                        {
                            DotNet();

                            SQLCE40();

                            GIS();

                            DBProviderService();

                            UpdaterInstaller();

                            VS2010();

                            SQLCLR2008();

                            CAD();

                            BeginInvoke((Action)(() => ts.Text = "Prepping folder permissions"));
                            BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                            try
                            {
                                SetAcl(@"C:\Program Files\New World Systems");

                                string LogEntry = DateTime.Now + @" C:\Program Files\New World Systems has User permissions set.";

                                LogEntryWriter(LogEntry);
                                SetAcl(@"C:\ProgramData\New World Systems");

                                string LogEntry1 = DateTime.Now + @" C:\ProgramData\New World Systems has User permissions set.";

                                LogEntryWriter(LogEntry1);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + " " + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }
                        }
                    }

                    //if cad install and not MSP
                    else if (result == DialogResult.No)
                    {
                        string title1 = "Cad Install Dialog";
                        string message1 = "would you like to Install CAD";
                        MessageBoxButtons buttons1 = MessageBoxButtons.YesNo;
                        DialogResult result1 = MessageBox.Show(message1, title1, buttons1);
                        if (result1 == DialogResult.Yes)
                        {
                            DotNet();

                            SQLCE40();

                            GIS();

                            DBProviderService();

                            UpdaterInstaller();

                            VS2010();

                            SQLCLR2008();

                            CAD();

                            BeginInvoke((Action)(() => ts.Text = "Prepping folder permissions"));
                            BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                            try
                            {
                                SetAcl(@"C:\Program Files\New World Systems");

                                string LogEntry = DateTime.Now + @" C:\Program Files\New World Systems has User permissions set.";

                                LogEntryWriter(LogEntry);
                                SetAcl(@"C:\ProgramData\New World Systems");

                                string LogEntry1 = DateTime.Now + @" C:\ProgramData\New World Systems has User permissions set.";

                                LogEntryWriter(LogEntry1);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + " " + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }
                        }
                    }
                }
                BeginInvoke((Action)(() => ts.Text = "MSP and/OR CAD is installed"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
            }

            //Install Incident Observer
            if (CustomInstallOption.GetItemCheckState(8) == CheckState.Checked)
            {
                IncidentObserver();
            }

            //Install ScenePD
            //this will show a prompt to install scene pd
            if (CustomInstallOption.GetItemCheckState(9) == CheckState.Checked)
            {
                ScenePD();
            }

            //Install SQL Compact 4.0
            //this will show a prompt to install SQL Compact 4.0
            if (CustomInstallOption.GetItemCheckState(10) == CheckState.Checked)
            {
                SQLCE40();
            }

            //Install VS 2010
            //this will install VS 2010 tools
            if (CustomInstallOption.GetItemCheckState(11) == CheckState.Checked)
            {
                VS2010();
            }

            //Restart Machine
            if (CustomInstallOption.GetItemCheckState(12) == CheckState.Checked)
            {
                BeginInvoke((Action)(() => ts.Text = "Shutting Down PC"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.OrangeRed));

                Process.Start("Shutdown", "/r");

                string LogEntry = DateTime.Now + @" Restart Initiated";

                LogEntryWriter(LogEntry);
            }
        }

        //itemized typical triage steps
        private void MobileTriageRun()
        {
            //set folder permissions
            if (MobileTriage.GetItemCheckState(0) == CheckState.Checked)
            {
                //this will set the correct folder permissions for 64 bit or 32 bit depending on the OS version
                if (Is64Bit.Checked == true)
                {
                    BeginInvoke((Action)(() => ts.Text = "Prepping folder permissions"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                    try
                    {
                        SetAcl(@"C:\Program Files (x86)\New World Systems");

                        string LogEntry = DateTime.Now + @" C:\Program Files (x86)\New World Systems has User permissions set.";

                        LogEntryWriter(LogEntry);

                        SetAcl(@"C:\ProgramData\New World Systems");

                        string LogEntry1 = DateTime.Now + @" C:\ProgramData\New World Systems has User permissions set.";

                        LogEntryWriter(LogEntry1);
                    }
                    catch (Exception ex)
                    {
                        string LogEntry = DateTime.Now + " " + ex.ToString();

                        LogEntryWriter(LogEntry);
                    }
                }
                else
                {
                    BeginInvoke((Action)(() => ts.Text = "Prepping folder permissions"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                    try
                    {
                        SetAcl(@"C:\Program Files\New World Systems");

                        string LogEntry = DateTime.Now + @" C:\Program Files\New World Systems has User permissions set.";

                        LogEntryWriter(LogEntry);

                        SetAcl(@"C:\ProgramData\New World Systems");

                        string LogEntry1 = DateTime.Now + @" C:\ProgramData\New World Systems has User Permissions Set.";

                        LogEntryWriter(LogEntry1);
                    }
                    catch (Exception ex)
                    {
                        string LogEntry = DateTime.Now + " " + ex.ToString();

                        LogEntryWriter(LogEntry);
                    }
                }

                BeginInvoke((Action)(() => ts.Text = "User Permissions are set"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
            }

            //stop and restart the updater service
            if (MobileTriage.GetItemCheckState(1) == CheckState.Checked)
            {
                //will stop the New World Updater Service
                StopService("NewWorldUpdaterService");

                //will make the application be non-responsive for 5 seconds
                //this will allow the new world service to stop
                Thread.Sleep(5000);

                //this will start the new world service
                StartService("NewWorldUpdaterService");

                BeginInvoke((Action)(() => ts.Text = "Updated Service Bounced"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
            }

            //reset the updater folder under programdata
            if (MobileTriage.GetItemCheckState(2) == CheckState.Checked)
            {
                //this will stop the new world updater service
                StopService("NewWorldUpdaterService");

                //this will cause the application to become unresponsive for 5 seconds
                Thread.Sleep(5000);

                try
                {
                    BeginInvoke((Action)(() => ts.Text = "Deleting Programdata Updater"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                    if (Directory.Exists(@"C:\Programdata\New World Systems\New World Updater"))
                    {
                        //this will delete the new world updater folder under programdata
                        MobileDelete(@"C:\Programdata\New World Systems\New World Updater");
                    }
                    else
                    {
                        string LogEntry = DateTime.Now + " New World Updater Folder cannot be found and was not removed.";

                        LogEntryWriter(LogEntry);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());

                    string LogEntry = DateTime.Now + " " + ex.ToString();

                    LogEntryWriter(LogEntry);
                }

                try
                {
                    if (Directory.Exists(@"C:\Programdata\New World Systems\GIS\Prod"))
                    {
                        MobileDelete(@"C:\Programdata\New World Systems\GIS\Prod");
                    }
                    else if (Directory.Exists(@"C:\Programdata\New World Systems\GIS\Test"))
                    {
                        MobileDelete(@"C:\Programdata\New World Systems\GIS\Test");
                    }
                    else if (Directory.Exists(@"C:\Programdata\New World Systems\GIS\Prd"))
                    {
                        MobileDelete(@"C:\Programdata\New World Systems\GIS\Prd");
                    }
                    else if (Directory.Exists(@"C:\Programdata\New World Systems\GIS\Tst"))
                    {
                        MobileDelete(@"C:\Programdata\New World Systems\GIS\Tst");
                    }
                    else
                    {
                        string LogEntry = DateTime.Now + " the GIS instance folder was not found, make sure that the client have their GIS data installed.";

                        LogEntryWriter(LogEntry);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());

                    string LogEntry = DateTime.Now + " " + ex.ToString();

                    LogEntryWriter(LogEntry);
                }

                //this will start the new world updater service
                StartService("NewWorldUpdaterService");

                BeginInvoke((Action)(() => ts.Text = "Programdata updater folder and service bounced"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
            }

            //deletes the fire mobile, police mobile, and updater folder under programdata
            if (MobileTriage.GetItemCheckState(3) == CheckState.Checked)
            {
                //this will stop the new world updater service
                StopService("NewWorldUpdaterService");

                //this will cause the application to hang for 5 seconds
                Thread.Sleep(5000);

                try
                {
                    BeginInvoke((Action)(() => ts.Text = "Deleting Programdata Updater"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

                    //this will attempt to delete the new world updater folder under programdata
                    MobileDelete(@"C:\Programdata\New World Systems\New World Updater");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());

                    string LogEntry = DateTime.Now + " " + ex.ToString();

                    LogEntryWriter(LogEntry);
                }

                try
                {
                    BeginInvoke((Action)(() => ts.Text = "Deleting Programdata Aegis Mobile"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

                    //this will attempt to delete the new world updater folder under programdata
                    MobileDelete(@"C:\Programdata\New World Systems\Aegis Mobile");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());

                    string LogEntry = DateTime.Now + " " + ex.ToString();

                    LogEntryWriter(LogEntry);
                }

                try
                {
                    BeginInvoke((Action)(() => ts.Text = "Deleting Fire Mobile Folder"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                    if (Is64Bit.Checked == true)
                    {
                        //this will delete the aegis fire mobile folder for 64 bit OS
                        MobileDelete(@"C:\Program Files (x86)\New World Systems\Aegis Fire Mobile");
                    }
                    else
                    {
                        //this will delete the aegis fire mobile folder for 32 bit OS
                        MobileDelete(@"C:\Program Files\New World Systems\Aegis Fire Mobile");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());

                    string LogEntry = DateTime.Now + " " + ex.ToString();

                    LogEntryWriter(LogEntry);
                }

                try
                {
                    BeginInvoke((Action)(() => ts.Text = "Deleting Police Mobile Folder"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                    if (Is64Bit.Checked == true)
                    {
                        //this will try to delete the aegis mobile folder for 64 bit OS
                        MobileDelete(@"C:\Program Files (x86)\New World Systems\Aegis Mobile");
                    }
                    else
                    {
                        //this will try to delete the aegis mobile folder for 32 bit OS
                        MobileDelete(@"C:\Program Files\New World Systems\Aegis Mobile");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());

                    string LogEntry = DateTime.Now + " " + ex.ToString();

                    LogEntryWriter(LogEntry);
                }

                StartService("NewWorldUpdaterService");

                BeginInvoke((Action)(() => ts.Text = "Fire Mobile/Police Mobile & ProgramData-Updater/Mobile Folder Deleted"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
            }

            //this will open the host file on the local machine
            if (MobileTriage.GetItemCheckState(4) == CheckState.Checked)
            {
                BeginInvoke((Action)(() => ts.Text = "Opening Host File"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

                OpenProgram(@"C:\Windows\System32\drivers\etc\hosts");

                BeginInvoke((Action)(() => ts.Text = "Host File Modified/Opened"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));
            }

            //will check to see if the Mobile Client Interface tester is in the MobileInstaller/NWS Addons folder
            //If the files are not present and are not already downloaded, then a download prompt will appear.
            if (MobileTriage.GetItemCheckState(5) == CheckState.Checked)
            {
                //this will run the Mobile Client Interface Tester utility if it is present, and prompt a new window to download if not.
                BeginInvoke((Action)(() => ts.Text = "Checking to see if Utility is in the proper location"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                if (File.Exists(@"C:\Temp\MobileInstaller\NWS Addons\Mobile Interface Tester (Tickets, Dispatch, AVL)\AegisMobileClientInterfaceTester.exe"))
                {
                    BeginInvoke((Action)(() => ts.Text = "Running Mobile Client Interface tester Utility"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));

                    string LogEntry = DateTime.Now + @" Mobile Client Interface Tester Started";

                    LogEntryWriter(LogEntry);

                    RunProgram("AegisMobileClientInterfaceTester.exe", @"C:\Temp\MobileInstaller\NWS Addons\Mobile Interface Tester (Tickets, Dispatch, AVL)");
                }
                else
                {
                    BeginInvoke((Action)(() => ts.Text = "Mobile Client Interface tester was not found - File Path to NWS Addon Folder"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.OrangeRed));

                    string LogEntry = DateTime.Now + @" ERROR: COULD NOT LOCATE MOBILE CLIENT INTERFACE UTILITY. Attempting to download.";

                    LogEntryWriter(LogEntry);

                    string LogEntry1 = DateTime.Now + @" Addon Download Folder Displayed.";

                    LogEntryWriter(LogEntry1);

                    BeginInvoke((Action)(() => secondForm.Show()));
                }
            }

            //will check to see if the Device tester is in the MobileInstaller/NWS Addons folder
            //If the files are not present and are not already downloaded, then a download prompt will appear.
            if (MobileTriage.GetItemCheckState(6) == CheckState.Checked)
            {
                //this will run the Mobile Client Device Tester utility if it is present, and if not display a custom error message
                BeginInvoke((Action)(() => ts.Text = "Checking to see if Mobile Client Device Tester is in the proper location"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                if (File.Exists(@"C:\Temp\MobileInstaller\NWS Addons\DeviceTester\DeviceTester.exe"))
                {
                    BeginInvoke((Action)(() => ts.Text = "Running Device tester Utility"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

                    string LogEntry = DateTime.Now + " Device Tester Utility Started";

                    LogEntryWriter(LogEntry);

                    RunProgram("DeviceTester.exe", @"C:\Temp\MobileInstaller\NWS Addons\DeviceTester");
                }
                else
                {
                    BeginInvoke((Action)(() => ts.Text = "Mobile Client Device tester was not found - File Path to NWS Addon Folder"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.OrangeRed));

                    string LogEntry = DateTime.Now + @" ERROR: COULD NOT LOCATE DEVICE TESTER UTILITY. Attempting to download.";

                    string LogEntry1 = DateTime.Now + @" Addon Download Folder Displayed.";

                    LogEntryWriter(LogEntry1);

                    BeginInvoke((Action)(() => secondForm.Show()));
                }
            }

            //will check to see if the Mobile Client GPS Test Utility is in the MobileInstaller/NWS Addons folder
            //If the files are not present and are not already downloaded, then a download prompt will appear.
            if (MobileTriage.GetItemCheckState(7) == CheckState.Checked)
            {
                if (File.Exists(@"C:\Program Files (x86)\Mobile GPS Tester\MobileTools.GpsTester.exe"))
                {
                    BeginInvoke((Action)(() => ts.Text = "Running Mobile GPS Tester"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));

                    string LogEntry = DateTime.Now + " Device Tester Utility Started";

                    LogEntryWriter(LogEntry);

                    RunProgram("MobileTools.GpsTester.exe", @"C:\Program Files (x86)\Mobile GPS Tester\");
                }
                else if (File.Exists(@"C:\Temp\MobileInstaller\NWS Addons\MobileGpsTesterSetup.msi"))
                {
                    InstallProgram("MobileGpsTesterSetup.msi", @"C:\Temp\MobileInstaller\NWS Addons\");

                    BeginInvoke((Action)(() => ts.Text = "Mobile GPS Tester has been installed"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));

                    BeginInvoke((Action)(() => ts.Text = "Mobile GPS Tester is being Run"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.ForestGreen));

                    RunProgram("MobileTools.GpsTester.exe", @"C:\Program Files (x86)\Mobile GPS Tester\");
                }
                else
                {
                    BeginInvoke((Action)(() => ts.Text = "Mobile Client GPS Test Utility was not found - FIle Path to NWS Addon Folder."));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.OrangeRed));

                    string LogEntry = DateTime.Now + @" ERROR: COULD NOT LOCATE GPS TESTER UTILITY. Attempting to Download";

                    LogEntryWriter(LogEntry);

                    string LogEntry1 = DateTime.Now + @" Addon Download Folder Displayed.";

                    LogEntryWriter(LogEntry1);

                    BeginInvoke((Action)(() => secondForm.Show()));
                }
            }

            //this will run the U-Blox Work Around for mobile if present. This will also continue off where it ended if there is a shutdown
            //If the files are not present and are not already downloaded, then a download prompt will appear.
            if (MobileTriage.GetItemCheckState(8) == CheckState.Checked)
            {
                BeginInvoke((Action)(() => ts.Text = "Checking to see if Utility is in the proper location"));
                BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));
                if (File.Exists(@"C:\Temp\MobileInstaller\NWS Addons\U-BLOX WorkAround\gps_config - Remove\install.exe"))
                {
                    if (!File.Exists(@"C:\Temp\MobileInstaller\NWS Addons\UBloxRemoveCompleted.txt"))
                    {
                        BeginInvoke((Action)(() => ts.Text = "Removing UBlox Configuration"));
                        BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

                        string LogEntry = DateTime.Now + " UBlox Configuration Removal Started";

                        LogEntryWriter(LogEntry);

                        RunProgram("install.exe", @"C:\Temp\MobileInstaller\NWS Addons\U-BLOX WorkAround\gps_config - Remove");

                        string path = @"C:\Temp\MobileInstaller\NWS Addons\UBloxRemoveCompleted.txt";

#pragma warning disable CS0642 // Possible mistaken empty statement
                        using (StreamWriter sw = File.CreateText(path)) ;
#pragma warning restore CS0642 // Possible mistaken empty statement

                        Process.Start("Shutdown", "/s");

                        string LogEntry1 = DateTime.Now + " Machine Shut down as part of Work Around SOP";

                        LogEntryWriter(LogEntry1);

                        string LogEntry2 = DateTime.Now + " UBlox Configuration Removal Successfully Completed";

                        LogEntryWriter(LogEntry2);

                        MessageBox.Show("The machine must shut down during this process. Please turn machine back on and re run this process when you log back in.");
                    }
                    else
                    {
                        BeginInvoke((Action)(() => ts.Text = "Setting UBlox Configuration"));
                        BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.DarkSlateBlue));

                        string LogEntry = DateTime.Now + " UBlox Configuration Setting Started";

                        LogEntryWriter(LogEntry);

                        RunProgram("install.exe", @"C:\Temp\MobileInstaller\NWS Addons\U-BLOX WorkAround\gps_config - Set");

                        string LogEntry1 = DateTime.Now + " UBlox Configuration Setting Successfully Completed";

                        LogEntryWriter(LogEntry1);
                    }
                }
                else
                {
                    BeginInvoke((Action)(() => ts.Text = "U-BLOX Work around was not found - File Path to NWS Addon Folder"));
                    BeginInvoke((Action)(() => ts.ForeColor = System.Drawing.Color.OrangeRed));

                    string LogEntry = DateTime.Now + @" ERROR: COULD NOT LOCATE UBLOX WORK AROUND FILES. ATTEMPTING TO DOWNLOAD";

                    LogEntryWriter(LogEntry);

                    string LogEntry1 = DateTime.Now + @" Addon Download Folder Displayed.";

                    LogEntryWriter(LogEntry1);

                    BeginInvoke((Action)(() => secondForm.Show()));
                }
            }
        }

        //Updater utility code - Tab 3

        //work done to add the police client to the updater config file
        private void PoliceClientSub()
        {
            XmlNodeList Applications = UpdaterConfig.GetElementsByTagName("applications");

            XmlNode nodeAdd = UpdaterConfig.CreateElement("add");

            XmlAttribute id = UpdaterConfig.CreateAttribute("id");
            id.Value = "Police Client";

            XmlAttribute uri = UpdaterConfig.CreateAttribute("manifestUri");
            uri.Value = "http://" + MobileServer.Text + "/MobileUpdates/Police Client/Update.xml";

            XmlAttribute location = UpdaterConfig.CreateAttribute("location");

            if (Is64Bit.Checked == true)
            {
                location.Value = @"C:\Program Files (x86)\New World Systems\Aegis Mobile\";
            }
            else
            {
                location.Value = @"C:\Program Files\New World Systems\Aegis Mobile\";
            }

            nodeAdd.Attributes.Append(id);
            nodeAdd.Attributes.Append(uri);
            nodeAdd.Attributes.Append(location);

            Applications[0].AppendChild(nodeAdd);

            string LogEntry = DateTime.Now + " Police Client" + " has been added to Updater Config.";

            LogEntryWriter(LogEntry);
        }

        //work done to add the fire client to the updater config file
        private void FireClientSub()
        {
            XmlNodeList Applications = UpdaterConfig.GetElementsByTagName("applications");

            XmlNode nodeAdd = UpdaterConfig.CreateElement("add");

            XmlAttribute id = UpdaterConfig.CreateAttribute("id");
            id.Value = "Fire Client";

            XmlAttribute uri = UpdaterConfig.CreateAttribute("manifestUri");
            uri.Value = "http://" + MobileServer.Text + "/MobileUpdates/Fire Client/Update.xml";

            XmlAttribute location = UpdaterConfig.CreateAttribute("location");

            if (Is64Bit.Checked == true)
            {
                location.Value = @"C:\Program Files (x86)\New World Systems\Aegis Fire Mobile\";
            }
            else
            {
                location.Value = @"C:\Program Files\New World Systems\Aegis Fire Mobile\";
            }

            nodeAdd.Attributes.Append(id);
            nodeAdd.Attributes.Append(uri);
            nodeAdd.Attributes.Append(location);

            Applications[0].AppendChild(nodeAdd);

            string LogEntry = DateTime.Now + " Fire Client" + " has been added to Updater Config.";

            LogEntryWriter(LogEntry);
        }

        //work done to add the merge client to the updater config file
        private void MergeClientSub()
        {
            XmlNodeList Applications = UpdaterConfig.GetElementsByTagName("applications");

            XmlNode nodeAdd = UpdaterConfig.CreateElement("add");

            XmlAttribute id = UpdaterConfig.CreateAttribute("id");
            id.Value = "Merge Client";

            XmlAttribute uri = UpdaterConfig.CreateAttribute("manifestUri");
            uri.Value = "http://" + MobileServer.Text + "/MobileUpdates/Merge Client/Update.xml";

            XmlAttribute location = UpdaterConfig.CreateAttribute("location");

            if (Is64Bit.Checked == true)
            {
                location.Value = @"C:\Program Files (x86)\New World Systems\Aegis Mobile\";
            }
            else
            {
                location.Value = @"C:\Program Files\New World Systems\Aegis Mobile\";
            }

            nodeAdd.Attributes.Append(id);
            nodeAdd.Attributes.Append(uri);
            nodeAdd.Attributes.Append(location);

            Applications[0].AppendChild(nodeAdd);

            string LogEntry = DateTime.Now + " Merge Client" + " has been added to Updater Config.";

            LogEntryWriter(LogEntry);
        }

        //work done to add the updater to the updater config file
        private void NewWorldUpdaterSub()
        {
            XmlNodeList Applications = UpdaterConfig.GetElementsByTagName("applications");

            XmlNode nodeAdd = UpdaterConfig.CreateElement("add");

            XmlAttribute id = UpdaterConfig.CreateAttribute("id");
            id.Value = "NWS Updater";

            XmlAttribute uri = UpdaterConfig.CreateAttribute("manifestUri");
            uri.Value = "http://" + MobileServer.Text + "/MobileUpdates/NWS Updater/Update.xml";

            XmlAttribute location = UpdaterConfig.CreateAttribute("location");

            if (Is64Bit.Checked == true)
            {
                location.Value = @"C:\Program Files (x86)\New World Systems\New World Automatic Updater";
            }
            else
            {
                location.Value = @"C:\Program Files\New World Systems\Aegis Mobile\New World Automatic Updater";
            }

            nodeAdd.Attributes.Append(id);
            nodeAdd.Attributes.Append(uri);
            nodeAdd.Attributes.Append(location);

            Applications[0].AppendChild(nodeAdd);

            string LogEntry = DateTime.Now + " NWS Updater" + " has been added to Updater Config.";

            LogEntryWriter(LogEntry);
        }

        //work done per filled in ORI field to add them to the updater config file
        private void ORISub(string ORI)
        {
            XmlNodeList Applications = UpdaterConfig.GetElementsByTagName("applications");

            XmlNode nodeAdd = UpdaterConfig.CreateElement("add");

            XmlAttribute id = UpdaterConfig.CreateAttribute("id");

            id.Value = ORI.ToUpper();

            XmlAttribute uri = UpdaterConfig.CreateAttribute("manifestUri");
            uri.Value = "http://" + MobileServer.Text + "/MobileUpdates/" + ORI + "/Update.xml";

            XmlAttribute location = UpdaterConfig.CreateAttribute("location");

            if (Is64Bit.Checked)
            {
                location.Value = @"C:\Program Files (x86)\New World Systems\Aegis Mobile\";
            }
            else
            {
                location.Value = @"C:\Program Files\New World Systems\Aegis Mobile\";
            }

            nodeAdd.Attributes.Append(id);
            nodeAdd.Attributes.Append(uri);
            nodeAdd.Attributes.Append(location);

            Applications[0].AppendChild(nodeAdd);

            string LogEntry = DateTime.Now + " " + ORI + " has been added to Updater Config.";

            LogEntryWriter(LogEntry);
        }

        //work done per filled in fdid field to added them to the updater config file
        private void FDIDSub(string FDID)
        {
            XmlNodeList Applications = UpdaterConfig.GetElementsByTagName("applications");

            XmlNode nodeAdd = UpdaterConfig.CreateElement("add");

            XmlAttribute id = UpdaterConfig.CreateAttribute("id");
            id.Value = FDID;

            XmlAttribute uri = UpdaterConfig.CreateAttribute("manifestUri");
            uri.Value = "http://" + MobileServer.Text + "/MobileUpdates/" + FDID + "/Update.xml";

            XmlAttribute location = UpdaterConfig.CreateAttribute("location");

            if (Is64Bit.Checked)
            {
                location.Value = @"C:\Program Files (x86)\New World Systems\Aegis Fire Mobile\";
            }
            else
            {
                location.Value = @"C:\Program Files\New World Systems\Aegis Fire Mobile\";
            }

            nodeAdd.Attributes.Append(id);
            nodeAdd.Attributes.Append(uri);
            nodeAdd.Attributes.Append(location);

            Applications[0].AppendChild(nodeAdd);

            string LogEntry = DateTime.Now + " " + FDID + " has been added to Updater Config.";

            LogEntryWriter(LogEntry);
        }

        //work done to compare information already in the updater config file to information that is in the utility
        //the information that is already present does not get added
        private void SeeIfNodesExist()
        {
            XmlNodeList PoliceClientNode = UpdaterConfig.GetElementsByTagName("add");
            List<XmlNode> toRemove = new List<XmlNode>();

            //try{
            foreach (XmlNode AddNode in PoliceClientNode)
            {
                XmlAttributeCollection attrColl = AddNode.Attributes;
                XmlAttribute AttrID = attrColl["id"];
                XmlAttribute AttrURI = attrColl["manifestUri"];

                if (AttrID != null)
                {
                    if ((AttrID.Value == "Police Client"))// && (AttrURI.Value == "http://" + MobileServer.Text + "/MobileUpdates/Police Client/Update.xml"))
                    {
                        toRemove.Add(AddNode);
                    }

                    if ((AttrID.Value == "Fire Client"))// && (AttrURI.Value == "http://" + MobileServer.Text + "/MobileUpdates/Fire Client/Update.xml"))
                    {
                        toRemove.Add(AddNode);
                    }

                    if ((AttrID.Value == "Merge Client"))// && (AttrURI.Value == "http://" + MobileServer.Text + "/MobileUpdates/Merge Client/Update.xml"))
                    {
                        toRemove.Add(AddNode);
                    }

                    if ((AttrID.Value == "NWS Updater"))// && (AttrURI.Value == "http://" + MobileServer.Text + "/MobileUpdates/Merge Client/Update.xml"))
                    {
                        toRemove.Add(AddNode);
                    }

                    foreach (Control c in tabPage3.Controls)
                    {
                        if (c.Name.Contains("ORI"))
                        {
                            if (c.Text != "")
                            {
                                string upperORI = c.Text.ToUpper();
                                if ((AttrID.Value == upperORI))// && (AttrURI.Value == "http://" + MobileServer.Text + "/MobileUpdates/" + ORI + "/Update.xml"))
                                {
                                    toRemove.Add(AddNode);
                                    break;
                                }
                            }
                        }
                    }

                    foreach (Control c in tabPage3.Controls)
                    {
                        if (c.Name.Contains("FDID"))
                        {
                            if (c.Text != "")
                            {
                                if ((AttrID.Value == c.Text))// && (AttrURI.Value == "http://" + MobileServer.Text + "/MobileUpdates/" + FDID + "/Update.xml"))
                                {
                                    toRemove.Add(AddNode);
                                    break;
                                }
                            }
                        }
                    }
                }
            } //foreach (XmlNode AddNode in PoliceClientNode)

            foreach (XmlNode xmlElement in toRemove)
            {
                try
                {
                    XmlNode node = xmlElement.ParentNode;
                    node.RemoveChild(xmlElement);
                }
                catch
                {
                    MessageBox.Show("ORI's and FDID's should not be the same");
                }
            }
        }

        //this will count and remove the ORI and FDID text boxes on the third tab page
        private void RemoveFormEntries()
        {
            //this does the counting
            foreach (Control c in tabPage3.Controls)
            {
                if (c.Name.Contains("ORI"))
                {
                    j++;
                }
            }

            //this will remove
            for (int i = 0; i <= j; i++)
            {
                //ORI specific
                foreach (Control c in tabPage3.Controls)
                {
                    if (c.Name.Contains("ORI"))
                    {
                        tabPage3.Controls.Remove(c);
                        break;
                    }
                }

                //FDID Specific
                foreach (Control c in tabPage3.Controls)
                {
                    if (c.Name.Contains("FDID"))
                    {
                        tabPage3.Controls.Remove(c);
                        break;
                    }
                }
            }
        }

        //this writes to the mobile pre req installer log file
        private void LogEntryWriter(string LogEntry)
        {
            using (StreamWriter file = new StreamWriter(("NWPSAdminLog.txt"), true))
            {
                file.WriteLine(LogEntry);
            }
        }

        //-------------------------------pre checker code - tab 4----------------------------------------------------------------

        private bool PreReqChecker(string ProgramName)
        {
            ManagementObjectSearcher mos = new ManagementObjectSearcher(
                @"SELECT * FROM Win32_Product WHERE Name = '" + ProgramName + "'");
            foreach (ManagementObject mo in mos.Get())
            {
                try
                {
                    if (mo["Name"].ToString() == ProgramName)
                    {
                        string LogEntry1 = DateTime.Now + " " + ProgramName + " was found by the PreReqChecker";

                        LogEntryWriter(LogEntry1);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);

                    string LogEntry2 = DateTime.Now + " " + ex.ToString();

                    LogEntryWriter(LogEntry2);
                }
            }

            string LogEntry3 = DateTime.Now + " " + ProgramName + " was not found by the PreReqChecker.";

            LogEntryWriter(LogEntry3);

            //no found
            return false;
        }

        //used to modify the pre req checker tab from the Uninstall function
        private void UninstallChecker(string ProgramName)
        {
            if (ProgramName.ToString() == "New World Automatic Updater")
            {
                try
                {
                    BeginInvoke((Action)(() => label27.Text = "Uninstalled"));
                    BeginInvoke((Action)(() => label27.ForeColor = System.Drawing.Color.OrangeRed));

                    string logentry1 = DateTime.Now + " Program Checker tab updated for Automatic Updater - UNINSTALLED";
                    LogEntryWriter(logentry1);
                }
                catch (Exception ex)
                {
                    string logentry2 = DateTime.Now + " " + ex.ToString();
                    LogEntryWriter(logentry2);
                }
            }

            if (ProgramName.ToString() == "Microsoft SQL Server Compact 4.0 x64 ENU")
            {
                try
                {
                    BeginInvoke((Action)(() => label28.Text = "Uninstalled"));
                    BeginInvoke((Action)(() => label28.ForeColor = System.Drawing.Color.OrangeRed));

                    string logentry1 = DateTime.Now + " Program Checker tab updated for SQL Compact 4.0 64bit - UNINSTALLED";
                    LogEntryWriter(logentry1);
                }
                catch (Exception ex)
                {
                    string logentry2 = DateTime.Now + " " + ex.ToString();
                    LogEntryWriter(logentry2);
                }
            }

            if (ProgramName.ToString() == "Microsoft SQL Server Compact 4.0 ENU")
            {
                try
                {
                    BeginInvoke((Action)(() => label28.Text = "Uninstalled"));
                    BeginInvoke((Action)(() => label28.ForeColor = System.Drawing.Color.OrangeRed));

                    string logentry1 = DateTime.Now + " Program Checker tab updated for SQL Compact 4.0 32bit - UNINSTALLED";
                    LogEntryWriter(logentry1);
                }
                catch (Exception ex)
                {
                    string logentry2 = DateTime.Now + " " + ex.ToString();
                    LogEntryWriter(logentry2);
                }
            }

            if (ProgramName.ToString() == "New World GIS Components x86")
            {
                try
                {
                    BeginInvoke((Action)(() => label29.Text = "Uninstalled"));
                    BeginInvoke((Action)(() => label29.ForeColor = System.Drawing.Color.OrangeRed));

                    string logentry1 = DateTime.Now + " Program Checker tab updated for 32 bit GIS Components - UNINSTALLED";
                    LogEntryWriter(logentry1);
                }
                catch (Exception ex)
                {
                    string logentry2 = DateTime.Now + " " + ex.ToString();
                    LogEntryWriter(logentry2);
                }
            }

            if (ProgramName.ToString() == "New World GIS Components x64")
            {
                try
                {
                    BeginInvoke((Action)(() => label30.Text = "Uninstalled"));
                    BeginInvoke((Action)(() => label30.ForeColor = System.Drawing.Color.OrangeRed));

                    string logentry1 = DateTime.Now + " Program Checker tab updated for 64 bit GIS Components - UNINSTALLED";
                    LogEntryWriter(logentry1);
                }
                catch (Exception ex)
                {
                    string logentry2 = DateTime.Now + " " + ex.ToString();
                    LogEntryWriter(logentry2);
                }
            }

            if (ProgramName.ToString() == "Microsoft SQL Server System CLR Types")
            {
                try
                {
                    BeginInvoke((Action)(() => label31.Text = "Uninstalled"));
                    BeginInvoke((Action)(() => label31.ForeColor = System.Drawing.Color.OrangeRed));

                    string logentry1 = DateTime.Now + " Program Checker tab updated for 32 bit SQL Server CLR Types - UNINSTALLED";
                    LogEntryWriter(logentry1);
                }
                catch (Exception ex)
                {
                    string logentry2 = DateTime.Now + " " + ex.ToString();
                    LogEntryWriter(logentry2);
                }
            }

            if (ProgramName.ToString() == "Microsoft SQL Server System CLR Types (x64)")
            {
                try
                {
                    BeginInvoke((Action)(() => label32.Text = "Uninstalled"));
                    BeginInvoke((Action)(() => label32.ForeColor = System.Drawing.Color.OrangeRed));

                    string logentry1 = DateTime.Now + " Program Checker tab updated for 64 bit SQL Server CLR Types - UNINSTALLED";
                    LogEntryWriter(logentry1);
                }
                catch (Exception ex)
                {
                    string logentry2 = DateTime.Now + " " + ex.ToString();
                    LogEntryWriter(logentry2);
                }
            }

            if (ProgramName.ToString() == "Microsoft SQL Server Compact 3.5 SP2 ENU")
            {
                try
                {
                    BeginInvoke((Action)(() => label33.Text = "Uninstalled"));
                    BeginInvoke((Action)(() => label33.ForeColor = System.Drawing.Color.OrangeRed));

                    string logentry1 = DateTime.Now + " Program Checker tab updated for 32 bit SQL Server Compact 3.5 SP2 - UNINSTALLED";
                    LogEntryWriter(logentry1);
                }
                catch (Exception ex)
                {
                    string logentry2 = DateTime.Now + " " + ex.ToString();
                    LogEntryWriter(logentry2);
                }
            }

            if (ProgramName.ToString() == "Microsoft SQL Server Compact 3.5 SP2 x64 ENU")
            {
                try
                {
                    BeginInvoke((Action)(() => label34.Text = "Uninstalled"));
                    BeginInvoke((Action)(() => label34.ForeColor = System.Drawing.Color.OrangeRed));

                    string logentry1 = DateTime.Now + " Program Checker tab updated for 64 bit SQL Server Compact 3.5 SP2 - UNINSTALLED";
                    LogEntryWriter(logentry1);
                }
                catch (Exception ex)
                {
                    string logentry2 = DateTime.Now + " " + ex.ToString();
                    LogEntryWriter(logentry2);
                }
            }

            if (ProgramName.ToString() == "ScenePD 6 Desktop Edition")
            {
                try
                {
                    BeginInvoke((Action)(() => label35.Text = "Uninstalled"));
                    BeginInvoke((Action)(() => label35.ForeColor = System.Drawing.Color.OrangeRed));

                    string logentry1 = DateTime.Now + " Program Checker tab updated for ScenePD 6 - UNINSTALLED";
                    LogEntryWriter(logentry1);
                }
                catch (Exception ex)
                {
                    string logentry2 = DateTime.Now + " " + ex.ToString();
                    LogEntryWriter(logentry2);
                }
            }

            if (ProgramName.ToString() == "ScenePD 6 ActiveX Control")
            {
                try
                {
                    BeginInvoke((Action)(() => label35.Text = "Uninstalled"));
                    BeginInvoke((Action)(() => label35.ForeColor = System.Drawing.Color.OrangeRed));

                    string logentry1 = DateTime.Now + " Program Checker tab updated for ScenePD 6 ActiveX Control - UNINSTALLED";
                    LogEntryWriter(logentry1);
                }
                catch (Exception ex)
                {
                    string logentry2 = DateTime.Now + " " + ex.ToString();
                    LogEntryWriter(logentry2);
                }
            }

            if (ProgramName.ToString() == "ScenePD 4")
            {
                try
                {
                    BeginInvoke((Action)(() => label35.Text = "Uninstalled"));
                    BeginInvoke((Action)(() => label35.ForeColor = System.Drawing.Color.OrangeRed));

                    string logentry1 = DateTime.Now + " Program Checker tab updated for ScenePD 4 - UNINSTALLED";
                    LogEntryWriter(logentry1);
                }
                catch (Exception ex)
                {
                    string logentry2 = DateTime.Now + " " + ex.ToString();
                    LogEntryWriter(logentry2);
                }
            }
        }

        //designed to modify the pre req checker tab from the run/install function
        private void InstallChecker(string ProgramName)
        {
            if (ProgramName.ToString() == "NewWorld.Management.Updater.msi")
            {
                try
                {
                    BeginInvoke((Action)(() => label27.Text = "Installed"));
                    BeginInvoke((Action)(() => label27.ForeColor = System.Drawing.Color.ForestGreen));

                    string logentry1 = DateTime.Now + " Program Checker tab updated for Automatic Updater - INSTALLED";
                    LogEntryWriter(logentry1);
                }
                catch (Exception ex)
                {
                    string logentry2 = DateTime.Now + " " + ex.ToString();
                    LogEntryWriter(logentry2);
                }
            }

            if (ProgramName.ToString() == "SSCERuntime_x64-ENU-4.0.exe")
            {
                try
                {
                    BeginInvoke((Action)(() => label28.Text = "Installed"));
                    BeginInvoke((Action)(() => label28.ForeColor = System.Drawing.Color.ForestGreen));

                    string logentry1 = DateTime.Now + " Program Checker tab updated for 64 bit SQL Compact 4.0 - INSTALLED";
                    LogEntryWriter(logentry1);
                }
                catch (Exception ex)
                {
                    string logentry2 = DateTime.Now + " " + ex.ToString();
                    LogEntryWriter(logentry2);
                }
            }

            if (ProgramName.ToString() == "SSCERuntime_x86-ENU-4.0.exe")
            {
                try
                {
                    BeginInvoke((Action)(() => label28.Text = "Installed"));
                    BeginInvoke((Action)(() => label28.ForeColor = System.Drawing.Color.ForestGreen));

                    string logentry1 = DateTime.Now + " Program Checker tab updated for 32 bit SQL Compact 4.0 - INSTALLED";
                    LogEntryWriter(logentry1);
                }
                catch (Exception ex)
                {
                    string logentry2 = DateTime.Now + " " + ex.ToString();
                    LogEntryWriter(logentry2);
                }
            }

            if (ProgramName.ToString() == "NewWorld.Gis.Components.x86.msi")
            {
                try
                {
                    BeginInvoke((Action)(() => label29.Text = "Installed"));
                    BeginInvoke((Action)(() => label29.ForeColor = System.Drawing.Color.ForestGreen));

                    string logentry1 = DateTime.Now + " Program Checker tab updated for 32 bit GIS Components - INSTALLED";
                    LogEntryWriter(logentry1);
                }
                catch (Exception ex)
                {
                    string logentry2 = DateTime.Now + " " + ex.ToString();
                    LogEntryWriter(logentry2);
                }
            }

            if (ProgramName.ToString() == "NewWorld.Gis.Components.x64.msi")
            {
                try
                {
                    BeginInvoke((Action)(() => label30.Text = "Installed"));
                    BeginInvoke((Action)(() => label30.ForeColor = System.Drawing.Color.ForestGreen));

                    string logentry1 = DateTime.Now + " Program Checker tab updated for 64 bit GIS Components - INSTALLED";
                    LogEntryWriter(logentry1);
                }
                catch (Exception ex)
                {
                    string logentry2 = DateTime.Now + " " + ex.ToString();
                    LogEntryWriter(logentry2);
                }
            }

            if (ProgramName.ToString() == "SQLSysClrTypesx86.msi")
            {
                try
                {
                    BeginInvoke((Action)(() => label31.Text = "Installed"));
                    BeginInvoke((Action)(() => label31.ForeColor = System.Drawing.Color.ForestGreen));

                    string logentry1 = DateTime.Now + " Program Checker tab updated for 32 bit SQL CLR Types - INSTALLED";
                    LogEntryWriter(logentry1);
                }
                catch (Exception ex)
                {
                    string logentry2 = DateTime.Now + " " + ex.ToString();
                    LogEntryWriter(logentry2);
                }
            }

            if (ProgramName.ToString() == "SQLSysClrTypesx64.msi")
            {
                try
                {
                    BeginInvoke((Action)(() => label32.Text = "Installed"));
                    BeginInvoke((Action)(() => label32.ForeColor = System.Drawing.Color.ForestGreen));

                    string logentry1 = DateTime.Now + " Program Checker tab updated for 64 bit SQL CLR Types - INSTALLED";
                    LogEntryWriter(logentry1);
                }
                catch (Exception ex)
                {
                    string logentry2 = DateTime.Now + " " + ex.ToString();
                    LogEntryWriter(logentry2);
                }
            }

            if (ProgramName.ToString() == "SSCERuntime_x86-ENU.msi")
            {
                try
                {
                    BeginInvoke((Action)(() => label33.Text = "Installed"));
                    BeginInvoke((Action)(() => label33.ForeColor = System.Drawing.Color.ForestGreen));

                    string logentry1 = DateTime.Now + " Program Checker tab updated for 32 bit SQL Compact 3.5 SP2 - INSTALLED";
                    LogEntryWriter(logentry1);
                }
                catch (Exception ex)
                {
                    string logentry2 = DateTime.Now + " " + ex.ToString();
                    LogEntryWriter(logentry2);
                }
            }

            if (ProgramName.ToString() == "SSCERuntime_x64-ENU.msi")
            {
                try
                {
                    BeginInvoke((Action)(() => label34.Text = "Installed"));
                    BeginInvoke((Action)(() => label34.ForeColor = System.Drawing.Color.ForestGreen));

                    string logentry1 = DateTime.Now + " Program Checker tab updated for 64 bit SQL Compact 3.5 SP2 - INSTALLED";
                    LogEntryWriter(logentry1);
                }
                catch (Exception ex)
                {
                    string logentry2 = DateTime.Now + " " + ex.ToString();
                    LogEntryWriter(logentry2);
                }
            }

            if (ProgramName.ToString() == "SPD6-4-8993.exe")
            {
                try
                {
                    BeginInvoke((Action)(() => label35.Text = "Installed"));
                    BeginInvoke((Action)(() => label35.ForeColor = System.Drawing.Color.ForestGreen));

                    string logentry1 = DateTime.Now + " Program Checker tab updated for ScenePD 6 - INSTALLED";
                    LogEntryWriter(logentry1);
                }
                catch (Exception ex)
                {
                    string logentry2 = DateTime.Now + " " + ex.ToString();
                    LogEntryWriter(logentry2);
                }
            }

            if (ProgramName.ToString() == "NewWorld.Management.Updater.msi")
            {
                try
                {
                    BeginInvoke((Action)(() => label35.Text = "Installed"));
                    BeginInvoke((Action)(() => label35.ForeColor = System.Drawing.Color.ForestGreen));

                    string logentry1 = DateTime.Now + " Program Checker tab updated for ScenePD 6 AcetiveX Controller - INSTALLED";
                    LogEntryWriter(logentry1);
                }
                catch (Exception ex)
                {
                    string logentry2 = DateTime.Now + " " + ex.ToString();
                    LogEntryWriter(logentry2);
                }
            }
        }

        //this will check the text on the pre req checker tab to make sure it is either default or uninstalled. If the text is not either of those it is installed and does not need to be checked.
        //I may change this in the future to account for uninstalled as a known status like installed technically is.
        private void PreStatusChecker()
        {
            //Updater Status Check
            if (label27.Text == "Waiting Initialization")
            {
                label27.Text = "Pending";
                label27.ForeColor = Color.DarkSlateBlue;
                label27.Refresh();
            }
            else if (label27.Text == "Uninstalled")
            {
                string logentry1 = DateTime.Now + " New World Automatic Updater is already uninstalled. Pre Req Checker not modified.";
                LogEntryWriter(logentry1);
            }
            else
            {
                string logentry1 = DateTime.Now + " New World Automatic Updater is already installed. Pre Req Checker not modified.";
                LogEntryWriter(logentry1);
            }

            //SQL Compact 4.0 Status Check
            if (label28.Text == "Waiting Initialization")
            {
                label28.Text = "Pending";
                label28.ForeColor = Color.DarkSlateBlue;
                label28.Refresh();
            }
            else if (label28.Text == "Uninstalled")
            {
                string logentry1 = DateTime.Now + " SQL Compact 4.0 is already uninstalled. Pre Req Checker not modified.";
                LogEntryWriter(logentry1);
            }
            else
            {
                string logentry1 = DateTime.Now + " SQL Compact 4.0 is already installed. Pre Req Checker not modified.";
                LogEntryWriter(logentry1);
            }

            //32 bit GIS Status Check
            if (label29.Text == "Waiting Initialization")
            {
                label29.Text = "Pending";
                label29.ForeColor = Color.DarkSlateBlue;
                label29.Refresh();
            }
            else if (label29.Text == "Uninstalled")
            {
                string logentry1 = DateTime.Now + " 32 bit GIS Components is already uninstalled. Pre Req Checker not modified.";
                LogEntryWriter(logentry1);
            }
            else
            {
                string logentry1 = DateTime.Now + " 32 bit GIS Components is already installed. Pre Req Checker not modified.";
                LogEntryWriter(logentry1);
            }

            //64 bit GIS Status Check
            if (label30.Text == "Waiting Initialization")
            {
                label30.Text = "Pending";
                label30.ForeColor = Color.DarkSlateBlue;
                label30.Refresh();
            }
            else if (label30.Text == "Uninstalled")
            {
                string logentry1 = DateTime.Now + " 64 bit GIS Components is already uninstalled. Pre Req Checker not modified.";
                LogEntryWriter(logentry1);
            }
            else
            {
                string logentry1 = DateTime.Now + " 64 bit GIS Components is already installed. Pre Req Checker not modified.";
                LogEntryWriter(logentry1);
            }

            //32 bit SQL CLR Status Check
            if (label31.Text == "Waiting Initialization")
            {
                label31.Text = "Pending";
                label31.ForeColor = Color.DarkSlateBlue;
                label31.Refresh();
            }
            else if (label31.Text == "Uninstalled")
            {
                string logentry1 = DateTime.Now + " 32 bit SQL CLR Types is already uninstalled. Pre Req Checker not modified.";
                LogEntryWriter(logentry1);
            }
            else
            {
                string logentry1 = DateTime.Now + " 32 bit SQL CLR Types is already installed. Pre Req Checker not modified.";
                LogEntryWriter(logentry1);
            }

            //64 bit SQL CLR Status Check
            if (label32.Text == "Waiting Initialization")
            {
                label32.Text = "Pending";
                label32.ForeColor = Color.DarkSlateBlue;
                label32.Refresh();
            }
            else if (label32.Text == "Uninstalled")
            {
                string logentry1 = DateTime.Now + " 64 bit SQL CLR Types is already uninstalled. Pre Req Checker not modified.";
                LogEntryWriter(logentry1);
            }
            else
            {
                string logentry1 = DateTime.Now + " 64 bit SQL CLR Types is already installed. Pre Req Checker not modified.";
                LogEntryWriter(logentry1);
            }

            //32 bit SQL Compact 3.5 Status Check
            if (label33.Text == "Waiting Initialization")
            {
                label33.Text = "Pending";
                label33.ForeColor = Color.DarkSlateBlue;
                label33.Refresh();
            }
            else if (label33.Text == "Uninstalled")
            {
                string logentry1 = DateTime.Now + " 32 bit SQL Compact 3.5 SP2 is already uninstalled. Pre Req Checker not modified.";
                LogEntryWriter(logentry1);
            }
            else
            {
                string logentry1 = DateTime.Now + " 32 bit SQL Compact 3.5 SP2 is already installed. Pre Req Checker not modified.";
                LogEntryWriter(logentry1);
            }

            //64 bit SQL Compact 3.5 Status Check
            if (label34.Text == "Waiting Initialization")
            {
                label34.Text = "Pending";
                label34.ForeColor = Color.DarkSlateBlue;
                label34.Refresh();
            }
            else if (label34.Text == "Uninstalled")
            {
                string logentry1 = DateTime.Now + " 64 bit SQL Compact 3.5 SP2 is already uninstalled. Pre Req Checker not modified.";
                LogEntryWriter(logentry1);
            }
            else
            {
                string logentry1 = DateTime.Now + " 64 bit SQL Compact 3.5 SP2 is already installed. Pre Req Checker not modified.";
                LogEntryWriter(logentry1);
            }

            //ScenePD Status Check
            if (label35.Text == "Waiting Initialization")
            {
                label35.Text = "Pending";
                label35.ForeColor = Color.DarkSlateBlue;
                label35.Refresh();
            }
            else if (label35.Text == "Uninstalled")
            {
                string logentry1 = DateTime.Now + " ScenePD is already uninstalled. Pre Req Checker not modified.";
                LogEntryWriter(logentry1);
            }
            else
            {
                string logentry1 = DateTime.Now + " ScenePD is already installed. Pre Req Checker not modified.";
                LogEntryWriter(logentry1);
            }

            //Hidden
            if (label36.Text == "Waiting Initialization")
            {
                label36.Text = "Pending";
                label36.Refresh();
            }
            else if (label36.Text == "Uninstalled")
            {
                label36.Text = "Pending";
                label36.Refresh();
            }
            else
            {
                //string logentry1 = DateTime.Now + " already installed. Pre Req Checker not modified.";
                //LogEntryWriter(logentry1);
            }

            //Hidden
            if (label37.Text == "Waiting Initialization")
            {
                label37.Text = "Pending";
                label37.Refresh();
            }
            else if (label37.Text == "Uninstalled")
            {
                label37.Text = "Pending";
                label37.Refresh();
            }
            else
            {
                //string logentry1 = DateTime.Now + " already installed. Pre Req Checker not modified.";
                //LogEntryWriter(logentry1);
            }

            ////Hidden
            if (label38.Text == "Waiting Initialization")
            {
                label38.Text = "Pending";
                label38.Refresh();
            }
            else if (label38.Text == "Uninstalled")
            {
                label38.Text = "Pending";
                label38.Refresh();
            }
            else
            {
                //string logentry1 = DateTime.Now + " already installed. Pre Req Checker not modified.";
                //LogEntryWriter(logentry1);
            }

            ////Hidden
            if (label39.Text == "Waiting Initialization")
            {
                label39.Text = "Pending";
                label39.Refresh();
            }
            else if (label39.Text == "Uninstalled")
            {
                label39.Text = "Pending";
                label39.Refresh();
            }
            else
            {
                //string logentry1 = DateTime.Now + " already installed. Pre Req Checker not modified.";
                //LogEntryWriter(logentry1);
            }
        }
    }
}