namespace EasyLinkGui {
    partial class ReportForm {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportForm));
            this.printPreviewControl1 = new System.Windows.Forms.PrintPreviewControl();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPreview = new System.Windows.Forms.TabPage();
            this.bPrint = new System.Windows.Forms.Button();
            this.tabEasyBuild = new System.Windows.Forms.TabPage();
            this.pbQrcode = new System.Windows.Forms.PictureBox();
            this.tabRequire = new System.Windows.Forms.TabPage();
            this.olvRequire = new BrightIdeasSoftware.ObjectListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvRequireQuantity = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ColumnSubName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.tabDestroy = new System.Windows.Forms.TabPage();
            this.olvDestroy = new BrightIdeasSoftware.ObjectListView();
            this.olvDeleteColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn15 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn4 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.tabLink = new System.Windows.Forms.TabPage();
            this.olvLinks = new BrightIdeasSoftware.ObjectListView();
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn7 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.listViewPrinter1 = new BrightIdeasSoftware.ListViewPrinter();
            this.printPreviewDialog1 = new System.Windows.Forms.PrintPreviewDialog();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.lProxyStatus = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPreview.SuspendLayout();
            this.tabEasyBuild.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbQrcode)).BeginInit();
            this.tabRequire.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvRequire)).BeginInit();
            this.tabDestroy.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvDestroy)).BeginInit();
            this.tabLink.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvLinks)).BeginInit();
            this.SuspendLayout();
            // 
            // printPreviewControl1
            // 
            this.printPreviewControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.printPreviewControl1.AutoZoom = false;
            this.printPreviewControl1.Columns = 3;
            this.printPreviewControl1.Location = new System.Drawing.Point(0, 61);
            this.printPreviewControl1.Name = "printPreviewControl1";
            this.printPreviewControl1.Rows = 2;
            this.printPreviewControl1.Size = new System.Drawing.Size(768, 339);
            this.printPreviewControl1.TabIndex = 0;
            this.printPreviewControl1.UseAntiAlias = true;
            this.printPreviewControl1.Zoom = 1D;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPreview);
            this.tabControl1.Controls.Add(this.tabEasyBuild);
            this.tabControl1.Controls.Add(this.tabRequire);
            this.tabControl1.Controls.Add(this.tabDestroy);
            this.tabControl1.Controls.Add(this.tabLink);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(776, 426);
            this.tabControl1.TabIndex = 1;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPreview
            // 
            this.tabPreview.Controls.Add(this.bPrint);
            this.tabPreview.Controls.Add(this.printPreviewControl1);
            this.tabPreview.Location = new System.Drawing.Point(4, 22);
            this.tabPreview.Name = "tabPreview";
            this.tabPreview.Padding = new System.Windows.Forms.Padding(3);
            this.tabPreview.Size = new System.Drawing.Size(768, 400);
            this.tabPreview.TabIndex = 3;
            this.tabPreview.Text = "Print preview";
            this.tabPreview.UseVisualStyleBackColor = true;
            // 
            // bPrint
            // 
            this.bPrint.Location = new System.Drawing.Point(6, 6);
            this.bPrint.Name = "bPrint";
            this.bPrint.Size = new System.Drawing.Size(75, 23);
            this.bPrint.TabIndex = 1;
            this.bPrint.Text = "Print";
            this.bPrint.UseVisualStyleBackColor = true;
            this.bPrint.Click += new System.EventHandler(this.bPrint_Click);
            // 
            // tabEasyBuild
            // 
            this.tabEasyBuild.Controls.Add(this.lProxyStatus);
            this.tabEasyBuild.Controls.Add(this.pbQrcode);
            this.tabEasyBuild.Location = new System.Drawing.Point(4, 22);
            this.tabEasyBuild.Name = "tabEasyBuild";
            this.tabEasyBuild.Padding = new System.Windows.Forms.Padding(3);
            this.tabEasyBuild.Size = new System.Drawing.Size(768, 400);
            this.tabEasyBuild.TabIndex = 4;
            this.tabEasyBuild.Text = "EasyBuild";
            this.tabEasyBuild.UseVisualStyleBackColor = true;
            // 
            // pbQrcode
            // 
            this.pbQrcode.Location = new System.Drawing.Point(6, 43);
            this.pbQrcode.Name = "pbQrcode";
            this.pbQrcode.Size = new System.Drawing.Size(321, 321);
            this.pbQrcode.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbQrcode.TabIndex = 0;
            this.pbQrcode.TabStop = false;
            // 
            // tabRequire
            // 
            this.tabRequire.Controls.Add(this.olvRequire);
            this.tabRequire.Location = new System.Drawing.Point(4, 22);
            this.tabRequire.Name = "tabRequire";
            this.tabRequire.Padding = new System.Windows.Forms.Padding(3);
            this.tabRequire.Size = new System.Drawing.Size(768, 400);
            this.tabRequire.TabIndex = 0;
            this.tabRequire.Text = "Require";
            this.tabRequire.UseVisualStyleBackColor = true;
            // 
            // olvRequire
            // 
            this.olvRequire.AllColumns.Add(this.olvColumn1);
            this.olvRequire.AllColumns.Add(this.olvRequireQuantity);
            this.olvRequire.AllColumns.Add(this.ColumnSubName);
            this.olvRequire.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.SingleClick;
            this.olvRequire.CheckedAspectName = "";
            this.olvRequire.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvRequireQuantity,
            this.ColumnSubName});
            this.olvRequire.Dock = System.Windows.Forms.DockStyle.Fill;
            this.olvRequire.Location = new System.Drawing.Point(3, 3);
            this.olvRequire.Name = "olvRequire";
            this.olvRequire.ShowImagesOnSubItems = true;
            this.olvRequire.Size = new System.Drawing.Size(762, 394);
            this.olvRequire.SortGroupItemsByPrimaryColumn = false;
            this.olvRequire.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.olvRequire.TabIndex = 23;
            this.olvRequire.UseCompatibleStateImageBehavior = false;
            this.olvRequire.UseFiltering = true;
            this.olvRequire.View = System.Windows.Forms.View.Details;
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "ItemName";
            this.olvColumn1.AspectToStringFormat = "";
            this.olvColumn1.CellPadding = null;
            this.olvColumn1.MinimumWidth = 100;
            this.olvColumn1.Text = "Item";
            this.olvColumn1.Width = 100;
            // 
            // olvRequireQuantity
            // 
            this.olvRequireQuantity.AspectName = "Quantity";
            this.olvRequireQuantity.CellPadding = null;
            this.olvRequireQuantity.Groupable = false;
            this.olvRequireQuantity.IsEditable = false;
            this.olvRequireQuantity.MinimumWidth = 100;
            this.olvRequireQuantity.Text = "Quantity";
            this.olvRequireQuantity.Width = 100;
            // 
            // ColumnSubName
            // 
            this.ColumnSubName.AspectName = "Subname";
            this.ColumnSubName.CellPadding = null;
            this.ColumnSubName.MinimumWidth = 300;
            this.ColumnSubName.Text = "Subname";
            this.ColumnSubName.Width = 300;
            // 
            // tabDestroy
            // 
            this.tabDestroy.Controls.Add(this.olvDestroy);
            this.tabDestroy.Location = new System.Drawing.Point(4, 22);
            this.tabDestroy.Name = "tabDestroy";
            this.tabDestroy.Padding = new System.Windows.Forms.Padding(3);
            this.tabDestroy.Size = new System.Drawing.Size(768, 400);
            this.tabDestroy.TabIndex = 1;
            this.tabDestroy.Text = "Destroy";
            this.tabDestroy.UseVisualStyleBackColor = true;
            // 
            // olvDestroy
            // 
            this.olvDestroy.AllColumns.Add(this.olvDeleteColumn2);
            this.olvDestroy.AllColumns.Add(this.olvColumn15);
            this.olvDestroy.AllColumns.Add(this.olvColumn3);
            this.olvDestroy.AllColumns.Add(this.olvColumn4);
            this.olvDestroy.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.SingleClick;
            this.olvDestroy.CheckedAspectName = "";
            this.olvDestroy.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvDeleteColumn2,
            this.olvColumn15,
            this.olvColumn3,
            this.olvColumn4});
            this.olvDestroy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.olvDestroy.Location = new System.Drawing.Point(3, 3);
            this.olvDestroy.Name = "olvDestroy";
            this.olvDestroy.ShowImagesOnSubItems = true;
            this.olvDestroy.Size = new System.Drawing.Size(762, 394);
            this.olvDestroy.SortGroupItemsByPrimaryColumn = false;
            this.olvDestroy.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.olvDestroy.TabIndex = 22;
            this.olvDestroy.UseCompatibleStateImageBehavior = false;
            this.olvDestroy.UseFiltering = true;
            this.olvDestroy.View = System.Windows.Forms.View.Details;
            // 
            // olvDeleteColumn2
            // 
            this.olvDeleteColumn2.AspectName = "Team";
            this.olvDeleteColumn2.AspectToStringFormat = "";
            this.olvDeleteColumn2.CellPadding = null;
            this.olvDeleteColumn2.MinimumWidth = 100;
            this.olvDeleteColumn2.Text = "Team";
            this.olvDeleteColumn2.Width = 100;
            // 
            // olvColumn15
            // 
            this.olvColumn15.AspectName = "Name";
            this.olvColumn15.CellPadding = null;
            this.olvColumn15.Groupable = false;
            this.olvColumn15.IsEditable = false;
            this.olvColumn15.MinimumWidth = 300;
            this.olvColumn15.Text = "Portal";
            this.olvColumn15.Width = 300;
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "Village";
            this.olvColumn3.CellPadding = null;
            this.olvColumn3.Text = "Ort";
            this.olvColumn3.Width = 150;
            // 
            // olvColumn4
            // 
            this.olvColumn4.AspectName = "Suburb";
            this.olvColumn4.CellPadding = null;
            this.olvColumn4.Text = "Sub";
            this.olvColumn4.Width = 150;
            // 
            // tabLink
            // 
            this.tabLink.Controls.Add(this.olvLinks);
            this.tabLink.Location = new System.Drawing.Point(4, 22);
            this.tabLink.Name = "tabLink";
            this.tabLink.Padding = new System.Windows.Forms.Padding(3);
            this.tabLink.Size = new System.Drawing.Size(768, 400);
            this.tabLink.TabIndex = 2;
            this.tabLink.Text = "Link";
            this.tabLink.UseVisualStyleBackColor = true;
            // 
            // olvLinks
            // 
            this.olvLinks.AllColumns.Add(this.olvColumn2);
            this.olvLinks.AllColumns.Add(this.olvColumn7);
            this.olvLinks.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.SingleClick;
            this.olvLinks.CheckedAspectName = "";
            this.olvLinks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn2,
            this.olvColumn7});
            this.olvLinks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.olvLinks.Location = new System.Drawing.Point(3, 3);
            this.olvLinks.Name = "olvLinks";
            this.olvLinks.ShowGroups = false;
            this.olvLinks.ShowImagesOnSubItems = true;
            this.olvLinks.Size = new System.Drawing.Size(762, 394);
            this.olvLinks.TabIndex = 22;
            this.olvLinks.UseCompatibleStateImageBehavior = false;
            this.olvLinks.UseFiltering = true;
            this.olvLinks.View = System.Windows.Forms.View.Details;
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "P1.Name";
            this.olvColumn2.CellPadding = null;
            this.olvColumn2.Groupable = false;
            this.olvColumn2.IsEditable = false;
            this.olvColumn2.MinimumWidth = 300;
            this.olvColumn2.Sortable = false;
            this.olvColumn2.Text = "Origin";
            this.olvColumn2.Width = 302;
            // 
            // olvColumn7
            // 
            this.olvColumn7.AspectName = "P2.Name";
            this.olvColumn7.AspectToStringFormat = "";
            this.olvColumn7.CellPadding = null;
            this.olvColumn7.MinimumWidth = 300;
            this.olvColumn7.Sortable = false;
            this.olvColumn7.Text = "Destination";
            this.olvColumn7.Width = 300;
            // 
            // listViewPrinter1
            // 
            // 
            // 
            // 
            this.listViewPrinter1.CellFormat.CanWrap = true;
            this.listViewPrinter1.CellFormat.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // 
            // 
            this.listViewPrinter1.FooterFormat.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Italic);
            // 
            // 
            // 
            this.listViewPrinter1.GroupHeaderFormat.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Bold);
            // 
            // 
            // 
            this.listViewPrinter1.HeaderFormat.Font = new System.Drawing.Font("Verdana", 24F);
            // 
            // 
            // 
            this.listViewPrinter1.ListHeaderFormat.CanWrap = true;
            this.listViewPrinter1.ListHeaderFormat.Font = new System.Drawing.Font("Verdana", 12F);
            this.listViewPrinter1.ListView = this.olvLinks;
            // 
            // printPreviewDialog1
            // 
            this.printPreviewDialog1.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.printPreviewDialog1.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.printPreviewDialog1.ClientSize = new System.Drawing.Size(400, 300);
            this.printPreviewDialog1.Enabled = true;
            this.printPreviewDialog1.Icon = ((System.Drawing.Icon)(resources.GetObject("printPreviewDialog1.Icon")));
            this.printPreviewDialog1.Name = "printPreviewDialog1";
            this.printPreviewDialog1.Visible = false;
            // 
            // printDialog1
            // 
            this.printDialog1.UseEXDialog = true;
            // 
            // lProxyStatus
            // 
            this.lProxyStatus.AutoSize = true;
            this.lProxyStatus.Location = new System.Drawing.Point(6, 3);
            this.lProxyStatus.Name = "lProxyStatus";
            this.lProxyStatus.Size = new System.Drawing.Size(35, 13);
            this.lProxyStatus.TabIndex = 1;
            this.lProxyStatus.Text = "label1";
            // 
            // ReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tabControl1);
            this.Name = "ReportForm";
            this.Text = "ReportForm";
            this.Load += new System.EventHandler(this.ReportForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPreview.ResumeLayout(false);
            this.tabEasyBuild.ResumeLayout(false);
            this.tabEasyBuild.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbQrcode)).EndInit();
            this.tabRequire.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.olvRequire)).EndInit();
            this.tabDestroy.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.olvDestroy)).EndInit();
            this.tabLink.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.olvLinks)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PrintPreviewControl printPreviewControl1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabRequire;
        private System.Windows.Forms.TabPage tabDestroy;
        private System.Windows.Forms.TabPage tabLink;
        private BrightIdeasSoftware.ObjectListView olvLinks;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private BrightIdeasSoftware.OLVColumn olvColumn7;
        private BrightIdeasSoftware.ListViewPrinter listViewPrinter1;
        private System.Windows.Forms.TabPage tabPreview;
        private BrightIdeasSoftware.ObjectListView olvDestroy;
        private BrightIdeasSoftware.OLVColumn olvDeleteColumn2;
        private BrightIdeasSoftware.OLVColumn olvColumn15;
        private BrightIdeasSoftware.ObjectListView olvRequire;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvRequireQuantity;
        private BrightIdeasSoftware.OLVColumn ColumnSubName;
        private System.Windows.Forms.Button bPrint;
        private System.Windows.Forms.PrintPreviewDialog printPreviewDialog1;
        private System.Windows.Forms.PrintDialog printDialog1;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private BrightIdeasSoftware.OLVColumn olvColumn4;
        private System.Windows.Forms.TabPage tabEasyBuild;
        private System.Windows.Forms.PictureBox pbQrcode;
        private System.Windows.Forms.Label lProxyStatus;
    }
}