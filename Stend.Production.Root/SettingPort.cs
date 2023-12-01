using Ankom.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using ZIP.DLMS;

namespace Stend.Production.Root
{
    public class StatusChangedEventArgs : EventArgs
    {
        public string EventText { get; set; }
        public PluginBase Step { get; set; }
        public StatusChangedEventArgs(string msg, PluginBase step = null) : base()
        {
            Step = step;
            EventText = msg;
        }
    }

    /// <summary>
    /// Делегат обработчика события изменения статуса процесса прошивку для указанного места стенда
    /// </summary>
    /// <param name="sender">источнк события <see cref="PlaceSetting"/></param>
    /// <param name="args">параметры события <see cref="StatusChangedEventArgs"/></param>
    public delegate void StatusChangedEventHandler(object sender, StatusChangedEventArgs args);

    /// <summary>
    /// Определяет настройки процесса прошивки для конкретного места стенда
    /// </summary>
    public class PlaceSetting 
    {
        private string stepStatus = "Не определено";
        private GXDLMSDevice _device = null;
        private string _port = "";
        private string _serial = "";
        public CancellationTokenSource _token = null;
        private byte[] _masterKey = new byte[0];

        public event StatusChangedEventHandler OnStatusChanged;

        public void FireStatusChanged(string statusDescription, PluginBase step)
        {
            string stepName = (step != null) ? $"[{step.StepName}]:" : string.Empty;
            StatusDescription = $"{stepName}{statusDescription}";
            if (OnStatusChanged != null)
                try
                {
                    OnStatusChanged(this, new StatusChangedEventArgs(statusDescription, step));
                }
                catch (Exception ex)
                {
                    throw;
                }
        }

        [JsonIgnore]
        public PluginBase ActiveStep { get; set; } = null;

        [JsonIgnore]
        public string StatusDescription
        {
            get => stepStatus;
            set
            {
                stepStatus = value;
            }
        }

        [JsonIgnore]
        public EnumStepStatus Status { get; set; } = EnumStepStatus.None;

        [JsonProperty("Tag")]
        public object Tag { get; set; } = null;

        [JsonIgnore]
        public bool Active => Status == EnumStepStatus.Run;

        public bool IsActive { get; set; }

        [JsonProperty("PlaceId")]
        public int PlaceId { get; set; } = 0;

        [JsonProperty("Port")]
        public string Port
        {
            get => _port;
            set
            {
                _port = value;
            }
        }

        public BindingList<PluginBase> Steps { get; set; }

        [JsonIgnore]
        public Task RunningTask { get; set; }

        [JsonProperty("Serial")]
        public string Serial
        {
            get => _serial;
            set
            {
                _serial = value;
            }
        }

        [JsonIgnore]
        public byte[] MasterKey
        {
            get => _masterKey;
            set => _masterKey = value;
        }

        public GXDLMSDevice Device => _device;

        public void SetDevice(string jsonSettings)
        {
            try
            {
                _device = JsonConvert.DeserializeObject<GXDLMSDevice>(jsonSettings);
                _device.Name = $"#{Serial}";
                _device.MediaSettings = $"<Port>{Port}</Port>";
            }
            catch(Exception ex)
            {
                _device = null;
                throw;
            }
        }

        private void StepEvent(object sender, string runState)
        {
            if(sender is PluginBase step)
            {
                FireStatusChanged($"{runState}", step);
            }
        }

        public void CancelTask()
        {
            if (_token != null)
            {
                _token.Cancel();
            }
        }

        public void Execute()
        {
            if (String.IsNullOrEmpty(Serial) || String.IsNullOrEmpty(Port))
            {
                FireStatusChanged($"Проверьте базовые настройки (ПУ #{Serial}, порт: {Port})", null);
                return;
            }
            if (Device == null)
            {
                FireStatusChanged("Не определен прибор учета для настройки", null);
                return;
            }
            if (Steps == null || Steps.Count == 0)
            {
                FireStatusChanged($"Не определены шаги настройки ПУ #{Serial}, порт [{Port}]", null);
                return;
            }
            if (RunningTask != null && !RunningTask.IsCompleted)
            {
                FireStatusChanged($"Задача настройки (ПУ #{Serial}, порт: {Port}) ещё не завершена", null);
                return;
            }
            if (_token != null)
            {
                _token.Dispose();
                _token = null;
                RunningTask = null;
            }
            _token = new CancellationTokenSource();
            CancellationToken ct = _token.Token;

            RunningTask = Task.Run(() =>
            {
                try
                {
                    ct.ThrowIfCancellationRequested();

                    ThrowIfCancelled(null);
                    Status = EnumStepStatus.Run;
                    FireStatusChanged($"{Status.ToName()} подключение к ПУ", null);

                    Device.InitializeConnection();

                    Helpers.Log.Error($"{Device.AuthenticationName} -- {Device.MediaSettings}");

                    foreach(var item in Steps)
                    {
                        ThrowIfCancelled(null);
                        item.OnStepEvent += StepEvent;

                        item.ExecuteStep(Device);
                    }

                    Status = EnumStepStatus.Succes;
                    FireStatusChanged($"Статус процесса прошивки: {Status.ToName()}", null);

                    Device.Disconnect();
                }
                catch(Exception ex)
                {
                    Status = EnumStepStatus.Fatal;
                    Device.Disconnect();
                    FireStatusChanged($"{Status.ToName()} процесса прошивки: {ex.Message}", null);
                }


                void ThrowIfCancelled(PluginBase step)
                {
                    if(ct.IsCancellationRequested)
                    {
                        Status = EnumStepStatus.Cancel;
                        FireStatusChanged("Отменено пользователем", step);
                        ct.ThrowIfCancellationRequested();
                    }
                }
            }, _token.Token);
            RunningTask = null;
        }
    }

    public class SettingPort
    {
        private List<PlaceSetting> _object = new List<PlaceSetting>();

        [JsonProperty("Objects")]
        public List<PlaceSetting> Object
        {
            get
            {
                return _object;
            }
            set
            {
                _object = value;
            }
        }
    }
}
