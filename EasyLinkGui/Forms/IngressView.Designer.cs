namespace EasyLinkGui {
    partial class IngressView {
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
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pWebView = new System.Windows.Forms.Panel();
            this.cbLoadPortals = new System.Windows.Forms.CheckBox();
            this.cbLoadLinks = new System.Windows.Forms.CheckBox();
            this.bSubmitPasscode = new System.Windows.Forms.Button();
            this.tbPasscode = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // pWebView
            // 
            this.pWebView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pWebView.Location = new System.Drawing.Point(12, 43);
            this.pWebView.Name = "pWebView";
            this.pWebView.Size = new System.Drawing.Size(537, 311);
            this.pWebView.TabIndex = 0;
            // 
            // cbLoadPortals
            // 
            this.cbLoadPortals.AutoSize = true;
            this.cbLoadPortals.Location = new System.Drawing.Point(12, 12);
            this.cbLoadPortals.Name = "cbLoadPortals";
            this.cbLoadPortals.Size = new System.Drawing.Size(84, 17);
            this.cbLoadPortals.TabIndex = 1;
            this.cbLoadPortals.Text = "Load portals";
            this.cbLoadPortals.UseVisualStyleBackColor = true;
            // 
            // cbLoadLinks
            // 
            this.cbLoadLinks.AutoSize = true;
            this.cbLoadLinks.Location = new System.Drawing.Point(102, 12);
            this.cbLoadLinks.Name = "cbLoadLinks";
            this.cbLoadLinks.Size = new System.Drawing.Size(74, 17);
            this.cbLoadLinks.TabIndex = 1;
            this.cbLoadLinks.Text = "Load links";
            this.cbLoadLinks.UseVisualStyleBackColor = true;
            // 
            // bSubmitPasscode
            // 
            this.bSubmitPasscode.Location = new System.Drawing.Point(399, 13);
            this.bSubmitPasscode.Name = "bSubmitPasscode";
            this.bSubmitPasscode.Size = new System.Drawing.Size(75, 23);
            this.bSubmitPasscode.TabIndex = 2;
            this.bSubmitPasscode.Text = "button1";
            this.bSubmitPasscode.UseVisualStyleBackColor = true;
            this.bSubmitPasscode.Visible = false;
            this.bSubmitPasscode.Click += new System.EventHandler(this.bSubmitPasscode_Click);
            // 
            // tbPasscode
            // 
            this.tbPasscode.Location = new System.Drawing.Point(293, 15);
            this.tbPasscode.Name = "tbPasscode";
            this.tbPasscode.Size = new System.Drawing.Size(100, 20);
            this.tbPasscode.TabIndex = 3;
            this.tbPasscode.Text = "2d8zaxjwl2";
            this.tbPasscode.Visible = false;
            // 
            // IngressView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(561, 366);
            this.Controls.Add(this.tbPasscode);
            this.Controls.Add(this.bSubmitPasscode);
            this.Controls.Add(this.cbLoadLinks);
            this.Controls.Add(this.cbLoadPortals);
            this.Controls.Add(this.pWebView);
            this.Name = "IngressView";
            this.Text = "MapView";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.IngressView_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel pWebView;
        private System.Windows.Forms.CheckBox cbLoadPortals;
        private System.Windows.Forms.CheckBox cbLoadLinks;
        private System.Windows.Forms.Button bSubmitPasscode;
        private System.Windows.Forms.TextBox tbPasscode;
    }
}