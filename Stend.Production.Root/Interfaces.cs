using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using ZIP.DLMS;

namespace Stend.Production.Root
{
    public interface ISteps
    {
        string Name { get; }
        /// <summary>
        /// Название исполняемого шага настройки счетчика
        /// </summary>
        [JsonProperty("StepName")]
        string StepName { get; set; }

        string Version { get; }

        string Description { get; set; }
        /// <summary>
        /// Вложенные шаги настройки
        /// </summary>
        List<PluginBase> InternalSteps { get; }
        /// <summary>
        /// Статус исполнения текущего шага <see cref="EnumStepStatus"/>
        /// </summary>
        [JsonIgnore]
        EnumStepStatus Status { get; set; }
        /// <summary>
        /// Исполняет весь перечень действий, предусмотренных шагом настройки
        /// </summary>
        void ExecuteStep(GXDLMSDevice device);



        void Show();
    }

    public interface IPluginHost
    {
        bool Register(PluginBase plug);
        Logger Log { get; set; }
    }

    public delegate void ExecuteScriptEvent(object sender, string msg);

    public abstract class PluginBase : ISteps
    {
        public abstract string Name { get; }

        public abstract string StepName { get; set; }

        public abstract string Description { get; set; }

        public abstract string Version { get; }

        public abstract string OBIS { get; set; }

        public abstract int Attribute { get; set; }

        [JsonIgnore]
        public abstract List<PluginBase> _internalSteps { get; set; }

        public abstract string Value { get; set; }

        public virtual UInt16 Order { get; set; } = 0;

        public virtual int ID { get; set; }

        [JsonIgnore]
        public abstract IPluginHost Host { get; set; }

        [JsonIgnore]
        public abstract UserControl EditControl { get; }

        private PluginBase _parent = null;

        public abstract EnumStepStatus Status { get; set; }

        public abstract void ExecuteStep(GXDLMSDevice device);

        public abstract void Show();

        private List<PluginBase> _properties;

        [JsonIgnore]
        public ExecuteScriptEvent OnStepEvent { get; set; } = null;

        public virtual List<PluginBase> InternalSteps
        {
            get
            {
                return _properties;
            }
            set
            {
                _properties = value;
                if(_properties != null)
                {
                    foreach(var item in _properties)
                    {
                        if (item is PluginBase step)
                            step.OnStepEvent += FireInternalStepEvent;
                    }
                }
            }
        }

        private void FireInternalStepEvent(object sender, string msg)
        {
            if(sender is PluginBase internalSender)
            {
                FireEvent($"[{internalSender}]: {msg}", internalSender);
            }
        }

        public virtual void FireEvent(string msg = null, PluginBase internalSender = null)
        {
            if(Owner != null)
            {
                if(internalSender != null)
                {
                    Owner.ActiveStep = internalSender;
                }
                else
                {
                    Owner.ActiveStep = internalSender;
                }
            }
            OnStepEvent?.Invoke(this, msg);
        }

        [JsonIgnore]
        public virtual PluginBase Parent
        {
            get { return _parent;  }
            set { _parent = value; if (_parent != null) Host = _parent.Host; }
        }

        List<PluginBase> _childrens = null;

        [JsonIgnore]
        public virtual PluginBase[] Childrens
        {
            get => _childrens?.ToArray();
            set => _childrens = value?.ToList();
        }

        /// <summary>
        /// Настройки устройства устанавливаемые при запуске калибровки
        /// </summary>
        [JsonIgnore]
        public PlaceSetting Owner { get; set; } = null;

        public virtual void AddLog(object msg, LogLevel level = null)
        {
            if(level == null)
            {
                level = LogLevel.Info;
            }
            if(Parent != null)
            {
                Parent.AddLog(msg);
            }
            else if(Host != null && Host.Log != null && Host.Log.IsEnabled(level))
            {
                Host.Log.Log(level, msg.ToString());
            }
        }

        public override string ToString()
        {
            return $"{StepName}: {Version}";
        }
    }

    public enum EnumStepStatus
    {
        [Description("Не определёно")]
        None,
        [Description("Выполняется")]
        Run,
        [Description("Завершено успешно")]
        Succes,
        [Description("Критическая ошибка")]
        Fatal,
        [Description("Прервано пользователем")]
        Cancel
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
}
