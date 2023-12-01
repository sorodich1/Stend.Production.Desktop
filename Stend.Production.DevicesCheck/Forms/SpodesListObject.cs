using DevExpress.XtraEditors;
using Stend.Production.Root;
using Stend.Pruduction;
using System;
using System.Windows.Forms;

namespace Stend.Production.DevicesCheck.Forms
{
    public partial class SpodesListObject : XtraForm
    {
        public Spodes Spodes { get; private set; }
        private readonly PlugIn _plugin;

        public SpodesListObject(PluginBase plugin)
        {
            _plugin = plugin as PlugIn;

            InitializeComponent();

            spodesBindingSource.DataSource = Spodes.spodes;
        }

        private void gridControl1_DoubleClick(object sender, EventArgs e)
        {
            var selectedObgect = gridView1.GetFocusedRow() as Spodes;

            if (selectedObgect != null)
            {
                if (_plugin.EditControl is UserControl userControl)
                {
                    RootScript.Spodes = selectedObgect;

                    UserControl ctrl = (UserControl)Activator.CreateInstance(userControl.GetType(), new object[] { _plugin });
                }
            }
            Close();
        }
    }
}