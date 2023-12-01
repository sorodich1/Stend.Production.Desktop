using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraEditors;
using Stend.Production.DevicesCheck.Forms;
using Stend.Production.Root;

using System;
using System.Collections.Generic;

namespace Stend.Production.DevicesCheck.Controls
{
    public partial class DeviceCheckControl : XtraUserControl
    {
        PlugIn _plugIn;
        public DeviceCheckControl()
        {
            InitializeComponent();
        }

        public DeviceCheckControl(PlugIn plugIn) : this()
        {
            _plugIn = plugIn;

            plugInBindingSource.DataSource = PlugIn.plugIns;

            if (RootScript.Spodes != null)
            {
                PlugIn plugin = new PlugIn()
                {
                    OBIS = RootScript.Spodes.OBIS,
                    Description = RootScript.Spodes.DescRU
                };

                PlugIn.plugIns.Add(plugin);

                plugInBindingSource.DataSource = PlugIn.plugIns;
                plugInBindingSource.ResetBindings(false);
            }
            RootScript.Spodes = null;
            //_plugIn.Col1 = Convert.ToInt32(textCommunicatorСode.Text);
            //_plugIn.Col2 = Convert.ToInt32(textControllerСode.Text);
            //_plugIn.Col3 = Convert.ToInt32(textControllerСode2.Text);
            //_plugIn.Col4 = Convert.ToInt32(textlyMeterCode.Text);
        }

        private void groupControl1_CustomButtonClick(object sender, BaseButtonEventArgs e)
        {
            try
            {
                switch (Convert.ToInt32(e.Button.Properties.Tag.ToString()))
                {
                    case 1:
                        _plugIn.AddLog("Выполняется добавление объекта");
                        SpodesListObject slo = new SpodesListObject(_plugIn);
                        slo.ShowDialog();
                        plugInBindingSource.ResetBindings(false);
                        break;
                }
            }
            catch (Exception ex)
            {
                _plugIn.AddLog($"Ошибка выполнения события: {ex.Message}");
            }
        }
    }
}
