namespace EasyLinkGui {
    partial class MainForm {
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsslMousePosition = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslTotalTested = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.bCalc = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.bShowParent = new System.Windows.Forms.Button();
            this.tbGameInfos = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.portaldatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.loadFromIntelMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.clearAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearDisabledToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showDebugFormToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.infoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.lActivceCount = new System.Windows.Forms.Label();
            this.olvGroup = new BrightIdeasSoftware.ObjectListView();
            this.olvDeleteColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnGroupName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.bLoadGroup = new System.Windows.Forms.Button();
            this.bSaveGroup = new System.Windows.Forms.Button();
            this.olvPortals = new BrightIdeasSoftware.FastObjectListView();
            this.olvEnabledColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn16 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn18 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnGeo = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn8 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn9 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn10 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn11 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn12 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn13 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn14 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn5 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn4 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn17 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.bDbInvert = new System.Windows.Forms.Button();
            this.bDbEnable = new System.Windows.Forms.Button();
            this.bDbDisable = new System.Windows.Forms.Button();
            this.tbDbSearch = new System.Windows.Forms.TextBox();
            this.tabMap = new System.Windows.Forms.TabPage();
            this.gmap = new GMap.NET.WindowsForms.GMapControl();
            this.cbShowLastHandled = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.nudThreadCount = new System.Windows.Forms.NumericUpDown();
            this.bCreateGameState = new System.Windows.Forms.Button();
            this.bCalcStop = new System.Windows.Forms.Button();
            this.timerFast = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.olvAnchors = new BrightIdeasSoftware.ObjectListView();
            this.olvDeleteColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn6 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabAnchors = new System.Windows.Forms.TabPage();
            this.tabLinks = new System.Windows.Forms.TabPage();
            this.olvLinks = new BrightIdeasSoftware.ObjectListView();
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn7 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.tabDestroyPortals = new System.Windows.Forms.TabPage();
            this.lDestroyStatus = new System.Windows.Forms.Label();
            this.olvDestroy = new BrightIdeasSoftware.ObjectListView();
            this.olvDeleteColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn15 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.listViewPrinter1 = new BrightIdeasSoftware.ListViewPrinter();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.tcMain.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvGroup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.olvPortals)).BeginInit();
            this.tabMap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudThreadCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.olvAnchors)).BeginInit();
            this.tabControl2.SuspendLayout();
            this.tabAnchors.SuspendLayout();
            this.tabLinks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvLinks)).BeginInit();
            this.tabDestroyPortals.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvDestroy)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslMousePosition,
            this.tsslTotalTested,
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 501);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1079, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsslMousePosition
            // 
            this.tsslMousePosition.Name = "tsslMousePosition";
            this.tsslMousePosition.Size = new System.Drawing.Size(118, 17);
            this.tsslMousePosition.Text = "toolStripStatusLabel1";
            // 
            // tsslTotalTested
            // 
            this.tsslTotalTested.Name = "tsslTotalTested";
            this.tsslTotalTested.Size = new System.Drawing.Size(118, 17);
            this.tsslTotalTested.Text = "toolStripStatusLabel1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // bCalc
            // 
            this.bCalc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bCalc.Location = new System.Drawing.Point(520, 6);
            this.bCalc.Name = "bCalc";
            this.bCalc.Size = new System.Drawing.Size(75, 23);
            this.bCalc.TabIndex = 5;
            this.bCalc.Text = "Auto link";
            this.bCalc.UseVisualStyleBackColor = true;
            this.bCalc.Click += new System.EventHandler(this.bCalc_Click);
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // bShowParent
            // 
            this.bShowParent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bShowParent.Location = new System.Drawing.Point(682, 6);
            this.bShowParent.Name = "bShowParent";
            this.bShowParent.Size = new System.Drawing.Size(88, 24);
            this.bShowParent.TabIndex = 8;
            this.bShowParent.Text = "Show parent";
            this.bShowParent.UseVisualStyleBackColor = true;
            this.bShowParent.Click += new System.EventHandler(this.bShowParent_Click);
            // 
            // tbGameInfos
            // 
            this.tbGameInfos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tbGameInfos.Location = new System.Drawing.Point(798, 305);
            this.tbGameInfos.Multiline = true;
            this.tbGameInfos.Name = "tbGameInfos";
            this.tbGameInfos.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbGameInfos.Size = new System.Drawing.Size(269, 190);
            this.tbGameInfos.TabIndex = 7;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.portaldatabaseToolStripMenuItem,
            this.reportToolStripMenuItem,
            this.debugToolStripMenuItem,
            this.infoToolStripMenuItem,
            this.toolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1079, 24);
            this.menuStrip1.TabIndex = 10;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.editToolStripMenuItem.Text = "Edit..";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // portaldatabaseToolStripMenuItem
            // 
            this.portaldatabaseToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.loadFromIntelMapToolStripMenuItem,
            this.toolStripSeparator2,
            this.toolStripSeparator3,
            this.clearAllToolStripMenuItem,
            this.clearDisabledToolStripMenuItem});
            this.portaldatabaseToolStripMenuItem.Name = "portaldatabaseToolStripMenuItem";
            this.portaldatabaseToolStripMenuItem.Size = new System.Drawing.Size(97, 20);
            this.portaldatabaseToolStripMenuItem.Text = "Portaldatabase";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(179, 6);
            // 
            // loadFromIntelMapToolStripMenuItem
            // 
            this.loadFromIntelMapToolStripMenuItem.Name = "loadFromIntelMapToolStripMenuItem";
            this.loadFromIntelMapToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.loadFromIntelMapToolStripMenuItem.Text = "Load from Intel map";
            this.loadFromIntelMapToolStripMenuItem.Click += new System.EventHandler(this.loadFromIntelMapToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(179, 6);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(179, 6);
            // 
            // clearAllToolStripMenuItem
            // 
            this.clearAllToolStripMenuItem.Name = "clearAllToolStripMenuItem";
            this.clearAllToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.clearAllToolStripMenuItem.Text = "Clear all";
            this.clearAllToolStripMenuItem.Click += new System.EventHandler(this.clearAllToolStripMenuItem_Click);
            // 
            // clearDisabledToolStripMenuItem
            // 
            this.clearDisabledToolStripMenuItem.Name = "clearDisabledToolStripMenuItem";
            this.clearDisabledToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.clearDisabledToolStripMenuItem.Text = "Clear disabled";
            this.clearDisabledToolStripMenuItem.Click += new System.EventHandler(this.clearDisabledToolStripMenuItem_Click);
            // 
            // reportToolStripMenuItem
            // 
            this.reportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generateReportToolStripMenuItem});
            this.reportToolStripMenuItem.Name = "reportToolStripMenuItem";
            this.reportToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.reportToolStripMenuItem.Text = "Report";
            // 
            // generateReportToolStripMenuItem
            // 
            this.generateReportToolStripMenuItem.Name = "generateReportToolStripMenuItem";
            this.generateReportToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.generateReportToolStripMenuItem.Text = "Generate Report";
            this.generateReportToolStripMenuItem.Click += new System.EventHandler(this.generateReportToolStripMenuItem_Click);
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showDebugFormToolStripMenuItem});
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.debugToolStripMenuItem.Text = "Debug";
            // 
            // showDebugFormToolStripMenuItem
            // 
            this.showDebugFormToolStripMenuItem.Name = "showDebugFormToolStripMenuItem";
            this.showDebugFormToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.showDebugFormToolStripMenuItem.Text = "Show debug form";
            this.showDebugFormToolStripMenuItem.Click += new System.EventHandler(this.showDebugFormToolStripMenuItem_Click);
            // 
            // infoToolStripMenuItem
            // 
            this.infoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.infoToolStripMenuItem.Name = "infoToolStripMenuItem";
            this.infoToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.infoToolStripMenuItem.Text = "Info";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(12, 20);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // tcMain
            // 
            this.tcMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcMain.Controls.Add(this.tabPage1);
            this.tcMain.Controls.Add(this.tabMap);
            this.tcMain.Location = new System.Drawing.Point(12, 27);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(784, 468);
            this.tcMain.TabIndex = 11;
            this.tcMain.SelectedIndexChanged += new System.EventHandler(this.tcMain_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.lActivceCount);
            this.tabPage1.Controls.Add(this.olvGroup);
            this.tabPage1.Controls.Add(this.bLoadGroup);
            this.tabPage1.Controls.Add(this.bSaveGroup);
            this.tabPage1.Controls.Add(this.olvPortals);
            this.tabPage1.Controls.Add(this.bDbInvert);
            this.tabPage1.Controls.Add(this.bDbEnable);
            this.tabPage1.Controls.Add(this.bDbDisable);
            this.tabPage1.Controls.Add(this.tbDbSearch);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(776, 442);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Portals";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // lActivceCount
            // 
            this.lActivceCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lActivceCount.AutoSize = true;
            this.lActivceCount.Location = new System.Drawing.Point(452, 13);
            this.lActivceCount.Name = "lActivceCount";
            this.lActivceCount.Size = new System.Drawing.Size(35, 13);
            this.lActivceCount.TabIndex = 23;
            this.lActivceCount.Text = "label2";
            // 
            // olvGroup
            // 
            this.olvGroup.AllColumns.Add(this.olvDeleteColumn1);
            this.olvGroup.AllColumns.Add(this.olvColumnGroupName);
            this.olvGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.olvGroup.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.olvGroup.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvDeleteColumn1,
            this.olvColumnGroupName});
            this.olvGroup.FullRowSelect = true;
            this.olvGroup.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.olvGroup.HideSelection = false;
            this.olvGroup.Location = new System.Drawing.Point(6, 34);
            this.olvGroup.Name = "olvGroup";
            this.olvGroup.ShowGroups = false;
            this.olvGroup.ShowHeaderInAllViews = false;
            this.olvGroup.ShowImagesOnSubItems = true;
            this.olvGroup.Size = new System.Drawing.Size(165, 373);
            this.olvGroup.TabIndex = 22;
            this.olvGroup.UseCompatibleStateImageBehavior = false;
            this.olvGroup.View = System.Windows.Forms.View.Details;
            this.olvGroup.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.olvGroup_CellEditStarting);
            this.olvGroup.SelectedIndexChanged += new System.EventHandler(this.olvGroup_SelectedIndexChanged);
            // 
            // olvDeleteColumn1
            // 
            this.olvDeleteColumn1.CellPadding = null;
            this.olvDeleteColumn1.Groupable = false;
            this.olvDeleteColumn1.Text = "";
            this.olvDeleteColumn1.Width = 40;
            // 
            // olvColumnGroupName
            // 
            this.olvColumnGroupName.AspectName = "Name";
            this.olvColumnGroupName.CellPadding = null;
            this.olvColumnGroupName.FillsFreeSpace = true;
            this.olvColumnGroupName.Groupable = false;
            this.olvColumnGroupName.Text = "Group";
            // 
            // bLoadGroup
            // 
            this.bLoadGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bLoadGroup.Location = new System.Drawing.Point(6, 413);
            this.bLoadGroup.Name = "bLoadGroup";
            this.bLoadGroup.Size = new System.Drawing.Size(165, 23);
            this.bLoadGroup.TabIndex = 21;
            this.bLoadGroup.Text = "Load group";
            this.bLoadGroup.UseVisualStyleBackColor = true;
            this.bLoadGroup.Click += new System.EventHandler(this.bLoadGroup_Click);
            // 
            // bSaveGroup
            // 
            this.bSaveGroup.Location = new System.Drawing.Point(6, 8);
            this.bSaveGroup.Name = "bSaveGroup";
            this.bSaveGroup.Size = new System.Drawing.Size(165, 23);
            this.bSaveGroup.TabIndex = 21;
            this.bSaveGroup.Text = "Save group";
            this.bSaveGroup.UseVisualStyleBackColor = true;
            this.bSaveGroup.Click += new System.EventHandler(this.bSaveGroup_Click);
            // 
            // olvPortals
            // 
            this.olvPortals.AllColumns.Add(this.olvEnabledColumn);
            this.olvPortals.AllColumns.Add(this.olvColumn3);
            this.olvPortals.AllColumns.Add(this.olvColumn16);
            this.olvPortals.AllColumns.Add(this.olvColumn18);
            this.olvPortals.AllColumns.Add(this.olvColumnGeo);
            this.olvPortals.AllColumns.Add(this.olvColumn8);
            this.olvPortals.AllColumns.Add(this.olvColumn9);
            this.olvPortals.AllColumns.Add(this.olvColumn10);
            this.olvPortals.AllColumns.Add(this.olvColumn11);
            this.olvPortals.AllColumns.Add(this.olvColumn12);
            this.olvPortals.AllColumns.Add(this.olvColumn13);
            this.olvPortals.AllColumns.Add(this.olvColumn14);
            this.olvPortals.AllColumns.Add(this.olvColumn5);
            this.olvPortals.AllColumns.Add(this.olvColumn4);
            this.olvPortals.AllColumns.Add(this.olvColumn17);
            this.olvPortals.AllColumns.Add(this.olvColumn1);
            this.olvPortals.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.olvPortals.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.olvPortals.CheckBoxes = true;
            this.olvPortals.CheckedAspectName = "Enabled";
            this.olvPortals.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvEnabledColumn,
            this.olvColumn3,
            this.olvColumn16,
            this.olvColumn18,
            this.olvColumnGeo,
            this.olvColumn8,
            this.olvColumn9,
            this.olvColumn10,
            this.olvColumn11,
            this.olvColumn12,
            this.olvColumn13,
            this.olvColumn14,
            this.olvColumn5,
            this.olvColumn4,
            this.olvColumn17,
            this.olvColumn1});
            this.olvPortals.EmptyListMsg = "Portaldatabase is empty. First load from [Portaldatabase] - [Load from intal map]" +
    "";
            this.olvPortals.EmptyListMsgFont = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.olvPortals.FullRowSelect = true;
            this.olvPortals.Location = new System.Drawing.Point(177, 34);
            this.olvPortals.Name = "olvPortals";
            this.olvPortals.ShowGroups = false;
            this.olvPortals.ShowImagesOnSubItems = true;
            this.olvPortals.Size = new System.Drawing.Size(593, 405);
            this.olvPortals.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.olvPortals.TabIndex = 19;
            this.olvPortals.UseCompatibleStateImageBehavior = false;
            this.olvPortals.UseFiltering = true;
            this.olvPortals.UseSubItemCheckBoxes = true;
            this.olvPortals.View = System.Windows.Forms.View.Details;
            this.olvPortals.VirtualMode = true;
            this.olvPortals.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.olv_FormatRow);
            this.olvPortals.ItemsChanged += new System.EventHandler<BrightIdeasSoftware.ItemsChangedEventArgs>(this.olv_ItemsChanged);
            this.olvPortals.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.olv_ItemChecked);
            // 
            // olvEnabledColumn
            // 
            this.olvEnabledColumn.AspectName = "Enabled";
            this.olvEnabledColumn.CellPadding = null;
            this.olvEnabledColumn.IsEditable = false;
            this.olvEnabledColumn.Text = "Enabled";
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "Name";
            this.olvColumn3.CellPadding = null;
            this.olvColumn3.IsEditable = false;
            this.olvColumn3.Text = "Name";
            this.olvColumn3.Width = 199;
            // 
            // olvColumn16
            // 
            this.olvColumn16.AspectName = "ResCount";
            this.olvColumn16.CellPadding = null;
            this.olvColumn16.Text = "ResCount";
            // 
            // olvColumn18
            // 
            this.olvColumn18.AspectName = "Level";
            this.olvColumn18.CellPadding = null;
            this.olvColumn18.Text = "Level";
            // 
            // olvColumnGeo
            // 
            this.olvColumnGeo.AspectName = "ReverseGeoCodingDone";
            this.olvColumnGeo.CellPadding = null;
            this.olvColumnGeo.CheckBoxes = true;
            this.olvColumnGeo.IsEditable = false;
            this.olvColumnGeo.Text = "GeoDone";
            // 
            // olvColumn8
            // 
            this.olvColumn8.AspectName = "Road";
            this.olvColumn8.CellPadding = null;
            this.olvColumn8.Text = "Road";
            // 
            // olvColumn9
            // 
            this.olvColumn9.AspectName = "Suburb";
            this.olvColumn9.CellPadding = null;
            this.olvColumn9.Text = "Suburb";
            // 
            // olvColumn10
            // 
            this.olvColumn10.AspectName = "Village";
            this.olvColumn10.CellPadding = null;
            this.olvColumn10.Text = "Village";
            // 
            // olvColumn11
            // 
            this.olvColumn11.AspectName = "County";
            this.olvColumn11.CellPadding = null;
            this.olvColumn11.Text = "County";
            // 
            // olvColumn12
            // 
            this.olvColumn12.AspectName = "State";
            this.olvColumn12.CellPadding = null;
            this.olvColumn12.Text = "State";
            // 
            // olvColumn13
            // 
            this.olvColumn13.AspectName = "Postcode";
            this.olvColumn13.CellPadding = null;
            this.olvColumn13.Text = "Postcode";
            // 
            // olvColumn14
            // 
            this.olvColumn14.AspectName = "Country";
            this.olvColumn14.CellPadding = null;
            this.olvColumn14.Text = "Country";
            // 
            // olvColumn5
            // 
            this.olvColumn5.AspectName = "Pos.Y";
            this.olvColumn5.CellPadding = null;
            this.olvColumn5.IsEditable = false;
            this.olvColumn5.Text = "Lat";
            // 
            // olvColumn4
            // 
            this.olvColumn4.AspectName = "Pos.X";
            this.olvColumn4.CellPadding = null;
            this.olvColumn4.IsEditable = false;
            this.olvColumn4.Text = "Lon";
            // 
            // olvColumn17
            // 
            this.olvColumn17.AspectName = "AddressName";
            this.olvColumn17.CellPadding = null;
            this.olvColumn17.Groupable = false;
            this.olvColumn17.IsEditable = false;
            this.olvColumn17.Searchable = false;
            this.olvColumn17.Sortable = false;
            this.olvColumn17.Text = "Address";
            this.olvColumn17.Width = 215;
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "Guid";
            this.olvColumn1.CellPadding = null;
            this.olvColumn1.IsEditable = false;
            this.olvColumn1.Text = "Guid";
            // 
            // bDbInvert
            // 
            this.bDbInvert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bDbInvert.Location = new System.Drawing.Point(533, 6);
            this.bDbInvert.Name = "bDbInvert";
            this.bDbInvert.Size = new System.Drawing.Size(75, 23);
            this.bDbInvert.TabIndex = 2;
            this.bDbInvert.Text = "Invert";
            this.bDbInvert.UseVisualStyleBackColor = true;
            this.bDbInvert.Click += new System.EventHandler(this.bDbInvert_Click);
            // 
            // bDbEnable
            // 
            this.bDbEnable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bDbEnable.Location = new System.Drawing.Point(614, 6);
            this.bDbEnable.Name = "bDbEnable";
            this.bDbEnable.Size = new System.Drawing.Size(75, 23);
            this.bDbEnable.TabIndex = 2;
            this.bDbEnable.Text = "Enable";
            this.bDbEnable.UseVisualStyleBackColor = true;
            this.bDbEnable.Click += new System.EventHandler(this.bDbEnable_Click);
            // 
            // bDbDisable
            // 
            this.bDbDisable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bDbDisable.Location = new System.Drawing.Point(695, 6);
            this.bDbDisable.Name = "bDbDisable";
            this.bDbDisable.Size = new System.Drawing.Size(75, 23);
            this.bDbDisable.TabIndex = 2;
            this.bDbDisable.Text = "Disable";
            this.bDbDisable.UseVisualStyleBackColor = true;
            this.bDbDisable.Click += new System.EventHandler(this.bDbDisable_Click);
            // 
            // tbDbSearch
            // 
            this.tbDbSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDbSearch.Location = new System.Drawing.Point(177, 8);
            this.tbDbSearch.Name = "tbDbSearch";
            this.tbDbSearch.Size = new System.Drawing.Size(269, 20);
            this.tbDbSearch.TabIndex = 1;
            this.tbDbSearch.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbDbSearch_KeyUp);
            // 
            // tabMap
            // 
            this.tabMap.Controls.Add(this.gmap);
            this.tabMap.Controls.Add(this.cbShowLastHandled);
            this.tabMap.Controls.Add(this.label1);
            this.tabMap.Controls.Add(this.nudThreadCount);
            this.tabMap.Controls.Add(this.bCreateGameState);
            this.tabMap.Controls.Add(this.bCalcStop);
            this.tabMap.Controls.Add(this.bCalc);
            this.tabMap.Controls.Add(this.bShowParent);
            this.tabMap.Location = new System.Drawing.Point(4, 22);
            this.tabMap.Name = "tabMap";
            this.tabMap.Padding = new System.Windows.Forms.Padding(3);
            this.tabMap.Size = new System.Drawing.Size(776, 442);
            this.tabMap.TabIndex = 1;
            this.tabMap.Text = "EasyLink Map";
            this.tabMap.UseVisualStyleBackColor = true;
            // 
            // gmap
            // 
            this.gmap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gmap.Bearing = 0F;
            this.gmap.CanDragMap = true;
            this.gmap.EmptyTileColor = System.Drawing.Color.Navy;
            this.gmap.GrayScaleMode = false;
            this.gmap.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gmap.LevelsKeepInMemmory = 5;
            this.gmap.Location = new System.Drawing.Point(6, 35);
            this.gmap.MarkersEnabled = true;
            this.gmap.MaxZoom = 2;
            this.gmap.MinZoom = 2;
            this.gmap.MouseWheelZoomEnabled = true;
            this.gmap.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionWithoutCenter;
            this.gmap.Name = "gmap";
            this.gmap.NegativeMode = false;
            this.gmap.PolygonsEnabled = true;
            this.gmap.RetryLoadTile = 0;
            this.gmap.RoutesEnabled = true;
            this.gmap.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.gmap.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.gmap.ShowTileGridLines = false;
            this.gmap.Size = new System.Drawing.Size(764, 379);
            this.gmap.TabIndex = 14;
            this.gmap.Zoom = 0D;
            this.gmap.OnMarkerClick += new GMap.NET.WindowsForms.MarkerClick(this.gmap_OnMarkerClick);
            this.gmap.OnMarkerDoubleClick += new GMap.NET.WindowsForms.MarkerDoubleClick(this.gmap_OnMarkerDoubleClick);
            this.gmap.OnMarkerEnter += new GMap.NET.WindowsForms.MarkerEnter(this.gmap_OnMarkerEnter);
            this.gmap.OnMapDrag += new GMap.NET.MapDrag(this.gmap_OnMapDrag);
            this.gmap.OnMapZoomChanged += new GMap.NET.MapZoomChanged(this.gmap_OnMapZoomChanged);
            // 
            // cbShowLastHandled
            // 
            this.cbShowLastHandled.AutoSize = true;
            this.cbShowLastHandled.Location = new System.Drawing.Point(131, 10);
            this.cbShowLastHandled.Name = "cbShowLastHandled";
            this.cbShowLastHandled.Size = new System.Drawing.Size(113, 17);
            this.cbShowLastHandled.TabIndex = 13;
            this.cbShowLastHandled.Text = "Show last handled";
            this.cbShowLastHandled.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(392, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Threadcount:";
            // 
            // nudThreadCount
            // 
            this.nudThreadCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudThreadCount.Location = new System.Drawing.Point(469, 9);
            this.nudThreadCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudThreadCount.Name = "nudThreadCount";
            this.nudThreadCount.Size = new System.Drawing.Size(42, 20);
            this.nudThreadCount.TabIndex = 10;
            this.nudThreadCount.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // bCreateGameState
            // 
            this.bCreateGameState.Location = new System.Drawing.Point(6, 6);
            this.bCreateGameState.Name = "bCreateGameState";
            this.bCreateGameState.Size = new System.Drawing.Size(119, 23);
            this.bCreateGameState.TabIndex = 9;
            this.bCreateGameState.Text = "Reload";
            this.bCreateGameState.UseVisualStyleBackColor = true;
            this.bCreateGameState.Click += new System.EventHandler(this.bCreateGameState_Click);
            // 
            // bCalcStop
            // 
            this.bCalcStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bCalcStop.Enabled = false;
            this.bCalcStop.Location = new System.Drawing.Point(601, 6);
            this.bCalcStop.Name = "bCalcStop";
            this.bCalcStop.Size = new System.Drawing.Size(75, 23);
            this.bCalcStop.TabIndex = 5;
            this.bCalcStop.Text = "Stop";
            this.bCalcStop.UseVisualStyleBackColor = true;
            this.bCalcStop.Click += new System.EventHandler(this.bCalcStop_Click);
            // 
            // timerFast
            // 
            this.timerFast.Enabled = true;
            this.timerFast.Tick += new System.EventHandler(this.timerFast_Tick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // olvAnchors
            // 
            this.olvAnchors.AllColumns.Add(this.olvDeleteColumn);
            this.olvAnchors.AllColumns.Add(this.olvColumn6);
            this.olvAnchors.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.SingleClick;
            this.olvAnchors.CheckedAspectName = "";
            this.olvAnchors.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvDeleteColumn,
            this.olvColumn6});
            this.olvAnchors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.olvAnchors.Location = new System.Drawing.Point(3, 3);
            this.olvAnchors.Name = "olvAnchors";
            this.olvAnchors.ShowImagesOnSubItems = true;
            this.olvAnchors.Size = new System.Drawing.Size(251, 240);
            this.olvAnchors.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.olvAnchors.TabIndex = 20;
            this.olvAnchors.UseCompatibleStateImageBehavior = false;
            this.olvAnchors.UseFiltering = true;
            this.olvAnchors.View = System.Windows.Forms.View.Details;
            this.olvAnchors.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.olvAnchors_CellEditStarting);
            // 
            // olvDeleteColumn
            // 
            this.olvDeleteColumn.AspectToStringFormat = "Delete";
            this.olvDeleteColumn.CellPadding = null;
            this.olvDeleteColumn.Groupable = false;
            this.olvDeleteColumn.Searchable = false;
            this.olvDeleteColumn.Sortable = false;
            this.olvDeleteColumn.Text = "";
            this.olvDeleteColumn.UseFiltering = false;
            this.olvDeleteColumn.Width = 40;
            // 
            // olvColumn6
            // 
            this.olvColumn6.AspectName = "Name";
            this.olvColumn6.CellPadding = null;
            this.olvColumn6.Groupable = false;
            this.olvColumn6.IsEditable = false;
            this.olvColumn6.Text = "Anchors";
            this.olvColumn6.Width = 169;
            // 
            // tabControl2
            // 
            this.tabControl2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl2.Controls.Add(this.tabAnchors);
            this.tabControl2.Controls.Add(this.tabLinks);
            this.tabControl2.Controls.Add(this.tabDestroyPortals);
            this.tabControl2.Location = new System.Drawing.Point(802, 27);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(265, 272);
            this.tabControl2.TabIndex = 13;
            // 
            // tabAnchors
            // 
            this.tabAnchors.Controls.Add(this.olvAnchors);
            this.tabAnchors.Location = new System.Drawing.Point(4, 22);
            this.tabAnchors.Name = "tabAnchors";
            this.tabAnchors.Padding = new System.Windows.Forms.Padding(3);
            this.tabAnchors.Size = new System.Drawing.Size(257, 246);
            this.tabAnchors.TabIndex = 0;
            this.tabAnchors.Text = "Anchors";
            this.tabAnchors.UseVisualStyleBackColor = true;
            // 
            // tabLinks
            // 
            this.tabLinks.Controls.Add(this.olvLinks);
            this.tabLinks.Location = new System.Drawing.Point(4, 22);
            this.tabLinks.Name = "tabLinks";
            this.tabLinks.Padding = new System.Windows.Forms.Padding(3);
            this.tabLinks.Size = new System.Drawing.Size(257, 246);
            this.tabLinks.TabIndex = 1;
            this.tabLinks.Text = "Links";
            this.tabLinks.UseVisualStyleBackColor = true;
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
            this.olvLinks.ShowImagesOnSubItems = true;
            this.olvLinks.Size = new System.Drawing.Size(251, 240);
            this.olvLinks.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.olvLinks.TabIndex = 21;
            this.olvLinks.UseCompatibleStateImageBehavior = false;
            this.olvLinks.UseFiltering = true;
            this.olvLinks.View = System.Windows.Forms.View.Details;
            this.olvLinks.Resize += new System.EventHandler(this.olvLinks_Resize);
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "P1.Name";
            this.olvColumn2.CellPadding = null;
            this.olvColumn2.Groupable = false;
            this.olvColumn2.IsEditable = false;
            this.olvColumn2.Text = "Origin";
            this.olvColumn2.Width = 169;
            // 
            // olvColumn7
            // 
            this.olvColumn7.AspectName = "P2.Name";
            this.olvColumn7.AspectToStringFormat = "";
            this.olvColumn7.CellPadding = null;
            this.olvColumn7.FillsFreeSpace = true;
            this.olvColumn7.Text = "Destination";
            // 
            // tabDestroyPortals
            // 
            this.tabDestroyPortals.Controls.Add(this.lDestroyStatus);
            this.tabDestroyPortals.Controls.Add(this.olvDestroy);
            this.tabDestroyPortals.Location = new System.Drawing.Point(4, 22);
            this.tabDestroyPortals.Name = "tabDestroyPortals";
            this.tabDestroyPortals.Padding = new System.Windows.Forms.Padding(3);
            this.tabDestroyPortals.Size = new System.Drawing.Size(257, 246);
            this.tabDestroyPortals.TabIndex = 2;
            this.tabDestroyPortals.Text = "Destroy";
            this.tabDestroyPortals.UseVisualStyleBackColor = true;
            // 
            // lDestroyStatus
            // 
            this.lDestroyStatus.AutoSize = true;
            this.lDestroyStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lDestroyStatus.Location = new System.Drawing.Point(6, 3);
            this.lDestroyStatus.Name = "lDestroyStatus";
            this.lDestroyStatus.Size = new System.Drawing.Size(45, 16);
            this.lDestroyStatus.TabIndex = 22;
            this.lDestroyStatus.Text = "label2";
            // 
            // olvDestroy
            // 
            this.olvDestroy.AllColumns.Add(this.olvDeleteColumn2);
            this.olvDestroy.AllColumns.Add(this.olvColumn15);
            this.olvDestroy.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.olvDestroy.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.SingleClick;
            this.olvDestroy.CheckedAspectName = "";
            this.olvDestroy.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvDeleteColumn2,
            this.olvColumn15});
            this.olvDestroy.Location = new System.Drawing.Point(3, 22);
            this.olvDestroy.Name = "olvDestroy";
            this.olvDestroy.ShowGroups = false;
            this.olvDestroy.ShowImagesOnSubItems = true;
            this.olvDestroy.Size = new System.Drawing.Size(251, 199);
            this.olvDestroy.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.olvDestroy.TabIndex = 21;
            this.olvDestroy.UseCompatibleStateImageBehavior = false;
            this.olvDestroy.UseFiltering = true;
            this.olvDestroy.View = System.Windows.Forms.View.Details;
            this.olvDestroy.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.olvDestroy_CellEditStarting);
            this.olvDestroy.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.olv_FormatRow);
            this.olvDestroy.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.olvDestroy_ColumnClick);
            // 
            // olvDeleteColumn2
            // 
            this.olvDeleteColumn2.AspectToStringFormat = "Delete";
            this.olvDeleteColumn2.CellPadding = null;
            this.olvDeleteColumn2.Text = "Delete all";
            // 
            // olvColumn15
            // 
            this.olvColumn15.AspectName = "Name";
            this.olvColumn15.CellPadding = null;
            this.olvColumn15.FillsFreeSpace = true;
            this.olvColumn15.Groupable = false;
            this.olvColumn15.IsEditable = false;
            this.olvColumn15.Text = "Destory";
            this.olvColumn15.Width = 169;
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
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1079, 523);
            this.Controls.Add(this.tabControl2);
            this.Controls.Add(this.tcMain);
            this.Controls.Add(this.tbGameInfos);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "EasyLink";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tcMain.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvGroup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.olvPortals)).EndInit();
            this.tabMap.ResumeLayout(false);
            this.tabMap.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudThreadCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.olvAnchors)).EndInit();
            this.tabControl2.ResumeLayout(false);
            this.tabAnchors.ResumeLayout(false);
            this.tabLinks.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.olvLinks)).EndInit();
            this.tabDestroyPortals.ResumeLayout(false);
            this.tabDestroyPortals.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvDestroy)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tsslMousePosition;
        private System.Windows.Forms.Button bCalc;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Button bShowParent;
        private System.Windows.Forms.ToolStripStatusLabel tsslTotalTested;
        private System.Windows.Forms.TextBox tbGameInfos;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem portaldatabaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabMap;
        private System.Windows.Forms.Button bCreateGameState;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudThreadCount;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem clearAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearDisabledToolStripMenuItem;
        private System.Windows.Forms.TextBox tbDbSearch;
        private System.Windows.Forms.Timer timerFast;
        private System.Windows.Forms.Button bDbInvert;
        private System.Windows.Forms.Button bDbEnable;
        private System.Windows.Forms.Button bDbDisable;
        private System.Windows.Forms.CheckBox cbShowLastHandled;
        private GMap.NET.WindowsForms.GMapControl gmap;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem infoToolStripMenuItem;
        private BrightIdeasSoftware.FastObjectListView olvPortals;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvEnabledColumn;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private BrightIdeasSoftware.OLVColumn olvColumn5;
        private BrightIdeasSoftware.OLVColumn olvColumn4;
        private BrightIdeasSoftware.ObjectListView olvAnchors;
        private BrightIdeasSoftware.OLVColumn olvColumn6;
        private BrightIdeasSoftware.OLVColumn olvDeleteColumn;
        private System.Windows.Forms.Button bLoadGroup;
        private System.Windows.Forms.Button bSaveGroup;
        private BrightIdeasSoftware.ObjectListView olvGroup;
        private BrightIdeasSoftware.OLVColumn olvDeleteColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumnGroupName;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabAnchors;
        private System.Windows.Forms.TabPage tabLinks;
        private BrightIdeasSoftware.ObjectListView olvLinks;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private BrightIdeasSoftware.OLVColumn olvColumn7;
        private BrightIdeasSoftware.OLVColumn olvColumnGeo;
        private BrightIdeasSoftware.OLVColumn olvColumn8;
        private BrightIdeasSoftware.OLVColumn olvColumn9;
        private BrightIdeasSoftware.OLVColumn olvColumn10;
        private BrightIdeasSoftware.OLVColumn olvColumn11;
        private BrightIdeasSoftware.OLVColumn olvColumn12;
        private BrightIdeasSoftware.OLVColumn olvColumn13;
        private BrightIdeasSoftware.OLVColumn olvColumn14;
        private System.Windows.Forms.Button bCalcStop;
        private System.Windows.Forms.Label lActivceCount;
        private BrightIdeasSoftware.ListViewPrinter listViewPrinter1;
        private System.Windows.Forms.TabPage tabDestroyPortals;
        private BrightIdeasSoftware.ObjectListView olvDestroy;
        private BrightIdeasSoftware.OLVColumn olvColumn15;
        private BrightIdeasSoftware.OLVColumn olvDeleteColumn2;
        private System.Windows.Forms.Label lDestroyStatus;
        private BrightIdeasSoftware.OLVColumn olvColumn17;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showDebugFormToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadFromIntelMapToolStripMenuItem;
        private BrightIdeasSoftware.OLVColumn olvColumn16;
        private BrightIdeasSoftware.OLVColumn olvColumn18;
        private System.Windows.Forms.ToolStripMenuItem reportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateReportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    }
}

