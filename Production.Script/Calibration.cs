using Ankom.Common;
using Gurux.DLMS;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;
using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using ZIP.DLMS;

namespace Production.Script
{
    public class CalibrationProcess : BaseStep
    {
        public CalibrationProcess() : base()
        {
            StepName = "Калибровка прибора учета";
        }

        /// <summary>
        /// Выполняет действия по калибровке прибора учета
        /// </summary>
        public override void ExecuteStep()
        {
            if (Owner != null)
            {
                GXDLMSDevice device = Owner.Device;
                if (device != null && InternalSteps != null)
                {
                    Status = EnumStepStatus.Run;
                    FireEvent("запущено на выполнение");
                    if (InternalSteps.Count > 0)
                        try
                        {
                            foreach (var value in InternalSteps)
                            {
                                if (value is BaseStep internalStep)
                                {
                                    internalStep.Owner = Owner;
                                    try
                                    {
                                        internalStep.ExecuteStep();
                                    }
                                    catch
                                    {
                                        internalStep.Status = EnumStepStatus.Fatal;
                                        throw;
                                    }
                                    internalStep.Status = EnumStepStatus.Succes;
                                }
                                else
                                    throw new Exception($"ожидается шаг задания типа BaseStep, получен {value.GetType().Name}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Status = EnumStepStatus.Fatal;
                            string err = $"{Status.ToName()} выполнения шага [{StepName}]: {ex.Message}";
                            FireEvent(err/*$"{ex.Message}"*/);
                            throw new Exception(err);
                        }
                    Status = EnumStepStatus.Succes;
                    FireEvent(Status.ToName());
                }
            }
            else
            {
                Status = EnumStepStatus.Fatal;
                string err = $"{Status.ToName()} выполнения шага [{StepName}]: отсутствуют настройки места прошивки";
                FireEvent(err);
                throw new Exception(err);
            }
        }
    }

    /// <summary>
    /// Базовый класс шага калибровки объекта
    /// </summary>
    public class CalibrationStep : WriteSimpleObject
    {
        private string _qaugeOBIS;
        private object _guageValue;
        private double modulResult;
        private double result = 0.0;
        private string _calibrationScript = "namespace Test\r\n {\r\n public class Calibrator\r\n {\r\n public static double Calculate(double BaseValue, double Value, double calibCoef)\r\n {\r\n  if (BaseValue == 0 || Value == 0)\r\n return 0;\r\n var calc = (Value - BaseValue) / BaseValue * 100;\r\n double res = (int)((100 / (calc + 100) - 1) * calibCoef);\r\n return res;\r\n }\r\n }\r\n }\r\n";

        /// <summary>
        /// Эталонное значение для калибровки объекта. 
        /// Конвертируется в нужный тип на шаге выполнения
        /// </summary>
        [JsonIgnore]
        public string ReferenceValue { get => base.Value; set => base.Value = value; }

        /// <summary>
        /// Коэффициент масштабирования показателя, используемый в процессе калибровки
        /// </summary>
        public object Scaler { get; set; } = 32767;

        /// <summary>
        /// Скрипт калибровки объекта. 
        /// </summary>
        /// <remarks>Скрипт пишется на C#. По умолчанию используем пространство имен Test, класс Calibrators, метод Calculate</remarks>
        public string Script { get => _calibrationScript; set => _calibrationScript = value; }

        /// <summary>
        /// OBIS-код измерителя реального значения калибруемого показателя. 
        /// </summary>
        public string GaugeOBIS { get => _qaugeOBIS; set => _qaugeOBIS = value; }

        // Индекс атрибута измерителя значения калибруемого показателя (для чтения)
        public int GaugeAttributeInx { get; set; } = 2;

        // Признак необходимости ожидания действия пользователя перед началом калибровки
        // может потребоаться установка другого эталонного значения, перезапуск стенда и т.п.
        public bool NeedUserAction { get; set; } = false;

        /// <summary>
        /// Измеренное значение калибруемого показателя. 
        /// </summary>
        private object GaugeValue { get => _guageValue; }

        public CalibrationStep() : base()
        {
            StepName = "Калибровка объекта";
        }

        private void DeviceCalibration(GXDLMSDevice device, GXDLMSObject qaugeObj)
        {
            object currentGuageValue = null;

            // загружаем объект коэффициента калибровки из Ассоциации для проверким его наличия 
            GXDLMSObject coefObj = device.Objects.FindByLN(ObjectType.None, OBIS);
            var valueCoef = Helpers.ReadAttributeValue(device, coefObj, AttributeInx);


            if (coefObj == null)
                throw new Exception($"Объект коэффициента калибровки {OBIS} отсутствует в Ассоциации ПУ.");


            if (qaugeObj == null)
                throw new Exception($"Объект измерителя {GaugeOBIS} отсутствует в Ассоциации ПУ.");

            // загружаем эталонное значение показателя из общих настроек
            if (IsCommon)
            {
                WriteSimpleObject commonParam = Owner.StendSettings.CommonParameters.FirstOrDefault(item => item.OBIS == OBIS && item.AttributeInx == AttributeInx);
                ReferenceValue = commonParam?.Value;
            }

            if (string.IsNullOrWhiteSpace(ReferenceValue))
                throw new Exception($"Не задано эталонное значение для калибровки объекта {OBIS}.");

            GXDLMSSettings settings = device.Comm.client.Settings;

            // TODO: !!!! Делать этого не требуется, если в Ассоциации ПУ тип данных уже установлен !!!!
            // считываем текущий коэф. калибровки для корректировки типа данных
            if (coefObj.GetDataType(AttributeInx) == DataType.None)
                Helpers.ReadAttributeValue(device, coefObj, AttributeInx);

            // читаем значение, полученное от измерителя
            currentGuageValue = Helpers.ReadAttributeValue(device, qaugeObj, GaugeAttributeInx);

            // реализация только для измерителей типа GXData, GXDLMSRegister
            // в остальных случаях не гарантируется корректный расчет коэффициент калибровки 
            _guageValue = Helpers.GetPropertyValue(qaugeObj, "Value");
            if (_guageValue == null)
            {
                GXLogWriter.WriteLog("Внимание! Проверьте соответствие масштабных коэффициентов эталонного и измеренного значений...");
                _guageValue = currentGuageValue; // (qaugeObj as IGXDLMSBase).GetValue(settings, new ValueEventArgs(qaugeObj, GaugeAttributeInx, 0, null));
            }

            if(Convert.ToInt32(valueCoef) != 0)
            {
                Helpers.WriteAttributeValue(device, coefObj, AttributeInx, 0);

                Thread.Sleep(2000);
            }

            // читаем значение, полученное от измерителя
            currentGuageValue = Helpers.ReadAttributeValue(device, qaugeObj, GaugeAttributeInx);

            // реализация только для измерителей типа GXData, GXDLMSRegister
            // в остальных случаях не гарантируется корректный расчет коэффициент калибровки 
            _guageValue = Helpers.GetPropertyValue(qaugeObj, "Value");
            if (_guageValue == null)
            {
                GXLogWriter.WriteLog("Внимание! Проверьте соответствие масштабных коэффициентов эталонного и измеренного значений...");
                _guageValue = currentGuageValue; // (qaugeObj as IGXDLMSBase).GetValue(settings, new ValueEventArgs(qaugeObj, GaugeAttributeInx, 0, null));
            }

            if(Convert.ToInt32(_guageValue) > 1)
            {
                // рассчитываем коэффициент калибровки
                var ratio = Calculate();
                Helpers.WriteAttributeValue(device, coefObj, AttributeInx, ratio);
                 ratio = (coefObj as IGXDLMSBase).GetValue(settings, new ValueEventArgs(coefObj, AttributeInx, 0, null));
                Helpers.ReadAttributeValue(device, qaugeObj, GaugeAttributeInx);
                _guageValue = Helpers.GetPropertyValue(qaugeObj, "Value");
                result = Convert.ToDouble(_guageValue) - Convert.ToDouble(ReferenceValue);
                // отправляем сообщение в общий лог и лог счетчика
                FireEvent($"Текущее значение [{ratio}]");
                Helpers.LogError.Error($"Откалиброванное значение до включение итераций {_guageValue}");

                //Включение итераций при неудовлетварительной калибровки
                if (OBIS == "1.0.96.165.0.255")
                {
                    int i = 0;
                    while(result > 0.2 || result < -0.2 || i < 5)
                    {
                        Helpers.ReadAttributeValue(device, qaugeObj, GaugeAttributeInx);
                        _guageValue = Helpers.GetPropertyValue(qaugeObj, "Value");
                        result = Convert.ToDouble(_guageValue) - Convert.ToDouble(ReferenceValue);
                        if (result > 0.2)
                        {
                            var calib =  Convert.ToInt32(ratio) + 1;
                            Helpers.WriteAttributeValue(device, coefObj, AttributeInx, ratio);
                            Helpers.ReadAttributeValue(device, qaugeObj, GaugeAttributeInx);
                            _guageValue = Helpers.GetPropertyValue(qaugeObj, "Value");
                            result = Convert.ToDouble(_guageValue) - Convert.ToDouble(ReferenceValue);
                            FireEvent($"Текущее значение [{ratio}]");
                        }
                       else if(result < -0.2)
                        {
                            var calib = Convert.ToInt32(ratio) - 1;
                            Helpers.WriteAttributeValue(device, coefObj, AttributeInx, ratio);
                            Helpers.ReadAttributeValue(device, qaugeObj, GaugeAttributeInx);
                            _guageValue = Helpers.GetPropertyValue(qaugeObj, "Value");
                            result = Convert.ToDouble(_guageValue) - Convert.ToDouble(ReferenceValue);
                            FireEvent($"Текущее значение [{ratio}]");
                        }
                        i++;
                    }
                }
            }
        }

        public override void ExecuteStep()
        {
            try
            {
                if (Owner != null && Owner.Device is GXDLMSDevice device)
                {
                    Status = EnumStepStatus.Run;
                    if (Owner.StendSettings.RangeValues == "")
                        Owner.StendSettings.RangeValues = "1";

                    // загружаем объект измерителя из Ассоциации для проверким его наличия 
                    GXDLMSObject qaugeObj = device.Objects.FindByLN(ObjectType.None, GaugeOBIS);
                    var valueCoef = Helpers.ReadAttributeValue(device, qaugeObj, AttributeInx);

                    int index = 0;

                    //if (Convert.ToInt32(valueCoef) != 0)
                    //{
                    //    while (index < 10)
                    //    {
                            DeviceCalibration(device, qaugeObj);
                            index++;
                            double rangeValues = Convert.ToDouble(Owner.StendSettings.RangeValues); 
                            //if (rangeValues >= modulResult)
                            //{
                            //    break;
                            //}
                            //if(index == 10)
                            //{
                            //    throw new Exception("Превышен лимит количества итераций");
                            //}
                        }
                    //}
                //}
                else
                {
                    throw new Exception("Не определены настройки шага.");
                }
            }
            catch (Exception ex)
            {
                Status = EnumStepStatus.Fatal;
                FireEvent($"{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Виртуальный метод вычисления коэффициента калибровки.
        /// Переопределите метод при изменении алгоритма вычисления
        /// </summary>
        /// <returns></returns>
        public virtual object Calculate()
        {
            try
            {
                if (!string.IsNullOrEmpty(Script))
                {
                    CompilerResults _scriptResults = null;
                    CodeDomProvider _compiler = CodeDomProvider.CreateProvider("CSharp");
                    CompilerParameters _parametrs = new CompilerParameters
                    {
                        CompilerOptions = "/t:library",
                        GenerateInMemory = true,
                        IncludeDebugInformation = false
                    };

                    _scriptResults = _compiler.CompileAssemblyFromSource(_parametrs, Script);
                    var obj = _scriptResults.CompiledAssembly.CreateInstance("Test.Calibrator");
                    object[] arrObj = { Convert.ToDouble(ReferenceValue), Convert.ToDouble(GaugeValue), Convert.ToInt32(Scaler) };
                    MethodInfo method = _scriptResults.CompiledAssembly.GetType("Test.Calibrator").GetMethod("Calculate");
                    return Convert.ToInt32(method.Invoke(obj, arrObj));
                    // _editValue = 0;
                }
                else
                    throw new Exception("Скрипт калибровки не задан.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    /// <summary>
    /// Реализация класс шага ожидания действия пользователя
    /// </summary>
    public class UserActionPromptStep : BaseStep
    {
        //Список устройств участвующих в калибровки
        [JsonIgnore]
        public static List<PlaceSetting> places = new List<PlaceSetting>();

        public bool ReconnectDevice { get; set; } = true;

        public UserActionPromptStep() => StepName = "Ожидание действия оператора";

        private static void CheckUserActionPrompt(PlaceSetting place)
        {
            if (
                place.ActiveStep is UserActionPromptStep step &&
                !(step.Status == EnumStepStatus.Fatal || step.Status == EnumStepStatus.Cancel) &&
                !(place.StendSettings.Tag is UserActionStatus)
               )
            {
                place.StendSettings.Tag = UserActionStatus.Start;
            }
            if (place.StendSettings.Tag is UserActionStatus userActtionStatus)
                if (userActtionStatus == UserActionStatus.Start)
                {
                    // ожидаем, пока все ПУ не приступят к выполнению шага UserActionPromptStep или не будет отменена их настройка
                    int placeCount = place.StendSettings.Objects.Count(item
                        => item.Status == EnumStepStatus.Fatal || item.Status == EnumStepStatus.Cancel || item.ActiveStep is UserActionPromptStep);
                    if (placeCount == places.Count)
                    {
                        if (Helpers.PromptMessage(place.ActiveStep.StepName + Environment.NewLine
                            + "Выполните необходимые действия и нажмите <OK>") == DialogResult.OK)
                            place.StendSettings.Tag = UserActionStatus.Ok;
                        else
                            place.StendSettings.Tag = UserActionStatus.Abort;
                    }
                }
                else
                if (userActtionStatus == UserActionStatus.Ok)
                {
                    // ожидаем, пока все ПУ перейдут к выполнению следующего шага
                    int placeCount = place.StendSettings.Objects.Count(item
                        => !(item.ActiveStep is UserActionPromptStep) && item.Status == EnumStepStatus.Run);
                    if (placeCount == 0)
                        place.StendSettings.Tag = null;
                }
                else
                if (userActtionStatus == UserActionStatus.Abort)
                {
                    // ожидаем, пока все ПУ отменят выполнение настройки
                    int placeCount = place.StendSettings.Objects.Count(item => item.Status == EnumStepStatus.Fatal);
                    if (placeCount == place.StendSettings.Objects.Count)
                        place.StendSettings.Tag = null;
                }
        }

        public override void ExecuteStep()
        {
            try
            {
                if(Owner != null && Owner.Device is GXDLMSDevice device)
                {
                    FireEvent("Ожидание ответа оператора");
                    Status = EnumStepStatus.Run;

                     if (ReconnectDevice)
                         device.Disconnect();

                     int stepCount = 0;
                     while(stepCount < 300)
                     {
                        Thread.Sleep(1000);
                        stepCount++;

                        //CheckUserActionPrompt(Owner);

                      if (Owner.StendSettings.Tag is UserActionStatus status)
                      {
                          if(status == UserActionStatus.Ok)
                          {
                              break;
                          }
                          else
                          {
                              if(status == UserActionStatus.Abort)
                              {
                                  throw new Exception("Операция прервана оператором");
                              }
                          }
                      }
                     }
                    if(stepCount >= 300)
                    {
                        throw new Exception("Таймаут ожидания ответа оператора");
                    }
                    if (ReconnectDevice)
                        device.InitializeConnection();
                    Status = EnumStepStatus.Succes;
                }
                else
                {
                    throw new Exception("Не определены настройки шага");
                }
            }
            catch(Exception ex)
            {
                Status = EnumStepStatus.Fatal;
                FireEvent($"{ex.Message}");
                throw;
            }
        }
    }
}
