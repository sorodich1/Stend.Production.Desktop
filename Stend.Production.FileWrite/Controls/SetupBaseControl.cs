using DevExpress.XtraEditors;
using Stend.Production.Root;
using System.Collections.Generic;

namespace Stend.Production.FileWrite.Controls
{
    public partial class SetupBaseControl : XtraUserControl
    {
        PluginObject _plugin;
        
        public SetupBaseControl()
        {
            InitializeComponent();

            pluginBaseBindingSource.DataSource = RootScript.childrens;
        }

        public SetupBaseControl(PluginObject plugin) : this()
        {
            _plugin = plugin;

            if (RootScript.Spodes != null)
            {
                var pluginObject = _plugin as PluginObject;

                var pluginClone = pluginObject.Clone();

                pluginClone.OBIS = RootScript.Spodes.OBIS;

                pluginClone.Description = RootScript.Spodes.DescRU;

                pluginClone.FileValue.Add(PluginObject.AutoscrollIndication);

                RootScript.childrens.Add(pluginClone);

                RootScript.Spodes = null;
            }

            pluginBaseBindingSource.DataSource = RootScript.childrens;
            pluginBaseBindingSource.ResetBindings(false);
        }
    }
}
