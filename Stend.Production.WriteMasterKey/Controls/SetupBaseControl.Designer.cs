
namespace Stend.Production.WriteMasterKey.Controls
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.textName = new DevExpress.XtraEditors.TextEdit();
            this.lcName = new DevExpress.XtraLayout.LayoutControlItem();
            this.textStepName = new DevExpress.XtraEditors.TextEdit();
            this.lcNameStep = new DevExpress.XtraLayout.LayoutControlItem();
            this.textMasterKey = new DevExpress.XtraEditors.TextEdit();
            this.lcMasterKey = new DevExpress.XtraLayout.LayoutControlItem();
            this.lbDescription = new DevExpress.XtraEditors.ListBoxControl();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textStepName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcNameStep)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textMasterKey.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcMasterKey)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbDescription)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.lbDescription);
            this.layoutControl1.Controls.Add(this.textName);
            this.layoutControl1.Controls.Add(this.textStepName);
            this.layoutControl1.Controls.Add(this.textMasterKey);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(595, 409);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lcName,
            this.lcNameStep,
            this.lcMasterKey,
            this.layoutControlItem4});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(595, 409);
            this.Root.TextVisible = false;
            // 
            // textName
            // 
            this.textName.Location = new System.Drawing.Point(149, 12);
            this.textName.Name = "textName";
            this.textName.Size = new System.Drawing.Size(434, 20);
            this.textName.StyleController = this.layoutControl1;
            this.textName.TabIndex = 4;
            // 
            // lcName
            // 
            this.lcName.Control = this.textName;
            this.lcName.Location = new System.Drawing.Point(0, 0);
            this.lcName.Name = "lcName";
            this.lcName.Size = new System.Drawing.Size(575, 24);
            this.lcName.Text = "Имя";
            this.lcName.TextSize = new System.Drawing.Size(125, 13);
            // 
            // textStepName
            // 
            this.textStepName.Location = new System.Drawing.Point(149, 36);
            this.textStepName.Name = "textStepName";
            this.textStepName.Size = new System.Drawing.Size(434, 20);
            this.textStepName.StyleController = this.layoutControl1;
            this.textStepName.TabIndex = 5;
            // 
            // lcNameStep
            // 
            this.lcNameStep.Control = this.textStepName;
            this.lcNameStep.Location = new System.Drawing.Point(0, 24);
            this.lcNameStep.Name = "lcNameStep";
            this.lcNameStep.Size = new System.Drawing.Size(575, 24);
            this.lcNameStep.Text = "Имя шага";
            this.lcNameStep.TextSize = new System.Drawing.Size(125, 13);
            // 
            // textMasterKey
            // 
            this.textMasterKey.Location = new System.Drawing.Point(149, 60);
            this.textMasterKey.Name = "textMasterKey";
            this.textMasterKey.Size = new System.Drawing.Size(434, 20);
            this.textMasterKey.StyleController = this.layoutControl1;
            this.textMasterKey.TabIndex = 6;
            // 
            // lcMasterKey
            // 
            this.lcMasterKey.Control = this.textMasterKey;
            this.lcMasterKey.Location = new System.Drawing.Point(0, 48);
            this.lcMasterKey.Name = "lcMasterKey";
            this.lcMasterKey.Size = new System.Drawing.Size(575, 24);
            this.lcMasterKey.Text = "Ключь мастер прошивки";
            this.lcMasterKey.TextSize = new System.Drawing.Size(125, 13);
            // 
            // lbDescription
            // 
            this.lbDescription.Location = new System.Drawing.Point(12, 84);
            this.lbDescription.Name = "lbDescription";
            this.lbDescription.Size = new System.Drawing.Size(571, 313);
            this.lbDescription.StyleController = this.layoutControl1;
            this.lbDescription.TabIndex = 7;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.lbDescription;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 72);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(575, 317);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // SetupBaseControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "SetupBaseControl";
            this.Size = new System.Drawing.Size(595, 409);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textStepName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcNameStep)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textMasterKey.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcMasterKey)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbDescription)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.ListBoxControl lbDescription;
        private DevExpress.XtraEditors.TextEdit textName;
        private DevExpress.XtraEditors.TextEdit textStepName;
        private DevExpress.XtraEditors.TextEdit textMasterKey;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem lcName;
        private DevExpress.XtraLayout.LayoutControlItem lcNameStep;
        private DevExpress.XtraLayout.LayoutControlItem lcMasterKey;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
    }
}
