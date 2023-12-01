using Gurux.DLMS.Enums;

using Newtonsoft.Json;
using NLog;
using Stend.Production.Calibration.Controls;
using Stend.Production.Root;
using System.Collections.Generic;
using System.Windows.Forms;
using ZIP.DLMS;

namespace Stend.Production.Calibration
{
    public class PlugIn : PluginBase, ISteps
    {
        private string _pluginName = "Calibration";
        private string _displayPlaginName = "Калибровка измерителей ПУ";
        private string _pluginDescription = "Выполняет калибровку заданных измерителей ПУ";
        private string _version = "1.1";
        private IPluginHost _host;
        private string _obis;
        private string _value;
        private int _attribute;
        private int _scaler;
        private string _script;
        //private string _script = "namespace Test\r\n {\r\n public class Calibrator\r\n {\r\n public static double Calculate(double BaseValue, double Value, double calibCoef)" +
        //    "\r\n {\r\n  if (BaseValue == 0 || Value == 0)\r\n return 0;\r\n var calc = (Value - BaseValue) / BaseValue * 100;" +
        //    "\r\n double res = (int)((100 / (calc + 100) - 1) * calibCoef);\r\n return res;\r\n }\r\n }\r\n }\r\n";
        private string _gaugeObis;
        private int _guageAttribute;
        private bool _needUserAction;


        [JsonProperty("NeedUserAction")]
        public bool NeedUserAction { get => _needUserAction; set => _needUserAction = value; }
        [JsonProperty("GuageAttribute")]
        public int GuageAttribute { get => _guageAttribute; set => _guageAttribute = value; }
        [JsonProperty("GuageOBIS")]
        public string GuageOBIS { get => _gaugeObis; set => _gaugeObis = value; }
        [JsonProperty("Script")]
        public string Script { get => _script; set => _script = value; }
        [JsonProperty("Scaler")]
        public int Scaler { get => _scaler; set => _scaler = value; }
        [JsonProperty("OBIS")]
        public override string OBIS { get => _obis; set => _obis = value; }

        [JsonProperty("Value")]
        public override string Value { get => _value; set => _value = value; }

        [JsonProperty("Attribute")]
        public override int Attribute
        {
            get { return _attribute; }
            set { _attribute = value; }
        }
        public override List<PluginBase> _internalSteps { get; set; }

        public override List<PluginBase> InternalSteps
        {
            get;
            set;
        }

        public override EnumStepStatus Status { get; set; } = EnumStepStatus.None;

        CalibControl _control = null;

        public override UserControl EditControl
        {
            get
            {
                if (_control == null) _control = new CalibControl();
                return _control;
            }
        }

        public override IPluginHost Host
        {
            get { return _host; }
            set
            {
                _host = value;
                _host.Register(this);
            }
        }

        public override string Name { get { return _pluginName; } }

        public override string StepName
        {
            get { return _displayPlaginName; }
            set { _displayPlaginName = value; }
        }
        public override string Description
        {
            get { return _pluginDescription; }
            set { _pluginDescription = value; }
        }

        public override string Version
        {
            get { return _version; }
        }

        public override void ExecuteStep(GXDLMSDevice device)
        {
            AddLog(this.ToString(), LogLevel.Debug);
            if (InternalSteps != null)
            {
                foreach (var step in InternalSteps)
                {
                    step.ExecuteStep(device);
                }
            }
        }

        public override void Show()
        {
            MainForm frm = new MainForm(this);

            frm.ShowDialog();
        }
    }
}
