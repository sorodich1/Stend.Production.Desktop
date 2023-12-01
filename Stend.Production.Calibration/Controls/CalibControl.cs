using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraEditors;
using Stend.Production.Root;
using Stend.Pruduction.Forms;
using System;
using System.Collections.Generic;

namespace Stend.Production.Calibration.Controls
{
    public partial class CalibControl : XtraUserControl
    {
        PluginBase _step = null;
        public static List<PlugIn> pluginBases = new List<PlugIn>();

        public CalibControl()
        {
            InitializeComponent();

            textScaler.Text = "32767";

            memoScript.Text = "namespace Test\r\n {\r\n public class Calibrator\r\n {\r\n public static double Calculate(double BaseValue, double Value, double calibCoef)\r\n {\r\n  if (BaseValue == 0 || Value == 0)\r\n return 0;\r\n var calc = (Value - BaseValue) / BaseValue * 100;\r\n double res = (int)((100 / (calc + 100) - 1) * calibCoef);\r\n return res;\r\n }\r\n }\r\n }\r\n";

        }

        public CalibControl(PluginBase step) : this()
        {
            _step = step;

            plugInBindingSource.DataSource = pluginBases;


            if (RootScript.Spodes != null)
            {
                PlugIn plugIn = new PlugIn()
                {
                    OBIS = RootScript.Spodes.OBIS,
                    Description = RootScript.Spodes.DescRU,
                    Scaler = Convert.ToInt32(textScaler.Text),
                    Script = memoScript.Text
                };

                pluginBases.Add(plugIn);

                plugInBindingSource.DataSource = pluginBases;
                plugInBindingSource.ResetBindings(false);
            }

            RootScript.Spodes = null;
        }

        private void gcCalib_CustomButtonClick(object sender, BaseButtonEventArgs e)
        {
            try
            {
                switch(Convert.ToInt32(e.Button.Properties.Tag.ToString()))
                {
                    case 1:
                        _step.AddLog("Выполняется добавление объекта");
                        SpodesListObject slo = new SpodesListObject(_step);
                        slo.ShowDialog();
                        plugInBindingSource.ResetBindings(false);
                        break;
                    case 2:

                        UserActionPromptStep promptStep = new UserActionPromptStep
                        {
                            StepName = "Пауза для калибровки измерителей ПУ",
                            Description = "Выполняет остановку перед калибровкой нетрали ПУ",
                        };

                        pluginBases.Add((PlugIn)promptStep);

                        plugInBindingSource.ResetBindings(false);
                        break;
                }
            }
            catch(Exception ex)
            {
                _step.AddLog($"Ошибка выполнения события: {ex.Message}");
            }
        }
    }
}
