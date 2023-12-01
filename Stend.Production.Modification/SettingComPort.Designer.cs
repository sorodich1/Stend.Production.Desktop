
namespace Stend.Production.Modification
{
    partial class SettingComPort
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
            this.gcSettingComPort = new DevExpress.XtraEditors.GroupControl();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btCansel = new DevExpress.XtraEditors.SimpleButton();
            this.btOK = new DevExpress.XtraEditors.SimpleButton();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.placeSettingBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.viewComPort = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colPlaceId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPort = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.labelError = new DevExpress.XtraLayout.SimpleLabelItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.settingPortBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.repPort = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.behaviorManager1 = new DevExpress.Utils.Behaviors.BehaviorManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.gcSettingComPort)).BeginInit();
            this.gcSettingComPort.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.placeSettingBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewComPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.labelError)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.settingPortBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // gcSettingComPort
            // 
            this.gcSettingComPort.Controls.Add(this.layoutControl1);
            this.gcSettingComPort.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcSettingComPort.Location = new System.Drawing.Point(0, 0);
            this.gcSettingComPort.Name = "gcSettingComPort";
            this.gcSettingComPort.Size = new System.Drawing.Size(372, 622);
            this.gcSettingComPort.TabIndex = 0;
            this.gcSettingComPort.Text = "Настройка последовательных портов";
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btCansel);
            this.layoutControl1.Controls.Add(this.btOK);
            this.layoutControl1.Controls.Add(this.gridControl1);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(2, 23);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2338, 359, 650, 400);
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(368, 597);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btCansel
            // 
            this.btCansel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCansel.Location = new System.Drawing.Point(2, 573);
            this.btCansel.Name = "btCansel";
            this.btCansel.Size = new System.Drawing.Size(180, 22);
            this.btCansel.StyleController = this.layoutControl1;
            this.btCansel.TabIndex = 6;
            this.btCansel.Text = "Отмена";
            // 
            // btOK
            // 
            this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btOK.Location = new System.Drawing.Point(186, 573);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(180, 22);
            this.btOK.StyleController = this.layoutControl1;
            this.btOK.TabIndex = 5;
            this.btOK.Text = "OK";
            // 
            // gridControl1
            // 
            this.gridControl1.DataSource = this.placeSettingBindingSource;
            this.gridControl1.Location = new System.Drawing.Point(2, 2);
            this.gridControl1.MainView = this.viewComPort;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repPort});
            this.gridControl1.Size = new System.Drawing.Size(364, 550);
            this.gridControl1.TabIndex = 4;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewComPort});
            // 
            // placeSettingBindingSource
            // 
            this.placeSettingBindingSource.DataSource = typeof(Stend.Production.Root.PlaceSetting);
            // 
            // viewComPort
            // 
            this.viewComPort.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colPlaceId,
            this.colPort});
            this.viewComPort.GridControl = this.gridControl1;
            this.viewComPort.Name = "viewComPort";
            this.viewComPort.OptionsView.ShowGroupPanel = false;
            this.viewComPort.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.viewComPort_CellValueChanged);
            // 
            // colPlaceId
            // 
            this.colPlaceId.Caption = "Место на стенде";
            this.colPlaceId.FieldName = "PlaceId";
            this.colPlaceId.Name = "colPlaceId";
            this.colPlaceId.Visible = true;
            this.colPlaceId.VisibleIndex = 0;
            this.colPlaceId.Width = 146;
            // 
            // colPort
            // 
            this.colPort.Caption = "Последовательный порт";
            this.colPort.ColumnEdit = this.repPort;
            this.colPort.FieldName = "Port";
            this.colPort.Name = "colPort";
            this.colPort.Visible = true;
            this.colPort.VisibleIndex = 1;
            this.colPort.Width = 193;
            // 
            // Root
            // 
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.labelError,
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(368, 597);
            this.Root.TextVisible = false;
            // 
            // labelError
            // 
            this.labelError.AllowHotTrack = false;
            this.labelError.Location = new System.Drawing.Point(0, 554);
            this.labelError.Name = "labelError";
            this.labelError.Size = new System.Drawing.Size(368, 17);
            this.labelError.Text = "Ошибка";
            this.labelError.TextSize = new System.Drawing.Size(40, 13);
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControl1;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(368, 554);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btOK;
            this.layoutControlItem2.Location = new System.Drawing.Point(184, 571);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(184, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btCansel;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 571);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(184, 26);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // settingPortBindingSource
            // 
            this.settingPortBindingSource.DataSource = typeof(Stend.Production.Root.SettingPort);
            // 
            // repPort
            // 
            this.repPort.AutoHeight = false;
            this.repPort.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repPort.Name = "repPort";
            // 
            // SettingComPort
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(372, 622);
            this.Controls.Add(this.gcSettingComPort);
            this.Name = "SettingComPort";
            this.Text = "SettingComPort";
            ((System.ComponentModel.ISupportInitialize)(this.gcSettingComPort)).EndInit();
            this.gcSettingComPort.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.placeSettingBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewComPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.labelError)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.settingPortBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.GroupControl gcSettingComPort;
        private System.Windows.Forms.BindingSource settingPortBindingSource;
        private System.Windows.Forms.BindingSource placeSettingBindingSource;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.SimpleButton btCansel;
        private DevExpress.XtraEditors.SimpleButton btOK;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView viewComPort;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.SimpleLabelItem labelError;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraGrid.Columns.GridColumn colPlaceId;
        private DevExpress.XtraGrid.Columns.GridColumn colPort;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repPort;
        private DevExpress.Utils.Behaviors.BehaviorManager behaviorManager1;
    }
}