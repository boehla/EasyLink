namespace EasyLinkGui {
    partial class AboutForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.llGithubLink = new System.Windows.Forms.LinkLabel();
            this.lVersion = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // llGithubLink
            // 
            this.llGithubLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.llGithubLink.AutoSize = true;
            this.llGithubLink.Location = new System.Drawing.Point(608, 428);
            this.llGithubLink.Name = "llGithubLink";
            this.llGithubLink.Size = new System.Drawing.Size(180, 13);
            this.llGithubLink.TabIndex = 0;
            this.llGithubLink.TabStop = true;
            this.llGithubLink.Text = "https://github.com/boehla/EasyLink";
            this.llGithubLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llGithubLink_LinkClicked);
            // 
            // lVersion
            // 
            this.lVersion.AutoSize = true;
            this.lVersion.Location = new System.Drawing.Point(12, 9);
            this.lVersion.Name = "lVersion";
            this.lVersion.Size = new System.Drawing.Size(35, 13);
            this.lVersion.TabIndex = 1;
            this.lVersion.Text = "label1";
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(12, 428);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(254, 13);
            this.linkLabel1.TabIndex = 0;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "https://t.me/joinchat/AExhwBZUJ2BjAUEocRm9xQ";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llGithubLink_LinkClicked);
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lVersion);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.llGithubLink);
            this.Name = "AboutForm";
            this.Text = "AboutForm";
            this.Load += new System.EventHandler(this.AboutForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel llGithubLink;
        private System.Windows.Forms.Label lVersion;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}