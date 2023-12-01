using DevExpress.XtraEditors;
using Stend.Production.Root;

namespace Stend.Production.WriteMasterKey.Controls
{
    public partial class SetupBaseControl : XtraUserControl
    {
        PluginBase _step = null;

        public SetupBaseControl()
        {
            InitializeComponent();
        }

        public SetupBaseControl(PluginBase plugin) : this()
        {
            _step = plugin;
        }
    }
}
