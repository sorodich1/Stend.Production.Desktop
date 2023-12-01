using DevExpress.XtraEditors;
using NLog;
using Stend.Production.Root;
using System;
using System.Windows.Forms;

namespace Stend.Production.Initialization.Forms
{
    public partial class SettingInitStep : XtraForm
    {
        private readonly PluginBase _plug;

        public SettingInitStep(PluginBase step)
        {
            InitializeComponent();

            if(step != null)
            {
                try
                {
                    _plug = step;
                    if(step.EditControl is UserControl userControl)
                    {
                        UserControl ctrl = (UserControl)Activator.CreateInstance(userControl.GetType(), new object[] { _plug });
                        ctrl.Parent = panelControl1;
                        ctrl.Dock = DockStyle.Fill;
                    }
                }
                catch(Exception ex)
                {
                    step.AddLog(ex.Message, LogLevel.Error);
                    throw;
                }
            }
        }

        private void btOK_Click(object sender, EventArgs e)
        {

        }
    }
}