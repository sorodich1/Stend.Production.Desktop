using Ankom.Common;
using Gurux.DLMS;
using Gurux.DLMS.Enums;
using Gurux.DLMS.ManufacturerSettings;
using Gurux.DLMS.Objects;
using NLog;
using Production.Script.Calendar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using ZIP.DLMS;

namespace Production.Script
{
    public class Helpers
    {
        public static string Caption = "Выпуск счетчиков Вектор.СПОДЭС";
        public static string baseDir = null;
        internal static bool UseLocalMeterTime = false;
        internal static bool UseTimeStatusInfo = false;
        public static string helperFileName = helpDir + "Stend.docx";
        public static string stendSettingFileName = dataDir + "PortSetting.json";
        public static string deviceFileName = dataDir + "DefaultDevice.zds";
        public static string arbitFileName = dataDir + "1.arbit";
        public static string scriptFileName = dataDir + "TestFullScript.json";
        public static string AssocExt = dataDir + "1.xml";
        public static string oldSettingDir = dataDir + "OldSettings.json";
        public static string defaultScriptFileName = dataDir + "FileLoad";
        public static string calendarDir = "Calendar.json";
        public static string specialDaysDir = "specialDays.json";
        public static string connectionString = "Data Source=PROD_SERVER\\ANKOM;Initial Catalog=ZIPStendDevices;User ID = Stend;Password=Iklm73;Connect Timeout=30";
        public static Logger LogInfo = LogManager.GetCurrentClassLogger();
        public static Logger LogError = LogManager.GetLogger("ConfiguratorError");
        public static GXManufacturerCollection GXManufacturers = new GXManufacturerCollection();

        public static string fileName { get; set; }

        public static string dataDir => baseDir + "Data\\";

        public static string logDir => baseDir + "Logs\\";

        public static string helpDir => baseDir + "Help\\";

        public static string deviceAssocExt => "xml";

        public static string deviceSettingsExt => "zds";

        public static string stendSettingExt => "stend";

        public static string scriptExt => "script";

        public static DialogResult ShowMessage(string Message, bool isError = false) => MessageBox.Show(Message, Helpers.Caption, MessageBoxButtons.OK, isError ? MessageBoxIcon.Hand : MessageBoxIcon.Asterisk);

