using DevExpress.XtraEditors;
using Stend.Production.Root;
using Stend.Production;
using System;
using System.Collections.Generic;
using Stend.Pruduction.Forms;

namespace Stend.Production.Initialization.Controls
{
    public partial class SetupBaseControl : XtraUserControl
    {
        public PluginBase SetupBase { get; set; }

        public SetupBaseControl()
        {
            InitializeComponent();
        }

        public List<StepData> stepDatas = new List<StepData>();

        public SetupBaseControl(PluginBase step) : this()
        {
            SetupBase = step;

            textNameStep.Text = SetupBase.StepName;
            textDescription.Text = SetupBase.Description;
        }

        private void gcSettingObject_CustomButtonClick(object sender, DevExpress.XtraBars.Docking2010.BaseButtonEventArgs e)
        {
            switch(Convert.ToInt32(e.Button.Properties.Tag.ToString()))
            {
                case 1:
                    break;
            }
        }
    }
}
