using DevExpress.XtraEditors;
using Stend.Production.Root;
using System;
using System.Windows.Forms;

namespace Stend.Production.Calibration
{
    public partial class MainForm : XtraForm
    {
        private readonly PluginBase _plug;
        public MainForm(PluginBase plug)
        {
            InitializeComponent();

            if (_plug != null)
            {
                try
                {
                    _plug = plug;

                    if (plug.EditControl is UserControl userControl)
                    {
                        UserControl ctrl = (UserControl)Activator.CreateInstance(userControl.GetType(), new object[] { _plug });
                        ctrl.Parent = panelCalib;
                        ctrl.Dock = DockStyle.Fill;
                    }
                }
                catch
                {

                }
            }
        }
    }
}