        public static DialogResult PromptMessage(string Prompt) => MessageBox.Show(Prompt, Helpers.Caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

        public static void LoadDeviceAssociations(GXDLMSMeter meter, string path, XmlSerializer x = null, string DeviceName = null)
        {
            if (!(meter is GXDLMSDevice gxdlmsDevice))
                return;
            try
            {
                if (x == null)
                    x = new XmlSerializer(typeof(GXDLMSObjectCollection), new List<Type>((IEnumerable<Type>)GXDLMSClient.GetObjectTypes())
          {
            typeof (GXDLMSAttributeSettings),
            typeof (GXDLMSAttribute)
          }.ToArray());
                gxdlmsDevice.Comm.client.Objects.Clear();
                gxdlmsDevice.Objects.Clear();
                using (Stream stream = (Stream)File.Open(path, FileMode.Open))
                {
                    if (string.IsNullOrWhiteSpace(DeviceName))
                        DeviceName = gxdlmsDevice.Name;
                    GXLogWriter.WriteLog("[" + DeviceName + "]: Загрузка файла ассоциаций счетчика " + path);
                    gxdlmsDevice.Objects = x.Deserialize(stream) as GXDLMSObjectCollection;
                    gxdlmsDevice.Comm.client.Objects.AddRange((IEnumerable<GXDLMSObject>)gxdlmsDevice.Objects);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                if (File.Exists(path))
                    File.Delete(path);
                throw ex;
            }
        }

        internal static void UpdateDB(PlaceSetting owner)
        {
            LogError.Error("Не происходит обнавление БД");
        }

        public static GXDateTime GetDLMSDateTime(DateTime initValue, bool isRealTime = true, bool skipDeviation = true)
        {
            DateTime dateTime1 = initValue;
            DateTime dateTime2 = new DateTime(dateTime1.Year, dateTime1.Month, dateTime1.Day,
                dateTime1.Hour, dateTime1.Minute, dateTime1.Second, 0);
            GXDateTime dlmsDateTime;
            if (isRealTime)
            {
                DateTime dateTime3 = dateTime2.AddSeconds(2.0);
                Thread.Sleep((int)((dateTime3 - dateTime1).TotalMilliseconds - 25.0));
                dlmsDateTime = new GXDateTime(dateTime3);
                dlmsDateTime.Skip &= ~DateTimeSkips.Ms;
            }
            else
            {
                dlmsDateTime = new GXDateTime(dateTime2.AddMilliseconds((double)dateTime1.Millisecond));
                dlmsDateTime.Skip &= ~DateTimeSkips.Ms;
            }
            return dlmsDateTime;
        }

        public static void ReadAllAttributes(GXDLMSObject deviceObject, GXDLMSDevice device)
        {
            if (device == null || deviceObject == null)
                throw new Exception("Недопустимые параметры процедуры записи");
            if (!device.ReadyToExchange)
                throw new Exception("Отсутствует подключение к ПУ");
            try
            {
                foreach (int num in ((IEnumerable<int>)(deviceObject as IGXDLMSBase).GetAttributeIndexToRead(false)).ToList<int>())
                {
                    if (deviceObject.GetAccess(num) != AccessMode.NoAccess && deviceObject.CanRead(num))
                        device.Comm.ReadValue(deviceObject, num);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось выполнить чтение " + ex.Message);
            }
        }

        public static object ReadAttributeValue(
          GXDLMSDevice device,
          GXDLMSObject deviceObject,
          int inx)
        {
            if (device == null || deviceObject == null)
                throw new Exception("Недопустимые параметры процедуры записи");
            if (!device.ReadyToExchange)
                throw new Exception("Отсутствует подключение к ПУ");
            if (deviceObject.GetAccess(inx) == AccessMode.NoAccess || !deviceObject.CanRead(inx))
                throw new Exception("Отсутствует доступ на чтение");
            try
            {
                GXDLMSClient client = device.Comm.client;
                return device.Comm.ReadValue(deviceObject, inx);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("[{0}] - ошибка чтения атрибута {1}: {2}", deviceObject.Description, inx, ex.Message));
            }
        }

        public static void WriteAttributeValue(
          GXDLMSDevice device,
          GXDLMSObject deviceObject,
          int inx,
          object value)
        {
            if (device == null || deviceObject == null)
                throw new Exception("Недопустимые параметры процедуры записи");

            if (!device.ReadyToExchange)
                throw new Exception("Отсутствует подключение к ПУ");

            if (!deviceObject.CanWrite(inx))
                throw new Exception("Отсутствует доступ на запись");

            try
            {
                GXDLMSClient client = device.Comm.client;
                if (value != null && !(value is DBNull))
                {



                    deviceObject.GetDataType(inx);



                    if (deviceObject.LogicalName == "0.0.96.9.0.255")
                    {
                        int? nullable = Convert.ChangeType(value, typeof(int)) as int?;
                        value = nullable.HasValue ? new int?(nullable.GetValueOrDefault() * 10) : new int?();
                    }
                    if (deviceObject.LogicalName == "0.0.96.1.0.255")
                    {
                        int? nullable = Convert.ChangeType(value, typeof(int)) as int?;
                        value = nullable;
                    }
                    if (deviceObject.LogicalName == "0.0.96.172.195.255")
                        value = GXDLMSConverter.ChangeType(value, DataType.UInt8, CultureInfo.CurrentCulture);
                    client.UpdateValue(deviceObject, inx, value);
                }
                else if (deviceObject.GetValues()[inx - 1] == null)
                    throw new Exception("невозможно установить значение в null");

                if(deviceObject.LogicalName != "0.0.1.0.0.255" && deviceObject.LogicalName != "0.0.96.172.195.255")
                    Thread.Sleep(1000);

                device.Comm.Write(deviceObject, inx);
            }
            catch (Exception ex)
            {
                if(deviceObject.LogicalName == "0.0.96.172.195.255")
                {
                    LogInfo.Info($"Выход из режима производства для устройства");
                }
                else
                {
                    throw new Exception(string.Format("[{0}] - ошибка записи атрибута {1}: {2}",
                        deviceObject.Description, inx, ex.Message));
                }
            }
        }

        //public static void WriteObjectMetod(GXDLMSObject obj, GXDLMSDevice dev, List<int> index)
        //{
        //    try
        //    {
        //        if (dev == null || obj == null)
        //            throw new Exception("Недопустимые параметры");
        //        GXDLMSClient client = (GXDLMSClient)dev.Comm.client;
        //        foreach (int index1 in index)
        //        {
        //            if (obj.GetAccess(index1) != AccessMode.NoAccess && obj.CanRead(index1))
        //                dev.Comm.Write(obj, index1);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Helpers.LogError.Error(ex.Message);
        //    }
        //}

        internal static object GetPropertyValue(object source, string properyName)
        {
            object propertyValue = (object)null;
            if (source != null)
            {
                MemberInfo memberInfo = ((IEnumerable<MemberInfo>)source.GetType().GetMembers()).FirstOrDefault<MemberInfo>((Func<MemberInfo, bool>)(item => item.MemberType == MemberTypes.Property && item.Name == properyName));
                if (memberInfo != (MemberInfo)null)
                {
                    try
                    {
                        PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
                        if (propertyInfo.GetIndexParameters().Length == 0 && propertyInfo.CanRead)
                            propertyValue = propertyInfo.GetValue(source);
                    }
                    catch
                    {
                        propertyValue = (object)null;
                    }
                }
            }
            return propertyValue;
        }

        public static void KeyComparison(List<object> index1, List<object> index2, StendSettings stend)
        {
            int int32_1 = Convert.ToInt32(index1[2]);
            int int32_2 = Convert.ToInt32(index2[2]);
            if (int32_1 != stend.FirmwareInfo.Controller1_Device1)
                throw new Exception(string.Format("Ошибка проверки кода контроллера 2 [meter(ui16)] --  код устройства = [{0}] -- эталонное значение =  [{1}]", (object)int32_1, (object)stend.FirmwareInfo.Controller1_Device1));
            if (int32_2 != stend.FirmwareInfo.Controller1_Device2)
                throw new Exception(string.Format("Ошибка проверки кода контроллера 2 [comm(ui16)] --  код устройства = [{0}] -- эталонное значение =  [{1}]", (object)int32_2, (object)stend.FirmwareInfo.Controller1_Device2));
        }

        public static bool CheckSerialDataBase(string serial)
        {
            bool flag = false;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand sqlCommand = new SqlCommand(string.Format("SELECT COUNT(*) FROM Devices WHERE Serial = '{0}'", serial), connection);
                connection.Open();
                flag = Convert.ToInt32(sqlCommand.ExecuteScalar()) == 0;
            }
            return flag;
        }

        public static void LoadDB(PlaceSetting placeSetting)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    string hexMasterKey = "0x" + HexUtils.BytesToHexStr(placeSetting.MasterKey);
                    CheckSerialDataBase(placeSetting.Serial);
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand();
                    sqlCommand.Connection = sqlConnection;
                    string date = DateTime.Now.ToString("yyyy-dd-MM HH:mm:ss");
                    sqlCommand.CommandText = string.Format("INSERT INTO Devices (Serial, MasterKey, DateTime, Id_CounterModifications, NumberBatch)" +
                        "VALUES (N'{0}' ,{1}, '{2}', {3}, {4})", placeSetting.Serial, hexMasterKey, date, placeSetting.StendSettings.FirmwareInfo.ID, placeSetting.StendSettings.NumberBatch);
                    sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                LogError.Error(ex.Message);
            }
        }

