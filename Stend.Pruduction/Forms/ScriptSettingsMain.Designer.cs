
namespace Stend.Pruduction
{
    partial class ScriptSettingsMain
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
            this.components = new System.ComponentModel.Container();
            this.rcMain = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.btLoadAssFile = new DevExpress.XtraBars.BarButtonItem();
            this.btUploadSetFile = new DevExpress.XtraBars.BarButtonItem();
            this.btSaveSetFile = new DevExpress.XtraBars.BarButtonItem();
            this.btViewingScript = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.btAddGroup = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.rbAssociationSetting = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.gcScriptList = new DevExpress.XtraEditors.GroupControl();
            this.treeList1 = new DevExpress.XtraTreeList.TreeList();
            this.colName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colDescription = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colStep = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colID = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colParentID = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colOrder = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn2 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn3 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.scriptTreeBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.gcSettingStep = new DevExpress.XtraEditors.GroupControl();
            this.panelControl = new DevExpress.XtraEditors.PanelControl();
            this.lcMain = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.dockManager1 = new DevExpress.XtraBars.Docking.DockManager(this.components);
            this.hideContainerBottom = new DevExpress.XtraBars.Docking.AutoHideContainer();
            this.NLog = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.logMemo = new DevExpress.XtraEditors.MemoEdit();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.textNameScript = new DevExpress.XtraEditors.TextEdit();
            this.lcNameScriptText = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.rcMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcScriptList)).BeginInit();
            this.gcScriptList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeList1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scriptTreeBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcSettingStep)).BeginInit();
            this.gcSettingStep.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).BeginInit();
            this.hideContainerBottom.SuspendLayout();
            this.NLog.SuspendLayout();
            this.dockPanel1_Container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logMemo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textNameScript.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcNameScriptText)).BeginInit();
            this.SuspendLayout();
            // 
            // rcMain
            // 
            this.rcMain.ExpandCollapseItem.Id = 0;
            this.rcMain.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.rcMain.ExpandCollapseItem,
            this.rcMain.SearchEditItem,
            this.btLoadAssFile,
            this.btUploadSetFile,
            this.btSaveSetFile,
            this.btViewingScript,
            this.barButtonItem1,
            this.btAddGroup,
            this.barButtonItem2});
            this.rcMain.Location = new System.Drawing.Point(0, 0);
            this.rcMain.MaxItemId = 9;
            this.rcMain.Name = "rcMain";
            this.rcMain.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.rcMain.Size = new System.Drawing.Size(1054, 158);
            // 
            // btLoadAssFile
            // 
            this.btLoadAssFile.Caption = "Загрузить файл ассоциаций";
            this.btLoadAssFile.Id = 1;
            this.btLoadAssFile.ImageOptions.SvgImage = global::Stend.Pruduction.Properties.Resources.open2;
            this.btLoadAssFile.Name = "btLoadAssFile";
            this.btLoadAssFile.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btLoadAssFile_ItemClick);
            // 
            // btUploadSetFile
            // 
            this.btUploadSetFile.Caption = "Загрузить файл настроек";
            this.btUploadSetFile.Id = 2;
            this.btUploadSetFile.ImageOptions.SvgImage = global::Stend.Pruduction.Properties.Resources.open2;
            this.btUploadSetFile.Name = "btUploadSetFile";
            // 
            // btSaveSetFile
            // 
            this.btSaveSetFile.Caption = "Сохранить файл настроек";
            this.btSaveSetFile.Id = 3;
            this.btSaveSetFile.ImageOptions.SvgImage = global::Stend.Pruduction.Properties.Resources.save;
            this.btSaveSetFile.Name = "btSaveSetFile";
            this.btSaveSetFile.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btSaveSetFile_ItemClick);
            // 
            // btViewingScript
            // 
            this.btViewingScript.Caption = "Просмотр скрипта";
            this.btViewingScript.Id = 4;
            this.btViewingScript.ImageOptions.SvgImage = global::Stend.Pruduction.Properties.Resources.bo_list;
            this.btViewingScript.Name = "btViewingScript";
            this.btViewingScript.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btViewingScript_ItemClick);
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Caption = "barButtonItem1";
            this.barButtonItem1.Id = 6;
            this.barButtonItem1.Name = "barButtonItem1";
            // 
            // btAddGroup
            // 
            this.btAddGroup.Caption = "Добавить группу";
            this.btAddGroup.Id = 7;
            this.btAddGroup.ImageOptions.SvgImage = global::Stend.Pruduction.Properties.Resources.actions_addcircled1;
            this.btAddGroup.Name = "btAddGroup";
            this.btAddGroup.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btAddGroup_ItemClick);
            // 
            // barButtonItem2
            // 
            this.barButtonItem2.Caption = "Тестирование скрипта";
            this.barButtonItem2.Id = 8;
            this.barButtonItem2.ImageOptions.SvgImage = global::Stend.Pruduction.Properties.Resources.gettingstarted;
            this.barButtonItem2.Name = "barButtonItem2";
            this.barButtonItem2.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem2_ItemClick);
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1,
            this.rbAssociationSetting});
            this.ribbonPage1.Name = "ribbonPage1";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.ItemLinks.Add(this.btLoadAssFile);
            this.ribbonPageGroup1.ItemLinks.Add(this.btUploadSetFile);
            this.ribbonPageGroup1.ItemLinks.Add(this.btSaveSetFile);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "Файл";
            // 
            // rbAssociationSetting
            // 
            this.rbAssociationSetting.ItemLinks.Add(this.btViewingScript);
            this.rbAssociationSetting.ItemLinks.Add(this.btAddGroup);
            this.rbAssociationSetting.ItemLinks.Add(this.barButtonItem2);
            this.rbAssociationSetting.Name = "rbAssociationSetting";
            this.rbAssociationSetting.Text = "Разработка";
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.textNameScript);
            this.layoutControl1.Controls.Add(this.layoutControl2);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 158);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(1054, 658);
            this.layoutControl1.TabIndex = 1;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.splitContainerControl1);
            this.layoutControl2.Location = new System.Drawing.Point(2, 26);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.Root = this.lcMain;
            this.layoutControl2.Size = new System.Drawing.Size(1050, 630);
            this.layoutControl2.TabIndex = 4;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // splitContainerControl1
            // 
            this.splitContainerControl1.Location = new System.Drawing.Point(2, 2);
            this.splitContainerControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.gcScriptList);
            this.splitContainerControl1.Panel1.Text = "Panel1";
            this.splitContainerControl1.Panel2.Controls.Add(this.gcSettingStep);
            this.splitContainerControl1.Panel2.Text = "Panel2";
            this.splitContainerControl1.Size = new System.Drawing.Size(1046, 626);
            this.splitContainerControl1.SplitterPosition = 457;
            this.splitContainerControl1.TabIndex = 4;
            // 
            // gcScriptList
            // 
            this.gcScriptList.Controls.Add(this.treeList1);
            this.gcScriptList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcScriptList.Location = new System.Drawing.Point(0, 0);
            this.gcScriptList.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gcScriptList.Name = "gcScriptList";
            this.gcScriptList.Size = new System.Drawing.Size(457, 626);
            this.gcScriptList.TabIndex = 0;
            this.gcScriptList.Text = "Список объектов калибровки";
            // 
            // treeList1
            // 
            this.treeList1.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.colName,
            this.colDescription,
            this.colStep,
            this.colID,
            this.colParentID,
            this.colOrder,
            this.treeListColumn1,
            this.treeListColumn2,
            this.treeListColumn3});
            this.treeList1.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.treeList1.CustomizationFormBounds = new System.Drawing.Rectangle(2714, 344, 252, 266);
            this.treeList1.DataSource = this.scriptTreeBindingSource;
            this.treeList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeList1.Location = new System.Drawing.Point(2, 23);
            this.treeList1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.treeList1.MenuManager = this.rcMain;
            this.treeList1.MinWidth = 17;
            this.treeList1.Name = "treeList1";
            this.treeList1.Size = new System.Drawing.Size(453, 601);
            this.treeList1.TabIndex = 4;
            this.treeList1.TreeLevelWidth = 15;
            this.treeList1.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.treeList1_FocusedNodeChanged);
            // 
            // colName
            // 
            this.colName.FieldName = "Step";
            this.colName.Name = "colName";
            this.colName.OptionsColumn.ReadOnly = true;
            this.colName.Width = 58;
            // 
            // colDescription
            // 
            this.colDescription.FieldName = "Description";
            this.colDescription.Name = "colDescription";
            this.colDescription.OptionsColumn.ReadOnly = true;
            this.colDescription.Width = 83;
            // 
            // colStep
            // 
            this.colStep.FieldName = "Step";
            this.colStep.Name = "colStep";
            this.colStep.OptionsColumn.ReadOnly = true;
            this.colStep.Width = 102;
            // 
            // colID
            // 
            this.colID.FieldName = "ID";
            this.colID.Name = "colID";
            this.colID.OptionsColumn.ReadOnly = true;
            this.colID.Width = 93;
            // 
            // colParentID
            // 
            this.colParentID.FieldName = "ParendID";
            this.colParentID.Name = "colParentID";
            this.colParentID.OptionsColumn.ReadOnly = true;
            this.colParentID.Width = 165;
            // 
            // colOrder
            // 
            this.colOrder.FieldName = "Order";
            this.colOrder.Name = "colOrder";
            this.colOrder.OptionsColumn.ReadOnly = true;
            this.colOrder.Width = 164;
            // 
            // treeListColumn1
            // 
            this.treeListColumn1.Caption = "colName";
            this.treeListColumn1.FieldName = "Name";
            this.treeListColumn1.Name = "treeListColumn1";
            this.treeListColumn1.Visible = true;
            this.treeListColumn1.VisibleIndex = 0;
            // 
            // treeListColumn2
            // 
            this.treeListColumn2.Caption = "colDesc";
            this.treeListColumn2.FieldName = "Description";
            this.treeListColumn2.Name = "treeListColumn2";
            this.treeListColumn2.Visible = true;
            this.treeListColumn2.VisibleIndex = 1;
            // 
            // treeListColumn3
            // 
            this.treeListColumn3.Caption = "colStep";
            this.treeListColumn3.FieldName = "Step";
            this.treeListColumn3.Name = "treeListColumn3";
            this.treeListColumn3.Visible = true;
            this.treeListColumn3.VisibleIndex = 2;
            // 
            // scriptTreeBindingSource
            // 
            this.scriptTreeBindingSource.DataSource = typeof(Stend.Pruduction.ScriptTree);
            // 
            // gcSettingStep
            // 
            this.gcSettingStep.Controls.Add(this.panelControl);
            this.gcSettingStep.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcSettingStep.Location = new System.Drawing.Point(0, 0);
            this.gcSettingStep.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gcSettingStep.Name = "gcSettingStep";
            this.gcSettingStep.Size = new System.Drawing.Size(579, 626);
            this.gcSettingStep.TabIndex = 0;
            this.gcSettingStep.Text = "Настройка шага калибровки";
            // 
            // panelControl
            // 
            this.panelControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl.Location = new System.Drawing.Point(2, 23);
            this.panelControl.Name = "panelControl";
            this.panelControl.Size = new System.Drawing.Size(575, 601);
            this.panelControl.TabIndex = 0;
            // 
            // lcMain
            // 
            this.lcMain.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.lcMain.GroupBordersVisible = false;
            this.lcMain.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2});
            this.lcMain.Name = "lcMain";
            this.lcMain.Size = new System.Drawing.Size(1050, 630);
            this.lcMain.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.splitContainerControl1;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(1050, 630);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.lcNameScriptText});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(1054, 658);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.layoutControl2;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(1054, 634);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // dockManager1
            // 
            this.dockManager1.AutoHideContainers.AddRange(new DevExpress.XtraBars.Docking.AutoHideContainer[] {
            this.hideContainerBottom});
            this.dockManager1.Form = this;
            this.dockManager1.TopZIndexControls.AddRange(new string[] {
            "DevExpress.XtraBars.BarDockControl",
            "DevExpress.XtraBars.StandaloneBarDockControl",
            "System.Windows.Forms.StatusBar",
            "System.Windows.Forms.MenuStrip",
            "System.Windows.Forms.StatusStrip",
            "DevExpress.XtraBars.Ribbon.RibbonStatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonControl",
            "DevExpress.XtraBars.Navigation.OfficeNavigationBar",
            "DevExpress.XtraBars.Navigation.TileNavPane",
            "DevExpress.XtraBars.TabFormControl",
            "DevExpress.XtraBars.FluentDesignSystem.FluentDesignFormControl",
            "DevExpress.XtraBars.ToolbarForm.ToolbarFormControl"});
            // 
            // hideContainerBottom
            // 
            this.hideContainerBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.hideContainerBottom.Controls.Add(this.NLog);
            this.hideContainerBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.hideContainerBottom.Location = new System.Drawing.Point(0, 816);
            this.hideContainerBottom.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.hideContainerBottom.Name = "hideContainerBottom";
            this.hideContainerBottom.Size = new System.Drawing.Size(1054, 21);
            // 
            // NLog
            // 
            this.NLog.Controls.Add(this.dockPanel1_Container);
            this.NLog.Dock = DevExpress.XtraBars.Docking.DockingStyle.Bottom;
            this.NLog.ID = new System.Guid("32434671-2079-4d18-977b-93d666e8dc78");
            this.NLog.Location = new System.Drawing.Point(0, 616);
            this.NLog.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.NLog.Name = "NLog";
            this.NLog.OriginalSize = new System.Drawing.Size(200, 200);
            this.NLog.SavedDock = DevExpress.XtraBars.Docking.DockingStyle.Bottom;
            this.NLog.SavedIndex = 0;
            this.NLog.Size = new System.Drawing.Size(1054, 200);
            this.NLog.Text = "Выходная информация";
            this.NLog.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;
            // 
            // dockPanel1_Container
            // 
            this.dockPanel1_Container.Controls.Add(this.logMemo);
            this.dockPanel1_Container.Location = new System.Drawing.Point(3, 27);
            this.dockPanel1_Container.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dockPanel1_Container.Name = "dockPanel1_Container";
            this.dockPanel1_Container.Size = new System.Drawing.Size(1048, 170);
            this.dockPanel1_Container.TabIndex = 0;
            // 
            // logMemo
            // 
            this.logMemo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logMemo.Location = new System.Drawing.Point(0, 0);
            this.logMemo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.logMemo.MenuManager = this.rcMain;
            this.logMemo.Name = "logMemo";
            this.logMemo.Size = new System.Drawing.Size(1048, 170);
            this.logMemo.TabIndex = 0;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // textNameScript
            // 
            this.textNameScript.Location = new System.Drawing.Point(96, 2);
            this.textNameScript.MenuManager = this.rcMain;
            this.textNameScript.Name = "textNameScript";
            this.textNameScript.Size = new System.Drawing.Size(956, 20);
            this.textNameScript.StyleController = this.layoutControl1;
            this.textNameScript.TabIndex = 5;
            // 
            // lcNameScriptText
            // 
            this.lcNameScriptText.Control = this.textNameScript;
            this.lcNameScriptText.Location = new System.Drawing.Point(0, 0);
            this.lcNameScriptText.Name = "lcNameScriptText";
            this.lcNameScriptText.Size = new System.Drawing.Size(1054, 24);
            this.lcNameScriptText.Text = "Имя модификации";
            this.lcNameScriptText.TextSize = new System.Drawing.Size(91, 13);
            // 
            // ScriptSettingsMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1054, 837);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.hideContainerBottom);
            this.Controls.Add(this.rcMain);
            this.Name = "ScriptSettingsMain";
            this.Ribbon = this.rcMain;
            this.Text = "Настройка скрипта";
            this.Load += new System.EventHandler(this.ScriptSettingsMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.rcMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcScriptList)).EndInit();
            this.gcScriptList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeList1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scriptTreeBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcSettingStep)).EndInit();
            this.gcSettingStep.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).EndInit();
            this.hideContainerBottom.ResumeLayout(false);
            this.NLog.ResumeLayout(false);
            this.dockPanel1_Container.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.logMemo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textNameScript.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcNameScriptText)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl rcMain;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem btLoadAssFile;
        private DevExpress.XtraBars.BarButtonItem btUploadSetFile;
        private DevExpress.XtraBars.BarButtonItem btSaveSetFile;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraLayout.LayoutControlGroup lcMain;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraEditors.GroupControl gcScriptList;
        private DevExpress.XtraEditors.GroupControl gcSettingStep;
        private DevExpress.XtraBars.Docking.DockManager dockManager1;
        private DevExpress.XtraBars.Docking.DockPanel NLog;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel1_Container;
        private DevExpress.XtraEditors.MemoEdit logMemo;
        private DevExpress.XtraBars.Docking.AutoHideContainer hideContainerBottom;
        private DevExpress.XtraBars.BarButtonItem btViewingScript;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rbAssociationSetting;
        private DevExpress.XtraTreeList.TreeList treeList1;
        private System.Windows.Forms.Timer timer1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colName;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colDescription;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colStep;
        private System.Windows.Forms.BindingSource scriptTreeBindingSource;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarButtonItem btAddGroup;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colID;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colParentID;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colOrder;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn2;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn3;
        private DevExpress.XtraEditors.PanelControl panelControl;
        private DevExpress.XtraBars.BarButtonItem barButtonItem2;
        private DevExpress.XtraEditors.TextEdit textNameScript;
        private DevExpress.XtraLayout.LayoutControlItem lcNameScriptText;
    }
}

