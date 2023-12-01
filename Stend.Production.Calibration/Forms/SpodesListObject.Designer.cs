
namespace Stend.Pruduction.Forms
{
    partial class SpodesListObject
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
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.gcSpodesList = new DevExpress.XtraGrid.GridControl();
            this.spodesBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.gViewSpodes = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colOBIS = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDescRU = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcSpodesList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spodesBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gViewSpodes)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.gcSpodesList);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(669, 588);
            this.panelControl1.TabIndex = 0;
            // 
            // gcSpodesList
            // 
            this.gcSpodesList.DataSource = this.spodesBindingSource;
            this.gcSpodesList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcSpodesList.Location = new System.Drawing.Point(2, 2);
            this.gcSpodesList.MainView = this.gViewSpodes;
            this.gcSpodesList.Name = "gcSpodesList";
            this.gcSpodesList.Size = new System.Drawing.Size(665, 584);
            this.gcSpodesList.TabIndex = 0;
            this.gcSpodesList.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gViewSpodes});
            this.gcSpodesList.DoubleClick += new System.EventHandler(this.gViewSpodes_DoubleClick);
            // 
            // spodesBindingSource
            // 
            this.spodesBindingSource.DataSource = typeof(Stend.Pruduction.Spodes);
            // 
            // gViewSpodes
            // 
            this.gViewSpodes.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colID,
            this.colOBIS,
            this.colDescRU});
            this.gViewSpodes.GridControl = this.gcSpodesList;
            this.gViewSpodes.Name = "gViewSpodes";
            // 
            // colID
            // 
            this.colID.FieldName = "ID";
            this.colID.Name = "colID";
            this.colID.Visible = true;
            this.colID.VisibleIndex = 0;
            // 
            // colOBIS
            // 
            this.colOBIS.FieldName = "OBIS";
            this.colOBIS.Name = "colOBIS";
            this.colOBIS.Visible = true;
            this.colOBIS.VisibleIndex = 1;
            // 
            // colDescRU
            // 
            this.colDescRU.FieldName = "DescRU";
            this.colDescRU.Name = "colDescRU";
            this.colDescRU.Visible = true;
            this.colDescRU.VisibleIndex = 2;
            // 
            // SpodesListObject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(669, 588);
            this.Controls.Add(this.panelControl1);
            this.Name = "SpodesListObject";
            this.Text = "SpodesListObject";
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcSpodesList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spodesBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gViewSpodes)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private System.Windows.Forms.BindingSource spodesBindingSource;
        private DevExpress.XtraGrid.GridControl gcSpodesList;
        private DevExpress.XtraGrid.Views.Grid.GridView gViewSpodes;
        private DevExpress.XtraGrid.Columns.GridColumn colID;
        private DevExpress.XtraGrid.Columns.GridColumn colOBIS;
        private DevExpress.XtraGrid.Columns.GridColumn colDescRU;
    }
}