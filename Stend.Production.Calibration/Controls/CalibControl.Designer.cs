
namespace Stend.Production.Calibration.Controls
{
    partial class CalibControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            DevExpress.XtraEditors.ButtonsPanelControl.ButtonImageOptions buttonImageOptions1 = new DevExpress.XtraEditors.ButtonsPanelControl.ButtonImageOptions();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CalibControl));
            DevExpress.XtraEditors.ButtonsPanelControl.ButtonImageOptions buttonImageOptions2 = new DevExpress.XtraEditors.ButtonsPanelControl.ButtonImageOptions();
            DevExpress.Utils.SuperToolTip superToolTip1 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipTitleItem toolTipTitleItem1 = new DevExpress.Utils.ToolTipTitleItem();
            this.plugInBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.gcCalib = new DevExpress.XtraEditors.GroupControl();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colGuageAttribute = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGuageOBIS = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colOBIS = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colValue = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAttribute = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colStatus = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colStepName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.textScaler = new DevExpress.XtraEditors.SpinEdit();
            this.memoScript = new DevExpress.XtraEditors.MemoEdit();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lyScaler = new DevExpress.XtraLayout.LayoutControlItem();
            this.lyScript = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.plugInBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcCalib)).BeginInit();
            this.gcCalib.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textScaler.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoScript.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lyScaler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lyScript)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // plugInBindingSource
            // 
            this.plugInBindingSource.DataSource = typeof(Stend.Production.Calibration.PlugIn);
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gcCalib);
            this.layoutControl1.Controls.Add(this.textScaler);
            this.layoutControl1.Controls.Add(this.memoScript);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(624, 478);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // gcCalib
            // 
            this.gcCalib.Controls.Add(this.gridControl1);
            buttonImageOptions1.Image = ((System.Drawing.Image)(resources.GetObject("buttonImageOptions1.Image")));
            buttonImageOptions2.Image = ((System.Drawing.Image)(resources.GetObject("buttonImageOptions2.Image")));
            toolTipTitleItem1.Text = "Пауза процесса калибровки";
            superToolTip1.Items.Add(toolTipTitleItem1);
            this.gcCalib.CustomHeaderButtons.AddRange(new DevExpress.XtraEditors.ButtonPanel.IBaseButton[] {
            new DevExpress.XtraEditors.ButtonsPanelControl.GroupBoxButton("", true, buttonImageOptions1, DevExpress.XtraBars.Docking2010.ButtonStyle.PushButton, "", -1, true, null, true, false, true, 1, -1),
            new DevExpress.XtraEditors.ButtonsPanelControl.GroupBoxButton("", true, buttonImageOptions2, DevExpress.XtraBars.Docking2010.ButtonStyle.PushButton, "", -1, true, superToolTip1, true, false, true, 2, -1)});
            this.gcCalib.Location = new System.Drawing.Point(2, 122);
            this.gcCalib.Name = "gcCalib";
            this.gcCalib.Size = new System.Drawing.Size(620, 354);
            this.gcCalib.TabIndex = 6;
            this.gcCalib.Text = "Калибровка объекта";
            this.gcCalib.CustomButtonClick += new DevExpress.XtraBars.Docking2010.BaseButtonEventHandler(this.gcCalib_CustomButtonClick);
            // 
            // gridControl1
            // 
            this.gridControl1.DataSource = this.plugInBindingSource;
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.Location = new System.Drawing.Point(2, 23);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(616, 329);
            this.gridControl1.TabIndex = 0;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colGuageAttribute,
            this.colGuageOBIS,
            this.colOBIS,
            this.colValue,
            this.colAttribute,
            this.colStatus,
            this.colName,
            this.colStepName,
            this.colDescription});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // colGuageAttribute
            // 
            this.colGuageAttribute.FieldName = "GuageAttribute";
            this.colGuageAttribute.Name = "colGuageAttribute";
            this.colGuageAttribute.Visible = true;
            this.colGuageAttribute.VisibleIndex = 0;
            // 
            // colGuageOBIS
            // 
            this.colGuageOBIS.FieldName = "GuageOBIS";
            this.colGuageOBIS.Name = "colGuageOBIS";
            this.colGuageOBIS.Visible = true;
            this.colGuageOBIS.VisibleIndex = 1;
            // 
            // colOBIS
            // 
            this.colOBIS.FieldName = "OBIS";
            this.colOBIS.Name = "colOBIS";
            this.colOBIS.Visible = true;
            this.colOBIS.VisibleIndex = 2;
            // 
            // colValue
            // 
            this.colValue.FieldName = "Value";
            this.colValue.Name = "colValue";
            this.colValue.Visible = true;
            this.colValue.VisibleIndex = 3;
            // 
            // colAttribute
            // 
            this.colAttribute.FieldName = "Attribute";
            this.colAttribute.Name = "colAttribute";
            this.colAttribute.Visible = true;
            this.colAttribute.VisibleIndex = 4;
            // 
            // colStatus
            // 
            this.colStatus.FieldName = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.Visible = true;
            this.colStatus.VisibleIndex = 5;
            // 
            // colName
            // 
            this.colName.FieldName = "Name";
            this.colName.Name = "colName";
            this.colName.OptionsColumn.ReadOnly = true;
            this.colName.Visible = true;
            this.colName.VisibleIndex = 6;
            // 
            // colStepName
            // 
            this.colStepName.FieldName = "StepName";
            this.colStepName.Name = "colStepName";
            this.colStepName.Visible = true;
            this.colStepName.VisibleIndex = 7;
            // 
            // colDescription
            // 
            this.colDescription.FieldName = "Description";
            this.colDescription.Name = "colDescription";
            this.colDescription.Visible = true;
            this.colDescription.VisibleIndex = 8;
            // 
            // textScaler
            // 
            this.textScaler.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.textScaler.Location = new System.Drawing.Point(92, 2);
            this.textScaler.Name = "textScaler";
            this.textScaler.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.textScaler.Size = new System.Drawing.Size(530, 20);
            this.textScaler.StyleController = this.layoutControl1;
            this.textScaler.TabIndex = 4;
            // 
            // memoScript
            // 
            this.memoScript.Location = new System.Drawing.Point(92, 26);
            this.memoScript.Name = "memoScript";
            this.memoScript.Size = new System.Drawing.Size(530, 92);
            this.memoScript.StyleController = this.layoutControl1;
            this.memoScript.TabIndex = 5;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lyScaler,
            this.lyScript,
            this.layoutControlItem1});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(624, 478);
            this.Root.TextVisible = false;
            // 
            // lyScaler
            // 
            this.lyScaler.Control = this.textScaler;
            this.lyScaler.Location = new System.Drawing.Point(0, 0);
            this.lyScaler.Name = "lyScaler";
            this.lyScaler.Size = new System.Drawing.Size(624, 24);
            this.lyScaler.Text = "Скалярное число";
            this.lyScaler.TextSize = new System.Drawing.Size(87, 13);
            // 
            // lyScript
            // 
            this.lyScript.Control = this.memoScript;
            this.lyScript.Location = new System.Drawing.Point(0, 24);
            this.lyScript.Name = "lyScript";
            this.lyScript.Size = new System.Drawing.Size(624, 96);
            this.lyScript.Text = "Скрипт";
            this.lyScript.TextSize = new System.Drawing.Size(87, 13);
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gcCalib;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 120);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(624, 358);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // CalibControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "CalibControl";
            this.Size = new System.Drawing.Size(624, 478);
            ((System.ComponentModel.ISupportInitialize)(this.plugInBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcCalib)).EndInit();
            this.gcCalib.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textScaler.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoScript.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lyScaler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lyScript)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.BindingSource plugInBindingSource;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.SpinEdit textScaler;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem lyScaler;
        private DevExpress.XtraEditors.MemoEdit memoScript;
        private DevExpress.XtraLayout.LayoutControlItem lyScript;
        private DevExpress.XtraEditors.GroupControl gcCalib;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn colGuageAttribute;
        private DevExpress.XtraGrid.Columns.GridColumn colGuageOBIS;
        private DevExpress.XtraGrid.Columns.GridColumn colOBIS;
        private DevExpress.XtraGrid.Columns.GridColumn colValue;
        private DevExpress.XtraGrid.Columns.GridColumn colAttribute;
        private DevExpress.XtraGrid.Columns.GridColumn colStatus;
        private DevExpress.XtraGrid.Columns.GridColumn colName;
        private DevExpress.XtraGrid.Columns.GridColumn colStepName;
        private DevExpress.XtraGrid.Columns.GridColumn colDescription;
    }
}
