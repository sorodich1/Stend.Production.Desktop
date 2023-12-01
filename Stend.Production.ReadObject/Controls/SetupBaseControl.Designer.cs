
namespace Stend.Production.ReadObject
{
    partial class SetupBaseControl
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
            this.greadReadControl = new DevExpress.XtraGrid.GridControl();
            this.pluginBaseBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colStepName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colVersion = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.greadReadControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pluginBaseBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // greadReadControl
            // 
            this.greadReadControl.DataSource = this.pluginBaseBindingSource;
            this.greadReadControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.greadReadControl.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.greadReadControl.Location = new System.Drawing.Point(0, 0);
            this.greadReadControl.MainView = this.gridView1;
            this.greadReadControl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.greadReadControl.Name = "greadReadControl";
            this.greadReadControl.Size = new System.Drawing.Size(711, 282);
            this.greadReadControl.TabIndex = 0;
            this.greadReadControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // pluginBaseBindingSource
            // 
            this.pluginBaseBindingSource.DataSource = typeof(Stend.Production.Root.PluginBase);
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colName,
            this.colStepName,
            this.colDescription,
            this.colVersion,
            this.gridColumn1,
            this.gridColumn3});
            this.gridView1.DetailHeight = 284;
            this.gridView1.GridControl = this.greadReadControl;
            this.gridView1.Name = "gridView1";
            // 
            // colName
            // 
            this.colName.FieldName = "Name";
            this.colName.MinWidth = 21;
            this.colName.Name = "colName";
            this.colName.OptionsColumn.ReadOnly = true;
            this.colName.Visible = true;
            this.colName.VisibleIndex = 0;
            this.colName.Width = 81;
            // 
            // colStepName
            // 
            this.colStepName.FieldName = "StepName";
            this.colStepName.MinWidth = 21;
            this.colStepName.Name = "colStepName";
            this.colStepName.Visible = true;
            this.colStepName.VisibleIndex = 1;
            this.colStepName.Width = 81;
            // 
            // colDescription
            // 
            this.colDescription.FieldName = "Description";
            this.colDescription.MinWidth = 21;
            this.colDescription.Name = "colDescription";
            this.colDescription.Visible = true;
            this.colDescription.VisibleIndex = 2;
            this.colDescription.Width = 81;
            // 
            // colVersion
            // 
            this.colVersion.FieldName = "Version";
            this.colVersion.MinWidth = 21;
            this.colVersion.Name = "colVersion";
            this.colVersion.OptionsColumn.ReadOnly = true;
            this.colVersion.Visible = true;
            this.colVersion.VisibleIndex = 3;
            this.colVersion.Width = 81;
            // 
            // gridColumn1
            // 
            this.gridColumn1.FieldName = "OBIS";
            this.gridColumn1.MinWidth = 21;
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 4;
            this.gridColumn1.Width = 81;
            // 
            // gridColumn3
            // 
            this.gridColumn3.FieldName = "Attribute";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 5;
            // 
            // SetupBaseControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.greadReadControl);
            this.Name = "SetupBaseControl";
            this.Size = new System.Drawing.Size(711, 282);
            ((System.ComponentModel.ISupportInitialize)(this.greadReadControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pluginBaseBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl greadReadControl;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private System.Windows.Forms.BindingSource pluginBaseBindingSource;
        private DevExpress.XtraGrid.Columns.GridColumn colName;
        private DevExpress.XtraGrid.Columns.GridColumn colStepName;
        private DevExpress.XtraGrid.Columns.GridColumn colDescription;
        private DevExpress.XtraGrid.Columns.GridColumn colVersion;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
    }
}
