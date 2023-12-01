using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using ZIP.DLMS;

namespace Production.Script
{
    interface IStep
    {
        /// <summary>
        /// Название исполняемого шага настройки счетчика
        /// </summary>
        [JsonProperty("StepName")]
        string StepName { get; }

        /// <summary>
        /// Вложенные шаги настройки
        /// </summary>
        List<object> InternalSteps { get; }

        /// <summary>
        /// Статус исполнения текущего шага <see cref="EnumStepStatus"/>
        /// </summary>
        [JsonIgnore]
        EnumStepStatus Status { get; set; }

        /// <summary>
        /// Запускает на исполнение указанный вложенный шаг настройки
        /// </summary>
        /// <param name="internalStep">экземпляр вложенного шага</param>
        void ExecuteMetod(object internalStep);

        /// <summary>
        /// Исполняет весь перечень действий, предусмотренных шагом настройки
        /// </summary>
        void ExecuteStep();
    }

    public delegate void ExecuteScriptEvent(object sender, string msg);

    public class BaseStep : IStep
    {
        /// <summary>
        /// Настройки места выпуска ПУ. Устанавливаются на этапе запуска прошивки
        /// </summary>
        [JsonIgnore]
        public PlaceSetting Owner { get; set; } = null;

        [JsonIgnore]
        public ExecuteScriptEvent OnStepEvent { get; set; }

        public virtual void FireEvent(string msg = null, BaseStep internalStep = null)
        {
            // установка активного шага прошивки
            if (Owner != null)
                if (internalStep != null)
                {
                    Owner.ActiveStep = internalStep;
                }
                else
                {
                    Owner.ActiveStep = internalStep;
                }
            OnStepEvent?.Invoke(this, msg);
        }

        [Description("Имя шага")]
        [JsonProperty("StepName")]
        public string StepName { get; set; }

        private List<object> _properties;
        /// <summary>
        /// Список вложенных шагов настройки ПУ (предпочтительно использование типа BaseStep), исполняемых на текущем шаге прошивки.
        /// </summary>
        public List<object> InternalSteps
        {
            get => _properties;
            set
            {
                _properties = value;
                if (_properties != null)
                    foreach (var item in _properties)
                        if (item is BaseStep step)
                            step.OnStepEvent += FireInternalStepEvent;
            }
        }

        //Реализовать возможность добавлять вложенные шаги.

        private void FireInternalStepEvent(object sender, string msg)
        {
            if (sender is BaseStep internalStep)
            {
                FireEvent($"[{internalStep.StepName}]: {msg}", internalStep);
            }
        }

        /// <summary>
        /// Текущий статус выполнения шага
        /// </summary>
        [Description("Статус")]
        [JsonIgnore]
        public EnumStepStatus Status { get; set; } = EnumStepStatus.None;

        // TODO: Вроде бы пока нигде не используем? Удалить при рефакторинге
        /// <summary>
        /// Выполнение указанного действия шага настройки ПУ
        /// Передаваемый аргумент (parameters) должен однозначно идентифицировать исполняемое действие
        /// </summary>
        /// <param name="parameters">параметры выполнения метода</param>
        public virtual void ExecuteMetod(object parameters)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Выполнение указанного шага настройки ПУ 
        /// </summary>
        public virtual void ExecuteStep()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Перечисление доступных состояний выполнения шага настройки ПУ
    /// </summary>
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

}
