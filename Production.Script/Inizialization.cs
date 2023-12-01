using Gurux.DLMS.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using Gurux.DLMS.Enums;
using System.Threading;
using ZIP.DLMS;
using Ankom.Common;
using Newtonsoft.Json;
using Gurux.DLMS;
using System.ComponentModel;
using Production.Script.Calendar;
using System.Globalization;

namespace Production.Script
{
    public class Initialization : BaseStep
    {

        public Initialization() : base()
        {
            StepName = "Производственное стирание и базовая настройка";
        }

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


                                    // выполняем дополнительную обработку 
                                    if (internalStep is ExecScriptObject script)
                                    {


                                        //if (Owner.StendSettings.Calendar != null && script.OBIS == "0.0.10.164.0.255")
                                        //{
                                        //    var obj = device.Objects.FindByLN(ObjectType.None, "0.0.13.0.0.255");
                                            
                                        //    var tt = script.Owner.StendSettings;
                                        //    var calendar = obj as GXDLMSActivityCalendar;



                                        //    //int[] ind = { 7, 8, 9 };
                                        //    //Helpers.FileCalendarLoad(script.Owner, device, ind);
                                        //    //device.Comm.ActivatePassiveCalendar();

                                        //}



                                        script.ExecuteStep();

                                        if (script.OBIS == "0.0.10.164.1.255")
                                            try
                                            {
                                                // после выполнения производственного сброса ожидаем отклчения ПУ в течение 
                                                // максимум 2 мин., отправляя запрос поддержки подключения с интервалом в 5 сек.

                                                script.FireEvent("Ожидание перезагрузки устройства...");

                                                int counter = 0;
                                                while (counter < 24)
                                                {
                                                    Thread.Sleep(5000);
                                                    if (device.ReadyToExchange)
                                                        try
                                                        {
                                                            device.Comm.KeepAlive();
                                                        }
                                                        catch (Exception)
                                                        {
                                                            device.Disconnect();
                                                            break;
                                                        }
                                                    counter++;
                                                }
                                                if (counter >= 24)
                                                    throw new Exception($"таймаут ожидания отключения ПУ");

                                                // восстанавливаем подключение
                                                Thread.Sleep(1000);
                                                device.InitializeConnection();
                                            }
                                            catch
                                            {
                                                internalStep.Status = EnumStepStatus.Fatal;
                                                throw;
                                            }
                                        script.Status = EnumStepStatus.Succes;

                                        //if (script.Owner.StendSettings.Calendar != null && script.OBIS == "0.0.10.164.0.255")
                                        //{
                                        //    int[] index = { 7, 8, 9 };

                                        //    Helpers.FileCalendarLoad(device.Objects.FindByLN(ObjectType.None, script.OBIS) ?? throw new Exception("Объект " + script.OBIS + " отсутствует в Ассоциации ПУ."), device, index);
                                        //}
                                    }
                                    else
                                    if (internalStep is WriteSimpleObject simpleObject)
                                    {
                                        try
                                        {
                                            if (simpleObject.OBIS == "0.0.96.1.0.255")
                                            {
                                                // настройка заводского номера - из общего списка прошиваемых ПУ производственного стенда
                                                simpleObject.Value = Owner.Serial;
                                            }
                                            else
                                            if (simpleObject.OBIS == "0.0.1.0.0.255" && simpleObject.AttributeInx == 2)
                                            {
                                                // синхронизация часов счетчика с временем ПК - динамическая настройка
                                                simpleObject.Value = Helpers.GetDLMSDateTime(DateTime.Now).DLMSValue;
                                            }
                                                simpleObject.ExecuteStep();

                                        }
                                        catch
                                        {
                                            internalStep.Status = EnumStepStatus.Fatal;
                                            throw;
                                        }
                                        simpleObject.Status = EnumStepStatus.Succes;
                                    }
                                    else if(internalStep is ReadSimpleObject simpleObj)
                                    {
                                        try
                                        {
                                            simpleObj.ExecuteStep();
                                            if(simpleObj.OBIS == "0.0.96.170.2.255")
                                            {
                                                GXStructure gXStructure = (GXStructure)simpleObj.Value;
                                                Helpers.KeyComparison((List<object>)gXStructure[0], 
                                                    (List<object>)gXStructure[1], Owner.StendSettings);
                                                Status = EnumStepStatus.Succes;
                                                FireEvent("Проверка кода контроллера 2");
                                            }
                                            else if(simpleObj.OBIS == "0.0.96.170.1.255")
                                            {
                                                GXStructure gXStructure = (GXStructure)simpleObj.Value;
                                                int indexContr_1 = Convert.ToInt32(gXStructure[0]);
                                                int indexContr_2 = Convert.ToInt32(gXStructure[1]);
                                                //int indexContr_3 = Convert.ToInt32(gXStructure[2]);

                                                if (indexContr_1 != Owner.StendSettings.FirmwareInfo.Controller2_Device1)
                                                    throw new Exception($"Ошибка проверки кода контроллера измерителя -- код устройства " +
                                                        $"[{indexContr_1}] -- эталонное значение [{Owner.StendSettings.FirmwareInfo.Controller2_Device1}]");

                                                if (indexContr_2 != Owner.StendSettings.FirmwareInfo.Controller2_Device2)
                                                    throw new Exception($"Ошибка проверки кода контроллера измерителя -- код устройства " +
                                                        $"[{indexContr_2}] -- эталонное значение [{Owner.StendSettings.FirmwareInfo.Controller2_Device2}]");

                                                //if (indexContr_3 != Owner.StendSettings.FirmwareInfo.Controller2_Device3)
                                                //    throw new Exception($"Ошибка проверки кода контроллера измерителя -- код устройства " +
                                                //        $"[{indexContr_3}] -- эталонное значение [{Owner.StendSettings.FirmwareInfo.Controller2_Device3}]");

                                                Status = EnumStepStatus.Succes;
                                                FireEvent("Проверка кода контроллера 1");
                                            }
                                        }
                                        catch
                                        {
                                            internalStep.Status = EnumStepStatus.Fatal;
                                            throw;
                                        }
                                        simpleObj.Status = EnumStepStatus.Succes;
                                    }
                                    else if (internalStep is PermissionsTable simObj)
                                    {
                                        try
                                        {
                                            simObj.ExecuteStep();
                                        }
                                        catch
                                        {
                                            internalStep.Status = EnumStepStatus.Fatal;
                                            throw;
                                        }
                                        simObj.Status = EnumStepStatus.Succes;
                                    }
                                    else if (internalStep is FileWrite filObj)
                                    {
                                        try
                                        {
                                            filObj.ExecuteStep();
                                        }
                                        catch
                                        {
                                            internalStep.Status = EnumStepStatus.Fatal;
                                            throw;
                                        }
                                        filObj.Status = EnumStepStatus.Succes;
                                    }
                                    else
                                        throw new Exception($"отсутствует обработчик шага задания типа {value.GetType().Name}");
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
    /// Установка простого объекта счетчика в указанное значение
    /// </summary>
    public class WriteSimpleObject : BaseStep
    {
        /// <summary>
        /// OBIS-код объекта счетчика для установки значения
        /// Объект выбирается из Ассоциации на шаге выполнения
        /// </summary>
        public string OBIS { get; set; }


