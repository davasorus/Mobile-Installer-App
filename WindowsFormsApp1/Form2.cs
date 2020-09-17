using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobileInstallApp
{
    public partial class Form2 : Form
    {
        private ToolStripLabel ts = new ToolStripLabel();

        private void Form2_Load(object sender, EventArgs e)
        {
        }

        public Form2()
        {
            InitializeComponent();
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            MobileCopy1(MSPServerPath.Text);
        }

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

            ts.Text = "Copy Complete";
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