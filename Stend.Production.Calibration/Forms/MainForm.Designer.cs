
namespace Stend.Production.Calibration
{
    partial class MainForm
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lyScaler = new DevExpress.XtraLayout.LayoutControlItem();
            this.lyScript = new DevExpress.XtraLayout.LayoutControlItem();
            this.textEdit2 = new DevExpress.XtraEditors.MemoEdit();
            this.textScaler = new DevExpress.XtraEditors.SpinEdit();
            this.panelCalib = new DevExpress.XtraEditors.PanelControl();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lyScaler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lyScript)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textScaler.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelCalib)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.panelCalib);
            this.layoutControl1.Controls.Add(this.textEdit2);
            this.layoutControl1.Controls.Add(this.textScaler);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(613, 487);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
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
            this.Root.Size = new System.Drawing.Size(613, 487);
            this.Root.TextVisible = false;
            // 
            // lyScaler
            // 
            this.lyScaler.Control = this.textScaler;
            this.lyScaler.Location = new System.Drawing.Point(0, 0);
            this.lyScaler.Name = "lyScaler";
            this.lyScaler.Size = new System.Drawing.Size(613, 24);
            this.lyScaler.Text = "Скалярное число";
            this.lyScaler.TextSize = new System.Drawing.Size(87, 13);
            // 
            // lyScript
            // 
            this.lyScript.Control = this.textEdit2;
            this.lyScript.Location = new System.Drawing.Point(0, 24);
            this.lyScript.Name = "lyScript";
            this.lyScript.Size = new System.Drawing.Size(613, 91);
            this.lyScript.Text = "Скрипт";
            this.lyScript.TextSize = new System.Drawing.Size(87, 13);
            // 
            // textEdit2
            // 
            this.textEdit2.Location = new System.Drawing.Point(101, 26);
            this.textEdit2.Name = "textEdit2";
            this.textEdit2.Size = new System.Drawing.Size(510, 87);
            this.textEdit2.StyleController = this.layoutControl1;
            this.textEdit2.TabIndex = 5;
            // 
            // textScaler
            // 
            this.textScaler.Location = new System.Drawing.Point(101, 2);
            this.textScaler.Name = "textScaler";
            this.textScaler.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.textScaler.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.textScaler.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            this.textScaler.Size = new System.Drawing.Size(510, 20);
            this.textScaler.StyleController = this.layoutControl1;
            this.textScaler.TabIndex = 4;
            // 
            // panelCalib
            // 
            this.panelCalib.Location = new System.Drawing.Point(2, 117);
            this.panelCalib.Name = "panelCalib";
            this.panelCalib.Size = new System.Drawing.Size(609, 368);
            this.panelCalib.TabIndex = 6;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.panelCalib;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 115);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(613, 372);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(613, 487);
            this.Controls.Add(this.layoutControl1);
            this.Name = "MainForm";
            this.Text = "MainForm";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lyScaler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lyScript)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textScaler.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelCalib)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.PanelControl panelCalib;
        private DevExpress.XtraEditors.MemoEdit textEdit2;
        private DevExpress.XtraEditors.SpinEdit textScaler;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem lyScaler;
        private DevExpress.XtraLayout.LayoutControlItem lyScript;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
    }
}