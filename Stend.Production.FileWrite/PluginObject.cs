using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;
using Newtonsoft.Json;
using Stend.Production.FileWrite.Controls;
using Stend.Production.Root;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ZIP.DLMS;

namespace Stend.Production.FileWrite
{
    public class PluginObject : PluginBase, ISteps
    {
        private string _pluginName = "FileWriteControl";
        private string _displayPlaginName = "Запись объекта ПУ из файла";
        private string _pluginDescription = "Выполнение записи объекта Сподес из загруженного файла";
        private string _version = "1.1";
        private IPluginHost _host;
        private string _obis;
        private int _attribute;

        [JsonIgnore]
        public static List<CaptureGridObject> AutoscrollIndication { get; set; }

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
                plugin.FileValue = this.FileValue;
            }
            catch (Exception ex)
            {
                plugin = null;
            }
            return plugin;
        }

        [JsonProperty("OBIS")]
        public override string OBIS { get => _obis; set => _obis = value; }

        [JsonProperty("Name")]
        public override string Name => _pluginName;

        [JsonProperty("StepName")]
        public override string StepName { get => _displayPlaginName; set => _displayPlaginName = value; }

        [JsonProperty("Description")]
        public override string Description { get => _pluginDescription; set => _pluginDescription = value; }

        [JsonProperty("Version")]
        public override string Version => _version;

        [JsonProperty("Attribute")]
        public override int Attribute { get => _attribute; set => _attribute = value; }
        public override List<PluginBase> _internalSteps { get; set; }

        [JsonProperty("InternalSteps")]
        public override List<PluginBase> InternalSteps { get; set; }

        [JsonIgnore]
        public override string Value { get; set; }

        [JsonProperty("FileValue")]
        public List<object> FileValue = new List<object>();


        public override IPluginHost Host
        {
            get { return _host; }
            set
            {
                _host = value;
                _host.Register(this);
            }
        }

        SetupBaseControl _control = null;

        public override UserControl EditControl
        {
            get
            {
                if(_control == null)
                {
                    _control = new SetupBaseControl(this);
                }
                return _control;
            }
        }


        public override EnumStepStatus Status { get; set; } = EnumStepStatus.None;


        public override void ExecuteStep(GXDLMSDevice device)
        {
            try
            {
                Status = EnumStepStatus.Run;
                int num;
                if (Owner != null)
                {
                    device = Owner.Device;
                    num = device != null ? 1 : 0;
                }
                else
                {
                    num = 0;
                }
                if(num == 0)
                {
                    throw new Exception("Не определены настройки шага.");
                }
                Status = EnumStepStatus.Run;
                
                if(OBIS == "0.0.13.0.0.255")
                {
                    ZIPDLMSCalendar calendar = new ZIPDLMSCalendar();

                    foreach (var item in FileValue)
                    {
                        calendar = item as ZIPDLMSCalendar;
                    }

                    //Helpers.FileCalendarLoad(device.Objects.FindByLN(Gurux.DLMS.Enums.ObjectType.None, OBIS) ?? throw new Exception("Объект" + OBIS + " отсутствует в Ассоциации ПУ."), device, int { 3, 4, 5}, calendar);
                }
                if(OBIS == "0.0.21.0.2.255" || OBIS == "0.0.21.0.164.255" || OBIS == "0.0.21.0.1.255")
                {
                    GXDLMSObject obj = device.Objects.FindByLN(Gurux.DLMS.Enums.ObjectType.None, OBIS) ?? throw new Exception("Объект" + OBIS + " отсутствует в Ассоциации ПУ.");

                    List<CaptureGridObject> AutoscrollIndication = null;

                    foreach(var item in FileValue)
                    {
                        if(item is CaptureGridObject captureGrid)
                        {
                            AutoscrollIndication.Add(captureGrid);
                        }
                    }

                    Helpers.CaptureObjectWrite(device, AutoscrollIndication, obj, Attribute);
                }
            }
            catch(Exception ex)
            {
                Status = EnumStepStatus.Fatal;
                FireEvent(ex.Message ?? "");
                throw;
            }
        }


        public override void Show()
        {
            throw new NotImplementedException();
        }
    }
}
