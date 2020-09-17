using MobileInstallApp;
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

namespace Mobile_App
{
    //Created 12/23/2017
    //Created by Sean Davitt, Tyler Tech NWPS Mobile Implementation Specialist
    //Designed for 64bit and 32 bit Windows

    //Changed .net requirement from 4.6.1 to 4.5 02/05/2018

    partial class NWPSPreReqInstaller : Form
    {
        private XmlDocument UpdaterConfig = new XmlDocument();
        private XmlDocument StartupSettings = new XmlDocument();
        private string SourcePath = @"";
        private BackgroundWorker bg;
        public string MSPServerName { get; private set; }
        private bool PoliceClientExists = false;
        private bool FireClientExists = false;
        private bool MergeClientExists = false;
        private int j;
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
        private Form2 secondForm = new Form2();

        private ToolStripLabel ts = new ToolStripLabel();

        //what the form does when it initializes every time
        private void Form1_Load(object sender, EventArgs e)
        {
            string LogEntry = @"----------- Start of log file" + " " + DateTime.Now + "-----------";

            //this will put the current date of application start per run. This allows for easier readability.
            if (File.Exists("MobileInstalLog.txt"))
            {
                using (StreamWriter file = new StreamWriter(("MobileInstallLog.txt"), true))
                {
                    file.WriteLine(LogEntry);
                }
            }
            else
            {
                using (StreamWriter file = new StreamWriter(("MobileInstallLog.txt"), true))
                {
                    file.WriteLine(LogEntry);
                }
            }

            InitialLoadofXML();

            statusStrip1.Items.AddRange(new ToolStripItem[] { ts });
            ts.Text = "Ready";

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
            //if there is a non 0 number in the configuration xml there will that many ORI and FDID fields created on startup
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

                string LogEntry1 = DateTime.Now + ex.ToString();

                LogEntryWriter(LogEntry1);
            }
        }

        //background worker and component initialize
        public NWPSPreReqInstaller()
        {
            InitializeComponent();

            bg = new BackgroundWorker();
            bg.DoWork += Bg_DoWork;
            bg.ProgressChanged += Bg_ProgressChanged;
            bg.RunWorkerCompleted += Bg_RunWorkerCompleted;
            bg.WorkerReportsProgress = true;
        }

        //Button Click events

        //run button for TabPage1
        //used when doing mobile client upgrade/removal/install
        private void Button2_Click(object sender, EventArgs e)
        {
            ProgressBar.Visible = false;
            ProgressBar.Enabled = false;

            //run combination mobile uninstall an mobile install
            if (Combo.Checked && Is64Bit.Checked == true)
            {
                //an exception thrown if the generate number text box is 0
                //exceptions also thrown for null and non-null/not numerical entries
                if (GenerateNumber.Text == "0")
                {
                    ts.Text = "Please Verify the Updater portion is configured";
                    throw new ArgumentException(@"ERROR: Updater Configuration section of the utility is not configured, please fill out the tab and try again.");
                }
                else if (GenerateNumber.Text == " ")
                {
                    ts.Text = "Please Verify the Updater portion is configured";
                    throw new ArgumentException(@"ERROR: Updater Configuration section of the utility is not configured, please fill out the tab and try again.");
                }
                else if (GenerateNumber.Text == "")
                {
                    ts.Text = "Please Verify the Updater portion is configured";
                    throw new ArgumentException(@"ERROR: Updater Configuration section of the utility is not configured, please fill out the tab and try again.");
                }

                ts.Text = "Modifying Mobile Updater Entries";
                FileWork64Bit();
                UpdaterWork64Bit();

                string LogEntry1 = DateTime.Now + " 64Bit Mobile Updater Entry Removal Completed";

                LogEntryWriter(LogEntry1);

                string LogEntry2 = DateTime.Now + " 64Bit Mobile Uninstall Initiated";

                LogEntryWriter(LogEntry2);

                ts.Text = "Uninstalling Mobile";
                Mobile64Uninstall();

                string LogEntry3 = DateTime.Now + " 64Bit Mobile Uninstall Completed";

                LogEntryWriter(LogEntry3);

                string LogEntry4 = DateTime.Now + " 64Bit Mobile Install Initiated";

                LogEntryWriter(LogEntry4);

                ts.Text = "Installing Mobile";
                Mobile64install();

                string LogEntry5 = DateTime.Now + " 64Bit Mobile Pre Req Install Completed";

                LogEntryWriter(LogEntry5);

                MessageBox.Show("Mobile Pre Reqs have been Installed");

                ts.Text = "Restarting PC";
                MobileRestart();
            }

            //run 64bit installer
            if (Is64Bit.Checked && InstallMobile.Checked == true)
            {
                //this accounts for the GenerateNumber Textbox being blank/Null
                if (GenerateNumber.Text == "0")
                {
                    ts.Text = "Please Verify the Updater portion is configured";
                    throw new ArgumentException(@"ERROR: Updater Configuration section of the utility is not configured, please fill out the tab and try again.");
                }
                else if (GenerateNumber.Text == " ")
                {
                    ts.Text = "Please Verify the Updater portion is configured";
                    throw new ArgumentException(@"ERROR: Updater Configuration section of the utility is not configured, please fill out the tab and try again.");
                }
                else if (GenerateNumber.Text == "")
                {
                    ts.Text = "Please Verify the Updater portion is configured";
                    throw new ArgumentException(@"ERROR: Updater Configuration section of the utility is not configured, please fill out the tab and try again.");
                }

                string LogEntry1 = DateTime.Now + " 64Bit Mobile Install Initiated";

                LogEntryWriter(LogEntry1);

                ts.Text = "Going to Install mobile";
                Mobile64install();

                string LogEntry2 = DateTime.Now + " 64Bit Mobile Pre Req Install Completed";

                LogEntryWriter(LogEntry2);

                ts.Text = "Restarting PC";
                MobileRestart();
            }

            //Run 64bit uninstaller
            if (Is64Bit.Checked && UninstallMobile.Checked == true)
            {
                ts.Text = "Modifying Mobile Updater Entries";
                FileWork64Bit();
                UpdaterWork64Bit();

                string LogEntry1 = DateTime.Now + " 64Bit Mobile Updater Entry Removal Completed";

                LogEntryWriter(LogEntry1);

                string LogEntry2 = DateTime.Now + " 64Bit Mobile Uninstall Initiated";

                LogEntryWriter(LogEntry2);

                ts.Text = "Going to Uninstall mobile";
                Mobile64Uninstall();

                string LogEntry3 = DateTime.Now + " 64Bit Mobile Uninstall Completed";

                LogEntryWriter(LogEntry3);

                ts.Text = "Restarting PC";
                MobileRestart();
            }

            //Run combination mobile uninstall and mobile install
            if (Combo.Checked && Is32bit.Checked == true)
            {
                if (GenerateNumber.Text == "0")
                {
                    ts.Text = "Please Verify the Updater portion is configured";
                    throw new ArgumentException(@"ERROR: Updater Configuration section of the utility is not configured, please fill out the tab and try again.");
                }
                else if (GenerateNumber.Text == " ")
                {
                    ts.Text = "Please Verify the Updater portion is configured";
                    throw new ArgumentException(@"ERROR: Updater Configuration section of the utility is not configured, please fill out the tab and try again.");
                }
                else if (GenerateNumber.Text == "")
                {
                    ts.Text = "Please Verify the Updater portion is configured";
                    throw new ArgumentException(@"ERROR: Updater Configuration section of the utility is not configured, please fill out the tab and try again.");
                }

                ts.Text = "Modifying Mobile Updater Entries";
                FileWork32Bit();
                UpdaterWork32Bit();

                string LogEntry1 = DateTime.Now + " 32Bit Mobile Updater Entries removal completed";

                LogEntryWriter(LogEntry1);

                string LogEntry2 = DateTime.Now + " 32Bit Mobile Uninstall Initiated";

                LogEntryWriter(LogEntry2);

                ts.Text = "Uninstalling Mobile";
                Mobile32Uninstaller();

                string LogEntry3 = DateTime.Now + " 32Bit Mobile Sucessfully uninstalled";

                LogEntryWriter(LogEntry3);

                string LogEntry4 = DateTime.Now + " 32Bit Mobile Install Initiated";

                LogEntryWriter(LogEntry4);

                ts.Text = "Going to Install mobile";
                Mobile32install();

                string LogEntry5 = DateTime.Now + " 32Bit Mobile Sucessfully Installed";

                LogEntryWriter(LogEntry5);

                ts.Text = "Restarting PC";
                MobileRestart();
            }

            //Run 32bit installer
            if (Is32bit.Checked && InstallMobile.Checked == true)
            {
                if (GenerateNumber.Text == "0")
                {
                    ts.Text = "Please Verify the Updater portion is configured";
                    throw new ArgumentException(@"ERROR: Updater Configuration section of the utility is not configured, please fill out the tab and try again.");
                }
                else if (GenerateNumber.Text == " ")
                {
                    ts.Text = "Please Verify the Updater portion is configured";
                    throw new ArgumentException(@"ERROR: Updater Configuration section of the utility is not configured, please fill out the tab and try again.");
                }
                else if (GenerateNumber.Text == "")
                {
                    ts.Text = "Please Verify the Updater portion is configured";
                    throw new ArgumentException(@"ERROR: Updater Configuration section of the utility is not configured, please fill out the tab and try again.");
                }

                string LogEntry2 = DateTime.Now + " 32Bit Mobile Install Initiated";

                LogEntryWriter(LogEntry2);

                ts.Text = "installing Mobile";
                Mobile32install();

                string LogEntry3 = DateTime.Now + " 32Bit Mobile Sucessfully Installed";

                LogEntryWriter(LogEntry3);

                ts.Text = "Restarting PC";
                MobileRestart();
            }

            //Run 32bit uninstaller
            if (Is32bit.Checked && UninstallMobile.Checked == true)
            {
                ts.Text = "Modifying Updater Files";
                FileWork32Bit();
                UpdaterWork32Bit();

                string LogEntry1 = DateTime.Now + " 32 Bit Mobile Updater Entries removal completed";

                LogEntryWriter(LogEntry1);

                string LogEntry2 = DateTime.Now + " 32Bit Mobile Uninstall Initiated";

                LogEntryWriter(LogEntry2);

                ts.Text = "Uninstalling Mobile";
                Mobile32Uninstaller();

                string LogEntry3 = DateTime.Now + " 32Bit Mobile Sucessfully uninstalled";

                LogEntryWriter(LogEntry3);

                ts.Text = "Restarting PC";
                MobileRestart();
            }
        }