        /// <summary>
        /// Запись параметров счётчика из загруженного файла
        /// </summary>
        /// <param name="obj">Загружаемый объект</param>
        /// <param name="dev">Настройки устройства</param>
        /// <param name="atribute">Массив заданных атрибутов</param>
        /// <param name="calendar">Календарь, загруженный из файла</param>
        public static void FileCalendarLoad(GXDLMSObject obj, GXDLMSDevice dev, int[] atribute, ZIPDLMSCalendar calendar)
        {
            try
            {
                if(dev == null || obj == null)
                {
                    throw new Exception("Не определены настройки устройств");
                }

                var activityCalendar = obj as GXDLMSActivityCalendar;

                calendar.ToDlmsPassive(activityCalendar);
                

                foreach (var index in atribute)
                {
                    WriteAttributeValue(dev, activityCalendar, index, null);
                }
                dev.Comm.ActivatePassiveCalendar(activityCalendar);
            }
            catch(Exception ex)
            {

            }
        }

        public static void FileCaptureObjectLoad(GXDLMSObject obj, GXDLMSDevice dev, int[] atribute)
        {
            try
            {
                if (dev == null || obj == null)
                {
                    throw new Exception("Не определены настройки устройств");
                }

                var profileGeneric = obj as GXDLMSProfileGeneric;

                WriteAttributeValue(dev, profileGeneric, 3, null);

                foreach (var index in profileGeneric.CaptureObjects)
                {
                    //WriteAttributeValue(dev, activityCalendar, index, null);
                }
                //dev.Comm.ActivatePassiveCalendar(activityCalendar);
            }
            catch (Exception ex)
            {

            }
        }

