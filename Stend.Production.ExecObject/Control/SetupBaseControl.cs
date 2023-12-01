using DevExpress.XtraEditors;
using Stend.Production.Root;

namespace Stend.Production.ExecObject.Control
{
    public partial class SetupBaseControl : XtraUserControl
    {
        public PluginBase SetupBase { get; set; }

        public SetupBaseControl()
        {
            InitializeComponent();

            pluginBaseBindingSource.DataSource = RootScript.childrens;
        }

        public SetupBaseControl(PluginBase setupBase) : this()
        {
            SetupBase = setupBase;

            if (RootScript.Spodes != null)
            {
                var pluginObject = SetupBase as PluginObject;

                var pluginClone = pluginObject.Clone();

                pluginClone.OBIS = RootScript.Spodes.OBIS;

                // pluginObject.Attr = RootScript.Spodes.Attr[0];

                pluginClone.Description = RootScript.Spodes.DescRU;

                RootScript.childrens.Add(pluginClone);

                RootScript.Spodes = null;

                pluginBaseBindingSource.DataSource = RootScript.childrens;
                pluginBaseBindingSource.ResetBindings(false);
            }
        }
    }
}
