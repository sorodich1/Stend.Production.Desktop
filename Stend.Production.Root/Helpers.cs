using Gurux.DLMS;
using Gurux.DLMS.Enums;
using Gurux.DLMS.ManufacturerSettings;
using Gurux.DLMS.Objects;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Serialization;
using ZIP.DLMS;

namespace Stend.Production.Root
{
    public class Helpers
    {
        public static string dataDir = $"{Application.StartupPath}\\Data\\DefaultFile\\";
        public static string deviceFileName = dataDir + "DefaultDevice.zds";
        public static string stendSettingFileName = dataDir + "PortSetting.json";
        public static string scriptFileName = dataDir + "ScriptDefault.json";
        public static string AssocExt = dataDir + "Association.xml";
        public static string oldSettingDir = dataDir + "OldSetting.json";


        public static GXManufacturerCollection GXManufacturers = new GXManufacturerCollection();
        public static Logger Log = LogManager.GetLogger("Configurator");

        public static void LoadPlugins(IPluginHost main, string pluginName)
        {
            RootScript._plugins = new List<PluginBase>();
            Log.Info("Загрузка действующих плагинов");
            string[] pluginFields = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory), "*.dll");
            foreach (string pluginsPatch in pluginFields)
            {
                Type typePlugin = null;
                try
                {
                    Assembly assembly = Assembly.LoadFrom(pluginsPatch);
                    if (assembly != null)
                    {
                        typePlugin = assembly.GetType(Path.GetFileNameWithoutExtension(pluginsPatch) + $"{pluginName}");
                    }
                }
                catch
                {
                    continue;
                }
                try
                {
                    if (typePlugin != null)
                    {
                        foreach (var plaginType in typePlugin.Assembly.DefinedTypes.Where(type => type.IsSubclassOf(typeof(PluginBase))))
                        {
                            var plugin = (PluginBase)Activator.CreateInstance(plaginType);
                            if(main != null)
                            {
                                plugin.Host = main;
                            }

                            (plugin as PluginBase)?.AddLog(plugin);
                            RootScript._plugins.Add(plugin);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Плагин не создан");
                    continue;
                }
            }
        }


        public static List<PluginBase> LoadPlug(string pluginName)
        {
            List<PluginBase> PluginBase = new List<PluginBase>();

            string[] pluginFields = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory), "*.dll");

