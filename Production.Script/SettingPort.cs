using Ankom.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ZIP.DLMS;
using System.Threading;
using System.Windows.Forms;
using Production.Script.Calendar;
using Gurux.DLMS;
using Gurux.DLMS.Objects;

namespace Production.Script
{
    /// <summary>
    /// Аргументы события <see cref="StatusChangedEventHandler"/>
    /// </summary>
    public class StatusChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Текстовое описание события (изменения статуса и т.д.)
        /// </summary>
        public string EventText { get; set; }
        /// <summary>
        /// Шаг скрипта настройки
        /// </summary>
        public BaseStep Step { get; set; }

        public StatusChangedEventArgs(string msg, BaseStep step = null) : base()
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
    public class PlaceSetting //: INotifyPropertyChanged
    {
        private string stepStatus = "Не определено";
        private GXDLMSDevice _device = null;
        private string port = "";
        private string serial = "";
        public CancellationTokenSource tokenSource = null;
        private byte[] _masterKey = new byte[0];

        public event StatusChangedEventHandler OnStatusChanged;
        public void FireStatusChanged(string statusDescription, BaseStep step)
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
        public StendSettings StendSettings { get; set; } = null;

        [JsonIgnore]
        public Task RunningTask { get; set; } = null;

        [JsonIgnore]
        public GXDLMSDevice Device => _device;

        [JsonIgnore]
        public byte[] MasterKey
        {
            get => _masterKey;
            internal set => _masterKey = value;
        }

        [JsonIgnore]
        public bool Active => Status == EnumStepStatus.Run;

        public bool IsActiv { get; set; }

        [JsonIgnore]
        public bool IsDuplicateSerialNumber { get; set; } = true;

        [JsonProperty("PlaceId")]
        public int PlaceId { get; set; } = 0;

        [JsonProperty("Serial")]
        public string Serial
        {
            get => serial;
            set
            {
                serial = value;
                FirePropertyChanged(nameof(Serial));
            }
        }

        [JsonProperty("Port")]
        public string Port
        {
            get => port;
            set
            {
                port = value;
                FirePropertyChanged(nameof(Port));
            }
        }

        [JsonIgnore]
        public BaseStep ActiveStep { get; set; } = null;

        [JsonIgnore]
        public string StatusDescription
        {
            get => stepStatus;
            set
            {
                stepStatus = value;
                FirePropertyChanged("Status");
            }
        }

        [JsonIgnore]
        public EnumStepStatus Status { get; set; } = EnumStepStatus.None;

        public BindingList<BaseStep> Steps { get; set; }

        public PlaceSetting()
        {
        }

        public PlaceSetting(string serial, string port, int placeId)
        {
            Serial = serial;
            Port = port;
            PlaceId = placeId;
        }