        //Copy Button
        private void CopyFiles_Click(object sender, EventArgs e)
        {
            ProgressBar.Visible = true;
            ProgressBar.Enabled = true;

            ts.Text = "prepping Temp Folder";
            Temp();

            SaveStartupSettings();

            ts.Text = "Copying required files locally";

            string LogEntry1 = DateTime.Now + " File Copy Initiated";

            LogEntryWriter(LogEntry1);

            ProgressBar.Value = 0;
            ProgressBar.Maximum = 24;
            bg.RunWorkerAsync();
        }

        //Help button logic on button click - WIP
        private void Button3_Click(object sender, EventArgs e)
        {
            var message = "Help Button \n\n";
            message += "If you didn't run this as Administrator close and re-run as Admin (Right click run as admin.) \n\n";
            message += "TAB 1: INSTALL/UNINSTALL OPTIONS \n\n";
            message += "1. Fill in the network path from this computer to the mobile server C Drive. E.X." + @" \\NWPSMMSPROD\C$" + "\n";
            message += "   1a. Fill in the network path from this computer to the msp server. E.X" + @" \\NWPSMSPPROD" + "\n";
            message += "2. Select the specific operation you'd like to perform. \n";
            message += "3. Uninstalling the mobile client will uninstall the mobile client AND all non-updater pre-reqs. \n";
            message += "4. Installing the mobile client will install all pre regs necessary to run the mobile client. \n";
            message += "5. Selecting the Both check box and hitting the run button will uninstall the mobile client -> install the mobile client. \n";
            message += "6. The copy button MUST BE RUN FIRST. The copy button copies the files necessary to installer and/or uninstall the mobile client. \n\n";
            message += "TAB 2: INSTALL/UNINSTALL CUSTOM OPTIONS  \n\n";
            message += "1. Uninstall Options  \n\n";
            message += "   1a. Broken up into customizable uninstalling: Uninstalling the clients you want, and pre reqs you want. Instead of the entire client/pre req suite. \n\n";
            message += "2. Install Options \n\n";
            message += "   2a. Broken up into customizable Installing: Installing the clients you want, and pre reqs you want. Instead of the entire client/pre req suite. \n\n";
            message += "3. Triage Options \n\n";
            message += "   3a. Typical Triage for the mobile client instead of having to do them by hand. \n\n";
            MessageBox.Show(message);
        }

