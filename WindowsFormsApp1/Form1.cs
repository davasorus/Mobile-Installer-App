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

    partial class Form1 : Form
    {
        private XmlDocument UpdaterConfig = new XmlDocument();
        private XmlDocument StartupSettings = new XmlDocument();
        private string SourcePath = @"";

        public string MSPServerName { get; private set; }
        private bool PoliceClientExists = false;
        private bool FireClientExists = false;
        private bool MergeClientExists = false;

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
        private ToolStripLabel ts = new ToolStripLabel();

        //what the form does when it initializes every time.
        private void Form1_Load(object sender, EventArgs e)
        {
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

            //will utilize prior configuration for mobile updater form if it exists
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
            }
        }

        public Form1()
        {
            InitializeComponent();

            bg = new BackgroundWorker();
            bg.DoWork += Bg_DoWork;
            bg.ProgressChanged += Bg_ProgressChanged;
            bg.RunWorkerCompleted += Bg_RunWorkerCompleted;
            bg.WorkerReportsProgress = true;
        }

        //Button Click events

        //run button
        private void Button2_Click(object sender, EventArgs e)
        {
            ProgressBar.Visible = false;
            ProgressBar.Enabled = false;

            //run combination mobile uninstall an mobile install
            if (Combo.Checked && Is64Bit.Checked == true)
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
                FileWork64Bit();
                UpdaterWork64Bit();

                ts.Text = "Uninstalling Mobile";
                Mobile64Uninstall();

                ts.Text = "Installing Mobile";
                Mobile64install();

                MessageBox.Show("Mobile has been Installed");

                ts.Text = "Restarting PC";
                MobileRestart();
            }

            //run 64bit installer
            if (Is64Bit.Checked && InstallMobile.Checked == true)
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

                ts.Text = "Going to Install mobile";
                Mobile64install();

                ts.Text = "Restarting PC";
                MobileRestart();
            }

            //Run 64bit uninstaller
            if (Is64Bit.Checked && UninstallMobile.Checked == true)
            {
                ts.Text = "Modifying Mobile Updater Entries";
                FileWork64Bit();
                UpdaterWork64Bit();

                ts.Text = "Going to Uninstall mobile";
                Mobile64Uninstall();

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

                ts.Text = "Uninstalling Mobile";
                Mobile32Uninstaller();

                ts.Text = "Going to Install mobile";
                Mobile32install();

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

                ts.Text = "installing Mobile";
                Mobile32install();

                ts.Text = "Restarting PC";
                MobileRestart();
            }

            //Run 32bit uninstaller
            if (Is32bit.Checked && UninstallMobile.Checked == true)
            {
                ts.Text = "Modifying Updater Files";
                FileWork32Bit();
                UpdaterWork32Bit();

                ts.Text = "Uninstalling Mobile";
                Mobile32Uninstaller();

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

            ProgressBar.Value = 0;
            ProgressBar.Maximum = 24;
            bg.RunWorkerAsync();
        }

        //Help button logic on button click
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

        //Customizable Install/Uninstall/Triage
        private void CustomRun_Click(object sender, EventArgs e)
        {
            ProgressBar.Visible = false;
            ProgressBar.Enabled = false;

            CustomUninstallRun();

            CustomInstallRun();

            MobileTriageRun();
        }

        //pre req install/uninstall methods

        //Mobile 64bit uninstaller
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
            }

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
            }

            ts.Text = "Running 32 bit GIS Components";
            try
            {
                InstallProgram(@"NewWorld.Gis.Components.x86.msi");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }

            ts.Text = "Running 64 bit GIS Components";
            try
            {
                InstallProgram(@"NewWorld.Gis.Components.x64.msi");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }

            ts.Text = "Running 64 bit Synchronization";
            try
            {
                InstallProgram(@"Synchronization-v2.1-x64-ENU.msi");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }

            ts.Text = "Running 64 bit Provider Services";
            try
            {
                InstallProgram(@"ProviderServices-v2.1-x64-ENU.msi");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }

            ts.Text = "Running 64 bit DB Providers";
            try
            {
                InstallProgram(@"DatabaseProviders-v3.1-x64-ENU.msi");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }

            ts.Text = "Installing Updater";
            try
            {
                InstallProgram(@"NewWorld.Management.Updater.msi");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
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
            }

            ts.Text = "Prepping folder permissions";
            try
            {
                SetAcl(@"C:\Program Files (x86)\New World Systems");
                SetAcl(@"C:\ProgramData\New World Systems");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }
        }

        //Mobile 32bit uninstaller
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
            }

            ts.Text = "Running 32bit SQL Runtime";
            try
            {
                InstallProgram(@"SSCERuntime_x86-ENU.msi");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }

            ts.Text = "Running 32 bit GIS Components";
            try
            {
                InstallProgram(@"NewWorld.Gis.Components.x86.msi");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }

            ts.Text = "Running 32 bit Synchronization";
            try
            {
                InstallProgram(@"Synchronization-v2.1-x86-ENU.msi");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }

            ts.Text = "Running 32 bit Provider Services";
            try
            {
                InstallProgram(@"ProviderServices-v2.1-x86-ENU.msi");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }

            ts.Text = "Running 32 bit DB Providers";
            try
            {
                InstallProgram(@"DatabaseProviders-v3.1-x86-ENU.msi");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }

            ts.Text = "Installing Updater";
            try
            {
                InstallProgram(@"NewWorld.Management.Updater.msi");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }

            ts.Text = "Running Mobile Updater Config form";
            try
            {
                RunProgram(@"Configure Updater for mobile V2.exe");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }

            ts.Text = "Prepping folder permissions";
            try
            {
                SetAcl(@"C:\Program Files\New World Systems");
                SetAcl(@"C:\ProgramData\New World Systems");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }
        }

        //File related work

        //will look into the updater config file and will replace any text that contains MobileUpdates with DeleteMe - 64 bit version
        private void FileWork64Bit()
        {
            string text = File.ReadAllText(@"C:\Program Files (x86)\New World Systems\New World Automatic Updater\NewWorld.Management.Updater.Service.exe.config");
            text = text.Replace(@"MobileUpdates", "DeleteMe");
            File.WriteAllText(@"C:\Program Files (x86)\New World Systems\New World Automatic Updater\NewWorld.Management.Updater.Service.exe.config", text);
        }

        //will look into the updater config file and remove any lines that contain DeleteMe - 64 bit version
        private void UpdaterWork64Bit()
        {
            string text = @"C:\Program Files (x86)\New World Systems\New World Automatic Updater\NewWorld.Management.Updater.Service.exe.config";

            string[] Lines = File.ReadAllLines(text);
            IEnumerable<string> newLines = Lines.Where(line => !line.Contains(@"/DeleteMe/"));
            File.WriteAllLines(text, newLines);
        }

        //Will look into the updater config file and will replace any text that contains MobileUpdates with DeleteMe - 32 bit version
        private void FileWork32Bit()
        {
            string text = File.ReadAllText(@"C:\Program Files\New World Systems\New World Automatic Updater\NewWorld.Management.Updater.Service.exe.config");
            text = text.Replace(@"MobileUpdates", "DeleteMe");
            File.WriteAllText(@"C:\Program Files\New World Systems\New World Automatic Updater\NewWorld.Management.Updater.Service.exe.config", text);
        }

        //will look into the updater config file and remove any lines that contain DeleteMe - 32 bit version
        private void UpdaterWork32Bit()
        {
            string text = @"C:\Program Files\New World Systems\New World Automatic Updater\NewWorld.Management.Updater.Service.exe.config";

            var Lines = File.ReadAllLines(text);
            var newLines = Lines.Where(line => !line.Contains(@"/DeleteMe/"));
            File.WriteAllLines(text, newLines);
        }

        //Mobile copy
        private void MobileCopy(string SourcePath)
        {
            //this copies all files within NwsHoldPath.text to C:\Temp\MobileInstaller recursively.
            //string SourcePath = NwsHoldPath.Text;
            string TargetPath = @"C:\Temp\MobileInstaller";
            IEnumerable<string> filepaths = Directory.EnumerateFiles(SourcePath, "*.*");
            if (NwsHoldPath.Text != "")
            {
                foreach (string file in filepaths)
                {
                    string replace = file.Replace(SourcePath, TargetPath);
                    File.Copy(file, replace, true);
                    File.SetAttributes(TargetPath, FileAttributes.Normal);

                    bg.ReportProgress(0);
                }
            }
        }

        //Folder Related work

        //Temp file Creation, MobileInstaller Creation, Temp file cleaning on button click - Created on 02/01
        private void Temp()
        {
            //This was modified on 01/30/2018
            //This creates a temp folder if the folder does not exist.
            Directory.CreateDirectory(@"C:\Temp");

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

            //This was modified on 01/30/2018
            //This creates the mobile installer inside the the temp
            Directory.CreateDirectory(@"C:\Temp\MobileInstaller");
        }

        //Deletes folders by path and recursvely deletes sub folders
        private void MobileDelete(string dir)
        {
            Directory.Delete(dir, true);
        }

        //Cleans up the temp folder and restarts the machine
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
            }

            try
            {
                ts.Text = "Deleting Pre Req Folder";

                MobileDelete(@"C:\Temp\MobileInstaller");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
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

        //Methods to run the pre req executables

        //This is will run any/all programs that need user interaction by name
        private void RunProgram(string ProgramName)
        {
            Process proc = null;

            string batdir = string.Format(@"C:\Temp\MobileInstaller");
            proc = new Process();
            proc.StartInfo.WorkingDirectory = batdir;
            proc.StartInfo.FileName = ProgramName;
            //proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.Start();
            proc.WaitForExit();
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
            proc.Start();
            proc.WaitForExit();
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
                            return (bool)hr;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }

                //was not found...
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        //XML related controls

        //XML Related information. Broken up between loading prior XML information OR creating a new XML with placeholder server location.
        private void InitialLoadofXML()
        {
            //Checking if the MobileInstallApp.xml exists, and loading the data if it does.
            if (File.Exists("MobileInstallApp.xml"))
            {
                StartupSettings.Load("MobileInstallApp.xml");

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
            }
            //Creation of a new MobileInstallApp.xml if one does not already exist.
            else
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = ("   "),
                    CloseOutput = true,
                    OmitXmlDeclaration = true
                };
                using (XmlWriter writer = XmlWriter.Create("MobileInstallApp.xml", settings))
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
            }
        }

        //When the xml is modified once it is changed for all other uses with that xml.
        private void SaveStartupSettings()
        {
            StartupSettings.Load("MobileInstallApp.xml");

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
            StartupSettings.Save("MobileInstallApp.xml");
        }

        //this will remove ORI entries from the Mobile Install App xml
        private void UpdateXMLORI()
        {
            string text = "MobileInstallApp.xml";

            string[] Lines = File.ReadAllLines(text);
            IEnumerable<string> newLines = Lines.Where(line => !line.Contains(@"ORI"));
            File.WriteAllLines(text, newLines);
        }

        //this will remove FDID entries from the Mobile Install App xml
        private void UpdateXMLFDID()
        {
            string text = "MobileInstallApp.xml";

            string[] Lines = File.ReadAllLines(text);
            IEnumerable<string> newLines = Lines.Where(line => !line.Contains(@"FDID"));
            File.WriteAllLines(text, newLines);
        }

        //this saves ORI entries to the xml to be used again
        private void CreateXMLORI(string ORI, string name)
        {
            XDocument xDocument = XDocument.Load("MobileInstallApp.xml");

            var doc = xDocument.Root.Element("root");

            xDocument.Root.Add(new XElement(name, ORI));

            xDocument.Save("MobileInstallApp.xml");
        }

        //this saves FDID entries to the xml to be used again
        private void CreateXMLFDID(string FDID, string name)
        {
            XDocument xDocument = XDocument.Load("MobileInstallApp.xml");

            var doc = xDocument.Root.Element("root");

            xDocument.Root.Add(new XElement(name, FDID));

            xDocument.Save("MobileInstallApp.xml");
        }

        //will load old/prior ORI config in xml
        private void LoadORIXML()
        {
            StartupSettings.Load("MobileInstallApp.xml");

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
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        //will load old/prior FDID onfig in xml
        private void LoadFDIDXML()
        {
            StartupSettings.Load("MobileInstallApp.xml");

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
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        //Background Worker code

        //Declairs the background worker
        private BackgroundWorker bg;

        //What to do when the Background worker is completed
        private void Bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ts.Text = "Files Copied Locally";
        }

        //What to do when progress is made
        private void Bg_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBar.Value += 1;
            //label.Content = e.ProgressPercentage;
        }

        //The actual work done
        private void Bg_DoWork(object sender, DoWorkEventArgs e)
        {
            MobileCopy(MSPServerPath.Text + @"\\_Client-Installation\\1 .NET Framework\\.NET 4.7.1"); //works

            MobileCopy(MSPServerPath.Text + @"\\_Client-Installation\\5 NWPS GIS Components\\GIS Components 1.0.69"); //works

            MobileCopy(MSPServerPath.Text + @"\\_Client-Installation\\3 SQL Compact Edition 3.5 SP2"); //works

            MobileCopy(MSPServerPath.Text + @"\\_Client-Installation\\9 Microsoft Sync Framework 2.1\\x64"); //works

            MobileCopy(MSPServerPath.Text + @"\\_Client-Installation\\9 Microsoft Sync Framework 2.1\\x86");// works

            if (Directory.Exists(MSPServerPath.Text + @"\\_Client-Installation\\4 NWPS Updater\\Updater 1.5.29"))
            {
                MobileCopy(MSPServerPath.Text + @"\\_Client-Installation\\4 NWPS Updater\\Updater 1.5.29"); //works
            }
            else
            {
                MobileCopy(MSPServerPath.Text + @"\\_Client-Installation\\4 NWPS Updater\\Updater 1.5.23"); //works
            }

            MobileCopy(MSPServerPath.Text + @"\_Client-Installation\\8 MSP Client");

            MobileCopy(MSPServerPath.Text + @"\\_Client-Installation\\10 Visual Studio 2010 Tools for Office Runtime");

            MobileCopy(MSPServerPath.Text + @"\\_Client-Installation\\12 SQL Server CLR Types 2008");

            MobileCopy(MSPServerPath.Text + @"\\_Client-Installation\\13 Enterprise CAD Client");

            bg.ReportProgress(0);
        }

        //Local service work

        //will stop the service by name
        private void StopService(string name)
        {
            ServiceController sc = new ServiceController(name);
            if (sc.Status.Equals(ServiceControllerStatus.Running))
            {
                sc.Stop();

                ts.Text = "Updater Service Stopped";
            }
            else
            {
                MessageBox.Show("Stoping Error");
            }
        }

        //will start the service by name
        private void StartService(string name)
        {
            ServiceController sc = new ServiceController(name);
            if (sc.Status.Equals(ServiceControllerStatus.Stopped))
            {
                sc.Start();

                ts.Text = "Updater Service Started";
            }
            else
            {
                MessageBox.Show("Starting Error");
            }
        }

        //itemized uninstall steps
        private void CustomUninstallRun()
        {
            //uninstall fire mobile
            if (CustomUninstallOptions.GetItemCheckState(0) == CheckState.Checked)
            {
                ts.Text = "Checking to uninstall Fire Mobile";
                UninstallProgram("Aegis Fire Mobile");

                UninstallProgram("Fire Mobile");
                ts.Text = "Fire Mobile is Uninstalled";

                ts.Text = "Uninstall Complete";
            }

            //uninstall police mobile
            if (CustomUninstallOptions.GetItemCheckState(1) == CheckState.Checked)
            {
                ts.Text = "Checking to uninstall Police Mobile";
                UninstallProgram("Aegis Mobile");

                UninstallProgram("Law Enforcement Mobile");
                ts.Text = "Police Mobile is Uninstalled";

                ts.Text = "Uninstall Complete";
            }

            //uninstall merge
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

            //delete client folders
            if (CustomUninstallOptions.GetItemCheckState(6) == CheckState.Checked)
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
                }

                StartService("NewWorldUpdaterService");

                ts.Text = "Uninstall Complete";
            }

            //remove mobile related updater entries
            if (CustomUninstallOptions.GetItemCheckState(7) == CheckState.Checked)
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

            //Uninstall MSP or CAD
            if (CustomUninstallOptions.GetItemCheckState(8) == CheckState.Checked)
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

                        ts.Text = "test MSP uninstall";

                        //if someone wants to uninstall CAD second
                        string title1 = "Cad uninstall Dialog";
                        string message1 = "would you like to uninstall CAD";
                        MessageBoxButtons buttons1 = MessageBoxButtons.YesNo;
                        DialogResult result1 = MessageBox.Show(message1, title1, buttons1);
                        if (result == DialogResult.Yes)
                        {
                            ts.Text = "uninstalling CAD";

                            UninstallProgram("New World  Enterprise CAD Client");

                            ts.Text = "Test CAD uninstall";
                        }
                    }

                    //if someone wants to uninstall CAD but not MSP
                    else if (result == DialogResult.No)
                    {
                        string title1 = "Cad uninstall Dialog";
                        string message1 = "would you like to uninstall CAD";
                        MessageBoxButtons buttons1 = MessageBoxButtons.YesNo;
                        DialogResult result1 = MessageBox.Show(message1, title1, buttons1);
                        if (result == DialogResult.Yes)
                        {
                            ts.Text = "uninstalling CAD";

                            UninstallProgram("New World  Enterprise CAD Client");

                            ts.Text = "Test CAD uninstall";
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

                        ts.Text = "test MSP uninstall";

                        //if someone wants to uninstall CAD second
                        string title1 = "Cad uninstall Dialog";
                        string message1 = "would you like to uninstall CAD";
                        MessageBoxButtons buttons1 = MessageBoxButtons.YesNo;
                        DialogResult result1 = MessageBox.Show(message1, title1, buttons1);
                        if (result == DialogResult.Yes)
                        {
                            ts.Text = "uninstalling CAD";

                            UninstallProgram("New World  Enterprise CAD Client");

                            ts.Text = "Test CAD uninstall";
                        }
                    }

                    //if someone wants to uninstall CAD
                    else if (result == DialogResult.No)
                    {
                        string title1 = "Cad uninstall Dialog";
                        string message1 = "would you like to uninstall CAD";
                        MessageBoxButtons buttons1 = MessageBoxButtons.YesNo;
                        DialogResult result1 = MessageBox.Show(message1, title1, buttons1);
                        if (result == DialogResult.Yes)
                        {
                            ts.Text = "uninstalling CAD";

                            UninstallProgram("New World  Enterprise CAD Client");

                            ts.Text = "Test CAD uninstall";
                        }
                    }
                }

                ts.Text = "Uninstall Complete";
            }

            //Restart Machine
            if (CustomUninstallOptions.GetItemCheckState(9) == CheckState.Checked)
            {
                ts.Text = "Shutting Down PC";

                Process.Start("Shutdown", "/r");
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
                    }

                    ts.Text = "Running 64 bit GIS Components";
                    try
                    {
                        InstallProgram(@"NewWorld.Gis.Components.x64.msi");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace.ToString());
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
                    }

                    ts.Text = "Running 64 bit Provider Services";
                    try
                    {
                        InstallProgram(@"ProviderServices-v2.1-x64-ENU.msi");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace.ToString());
                    }

                    ts.Text = "Running 64 bit DB Providers";
                    try
                    {
                        InstallProgram(@"DatabaseProviders-v3.1-x64-ENU.msi");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace.ToString());
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
                    }

                    ts.Text = "Running 32 bit Provider Services";
                    try
                    {
                        InstallProgram(@"ProviderServices-v2.1-x86-ENU.msi");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace.ToString());
                    }

                    ts.Text = "Running 32 bit DB Providers";
                    try
                    {
                        InstallProgram(@"DatabaseProviders-v3.1-x86-ENU.msi");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace.ToString());
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
                        SetAcl(@"C:\ProgramData\New World Systems");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace.ToString());
                    }
                }
                else
                {
                    ts.Text = "Prepping folder permissions";
                    try
                    {
                        SetAcl(@"C:\Program Files\New World Systems");
                        SetAcl(@"C:\ProgramData\New World Systems");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace.ToString());
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
                        }

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
                        }

                        ts.Text = "Running 32 bit GIS Components";
                        try
                        {
                            InstallProgram(@"NewWorld.Gis.Components.x86.msi");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace.ToString());
                        }

                        ts.Text = "Running 64 bit GIS Components";
                        try
                        {
                            InstallProgram(@"NewWorld.Gis.Components.x64.msi");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace.ToString());
                        }

                        ts.Text = "Installing Updater";
                        try
                        {
                            InstallProgram(@"NewWorld.Management.Updater.msi");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace.ToString());
                        }

                        ts.Text = "Installing MSP";
                        try
                        {
                            RunProgram("NewWorldMSPClient.msi");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace.ToString());
                        }

                        ts.Text = "Prepping folder permissions";
                        try
                        {
                            SetAcl(@"C:\Program Files (x86)\New World Systems");
                            SetAcl(@"C:\ProgramData\New World Systems");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace.ToString());
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
                            }

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
                            }

                            ts.Text = "Running 32 bit GIS Components";
                            try
                            {
                                InstallProgram(@"NewWorld.Gis.Components.x86.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
                            }

                            ts.Text = "Running 64 bit GIS Components";
                            try
                            {
                                InstallProgram(@"NewWorld.Gis.Components.x64.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
                            }

                            ts.Text = "Running 64 bit Synchronization";
                            try
                            {
                                InstallProgram(@"Synchronization-v2.1-x64-ENU.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
                            }

                            ts.Text = "Running 64 bit Provider Services";
                            try
                            {
                                InstallProgram(@"ProviderServices-v2.1-x64-ENU.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
                            }

                            ts.Text = "Running 64 bit DB Providers";
                            try
                            {
                                InstallProgram(@"DatabaseProviders-v3.1-x64-ENU.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
                            }

                            ts.Text = "Installing Updater";
                            try
                            {
                                InstallProgram(@"NewWorld.Management.Updater.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
                            }

                            ts.Text = "Running Primary Interop Assemblies for Office";
                            try
                            {
                                InstallProgram(@"vstor_redist.exe");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
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
                            }

                            ts.Text = "Installing CAD";
                            try
                            {
                                RunProgram("NewWorld.Enterprise.CAD.Client.x64.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
                            }

                            ts.Text = "Prepping folder permissions";
                            try
                            {
                                SetAcl(@"C:\Program Files (x86)\New World Systems");
                                SetAcl(@"C:\ProgramData\New World Systems");
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
                            }

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
                            }

                            ts.Text = "Running 32 bit GIS Components";
                            try
                            {
                                InstallProgram(@"NewWorld.Gis.Components.x86.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
                            }

                            ts.Text = "Running 64 bit GIS Components";
                            try
                            {
                                InstallProgram(@"NewWorld.Gis.Components.x64.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
                            }

                            ts.Text = "Running 64 bit Synchronization";
                            try
                            {
                                InstallProgram(@"Synchronization-v2.1-x64-ENU.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
                            }

                            ts.Text = "Running 64 bit Provider Services";
                            try
                            {
                                InstallProgram(@"ProviderServices-v2.1-x64-ENU.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
                            }

                            ts.Text = "Running 64 bit DB Providers";
                            try
                            {
                                InstallProgram(@"DatabaseProviders-v3.1-x64-ENU.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
                            }

                            ts.Text = "Installing Updater";
                            try
                            {
                                InstallProgram(@"NewWorld.Management.Updater.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
                            }

                            ts.Text = "Running Primary Interop Assemblies for Office";
                            try
                            {
                                InstallProgram(@"vstor_redist.exe");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
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
                            }

                            ts.Text = "Installing CAD";
                            try
                            {
                                RunProgram("NewWorld.Enterprise.CAD.Client.x64.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
                            }

                            ts.Text = "Prepping folder permissions";
                            try
                            {
                                SetAcl(@"C:\Program Files (x86)\New World Systems");
                                SetAcl(@"C:\ProgramData\New World Systems");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
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
                        }

                        ts.Text = "Running 32bit SQL Runtime";
                        try
                        {
                            InstallProgram(@"SSCERuntime_x86-ENU.msi");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace.ToString());
                        }

                        ts.Text = "Running 32 bit GIS Components";
                        try
                        {
                            InstallProgram(@"NewWorld.Gis.Components.x86.msi");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace.ToString());
                        }

                        ts.Text = "Installing Updater";
                        try
                        {
                            InstallProgram(@"NewWorld.Management.Updater.msi");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace.ToString());
                        }

                        ts.Text = "Installing MSP";
                        try
                        {
                            RunProgram("NewWorldMSPClient.msi");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace.ToString());
                        }

                        ts.Text = "Prepping folder permissions";
                        try
                        {
                            SetAcl(@"C:\Program Files\New World Systems");
                            SetAcl(@"C:\ProgramData\New World Systems");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace.ToString());
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
                            }

                            ts.Text = "Running 32bit SQL Runtime";
                            try
                            {
                                InstallProgram(@"SSCERuntime_x86-ENU.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
                            }

                            ts.Text = "Running 32 bit GIS Components";
                            try
                            {
                                InstallProgram(@"NewWorld.Gis.Components.x86.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
                            }

                            ts.Text = "Running 32 bit Synchronization";
                            try
                            {
                                InstallProgram(@"Synchronization-v2.1-x86-ENU.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
                            }

                            ts.Text = "Running 32 bit Provider Services";
                            try
                            {
                                InstallProgram(@"ProviderServices-v2.1-x86-ENU.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
                            }

                            ts.Text = "Running 32 bit DB Providers";
                            try
                            {
                                InstallProgram(@"DatabaseProviders-v3.1-x86-ENU.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
                            }

                            ts.Text = "Installing Updater";
                            try
                            {
                                InstallProgram(@"NewWorld.Management.Updater.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
                            }

                            ts.Text = "Running Primary Interop Assemblies for Office";
                            try
                            {
                                InstallProgram(@"vstor_redist.exe");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
                            }

                            ts.Text = "SQL Server CLR Types 2008";
                            try
                            {
                                InstallProgram(@"SQLSysClrTypesx86.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
                            }

                            ts.Text = "Installing CAD";
                            try
                            {
                                RunProgram("NewWorld.Enterprise.CAD.Client.x86.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
                            }

                            ts.Text = "Prepping folder permissions";
                            try
                            {
                                SetAcl(@"C:\Program Files\New World Systems");
                                SetAcl(@"C:\ProgramData\New World Systems");
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
                            }

                            ts.Text = "Running 32bit SQL Runtime";
                            try
                            {
                                InstallProgram(@"SSCERuntime_x86-ENU.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
                            }

                            ts.Text = "Running 32 bit GIS Components";
                            try
                            {
                                InstallProgram(@"NewWorld.Gis.Components.x86.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
                            }

                            ts.Text = "Running 32 bit Synchronization";
                            try
                            {
                                InstallProgram(@"Synchronization-v2.1-x86-ENU.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
                            }

                            ts.Text = "Running 32 bit Provider Services";
                            try
                            {
                                InstallProgram(@"ProviderServices-v2.1-x86-ENU.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
                            }

                            ts.Text = "Running 32 bit DB Providers";
                            try
                            {
                                InstallProgram(@"DatabaseProviders-v3.1-x86-ENU.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
                            }

                            ts.Text = "Installing Updater";
                            try
                            {
                                InstallProgram(@"NewWorld.Management.Updater.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
                            }

                            ts.Text = "Running Primary Interop Assemblies for Office";
                            try
                            {
                                InstallProgram(@"vstor_redist.exe");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
                            }

                            ts.Text = "SQL Server CLR Types 2008";
                            try
                            {
                                InstallProgram(@"SQLSysClrTypesx86.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
                            }

                            ts.Text = "Installing CAD";
                            try
                            {
                                RunProgram("NewWorld.Enterprise.CAD.Client.x86.msi");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
                            }

                            ts.Text = "Prepping folder permissions";
                            try
                            {
                                SetAcl(@"C:\Program Files\New World Systems");
                                SetAcl(@"C:\ProgramData\New World Systems");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace.ToString());
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
            }
        }

        //itemized typical triage steps
        private void MobileTriageRun()
        {
            //set folder permissions
            if (MobileTriage.GetItemCheckState(0) == CheckState.Checked)
            {
                if (Is64Bit.Checked == true)
                {
                    ts.Text = "Prepping folder permissions";
                    try
                    {
                        SetAcl(@"C:\Program Files (x86)\New World Systems");
                        SetAcl(@"C:\ProgramData\New World Systems");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace.ToString());
                    }
                }
                else
                {
                    ts.Text = "Prepping folder permissions";
                    try
                    {
                        SetAcl(@"C:\Program Files\New World Systems");
                        SetAcl(@"C:\ProgramData\New World Systems");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace.ToString());
                    }
                }

                ts.Text = "Triage Complete";
            }

            //stop and restart the updater service
            if (MobileTriage.GetItemCheckState(1) == CheckState.Checked)
            {
                StopService("NewWorldUpdaterService");

                Thread.Sleep(5000);

                StartService("NewWorldUpdaterService");

                ts.Text = "Triage Complete";
            }

            //reset the updater folder under programdata
            if (MobileTriage.GetItemCheckState(2) == CheckState.Checked)
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
                }

                StartService("NewWorldUpdaterService");

                ts.Text = "Triage Complete";
            }

            //deletes the fire mobile, police mobile, and updater folder under programdata
            if (MobileTriage.GetItemCheckState(3) == CheckState.Checked)
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
                }

                StartService("NewWorldUpdaterService");

                ts.Text = "Triage Complete";
            }

            //will check to see if the Mobile Client Interface tester is in the MobileInstaller folder
            //if it is the program is run otherwise an exception is thrown
            if (MobileTriage.GetItemCheckState(4) == CheckState.Checked)
            {
                ts.Text = "Checking to see if Utility is in the proper location";
                if (File.Exists(@"C:\Temp\MobileInstaller\AegisMobileClientInterfaceTester.exe"))
                {
                    ts.Text = "Running Mobile Client Interface tester Utility";
                    RunProgram("AegisMobileClientInterfaceTester.exe");
                }
                else
                {
                    ts.Text = "Error see exception message";
                    throw new ArgumentException(@"ERROR: COULD NOT LOCATE MOBILE CLIENT INTERFACE UTILITY. Please make sure it in C:\Temp\MobileInstaller and try again.");
                    //ts.Text = @"ERROR: COULD NOT LOCATE MOBILE CLIENT INTERFACE UTILITY. Please make sure it in C:\Temp\MobileInstaller and try again.";
                }
                ts.Text = "Triage Complete";
            }

            //will check to see if the Device tester is in the MobileInstaller folder
            //if it is the program is run otherwise an exception is thrown
            if (MobileTriage.GetItemCheckState(5) == CheckState.Checked)
            {
                ts.Text = "Checking to see if Utility is in the proper location";
                if (File.Exists(@"C:\Temp\MobileInstaller\DeviceTester.exe"))
                {
                    ts.Text = "Running Device tester Utility";
                    RunProgram("DeviceTester.exe");
                }
                else
                {
                    ts.Text = "Error see exception message";
                    throw new ArgumentException(@"ERROR: COULD NOT LOCATE DEVICE TESTER UTILITY. Please make sure it in C:\Temp\MobileInstaller and try again.");
                    //ts.Text = @"ERROR: COULD NOT LOCATE MOBILE CLIENT INTERFACE UTILITY. Please make sure it in C:\Temp\MobileInstaller and try again.";
                }

                ts.Text = "Triage Complete";
            }
        }

        //Updater utility code

        //work done when the append button is pressed
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
        private void ORIGenerate_Click(object sender, EventArgs e)
        {
            label20.Visible = true;
            label8.Visible = true;

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
                }
            }
            catch (Exception)
            {
                MessageBox.Show(e.ToString());
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
            catch (Exception)
            {
                MessageBox.Show(e.ToString());
            }

            LoadFDIDXML();
        }

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
    }
}