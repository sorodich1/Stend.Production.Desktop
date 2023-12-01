using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace Ankom.Params
{
    /// <summary>
    /// Перечисление определяет допустимые уровни доступа к счетчику.
    /// 0x00 (Protected) – доступ к командам с паролем;
    /// 0x01 (UnProtected) – доступ к командам без пароля;
    /// При попытке обращения к команде с недостаточным уровнем доступа - ошибка!!!
    /// </summary>
    public enum AccessLevels : byte { Protected, UnProtected };
    [Flags]
    public enum EnergyType : byte
    {
        ActivePlus = 0x01,
        ActiveMinus = 0x02,
        ReactivePlus = 0x04,
        ReactiveMinus = 0x08,
        ActiveModule = 0x10,
        ReactiveModule = 0x20
    }

    static class CIniFile
    {
        [DllImport("kernel32.dll")]
        public extern static int GetPrivateProfileString(String AppName, String KeyName, String Default, StringBuilder ReturnedString, UInt32 Size, String FileName);
        [DllImport("kernel32.dll")]
        public extern static int GetPrivateProfileInt(String AppName, String KeyName, int Default, String FileName);
    }

    public delegate void WriteProgress(int value);
    public class CommandRunProgress
    {
        int value = 0;
        int max = 1;
        bool started = false;
        bool finished = true;
        static bool isprogress;
        WriteProgress _writeProgress = InternalWriteProgress;

        public CommandRunProgress(WriteProgress writeProgress = null)
        {
            Start();
            if (writeProgress != null)
                _writeProgress = writeProgress;
        }

        static void InternalWriteProgress(int openclose = 0)
        {
            if (openclose < 0)
            {
                System.Console.Write(DateTime.Now.ToLongTimeString() + " [");
                isprogress = true;
            }

            if (openclose == 0)
            {
                System.Console.Write("=");
            }

            if (openclose > 0)
            {
                System.Console.WriteLine("]");
                isprogress = false;

            }
        }
        virtual public void Finish()
        {
            if (!finished || isprogress)
            {
                _writeProgress?.Invoke(1);
                finished = true;
            }
            isprogress = false;
        }

        virtual public void Start()
        {

            Finish();

            value = 0;
            max = 1;
            finished = true;
            started = false;
        }

        virtual public void OnProgress(object sender, string description, int current, int maximium)
        {
            //            if (maximium < 10)
            //                return;
            if (maximium > max)
            {
                max = maximium;
            }
            if (current > max)
            {
                max = current;
            }

            if (!started)
            {
                _writeProgress?.Invoke(-1);
                started = true;
                finished = false;
            }

            if (current == 0) //start
            {
                value = 0;
            }

            while ((int)(current / (max / 20.0)) > value)
            {
                _writeProgress?.Invoke(value++);
            }

            if (current >= max) //finish
            {
                value = 0;
                Start();
            }
        }
    }

    public class LogKind
    {
        public UInt16 ID { get; set; }
        public String Description { get; set; }
        public String Name { get; set; }
        public Char DataType { get; set; }
        public String Unit { get; set; }

        public LogKind(UInt16 ID, String Description, String Name, Char DataType, String Unit)
        {
            this.ID = ID;
            this.Description = Description;
            this.Name = Name;
            this.DataType = DataType;
            this.Unit = Unit;
        }
    }

    /// <summary>
    /// Статический класс, определяющий список допустимых типов событий счетчика,
    /// сохраняемых в БД Политариф-А в ходе его опроса или в процессе мониторинга
    /// </summary>
    public static class LogKindList
    {
        static List<LogKind> _logsList = new List<LogKind>() {
            (new LogKind( 0, "Неопределенное событие", "undefined", 's', null)),
            (new LogKind( 1, "Tемпература", "temp", 'f', "°C")),
            (new LogKind( 2, "Частота сети", "net_freq", 'f', "Гц")),

            (new LogKind( 3, "Напряжение фазы A", "phaseA_U", 'f', "В")),
            (new LogKind( 4, "Напряжение фазы B", "phaseB_U", 'f', "В")),
            (new LogKind( 5, "Напряжение фазы C", "phaseC_U", 'f', "В")),

            (new LogKind( 6, "Активная мощность", "cur_power_activ", 'f', "Вт")),
            (new LogKind( 7, "Реактивная мощность", "cur_power_reactiv", 'f', "Вт")),

            (new LogKind( 8, "Отключение фазы A", "phaseA_onoff", 'd', null)),
            (new LogKind( 9, "Отключение фазы B", "phaseB_onoff", 'd', null)),
            (new LogKind(10, "Отключение фазы C", "phaseC_onoff", 'd', null)),

            (new LogKind(11, "Коррекция времени", "time_correction", 'd', null)),
            (new LogKind(12, "Изменение тарифного расписания", "tarif_shedule_correction", 'd', null)),
            (new LogKind(13, "Время сброса", "reset_data", 'd', null)),
            (new LogKind(14, "Время инициализации 1-ого массива мощности", "init_1_arr_power", 'd', null)),
            (new LogKind(15, "Отключение резервного питания", "reserve_supply_onoff", 'd', null)),
            (new LogKind(16, "Попытка несанкционированного доступа", "unautorized_access", 'd', null)),
            (new LogKind(17, "Перепрограммирование счетчика", "reprogram_counter", 'd', null)),

            (new LogKind(18, "Выход за мин. ПДЗ напряжения (фаза A)", "phaseA_min_limit_U", 'd', null)),
            (new LogKind(19, "Выход за мин. НДЗ напряжения (фаза A)", "phaseA_min_normal_U", 'd', null)),
            (new LogKind(20, "Выход за макс. HДЗ напряжения (фаза A)", "phaseA_max_normal_U", 'd', null)),
            (new LogKind(21, "Выход за макс. ПДЗ напряжения (фаза A)", "phaseA_max_limit_U", 'd', null)),

            (new LogKind(22, "Выход за мин. ПДЗ напряжения (фаза B)", "phaseB_min_limit_U", 'd', null)),
            (new LogKind(23, "Выход за мин. НДЗ напряжения (фаза B)", "phaseB_min_normal_U", 'd', null)),
            (new LogKind(24, "Выход за макс. HДЗ напряжения (фаза B)", "phaseB_max_normal_U", 'd', null)),
            (new LogKind(25, "Выход за макс. ПДЗ напряжения (фаза B)", "phaseB_max_limit_U", 'd', null)),

            (new LogKind(26, "Выход за мин. ПДЗ напряжения (фаза C)", "phaseC_min_limit_U", 'd', null)),
            (new LogKind(27, "Выход за мин. НДЗ напряжения (фаза C)", "phaseC_min_normal_U", 'd', null)),
            (new LogKind(28, "Выход за макс. HДЗ напряжения (фаза C)", "phaseC_max_normal_U", 'd', null)),
            (new LogKind(29, "Выход за макс. ПДЗ напряжения (фаза C)", "phaseC_max_limit_U", 'd', null)),

            (new LogKind(30, "Изменение состояния счетчика", "counter_state", 'd', null)),

            (new LogKind(31, "Отключение фазы A", "phaseA_off", 'd', null)),
            (new LogKind(32, "Отключение фазы B", "phaseB_off", 'd', null)),
            (new LogKind(33, "Отключение фазы C", "phaseC_off", 'd', null)),

            (new LogKind(34, "Идентификатор концентратора", "NetUPD", 'd', null)),

            (new LogKind(35, "Ток фазы A", "phase_A_I", 'f', "A")),
            (new LogKind(36, "Ток фазы B", "phase_B_I", 'f', "A")),
            (new LogKind(37, "Ток фазы C", "phase_C_I", 'f', "A")),

            (new LogKind(38, "cos(phi) фазы A", "cosphi_a", 'f', null)),
            (new LogKind(39, "cos(phi) фазы B", "cosphi_b", 'f', null)),
            (new LogKind(40, "cos(phi) фазы C", "cosphi_c", 'f', null)),
            (new LogKind(41, "cos(phi) общий", "cosphi", 'f', null)),

            (new LogKind(42, "Текущая активная мощность (фаза A)", "cur_power_activ_phaseA", 'f', "Вт")),
            (new LogKind(43, "Текущая активная мощность (фаза B)", "cur_power_activ_phaseB", 'f', "Вт")),
            (new LogKind(44, "Текущая активная мощность (фаза C)", "cur_power_activ_phaseC", 'f', "Вт")),
            (new LogKind(45, "Текущая активная мощность (суммарная)", "cur_power_activ_phase_sum", 'f', "Вт")),

            (new LogKind(46, "Текущая реактивная мощность (фаза A)", "cur_power_reactiv_phaseA", 'f', "Вар")),
            (new LogKind(47, "Текущая реактивная мощность (фаза B)", "cur_power_reactiv_phaseB", 'f', "Вар")),
            (new LogKind(48, "Текущая реактивная мощность (фаза C)", "cur_power_reactiv_phaseC", 'f', "Вар")),
            (new LogKind(49, "Текущая реактивная мощность (суммарная)", "cur_power_reactiv_phase_sum", 'f', "Вар")),

            (new LogKind(50, "Превышение макс. НДЗ частоты", "freq_max_normal", 'f', "Гц")),
            (new LogKind(51, "Превышение мин. НДЗ частоты", "freq_min_normal", 'f', "Гц")),
            (new LogKind(52, "Превышение макс. ПДЗ частоты", "freq_max_limit", 'f', "Гц")),
            (new LogKind(53, "Превышение мин. ПДЗ частоты", "freq_min_limit", 'f', "Гц")),

            (new LogKind(54, "Сообщения о самодиагностике сети", "self_diag", 's', null)),
            (new LogKind(55, "Управление нагрузкой сети", "load_control", 's', null)),
            (new LogKind(56, "Лимит мощности", "pow_limit", 'f', "Вт")),
            (new LogKind(57, "Состояние реле", "rele_state", 's', null)),
            (new LogKind(58, "Превышение установленного лимита мощности", "power_limit_excess", 'd', null)),
            (new LogKind(59, "Статус управления лимитом мощности", "limit_control", 'f', null)),
            (new LogKind(60, "Статус опроса счетчика", "request_status", 'f', null)),
            (new LogKind(61, "Действия оператора (параметризация)", "operator_action", 'f', null))
        };


        public static string GetLogKindName(LogKind logKind)
        {
            // идентификатор журнала Политариф 
            return logKind.Name;
        }
        public static List<LogKind> LogsList
        {
            get
            {
                return _logsList;
            }
        }
    }

    public class SEAll
    {
        public UInt16 tarif;
        public String file;
        public bool valid = false;

//        public double[,] values = new double[4, 4];

//        UInt16 _col;
    }

    public class SEDaily : SEAll
    {
        public UInt16 col { get; set; }
        public DateTime  startDate { 
            get { 
                return (col == 0) ? DateTime.MinValue : DateTime.Today.AddDays(-(col+1)); 
            } 
        }
    }

    public class SEMonthly : SEDaily
    {
        public new DateTime startDate
        {
            get
            {
                return (col == 0) ? DateTime.MinValue : DateTime.Today.AddMonths(-(col+1));
            }
        }
    }

    public class SPProf : SEDaily
    {
    }

    public class SLogBase
    {
        public LogProfile type { get; protected set; }
        public Int32 col;
        public String file;
        public bool valid = false;

        public SLogBase(LogProfile profile, int recCount = 0)
        {
            type = profile;
            col = (Int16)recCount;
        }

        public DateTime startDate
        {
            get
            {
                return (col == 0) ? DateTime.MinValue : DateTime.Today.AddMonths(-col);
            }
        }
    }

    public class SOn_Off : SLogBase
    {
        public SOn_Off(): base(LogProfile.PowerOnOff, 16)  
        {
        }
    }

    public enum EnergyProfile
    {
        /// <summary>
        /// профиль учета энергии - полный перечень фиксаций в профиле (по всем периодам учёта)  
        /// или данные регистра о текущих показаниях счетчика на момент запроса
        /// </summary>
        All,
        /// <summary>
        /// профиль учета энергии - суточные фиксации 
        /// </summary>
        Daily,
        /// <summary>
        /// профиль учета энергии - месячные фиксации в профиле учета или  
        /// данные регистра о показаниях счетчика за последний полный месяц
        /// </summary>
        Monthly,
        /// <summary>
        /// профиль нагрузки по периодам интегрирования 
        /// </summary>
        Power,
        /// <summary>
        /// профиль учета максимальной мощности 
        /// </summary>
        MaxPower
    }

    public enum LogProfile
    {
        Undefined,
        /// <summary>
        /// журнал реле нагрузки
        /// </summary>
        ReleOnOff,
        /// <summary>
        /// журнал коррекции часов 
        /// </summary>
        ClockCorrection,
        /// <summary>
        /// журнал параметрирования 
        /// </summary>
        Parameterization,
        /// <summary>
        /// журнал вскрытия кожуха 
        /// </summary>
        СaseOpening,
        /// <summary>
        /// журнал вскрытия клеммной колодки 
        /// </summary>
        TerminalOpening,
        /// <summary>
        /// журнал превышения мощности 
        /// </summary>
        PowerExcess,
        /// <summary>
        /// журнал превышения тока 
        /// </summary>
        CurrentExcess,
        /// <summary>
        /// журнал превышения напряжения 
        /// </summary>
        VoltageExcess,
        /// <summary>
        /// журнал понижения напряжения 
        /// </summary>
        VoltageLow,
        /// <summary>
        /// журнал пропадания питания 
        /// </summary>
        PowerOnOff,
    }

    /// <summary>
    /// Парметры опроса - перечень запрашиваемых данных счетчика (тип данных, глубина и т.п.) с использованием Политариф-А
    /// </summary>
    public class DeviceRequest
    {
        public byte[] power_type = new byte[4];

        public SEAll EAll;
        public SEDaily EDaily;
        public SEMonthly EMonthly;
        public SPProf PProf;
        public SOn_Off PowerOnOff;
        public SLogBase AllMeterLogs, 
            ReleOnOff, 
            Clock, 
            UnautorizedAccess, 
            PowerExcess, 
            CurrentExcess, 
            VoltageExcess, 
            VoltageLow;

        //[MAIN_SECTION]
        public String timeFile; 
        //[PC_File]
        public String meterLogsFile;

        public bool valid;

        public BaseDevice Meter { get; private set; }

        public DeviceRequest(BaseDevice device)
        {
            try
            {
                valid = false;

                Meter = device;
                device.requestParams = this;

                AllMeterLogs = new SLogBase(LogProfile.Undefined);
                Clock = UnautorizedAccess = PowerExcess = CurrentExcess = VoltageExcess = VoltageLow = ReleOnOff = AllMeterLogs;

                Clock = new SLogBase(LogProfile.ClockCorrection);
                UnautorizedAccess = new SLogBase(LogProfile.СaseOpening);
                PowerExcess = new SLogBase(LogProfile.PowerExcess);
                CurrentExcess = new SLogBase(LogProfile.CurrentExcess);
                VoltageExcess = new SLogBase(LogProfile.VoltageExcess);
                VoltageLow = new SLogBase(LogProfile.VoltageLow);
                
                StringBuilder tmp = new StringBuilder(256, 256);

                CIniFile.GetPrivateProfileString("E_ALL", "File", "", tmp, 256, device.name_tp);
                if (tmp.ToString() == "")
                {
                    EAll = null;
                }
                else
                {
                    EAll = new SEAll();
                    EAll.file = tmp.ToString();
                    CIniFile.GetPrivateProfileString("E_ALL", "PowerType", "", tmp, 256, device.name_tp);
                    BaseDevice.SetPowerType(tmp.ToString(), power_type);
                    EAll.tarif = (UInt16)CIniFile.GetPrivateProfileInt("E_ALL", "Tarif", 4, device.name_tp);
                }

                CIniFile.GetPrivateProfileString("E_DAILY", "File", "", tmp, 256, device.name_tp);
                if (tmp.ToString() == "")
                {
                    EDaily = null;
                }
                else
                {
                    EDaily = new SEDaily();
                    EDaily.file = tmp.ToString();
                    EDaily.col = (UInt16)CIniFile.GetPrivateProfileInt("E_DAILY", "Col", 0, device.name_tp);
                    EDaily.tarif = (UInt16)CIniFile.GetPrivateProfileInt("E_DAILY", "Tarif", 4, device.name_tp);
                }

                CIniFile.GetPrivateProfileString("E_MONTHLY", "File", "", tmp, 256, device.name_tp);
                if (tmp.ToString() == "")
                {
                    EMonthly = null;
                }
                else
                {
                    EMonthly = new SEMonthly();
                    EMonthly.file = tmp.ToString();
                    EMonthly.col = (UInt16)CIniFile.GetPrivateProfileInt("E_MONTHLY", "Col", 0, device.name_tp);
                    EMonthly.tarif = (UInt16)CIniFile.GetPrivateProfileInt("E_MONTHLY", "Tarif", 4, device.name_tp);
                }

                CIniFile.GetPrivateProfileString("P_profile1", "File", "", tmp, 256, device.name_tp);
                if (tmp.ToString() == "")
                {
                    PProf = null;
                }
                else
                {
                    PProf = new SPProf();
                    PProf.file = tmp.ToString();
                    PProf.col = (UInt16)CIniFile.GetPrivateProfileInt("P_profile1", "Col", 0, device.name_tp);
                }

                CIniFile.GetPrivateProfileString("ON_OFF", "File", "", tmp, 256, device.name_tp);
                if (tmp.ToString() == "")
                {
                    PowerOnOff = null;
                }
                else
                {
                    PowerOnOff = new SOn_Off();
                    PowerOnOff.file = tmp.ToString();
                    PowerOnOff.col = CIniFile.GetPrivateProfileInt("ON_OFF", "Col", 0, device.name_tp);
                }

                // корректировка выполняется при отличии времени счетчика и сервера более чем на 30 сек. или при явном указании в MeterParams (параметр timeset)
                CIniFile.GetPrivateProfileString("MAIN_SECTION", "TimeCntPc", @".\data_CE272XA\time_data.txt", tmp, 256, device.name_tp);
                timeFile = tmp.ToString();

                // логи счетчика: качество энергии, ... - читаем все!!!
                if (Meter.readNetQuality || Meter.readMeterLogs || (Meter.taskID != 0))
                {
                    CIniFile.GetPrivateProfileString("Param_Count", "File", "", tmp, 256, device.name_tp);
                    meterLogsFile = tmp.ToString();
                    AllMeterLogs.file = meterLogsFile;
                }
                else
                {
                    AllMeterLogs = ReleOnOff = Clock = UnautorizedAccess = PowerExcess = CurrentExcess = VoltageExcess = VoltageLow = null;
                }

                valid = true;
            }
            catch
            {
            }
        }

    }

}