        public static byte[] LogicalNameToButes(string value)
        {
            try
            {
                string[] codes = value.Split('.');
                List<byte> obis = new List<byte>();
                foreach(var item in codes)
                {
                    obis.Add(Convert.ToByte(item));
                }
                return obis.ToArray();
            }
            catch
            {
                return null;
            }
        }

        internal static void CaptureObjectWrite(GXDLMSDevice device, List<CaptureGridObject> autoscrollIndication, GXDLMSObject obj)
        {
            BindingList<CaptureGridObject> _captObjects = new BindingList<CaptureGridObject>();

           // GXDLMSObject obj = device.Objects.FindByLN(ObjectType.None, OBIS) ?? throw new Exception("Объект " + OBIS + " отсутствует в Ассоциации ПУ.");

            List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> CaptureObjects = new List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>();

            var value = new List<object>();

            int _dataIndex = 0;

            foreach (var item in autoscrollIndication)
            {
                if (item != null)
                {
                    if (obj != null)
                    {
                        GXDLMSObject gxObg = device.Objects.FindByLN(ObjectType.None, item.OBIS);

                        GXDLMSCaptureObject co = new GXDLMSCaptureObject(Convert.ToInt32(item.AttributeInx), ++_dataIndex);

                        CaptureGridObject cgo = new CaptureGridObject(new GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(gxObg, co));

                        _captObjects.Add(cgo);
                    }
                }
            }

            foreach (var item in _captObjects)
            {
                CaptureObjects.Add(item.Column);
            }

            foreach (var item in CaptureObjects)
            {
                value.Add(new List<object>()
                            {
                                (int)item.Key.ObjectType,
                                Helpers.LogicalNameToButes(item.Key.LogicalName),
                                item.Value.AttributeIndex,
                                item.Value.DataIndex
                            });
            }
            object valueObg = value;
            DataType _dataType = DataType.Array;
            try
            {
                valueObg = GXDLMSConverter.ChangeType(value, _dataType, CultureInfo.CurrentCulture);
            }
            catch { }

            GXDLMSClient client = device.Comm.client;

            client.UpdateValue(obj, 3, value);

            device.Comm.Write(obj, 3);
        }
    }
}
