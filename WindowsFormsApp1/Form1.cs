﻿using System;
using System.Security.AccessControl;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Windows.Forms;
using System.Xml;
using System.ServiceProcess;

namespace Mobile_App
{
    //Created 12/23/2017
    //Created by Sean Davitt, Tyler Tech NWPS Mobile Implementation Specialist
    //Designed for 64bit and 32 bit Windows

    //Changed .net requirement from 4.6.1 to 4.5 02/05/2018

    partial class Form1 : Form
    {
        private XmlDocument StartupSettings = new XmlDocument();
        private string SourcePath = @"";

        public string MSPServerName { get; private set; }

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
            ts.Text = "prepping Temp Folder";
            Temp();

            SaveStartupSettings();

            ts.Text = "Copying required files locally";
            //MobileCopy();
            ProgressBar.Value = 0;
            ProgressBar.Maximum = 16;
            bg.RunWorkerAsync();
        }

        //Help button logic on button click
        private void Button3_Click(object sender, EventArgs e)
        {
            var message = "Help Button \n\n";
            message += "If you didn't run this as Administrator close and re-run as Admin (Right click run as admin.) \n\n";
            message += "1. Fill in the network path from this computer to the mobile server C Drive. \n";
            message += "   1a. Fill in the network path from this computer to the msp server. \n";
            message += "2. Select the specific operation you'd like to perform. \n";
            message += "3. Uninstalling the mobile client will uninstall the mobile client AND all non-updater pre-reqs. \n";
            message += "4. Installing the mobile client will install all pre-regs necessary to run the mobile client. \n";
            message += "5. Selecting the Both check box and hitting the run button will uninstall the mobile client -> install the mobile client. \n";
            message += "6. The copy button MUST BE RUN FIRST. The copy button copies the files necessary to installer and/or uninstall the mobile client. \n\n";
            MessageBox.Show(message);
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
                RunProgram(@"Configure Updater for mobile V2.exe");
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
            string[] filepaths = Directory.GetFiles(SourcePath, "*.*");
            if (NwsHoldPath.Text != "")
            {
                foreach (string file in filepaths)
                {
                    try
                    {
                        string replace = file.Replace(SourcePath, TargetPath);
                        File.Copy(file, replace, true);
                        File.SetAttributes(TargetPath, FileAttributes.Normal);

                        bg.ReportProgress(0);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace.ToString());
                    }
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

        private void MobileDelete(string dir)
        {
            Directory.Delete(dir, true);
        }

        //Cleans up the temp folder and restarts the machine
        private void MobileRestart()
        {
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

            MobileDelete(@"C:\Programdata\New World Systems\New World Updater");

            Process.Start("Shutdown", "/r");

            Application.Exit();
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
                MSPServerPath.Text = StartupSettings.GetElementsByTagName("MSPServerPath")[0].InnerText;
                TargetPath = @"C:\Temp\MobileInstaller";
            }
            //Creation of a new MobileInstallApp.xml if one does not already exist.
            else
            {
                SourcePath = NwsHoldPath.Text;
                MSPServerName = MSPServerPath.Text;

                //root of the XML
                XmlNode root = StartupSettings.CreateElement("root");
                StartupSettings.AppendChild(root);

                //NWS HOLD PATH
                XmlNode NwsPathNode = StartupSettings.CreateElement("SourcePath");
                NwsPathNode.InnerText = @"File Path to Mobile Pre-reqs";
                root.AppendChild(NwsPathNode);

                //NWS MSP Server
                XmlNode MSPPathNode = StartupSettings.CreateElement("MSPServerPath");
                MSPPathNode.InnerText = @"FIle Path to MSP Client Install Folder";
                root.AppendChild(MSPPathNode);
            }
        }

        //When the xml is modified once it is changed for all other uses with that xml.
        private void SaveStartupSettings()
        {
            StartupSettings.GetElementsByTagName("SourcePath")[0].InnerText = NwsHoldPath.Text;
            StartupSettings.GetElementsByTagName("MSPServerPath")[0].InnerText = MSPServerPath.Text;
            StartupSettings.Save("MobileInstallApp.xml");

            //Save the start up settings
            StartupSettings.Save("MobileInstallApp.xml");
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
            MobileCopy(MSPServerPath.Text + @"\_Client-Installation\1 .NET Framework\.NET 4.7.1");

            MobileCopy(MSPServerPath.Text + @"\_Client-Installation\5 NWPS GIS Components\GIS Components 1.0.69");

            MobileCopy(MSPServerPath.Text + @"\_Client-Installation\3 SQL Compact Edition 3.5 SP2");

            MobileCopy(MSPServerPath.Text + @"\_Client-Installation\9 Microsoft Sync Framework 2.1\x64");

            MobileCopy(MSPServerPath.Text + @"\_Client-Installation\9 Microsoft Sync Framework 2.1\x86");

            MobileCopy(MSPServerPath.Text + @"\_Client-Installation\4 NWPS Updater\Updater 1.5.29");

            MobileCopy(NwsHoldPath.Text + @"NWS Hold\Client Initial Setup and Installation\7  Use updater configuration utility\Configure updater for Mobile V2");

            bg.ReportProgress(0);
        }

        //MISC

        //Message to display after the files are copied
        private void MessageCopy()
        {
            MessageBox.Show("The files are copied. Please press the run button.");
        }
    }
}