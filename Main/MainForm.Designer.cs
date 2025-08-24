namespace ThunderClientPatcher
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btnPatch = new System.Windows.Forms.Button();
            this.lbName = new System.Windows.Forms.Label();
            this.lbStatus = new System.Windows.Forms.Label();
            this.lbVersion = new System.Windows.Forms.Label();
            this.btnRestore = new System.Windows.Forms.Button();
            this.lbMyRepo = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // btnPatch
            // 
            this.btnPatch.Enabled = false;
            this.btnPatch.Location = new System.Drawing.Point(312, 13);
            this.btnPatch.Name = "btnPatch";
            this.btnPatch.Size = new System.Drawing.Size(86, 42);
            this.btnPatch.TabIndex = 0;
            this.btnPatch.Text = "Patch";
            this.btnPatch.UseVisualStyleBackColor = true;
            this.btnPatch.Click += new System.EventHandler(this.btnPatch_Click);
            // 
            // lbName
            // 
            this.lbName.AutoSize = true;
            this.lbName.Location = new System.Drawing.Point(14, 9);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(161, 18);
            this.lbName.TabIndex = 1;
            this.lbName.Text = "Name :  Thunder Client";
            // 
            // lbStatus
            // 
            this.lbStatus.AutoSize = true;
            this.lbStatus.Location = new System.Drawing.Point(14, 106);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(64, 18);
            this.lbStatus.TabIndex = 2;
            this.lbStatus.Text = "Status : ";
            // 
            // lbVersion
            // 
            this.lbVersion.AutoSize = true;
            this.lbVersion.Location = new System.Drawing.Point(14, 41);
            this.lbVersion.Name = "lbVersion";
            this.lbVersion.Size = new System.Drawing.Size(65, 18);
            this.lbVersion.TabIndex = 3;
            this.lbVersion.Text = "Version :";
            // 
            // btnRestore
            // 
            this.btnRestore.Enabled = false;
            this.btnRestore.Location = new System.Drawing.Point(312, 61);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(86, 42);
            this.btnRestore.TabIndex = 4;
            this.btnRestore.Text = "Restore";
            this.btnRestore.UseVisualStyleBackColor = true;
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // lbMyRepo
            // 
            this.lbMyRepo.AutoSize = true;
            this.lbMyRepo.Location = new System.Drawing.Point(14, 75);
            this.lbMyRepo.Name = "lbMyRepo";
            this.lbMyRepo.Size = new System.Drawing.Size(119, 18);
            this.lbMyRepo.TabIndex = 5;
            this.lbMyRepo.TabStop = true;
            this.lbMyRepo.Text = "Github : @RE0x0";
            this.lbMyRepo.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbMyRepo_LinkClicked);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 138);
            this.Controls.Add(this.lbMyRepo);
            this.Controls.Add(this.btnRestore);
            this.Controls.Add(this.lbVersion);
            this.Controls.Add(this.lbStatus);
            this.Controls.Add(this.lbName);
            this.Controls.Add(this.btnPatch);
            this.Font = new System.Drawing.Font("Tahoma", 8.5F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Opacity = 0.94D;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ThunderClientPatcher ==RE0x0==";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPatch;
        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.Label lbVersion;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.LinkLabel lbMyRepo;
    }
}

