using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Mobile_App
{
    //Created 12/23/2017
    //Created by Sean Davitt, Tyler Tech NWPS Mobile Implimentation Specialist
    //Designed for 64bit and 32 bit Windows 

    //WIP remaining
    //Show text message if no operation is selected
    //Saving the start up settings and changing the text in the xml

    //Changed .net requirement from 4.6.1 to 4.5 02/05/2018

    partial class Form1 : Form
    {
        XmlDocument StartupSettings = new XmlDocument();
        string SourcePath = @"";
        string TargetPath = @"";
        bool is64bit = false;
        bool installmobile = false;
        bool uninstallmobile = false;
        List<string> serverlocation_text = new List<string>();
        public bool Is64bit { get => is64bit; set => is64bit = value; }
        public bool Installmobile { get => installmobile; set => installmobile = value; }
        public bool Uninstallmobile { get => uninstallmobile; set => uninstallmobile = value; }
        public string ClientPath1 { get => TargetPath; set => TargetPath = value; }
        public string SourcePath1 { get => SourcePath; set => SourcePath = value; }

        //what the form does when it itializes every time.
        private void Form1_Load(object sender, EventArgs e)
        {
            InitialLoadofXML();

            //checks if a directory exists to determin a 64 or 32 bit machine and configures the check boxes accordingly.
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
        }

        //run button
        private void Button2_Click(object sender, EventArgs e)
        {
            //show a message if the user does not select an operation to run. WIP
            if (UninstallMobile.Checked && InstallMobile.Checked == false)
            {
                MessageBox.Show("You did not check Uninstall or install for the mobile client. Please select an operation to run: Uninstall, Install, or both.");
            }

            //run combination mobile uninstall an mobile instll
            if (Combo.Checked && Is64Bit.Checked == true)
            {
                Mobile64Uninstall();

                Mobile64install();

                //MobileRestart();
            }

            //run 64bit installer
            if (Is64Bit.Checked && InstallMobile.Checked == true)
            {

                Mobile64install();

                //MobileRestart();
            }

            //Run 64bit uninstaller
            if (Is64Bit.Checked && UninstallMobile.Checked == true)
            {
                Mobile64Uninstall();

                //MobileRestart();
            }

            //Run combination mobile uninstall and mobile install
            if (Combo.Checked && Is32bit.Checked == true)
            {
                Mobile32Uninstaller();

                Mobile32install();

                //MobileRestart();
            }

            //Run 32bit installer
            if (Is32bit.Checked && InstallMobile.Checked == true)
            {
                Mobile32install();

                //MobileRestart();
            }


            //Run 32bit uninstaller
            if (Is32bit.Checked && UninstallMobile.Checked == true)
            {
                Mobile32Uninstaller();

                //MobileRestart();
            }
        }

        //Copy Button
        private void CopyFiles_Click(object sender, EventArgs e)
        {
            Temp();

            MobileCopy();

            MessageCopy();
        }

        //
        //XML Related information. Broken up between loading prior XML information OR creating a new XML with placeholder server location.
        //
        private void InitialLoadofXML()
        {
            //Checking if the MobileInstallApp.xml exists, and loading the data if it does.
            if (File.Exists("MobileInstallApp.xml"))
            {
                StartupSettings.Load("MobileInstallApp.xml");
                NwsHoldPath.Text = StartupSettings.GetElementsByTagName("SourcePath")[0].InnerText;
                TargetPath = @"C:\Temp\MobileInstaller";
            }
            //Creation of a new MobileInstallApp.xml if one does not already exist.
            else
            {
                SourcePath = NwsHoldPath.Text;

                //root of the XML
                XmlNode root = StartupSettings.CreateElement("root");
                StartupSettings.AppendChild(root);

                //NWS HOLD PATH
                XmlNode NwsPathNode = StartupSettings.CreateElement("SourcePath");
                NwsPathNode.InnerText = @"File Path to Mobile Pre-reqs";
                root.AppendChild(NwsPathNode);

                //Save the start up settings
                StartupSettings.Save("MobileInstallApp.xml");
            }
        }

        //
        //WIP so that when the xml is modified once it is changed for all other subsiquint uses with that xml.
        //
        private void SaveStartupSettings()
        {
            StartupSettings.GetElementsByTagName("SourcePath")[0].InnerText = NwsHoldPath.Text;
            StartupSettings.Save("MobileInstallApp.xml");
        }

        //
        //Mobile 64bit uninstaller
        //
        private void Mobile64Uninstall()
        {
            //Runs a batch file that is configured to Uninstall all pre-reqs that are required for mobile to function properly AND DELETES ALL LOCAL FOLDERS FOR MOBILE.
            Process proc = null;
            try
            {
                string batdir = string.Format(@"C:\Temp\MobileInstaller");
                proc = new Process();
                proc.StartInfo.WorkingDirectory = batdir;
                proc.StartInfo.FileName = "64Uninstaller.bat";
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.Verb = "runas";
                proc.Start();
                proc.WaitForExit();
                MessageBox.Show("64bit Mobile Client is uninstalled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }
        }

        //
        //Mobile 64bit installer
        //
        private void Mobile64install()
        {
            //Reaches out to for a batch file that will install ALL pre-reqs for the mobile client in the correct order, and will force the user to enter in information when configuring the updater for mobile: Server information, client to be installed, and slecting between 64 or 32 bit. 

            Process proc = null;
            try
            {
                string batdir = string.Format(@"C:\Temp\MobileInstaller");
                proc = new Process();
                proc.StartInfo.WorkingDirectory = batdir;
                proc.StartInfo.FileName = "dotNetFx471_Full_setup_Offline.exe";
                proc.StartInfo.Arguments = "/quiet";
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                proc.Start();
                proc.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }

            try
            {
                string batdir = string.Format(@"C:\Temp\MobileInstaller");
                proc = new Process();
                proc.StartInfo.WorkingDirectory = batdir;
                proc.StartInfo.FileName = "SSCERuntime_x86-ENU.msi";
                proc.StartInfo.Arguments = "/quiet";
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                proc.Start();
                proc.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }

            try
            {
                string batdir = string.Format(@"C:\Temp\MobileInstaller");
                proc = new Process();
                proc.StartInfo.WorkingDirectory = batdir;
                proc.StartInfo.FileName = "SSCERuntime_x64-ENU.msi";
                proc.StartInfo.Arguments = "/quiet";
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                proc.Start();
                proc.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }

            try
            {
                string batdir = string.Format(@"C:\Temp\MobileInstaller");
                proc = new Process();
                proc.StartInfo.WorkingDirectory = batdir;
                proc.StartInfo.FileName = "NewWorld.Gis.Components.x86.msi";
                proc.StartInfo.Arguments = "/quiet";
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                proc.Start();
                proc.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }

            try
            {
                string batdir = string.Format(@"C:\Temp\MobileInstaller");
                proc = new Process();
                proc.StartInfo.WorkingDirectory = batdir;
                proc.StartInfo.FileName = "NewWorld.Gis.Components.x64.msi";
                proc.StartInfo.Arguments = "/quiet";
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                proc.Start();
                proc.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }

            try
            {
                string batdir = string.Format(@"C:\Temp\MobileInstaller");
                proc = new Process();
                proc.StartInfo.WorkingDirectory = batdir;
                proc.StartInfo.FileName = "Synchronization-v2.1-x64-ENU.msi";
                proc.StartInfo.Arguments = "/quiet";
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                proc.Start();
                proc.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }

            try
            {
                string batdir = string.Format(@"C:\Temp\MobileInstaller");
                proc = new Process();
                proc.StartInfo.WorkingDirectory = batdir;
                proc.StartInfo.FileName = "ProviderServices-v2.1-x64-ENU.msi";
                proc.StartInfo.Arguments = "/quiet";
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                proc.Start();
                proc.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }

            try
            {
                string batdir = string.Format(@"C:\Temp\MobileInstaller");
                proc = new Process();
                proc.StartInfo.WorkingDirectory = batdir;
                proc.StartInfo.FileName = "DatabaseProviders-v3.1-x64-ENU.msi";
                proc.StartInfo.Arguments = "/quiet";
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                proc.Start();
                proc.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }

            try
            {
                string batdir = string.Format(@"C:\Temp\MobileInstaller");
                proc = new Process();
                proc.StartInfo.WorkingDirectory = batdir;
                proc.StartInfo.FileName = "NewWorld.Management.Updater.msi";
                proc.StartInfo.Arguments = "/quiet";
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                proc.Start();
                proc.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }

            try
            {
                string batdir = string.Format(@"C:\Temp\MobileInstaller");
                proc = new Process();
                proc.StartInfo.WorkingDirectory = batdir;
                proc.StartInfo.FileName = "ConfigureUpdaterformobileV2.exe";
                //proc.StartInfo.Arguments = "/quiet";
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                proc.Start();
                proc.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }

            try
            {
                string batdir = string.Format(@"C:\Temp\MobileInstaller");
                proc = new Process();
                proc.StartInfo.WorkingDirectory = batdir;
                proc.StartInfo.FileName = "64folderprep.bat";
                proc.StartInfo.CreateNoWindow = false;
                proc.StartInfo.Verb = "runas";
                proc.Start();
                proc.WaitForExit();
                MessageBox.Show("64bit Mobile Client is installed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }

        }

        //
        //Mobile copy
        //
        private void MobileCopy()
        {
            //this copies all files within NwsHoldPath.text to C:\Temp\MobileInstaller recursively.
            string SourcePath = NwsHoldPath.Text;
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
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace.ToString());
                    }
                }
            }
        }

        //
        //Mobile 32bit uninstaller
        //
        private void Mobile32Uninstaller()
        {
            //Runs a batch file that is configured to Uninstall all pre-reqs that are required for mobile to function properly AND DELETES ALL LOCAL FOLDERS FOR MOBILE.
            Process proc = null;
            try
            {
                string batdir = string.Format(@"C:\Temp\MobileInstaller");
                proc = new Process();
                proc.StartInfo.WorkingDirectory = batdir;
                proc.StartInfo.FileName = "32Uninstaller.bat";
                proc.StartInfo.CreateNoWindow = false;
                proc.StartInfo.Verb = "runas";
                proc.Start();
                proc.WaitForExit();
                MessageBox.Show("64bit Mobile Client is uninstalled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }
        }

        //
        //Mobile 32bit Installer
        //
        private void Mobile32install()
        {
            try
            {
                //Reaches out to for a batch file that will install ALL pre-reqs for the mobile client in the correct order, and will force the user to enter in information when configuring the updater for mobile: Server information, client to be installed, and slecting between 64 or 32 bit.
                Process proc = null;
                try
                {
                    string batdir = string.Format(@"C:\Temp\MobileInstaller");
                    proc = new Process();
                    proc.StartInfo.WorkingDirectory = batdir;
                    proc.StartInfo.FileName = "32Installer.bat";
                    proc.StartInfo.CreateNoWindow = false;
                    proc.StartInfo.Verb = "runas";
                    proc.Start();
                    proc.WaitForExit();
                    MessageBox.Show("64bit Mobile Client is installed.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }
        }
        
        //
        //Temp file Creation, MobileInstaller Creation, Temp file cleaning on button click - Created on 02/01
        //
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

        //
        //Help button logic on button click
        //
        private void Button3_Click(object sender, EventArgs e)
        {
            var message = "Help Button \n\n";
            message += "If you didn't run this as Administrator close and re-run as Admin (Right click run as admin.) \n\n";
            message += "1. Fill in the file path from this computer to the mobile server ->NWSHOLD folder ->Mobile Installer App ->Pre-Reqs. \n";
            message += "2. Select the specific operation you'd like to perform. \n";
            message += "3. Uninstalling the mobile client will uninstall the mobile client AND delete all files related to the mobile client. \n";
            message += "4. Installing the mobile client will install all pre-regs necessary to run the mobile client. \n";
            message += "5. Selecting the Uninstal and Install functions and hitting the run button will uninstal the mobile client -> delete files -> install the mobile client. \n";
            message += "6. The copy button MUST BE RUN FIRST. The copy button copies the files neccesary to installer and/or uninstall the mobile client. \n\n";
            MessageBox.Show(message);
        }

        private void MessageCopy()
        {
            MessageBox.Show("The files are copied. Please press the run button.");
        }

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

            Process.Start("Shutdown", "/r");

            Application.Exit();
        }

        //Below is unused button click/text changed 
        private void CheckBox3_CheckedChanged(object sender, EventArgs e)
        {

        }
        
        private void CheckBox4_CheckedChanged(object sender, EventArgs e)
        {

        }
        
        private void CheckBox5_CheckedChanged(object sender, EventArgs e)
        {

        }
        
        private void Serverlocation_txt_TextChanged(object sender, EventArgs e)
        {

        }

        private void ProgressBar1_Click(object sender, EventArgs e)
        {

        }
        
    }
}