        //Customizable Install/Uninstall/Triage Run Button
        //this is pressed when uninstalling/Installing certain portions of the software suite
        private void CustomRun_Click(object sender, EventArgs e)
        {
            ProgressBar.Visible = false;
            ProgressBar.Enabled = false;

            CustomUninstallRun();

            CustomInstallRun();

            MobileTriageRun();
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
            //this also modifies the pre req installer config xml for each ORI
            foreach (Control c in tabPage3.Controls)
            {
                if (c.Name.Contains("ORI"))
                {
                    if (c.Text != "")
                    {
                        ts.Text = "ORIs Added";
                        string ORI = c.Text;
                        string Name = c.Name;

                        CreateXMLORI(ORI, Name);

                        ORISub(ORI);
                    }
                }
            }

            //this creates the ORI text boxes
            //this also modifies the pre req installer config xml for each FDID
            foreach (Control c in tabPage3.Controls)
            {
                if (c.Name.Contains("FDID"))
                {
                    if (c.Text != "")
                    {
                        ts.Text = "FDIDs Added";
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

                string LogEntry = DateTime.Now + ex.ToString();

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

                string LogEntry = DateTime.Now + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            LoadFDIDXML();
        }

        //pre req install/uninstall methods

        //Mobile 64bit uninstaller
        //this will uninstall NWPS Mobile, NWS Mobile, Pre Reqs for 64bit mobile
        private void Mobile64Uninstall()
        {
            ts.Text = "Checking to uninstall Police Mobile";
            UninstallProgram("Aegis Mobile");

            UninstallProgram("Law Enforcement Mobile");
            ts.Text = "Police Mobile is Uninstalled";

            ts.Text = "Checking to uninstall Fire Mobile";
            UninstallProgram("Aegis Fire Mobile");

            UninstallProgram("Fire Mobile");
            ts.Text = "Fire Mobile is Uninstalled";

            ts.Text = "Checking to uninstall Mobile Merge";
            UninstallProgram("Aegis Mobile Merge");

            UninstallProgram("Mobile Merge");
            ts.Text = "Mobile Merge is Uninstalled";

            ts.Text = "Uninstalling DB Providers";
            UninstallProgram("Microsoft Sync Framework 3.1 Database Providers (x64) ENU");

            ts.Text = "Uninstalling Provider Services";
            UninstallProgram("Microsoft Sync Framework 2.1 Provider Services (x64) ENU");

            ts.Text = "Uninstalling Core Components";
            UninstallProgram("Microsoft Sync Framework 2.1 Core Components (x64) ENU");

            ts.Text = "Uninstalling GIS - Old";
            UninstallProgram("New World GIS Components");

            ts.Text = "Uninstalling GIS - New";
            UninstallProgram("New World GIS Components x64");

            UninstallProgram("New World GIS Components x86");
            ts.Text = "GIS is Uninstalled";

            ts.Text = "Uninstalling SQL Server Compact 3.5 SP2";
            UninstallProgram("Microsoft SQL Server Compact 3.5 SP2 x64 ENU");

            UninstallProgram("Microsoft SQL Server Compact 3.5 SP2 ENU");
            ts.Text = "SQL Server Compact 3.5 SP2 is Uninstalled";

            ts.Text = "Uninstalling Nova PDF";

            UninstallProgram("NWPS Enterprise Mobile PDF Printer");

            UninstallProgram("novaPDF 8 Printer Driver");

            UninstallProgram("novaPDF 8 SDK COM (x86)");

            UninstallProgram("novaPDF 8 SDK COM (x64)");
        }

        //Mobile 64bit installer
        //this will install the Pre Reqs required for 64bit mobile, ensure folder permissions are correct, and that the updater is configured for mobile.
        private void Mobile64install()
        {
            ts.Text = "Running 4.7.1 .Net";
            try
            {
                InstallProgram(@"dotNetFx471_Full_setup_Offline.exe");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            ts.Text = "Running 32bit SQL Runtime";
            try
            {
                InstallProgram(@"SSCERuntime_x86-ENU.msi");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            ts.Text = "Running 64 bit SQL Runtime";
            try
            {
                InstallProgram(@"SSCERuntime_x64-ENU.msi");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            ts.Text = "Running 32 bit GIS Components";
            try
            {
                InstallProgram(@"NewWorld.Gis.Components.x86.msi");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            ts.Text = "Running 64 bit GIS Components";
            try
            {
                InstallProgram(@"NewWorld.Gis.Components.x64.msi");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            ts.Text = "Running 64 bit Synchronization";
            try
            {
                InstallProgram(@"Synchronization-v2.1-x64-ENU.msi");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            ts.Text = "Running 64 bit Provider Services";
            try
            {
                InstallProgram(@"ProviderServices-v2.1-x64-ENU.msi");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            ts.Text = "Running 64 bit DB Providers";
            try
            {
                InstallProgram(@"DatabaseProviders-v3.1-x64-ENU.msi");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            ts.Text = "Installing Updater";
            try
            {
                InstallProgram(@"NewWorld.Management.Updater.msi");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            ts.Text = "Running Mobile Updater Config form";
            try
            {
                if (GenerateNumber.Text == "0")
                {
                    ts.Text = "Please Verify the Updater portion is configured and attempt again.";
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

                string LogEntry = DateTime.Now + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            ts.Text = "Prepping folder permissions";
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

                string LogEntry = DateTime.Now + ex.ToString();

                LogEntryWriter(LogEntry);
            }
        }

        //Mobile 32bit uninstaller
        //this will uninstall NWPS Mobile, NWS Mobile, Pre Reqs for 32bit mobile
        private void Mobile32Uninstaller()
        {
            ts.Text = "Checking to uninstall Police Mobile";
            UninstallProgram("Aegis Mobile");

            UninstallProgram("Law Enforcement Mobile");
            ts.Text = "Police Mobile is Uninstalled";

            ts.Text = "Checking to uninstall Fire Mobile";
            UninstallProgram("Aegis Fire Mobile");

            UninstallProgram("Fire Mobile");
            ts.Text = "Fire Mobile is Uninstalled";

            ts.Text = "Checking to uninstall Mobile Merge";
            UninstallProgram("Aegis Mobile Merge");

            UninstallProgram("Mobile Merge");
            ts.Text = "Mobile Merge is Uninstalled";

            ts.Text = "Uninstalling DB Providers";
            UninstallProgram("Microsoft Sync Framework 3.1 Database Providers (x86) ENU");

            ts.Text = "Uninstalling Provider Services";
            UninstallProgram("Microsoft Sync Framework 2.1 Provider Services (x86) ENU");

            ts.Text = "Uninstalling Core Components";
            UninstallProgram("Microsoft Sync Framework 2.1 Core Components (x86) ENU");

            ts.Text = "Uninstalling GIS - Old";
            UninstallProgram("New World GIS Components");

            ts.Text = "Uninstalling GIS - New";
            UninstallProgram("New World GIS Components x86");
            ts.Text = "GIS is Uninstalled";

            UninstallProgram("Microsoft SQL Server Compact 3.5 SP2 ENU");
            ts.Text = "SQL Server Compact 3.5 SP2 is Uninstalled";

            ts.Text = "Uninstalling Nova PDF";

            UninstallProgram("NWPS Enterprise Mobile PDF Printer");

            UninstallProgram("novaPDF 8 Printer Driver");

            UninstallProgram("novaPDF 8 SDK COM (x86)");

            UninstallProgram("novaPDF 8 SDK COM (x64)");
        }

        //Mobile 32bit Installer
        //this will install the Pre Reqs required for 32bit mobile, ensure folder permissions are correct, and that the updater is configured for mobile.
        private void Mobile32install()
        {
            ts.Text = "Running 4.7.1 .Net";
            try
            {
                InstallProgram(@"dotNetFx471_Full_setup_Offline.exe");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            ts.Text = "Running 32bit SQL Runtime";
            try
            {
                InstallProgram(@"SSCERuntime_x86-ENU.msi");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            ts.Text = "Running 32 bit GIS Components";
            try
            {
                InstallProgram(@"NewWorld.Gis.Components.x86.msi");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            ts.Text = "Running 32 bit Synchronization";
            try
            {
                InstallProgram(@"Synchronization-v2.1-x86-ENU.msi");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            ts.Text = "Running 32 bit Provider Services";
            try
            {
                InstallProgram(@"ProviderServices-v2.1-x86-ENU.msi");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            ts.Text = "Running 32 bit DB Providers";
            try
            {
                InstallProgram(@"DatabaseProviders-v3.1-x86-ENU.msi");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            ts.Text = "Installing Updater";
            try
            {
                InstallProgram(@"NewWorld.Management.Updater.msi");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            ts.Text = "Running Mobile Updater Config form";
            try
            {
                RunProgram(@"Configure Updater for mobile V2.exe", @"C:\Temp\MobileInstaller");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            ts.Text = "Prepping folder permissions";
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

                string LogEntry = DateTime.Now + ex.ToString();

                LogEntryWriter(LogEntry);
            }
        }

        //File related work

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

        //Folder Related work

        //Mobile copy
        //this will copy all files located at the NWSHoldPath.txt to the MobileInstaller folder within C:\Temp
        private void MobileCopy(string SourcePath)
        {
            string TargetPath = @"C:\Temp\MobileInstaller\";
            IEnumerable<string> filepaths = Directory.EnumerateFiles(SourcePath, "*.*");
            if (NwsHoldPath.Text != "")
            {
                foreach (string file in filepaths)
                {
                    string replace = file.Replace(SourcePath, TargetPath);
                    File.Copy(file, replace, true);
                    File.SetAttributes(TargetPath, FileAttributes.Normal);

                    bg.ReportProgress(0);

                    string LogEntry = DateTime.Now + " " + file + " has been copied.";

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

            string LogEntry = DateTime.Now + "NWS Addon Folder Copied " + "has been copied.";

            LogEntryWriter(LogEntry);
        }

        //Temp file Creation, MobileInstaller Creation, Temp file cleaning on button click - Created on 02/01
        private void Temp()
        {
            //This was modified on 01/30/2018
            //This creates a temp folder if the folder does not exist.
            Directory.CreateDirectory(@"C:\Temp");

            /*
            //Deletes all files under C:\Temp
            DirectoryInfo di = new DirectoryInfo(@"C:\Temp");
            foreach (FileInfo File in di.GetFiles())
            {
                File.Delete();
            }

            //Deletes all folders under C:\Temp
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            */
            //This was modified on 01/30/2018
            //This creates the mobile installer inside the temp
            Directory.CreateDirectory(@"C:\Temp\MobileInstaller");
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
                ts.Text = "Deleting Programdata Updater";

                MobileDelete(@"C:\Programdata\New World Systems\New World Updater");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            try
            {
                ts.Text = "Deleting Fire Mobile Folder";
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

                string LogEntry = DateTime.Now + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            try
            {
                ts.Text = "Deleting Police Mobile Folder";
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

                string LogEntry = DateTime.Now + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            try
            {
                ts.Text = "Deleting Pre Req Folder";

                MobileDelete(@"C:\Temp\MobileInstaller");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry = DateTime.Now + ex.ToString();

                LogEntryWriter(LogEntry);
            }

            StartService("NewWorldUpdaterService");

            ts.Text = "Shutting Down PC";

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
        private void InstallProgram(string PreReqName)
        {
            Process proc = null;

            string batdir = string.Format(@"C:\Temp\MobileInstaller");
            proc = new Process();
            proc.StartInfo.WorkingDirectory = batdir;
            proc.StartInfo.FileName = PreReqName;
            proc.StartInfo.Arguments = "/quiet";
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            string LogEntry1 = DateTime.Now + " attempting to install " + PreReqName;

            LogEntryWriter(LogEntry1);

            proc.Start();
            proc.WaitForExit();

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

        //This is the method that will silently uninstall pre - reqs by name
        private bool UninstallProgram(string ProgramName)
        {
            try
            {
                ManagementObjectSearcher mos = new ManagementObjectSearcher(
                  "SELECT * FROM Win32_Product WHERE Name = '" + ProgramName + "'");
                foreach (ManagementObject mo in mos.Get())
                {
                    try
                    {
                        if (mo["Name"].ToString() == ProgramName)
                        {
                            object hr = mo.InvokeMethod("Uninstall", null);

                            string LogEntry1 = DateTime.Now + " " + ProgramName + " has been uninstalled";

                            LogEntryWriter(LogEntry1);

                            return (bool)hr;
                        }
                    }
                    catch (InvalidCastException)
                    {
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);

                        string LogEntry2 = DateTime.Now + " " + ex.ToString();

                        LogEntryWriter(LogEntry2);
                    }
                }

                string LogEntry3 = DateTime.Now + " " + ProgramName + " was not uninstalled. It was either not installed or detected.";

                LogEntryWriter(LogEntry3);

                //was not found...
                return false;
            }
            catch (InvalidCastException)
            {
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                string LogEntry4 = DateTime.Now + " " + ex.ToString();

                LogEntryWriter(LogEntry4);

                return false;
            }
        }

        //XML related controls

        //XML Related information. Broken up between loading prior XML information OR creating a new XML with placeholder server location.
        private void InitialLoadofXML()
        {
            //Checking if the PreReqAppSettings.xml exists, and loading the data if it does.
            if (File.Exists("PreReqAppSettings.xml"))
            {
                StartupSettings.Load("PreReqAppSettings.xml");

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
                using (XmlWriter writer = XmlWriter.Create("PreReqAppSettings.xml", settings))
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

        //When the xml is modified once it is changed for all other uses with that xml.
        private void SaveStartupSettings()
        {
            StartupSettings.Load("PreReqAppSettings.xml");

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
            StartupSettings.Save("PreReqAppSettings.xml");

            string LogEntry = DateTime.Now + " App Setting XML Updated";

            LogEntryWriter(LogEntry);
        }

        //this will remove ORI entries from the Mobile Install App xml
        private void UpdateXMLORI()
        {
            string text = "PreReqAppSettings.xml";

            string[] Lines = File.ReadAllLines(text);
            IEnumerable<string> newLines = Lines.Where(line => !line.Contains(@"ORI"));
            File.WriteAllLines(text, newLines);

            string LogEntry = DateTime.Now + " ORI/s Added to XML.";

            LogEntryWriter(LogEntry);
        }

        //this will remove FDID entries from the Mobile Install App xml
        private void UpdateXMLFDID()
        {
            string text = "PreReqAppSettings.xml";

            string[] Lines = File.ReadAllLines(text);
            IEnumerable<string> newLines = Lines.Where(line => !line.Contains(@"FDID"));
            File.WriteAllLines(text, newLines);

            string LogEntry = DateTime.Now + " FDID/s added to XML";

            LogEntryWriter(LogEntry);
        }

        //this saves ORI entries to the xml to be used again
        private void CreateXMLORI(string ORI, string name)
        {
            XDocument xDocument = XDocument.Load("PreReqAppSettings.xml");

            var doc = xDocument.Root.Element("root");

            xDocument.Root.Add(new XElement(name, ORI));

            xDocument.Save("PreReqAppSettings.xml");
        }

        //this saves FDID entries to the xml to be used again
        private void CreateXMLFDID(string FDID, string name)
        {
            XDocument xDocument = XDocument.Load("PreReqAppSettings.xml");

            var doc = xDocument.Root.Element("root");

            xDocument.Root.Add(new XElement(name, FDID));

            xDocument.Save("PreReqAppSettings.xml");
        }

        //will load old/prior ORI config in xml
        //this is will use the text from the xml file that corresponds to the ORI text fields in the application
        private void LoadORIXML()
        {
            StartupSettings.Load("PreReqAppSettings.xml");

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

                            string LogEntry1 = DateTime.Now + ex.ToString();

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

        //will load old/prior FDID config in xml
        //this is will use the text from the xml file that corresponds to the FDID text fields in the application
        private void LoadFDIDXML()
        {
            StartupSettings.Load("PreReqAppSettings.xml");

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

                            string LogEntry1 = DateTime.Now + ex.ToString();

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

        //Background Worker code

        //What to do when the Background worker is completed
        private void Bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ts.Text = "Files Copied Locally";

            string LogEntry1 = DateTime.Now + " File Copy Finished";

            LogEntryWriter(LogEntry1);
        }

        //What to do when progress is made
        private void Bg_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBar.Value += 1;
        }

        //The actual work done
        private void Bg_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Directory.Exists(MSPServerPath.Text + @"\\_Client-Installation\"))
            {
                MobileCopy(MSPServerPath.Text + @"\\_Client-Installation\\1 .NET Framework\\.NET 4.7.1");

                MobileCopy(MSPServerPath.Text + @"\\_Client-Installation\\5 NWPS GIS Components\\GIS Components 1.0.69");

                MobileCopy(MSPServerPath.Text + @"\\_Client-Installation\\3 SQL Compact Edition 3.5 SP2");

                MobileCopy(MSPServerPath.Text + @"\\_Client-Installation\\9 Microsoft Sync Framework 2.1\\x64");

                MobileCopy(MSPServerPath.Text + @"\\_Client-Installation\\9 Microsoft Sync Framework 2.1\\x86");

                //exists to be cross version combatable: will download the updater msi provided it is a known version
                if (Directory.Exists(MSPServerPath.Text + @"\\_Client-Installation\\4 NWPS Updater\\Updater 1.5.29"))
                {
                    MobileCopy(MSPServerPath.Text + @"\\_Client-Installation\\4 NWPS Updater\\Updater 1.5.29");
                }
                else
                {
                    MobileCopy(MSPServerPath.Text + @"\\_Client-Installation\\4 NWPS Updater\\Updater 1.5.23");
                }

                MobileCopy(MSPServerPath.Text + @"\_Client-Installation\\8 MSP Client");

                MobileCopy(MSPServerPath.Text + @"\\_Client-Installation\\10 Visual Studio 2010 Tools for Office Runtime");

                MobileCopy(MSPServerPath.Text + @"\\_Client-Installation\\12 SQL Server CLR Types 2008");

                MobileCopy(MSPServerPath.Text + @"\\_Client-Installation\\13 Enterprise CAD Client");
            }
            else if (Directory.Exists(MSPServerPath.Text + @"\\DeviceTester\"))
            {
                MobileCopy1(MSPServerPath.Text);
            }
            else
            {
                MobileCopy(MSPServerPath.Text);
            }

            bg.ReportProgress(0);
        }

        //Local service work

        //will stop the service by name
        //currently only used for the Updater Service
        private void StopService(string name)
        {
            ServiceController sc = new ServiceController(name);
            if (sc.Status.Equals(ServiceControllerStatus.Running))
            {
                sc.Stop();

                string LogEntry = DateTime.Now + " " + name + " has been stopped.";

                LogEntryWriter(LogEntry);

                ts.Text = name + "Service Stopped";
            }
            else
            {
                ts.Text = name + " Service had an issue stopping";

                string LogEntry = DateTime.Now + " " + name + " Service is not currently Started. Cannot stop a stopped Service";

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

                    ts.Text = name + " Service Started";
                }
                else
                {
                    ts.Text = name + " Service had an issue starting";

                    string LogEntry = DateTime.Now + " " + name + " Service is not currently stopped. Cannot Start a started Service";

                    LogEntryWriter(LogEntry);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());

                string LogEntry1 = DateTime.Now + ex.ToString();

                LogEntryWriter(LogEntry1);

                ts.Text = name + " Service had an issue starting";

                string LogEntry = DateTime.Now + " " + name + " Service is not currently stopped. Cannot Start a started Service";

                LogEntryWriter(LogEntry);
            }
        }

        //itemized uninstall steps
        private void CustomUninstallRun()
        {
            //uninstall fire mobile
            //will uninstall fire mobile for NWPS and NWS
            if (CustomUninstallOptions.GetItemCheckState(0) == CheckState.Checked)
            {
                ts.Text = "Checking to uninstall Fire Mobile";
                UninstallProgram("Aegis Fire Mobile");

                UninstallProgram("Fire Mobile");
                ts.Text = "Fire Mobile is Uninstalled";

                ts.Text = "Uninstall Complete";
            }

            //uninstall police mobile
            //will uninstall police mobile for NWPS and NWS
            if (CustomUninstallOptions.GetItemCheckState(1) == CheckState.Checked)
            {
                ts.Text = "Checking to uninstall Police Mobile";
                UninstallProgram("Aegis Mobile");

                UninstallProgram("Law Enforcement Mobile");
                ts.Text = "Police Mobile is Uninstalled";

                ts.Text = "Uninstall Complete";
            }

            //uninstall merge
            //will uninstall the merge client for NWPS and NWS
            if (CustomUninstallOptions.GetItemCheckState(2) == CheckState.Checked)
            {
                ts.Text = "Checking to uninstall Mobile Merge";
                UninstallProgram("Aegis Mobile Merge");

                UninstallProgram("Mobile Merge");
                ts.Text = "Mobile Merge is Uninstalled";

                ts.Text = "Uninstall Complete";
            }

            //uninstall novaPDF
            if (CustomUninstallOptions.GetItemCheckState(3) == CheckState.Checked)
            {
                ts.Text = "Uninstalling Nova PDF";

                UninstallProgram("NWPS Enterprise Mobile PDF Printer");

                UninstallProgram("novaPDF 8 Printer Driver");

                UninstallProgram("novaPDF 8 SDK COM (x86)");

                UninstallProgram("novaPDF 8 SDK COM (x64)");

                ts.Text = "NOVA PDF is Uninstalled";

                ts.Text = "Uninstall Complete";
            }

            //uninstall GIS
            if (CustomUninstallOptions.GetItemCheckState(4) == CheckState.Checked)
            {
                ts.Text = "Uninstalling GIS - Old";
                UninstallProgram("New World GIS Components");

                ts.Text = "Uninstalling GIS - New";
                UninstallProgram("New World GIS Components x64");

                UninstallProgram("New World GIS Components x86");
                ts.Text = "GIS is Uninstalled";

                ts.Text = "Uninstall Complete";
            }

            //Uninstall SQL Compact
            if (CustomUninstallOptions.GetItemCheckState(5) == CheckState.Checked)
            {
                ts.Text = "Uninstalling SQL Server Compact 3.5 SP2";
                UninstallProgram("Microsoft SQL Server Compact 3.5 SP2 x64 ENU");

                UninstallProgram("Microsoft SQL Server Compact 3.5 SP2 ENU");
                ts.Text = "SQL Server Compact 3.5 SP2 is Uninstalled";

                ts.Text = "Uninstall Complete";
            }

            //Uninstall Updater client
            if (CustomUninstallOptions.GetItemCheckState(6) == CheckState.Checked)
            {
                UninstallProgram("New World Automatic Updater");

                ts.Text = "Uninstall Complete";
            }

            //delete client folders
            if (CustomUninstallOptions.GetItemCheckState(7) == CheckState.Checked)
            {
                StopService("NewWorldUpdaterService");

                Thread.Sleep(5000);

                //delete programdata updater
                try
                {
                    ts.Text = "Deleting Programdata Updater";

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
                    ts.Text = "Deleting Fire Mobile Folder";
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
                    ts.Text = "Deleting Police Mobile Folder";
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

                ts.Text = "Uninstall Complete";
            }

            //remove mobile related updater entries
            if (CustomUninstallOptions.GetItemCheckState(8) == CheckState.Checked)
            {
                if (Is64Bit.Checked == true)
                {
                    ts.Text = "Modifying Mobile Updater Entries";
                    FileWork64Bit();
                    UpdaterWork64Bit();
                    ts.Text = "Mobile Updater Entries are removed";
                }
                else
                {
                    ts.Text = "Modifying Mobile Updater Entries";
                    FileWork32Bit();
                    UpdaterWork32Bit();
                    ts.Text = "Mobile Updater Entries are removed";
                }

                ts.Text = "Uninstall Complete";
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
                        ts.Text = "uninstalling MSP";

                        UninstallProgram("New World MSP Client");

                        UninstallProgram("New World Aegis Client");

                        UninstallProgram("New World Aegis MSP Client");

                        ts.Text = "MSP has been uninstalled";

                        //if someone wants to uninstall CAD second
                        string title1 = "Cad uninstall Dialog";
                        string message1 = "would you like to uninstall CAD";
                        MessageBoxButtons buttons1 = MessageBoxButtons.YesNo;
                        DialogResult result1 = MessageBox.Show(message1, title1, buttons1);
                        if (result1 == DialogResult.Yes)
                        {
                            ts.Text = "uninstalling CAD";

                            UninstallProgram("New World Enterprise CAD Client");

                            ts.Text = "CAD has been uninstalled";
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
                            ts.Text = "uninstalling CAD";

                            UninstallProgram("New World  Enterprise CAD Client");

                            ts.Text = "CAD has been uninstalled";
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
                        ts.Text = "uninstalling MSP";

                        UninstallProgram("New World MSP Client");

                        UninstallProgram("New World Aegis Client");

                        UninstallProgram("New World Aegis MSP Client");

                        ts.Text = "MSP has been uninstalled";

                        //if someone wants to uninstall CAD second
                        string title1 = "Cad uninstall Dialog";
                        string message1 = "would you like to uninstall CAD";
                        MessageBoxButtons buttons1 = MessageBoxButtons.YesNo;
                        DialogResult result1 = MessageBox.Show(message1, title1, buttons1);
                        if (result1 == DialogResult.Yes)
                        {
                            ts.Text = "uninstalling CAD";

                            UninstallProgram("New World Enterprise CAD Client");

                            ts.Text = "CAD has been uninstalled";
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
                            ts.Text = "uninstalling CAD";

                            UninstallProgram("New World  Enterprise CAD Client");

                            ts.Text = "CAD has been uninstalled";
                        }
                    }
                }

                ts.Text = "Uninstall Complete";
            }

            //Restart Machine
            if (CustomUninstallOptions.GetItemCheckState(10) == CheckState.Checked)
            {
                ts.Text = "Shutting Down PC";

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
                ts.Text = "Running 4.7.1 .Net";
                try
                {
                    InstallProgram(@"dotNetFx471_Full_setup_Offline.exe");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());

                    string LogEntry = DateTime.Now + ex.ToString();

                    LogEntryWriter(LogEntry);
                }

                ts.Text = "Install Complete";
            }

            //install SQL Runtime
            if (CustomInstallOption.GetItemCheckState(1) == CheckState.Checked)
            {
                if (Is64Bit.Checked == true)
                {
                    ts.Text = "Running 32bit SQL Runtime";
                    try
                    {
                        InstallProgram(@"SSCERuntime_x86-ENU.msi");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace.ToString());
                    }

                    ts.Text = "Running 64 bit SQL Runtime";
                    try
                    {
                        InstallProgram(@"SSCERuntime_x64-ENU.msi");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace.ToString());

                        string LogEntry = DateTime.Now + ex.ToString();

                        LogEntryWriter(LogEntry);
                    }
                }
                else
                {
                    ts.Text = "Running 32bit SQL Runtime";
                    try
                    {
                        InstallProgram(@"SSCERuntime_x86-ENU.msi");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace.ToString());

                        string LogEntry = DateTime.Now + ex.ToString();

                        LogEntryWriter(LogEntry);
                    }
                }

                ts.Text = "Install Complete";
            }

            //install GIS components
            if (CustomInstallOption.GetItemCheckState(2) == CheckState.Checked)
            {
                if (Is64Bit.Checked == true)
                {
                    ts.Text = "Running 32 bit GIS Components";
                    try
                    {
                        InstallProgram(@"NewWorld.Gis.Components.x86.msi");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace.ToString());

                        string LogEntry = DateTime.Now + ex.ToString();

                        LogEntryWriter(LogEntry);
                    }

                    ts.Text = "Running 64 bit GIS Components";
                    try
                    {
                        InstallProgram(@"NewWorld.Gis.Components.x64.msi");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace.ToString());

                        string LogEntry = DateTime.Now + ex.ToString();

                        LogEntryWriter(LogEntry);
                    }
                }
                else
                {
                    ts.Text = "Running 32 bit GIS Components";
                    try
                    {
                        InstallProgram(@"NewWorld.Gis.Components.x86.msi");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace.ToString());

                        string LogEntry = DateTime.Now + ex.ToString();

                        LogEntryWriter(LogEntry);
                    }
                }

                ts.Text = "Install Complete";
            }

            //install DB Provider services
            if (CustomInstallOption.GetItemCheckState(3) == CheckState.Checked)
            {
                if (Is64Bit.Checked == true)
                {
                    ts.Text = "Running 64 bit Synchronization";
                    try
                    {
                        InstallProgram(@"Synchronization-v2.1-x64-ENU.msi");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace.ToString());

                        string LogEntry = DateTime.Now + ex.ToString();

                        LogEntryWriter(LogEntry);
                    }

                    ts.Text = "Running 64 bit Provider Services";
                    try
                    {
                        InstallProgram(@"ProviderServices-v2.1-x64-ENU.msi");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace.ToString());

                        string LogEntry = DateTime.Now + ex.ToString();

                        LogEntryWriter(LogEntry);
                    }

                    ts.Text = "Running 64 bit DB Providers";
                    try
                    {
                        InstallProgram(@"DatabaseProviders-v3.1-x64-ENU.msi");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace.ToString());

                        string LogEntry = DateTime.Now + ex.ToString();

                        LogEntryWriter(LogEntry);
                    }
                }
                else
                {
                    ts.Text = "Running 32 bit Synchronization";
                    try
                    {
                        InstallProgram(@"Synchronization-v2.1-x86-ENU.msi");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace.ToString());

                        string LogEntry = DateTime.Now + ex.ToString();

                        LogEntryWriter(LogEntry);
                    }

                    ts.Text = "Running 32 bit Provider Services";
                    try
                    {
                        InstallProgram(@"ProviderServices-v2.1-x86-ENU.msi");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace.ToString());

                        string LogEntry = DateTime.Now + ex.ToString();

                        LogEntryWriter(LogEntry);
                    }

                    ts.Text = "Running 32 bit DB Providers";
                    try
                    {
                        InstallProgram(@"DatabaseProviders-v3.1-x86-ENU.msi");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace.ToString());

                        string LogEntry = DateTime.Now + ex.ToString();

                        LogEntryWriter(LogEntry);
                    }
                }

                ts.Text = "Install Complete";
            }

            //install Updater
            if (CustomInstallOption.GetItemCheckState(4) == CheckState.Checked)
            {
                ts.Text = "Installing Updater";
                try
                {
                    InstallProgram(@"NewWorld.Management.Updater.msi");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());

                    string LogEntry = DateTime.Now + ex.ToString();

                    LogEntryWriter(LogEntry);
                }

                ts.Text = "Install Complete";
            }

            //Running Updater Config form
            if (CustomInstallOption.GetItemCheckState(5) == CheckState.Checked)
            {
                ts.Text = "Running Mobile Updater Config form";
                try
                {
                    if (GenerateNumber.Text == "0")
                    {
                        ts.Text = "Please Verify the Updater portion is configured";
                        throw new ArgumentException(@"ERROR: Updater Configuration section of the utility is not configured, please fill out the tab and try again.");
                    }
                    else if (GenerateNumber.Text == " ")
                    {
                        ts.Text = "Please Verify the Updater portion is configured";
                        throw new ArgumentException(@"ERROR: Updater Configuration section of the utility is not configured, please fill out the tab and try again.");
                    }
                    else if (GenerateNumber.Text == "")
                    {
                        ts.Text = "Please Verify the Updater portion is configured";
                        throw new ArgumentException(@"ERROR: Updater Configuration section of the utility is not configured, please fill out the tab and try again.");
                    }
                    else
                    {
                        UpdaterAppend_Click(new object(), new EventArgs());
                        ts.Text = "ORI/FDID Update Complete";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());

                    string LogEntry = DateTime.Now + ex.ToString();

                    LogEntryWriter(LogEntry);
                }
            }

            //Setting Folder Permissions
            if (CustomInstallOption.GetItemCheckState(6) == CheckState.Checked)
            {
                if (Is64Bit.Checked == true)
                {
                    ts.Text = "Prepping folder permissions";
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

                        string LogEntry1 = DateTime.Now + ex.ToString();

                        LogEntryWriter(LogEntry1);
                    }
                }
                else
                {
                    ts.Text = "Prepping folder permissions";
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

                        string LogEntry = DateTime.Now + ex.ToString();

                        LogEntryWriter(LogEntry);
                    }
                }

                ts.Text = "Install Complete";
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
                        ts.Text = "Running 4.7.1 .Net";
                        try
                        {
                            InstallProgram(@"dotNetFx471_Full_setup_Offline.exe");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace.ToString());

                            string LogEntry = DateTime.Now + ex.ToString();

                            LogEntryWriter(LogEntry);
                        }

                        ts.Text = "Running 32bit SQL Runtime";
                        try
                        {
                            InstallProgram(@"SSCERuntime_x86-ENU.msi");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace.ToString());

                            string LogEntry = DateTime.Now + ex.ToString();

                            LogEntryWriter(LogEntry);
                        }

                        ts.Text = "Running 64 bit SQL Runtime";
                        try
                        {
                            InstallProgram(@"SSCERuntime_x64-ENU.msi");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace.ToString());

                            string LogEntry = DateTime.Now + ex.ToString();

                            LogEntryWriter(LogEntry);
                        }

                        ts.Text = "Running 32 bit GIS Components";
                        try
                        {
                            InstallProgram(@"NewWorld.Gis.Components.x86.msi");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace.ToString());

                            string LogEntry = DateTime.Now + ex.ToString();

                            LogEntryWriter(LogEntry);
                        }

                        ts.Text = "Running 64 bit GIS Components";
                        try
                        {
                            InstallProgram(@"NewWorld.Gis.Components.x64.msi");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace.ToString());

                            string LogEntry = DateTime.Now + ex.ToString();

                            LogEntryWriter(LogEntry);
                        }

                        ts.Text = "Installing Updater";
                        try
                        {
                            InstallProgram(@"NewWorld.Management.Updater.msi");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace.ToString());

                            string LogEntry = DateTime.Now + ex.ToString();

                            LogEntryWriter(LogEntry);
                        }

                        ts.Text = "Installing MSP";
                        try
                        {
                            RunProgram("NewWorldMSPClient.msi", @"C:\Temp\MobileInstaller");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace.ToString());

                            string LogEntry = DateTime.Now + ex.ToString();

                            LogEntryWriter(LogEntry);
                        }

                        ts.Text = "Prepping folder permissions";
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

                            string LogEntry = DateTime.Now + ex.ToString();

                            LogEntryWriter(LogEntry);
                        }

                        string title1 = "Cad Install Dialog";
                        string message1 = "would you like to Install CAD";
                        MessageBoxButtons buttons1 = MessageBoxButtons.YesNo;
                        DialogResult result1 = MessageBox.Show(message1, title1, buttons1);

                        //if install CAD second
                        if (result1 == DialogResult.Yes)
                        {
                            ts.Text = "Running 4.7.1 .Net";
                            try
                            {
                                InstallProgram(@"dotNetFx471_Full_setup_Offline.exe");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Running 32bit SQL Runtime";
                            try
                            {
                                InstallProgram(@"SSCERuntime_x86-ENU.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Running 64 bit SQL Runtime";
                            try
                            {
                                InstallProgram(@"SSCERuntime_x64-ENU.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Running 32 bit GIS Components";
                            try
                            {
                                InstallProgram(@"NewWorld.Gis.Components.x86.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Running 64 bit GIS Components";
                            try
                            {
                                InstallProgram(@"NewWorld.Gis.Components.x64.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Running 64 bit Synchronization";
                            try
                            {
                                InstallProgram(@"Synchronization-v2.1-x64-ENU.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Running 64 bit Provider Services";
                            try
                            {
                                InstallProgram(@"ProviderServices-v2.1-x64-ENU.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Running 64 bit DB Providers";
                            try
                            {
                                InstallProgram(@"DatabaseProviders-v3.1-x64-ENU.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Installing Updater";
                            try
                            {
                                InstallProgram(@"NewWorld.Management.Updater.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Running Primary Interop Assemblies for Office";
                            try
                            {
                                InstallProgram(@"vstor_redist.exe");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "SQL Server CLR Types 2008";
                            try
                            {
                                InstallProgram(@"SQLSysClrTypesx86.msi");
                                InstallProgram(@"SQLSysClrTypesx64.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Installing CAD";
                            try
                            {
                                RunProgram("NewWorld.Enterprise.CAD.Client.x64.msi", @"C:\Temp\MobileInstaller");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Prepping folder permissions";
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

                                string LogEntry = DateTime.Now + ex.ToString();

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
                            ts.Text = "Running 4.7.1 .Net";
                            try
                            {
                                InstallProgram(@"dotNetFx471_Full_setup_Offline.exe");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Running 32bit SQL Runtime";
                            try
                            {
                                InstallProgram(@"SSCERuntime_x86-ENU.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Running 64 bit SQL Runtime";
                            try
                            {
                                InstallProgram(@"SSCERuntime_x64-ENU.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Running 32 bit GIS Components";
                            try
                            {
                                InstallProgram(@"NewWorld.Gis.Components.x86.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Running 64 bit GIS Components";
                            try
                            {
                                InstallProgram(@"NewWorld.Gis.Components.x64.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Running 64 bit Synchronization";
                            try
                            {
                                InstallProgram(@"Synchronization-v2.1-x64-ENU.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Running 64 bit Provider Services";
                            try
                            {
                                InstallProgram(@"ProviderServices-v2.1-x64-ENU.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Running 64 bit DB Providers";
                            try
                            {
                                InstallProgram(@"DatabaseProviders-v3.1-x64-ENU.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Installing Updater";
                            try
                            {
                                InstallProgram(@"NewWorld.Management.Updater.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Running Primary Interop Assemblies for Office";
                            try
                            {
                                InstallProgram(@"vstor_redist.exe");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "SQL Server CLR Types 2008";
                            try
                            {
                                InstallProgram(@"SQLSysClrTypesx86.msi");
                                InstallProgram(@"SQLSysClrTypesx64.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Installing CAD";
                            try
                            {
                                RunProgram("NewWorld.Enterprise.CAD.Client.x64.msi", @"C:\Temp\MobileInstaller");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Prepping folder permissions";
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

                                string LogEntry = DateTime.Now + ex.ToString();

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
                        ts.Text = "Running 4.7.1 .Net";
                        try
                        {
                            InstallProgram(@"dotNetFx471_Full_setup_Offline.exe");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace.ToString());

                            string LogEntry = DateTime.Now + ex.ToString();

                            LogEntryWriter(LogEntry);
                        }

                        ts.Text = "Running 32bit SQL Runtime";
                        try
                        {
                            InstallProgram(@"SSCERuntime_x86-ENU.msi");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace.ToString());

                            string LogEntry = DateTime.Now + ex.ToString();

                            LogEntryWriter(LogEntry);
                        }

                        ts.Text = "Running 32 bit GIS Components";
                        try
                        {
                            InstallProgram(@"NewWorld.Gis.Components.x86.msi");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace.ToString());

                            string LogEntry = DateTime.Now + ex.ToString();

                            LogEntryWriter(LogEntry);
                        }

                        ts.Text = "Installing Updater";
                        try
                        {
                            InstallProgram(@"NewWorld.Management.Updater.msi");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace.ToString());

                            string LogEntry = DateTime.Now + ex.ToString();

                            LogEntryWriter(LogEntry);
                        }

                        ts.Text = "Installing MSP";
                        try
                        {
                            RunProgram("NewWorldMSPClient.msi", @"C:\Temp\MobileInstaller");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace.ToString());

                            string LogEntry = DateTime.Now + ex.ToString();

                            LogEntryWriter(LogEntry);
                        }

                        ts.Text = "Prepping folder permissions";
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

                            string LogEntry = DateTime.Now + ex.ToString();

                            LogEntryWriter(LogEntry);
                        }

                        string title1 = "Cad Install Dialog";
                        string message1 = "would you like to Install CAD";
                        MessageBoxButtons buttons1 = MessageBoxButtons.YesNo;
                        DialogResult result1 = MessageBox.Show(message1, title1, buttons1);

                        //if install CAD second
                        if (result1 == DialogResult.Yes)
                        {
                            ts.Text = "Running 4.7.1 .Net";
                            try
                            {
                                InstallProgram(@"dotNetFx471_Full_setup_Offline.exe");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Running 32bit SQL Runtime";
                            try
                            {
                                InstallProgram(@"SSCERuntime_x86-ENU.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Running 32 bit GIS Components";
                            try
                            {
                                InstallProgram(@"NewWorld.Gis.Components.x86.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Running 32 bit Synchronization";
                            try
                            {
                                InstallProgram(@"Synchronization-v2.1-x86-ENU.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Running 32 bit Provider Services";
                            try
                            {
                                InstallProgram(@"ProviderServices-v2.1-x86-ENU.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Running 32 bit DB Providers";
                            try
                            {
                                InstallProgram(@"DatabaseProviders-v3.1-x86-ENU.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Installing Updater";
                            try
                            {
                                InstallProgram(@"NewWorld.Management.Updater.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Running Primary Interop Assemblies for Office";
                            try
                            {
                                InstallProgram(@"vstor_redist.exe");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "SQL Server CLR Types 2008";
                            try
                            {
                                InstallProgram(@"SQLSysClrTypesx86.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Installing CAD";
                            try
                            {
                                RunProgram("NewWorld.Enterprise.CAD.Client.x86.msi", @"C:\Temp\MobileInstaller");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Prepping folder permissions";
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
                            ts.Text = "Running 4.7.1 .Net";
                            try
                            {
                                InstallProgram(@"dotNetFx471_Full_setup_Offline.exe");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Running 32bit SQL Runtime";
                            try
                            {
                                InstallProgram(@"SSCERuntime_x86-ENU.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Running 32 bit GIS Components";
                            try
                            {
                                InstallProgram(@"NewWorld.Gis.Components.x86.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Running 32 bit Synchronization";
                            try
                            {
                                InstallProgram(@"Synchronization-v2.1-x86-ENU.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Running 32 bit Provider Services";
                            try
                            {
                                InstallProgram(@"ProviderServices-v2.1-x86-ENU.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Running 32 bit DB Providers";
                            try
                            {
                                InstallProgram(@"DatabaseProviders-v3.1-x86-ENU.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Installing Updater";
                            try
                            {
                                InstallProgram(@"NewWorld.Management.Updater.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Running Primary Interop Assemblies for Office";
                            try
                            {
                                InstallProgram(@"vstor_redist.exe");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "SQL Server CLR Types 2008";
                            try
                            {
                                InstallProgram(@"SQLSysClrTypesx86.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Installing CAD";
                            try
                            {
                                RunProgram("NewWorld.Enterprise.CAD.Client.x86.msi", @"C:\Temp\MobileInstaller");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }

                            ts.Text = "Prepping folder permissions";
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

                                string LogEntry = DateTime.Now + ex.ToString();

                                LogEntryWriter(LogEntry);
                            }
                        }
                    }
                }
                ts.Text = "Install Complete";
            }

            //Restart Machine
            if (CustomInstallOption.GetItemCheckState(8) == CheckState.Checked)
            {
                ts.Text = "Shutting Down PC";

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
                    ts.Text = "Prepping folder permissions";
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

                        string LogEntry = DateTime.Now + ex.ToString();

                        LogEntryWriter(LogEntry);
                    }
                }
                else
                {
                    ts.Text = "Prepping folder permissions";
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
                        Console.WriteLine(ex.StackTrace.ToString());

                        string LogEntry = DateTime.Now + ex.ToString();

                        LogEntryWriter(LogEntry);
                    }
                }

                ts.Text = "Triage Complete";
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

                ts.Text = "Triage Complete";
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
                    ts.Text = "Deleting Programdata Updater";
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

                    string LogEntry = DateTime.Now + ex.ToString();

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

                    string LogEntry = DateTime.Now + ex.ToString();

                    LogEntryWriter(LogEntry);
                }

                //this will start the new world updater service
                StartService("NewWorldUpdaterService");

                ts.Text = "Triage Complete";
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
                    ts.Text = "Deleting Programdata Updater";

                    //this will attempt to delete the new world updater folder under programdata
                    MobileDelete(@"C:\Programdata\New World Systems\New World Updater");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());

                    string LogEntry = DateTime.Now + ex.ToString();

                    LogEntryWriter(LogEntry);
                }

                try
                {
                    ts.Text = "Deleting Programdata Aegis Mobile";

                    //this will attempt to delete the new world updater folder under programdata
                    MobileDelete(@"C:\Programdata\New World Systems\Aegis Mobile");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());

                    string LogEntry = DateTime.Now + ex.ToString();

                    LogEntryWriter(LogEntry);
                }

                try
                {
                    ts.Text = "Deleting Fire Mobile Folder";
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

                    string LogEntry = DateTime.Now + ex.ToString();

                    LogEntryWriter(LogEntry);
                }

                try
                {
                    ts.Text = "Deleting Police Mobile Folder";
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

                    string LogEntry = DateTime.Now + ex.ToString();

                    LogEntryWriter(LogEntry);
                }

                StartService("NewWorldUpdaterService");

                ts.Text = "Triage Complete";
            }

            //will check to see if the Mobile Client Interface tester is in the MobileInstaller folder
            //if it is the program is run otherwise an exception is thrown
            if (MobileTriage.GetItemCheckState(4) == CheckState.Checked)
            {
                //this will run the Mobile Client Interface Tester utility if it is present, and if not display a custom error message
                ts.Text = "Checking to see if Utility is in the proper location";
                if (File.Exists(@"C:\Temp\MobileInstaller\AegisMobileClientInterfaceTester.exe"))
                {
                    ts.Text = "Running Mobile Client Interface tester Utility";

                    string LogEntry = DateTime.Now + @" Mobile Client Interface Tester Started";

                    LogEntryWriter(LogEntry);

                    RunProgram("AegisMobileClientInterfaceTester.exe", @"C:\Temp\MobileInstaller");
                }
                else
                {
                    ts.Text = "Error see exception message";

                    string LogEntry = DateTime.Now + @" ERROR: COULD NOT LOCATE MOBILE CLIENT INTERFACE UTILITY. Please make sure it in C:\Temp\MobileInstaller and try again.";

                    LogEntryWriter(LogEntry);

                    throw new ArgumentException(@"ERROR: COULD NOT LOCATE MOBILE CLIENT INTERFACE UTILITY. Please make sure it in C:\Temp\MobileInstaller and try again.");
                    //ts.Text = @"ERROR: COULD NOT LOCATE MOBILE CLIENT INTERFACE UTILITY. Please make sure it in C:\Temp\MobileInstaller and try again.";
                }
                ts.Text = "Triage Complete";
            }

            //will check to see if the Device tester is in the MobileInstaller folder
            //if it is the program is run otherwise an exception is thrown
            if (MobileTriage.GetItemCheckState(5) == CheckState.Checked)
            {
                //this will run the Mobile Client Device Tester utility if it is present, and if not display a custom error message
                ts.Text = "Checking to see if Utility is in the proper location";
                if (File.Exists(@"C:\Temp\MobileInstaller\NWS Addons\DeviceTester\DeviceTester.exe"))
                {
                    ts.Text = "Running Device tester Utility";

                    string LogEntry = DateTime.Now + " Device Tester Utility Started";

                    LogEntryWriter(LogEntry);

                    RunProgram("DeviceTester.exe", @"C:\Temp\MobileInstaller\NWS Addons\DeviceTester");
                }
                else
                {
                    ts.Text = "Error see exception message";

                    string LogEntry = DateTime.Now + @" ERROR: COULD NOT LOCATE DEVICE TESTER UTILITY. Please make sure it in C:\Temp\MobileInstaller\NWS Addons\DeviceTester and try again.";

                    LogEntryWriter(LogEntry);

                    secondForm.Show();

                    //throw new ArgumentException(@"ERROR: COULD NOT LOCATE DEVICE TESTER UTILITY. Please make sure it in C:\Temp\MobileInstaller and try again.");
                    //ts.Text = @"ERROR: COULD NOT LOCATE MOBILE CLIENT INTERFACE UTILITY. Please make sure it in C:\Temp\MobileInstaller and try again.";
                }

                ts.Text = "Triage Complete";
            }
        }

        //Updater utility code

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

        //this writes to the mobile pre req installer logfile
        private void LogEntryWriter(string LogEntry)
        {
            using (StreamWriter file = new StreamWriter(("MobileInstallLog.txt"), true))
            {
                file.WriteLine(LogEntry);
            }
        }
    }
}