            foreach (string pluginsPatch in pluginFields)
            {
                Type typePlugin = null;
                try
                {
                    Assembly assembly = Assembly.LoadFrom(pluginsPatch);
                    if (assembly != null)
                    {
                        typePlugin = assembly.GetType(Path.GetFileNameWithoutExtension(pluginsPatch) + $"{pluginName}");
                    }
                }
                catch
                {
                    continue;
                }
                try
                {
                    if (typePlugin != null)
                    {
                        foreach (var plaginType in typePlugin.Assembly.DefinedTypes.Where(type => type.IsSubclassOf(typeof(PluginBase))))
                        {
                            var plugin = (PluginBase)Activator.CreateInstance(plaginType);

                            (plugin as PluginBase)?.AddLog(plugin);
                            PluginBase.Add(plugin);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Плагин не создан");
                    continue;
                }
            }
            return PluginBase;
        }

        public static void FileCalendarLoad(GXDLMSObject obj, GXDLMSDevice dev, int[] attribute, ZIPDLMSCalendar calendar)
        {
            try
            {
                if (dev == null || obj == null)
                {
                    throw new Exception("Не определены настройки устройств");
                }

                var activityCalendar = obj as GXDLMSActivityCalendar;

                calendar.ToDlmsPassive(activityCalendar);


                foreach (var index in attribute)
                {
                    WriteAttributeValue(dev, activityCalendar, index, null);
                }
                dev.Comm.ActivatePassiveCalendar(activityCalendar);
            }
            catch (Exception ex)
            {

            }
        }

        public static void CaptureObjectWrite(GXDLMSDevice device, List<CaptureGridObject> fileValue, GXDLMSObject obj, int attr)
        {
            BindingList<CaptureGridObject> _captObjects = new BindingList<CaptureGridObject>();

            List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> CaptureObjects = new List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>();

            var value = new List<object>();

            int _dataIndex = 0;

            foreach (var item in fileValue)
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

            client.UpdateValue(obj, attr, value);

            device.Comm.Write(obj, attr);
        }

        private static byte[] LogicalNameToButes(string logicalName)
        {
            try
            {
                string[] codes = logicalName.Split('.');
                List<byte> obis = new List<byte>();
                foreach (var item in codes)
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

        public static RootScript LoadProductionScript(string scriptSetting)
        {
            return JsonConvert.DeserializeObject<RootScript>(scriptSetting, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });
        }

        public static object ReadAttributeValue(GXDLMSDevice device, GXDLMSObject deviceObject, int inx)
        {
            if (device == null || deviceObject == null)
            {
                throw new Exception("Недопустимые параметры процедуры записи");
            }
            if (!device.ReadyToExchange)
            {
                throw new Exception("Отсутствует подключение к ПУ");
            }
            if (deviceObject.GetAccess(inx) == AccessMode.NoAccess || !deviceObject.CanRead(inx))
            {
                throw new Exception("Отсутствует доступ на чтение");
            }

            try
            {
                GXDLMSClient client = device.Comm.client;
                return device.Comm.ReadValue(deviceObject, inx);
            }
            catch(Exception ex)
            {
                throw new Exception(string.Format("[{0}] - ошибка чтения атрибута {1}: {2}", deviceObject.Description, inx, ex.Message));
            }
        }

        public static void ReadAllAttributes(GXDLMSObject deviceObject, GXDLMSDevice dev)
        {
            if (dev == null || deviceObject == null)
            {
                throw new Exception("Недопустимые параметры процедуры записи");
            }
            if(!dev.ReadyToExchange)
            {
                throw new Exception("Отсутствует подключение к ПУ");
            }
            try
            {
                foreach (int num in ((IEnumerable<int>)(deviceObject as IGXDLMSBase).GetAttributeIndexToRead(false)).ToList<int>())
                {
                    if (deviceObject.GetAccess(num) != AccessMode.NoAccess && deviceObject.CanRead(num))
                    {
                        dev.Comm.ReadValue(deviceObject, num);
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Не удалось выполнить чтение " + ex.Message);
            }
        }

        //Доделать
        public static void WriteAttributeValue(GXDLMSDevice device, GXDLMSObject obj, int index, object value)
        {
            if(device == null || obj == null)
            {
                throw new Exception("Недопустимые параметры процедуры записи");
            }
            if(!device.ReadyToExchange)
            {
                throw new Exception("Отсутствует подключение к ПУ");
            }
            if(obj.CanWrite(index))
            {
                throw new Exception("Отсутствует доступ на запись");
            }

            try
            {

            }
            catch(Exception ex)
            {
                if(obj.LogicalName == "0.0.96.172.195.255")
                {
                    Log.Info("Выход из режима производства для устройства");
                }
                else
                {
                    throw new Exception(string.Format("[{0}] - ошибка записи атрибута {1}: {2}", obj.Description, index, ex.Message));
                }
            }
        }

        public static void LoadDeviceAssociations(GXDLMSMeter meter, string path, XmlSerializer x = null, string DeviceName = null)
        {
            if (!(meter is GXDLMSDevice gxdlmsDevice))
            {
                return;
            }
            try
            {
                if (x == null)
                {
                    x = new XmlSerializer(typeof(GXDLMSObjectCollection), new List<Type>((IEnumerable<Type>)GXDLMSClient.GetObjectTypes())
                    {
                        typeof (GXDLMSAttributeSettings), typeof (GXDLMSAttribute)
                    }.ToArray());
                }
                gxdlmsDevice.Comm.client.Objects.Clear();
                gxdlmsDevice.Objects.Clear();
                using (Stream stream = (Stream)File.Open(path, FileMode.Open))
                {
                    if (string.IsNullOrWhiteSpace(DeviceName))
                    {
                        DeviceName = gxdlmsDevice.Name;
                    }
                    GXLogWriter.WriteLog("[" + DeviceName + "]: Загрузка файла ассоциаций счетчика " + path);
                    gxdlmsDevice.Objects = x.Deserialize(stream) as GXDLMSObjectCollection;
                    gxdlmsDevice.Comm.client.Objects.AddRange((IEnumerable<GXDLMSObject>)gxdlmsDevice.Objects);
                    stream.Close();
                }
            }
            catch(Exception ex)
            {
                if (File.Exists(path))
                    File.Delete(path);
                throw ex;
            }
        }
    }
}
