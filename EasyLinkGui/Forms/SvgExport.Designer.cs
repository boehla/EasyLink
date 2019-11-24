namespace EasyLinkGui.Forms {
    partial class SvgExport {
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbGameLinks = new System.Windows.Forms.CheckBox();
            this.cbExternLinks = new System.Windows.Forms.CheckBox();
            this.bCancel = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.cbGameLinks);
            this.groupBox1.Controls.Add(this.cbExternLinks);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(197, 73);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Export Settings";
            // 
            // cbGameLinks
            // 
            this.cbGameLinks.AutoSize = true;
            this.cbGameLinks.Checked = true;
            this.cbGameLinks.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbGameLinks.Location = new System.Drawing.Point(6, 42);
            this.cbGameLinks.Name = "cbGameLinks";
            this.cbGameLinks.Size = new System.Drawing.Size(78, 17);
            this.cbGameLinks.TabIndex = 0;
            this.cbGameLinks.Text = "Game links";
            this.cbGameLinks.UseVisualStyleBackColor = true;
            // 
            // cbExternLinks
            // 
            this.cbExternLinks.AutoSize = true;
            this.cbExternLinks.Checked = true;
            this.cbExternLinks.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbExternLinks.Location = new System.Drawing.Point(6, 19);
            this.cbExternLinks.Name = "cbExternLinks";
            this.cbExternLinks.Size = new System.Drawing.Size(80, 17);
            this.cbExternLinks.TabIndex = 0;
            this.cbExternLinks.Text = "Extern links";
            this.cbExternLinks.UseVisualStyleBackColor = true;
            // 
            // bCancel
            // 
            this.bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bCancel.Location = new System.Drawing.Point(134, 91);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(75, 23);
            this.bCancel.TabIndex = 1;
            this.bCancel.Text = "Cancel";
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Click += new System.EventHandler(this.BCancel_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.Location = new System.Drawing.Point(12, 91);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Export";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // SvgExport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(223, 126);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.groupBox1);
            this.Name = "SvgExport";
            this.Text = "SvgExport";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cbGameLinks;
        private System.Windows.Forms.CheckBox cbExternLinks;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.Button button2;
    }
}