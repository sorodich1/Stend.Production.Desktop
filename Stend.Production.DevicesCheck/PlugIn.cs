using Newtonsoft.Json;
using Stend.Production.DevicesCheck.Controls;
using Stend.Production.DevicesCheck.Forms;
using Stend.Production.Root;
using System.Collections.Generic;
using System.Windows.Forms;
using ZIP.DLMS;

namespace Stend.Production.DevicesCheck
{
    public class PlugIn : PluginBase, ISteps
    {
        private string _pluginName = "DevicesCheck";
        private string _displayPlaginName = "Проверка устройства";
        private string _pluginDescription = "Выполнение сверки объектов с базой данных";
        private string _version = "1.1";
        private IPluginHost _host;
        private string _obis;
        private string _value;
        private int _attribute;


        public static List<PlugIn> plugIns = new List<PlugIn>();
        public override string Name { get => _pluginName; }

        public override string StepName { get => _displayPlaginName; set => _displayPlaginName = value; }

        [JsonIgnore]
        public int[] StructUi { get => Convert(Value); }

        public override string Description { get => _pluginDescription; set => _pluginDescription = value; }

        public override string Version { get => _version; }

        public override string OBIS { get => _obis; set => _obis = value; }

        public override int Attribute { get => _attribute; set => _attribute = value; }

        public override List<PluginBase> _internalSteps { get; set; }


        public override string Value { get => _value; set => _value = value; }

        private int[] Convert(string col)
        {
            int[] numbers = null;
            if (col != null)
            {
                string[] substring = col.Split(',');
                numbers = new int[substring.Length];
                for(int i = 0; i < substring.Length; i++)
                {
                    string trimmedSubstring = substring[i].Trim();
                    numbers[i] = int.Parse(trimmedSubstring);
                }
            }
            return numbers;
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

        DeviceCheckControl _control = null;

        public override UserControl EditControl
        {
            get
            {
                if(_control == null)
                {
                    _control = new DeviceCheckControl();
                }
                return _control;
            }
        }

        public override EnumStepStatus Status { get; set; } = EnumStepStatus.None;

        public override void ExecuteStep(GXDLMSDevice device)
        {
           
        }

        public override void Show()
        {
            MainForm frm = new MainForm(this);
            frm.ShowDialog();
        }
    }
}
