using DevExpress.XtraEditors;
using NLog;
using Stend.Production.Root;
using System;
using System.Windows.Forms;

namespace Stend.Production.ReadObject
{
    public partial class MainForm : XtraForm
    {
        private readonly PluginBase _plug;

        public MainForm(PluginBase step)
        {
            InitializeComponent();

            if (step != null)
            {
                try
                {
                    _plug = step;
                    if (step.EditControl is UserControl userControl)
                    {
                        UserControl ctrl = (UserControl)Activator.CreateInstance(userControl.GetType(), new object[] { _plug });
                        ctrl.Parent = labelControl1;
                        ctrl.Dock = DockStyle.Fill;
                    }
                }
                catch (Exception ex)
                {
                    step.AddLog(ex.Message, LogLevel.Error);
                    throw;
                }
            }
        }
    }
}