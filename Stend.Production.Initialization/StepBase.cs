using Gurux.DLMS.Enums;
using Newtonsoft.Json;
using NLog;
using Stend.Production.Initialization.Controls;
using Stend.Production.Initialization.Forms;
using Stend.Production.Root;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ZIP.DLMS;

namespace Stend.Production.Initialization
{
    public class StepBase : PluginBase, ISteps
    {
        private string _pluginName = "SettingObject";
        private string _displayPlaginName = "Настройка объекта ПУ";
        private string _pluginDescription = "Выполнение Настройки указанного объекта";
        private string _version = "1.00";
        private IPluginHost _host;
        private string _obis;
        private string _value;
        private int _attribute;

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

        public override string Name
        {
            get { return _pluginName; }
        }

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

        public override List<PluginBase> InternalSteps { get; set; }

        public override EnumStepStatus Status { get; set; } = EnumStepStatus.None;

        SetupBaseControl _control;

        public override UserControl EditControl
        {
            get
            {
                if (_control == null)
                {
                    _control = new SetupBaseControl(this);
                }
                return _control;
            }
        }

        public override IPluginHost Host
        {
            get
            {
                return _host;
            }
            set
            {
                _host = value;
                _host?.Register(this);
            }
        }

        public override List<PluginBase> _internalSteps { get; set; }

        public override void ExecuteStep(GXDLMSDevice device)
        {
            throw new NotImplementedException();
        }

        public override void Show()
        {
            using (SettingInitStep dig = new SettingInitStep(this))
            {
                dig.ShowDialog();
            }
        }

        public override string ToString()
        {
            string obis = String.IsNullOrEmpty(OBIS) ? String.Empty : $"[{OBIS}]";
            return base.ToString() + obis;
        }

        public override void AddLog(object msg, LogLevel level = null)
        {
            base.AddLog(msg, level);
        }
    }
}
