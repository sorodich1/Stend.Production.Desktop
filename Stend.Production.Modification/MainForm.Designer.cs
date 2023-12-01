
namespace Stend.Production.Modification
{
    partial class Form1
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
            this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.btStart = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem3 = new DevExpress.XtraBars.BarButtonItem();
            this.btSettingsStend = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem5 = new DevExpress.XtraBars.BarButtonItem();
            this.btSetModific = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.splitMain = new DevExpress.XtraEditors.SplitContainerControl();
            this.gcStend = new DevExpress.XtraEditors.GroupControl();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.placeSettingBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colIsActive = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSerial = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPort = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colStatusDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colStatus = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcStend)).BeginInit();
            this.gcStend.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.placeSettingBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbonControl1
            // 
            this.ribbonControl1.ExpandCollapseItem.Id = 0;
            this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonControl1.ExpandCollapseItem,
            this.ribbonControl1.SearchEditItem,
            this.barButtonItem1,
            this.btStart,
            this.barButtonItem3,
            this.btSettingsStend,
            this.barButtonItem5,
            this.btSetModific});
            this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
            this.ribbonControl1.MaxItemId = 7;
            this.ribbonControl1.Name = "ribbonControl1";
            this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbonControl1.QuickToolbarItemLinks.Add(this.barButtonItem1);
            this.ribbonControl1.Size = new System.Drawing.Size(818, 158);
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Caption = "Старт";
            this.barButtonItem1.Id = 1;
            this.barButtonItem1.ImageOptions.SvgImage = global::Stend.Production.Modification.Properties.Resources.gettingstarted;
            this.barButtonItem1.Name = "barButtonItem1";
            // 
            // btStart
            // 
            this.btStart.Caption = "Запуск калибровки";
            this.btStart.Id = 2;
            this.btStart.ImageOptions.SvgImage = global::Stend.Production.Modification.Properties.Resources.gettingstarted1;
            this.btStart.Name = "btStart";
            this.btStart.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btStart_ItemClick);
            // 
            // barButtonItem3
            // 
            this.barButtonItem3.Caption = "Остановка калибровки";
            this.barButtonItem3.Id = 3;
            this.barButtonItem3.ImageOptions.SvgImage = global::Stend.Production.Modification.Properties.Resources.paymentunpaid;
            this.barButtonItem3.Name = "barButtonItem3";
            // 
            // btSettingsStend
            // 
            this.btSettingsStend.Caption = "Настройка стенда";
            this.btSettingsStend.Id = 4;
            this.btSettingsStend.ImageOptions.SvgImage = global::Stend.Production.Modification.Properties.Resources.parameters;
            this.btSettingsStend.Name = "btSettingsStend";
            this.btSettingsStend.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btSettingsStend_ItemClick);
            // 
            // barButtonItem5
            // 
            this.barButtonItem5.Caption = "База данных";
            this.barButtonItem5.Id = 5;
            this.barButtonItem5.ImageOptions.SvgImage = global::Stend.Production.Modification.Properties.Resources.editdatasource;
            this.barButtonItem5.Name = "barButtonItem5";
            // 
            // btSetModific
            // 
            this.btSetModific.Caption = "Настройка мадификации";
            this.btSetModific.Id = 6;
            this.btSetModific.ImageOptions.SvgImage = global::Stend.Production.Modification.Properties.Resources.pivottableoptions;
            this.btSetModific.Name = "btSetModific";
            this.btSetModific.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btSetModific_ItemClick);
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1,
            this.ribbonPageGroup2});
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "Выполнение программы";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.ItemLinks.Add(this.btStart);
            this.ribbonPageGroup1.ItemLinks.Add(this.barButtonItem3);
            this.ribbonPageGroup1.ItemLinks.Add(this.btSettingsStend);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "Работа программы";
            // 
            // ribbonPageGroup2
            // 
            this.ribbonPageGroup2.ItemLinks.Add(this.barButtonItem5);
            this.ribbonPageGroup2.ItemLinks.Add(this.btSetModific);
            this.ribbonPageGroup2.Name = "ribbonPageGroup2";
            this.ribbonPageGroup2.Text = "Дополнительные настройки";
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 158);
            this.splitMain.Name = "splitMain";
            this.splitMain.Panel1.Text = "Panel1";
            this.splitMain.Panel2.Controls.Add(this.gcStend);
            this.splitMain.Panel2.Text = "Panel2";
            this.splitMain.Size = new System.Drawing.Size(818, 597);
            this.splitMain.SplitterPosition = 275;
            this.splitMain.TabIndex = 1;
            // 
            // gcStend
            // 
            this.gcStend.Controls.Add(this.gridControl1);
            this.gcStend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcStend.Location = new System.Drawing.Point(0, 0);
            this.gcStend.Name = "gcStend";
            this.gcStend.Size = new System.Drawing.Size(533, 597);
            this.gcStend.TabIndex = 0;
            this.gcStend.Text = "Исполняемые устройства";
            // 
            // gridControl1
            // 
            this.gridControl1.DataSource = this.placeSettingBindingSource;
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.Location = new System.Drawing.Point(2, 23);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.MenuManager = this.ribbonControl1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(529, 572);
            this.gridControl1.TabIndex = 0;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // placeSettingBindingSource
            // 
            this.placeSettingBindingSource.DataSource = typeof(Stend.Production.Root.PlaceSetting);
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colIsActive,
            this.colNumber,
            this.colSerial,
            this.colPort,
            this.colStatusDescription,
            this.colStatus});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // colIsActive
            // 
            this.colIsActive.Caption = "Активный статус";
            this.colIsActive.FieldName = "IsActive";
            this.colIsActive.Name = "colIsActive";
            this.colIsActive.Visible = true;
            this.colIsActive.VisibleIndex = 0;
            // 
            // colNumber
            // 
            this.colNumber.Caption = "Номер места на стенде";
            this.colNumber.FieldName = "PlaceId";
            this.colNumber.Name = "colNumber";
            this.colNumber.Visible = true;
            this.colNumber.VisibleIndex = 1;
            // 
            // colSerial
            // 
            this.colSerial.Caption = "Серийный номер";
            this.colSerial.FieldName = "Serial";
            this.colSerial.Name = "colSerial";
            this.colSerial.Visible = true;
            this.colSerial.VisibleIndex = 2;
            // 
            // colPort
            // 
            this.colPort.Caption = "Последовательный порт";
            this.colPort.FieldName = "Port";
            this.colPort.Name = "colPort";
            this.colPort.Visible = true;
            this.colPort.VisibleIndex = 3;
            // 
            // colStatusDescription
            // 
            this.colStatusDescription.Caption = "Описание выполнения";
            this.colStatusDescription.FieldName = "StatusDescription";
            this.colStatusDescription.Name = "colStatusDescription";
            this.colStatusDescription.Visible = true;
            this.colStatusDescription.VisibleIndex = 4;
            // 
            // colStatus
            // 
            this.colStatus.Caption = "Статус";
            this.colStatus.FieldName = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.Visible = true;
            this.colStatus.VisibleIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(818, 755);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.ribbonControl1);
            this.Name = "Form1";
            this.Ribbon = this.ribbonControl1;
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcStend)).EndInit();
            this.gcStend.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.placeSettingBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarButtonItem btStart;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem3;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraBars.BarButtonItem btSettingsStend;
        private DevExpress.XtraBars.BarButtonItem barButtonItem5;
        private DevExpress.XtraBars.BarButtonItem btSetModific;
        private DevExpress.XtraEditors.SplitContainerControl splitMain;
        private DevExpress.XtraEditors.GroupControl gcStend;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn colIsActive;
        private DevExpress.XtraGrid.Columns.GridColumn colNumber;
        private DevExpress.XtraGrid.Columns.GridColumn colSerial;
        private DevExpress.XtraGrid.Columns.GridColumn colPort;
        private System.Windows.Forms.BindingSource placeSettingBindingSource;
        private DevExpress.XtraGrid.Columns.GridColumn colStatusDescription;
        private DevExpress.XtraGrid.Columns.GridColumn colStatus;
    }
}

