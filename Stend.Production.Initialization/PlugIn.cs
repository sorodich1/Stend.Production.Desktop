using Newtonsoft.Json;
using NLog;
using Stend.Production.Initialization.Controls;
using Stend.Production.Initialization.Forms;
using Stend.Production.Root;
using System.Collections.Generic;
using System.Windows.Forms;
using ZIP.DLMS;

namespace Stend.Production.Initialization
{
    public class PlugIn : PluginBase, ISteps
    {
        private string _pluginName = "Initialization";
        private string _displayPlaginName = "Настройка объектов ПУ";
        private string _pluginDescription = "Выполнение Настройки списка объектов";
        private string _version = "1.1";
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
        public override List<PluginBase> _internalSteps { get; set; }

        public override List<PluginBase> InternalSteps
        {
            get;
            set;
        }

        public override EnumStepStatus Status { get; set; } = EnumStepStatus.None;

        InitControl _control = null;

        public override UserControl EditControl
        {
            get
            {
                if (_control == null) _control = new InitControl();
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
                    //if(step.OBIS == "0.0.10.164.1.255")
                    //{
                    //    try
                    //    {
                    //        step.FireEvent("Ожидание перезагрузки устройства");

                    //        int counter = 0;
                    //        while(counter < 24)
                    //        {
                    //            Thread.Sleep(5000);
                    //            if(device.ReadyToExchange)
                    //            {
                    //                try
                    //                {
                    //                    device.Comm.KeepAlive();
                    //                }
                    //                catch(Exception)
                    //                {
                    //                    device.Disconnect();
                    //                    break;
                    //                }
                    //            }
                    //        }
                    //        if(counter >= 24)
                    //        {
                    //            throw new Exception("Таймаут ожидания отключения");
                    //        }

                    //        Thread.Sleep(1000);
                    //        device.InitializeConnection();
                    //    }
                    //    catch(Exception)
                    //    {
                    //        step.Status = EnumStepStatus.Fatal;
                    //        throw;
                    //    }
                    //}
                    step.Status = EnumStepStatus.Succes;
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
