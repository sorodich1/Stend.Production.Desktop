using DevExpress.XtraEditors;
using NLog;
using Stend.Production.Root;
using System;
using System.Windows.Forms;

namespace Stend.Production.DevicesCheck.Forms
{
    public partial class MainForm : XtraForm
    {
        private readonly PluginBase _plugin;

        public MainForm(PluginBase plugin)
        {
            InitializeComponent();

            if (plugin != null)
            {
                try
                {
                    _plugin = plugin;

                    if (plugin.EditControl is UserControl userControl)
                    {
                        UserControl ctrl = (UserControl)Activator.CreateInstance(userControl.GetType(), new object[] { _plugin });
                        ctrl.Parent = panelControl1;
                        ctrl.Dock = DockStyle.Fill;
                    }
                }
                catch (Exception ex)
                {
                    plugin.AddLog(ex.Message, LogLevel.Error);
                }
            }
        }

        private void btOK_Click(object sender, EventArgs e)
        {

        }
    }
}