using DevExpress.XtraEditors;
using Stend.Production.Root;
using System;
using System.Windows.Forms;

namespace Stend.Pruduction.Controls
{
    public partial class BaseStepControl : XtraUserControl
    {
        public ScriptTree scriptTree { get; set; }
        UserControl userControl = null;

        public BaseStepControl()
        {
            InitializeComponent();
            cbPlugin.Properties.Items.Clear();

            foreach(var item in RootScript._plugins)
            {
                if(item == null)
                {
                    continue;
                }
                cbPlugin.Properties.Items.Add(item);
            }
        }

        public BaseStepControl(ScriptTree step) : this()
        {
            scriptTree = step;
            CreatePluginControl();
            UpdateUI();
        }

        private void UpdateUI()
        {
            cbPlugin.SelectedIndexChanged -= cbPlugin_SelectedIndexChanged;
            try
            {
                if(scriptTree != null)
                {
                    cbLevel.EditValue = scriptTree.ParentID;
                    cbPlugin.EditValue = (PluginBase)scriptTree.Step;
                }
            }
            finally
            {
                cbPlugin.SelectedIndexChanged += cbPlugin_SelectedIndexChanged;
            }
        }

        private void CreatePluginControl()
        {
            if(scriptTree.Step != null)
            {
                try
                {
                    pcSettingMain.Controls.Remove(userControl);
                    if(scriptTree.Step.EditControl is UserControl userCtrl)
                    {
                        userControl = (UserControl)Activator.CreateInstance(userCtrl.GetType(), new object[] { scriptTree.Step });
                        userControl.Parent = pcSettingMain;
                        userControl.Dock = DockStyle.Fill;
                    }
                    else
                    {
                        throw new Exception("Отсутствует редактор плагина");
                    }
                }
                catch(Exception ex)
                {

                }
            }
        }

        private void cbPlugin_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cbPlugin.SelectedItem != null)
            {
                scriptTree.Step = Activator.CreateInstance(cbPlugin.SelectedItem.GetType()) as PluginBase;
                CreatePluginControl();
            }
        }
    }
}
