namespace EasyLinkGui {
    partial class BaseSettingsForm {
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
            this.pbSettings = new System.Windows.Forms.PropertyGrid();
            this.bOk = new System.Windows.Forms.Button();
            this.bAccept = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // pbSettings
            // 
            this.pbSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbSettings.Location = new System.Drawing.Point(0, 0);
            this.pbSettings.Name = "pbSettings";
            this.pbSettings.Size = new System.Drawing.Size(532, 368);
            this.pbSettings.TabIndex = 0;
            // 
            // bOk
            // 
            this.bOk.Location = new System.Drawing.Point(445, 374);
            this.bOk.Name = "bOk";
            this.bOk.Size = new System.Drawing.Size(75, 23);
            this.bOk.TabIndex = 1;
            this.bOk.Text = "Ok";
            this.bOk.UseVisualStyleBackColor = true;
            this.bOk.Click += new System.EventHandler(this.BOk_Click);
            // 
            // bAccept
            // 
            this.bAccept.Location = new System.Drawing.Point(364, 374);
            this.bAccept.Name = "bAccept";
            this.bAccept.Size = new System.Drawing.Size(75, 23);
            this.bAccept.TabIndex = 1;
            this.bAccept.Text = "Accept";
            this.bAccept.UseVisualStyleBackColor = true;
            // 
            // BaseSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 401);
            this.Controls.Add(this.bAccept);
            this.Controls.Add(this.bOk);
            this.Controls.Add(this.pbSettings);
            this.Name = "BaseSettingsForm";
            this.Text = "SettingsForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid pbSettings;
        private System.Windows.Forms.Button bOk;
        private System.Windows.Forms.Button bAccept;
    }
}