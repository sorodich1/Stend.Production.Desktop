using Gurux.DLMS;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;
using Newtonsoft.Json;
using NLog;
using Stend.Production.Root;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ZIP.DLMS;

namespace Stend.Production.ReadObject
{
    public class PluginObject : PluginBase, ISteps
    {
        private string _pluginName = "ReadControl";
        private string _displayPlaginName = "Чтение объекта ПУ";
        private string _pluginDescription = "Выполнение чтения объекта Сподес";
        private string _version = "1.1";
        private IPluginHost _host;
        private string _obis;
        private int _attribute;
        private string _value;

        [JsonProperty("OBIS")]
        public override string OBIS { get => _obis; set => _obis = value; }

        [JsonProperty("Value")]
        public override string Value { get => _value; set => _value = value; }

        public int AttributeInx { get; set; } = 2;

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

        public PluginObject Clone()
        {
            PluginObject plugin = new PluginObject();
            try
            {
                plugin.Attribute = this.Attribute;
                plugin.Childrens = this.Childrens;
                plugin.Description = this.Description;
                plugin.ID = this.ID;
                plugin.InternalSteps = this.InternalSteps;
                plugin.OBIS = this.OBIS;
                plugin.Order = this.Order;
                plugin.Owner = this.Owner;
                plugin.Parent = this.Parent;
                plugin.StepName = this.StepName;
                plugin.Value = this.Value;
            }
            catch(Exception ex)
            {
                plugin = null;
            }
            return plugin;
        }

        public override EnumStepStatus Status { get; set; } = EnumStepStatus.None;

        SetupBaseControl _control = null;

        public override UserControl EditControl
        {
            get
            {
                if (_control == null) _control = new SetupBaseControl(this);
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
            try
            {
                if (device == null)
                {
                    throw new Exception("Не определены настройки шага.");
                }
                Status = EnumStepStatus.Run;
                // загружаем объект из Ассоциации для проверким его наличия и устаноавки корректных атрибутов
                GXDLMSObject obj = device.Objects.FindByLN(ObjectType.None, OBIS);

                if (obj == null)
                    throw new Exception($"Объект {OBIS} отсутствует в Ассоциации ПУ.");

                GXDLMSSettings settings = device.Comm.client.Settings;

                Helpers.ReadAttributeValue(device, obj, Attribute);

                _value = (obj as IGXDLMSBase).GetValue(settings, new ValueEventArgs(obj, AttributeInx, 0, null)).ToString();

                // отправляем сообщение в общий лог и лог счетчика
                FireEvent($"Текущее значение [{Value}]");
            }
            catch(Exception ex)
            {
                Status = EnumStepStatus.Fatal;
                FireEvent($"{ex.Message}");
                throw;
            }
        }

        public override void Show()
        {
            MainForm frm = new MainForm(this);

            frm.ShowDialog();
        }
    }
}
