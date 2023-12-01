//
// --------------------------------------------------------------------------
//  Gurux Ltd
//
//
//
// Version:         $Revision: 12310 $,
//                  $Date: 2021-01-29 08:21:47 +0200 (pe, 29 tammi 2021) $
//                  $Author: gurux01 $
//
// Copyright (c) Gurux Ltd
//
//---------------------------------------------------------------------------
//
//  DESCRIPTION
//
// This file is a part of Gurux Device Framework.
//
// Gurux Device Framework is Open Source software; you can redistribute it
// and/or modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; version 2 of the License.
// Gurux Device Framework is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License for more details.
//
// More information of Gurux DLMS/COSEM Director: https://www.gurux.org/GXDLMSDirector
//
// This code is licensed under the GNU General Public License v2.
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------
#if ZIP
using Ankom.Common;
#endif
using Gurux.Common;
using Gurux.DLMS;
using Gurux.DLMS.Enums;
using Gurux.DLMS.ManufacturerSettings;
using Gurux.DLMS.Objects;
using Gurux.DLMS.Objects.Enums;
using Gurux.DLMS.Secure;
using Gurux.Net;
using Gurux.Serial;
using NLog;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace ZIP.DLMS
{
    public class ZipPwddKey
    {
        public static string Key = "ООО «ПЕТЕРБУРГСКИЙ ЗАВОД ИЗМЕРИТЕЛЬНЫХ ПРИБОРОВ»";
    }

    public delegate void ProgressEventHandler(object sender, string description, int current, int maximium);
    public delegate void DisconnectEventHandler(object sender);
    public delegate void StatusEventHandler(object sender, DeviceState status);
    public delegate void ErrorEventHandler(GXDLMSObject sender, int index, object data, Exception ex);
    public delegate void ReadEventHandler(GXDLMSClient client, GXDLMSObject sender, int index, object data, object parameters, Exception ex);
    public delegate void WriteEventHandler(GXDLMSClient client, GXDLMSObject sender, int index, object data, Exception ex);
    public delegate void MessageTraceEventHandler(DateTime time, GXDLMSDevice sender, string trace, byte[] data, int payload, string path, int duration);

    public class GXDLMSCommunicator
    {
        private const string csMediaClosedMsg = "Канал обмена закрыт.";
        private const string csAsyncWorkCancelledMsg = "Операция отменена пользователем.";
        private const string csReadingObjectMsg = "Чтение объекта {0}, интерфейс {1}:{2} [{3}]";
        private const string csMehtodObjectMsg = "Вызов метода объекта {0}, интерфейс {1}";
        private const string csWriteObjectMsg = "Запись объекта {0}, интерфейс {1}:{2}";
        private const string csReleaseRequestMsg = "Запрос на освобождение";
        private const string csDisconnectRequestMsg = "Запрос на отключение";
        private const string csRequestAttemptMsg = "Не удалось отправить данные. Повторная попытка ";
        private const string csTimeoutReplyMsg = "Не удалось получить ответ от устройства в заданное время.";
        private const string csUserAbortMsg = "Операция прервана пользователем.";
        private const string csDurationMsg = "Длительность: ";
        private const string csUnknownBaudRate = "Неизвестная скорость последовательного подключения.";
        private const string csUnknownManufactMsg = "Неизвестный производитель ";
        private const string csIllegalReplyMsg = "Не совпадает количество запрошенных и считанных элементов. ";
        private const string csIllegalReadListMsg = "Невозможно сопоставить список атрибутов из-за ошибкок чтения. ";
        private const string csInitNetConnection = "Инициализация сетевого подключения.";
        private const string csInitSerialConnection = "Инициализация последовательного подключения.";
        private const string csInitTerminalConnection = "Инициализация модемного подключения.";
        private const string csMissingManufactureMsg = "Тип производителя ПУ не совпадает. Производитель - {0}, а он должен быть {1}.";
        private const string csSNRMRequestMsg = "Отправка запроса SNRM.";

        /// <summary>
        /// команда пользователя о необходимости прерывания обмена со счетчиком
        /// </summary>
        private bool _userCanceled
        {
            get => parent != null && parent.AsyncWorkCanceled;
        }

        public void ExecuteScript(GXDLMSScriptTable current, GXDLMSScript activeScript)
        {
            try
            {
                if (media != null && media.IsOpen)
                {
                    GXReplyData reply = new GXReplyData();
                    try
                    {
                        byte[][] tmp = current.Execute(client, activeScript);
                        string str = $"[{current.Description}]: выполнение скрипта {activeScript.Id}";
                        try
                        {
                            ReadDataBlock(tmp[0], str, reply);
                        }
                        catch (Exception ex)
                        {
                            //Update frame ID if meter returns error.
                            if (client.InterfaceType == InterfaceType.HDLC ||
                                client.InterfaceType == InterfaceType.HdlcWithModeE ||
                                client.InterfaceType == InterfaceType.PlcHdlc)
                            {
                                GXDLMSClient.GetHdlcAddressInfo(new GXByteBuffer(tmp[0]), out int t, out int source, out byte type);
                                client.HdlcSettings.SenderFrame = type;
                            }
                            throw ex;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private const string csDisconnectRequest = "Отправка запроса Disconnect.";
        private const string csAARQRequestMsg = "Отправка запроса AARQ.";
        private const string csUAReplySuccessMsg = "Разбор UA-ответа успешно завершен.";
        private const string csAAREReplySuccessMsg = "Разбор AARE-ответа успешно завершен.";

        /// <summary>
        /// Максимальный размер блока полезных данных.
        /// </summary>
        internal int payload;

        /// <summary>
        /// Файл протокола.
        /// </summary>
        internal string LogFile;

        internal DateTime lastTransaction = DateTime.MinValue;
        internal DateTime connectionStartTime;
        internal GXDLMSDevice parent;
        public Control parentForm;
        public IGXMedia media = null;
        public GXDLMSSecureClient client;
        //private GXManufacturer Manufacturer;


        public GXDLMSCommunicator(GXDLMSDevice parent, Gurux.Common.IGXMedia media)
        {
            this.parent = parent;
            this.media = media;
            client = new GXDLMSSecureClient();
        }

        public ProgressEventHandler OnProgress;
        public ReadEventHandler OnBeforeRead;
        public ReadEventHandler OnAfterRead;
        public ErrorEventHandler OnError;
        public WriteEventHandler OnAfterWrite;

        /// <summary>
        /// Формирует описание DLMS-объекта
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetObjectDescription(GXDLMSObject obj)
        {
            return
                (obj == null) ? String.Empty : $"{obj.LogicalName}: {obj.Description}";
        }

        public byte[] SNRMRequest()
        {
            payload = 0;
            return client.SNRMRequest();
        }

        public void ParseUAResponse(GXByteBuffer data)
        {
            client.ParseUAResponse(data);
        }

        public byte[][] AARQRequest()
        {
            return client.AARQRequest();
        }

        public void ParseAAREResponse(GXByteBuffer data)
        {
            client.ParseAAREResponse(data);
        }

        public byte[] Read(GXDLMSObject it, int attributeOrdinal)
        {
            byte[] tmp = client.Read(it, attributeOrdinal)[0];
            //            GXLogWriter.WriteLog(string.Format(csReadingObjectMsg, parent.Name + ": " + GetObjectDescription(it), it.ObjectType, attributeOrdinal), tmp);
            return tmp;
        }

        public byte[][] Read(GXDLMSObject it, int attributeOrdinal, string parameters)
        {
            if (it is GXDLMSProfileGeneric generic && attributeOrdinal == 2 && parameters != null)
            {
                GXStructure p = (GXStructure)GXDLMSTranslator.XmlToValue(parameters);
                if ((int)p[0] == 1)
                {
                    GXStructure arr = (GXStructure)p[1];
                    GXDateTime start = (GXDateTime)client.ChangeType(new GXByteBuffer((byte[])arr[1]), DataType.DateTime);
                    GXDateTime end = (GXDateTime)client.ChangeType(new GXByteBuffer((byte[])arr[2]), DataType.DateTime);
                    return client.ReadRowsByRange(generic, start, end);
                }
                if ((int)p[0] == 2)
                {
                    GXStructure arr = (GXStructure)p[1];
                    UInt32 index = (UInt32)arr[0];
                    UInt32 count = (UInt32)arr[1] - index + 1;
                    return client.ReadRowsByEntry(generic, index, count);
                }
            }
            return client.Read(it, attributeOrdinal);
        }

        public void MethodRequest(GXDLMSObject target, int methodIndex, object data, string text, GXReplyData reply)
        {
            byte[][] tmp;
            if (data is byte[])
            {
                tmp = client.Method(target, methodIndex, data, DataType.Array);
            }
            else
            {
                tmp = client.Method(target, methodIndex, data, GXDLMSConverter.GetDLMSDataType(data));
            }
            int pos = 0;
            string str = string.Format(csMehtodObjectMsg, parent.Name + ":" + target.LogicalName, target.ObjectType);
            foreach (byte[] it in tmp)
            {
                reply.Clear();
                if (tmp.Length != 1)
                {
                    ++pos;
                    NotifyProgress(text, pos, tmp.Length);
                }
                try
                {
                    ReadDataBlock(it, str, reply);
                }
                catch (Exception ex)
                {
                    //Update frame ID if meter returns error.
                    if (client.InterfaceType == InterfaceType.HDLC ||
                        client.InterfaceType == InterfaceType.HdlcWithModeE ||
                        client.InterfaceType == InterfaceType.PlcHdlc)
                    {
                        GXDLMSClient.GetHdlcAddressInfo(new GXByteBuffer(it), out int t, out int source, out byte type);
                        client.HdlcSettings.SenderFrame = type;
                    }
                    throw ex;
                }
            }
            NotifyProgress(text, 1, 1);
        }

        byte[] ReleaseRequest()
        {
            byte[][] data = client.ReleaseRequest();
            if (data == null)
            {
                return null;
            }
            GXLogWriter.WriteLog(parent.Name + ": " + csReleaseRequestMsg, data[0]);
            return data[0];
        }

        public byte[] DisconnectRequest()
        {
            return DisconnectRequest(false);
        }

        public byte[] DisconnectRequest(bool force)
        {
            byte[] data = client.DisconnectRequest(force);
            if (data == null)
            {
                return null;
            }
            //GXLogWriter.WriteLog("Запрос на отключение");
            return data;
        }

        public void ActivatePassiveCalendar(GXDLMSActivityCalendar activityCalendar)
        {
            try
            {
                if (media != null && media.IsOpen)
                {
                    GXReplyData reply = new GXReplyData();
                    try
                    {
                        byte[][] tmp = activityCalendar.ActivatePassiveCalendar(client);
                        string str = string.Format("Активация пассивного календаря {0}:{1}", activityCalendar.LogicalName, activityCalendar.ObjectType);
                        try
                        {
                            ReadDataBlock(tmp[0], str, reply);
                        }
                        catch (Exception ex)
                        {
                            //Update frame ID if meter returns error.
                            if (client.InterfaceType == InterfaceType.HDLC ||
                                client.InterfaceType == InterfaceType.HdlcWithModeE ||
                                client.InterfaceType == InterfaceType.PlcHdlc)
                            {
                                GXDLMSClient.GetHdlcAddressInfo(new GXByteBuffer(tmp[0]), out int t, out int source, out byte type);
                                client.HdlcSettings.SenderFrame = type;
                            }
                            throw ex;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SetDisconnectControl(GXDLMSDisconnectControl disconnectControl, bool On)
        {
            try
            {
                if (media != null && media.IsOpen)
                {
                    GXReplyData reply = new GXReplyData();
                    try
                    {
                        byte[][] tmp = (On) ? disconnectControl.RemoteReconnect(client) : disconnectControl.RemoteDisconnect(client);
                        string str = string.Format("Команда управление нагрузкой {0}:{1}:{2}", disconnectControl.LogicalName, disconnectControl.ObjectType, On);
                        try
                        {
                            ReadDataBlock(tmp[0], str, reply);
                            // чтение состояния реле и нагрузки
                            ReadValue(disconnectControl, 2);
                            ReadValue(disconnectControl, 3);
                        }
                        catch (Exception ex)
                        {
                            //Update frame ID if meter returns error.
                            if (client.InterfaceType == InterfaceType.HDLC ||
                                client.InterfaceType == InterfaceType.HdlcWithModeE ||
                                client.InterfaceType == InterfaceType.PlcHdlc)
                            {
                                GXDLMSClient.GetHdlcAddressInfo(new GXByteBuffer(tmp[0]), out int t, out int source, out byte type);
                                client.HdlcSettings.SenderFrame = type;
                            }
                            throw ex;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ActivatePush(GXDLMSPushSetup current)
        {
            try
            {
                if (media != null && media.IsOpen)
                {
                    GXReplyData reply = new GXReplyData();
                    try
                    {
                        byte[][] tmp = current.Activate(client);
                        string str = string.Format("Отправка Push сообщения {0}", current.LogicalName);
                        try
                        {
                            ReadDataBlock(tmp, str, reply);
                        }
                        catch (Exception ex)
                        {
                            if (client.InterfaceType == InterfaceType.HDLC ||
                                client.InterfaceType == InterfaceType.HdlcWithModeE ||
                                client.InterfaceType == InterfaceType.PlcHdlc)
                            {
                                GXDLMSClient.GetHdlcAddressInfo(new GXByteBuffer(tmp[0]), out int t, out int source, out byte type);
                                client.HdlcSettings.SenderFrame = type;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CaptureProfileData(GXDLMSProfileGeneric profileGeneric)
        {
            try
            {
                if (media != null && media.IsOpen)
                {
                    GXReplyData reply = new GXReplyData();
                    try
                    {
                        byte[][] tmp = profileGeneric.Capture(client);
                        string str = string.Format("Захват данных профиля {0}", profileGeneric.LogicalName);
                        try
                        {
                            ReadDataBlock(tmp[0], str, reply);
                        }
                        catch (Exception ex)
                        {
                            //Update frame ID if meter returns error.
                            if (client.InterfaceType == InterfaceType.HDLC ||
                                client.InterfaceType == InterfaceType.HdlcWithModeE ||
                                client.InterfaceType == InterfaceType.PlcHdlc)
                            {
                                GXDLMSClient.GetHdlcAddressInfo(new GXByteBuffer(tmp[0]), out int t, out int source, out byte type);
                                client.HdlcSettings.SenderFrame = type;
                            }
                            throw ex;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ResetProfileBuffer(GXDLMSProfileGeneric profileGeneric)
        {
            try
            {
                if (media != null && media.IsOpen)
                {
                    GXReplyData reply = new GXReplyData();
                    try
                    {
                        byte[][] tmp = profileGeneric.Reset(client);
                        string str = string.Format("Очистка буфера данных профиля {0}", profileGeneric.LogicalName);
                        try
                        {
                            ReadDataBlock(tmp[0], str, reply);
                        }
                        catch (Exception ex)
                        {
                            //Update frame ID if meter returns error.
                            if (client.InterfaceType == InterfaceType.HDLC ||
                                client.InterfaceType == InterfaceType.HdlcWithModeE ||
                                client.InterfaceType == InterfaceType.PlcHdlc)
                            {
                                GXDLMSClient.GetHdlcAddressInfo(new GXByteBuffer(tmp[0]), out int t, out int source, out byte type);
                                client.HdlcSettings.SenderFrame = type;
                            }
                            throw ex;
                        }
                        // проверяем число захваченных объектов. Д.б. 0
                        //ReadValue(profileGeneric, 7);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ResetRegisterValue(GXDLMSObject obj)
        {
            try
            {
                if (media != null && media.IsOpen && obj is GXDLMSRegister register)
                {
                    GXReplyData reply = new GXReplyData();
                    try
                    {
                        byte[][] tmp = register.Reset(client);
                        string str = string.Format("Сброс регистра {0}", register.LogicalName);
                        try
                        {
                            ReadDataBlock(tmp[0], str, reply);
                        }
                        catch (Exception ex)
                        {
                            //Update frame ID if meter returns error.
                            if (client.InterfaceType == InterfaceType.HDLC ||
                                client.InterfaceType == InterfaceType.HdlcWithModeE ||
                                client.InterfaceType == InterfaceType.PlcHdlc)
                            {
                                GXDLMSClient.GetHdlcAddressInfo(new GXByteBuffer(tmp[0]), out int t, out int source, out byte type);
                                client.HdlcSettings.SenderFrame = type;
                            }
                            throw ex;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Сдвигает время на n секунд (-900 <= n >= 900).
        /// </summary>
        /// <param name="clock"></param>
        /// <param name="n"></param>
        public void ClockShiftTime(GXDLMSClock clock, int n)
        {
            try
            {
                if (media != null && media.IsOpen)
                {
                    GXReplyData reply = new GXReplyData();
                    try
                    {
                        byte[][] tmp = clock.ShiftTime(client, n);
                        string str = string.Format("Сдвиг времени, {0} сек.", n);
                        try
                        {
                            ReadDataBlock(tmp[0], str, reply);
                        }
                        catch (Exception ex)
                        {
                            //Update frame ID if meter returns error.
                            if (client.InterfaceType == InterfaceType.HDLC ||
                                client.InterfaceType == InterfaceType.HdlcWithModeE ||
                                client.InterfaceType == InterfaceType.PlcHdlc)
                            {
                                GXDLMSClient.GetHdlcAddressInfo(new GXByteBuffer(tmp[0]), out int t, out int source, out byte type);
                                client.HdlcSettings.SenderFrame = type;
                            }
                            throw ex;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Предустановка нового значение часов (preset_time) и интервала validity_interval, 
        /// в пределах которого указанное время может быть активировано.
        /// </summary>
        /// <param name="clock"></param>
        /// <param name="presetTime"></param>
        /// <param name="validityIntervalStart"></param>
        /// <param name="validityIntervalEnd"></param>
        public void ClockPresetAdjustingTime(GXDLMSClock clock, DateTime presetTime, DateTime validityIntervalStart, DateTime validityIntervalEnd)
        {
            try
            {
                if (media != null && media.IsOpen)
                {
                    GXReplyData reply = new GXReplyData();
                    try
                    {
                        byte[][] tmp = clock.PresetAdjustingTime(client, presetTime, validityIntervalStart, validityIntervalEnd);
                        string str = string.Format("Предустановка часов и интервала активации");
                        try
                        {
                            ReadDataBlock(tmp[0], str, reply);
                        }
                        catch (Exception ex)
                        {
                            //Update frame ID if meter returns error.
                            if (client.InterfaceType == InterfaceType.HDLC ||
                                client.InterfaceType == InterfaceType.HdlcWithModeE ||
                                client.InterfaceType == InterfaceType.PlcHdlc)
                            {
                                GXDLMSClient.GetHdlcAddressInfo(new GXByteBuffer(tmp[0]), out int t, out int source, out byte type);
                                client.HdlcSettings.SenderFrame = type;
                            }
                            throw ex;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Выполняет попытку активации предустановленного значения часов с учетом, 
        /// интервала в пределах которого указанное время может быть установлено.
        /// </summary>
        /// <param name="clock"></param>
        public void ClockAdjustingTime(GXDLMSClock clock)
        {
            try
            {
                if (media != null && media.IsOpen)
                {
                    GXReplyData reply = new GXReplyData();
                    try
                    {
                        byte[][] tmp = clock.AdjustToPresetTime(client);
                        string str = string.Format("Активация предустановленного значения часов");
                        try
                        {
                            ReadDataBlock(tmp[0], str, reply);
                        }
                        catch (Exception ex)
                        {
                            //Update frame ID if meter returns error.
                            if (client.InterfaceType == InterfaceType.HDLC ||
                                client.InterfaceType == InterfaceType.HdlcWithModeE ||
                                client.InterfaceType == InterfaceType.PlcHdlc)
                            {
                                GXDLMSClient.GetHdlcAddressInfo(new GXByteBuffer(tmp[0]), out int t, out int source, out byte type);
                                client.HdlcSettings.SenderFrame = type;
                            }
                            throw ex;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Выполняет cдвиг времени часов ПУ к ближайшей четверти часа
        /// </summary>
        /// <param name="clock"></param>
        public void ClockAdjustToQuarter(GXDLMSClock clock)
        {
            try
            {
                if (media != null && media.IsOpen)
                {
                    GXReplyData reply = new GXReplyData();
                    try
                    {
                        byte[][] tmp = clock.AdjustToQuarter(client);
                        string str = string.Format("Сдвиг времени к ближайшей четверти часа");
                        try
                        {
                            ReadDataBlock(tmp[0], str, reply);
                        }
                        catch (Exception ex)
                        {
                            //Update frame ID if meter returns error.
                            if (client.InterfaceType == InterfaceType.HDLC ||
                                client.InterfaceType == InterfaceType.HdlcWithModeE ||
                                client.InterfaceType == InterfaceType.PlcHdlc)
                            {
                                GXDLMSClient.GetHdlcAddressInfo(new GXByteBuffer(tmp[0]), out int t, out int source, out byte type);
                                client.HdlcSettings.SenderFrame = type;
                            }
                            throw ex;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Выполняет cдвиг времени часов ПУ к ближайшему расчетному периоду
        /// </summary>
        /// <param name="clock"></param>
        public void ClockAdjustToMeasuringPeriod(GXDLMSClock clock)
        {
            try
            {
                if (media != null && media.IsOpen)
                {
                    GXReplyData reply = new GXReplyData();
                    try
                    {
                        byte[][] tmp = clock.AdjustToMeasuringPeriod(client);
                        string str = string.Format("Сдвиг времени к ближайшему расчетному периоду");
                        try
                        {
                            ReadDataBlock(tmp[0], str, reply);
                        }
                        catch (Exception ex)
                        {
                            //Update frame ID if meter returns error.
                            if (client.InterfaceType == InterfaceType.HDLC ||
                                client.InterfaceType == InterfaceType.HdlcWithModeE ||
                                client.InterfaceType == InterfaceType.PlcHdlc)
                            {
                                GXDLMSClient.GetHdlcAddressInfo(new GXByteBuffer(tmp[0]), out int t, out int source, out byte type);
                                client.HdlcSettings.SenderFrame = type;
                            }
                            throw ex;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Выполняет cдвиг времени часов ПУ к ближайшей минуте
        /// </summary>
        /// <param name="clock"></param>
        public void ClockAdjustToMinute(GXDLMSClock clock)
        {
            try
            {
                if (media != null && media.IsOpen)
                {
                    GXReplyData reply = new GXReplyData();
                    try
                    {
                        byte[][] tmp = clock.AdjustToMinute(client);
                        string str = string.Format("Сдвиг времени к ближайшей минуте");
                        try
                        {
                            ReadDataBlock(tmp[0], str, reply);
                        }
                        catch (Exception ex)
                        {
                            //Update frame ID if meter returns error.
                            if (client.InterfaceType == InterfaceType.HDLC ||
                                client.InterfaceType == InterfaceType.HdlcWithModeE ||
                                client.InterfaceType == InterfaceType.PlcHdlc)
                            {
                                GXDLMSClient.GetHdlcAddressInfo(new GXByteBuffer(tmp[0]), out int t, out int source, out byte type);
                                client.HdlcSettings.SenderFrame = type;
                            }
                            throw ex;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Disconnect()
        {
            try
            {
                if (media != null && media.IsOpen)
                {
                    GXReplyData reply = new GXReplyData();
                    try
                    {
                        //Release is call only for secured connections.
                        //All meters are not supporting Release and it's causing problems.
                        if (client.InterfaceType == InterfaceType.WRAPPER ||
                            (client.Ciphering.Security != (byte)Security.None && !parent.PreEstablished))
                        {
                            byte[] data = ReleaseRequest();
                            if (data != null)
                            {
                                ReadDataBlock(data, csReleaseRequestMsg, reply);
                            }
                        }

                    }
                    catch (Exception)
                    {
                        // Only part of meters (not all!) support this functionality (release).
                    }

                    try
                    {
                        reply.Clear();
                        if ((client.InterfaceType == InterfaceType.HDLC ||
                            client.InterfaceType == InterfaceType.HdlcWithModeE ||
                            client.InterfaceType == InterfaceType.PlcHdlc)
                            && !parent.PreEstablished)
                        {
                            ReadDataBlock(DisconnectRequest(true), csDisconnectRequestMsg, reply);
                        }
                    }
                    catch (Exception)
                    {
                        // Only part of meters (not all!) support this functionality (release).
                    }
                }
            }
            finally
            {
                if (media != null)
                {
                    media.Close();
                }
            }
        }

        public void ReadDLMSPacket(byte[] data, GXReplyData reply)
        {
            ReadDLMSPacket(data, parent.ResendCount, reply);
        }

        /// <summary>
        /// Считывание пакета данных DLMS с устройства.
        /// </summary>
        /// <param name="data">Данные для отправки.</param>
        /// <param name="tryCount">Количество попыток отправки.</param>
        /// <param name="reply">Ответ прибора учета.</param>
        public void ReadDLMSPacket(byte[] data, int tryCount, GXReplyData reply)
        {
            if ((data == null || data.Length == 0) && !reply.IsStreaming())
            {
                return;
            }

            if (_userCanceled)
            {
                reply.Error = (int)ErrorCode.UserAbortMode;
                throw new GXDLMSException(reply.Error);
            }

            GXReplyData notify = new GXReplyData();
            reply.Error = 0;
            object eop = (byte)0x7E;
            //In network connection terminator is not used.
            if (client.InterfaceType != InterfaceType.HDLC &&
                client.InterfaceType != InterfaceType.HdlcWithModeE &&
                client.InterfaceType != InterfaceType.PlcHdlc)
            {
                eop = null;
            }
            int pos = 0;
            bool succeeded = false;
            ReceiveParameters<byte[]> p = new ReceiveParameters<byte[]>()
            {
                AllData = false,
                Eop = eop,
                Count = eop == null ? 8 : 5,
                WaitTime = parent.WaitTime * 1000,
            };
            DateTime start = DateTime.Now;
            GXByteBuffer rd = new GXByteBuffer();

            lock (media.Synchronous)
                try
                {
                    if (!media.IsOpen)
                    {
                        throw new InvalidOperationException(csMediaClosedMsg);
                    }

                    if (data != null)
                    {

                        // sem 30.11.2021
                        ulong bytesSend = media.BytesSent;
                        media.Send(data, null);
                        start = DateTime.Now;
                        //GXLogWriter.WriteLog($"{parent.Name}: <- {media.BytesSent - bytesSend} байт");
                    }
                    do
                    {

                        if (!media.IsOpen)
                        {
                            throw new InvalidOperationException(csMediaClosedMsg);
                        }

                        // sem 30.11.2021
                        ulong bytesReceived = media.BytesReceived;
                        succeeded = media.Receive(p);
                        //GXLogWriter.WriteLog($"{parent.Name}: -> {media.BytesReceived - bytesReceived} байт");

                        if (!succeeded && !_userCanceled)
                        {
                            //Try to read again...
                            if (++pos < tryCount)
                            {
                                //If Eop is not set read one byte at time.
                                if (p.Eop == null)
                                {
                                    p.Count = 1;
                                }
                                string log = parent.Name + ": " + csRequestAttemptMsg + pos.ToString() + "/" + tryCount;
                                GXLogWriter.WriteLog(log);
                                parent.OnTrace?.Invoke(DateTime.Now, parent, log, p.Reply, 0, LogFile, 0);

                                // sem 30.11.2021
                                ulong bytesSend = media.BytesSent;

                                media.Send(data, null);

                                //GXLogWriter.WriteLog($"{parent.Name}: <- {media.BytesSent - bytesSend} байт");

                                continue;
                            }

                            string err = parent.Name + ": " + csTimeoutReplyMsg;
                            GXLogWriter.WriteLog(err, p.Reply);
                            parent.OnTrace?.Invoke(DateTime.Now, parent, err, p.Reply, 0, LogFile, 0);
                            throw new TimeoutException(err);
                        }
                        // sem 07.11.2022
                        // прекращаем попытки отправки по команде пользователя
                        else
                        if (_userCanceled)
                        {
                            if (succeeded)
                            {
                                rd.Position = 0;
                                rd.Set(p.Reply);
                                GXLogWriter.WriteLog(parent.Name + ": " /*null*/, rd.Array());
                            }
                            reply.Error = (int)ErrorCode.UserAbortMode;
                            throw new GXDLMSException(reply.Error);
                        }
                    } while (!succeeded && pos != tryCount);

                    rd = new GXByteBuffer(p.Reply);
                    try
                    {
                        pos = 0;
                        while (rd.GetUInt8(0) == '\t')
                        {
                            pos = 1;
                            while (pos < rd.Size)
                            {
                                if (rd.GetUInt8(pos) == 0)
                                {
                                    ++pos;
                                    parent.OnEvent?.Invoke(media, new ReceiveEventArgs(rd.SubArray(0, pos), media.ToString()));
                                    rd.Position = pos;
                                    rd.Trim();
                                    break;
                                }
                                ++pos;
                            }
                            pos = 0;
                        }

                        //Loop until whole COSEM packet is received
                        //or if received data is echo.
                        while
                              (
                                rd.Compare(data) ||
                                !client.GetData(rd, reply, notify)
                              )
                        {
                            int framePosition = rd.Position;
                            rd.Position = 0;
                            if (rd.Compare(data))
                            {
                                rd.Clear();
                            }
                            else
                            {
                                rd.Position = framePosition;
                            }
                            p.Reply = null;
                            if (notify.Data.Size != 0)
                            {
                                // Handle notify.
                                if (!notify.IsMoreData)
                                {
                                    parent.OnEvent?.Invoke(media, new ReceiveEventArgs(rd.Array(), media.ToString()));
                                    rd.Trim();
                                    notify.Clear();
                                    p.Eop = eop;
                                }
                                continue;
                            }
                            //If Eop is not set read one byte at time.
                            if (p.Eop == null)
                            {
                                p.Count = client.GetFrameSize(rd);
                            }

                            if (!media.IsOpen)
                            {
                                throw new InvalidOperationException(csMediaClosedMsg);
                            }

                            bool received = media.Receive(p);
                            if (!received && !_userCanceled)
                            {
                                string err;
                                //Try to read again...
                                if (++pos <= tryCount)
                                {
                                    err = parent.Name + ": " + csRequestAttemptMsg + pos.ToString() + "/3";
                                    System.Diagnostics.Debug.WriteLine(err);
                                    GXLogWriter.WriteLog(err, data);
                                    parent.OnTrace?.Invoke(DateTime.Now, parent, err, rd.Array(), 0, LogFile, 0);
                                    p.Reply = null;

                                    media.Send(data, null);
                                    continue;
                                }
                                err = parent.Name + ": " + csTimeoutReplyMsg;
                                GXLogWriter.WriteLog(err, rd.Array());
                                parent.OnTrace?.Invoke(DateTime.Now, parent, err, rd.Array(), 0, LogFile, 0);
                                throw new TimeoutException(err);
                            }
                            // sem 07.11.2022
                            // прекращаем попытки отправки по команде пользователя
                            else
                            if (_userCanceled)
                            {
                                if (received)
                                {
                                    rd.Position = 0;
                                    rd.Set(p.Reply);
                                    GXLogWriter.WriteLog(parent.Name + ": " /*null*/, rd.Array());
                                }
                                reply.Error = (int)ErrorCode.UserAbortMode;
                                throw new GXDLMSException(reply.Error);
                            }

                            rd.Position = 0;
                            rd.Set(p.Reply);
                        }
                    }
                    catch (Exception)
                    {
                        if (rd.Size != 0)
                        {
                            GXLogWriter.WriteLog(parent.Name + ": " /*null*/, rd.Array());
                            if (parent.OnTrace != null)
                            {
                                int size = 0;
                                if (reply.Data != null)
                                {
                                    size = reply.Data.Size;
                                }
                                if (Properties.Settings.Default.TraceTime)
                                {
                                    parent.OnTrace(DateTime.Now, parent, "\r\nRX:\t", rd.Array(), size, LogFile, (int)(DateTime.Now - start).TotalMilliseconds);
                                }
                                else
                                {
                                    parent.OnTrace(DateTime.Now, parent, "RX:\t", rd.Array(), size, LogFile, (int)(DateTime.Now - start).TotalMilliseconds);
                                }
                            }
                            if (Properties.Settings.Default.LogDuration)
                            {
                                GXLogWriter.WriteLog(parent.Name + ": " + csDurationMsg + ((int)(DateTime.Now - start).TotalMilliseconds).ToString(), false);
                            }
                        }
                        //Throw original exception.
                        throw;
                    }
                }
                catch (TimeoutException ex)
                {
                    GXLogWriter.WriteLog($"{parent.Name}: {ex.Message}");
                    throw;
                }
                catch (GXDLMSException dlmsEx)
                {
                    if (dlmsEx.ErrorCode == (int)ErrorCode.UserAbortMode)
                    {
                        string err = parent.Name + ": " + csUserAbortMsg;
                        GXLogWriter.WriteLog(err);
                        parent.Disconnect();
                        throw;
                    }
                }
            GXLogWriter.WriteLog(parent.Name + ": " /*null*/, rd.Array());
            if (parent.OnTrace != null)
            {
                int size = 0;
                if (reply.Data != null)
                {
                    size = reply.Data.Size;
                }
                if (Properties.Settings.Default.TraceTime)
                {
                    parent.OnTrace(DateTime.Now, parent, "\r\nRX:\t", rd.Array(), size, LogFile, (int)(DateTime.Now - start).TotalMilliseconds);
                }
                else
                {
                    parent.OnTrace(DateTime.Now, parent, "RX:\t", rd.Array(), size, LogFile, (int)(DateTime.Now - start).TotalMilliseconds);
                }
            }

            if (Properties.Settings.Default.LogDuration)
            {
                GXLogWriter.WriteLog(parent.Name + ": " + csDurationMsg + ((int)(DateTime.Now - start).TotalMilliseconds).ToString(), false);
            }

            if (reply.Error != 0)
            {
#if ZIP
                ErrorCode errCode = (ErrorCode)reply.Error;
                GXLogWriter.WriteLog($"{parent.Name}: Ошибка: {errCode.ToName()}");
                if (errCode == ErrorCode.DisconnectMode || errCode == ErrorCode.Rejected)
                {
#else
                if ((ErrorCode)reply.Error == ErrorCode.DisconnectMode)
                {
#endif
                    parent.Disconnect();
                }

                throw new GXDLMSException(reply.Error);
            }
            //else
            //    parent.KeepAliveRestart();
        }

        private char GetIecBaudRate(int baudrate)
        {
            char rate;
            switch (baudrate)
            {
                case 300:
                    rate = '0';
                    break;
                case 600:
                    rate = '1';
                    break;
                case 1200:
                    rate = '2';
                    break;
                case 2400:
                    rate = '3';
                    break;
                case 4800:
                    rate = '4';
                    break;
                case 9600:
                    rate = '5';
                    break;
                case 19200:
                    rate = '6';
                    break;
                default:
                    throw new Exception(csUnknownBaudRate);
            }
            return rate;
        }

        /// <summary>
        /// Send IEC disconnect message.
        /// </summary>
        void DiscIEC()
        {
            ReceiveParameters<string> p = new ReceiveParameters<string>()
            {
                AllData = false,
                Eop = (byte)0x0A,
                WaitTime = parent.WaitTime * 1000
            };
            string data = (char)0x01 + "B0" + (char)0x03 + "\r\n";
            media.Send(data, null);
            p.Count = 1;
            media.Receive(p);
        }

        internal string InitializeIEC()
        {
            string manufactureID = null;
            if (parent.Manufacturer != null)
            {
                GXManufacturer manufacturer = this.parent.Manufacturers.FindByIdentification(parent.Manufacturer);
                if (manufacturer == null)
                {
                    throw new Exception(csUnknownManufactMsg + parent.Manufacturer);
                }
            }
            byte Terminator = 0x0A;
            if (!media.IsOpen)
            {
                if (media is GXSerial serialPort)
                {
                    int timeout = (parent == null || (parent != null && parent.WaitTime <= 0)
                            ? 5
                            : parent.WaitTime + 1) * 1000;
                    if (serialPort.ReadTimeout == -1)
                    {
                        serialPort.ReadTimeout = timeout;
                    }
                    if (serialPort.WriteTimeout == -1)
                    {
                        serialPort.WriteTimeout = timeout;
                    }
                    //GXLogWriter.WriteLog($"{parent?.Name}: Таймаут (мсек.) операций записи {serialPort.WriteTimeout}, чтения {serialPort.ReadTimeout} ");
                }
                media.Open();
            }
            if (media is GXSerial)
            {
                //Some meters need a little break.
                Thread.Sleep(1000);
            }

            // получение данных устройства через оптопорт
            if (media is GXSerial serial && parent.InterfaceType == InterfaceType.HdlcWithModeE)
            {
                string data = "/?!\r\n";
                if (this.parent.HDLCAddressing == HDLCAddressType.SerialNumber)
                {
                    data = "/?" + this.parent.PhysicalAddress + "!\r\n";
                }
                GXLogWriter.WriteLog("Отправка IEC:" + data);
                ReceiveParameters<string> p = new ReceiveParameters<string>()
                {
                    AllData = false,
                    Eop = Terminator,
                    WaitTime = parent.WaitTime * 1000
                };
                lock (media.Synchronous)
                {
                    media.Send(data, null);
                    if (!media.Receive(p))
                    {
                        //Try to move away from mode E.
                        try
                        {
                            GXReplyData reply = new GXReplyData();
                            this.ReadDLMSPacket(this.DisconnectRequest(), 1, reply);
                        }
                        catch (Exception)
                        {
                        }
                        DiscIEC();
                        string str = parent.Name + ": " + csTimeoutReplyMsg;
                        GXLogWriter.WriteLog(str);
                        media.Send(data, null);
                        if (!media.Receive(p))
                        {
                            throw new Exception(str);
                        }
                    }
                    //If echo is used.
                    if (p.Reply == data)
                    {
                        p.Reply = null;
                        if (!media.Receive(p))
                        {
                            //Try to move away from mode E.
                            GXReplyData reply = new GXReplyData();
                            this.ReadDLMSPacket(this.DisconnectRequest(), 1, reply);
                            if (serial != null)
                            {
                                DiscIEC();
                                serial.DtrEnable = serial.RtsEnable = false;
                                serial.BaudRate = 9600;
                                serial.DtrEnable = serial.RtsEnable = true;
                                DiscIEC();
                            }
                            data = parent.Name + ": " + csTimeoutReplyMsg;
                            GXLogWriter.WriteLog(data);
                            throw new Exception(data);
                        }
                    }
                }
                int pos = 0;
                //With some meters there might be some extra invalid chars. Remove them.
                while (pos < p.Reply.Length && p.Reply[pos] != '/')
                {
                    ++pos;
                }
                GXLogWriter.WriteLog(parent.Name + ": " + "Получен пакет: " + p.Reply);
                if (p.Reply[pos] != '/')
                {
                    p.WaitTime = 100;
                    media.Receive(p);
                    throw new Exception("Недопустимый ответ.");
                }
                manufactureID = p.Reply.Substring(pos + 1, 3);
                UpdateManufactureSettings(manufactureID);

                char baudrate = p.Reply[pos + 4];
                int BaudRate;
                switch (baudrate)
                {
                    case '0':
                        BaudRate = 300;
                        break;
                    case '1':
                        BaudRate = 600;
                        break;
                    case '2':
                        BaudRate = 1200;
                        break;
                    case '3':
                        BaudRate = 2400;
                        break;
                    case '4':
                        BaudRate = 4800;
                        break;
                    case '5':
                        BaudRate = 9600;
                        break;
                    case '6':
                        BaudRate = 19200;
                        break;
                    default:
                        throw new Exception(csUnknownBaudRate);
                }
                if (parent.MaximumBaudRate != 0)
                {
                    BaudRate = parent.MaximumBaudRate;
                    baudrate = GetIecBaudRate(BaudRate);
                    GXLogWriter.WriteLog(parent.Name + ": " + "Установлена макс. скорость обмена : " + BaudRate.ToString());
                }
                GXLogWriter.WriteLog(parent.Name + ": " + "Скорость обмена : " + BaudRate.ToString());
                //Send ACK
                //Send Protocol control character
                // "2" HDLC protocol procedure (Mode E)
                byte controlCharacter = (byte)'2';
                //Send Baud rate character
                //Mode control character
                byte ModeControlCharacter = (byte)'2';
                //"2" //(HDLC protocol procedure) (Binary mode)
                //Set mode E.
                byte[] arr = new byte[] { 0x06, controlCharacter, (byte)baudrate, ModeControlCharacter, 13, 10 };
                GXLogWriter.WriteLog(parent.Name + ": " + "Переключение в режим E.", arr);
                lock (media.Synchronous)
                {
                    p.Reply = null;
                    media.Send(arr, null);
                    p.WaitTime = 2000;
                    //Note! All meters do not echo this.
                    media.Receive(p);
                    if (p.Reply != null)
                    {
                        GXLogWriter.WriteLog(parent.Name + ": " + "Получено: " + p.Reply);
                    }
                    media.Close();
                    serial.BaudRate = BaudRate;
                    serial.DataBits = 8;
                    serial.Parity = Parity.None;
                    serial.StopBits = StopBits.One;
                    media.Open();
                    //Some meters need this sleep. Do not remove.
                    Thread.Sleep(1000);
                }
            }
            return manufactureID;
        }

        void Media_OnTrace(object sender, TraceEventArgs e)
        {
            GXLogWriter.WriteLog(e.ToString());
            parent.OnTrace?.Invoke(DateTime.Now, parent, e.ToString(), null, 0, LogFile, 0);
        }

        /// <summary>
        /// Initialize network connection settings.
        /// </summary>
        /// <returns></returns>
        void InitNet()
        {
            try
            {
                if (parent.UseRemoteSerial)
                {
                    InitializeIEC();
                }
                else
                {
                    media.Open();
                }
            }
            catch (Exception Ex)
            {
                if (media != null)
                {
                    media.Close();
                }
                throw Ex;
            }
        }

        public int ClientAddress
        {
            get
            {
                return client.ClientAddress;
            }
        }

        public int ServerAddress
        {
            get
            {
                return client.ServerAddress;
            }
        }

        public void UpdateManufactureSettings(string id)
        {
            if (!media.IsOpen)
            {
                media.Settings = parent.MediaSettings;
            }
            if (!string.IsNullOrEmpty(this.parent.Manufacturer) && string.Compare(this.parent.Manufacturer, id, true) != 0)
            {
                throw new Exception(string.Format(csMissingManufactureMsg, id, this.parent.Manufacturer));
            }
            GXManufacturer manufacturer = null;
            if (!string.IsNullOrEmpty(this.parent.Manufacturer))
            {
                manufacturer = this.parent.Manufacturers.FindByIdentification(id);
                if (manufacturer == null)
                {
                    throw new Exception(csUnknownManufactMsg + id);
                }
                this.parent.Manufacturer = manufacturer.Identification;
            }

            // sem 03.12.2021 проверяем, что устройство не производства ELGAMA
            // иначе используем настройки из драйвера Политариф
            if (this.parent.Manufacturer != "EGM")
            {
                client.Standard = this.parent.Standard;
                client.Authentication = this.parent.Authentication;
                client.InterfaceType = InterfaceType.HDLC;

                client.UseLogicalNameReferencing = this.parent.UseLogicalNameReferencing;
                client.ProposedConformance = (Conformance)parent.Conformance;
                client.UseUtc2NormalTime = parent.UtcTimeZone;
                client.DateTimeSkips = parent.DateTimeSkips;

                //Show media verbose.
                if (this.parent.Verbose && media.Trace != System.Diagnostics.TraceLevel.Verbose)
                {
                    media.Trace = System.Diagnostics.TraceLevel.Verbose;
                    media.OnTrace += new TraceEventHandler(Media_OnTrace);
                }
                else if (!this.parent.Verbose && media.Trace == System.Diagnostics.TraceLevel.Verbose)
                {
                    media.Trace = System.Diagnostics.TraceLevel.Off;
                    media.OnTrace -= new TraceEventHandler(Media_OnTrace);
                }

                //If network media is used check is manufacturer supporting IEC 62056-47
                if (client.InterfaceType == InterfaceType.WRAPPER && (parent.UseRemoteSerial || this.media is GXSerial))
                {
                    client.InterfaceType = InterfaceType.HDLC;
                }
                client.ClientAddress = parent.ClientAddress;
                if (parent.HDLCAddressing == HDLCAddressType.SerialNumber && manufacturer != null)
                {
                    string formula = null;
                    GXServerAddress server = manufacturer.GetServer(parent.HDLCAddressing);
                    if (server != null)
                    {
                        formula = server.Formula;
                    }
                    client.ServerAddress = GXDLMSClient.GetServerAddressFromSerialNumber(parent.PhysicalAddress, parent.LogicalAddress, formula);
                    client.ServerAddressSize = 4;
                }
                else
                {
                    if (client.InterfaceType == InterfaceType.WRAPPER)
                    {
                        client.ServerAddress = Convert.ToInt32(parent.PhysicalAddress);
                    }
                    else
                    {
                        client.ServerAddress = GXDLMSClient.GetServerAddress(
                            parent.LogicalAddress,
                            Convert.ToInt32(parent.PhysicalAddress),
                            parent.ServerAddressSize
                        );
                        client.ServerAddressSize = parent.ServerAddressSize;
                    }
                }

                /* sem 05.07.2021 Восстановить и отладить работу с блоком шифрования
                client.Ciphering.Security = parent.Security;
                if (!string.IsNullOrEmpty(parent.SystemTitle) && !string.IsNullOrEmpty(parent.BlockCipherKey) && !string.IsNullOrEmpty(parent.AuthenticationKey))
                {
                    client.Ciphering.SystemTitle = GXCommon.HexToBytes(parent.SystemTitle);
                    client.Ciphering.BlockCipherKey = GXCommon.HexToBytes(parent.BlockCipherKey);
                    client.Ciphering.AuthenticationKey = GXCommon.HexToBytes(parent.AuthenticationKey);
                    client.Ciphering.InvocationCounter = parent.InvocationCounter;
                }
                else
                {
                    client.Ciphering.SystemTitle = null;
                    client.Ciphering.BlockCipherKey = null;
                    client.Ciphering.AuthenticationKey = null;
                    client.Ciphering.InvocationCounter = 0;
                }

                if (!string.IsNullOrEmpty(parent.Challenge))
                {
                    client.CtoSChallenge = GXCommon.HexToBytes(parent.Challenge);
                }
                else
                {
                    client.CtoSChallenge = null;
                }

                if (!string.IsNullOrEmpty(parent.PhysicalDeviceAddress))
                {
                    client.Gateway = new GXDLMSGateway
                    {
                        NetworkId = parent.NetworkId,
                        PhysicalDeviceAddress = GXCommon.HexToBytes(parent.PhysicalDeviceAddress)
                    };
                }
                else
                {
                    client.Gateway = null;
                }
                */
            }
            else
            {
                // sem 03.12.2021 из драйвера Политариф

                //if (
                //    media is GXNet && InitializeAL

                //)
                //{
                //    parent.InterfaceType = InterfaceType.WRAPPER;
                //}
                //else
                //{
                //    client.InterfaceType = InterfaceType.HDLC;
                //}

                client.InterfaceType = InterfaceType.HDLC;
                client.UseLogicalNameReferencing = true;
                client.Authentication = Authentication.Low;

                client.ClientAddress = 0x01;
                client.ServerAddress = GXDLMSClient.GetServerAddressFromSerialNumber(parent.PhysicalAddress, parent.LogicalAddress);
                client.ServerAddressSize = 4;
                client.ProposedConformance = (Conformance)(0x1C1B20);
            }
        }

        /// <summary>
        /// Initialize serial port connection to COSEM/DLMS device.
        /// </summary>
        /// <returns></returns>
        void InitSerial()
        {
            try
            {
                InitializeIEC();
            }
            catch (Exception Ex)
            {
                if (media != null)
                {
                    media.Close();
                }
                throw Ex;
            }
        }

        /// <summary>
        /// Initialize serial port connection to COSEM/DLMS device.
        /// </summary>
        /// <returns></returns>
        void InitTerminal()
        {
            try
            {
                InitializeIEC();
            }
            catch (Exception Ex)
            {
                if (media != null)
                {
                    media.Close();
                }
                throw Ex;
            }
        }

        public void UpdateSettings()
        {
            if (!media.IsOpen)
            {
                media.Settings = parent.MediaSettings;
            }
            client.Authentication = this.parent.Authentication;
            client.InterfaceType = parent.InterfaceType;
            client.Plc.MacSourceAddress = parent.MACSourceAddress;
            client.Plc.MacDestinationAddress = parent.MacDestinationAddress;

            if (!string.IsNullOrEmpty(this.parent.Password))
            {
                client.Password = CryptHelper.Decrypt(this.parent.Password, ZipPwddKey.Key);
            }
            else if (this.parent.HexPassword != null)
            {
                client.Password = CryptHelper.Decrypt(this.parent.HexPassword, ZipPwddKey.Key);
            }

            client.UseLogicalNameReferencing = this.parent.UseLogicalNameReferencing;
            client.UseUtc2NormalTime = parent.UtcTimeZone;
            //Show media verbose.
            if (this.parent.Verbose && media.Trace != System.Diagnostics.TraceLevel.Verbose)
            {
                media.Trace = System.Diagnostics.TraceLevel.Verbose;
                media.OnTrace += new TraceEventHandler(Media_OnTrace);
            }
            else if (!this.parent.Verbose && media.Trace == System.Diagnostics.TraceLevel.Verbose)
            {
                media.Trace = System.Diagnostics.TraceLevel.Off;
                media.OnTrace -= new TraceEventHandler(Media_OnTrace);
            }

            //If network media is used check is manufacturer supporting IEC 62056-47
            if (client.InterfaceType == InterfaceType.WRAPPER && (parent.UseRemoteSerial || this.media is GXSerial))
            {
                client.InterfaceType = InterfaceType.HDLC;
            }

            client.ClientAddress = parent.ClientAddress;
            if (parent.HDLCAddressing == HDLCAddressType.SerialNumber)
            {
                string formula = null;
                GXManufacturer manufacturer = this.parent.Manufacturers.FindByIdentification(parent.Manufacturer);
                GXServerAddress server = manufacturer.GetServer(parent.HDLCAddressing);
                if (server != null)
                {
                    formula = server.Formula;
                }
                client.ServerAddress = GXDLMSClient.GetServerAddressFromSerialNumber(parent.PhysicalAddress, parent.LogicalAddress, formula);// GetServerAddress(Convert.ToInt32(parent.PhysicalAddress), formula);
                client.ServerAddressSize = 4;
            }
            else
            {
                if (client.InterfaceType == InterfaceType.HDLC || client.InterfaceType == InterfaceType.HdlcWithModeE || client.InterfaceType == InterfaceType.PlcHdlc)
                {
                    client.ServerAddress = GXDLMSClient.GetServerAddress(parent.LogicalAddress, Convert.ToInt32(parent.PhysicalAddress), parent.ServerAddressSize);
                    client.ServerAddressSize = parent.ServerAddressSize;
                }
                else
                {
                    client.ServerAddress = Convert.ToInt32(parent.PhysicalAddress);
                }
            }
            client.Ciphering.Security = parent.Security;
            if (parent.SystemTitle != null && parent.BlockCipherKey != null && parent.AuthenticationKey != null)
            {
                client.Ciphering.SystemTitle = GXCommon.HexToBytes(parent.SystemTitle);
                client.Ciphering.BlockCipherKey = GXCommon.HexToBytes(parent.BlockCipherKey);
                client.Ciphering.AuthenticationKey = GXCommon.HexToBytes(parent.AuthenticationKey);
                client.Ciphering.InvocationCounter = parent.InvocationCounter;
            }
            else
            {
                client.Ciphering.SystemTitle = null;
                client.Ciphering.BlockCipherKey = null;
                client.Ciphering.AuthenticationKey = null;
                client.Ciphering.InvocationCounter = 0;
            }

            if (!string.IsNullOrEmpty(parent.Challenge))
            {
                client.CtoSChallenge = GXCommon.HexToBytes(parent.Challenge);
            }
            else
            {
                client.CtoSChallenge = null;
            }
            if (!string.IsNullOrEmpty(parent.DedicatedKey))
            {
                client.Ciphering.DedicatedKey = GXCommon.HexToBytes(parent.DedicatedKey);
            }
            else
            {
                client.Ciphering.DedicatedKey = null;
            }
            client.HdlcSettings.WindowSizeRX = parent.WindowSizeRX;
            client.HdlcSettings.WindowSizeTX = parent.WindowSizeTX;
            client.HdlcSettings.UseFrameSize = parent.UseFrameSize;
            client.HdlcSettings.MaxInfoRX = parent.MaxInfoRX;
            client.HdlcSettings.MaxInfoTX = parent.MaxInfoTX;
            client.MaxReceivePDUSize = parent.PduSize;
            client.UserId = parent.UserId;
            client.Priority = parent.Priority;
            client.ServiceClass = parent.ServiceClass;
            if (parent.PreEstablished)
            {
                client.ServerSystemTitle = GXCommon.HexToBytes(parent.ServerSystemTitle);
            }
        }

        public void InitializeConnection(bool force)
        {
            if (force || !media.IsOpen)
            {
                if (!string.IsNullOrEmpty(parent.Manufacturer))
                {
                    UpdateManufactureSettings(parent.Manufacturer);
                }
                if (media is GXSerial serial)
                {
                    GXLogWriter.WriteLog(parent.Name + ": " + csInitSerialConnection);
                    InitSerial();
                    connectionStartTime = DateTime.Now;
                }
                else if (media is GXNet)
                {
                    GXLogWriter.WriteLog(parent.Name + ": " + csInitNetConnection);
                    InitNet();
                    //Some Electricity meters need some time before first message can be send.
                    System.Threading.Thread.Sleep(500);
                }
                else if (media is Gurux.Terminal.GXTerminal)
                {
                    GXLogWriter.WriteLog(parent.Name + ": " + csInitTerminalConnection);
                    InitTerminal();
                }
                else
                {
                    if (media is IGXMedia2 media1)
                    {
                        media1.AsyncWaitTime = (uint)parent.WaitTime;
                    }
                    media.Open();
                }
            }
            try
            {
                // для счетчиков, произведенных не ELGAMA
                if (parent.Manufacturer != "EGM")
                {
                    GXReplyData reply = new GXReplyData();
                    byte[] data;
                    UpdateSettings();
                    //Read frame counter if GeneralProtection is used.
                    if (!string.IsNullOrEmpty(parent.FrameCounter) && client.Ciphering != null && client.Ciphering.Security != (byte)Security.None)
                    {
                        reply.Clear();
                        int add = client.ClientAddress;
                        Authentication auth = client.Authentication;
                        Security security = client.Ciphering.Security;
                        byte[] challenge = client.CtoSChallenge;
                        try
                        {
                            client.ClientAddress = 16;
                            client.Authentication = Authentication.None;
                            client.Ciphering.Security = (byte)Security.None;

                            data = SNRMRequest();
                            if (data != null)
                            {
                                try
                                {
                                    ReadDataBlock(data, csSNRMRequestMsg, 1, 1, reply);
                                }
                                catch (TimeoutException)
                                {
                                    reply.Clear();
                                    ReadDataBlock(DisconnectRequest(true), csDisconnectRequest, 1, 1, reply);
                                    reply.Clear();
                                    ReadDataBlock(data, csSNRMRequestMsg, 1, 1, reply);
                                }
                                catch (Exception e)
                                {
                                    reply.Clear();
                                    ReadDataBlock(DisconnectRequest(), csDisconnectRequest, 1, 1, reply);
                                    throw e;
                                }
                                GXLogWriter.WriteLog(parent.Name + ": " + csUAReplySuccessMsg);
                                //Has server accepted client.
                                ParseUAResponse(reply.Data);
                            }
                            ReadDataBlock(AARQRequest(), csAARQRequestMsg, reply);
                            try
                            {
                                //Parse reply.
                                ParseAAREResponse(reply.Data);
                                GXLogWriter.WriteLog(parent.Name + ": " + csAAREReplySuccessMsg);
                                reply.Clear();
                                GXDLMSData d = new GXDLMSData(parent.FrameCounter);
                                ReadDLMSPacket(Read(d, 2), reply);
                                client.UpdateValue(d, 2, reply.Value);
                                client.Ciphering.InvocationCounter = parent.InvocationCounter = 1 + Convert.ToUInt32(d.Value);
                                reply.Clear();
                                ReadDataBlock(DisconnectRequest(), csDisconnectRequestMsg, reply);
                            }
                            catch (Exception Ex)
                            {
                                reply.Clear();
                                ReadDataBlock(DisconnectRequest(), csDisconnectRequestMsg, reply);
                                throw Ex;
                            }
                        }
                        finally
                        {
                            client.ClientAddress = add;
                            client.Authentication = auth;
                            client.Ciphering.Security = security;
                            client.CtoSChallenge = challenge;
                        }
                    }
                    data = SNRMRequest();
                    if (data != null)
                    {
                        try
                        {
                            reply.Clear();
                            ReadDataBlock(data, csSNRMRequestMsg, 1, parent.ResendCount, reply);
                        }
                        catch (TimeoutException)
                        {
                            reply.Clear();
                            ReadDataBlock(DisconnectRequest(true), csDisconnectRequest, 1, parent.ResendCount, reply);
                            reply.Clear();
                            ReadDataBlock(data, csSNRMRequestMsg, reply);
                        }
                        catch (Exception e)
                        {
                            reply.Clear();
                            ReadDataBlock(DisconnectRequest(), csDisconnectRequest, reply);
                            throw e;
                        }
                        GXLogWriter.WriteLog(parent.Name + ": " + csUAReplySuccessMsg);
                        //Has server accepted client.
                        ParseUAResponse(reply.Data);
                    }
                    if (!parent.PreEstablished)
                    {
                        //Generate AARQ request.
                        //Split requests to multiple packets if needed.
                        //If password is used all data might not fit to one packet.
                        reply.Clear();
                        ReadDataBlock(AARQRequest(), csAARQRequestMsg, reply);
                        try
                        {
                            //Parse reply.
                            ParseAAREResponse(reply.Data);
                            GXLogWriter.WriteLog(parent.Name + ": " + csAAREReplySuccessMsg);
                        }
                        catch (Exception Ex)
                        {
                            reply.Clear();
                            ReadDLMSPacket(DisconnectRequest(), 1, reply);
                            throw Ex;
                        }
                        //If authentication is required.
                        if (client.Authentication > Authentication.Low)
                        {
                            reply.Clear();
                            ReadDataBlock(client.GetApplicationAssociationRequest(), "Authenticating.", reply);
                            client.ParseApplicationAssociationResponse(reply.Data);
                        }
                    }
                }
                else
                {   // счетчики ELGAMA в отдельном блоке...
                    GXLogWriter.WriteLog("SNRM/UA: ", new byte[0]);
                    var reply = new GXReplyData();
                    byte[] data;
                    data = SNRMRequest();
                    if (data != null)
                    {
                        ReadDLMSPacket(data, reply);
                        ParseUAResponse(reply.Data);
                    }

                    GXLogWriter.WriteLog("AARQ/AARE: ", new byte[0]);
                    foreach (byte[] it in AARQRequest())
                    {
                        reply.Clear();
                        ReadDLMSPacket(it, reply);
                    }
                    ParseAAREResponse(reply.Data);

                    reply.Clear();
                    if (client.IsAuthenticationRequired)
                    {
                        foreach (byte[] it in client.GetApplicationAssociationRequest())
                        {
                            reply.Clear();
                            ReadDLMSPacket(it, reply);
                        }
                        client.ParseApplicationAssociationResponse(reply.Data);
                    }

                }
                GXLogWriter.WriteLog($"Устройство [{parent.Name}] подключено.");
            }
            catch (Exception)
            {
                if (media is GXSerial && parent.StartProtocol == StartProtocolType.IEC)
                {
                    ReceiveParameters<string> p = new ReceiveParameters<string>()
                    {
                        Eop = (byte)0xA,
                        WaitTime = parent.WaitTime * 1000
                    };
                    lock (media.Synchronous)
                    {
                        string data = (char)0x01 + "B0" + (char)0x03 + "\r\n";
                        media.Send(data, null);
                        media.Receive(p);
                    }
                }
                throw;
            }
            //parent.KeepAliveRestart();
        }

        void NotifyProgress(string description, int current, int maximium)
        {
            OnProgress?.Invoke(this, description, current, maximium);
        }

        public void ReadDataBlock(byte[][] data, string logText, GXReplyData reply)
        {
            foreach (byte[] it in data)
            {
                reply.Clear();
                try
                {
                    ReadDataBlock(it, logText, reply);
                }
                catch (Exception ex)
                {
                    // Update frame ID if meter returns error.
                    if (client.InterfaceType == InterfaceType.HDLC ||
                        client.InterfaceType == InterfaceType.HdlcWithModeE ||
                        client.InterfaceType == InterfaceType.PlcHdlc)
                    {
                        GXDLMSClient.GetHdlcAddressInfo(new GXByteBuffer(it), out int target, out int source, out byte type);
                        client.HdlcSettings.SenderFrame = type;
                    }
                    GXLogWriter.WriteLog($"Ошибка ReadDataBlock: {ex.Message}", LogLevel.Error);
                    // TODO: SEM - оттестировать генерацию исключения (оставить\удалить)
                    throw ex;
                }
            }
        }

        public void ReadDataBlock(byte[] data, string logText, GXReplyData reply)
        {
            ReadDataBlock(data, logText, 1, reply);
        }

        public delegate void DataReceivedEventHandler(object sender, GXReplyData reply);
        public event DataReceivedEventHandler OnDataReceived;
        GXDLMSProfileGeneric CurrentProfileGeneric;

        void OnProfileGenericDataReceived(object sender, GXReplyData reply)
        {
            if (reply.Value != null)
            {
                lock (reply)
                {
                    client.UpdateValue(CurrentProfileGeneric, 2, reply.Value);
                    reply.Value = new Object[0];
                }
                OnAfterRead?.Invoke(null, CurrentProfileGeneric, 2, null, null, null);
            }
        }

        internal void ReadDataBlock(byte[][] data, string logText, int multiplier, int tryCount, GXReplyData reply)
        {
            foreach (byte[] it in data)
            {
                reply.Clear();
                try
                {
                    ReadDataBlock(it, logText, multiplier, tryCount, reply);
                }
                catch (Exception ex)
                {
                    //Update frame ID if meter returns error.
                    if (client.InterfaceType == InterfaceType.HDLC ||
                        client.InterfaceType == InterfaceType.HdlcWithModeE ||
                        client.InterfaceType == InterfaceType.PlcHdlc)
                    {
                        GXDLMSClient.GetHdlcAddressInfo(new GXByteBuffer(it), out int target, out int source, out byte type);
                        //if(reply.Error == (int)ErrorCode.LongGetOrReadAborted)
                        //{
                        //    client.Settings.ResetBlockIndex();
                        //    client.Settings.ResetFrameSequence();
                        //}
                        //else
                        client.HdlcSettings.SenderFrame = type;
                    }
                    throw ex;
                }
            }
        }

        internal void ReadDataBlock(byte[][] data, string logText, int multiplier, GXReplyData reply)
        {
            foreach (byte[] it in data)
            {
                reply.Clear();
                try
                {
                    ReadDataBlock(it, logText, multiplier, reply);
                }
                catch (Exception ex)
                {
                    //Update frame ID if meter returns error.
                    if (client.InterfaceType == InterfaceType.HDLC ||
                        client.InterfaceType == InterfaceType.HdlcWithModeE ||
                        client.InterfaceType == InterfaceType.PlcHdlc)
                    {
                        GXDLMSClient.GetHdlcAddressInfo(new GXByteBuffer(it), out int target, out int source, out byte type);
                        //if (reply.Error == (int)ErrorCode.LongGetOrReadAborted)
                        //{
                        //    client.Settings.ResetBlockIndex();
                        //    client.Settings.ResetFrameSequence();
                        //}
                        //else
                        client.HdlcSettings.SenderFrame = type;
                    }
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Read data block from the device.
        /// </summary>
        /// <param name="data">data to send</param>
        /// <param name="text">Progress text.</param>
        /// <param name="multiplier"></param>
        /// <returns>Received data.</returns>
        internal void ReadDataBlock(byte[] data, string text, int multiplier, GXReplyData reply)
        {
            ReadDataBlock(data, text, multiplier, parent.ResendCount, reply);
        }

        /// <summary>
        /// Read data block from the device.
        /// </summary>
        /// <param name="data">data to send</param>
        /// <param name="text">Progress text.</param>
        /// <param name="multiplier"></param>
        /// <returns>Received data.</returns>
        internal void ReadDataBlock(byte[] data, string text, int multiplier, int tryCount, GXReplyData reply)
        {
            if (data == null || _userCanceled)
            {
                return;
            }

            const string csGetNext = "Получение следующего";
            lastTransaction = DateTime.Now;
            GXLogWriter.WriteLog(parent.Name + ": " + text, data);
            parent.OnTrace?.Invoke(lastTransaction, parent, text + "\r\nTX:\t", data, 0, LogFile, 0);

            ReadDLMSPacket(data, tryCount, reply);

            OnDataReceived?.Invoke(this, reply);
            if (reply.IsMoreData && !_userCanceled)
            {
                if (reply.TotalCount != 1)
                {
                    NotifyProgress(text, 1, multiplier * reply.TotalCount);
                }
                while (reply.IsMoreData && !_userCanceled)
                {
                    data = client.ReceiverReady(reply.MoreData);
                    if ((reply.MoreData & RequestTypes.Frame) != 0)
                    {
                        GXLogWriter.WriteLog($"{csGetNext} кадра.");
                    }
                    else
                    {
                        GXLogWriter.WriteLog($"{csGetNext} блока данных.");
                    }
                    parent.OnTrace?.Invoke(DateTime.Now, parent, "\r\nTX:\t", data, 0, LogFile, 0);
                    GXLogWriter.WriteLog(text, data);
                    ReadDLMSPacket(data, reply);
                    OnDataReceived?.Invoke(this, reply);
                    if (reply.TotalCount != 1)
                    {
                        NotifyProgress(text, reply.Count, multiplier * reply.TotalCount);
                    }
                }
            }
        }

        public GXDLMSObjectCollection GetObjects(string logicalName = null)
        {
            const string csGetObjects = "Загрузка текущей ассоциации ПУ";
            GXLogWriter.WriteLog($"--- {csGetObjects} {parent.Name} ---");
            GXDLMSObjectCollection objs;
            GXReplyData reply = new GXReplyData();
            try
            {
                if (OnBeforeRead != null)
                {
                    GXDLMSObject target;
                    if (client.UseLogicalNameReferencing)
                    {
                        target = new GXDLMSAssociationLogicalName();
                    }
                    else
                    {
                        target = new GXDLMSAssociationShortName();
                    }
                    OnBeforeRead(client, target, 2, null, null, null);
                }
                ReadDataBlock(client.GetObjectsRequest(logicalName), csGetObjects, 3, 1, reply);
            }
            catch (Exception Ex)
            {
                throw new Exception($"Ошибка в GetObjects: {Ex.Message}");
            }

            objs = client.ParseObjects(reply.Data, true);
            if (OnAfterRead != null)
            {
                if (client.UseLogicalNameReferencing && string.IsNullOrEmpty(logicalName))
                {
                    GXDLMSObject ln = client.Objects.FindByLN(ObjectType.AssociationLogicalName, "0.0.40.0.0.255");
                    //All meters don't add default association.
                    if (ln == null)
                    {
                        ln = client.Objects.GetObjects(ObjectType.AssociationLogicalName)[0];
                    }
                    OnAfterRead(client, ln, 2, reply.Data, null, null);
                }
                else
                {
                    OnAfterRead(client, client.Objects.FindBySN(0xFA00), 2, reply.Data, null, null);
                }
            }
            GXLogWriter.WriteLog($"--- {csGetObjects} {parent.Name}:  всего {objs.Count.ToString()} объектов. ---");
            return objs;
        }

        class GXAttributeRead
        {
            public PropertyInfo Info;
            public GXDLMSAttribute Attribute;

            public GXAttributeRead(PropertyInfo info, GXDLMSAttribute attribute)
            {
                Info = info;
                Attribute = attribute;
            }
        }

        delegate void ClearProfileGenericDataEventHandler();

        public void ReadProfileGenericData(GXDLMSObject obj)
        {
            GXReplyData reply = new GXReplyData();
            int it = 2;
            //If object is not ProfileGeneric.
            if (!(obj is GXDLMSProfileGeneric))
            {
                obj.ClearStatus(it);
                GXLogWriter.WriteLog($"{parent.Name}: Объект не является классом GXDLMSProfileGeneric.", LogLevel.Error);
                return;
            }

            //If reading is not allowed.
            if ((obj.GetAccess(it) & AccessMode.Read) == 0)
            {
                obj.ClearStatus(it);
                GXLogWriter.WriteLog($"{parent.Name}: Отсутствуют права доступа на чтение данных профиля.", LogLevel.Error);
                return;
            }

            obj.ClearStatus(it);
            reply.Clear();
            OnBeforeRead?.Invoke(null, obj, it, null, null, null);
            object parameters = null;
            PduEventHandler p = new PduEventHandler(delegate (object sender1, byte[] value)
            {
                try
                {
                    GXDLMSTranslator t = new GXDLMSTranslator
                    {
                        Hex = false
                    };
                    string xml = null;
                    xml = t.PduToXml(value);
                    XmlDocument doc2 = new XmlDocument();
                    doc2.LoadXml(xml);
                    var tags = doc2.GetElementsByTagName("AccessParameters");
                    if (tags != null && tags.Count != 0)
                    {
                        xml = tags[0].InnerXml;
                        int type = int.Parse(doc2.GetElementsByTagName("AccessSelector")[0].Attributes[0].Value);
                        parameters = new GXStructure() { type, GXDLMSTranslator.XmlToValue(xml) };
                    }
                }
                catch (Exception)
                {
                    //Ignore error.
                }
            });
            try
            {
                byte[][] tmp;
                const string csGetProfileData = "Чтение данных профиля";
                CurrentProfileGeneric = obj as GXDLMSProfileGeneric;
                client.OnPdu += p;
                OnDataReceived += new GXDLMSCommunicator.DataReceivedEventHandler(this.OnProfileGenericDataReceived);
                if (CurrentProfileGeneric.AccessSelector == AccessRange.Range ||
                        CurrentProfileGeneric.AccessSelector == AccessRange.Last)
                {
                    if (!(CurrentProfileGeneric.From is GXDateTime start))
                    {
                        start = Convert.ToDateTime(CurrentProfileGeneric.From);
                    }
                    if (!(CurrentProfileGeneric.To is GXDateTime end))
                    {
                        end = Convert.ToDateTime(CurrentProfileGeneric.To);
                    }
#if !ZIP
                    //Set seconds to zero.
                    start.Value = start.Value.AddSeconds(-start.Value.Second);
                    end.Value = end.Value.AddSeconds(-end.Value.Second);
#endif
                    tmp = client.ReadRowsByRange(CurrentProfileGeneric, start, end);
                    ReadDataBlock(tmp, csGetProfileData, 1, reply);
                }
                else if (CurrentProfileGeneric.AccessSelector == AccessRange.Entry)
                {
                    uint
                        from = Convert.ToUInt32(CurrentProfileGeneric.From),
                        count = Convert.ToUInt32(CurrentProfileGeneric.To);
                    tmp = client.ReadRowsByEntry(CurrentProfileGeneric, from, count);
                    ReadDataBlock(tmp, $"[{CurrentProfileGeneric.Description}]: {csGetProfileData}", 1, reply);
                }
                else //Read all.
                {
                    tmp = client.Read(CurrentProfileGeneric, 2);
                    ReadDataBlock(tmp, $"[{CurrentProfileGeneric.Description}]: {csGetProfileData}", 1, reply);
                }
                OnAfterRead?.Invoke(client, obj, it, reply.Data, parameters, null);
            }
            catch (GXDLMSException ex)
            {
                obj.SetLastError(it, ex);
                if (ex.ErrorCode == (int)ErrorCode.ReadWriteDenied ||
                        ex.ErrorCode == (int)ErrorCode.UndefinedObject ||
                        ex.ErrorCode == (int)ErrorCode.UnavailableObject ||
                        //Actaris returns access violation error.
                        ex.ErrorCode == (int)ErrorCode.AccessViolated ||
                        ex.ErrorCode == (int)ErrorCode.OtherReason)
                {
                    //Some meters return OtherReason if Profile Generic buffer is try to read with selective access.
                    if (!(obj is GXDLMSProfileGeneric && it == 2 && ex.ErrorCode == (int)ErrorCode.OtherReason))
                    {
                        obj.SetAccess(it, AccessMode.NoAccess);
                    }
                    OnAfterRead?.Invoke(null, obj, it, null, null, ex);
                }
                else
                {
                    //if (ex.ErrorCode == (int)ErrorCode.UserAbortMode)
                    //{
                    //    parent.PreEstablished = true;
                    //    parent.Comm.InitializeConnection(true);
                    //    return;
                    //}
                    OnAfterRead?.Invoke(null, obj, it, null, null, ex);
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                OnAfterRead?.Invoke(null, obj, it, null, null, ex);
                obj.SetLastError(it, ex);
                throw ex;
            }
            finally
            {
                client.OnPdu -= p;
                OnDataReceived -= new GXDLMSCommunicator.DataReceivedEventHandler(OnProfileGenericDataReceived);
            }
        }

        /// <summary>
        /// Read object.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="obj">Object to read.</param>
        /// <param name="forceRead">Force all attributes read.</param>
        public void Read(object sender, GXDLMSObject obj, bool forceRead)
        {
            GXReplyData reply = new GXReplyData();
            int[] indexes = (obj as IGXDLMSBase).GetAttributeIndexToRead(forceRead);
            foreach (int it in indexes)
            {
                //If reading is not allowed.
                if ((obj.GetAccess(it) & AccessMode.Read) == 0)
                {
                    obj.ClearStatus(it);
                    continue;
                }

                //If object is static and it's already read.
                if (!forceRead && obj.GetStatic(it) && obj.GetLastReadTime(it) != DateTime.MinValue)
                {
                    continue;
                }
                //Profile generic capture objects is not read here.
                if (forceRead && obj is GXDLMSProfileGeneric && it == 3)
                {
                    continue;
                }
                //obj.ClearStatus(it);
                //reply.Clear();
                if (obj is GXDLMSProfileGeneric && it == 2)
                    ReadProfileGenericData(obj);
                //{
                //    OnBeforeRead?.Invoke(null, obj, it, null, null, null);
                //    object parameters = null;
                //    PduEventHandler p = new PduEventHandler(delegate (object sender1, byte[] value)
                //    {
                //        try
                //        {
                //            GXDLMSTranslator t = new GXDLMSTranslator
                //            {
                //                Hex = false
                //            };
                //            string xml = null;
                //            xml = t.PduToXml(value);
                //            XmlDocument doc2 = new XmlDocument();
                //            doc2.LoadXml(xml);
                //            var tags = doc2.GetElementsByTagName("AccessParameters");
                //            if (tags != null && tags.Count != 0)
                //            {
                //                xml = tags[0].InnerXml;
                //                int type = int.Parse(doc2.GetElementsByTagName("AccessSelector")[0].Attributes[0].Value);
                //                parameters = new GXStructure() { type, GXDLMSTranslator.XmlToValue(xml) };
                //            }
                //        }
                //        catch (Exception)
                //        {
                //            //Ignore error.
                //        }
                //    });
                //    try
                //    {
                //        byte[][] tmp;
                //        const string csGetProfileData = "Чтение данных профиля";
                //        CurrentProfileGeneric = obj as GXDLMSProfileGeneric;
                //        client.OnPdu += p;
                //        OnDataReceived += new GXDLMSCommunicator.DataReceivedEventHandler(this.OnProfileGenericDataReceived);
                //        if (CurrentProfileGeneric.AccessSelector == AccessRange.Range ||
                //                CurrentProfileGeneric.AccessSelector == AccessRange.Last)
                //        {
                //            if (!(CurrentProfileGeneric.From is GXDateTime start))
                //            {
                //                start = Convert.ToDateTime(CurrentProfileGeneric.From);
                //            }
                //            if (!(CurrentProfileGeneric.To is GXDateTime end))
                //            {
                //                end = Convert.ToDateTime(CurrentProfileGeneric.To);
                //            }
                //            //Set seconds to zero.
                //            start.Value = start.Value.AddSeconds(-start.Value.Second);
                //            end.Value = end.Value.AddSeconds(-end.Value.Second);
                //            tmp = client.ReadRowsByRange(CurrentProfileGeneric, start, end);
                //            ReadDataBlock(tmp, csGetProfileData, 1, reply);
                //        }
                //        else if (CurrentProfileGeneric.AccessSelector == AccessRange.Entry)
                //        {
                //            tmp = client.ReadRowsByEntry(CurrentProfileGeneric, Convert.ToUInt32(CurrentProfileGeneric.From), Convert.ToUInt32(CurrentProfileGeneric.To));
                //            ReadDataBlock(tmp, $"[{CurrentProfileGeneric.Description}]: {csGetProfileData}", 1, reply);
                //        }
                //        else //Read all.
                //        {
                //            tmp = client.Read(CurrentProfileGeneric, 2);
                //            ReadDataBlock(tmp, $"[{CurrentProfileGeneric.Description}]: {csGetProfileData}", 1, reply);
                //        }
                //        OnAfterRead?.Invoke(client, obj, it, reply.Data, parameters, null);
                //    }
                //    catch (GXDLMSException ex)
                //    {
                //        obj.SetLastError(it, ex);
                //        if (ex.ErrorCode == (int)ErrorCode.ReadWriteDenied ||
                //                ex.ErrorCode == (int)ErrorCode.UndefinedObject ||
                //                ex.ErrorCode == (int)ErrorCode.UnavailableObject ||
                //                //Actaris returns access violation error.
                //                ex.ErrorCode == (int)ErrorCode.AccessViolated ||
                //                ex.ErrorCode == (int)ErrorCode.OtherReason)
                //        {
                //            //Some meters return OtherReason if Profile Generic buffer is try to read with selective access.
                //            if (!(obj is GXDLMSProfileGeneric && it == 2 && ex.ErrorCode == (int)ErrorCode.OtherReason))
                //            {
                //                obj.SetAccess(it, AccessMode.NoAccess);
                //            }
                //            OnAfterRead?.Invoke(null, obj, it, null, null, ex);
                //            continue;
                //        }
                //        else
                //        {
                //            OnAfterRead?.Invoke(null, obj, it, null, null, ex);
                //            throw ex;
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        OnAfterRead?.Invoke(null, obj, it, null, null, ex);
                //        obj.SetLastError(it, ex);
                //        throw ex;
                //    }
                //    finally
                //    {
                //        client.OnPdu -= p;
                //        OnDataReceived -= new GXDLMSCommunicator.DataReceivedEventHandler(OnProfileGenericDataReceived);
                //    }
                //    continue;
                //}
                else
                {
                    obj.ClearStatus(it);
                    reply.Clear();

                    OnBeforeRead?.Invoke(client, obj, it, null, null, null);
                    byte[] data = client.Read(obj.Name, obj.ObjectType, it)[0];
                    try
                    {
                        ReadDataBlock(data, $"Получение объекта {obj.ObjectType}:{it}", reply);
                    }
                    catch (GXDLMSException ex)
                    {
                        obj.SetLastError(it, ex);
                        if (ex.ErrorCode == (int)ErrorCode.ReadWriteDenied ||
                                ex.ErrorCode == (int)ErrorCode.UndefinedObject ||
                                ex.ErrorCode == (int)ErrorCode.UnavailableObject ||
                                //Actaris returns access violation error.
                                ex.ErrorCode == (int)ErrorCode.AccessViolated ||
                                ex.ErrorCode == (int)ErrorCode.OtherReason ||
                                ex.ErrorCode == (int)ErrorCode.InconsistentClass)
                        {
                            obj.SetAccess(it, AccessMode.NoAccess);
                            OnAfterRead?.Invoke(client, obj, it, null, null, ex);
                            continue;
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                    catch (Exception ex)
                    {
                        obj.SetLastError(it, ex);
                        throw ex;
                    }
                    if (obj is IGXDLMSBase)
                    {
                        object value = reply.Value;
                        DataType type;
                        if (value is byte[] byteArray && (type = obj.GetUIDataType(it)) != DataType.None)
                        {
                            value = GXDLMSClient.ChangeType(byteArray, type, client.UseUtc2NormalTime);
                        }
                        if (reply.DataType != DataType.None && obj.GetDataType(it) == DataType.None)
                        {
                            obj.SetDataType(it, reply.DataType);
                        }
                        client.UpdateValue(obj, it, value);
                    }
                    OnAfterRead?.Invoke(client, obj, it, reply.Value, null, null);
                    obj.SetLastReadTime(it, DateTime.Now);
                }
            }
        }

        public bool Write(GXDLMSObject obj, int index)
        {
            bool retCode = false;
            GXReplyData reply = new GXReplyData();
            for (int it = 1; it != (obj as IGXDLMSBase).GetAttributeCount() + 1; ++it)
            {
                reply.Clear();
                if (it == index || (index == 0 && obj.GetDirty(it, out object val)))
                {
                    bool forced = false;
                    GXDLMSAttributeSettings att = obj.Attributes.Find(it);
                    //Read DLMS data type if not known.
                    DataType type = obj.GetDataType(it);
                    if (type == DataType.None)
                    {
                        byte[] data = client.Read(obj, it)[0];
                        ReadDataBlock(data, "Получение типа объекта " + obj.ObjectType, reply);
                        type = reply.DataType;
                        if (type != DataType.None)
                        {
                            obj.SetDataType(it, type);
                        }
                        reply.Clear();
                    }
                    try
                    {
                        if (att != null && att.ForceToBlocks)
                        {
                            forced = client.ForceToBlocks = true;
                        }
                        try
                        {
                            ReadDataBlock(client.Write(obj, it), string.Format(csWriteObjectMsg, GetObjectDescription(obj), obj.ObjectType, index), reply);
                            ValueEventArgs e1 = new ValueEventArgs(obj, it, 0, null);
                            string xml = GXDLMSTranslator.ValueToXml(((IGXDLMSBase)obj).GetValue(client.Settings, e1));
                            OnAfterWrite?.Invoke(client, obj, it, xml, null);
                        }
                        catch (GXDLMSException ex)
                        {
                            OnError?.Invoke(obj, it, null, ex);
                            OnAfterWrite?.Invoke(client, obj, it, null, ex);
                            throw ex;
                        }
                        //Read data once again to make sure it is updated.
                        reply.Clear();
                        byte[] data = client.Read(obj, it)[0];
                        string taskDesc = string.Format(csReadingObjectMsg, GetObjectDescription(obj), obj.ObjectType, index, (obj as IGXDLMSBase).GetNames()[index - 1]);
                        ReadDataBlock(data, taskDesc, reply);
                        val = reply.Value;
                        if (val is byte[] byteArray && (type = obj.GetUIDataType(it)) != DataType.None)
                        {
                            val = GXDLMSClient.ChangeType(byteArray, type, client.UseUtc2NormalTime);
                        }
                        client.UpdateValue(obj, it, val);
                        retCode = true;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (forced)
                        {
                            client.ForceToBlocks = false;
                        }
                    }
                }
            }
            return retCode;
        }

        /// <summary>
        /// Чтение списка атрибутов.
        /// </summary>
        public void ReadList(List<KeyValuePair<GXDLMSObject, int>> list)
        {
            try
            {
                byte[][] data = client.ReadList(list);
                GXReplyData reply = new GXReplyData();
                List<object> values = new List<object>();
                bool failed = false;
                foreach (byte[] it in data)
                    try
                    {
                        ReadDataBlock(it, "", 1, 1, reply);

                        // Value is null if data is send in multiple frames.
                        if (reply.Value is IEnumerable<object> enumerable)
                        {
                            values.AddRange(enumerable);
                        }
                        reply.Clear();
                    }
                    catch (Exception ex)
                    {
                        reply.Clear();
                        failed = true;
                        GXLogWriter.WriteLog(ex.Message, LogLevel.Error);
                    }

                string err = String.Empty;
                if (failed)
                {
                    err = err + csIllegalReadListMsg;
                }
                if (values.Count != list.Count)
                {
                    err = err + csIllegalReplyMsg;
                }
                if (!String.IsNullOrEmpty(err))
                    throw new Exception(err);

                client.UpdateValues(list, values);
            }
            catch (Exception ex)
            {
                string err = $"Операция отменена: {ex.Message}";
                throw new Exception(err);
            }
        }

        public object ReadValue(GXDLMSObject obj, int attributeOrdinal)
        {
            if (obj == null)
                return null;
            GXReplyData reply = new GXReplyData();
            string[] attrNames = (obj as IGXDLMSBase)?.GetNames();
            string str = string.Format(csReadingObjectMsg,
                GetObjectDescription(obj),
                obj.ObjectType,
                attributeOrdinal,
                attrNames != null && attrNames.Length >= attributeOrdinal
                   ? attrNames[attributeOrdinal - 1]
                   : String.Empty);
            ReadDataBlock(client.Read(obj, attributeOrdinal), str, reply);
            try
            {
                // тип данных атрибута неизвестен или не совпадает с полученным
                DataType type = obj.GetDataType(attributeOrdinal);
                if (type == DataType.None || type != reply.DataType)
                {
                    // обновить тип данных
                    obj.SetDataType(attributeOrdinal, reply.DataType);
                }
                client.UpdateValue(obj, attributeOrdinal, reply.Value);
                obj.SetLastReadTime(attributeOrdinal, DateTime.Now);
            }
            catch (Exception ex)
            {
                // пропускаем ошибки чтения/обновления значения
                GXLogWriter.WriteLog($"Ошибка чтения [{obj.LogicalName}]:{attributeOrdinal}: {ex.Message}", LogLevel.Error);
            }
            return reply.Value;
        }

        public void GetProfileGenericColumns(GXDLMSProfileGeneric item)
        {
            GXReplyData reply = new GXReplyData();
            ReadDataBlock(Read(item, 3), $"Получение списка объектов захвата профиля {item.Description}", reply);
            client.UpdateValue(item, 3, reply.Value);
        }

        public void KeepAlive()
        {
            GXReplyData reply = new GXReplyData();
            byte[] data = client.GetKeepAlive();
            ReadDataBlock(data, "Отправка запроса поддержки подключения", reply);
        }
    }
}
