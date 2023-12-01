using DevExpress.XtraEditors;
using Stend.Production.Calibration.Controls;
using Stend.Production.DevicesCheck;
using Stend.Production.Root;
using Stend.Pruduction.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Stend.Pruduction.Forms
{
    public partial class ScriptTreeForm : XtraForm
    {
        public ScriptTree ScriptTree { get; set; }

        public ScriptTreeForm()
        {
            InitializeComponent();
        }

        public ScriptTreeForm(ScriptTree scriptTree) : this()
        {
            ScriptTree = scriptTree;
            if(ScriptTree != null)
            {
                BaseStepControl baseStepControl = new BaseStepControl(ScriptTree);
                if(baseStepControl != null)
                {
                    baseStepControl.Parent = panelControl1;
                    baseStepControl.Dock = DockStyle.Fill;
                }
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            int id = 0;

            int purId = RootScript.scriptTree.Count == 0 ? 0 : RootScript.scriptTree.Max(item => item.ID);

            ScriptTree childrenItem = RootScript.scriptTree.FirstOrDefault(item => item.ID == ScriptTree.ID);

            ScriptTree.Step.InternalSteps = RootScript.childrens;

            if(ScriptTree.Step.Name == "Calibration")
            {
                List<PluginBase> plugIn = new List<PluginBase>();

                foreach(var item in CalibControl.pluginBases)
                {
                    plugIn.Add(item);
                }

                ScriptTree.Step.InternalSteps = plugIn;
            }
            if (ScriptTree.Step.Name == "DevicesCheck")
            {
                List<PluginBase> plugIn = new List<PluginBase>();

                foreach (var item in PlugIn.plugIns)
                {
                    plugIn.Add(item);
                }

                ScriptTree.Step.InternalSteps = plugIn;
            }
            //foreach (var item in RootScript.childrens)
            //{
            //    // item.Parent = ScriptTree.Step;

            //    // ScriptTree scriptTree = new ScriptTree(++id, purId, item);

            //    //RootScript.scriptTree.Add(new ScriptTree(++id, purId, item));

            //}
        }
    }
}