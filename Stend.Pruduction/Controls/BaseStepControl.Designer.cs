
namespace Stend.Pruduction.Controls
{
    partial class BaseStepControl
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
            this.cbLevel = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lcLevelStep = new DevExpress.XtraLayout.LayoutControlItem();
            this.cbPlugin = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lcPlugin = new DevExpress.XtraLayout.LayoutControlItem();
            this.gcSettingStep = new DevExpress.XtraEditors.GroupControl();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.pcSettingMain = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbLevel.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcLevelStep)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbPlugin.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPlugin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcSettingStep)).BeginInit();
            this.gcSettingStep.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcSettingMain)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gcSettingStep);
            this.layoutControl1.Controls.Add(this.cbLevel);
            this.layoutControl1.Controls.Add(this.cbPlugin);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(562, 395);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lcLevelStep,
            this.lcPlugin,
            this.layoutControlItem1});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(562, 395);
            this.Root.TextVisible = false;
            // 
            // cbLevel
            // 
            this.cbLevel.Location = new System.Drawing.Point(85, 2);
            this.cbLevel.Name = "cbLevel";
            this.cbLevel.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbLevel.Size = new System.Drawing.Size(475, 20);
            this.cbLevel.StyleController = this.layoutControl1;
            this.cbLevel.TabIndex = 4;
            // 
            // lcLevelStep
            // 
            this.lcLevelStep.Control = this.cbLevel;
            this.lcLevelStep.Location = new System.Drawing.Point(0, 0);
            this.lcLevelStep.Name = "lcLevelStep";
            this.lcLevelStep.Size = new System.Drawing.Size(562, 24);
            this.lcLevelStep.Text = "Уровень шага";
            this.lcLevelStep.TextSize = new System.Drawing.Size(71, 13);
            // 
            // cbPlugin
            // 
            this.cbPlugin.Location = new System.Drawing.Point(85, 26);
            this.cbPlugin.Name = "cbPlugin";
            this.cbPlugin.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbPlugin.Size = new System.Drawing.Size(475, 20);
            this.cbPlugin.StyleController = this.layoutControl1;
            this.cbPlugin.TabIndex = 5;
            this.cbPlugin.SelectedIndexChanged += new System.EventHandler(this.cbPlugin_SelectedIndexChanged);
            // 
            // lcPlugin
            // 
            this.lcPlugin.Control = this.cbPlugin;
            this.lcPlugin.Location = new System.Drawing.Point(0, 24);
            this.lcPlugin.Name = "lcPlugin";
            this.lcPlugin.Size = new System.Drawing.Size(562, 24);
            this.lcPlugin.Text = "Плагин";
            this.lcPlugin.TextSize = new System.Drawing.Size(71, 13);
            // 
            // gcSettingStep
            // 
            this.gcSettingStep.Controls.Add(this.pcSettingMain);
            this.gcSettingStep.Location = new System.Drawing.Point(2, 50);
            this.gcSettingStep.Name = "gcSettingStep";
            this.gcSettingStep.Size = new System.Drawing.Size(558, 343);
            this.gcSettingStep.TabIndex = 6;
            this.gcSettingStep.Text = "Настройка шага";
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gcSettingStep;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 48);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(562, 347);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // pcSettingMain
            // 
            this.pcSettingMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pcSettingMain.Location = new System.Drawing.Point(2, 23);
            this.pcSettingMain.Name = "pcSettingMain";
            this.pcSettingMain.Size = new System.Drawing.Size(554, 318);
            this.pcSettingMain.TabIndex = 0;
            // 
            // BaseStepControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "BaseStepControl";
            this.Size = new System.Drawing.Size(562, 395);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbLevel.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcLevelStep)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbPlugin.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPlugin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcSettingStep)).EndInit();
            this.gcSettingStep.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcSettingMain)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.GroupControl gcSettingStep;
        private DevExpress.XtraEditors.ComboBoxEdit cbLevel;
        private DevExpress.XtraEditors.ComboBoxEdit cbPlugin;
        private DevExpress.XtraLayout.LayoutControlItem lcLevelStep;
        private DevExpress.XtraLayout.LayoutControlItem lcPlugin;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.PanelControl pcSettingMain;
    }
}