        /// <summary>
        /// Индекс атрибута объекта для установки
        /// </summary>
        public int AttributeInx { get; set; } = 2;

        /// <summary>
        /// Устанавливаемое значение. 
        /// Конвертируется в нужный тип на шаге выполнения
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Признак общего значения для всез прошиваемых ПУ в сеансе. 
        /// Назначается перед сеансом в пользовательском UI
        /// </summary>
        public bool IsCommon { get; set; } = false;

        public WriteSimpleObject() : base()
        {
            StepName = "Настройка объекта";
        }

        public override void ExecuteStep()
        {
            try
            {
                if (Owner != null && Owner.Device is GXDLMSDevice device)
                {
                    Status = EnumStepStatus.Run;
                    // загружаем объект из Ассоциации для проверким его наличия и устаноавки корректных атрибутов
                    GXDLMSObject obj = device.Objects.FindByLN(ObjectType.None, OBIS);
                    if (obj == null)
                        throw new Exception($"Объект {OBIS} отсутствует в Ассоциации ПУ.");

                    // значениe объекта получаем из общих настроек стенда?
                    if (IsCommon)
                    {
                        WriteSimpleObject commonParam = Owner.StendSettings.CommonParameters.FirstOrDefault(item => item.OBIS == OBIS && item.AttributeInx == AttributeInx);
                        Value = commonParam?.Value;
                    }

                    if(string.IsNullOrWhiteSpace(Value))
                        throw new Exception($"Не задано значение объекта {OBIS}.");

                    GXDLMSSettings settings = device.Comm.client.Settings;
                    Helpers.WriteAttributeValue(device, obj, AttributeInx, Value);
                    var resultValue = (obj as IGXDLMSBase).GetValue(settings, new ValueEventArgs(obj, AttributeInx, 0, null));

                    // отправляем сообщение в общий лог и лог счетчика
                    FireEvent($"Текущее значение [{resultValue}]");

                    // установим в Ok после полной обработки на родительском уровне!!!
                    //Status = EnumStepStatus.Succes;
                }
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
    }

    /// <summary>
    /// Установка простого объекта счетчика в указанное значение
    /// </summary>
    public class ReadSimpleObject : BaseStep
    {
        private object _value = null;

        /// <summary>
        /// OBIS-код объекта счетчика для установки значения
        /// Объект выбирается из Ассоциации на шаге выполнения
        /// </summary>
        public string OBIS { get; set; }


        /// <summary>
        /// Индекс атрибута объекта для установки
        /// </summary>
        public int AttributeInx { get; set; } = 2;

        /// <summary>
        /// Считанное значение. 
        /// Конвертируется в нужный тип на шаге выполнения
        /// </summary>
        [JsonIgnore]
        public object Value { get => _value; }

        public ReadSimpleObject() : base()
        {
            StepName = "Чтение объекта";
        }

        public override void ExecuteStep()
        {
            try
            {
                if (Owner != null && Owner.Device is GXDLMSDevice device)
                {
                    Status = EnumStepStatus.Run;
                    // загружаем объект из Ассоциации для проверким его наличия и устаноавки корректных атрибутов
                    GXDLMSObject obj = device.Objects.FindByLN(ObjectType.None, OBIS);
                    if (obj == null)
                        throw new Exception($"Объект {OBIS} отсутствует в Ассоциации ПУ.");

                    GXDLMSSettings settings = device.Comm.client.Settings;
                    Helpers.ReadAttributeValue(device, obj, AttributeInx);
                    _value = (obj as IGXDLMSBase).GetValue(settings, new ValueEventArgs(obj, AttributeInx, 0, null));

                    // отправляем сообщение в общий лог и лог счетчика
                    FireEvent($"Текущее значение [{Value}]");

                    // установим в Ok после полной обработки на родительском уровне!!!
                    //Status = EnumStepStatus.Succes;
                }
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
    }

    /// <summary>
    /// Выполнение сценария счетчика с указанным идентификатором   
    /// </summary>
    public class ExecScriptObject : WriteSimpleObject
    {
        /// <summary>
        /// Индентификатор исполняемого сценария таблицы задается свойством Value базового класса
        /// </summary>
        [JsonIgnore]
        public int ScriptID { get => Convert.ToInt32(base.Value); set => base.Value = value.ToString(); }

        public ExecScriptObject() : base()
        {
            StepName = "Исполнение скрипта";
        }

        public override void ExecuteStep()
        {
            try
            {
                if (Owner != null && Owner.Device is GXDLMSDevice device)
                {
                    Status = EnumStepStatus.Run;
                    
                    // загружаем объект из Ассоциации для проверким его наличия и устаноавки корректных атрибутов
                    GXDLMSObject obj = device.Objects.FindByLN(ObjectType.None, OBIS);
                    if (obj == null)
                        throw new Exception($"Объект {OBIS} отсутствует в Ассоциации ПУ.");

                    //if (!string.IsNullOrWhiteSpace(StepName))
                    //    obj.Description = StepName;

                    if (obj is GXDLMSScriptTable scriptTable)
                    {
                        // считываем список скриптов таблицы сценариев. Атрибут объекта 2
                        Helpers.ReadAttributeValue(device, scriptTable, 2); // AttributeInx);
                        GXDLMSScript script = scriptTable.Scripts.FirstOrDefault(item => item.Id == ScriptID);
                        if (script == null)
                            throw new Exception($"Отсутствует скрипт с ID = {ScriptID}");
                        device.Comm.ExecuteScript(scriptTable, script);
                        // генерим событие для обработки на верхнем уровне (отправка сообщение в протокол, ...)
                        FireEvent($"Исполнен скрипт [{ScriptID}]");
                        // установим в Ok после полной обработки на родительском уровне!!!
                        //Status = EnumStepStatus.Succes;
                    }
                    else
                        throw new Exception($"Объект {obj.Description} не является таблицей сценариев.");
                }
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
    }

    public class PermissionsTable : BaseStep
    {
        public string[] Value { get; set; }

        public string OBIS { get; set; } = "0.0.96.3.20.255";

        public int AttributeInx { get; set; } = 3;

        public PermissionsTable() => StepName = "Настройка объекта";

        public GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject> ttttt = new GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>();



        public override void ExecuteStep()
        {
            try
            {
                if (Owner != null && Owner.Device is GXDLMSDevice device)
                {
                    Status = EnumStepStatus.Run;
                    int num;
                    if (Owner != null)
                    {
                        device = Owner.Device;
                        num = device != null ? 1 : 0;
                    }
                    else
                        num = 0;
                    if (num == 0)
                        throw new Exception("Не определены настройки шага.");
                    Status = EnumStepStatus.Run;
                    Helpers.WriteAttributeValue(device, device.Objects.FindByLN(ObjectType.None, OBIS) ?? throw new Exception("Объект " + OBIS + " отсутствует в Ассоциации ПУ."), AttributeInx, Value);
                }
            }
            catch (Exception ex)
            {
                Status = EnumStepStatus.Fatal;
                FireEvent(ex.Message ?? "");
                throw;
            }
        }
    }
    public class FileWrite : BaseStep
    {
        public string[] Value { get; set; }

        public string OBIS { get; set; }

        public int[] AttributeInx { get; set; }

        public FileWrite() => StepName = "Настройка объекта";
        public override void ExecuteStep()
        {
            try
            {
                if (Owner != null && Owner.Device is GXDLMSDevice device)
                {
                    Status = EnumStepStatus.Run;
                    int num;
                    if (Owner != null)
                    {
                        device = Owner.Device;
                        num = device != null ? 1 : 0;
                    }
                    else
                        num = 0;
                    if (num == 0)
                        throw new Exception("Не определены настройки шага.");
                    Status = EnumStepStatus.Run;


                    //Если определённые настройки загружены
                    if(Owner.StendSettings.Calendar != null && OBIS == "0.0.13.0.0.255")
                        Helpers.FileCalendarLoad(device.Objects.FindByLN(ObjectType.None, OBIS) ?? throw new Exception("Объект " + OBIS + " отсутствует в Ассоциации ПУ."), device, AttributeInx, Owner.StendSettings.Calendar);

                     if(OBIS == "0.0.21.0.2.255" || OBIS == "0.0.21.0.164.255" || OBIS == "0.0.21.0.1.255")
                    {
                        List<CaptureGridObject> AutoscrollIndication = null;

                        if (OBIS == "0.0.21.0.2.255")
                            AutoscrollIndication = Owner.StendSettings.ButtonIndication;
                        if (OBIS == "0.0.21.0.164.255")
                            AutoscrollIndication = Owner.StendSettings.ButtonIndication2;
                        if (OBIS == "0.0.21.0.1.255")
                            AutoscrollIndication = Owner.StendSettings.AutoscrollIndication;


                        GXDLMSObject obj = device.Objects.FindByLN(ObjectType.None, OBIS) ?? throw new Exception("Объект " + OBIS + " отсутствует в Ассоциации ПУ.");

                        Helpers.CaptureObjectWrite(device, AutoscrollIndication, obj);
                    } 
                    else
                        return;
                    //////////////////////////////////////////
                }
            }
            catch (Exception ex)
            {
                Status = EnumStepStatus.Fatal;
                FireEvent(ex.Message ?? "");
                throw;
            }
        }
    }
}
