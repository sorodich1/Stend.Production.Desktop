using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Ankom.Params
{
    /// <summary>
    /// Перечень параметров опрашиваемого устройства для организации его опроса с использованием Политариф-А
    /// </summary>
    public class BaseDevice
    {
        /// <summary>
        /// Счетчик числа выполненных в ходе опроса команд
        /// </summary>
        public int requestID = 0;

        /// <summary>
        /// Дескриптор (handle) задачи мониторинга. Если равен 0, выполняется обычный опрос
        /// </summary>
        public int taskID = 0;

        /// <summary>
        /// Уровень доступа к командам счетчика
        /// </summary>
        public AccessLevels accessLevel = AccessLevels.UnProtected;

        /// <summary>
        /// Путь к файлу с параметрами драйвера и самого опрашиваемого счетчика, 
        /// а также дополнительными данными для параметризации счетчика: часы, реле управления нагрузкой
        /// </summary>
        public String requestFile;

        /// <summary>
        /// Kод производителя счетчика, например: EGM для счетчиков GAMMA
        /// </summary>
        public String manufacturerID;

        /// <summary>
        /// Cерийный номер счетчика, например: 1387034
        /// </summary>
        public int serialNumber;

        /// <summary>
        /// Cетевой номер счетчика. Для счетчиков GAMMA вычисляется на базе его серийного номера, ЦЭ27XX - не нужен?
        /// </summary>
        public int netAdr;

        /// <summary>
        /// Пароль счетчика: по умолчанию пустой для чтения счетчиков и требуется при установке параметров
        /// </summary>
        public UInt32 password;

        /// <summary>
        /// Порт канала обмена
        /// </summary>
        public UInt16 com_port = 0;

        private string _channelAddr;

        /// <summary>
        /// Адрес канала обмена. В зависимости от типа канала может быть адресом конечной точки сервиса обмена, 
        /// IP-адресом при обмене по TCP-IP или GPRS, номером CSD-телефона, или вовсе отсутствовать 
        /// </summary>
        public string ChannelAddr
        {
            get
            {
                return _channelAddr;
            }
            set
            {
                _channelAddr = value;
            }
        }

        public bool valid;

        public String[] modification = { "" };
        public bool is3phase = false;

        /// <summary>
        /// Путь к файлу с параметрами опроса счетчика, определяющими перечень считываемых даннных
        /// </summary>
        public String name_tp;

        /// <summary>
        /// Путь к файлу протокола опроса счетчика
        /// </summary>
        public string protocol_log;

        /// <summary>
        /// Путь к файлу протокола отладочной информации - дополнительно к данным общего лога 
        /// содержит детальную информацию о процессе обмена на уровне команд (запросы и ответы)
        /// </summary>
        public string debug_log;

        /// <summary>
        /// Признак необходимости установки (корректировки) даты и времени
        /// </summary>
        public bool timeset;

        /// <summary>
        /// Признак необходимости отключения (true)/включения(false) нагрузки. 
        /// значение по умолчанию null, если соответствующий параметр опроса не задан 
        /// </summary>
        public bool? relayOff = null;

        /// <summary>
        /// Признак необходимости включения(true)/отключения(false) признака управления реле по  
        /// превышению лимита мощности. Значение по умолчанию null, если соответствующий параметр 
        /// опроса не задан и используется значение прошитое в счетчике
        /// </summary>
        public bool? relayPowerLimitOn = null;

        /// <summary>
        /// Значение лимита мощности (Ватт). 
        /// Значение по умолчанию null, если соответствующее значение в параметрах опроса 
        /// не задано и используется значение, прошитое в счетчике  
        /// </summary>
        public int? relayPowerLimitValue = null;

        internal DeviceRequest requestParams = null;

        internal bool
            readNetQuality = false,
            readMeterLogs = false;

        public BaseDevice(String requestFileName, int meter)
        {
            valid = false;

            if (!File.Exists(requestFileName))
                return;

            requestFile = requestFileName;
            StringBuilder tmp = new StringBuilder(256, 256);

            com_port = (UInt16)CIniFile.GetPrivateProfileInt("MAIN_SECTION", "COM", 0, requestFile);

            if (com_port > 1000) // подключение TCP/IP 
            {
                CIniFile.GetPrivateProfileString("MAIN_SECTION", "Ip", "127.0.0.1", tmp, 256, requestFile);
                _channelAddr = tmp.ToString();
                tmp.Length = 0;
            }
            else
                _channelAddr = "";

            // признак работы задачи мониторинга состояния счетчиков
            taskID = CIniFile.GetPrivateProfileInt("MAIN_SECTION", "TaskID", 0, requestFile);

            String meter_section = "Counter_" + meter.ToString();
            serialNumber = CIniFile.GetPrivateProfileInt(meter_section, "NCount", 0, requestFile);
            if (serialNumber == 0)
                return;

            netAdr = CIniFile.GetPrivateProfileInt(meter_section, "NetAdr", 0, requestFile);
            if (netAdr == 0)
                netAdr = serialNumber;

            // признак чтения параметров качества сети
            int param = CIniFile.GetPrivateProfileInt(meter_section, "NetQuality", 0, requestFile);
            readNetQuality = param != 0;

            // признак чтения журналов счетчика
            param = CIniFile.GetPrivateProfileInt(meter_section, "MeterLogs", 0, requestFile);
            readMeterLogs = param != 0;

            manufacturerID = tmp.ToString();
            tmp.Length = 0;

            password = (UInt32)CIniFile.GetPrivateProfileInt(meter_section, "Passw", 0, requestFile);
            if (password != 0)
                accessLevel = AccessLevels.Protected;

            tmp.Length = 0;
            CIniFile.GetPrivateProfileString(meter_section, "Name_TP", "", tmp, 256, requestFile);
            name_tp = tmp.ToString();


            CIniFile.GetPrivateProfileString("MAIN_SECTION", "Protocol", @".\base\FlogModem.txt", tmp, 256, requestFile);
            protocol_log = tmp.ToString();

            // полный (отладочный) протокол работы счетчика
            CIniFile.GetPrivateProfileString("MAIN_SECTION", "LogMTr", "", tmp, 256, requestFile);
            debug_log = tmp.ToString();

            // установка признака "ручного" (по команде с сервера) переключения реле
            int relayValue = CIniFile.GetPrivateProfileInt("RELE", "TurnOn", -1, requestFile);
            if (relayValue >= 0)
                // в опросе задан признак управления реле, иначе оставляем null
                relayOff = (relayValue == 3) ? true : false;

            // параметризация управления реле по лимиту мощности
            int relayLimitOnEnabled = (int)CIniFile.GetPrivateProfileInt("Action", "Type", -1, requestFile);
            // если в опросе задан признак управления реле по лимиту мощности, устанавливаем сам признак и значение лимита
            if (relayLimitOnEnabled >= 0)
            {
                relayPowerLimitOn = (relayLimitOnEnabled == 0) ? false : true;
                if (!relayPowerLimitOn.Value)
                    // установлено только отключение управления реле по лимиту - само значение лимита оставляем прежним
                    relayPowerLimitValue = -1;
                else
                    relayPowerLimitValue = (int)CIniFile.GetPrivateProfileInt("Action", "Value", 0, requestFile);
            }

            // указание на необходимость синхронизации времени счетчика с сервером
            timeset = (CIniFile.GetPrivateProfileInt(meter_section, "TimeSet", 0, requestFile) > 0);

            if (
                serialNumber != 0 && name_tp.Length != 0
                )
                valid = true;
            else
                valid = false;

        }

        public bool HasRele()
        {
            if ((modification == null) || (modification.Length == 0))
                return false;
            else
            {
                return false;
            }
        }

        internal static EnergyType SetPowerType(String str, byte[] pt)
        {
            EnergyType et = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == 'A')
                {
                    pt[0] = 1; et |= EnergyType.ActivePlus;
                }
                if (str[i] == 'B')
                {
                    pt[1] = 1; et |= EnergyType.ActiveMinus;
                }
                if (str[i] == 'C')
                {
                    pt[2] = 1; et |= EnergyType.ReactivePlus;
                }
                if (str[i] == 'D')
                {
                    pt[3] = 1; et |= EnergyType.ReactiveMinus;
                }
            }
            return et;
        }
    }
}
