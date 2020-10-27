using System;
using System.IO;
using System.Windows.Forms;

namespace MobileInstallApp
{
    public partial class NWPSADDONDOWNLOAD : Form
    {
        private ToolStripLabel ts1 = new ToolStripLabel();

        private void Form2_Load(object sender, EventArgs e)
        {
            //string LogEntry = @"----------- Start of Sub log file" + " " + DateTime.Now + "-----------";

            StatusStrip1.Items.AddRange(new ToolStripItem[] { ts1 });
            ts1.Text = "Ready";
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public NWPSADDONDOWNLOAD()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
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

            string LogEntry = DateTime.Now + " " + SourcePath + " Folder " + "has been copied.";

            LogEntryWriter(LogEntry);

            ts1.Text = "Copy Complete. Please Close Window and Press the Run button again.";
        }

        //this writes to the mobile pre req installer logfile
        private void LogEntryWriter(string LogEntry)
        {
            using (StreamWriter file = new StreamWriter(("NWPSAPPLog.txt"), true))
            {
                file.WriteLine(LogEntry);
            }
        }
    }
}