        /// <summary>
        /// Загрузка параметров счетчика из строки настроек <see cref="jsonSettings"/>  
        /// См. файлы настроек конфигуратора
        /// </summary>
        /// <param name="jsonSettings">параметры счетчика в формате JSON</param>
        public void SetDevice(string jsonSettings)
        {
            try
            {
                _device = JsonConvert.DeserializeObject<GXDLMSDevice>(jsonSettings);
                _device.Name = $"#{Serial}";
                _device.MediaSettings = $"<Port>{Port}</Port>";
                FirePropertyChanged("Device");
            }
            catch
            {
                _device = null;
                throw;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void FirePropertyChanged([CallerMemberName] string prop = "")
        {
            //if (PropertyChanged != null)
            //    PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        public void CancelTask()
        {
            if (tokenSource != null)
            {
                tokenSource.Cancel();
            }
        }

        //Общий метод выполнения
        public void Execute()
        {
            // проверяем наличие необходимых настроек
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

            if (tokenSource != null)
            {
                tokenSource.Dispose();
                tokenSource = null;
                RunningTask = null;
            }

            tokenSource = new CancellationTokenSource();
            CancellationToken ct = tokenSource.Token;

            RunningTask = Task.Run(() =>
            {
                try
                {
                    // Were we already canceled?
                    ct.ThrowIfCancellationRequested();

                    ThrowIfCancelled(null);
                    Status = EnumStepStatus.Run;
                    FireStatusChanged($"{Status.ToName()} подключение к ПУ", null);

                    Device.InitializeConnection();

                    Helpers.LogError.Error($"{Device.AuthenticationName} -- {Device.MediaSettings}");

                    foreach (var item in Steps)
                    {
                        ThrowIfCancelled(null);
                        item.OnStepEvent += StepEvent;

                        //Добавление устройства в список для ожидания ответа оператора

                        // изменение статуса шага отслеживается в StepEvent для изменения статуса всего задания  
                        item.ExecuteStep();
                    }
                    Status = EnumStepStatus.Succes;
                    FireStatusChanged($"Статус процесса прошивки: {Status.ToName()}", null);
                    Device.Disconnect();
                }
                catch (Exception ex)
                {
                    Device.Disconnect();
                    Status = EnumStepStatus.Fatal;
                    FireStatusChanged($"{Status.ToName()} процесса прошивки: {ex.Message}", null);
                }
            }, tokenSource.Token); // Pass same token to Task.Run.

            RunningTask = null;


            // генерим исключение, если прервано пользователем
            void ThrowIfCancelled(BaseStep step)
            {
                if (ct.IsCancellationRequested)
                {
                    // Clean up here, then...
                    Status = EnumStepStatus.Cancel;
                    FireStatusChanged("Отменено пользователем", step);
                    ct.ThrowIfCancellationRequested();
                }
            }
        }

        /// <summary>
        /// Обработчик события выполнения шага задания
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="runState"></param>
        private void StepEvent(object sender, string runState)
        {
            if (sender is BaseStep step)
            {
                FireStatusChanged($"{runState}", step);
            }
        }

        public override string ToString()
        {
            return $"#{Serial}:{Port}";
        }
    }

    public enum UserActionStatus
    {
        /// <summary>
        /// Переход в режим ожидания ответа
        /// </summary>
        Start,
        /// <summary>
        /// Отображение диалога ожидания действий
        /// </summary>
        WaitReply,
        /// <summary>
        /// Подтверждение оператором выполнения требуемых действий
        /// </summary>
        Ok,
        /// <summary>
        /// Прерывание оператором (отказ от) выполнения требуемых действий
        /// </summary>
        Abort
    }

    public class StendSettings
    {
        private List<PlaceSetting> _objects = new List<PlaceSetting>();

        /// <summary>
        /// Дополнительные параметры, которые возможно могут понадобиться...
        /// </summary>
        [JsonProperty("Tag")]
        public object Tag { get; set; } = null;

        /// <summary>
        /// Отключение полной автоматической калибровки
        /// </summary>
        public bool IsStopFinalization { get; set; }
        /// <summary>
        /// Включение полной автоматической калибровки
        /// </summary>
        public bool IsFullCalibration { get; set; }

        /// <summary>
        /// Таймаут неактивности в секундах
        /// </summary>
        public string InactivityTimeout { get; set; }

        /// <summary>
        /// Процент расхождение коэфициента калибровки с реальным значением
        /// </summary>
        public string RangeValues { get; set; }

        public ZIPDLMSCalendar Calendar { get; set; }

        public List<CaptureGridObject> ButtonIndication { get; set; }
        public List<CaptureGridObject> ButtonIndication2 { get; set; }
        public List<CaptureGridObject> AutoscrollIndication { get; set; }
        public BindingList<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> CaptureObject { get; set; } = new BindingList<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>();

        /// <summary>
        /// Номер партии для записи в БД 
        /// </summary>
        [JsonProperty("NumberBranch")]
        public int NumberBatch { get; set; }

        /// <summary>
        /// Выход из режима производства
        /// </summary>
        [JsonProperty("ExitText")]
        public string ExitText { get; set; }

        public FirmwareInfo FirmwareInfo { get; set; }

        [JsonProperty("CommonParameters")]
        public List<WriteSimpleObject> CommonParameters { get; set; } = new List<WriteSimpleObject>();

        [JsonProperty("Objects")]
        public List<PlaceSetting> Objects
        {
            get => _objects;
            set
            {
                _objects = value;
            }
        }

        /// <summary>
        /// Вызываем из UI стенда для настройки (установки) параметров, 
        /// общих для всех счетчиков в данном сеансе прошивки
        /// </summary>
        /// <param name="steps"></param>
        public void GenerateCommonParameters(List<BaseStep> steps, bool recreate = false)
        {
            if (recreate)
                CommonParameters = new List<WriteSimpleObject>();
            foreach (BaseStep step in steps)
            {
                var commonParam = step.InternalSteps?.FirstOrDefault(item => (item is WriteSimpleObject) && ((WriteSimpleObject)item).IsCommon) as WriteSimpleObject;
                if (commonParam != null)
                    CommonParameters.Add(commonParam);
            }
        }
    }
}
