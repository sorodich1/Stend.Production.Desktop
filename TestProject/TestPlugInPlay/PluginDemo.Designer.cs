
namespace TestPlugInPlay
{
    partial class PluginDemo
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
            this.lvPluginsList = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // lvPluginsList
            // 
            this.lvPluginsList.HideSelection = false;
            this.lvPluginsList.Location = new System.Drawing.Point(12, 12);
            this.lvPluginsList.Name = "lvPluginsList";
            this.lvPluginsList.Size = new System.Drawing.Size(522, 720);
            this.lvPluginsList.TabIndex = 0;
            this.lvPluginsList.UseCompatibleStateImageBehavior = false;
            this.lvPluginsList.View = System.Windows.Forms.View.Details;
            // 
            // PluginDemo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(937, 744);
            this.Controls.Add(this.lvPluginsList);
            this.Name = "PluginDemo";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvPluginsList;
    }
}

