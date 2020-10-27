namespace MobileInstallApp
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    partial class NWPSADDONDOWNLOAD
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NWPSADDONDOWNLOAD));
            this.label1 = new System.Windows.Forms.Label();
            this.CopyButton = new System.Windows.Forms.Button();
            this.MSPServerPath = new System.Windows.Forms.TextBox();
            this.StatusStrip1 = new System.Windows.Forms.StatusStrip();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(155, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "File Path to NWS Addon Folder";
            // 
            // CopyButton
            // 
            this.CopyButton.Location = new System.Drawing.Point(287, 23);
            this.CopyButton.Name = "CopyButton";
            this.CopyButton.Size = new System.Drawing.Size(75, 27);
            this.CopyButton.TabIndex = 17;
            this.CopyButton.Text = "Copy";
            this.CopyButton.UseVisualStyleBackColor = true;
            this.CopyButton.Click += new System.EventHandler(this.CopyButton_Click);
            // 
            // MSPServerPath
            // 
            this.MSPServerPath.Location = new System.Drawing.Point(12, 23);
            this.MSPServerPath.Multiline = true;
            this.MSPServerPath.Name = "MSPServerPath";
            this.MSPServerPath.Size = new System.Drawing.Size(269, 36);
            this.MSPServerPath.TabIndex = 19;
            this.MSPServerPath.Text = "\\\\MSPServerName\\";
            // 
            // StatusStrip1
            // 
            this.StatusStrip1.Location = new System.Drawing.Point(0, 69);
            this.StatusStrip1.Name = "StatusStrip1";
            this.StatusStrip1.Size = new System.Drawing.Size(417, 22);
            this.StatusStrip1.TabIndex = 20;
            this.StatusStrip1.Text = "statusStrip1";
            // 
            // NWPSADDONDOWNLOAD
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(417, 91);
            this.Controls.Add(this.StatusStrip1);
            this.Controls.Add(this.MSPServerPath);
            this.Controls.Add(this.CopyButton);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NWPSADDONDOWNLOAD";
            this.Text = "NWS Addon Download";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button CopyButton;
        private System.Windows.Forms.TextBox MSPServerPath;
        private System.Windows.Forms.StatusStrip StatusStrip1;
    }
}