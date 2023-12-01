using Gurux.DLMS;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Stend.Production.Root
{
    public class ZIPDLMSCalendar
    {
        private bool _changed;

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("seasons")]
        public BindingList<SeasonProfile> Seasons { get; set; }
        [JsonProperty("weeks")]
        public BindingList<WeekProfile> Weeks { get; set; }
        [JsonProperty("days")]
        public BindingList<DayProfile> Days { get; set; }
        [JsonIgnore]
        public bool Changed { get => _changed; }
        [JsonIgnore]
        public bool IsValid { get => CheckValid(); }

        public event EventHandler OnChanged;


        private void SetChanged()
        {
            _changed = true;
            OnChanged?.Invoke(this, new EventArgs());
        }

        private bool CheckValid()
        {
            return
                !string.IsNullOrEmpty(Name) &&
                (Days.Count > 0) && //(Days.First(item => !item.IsValid) == null) &&
                (Weeks.Count > 0) && //(Weeks.First(item => !item.IsValid) == null) &&
                (Seasons.Count > 0); //&& (Seasons.First(item => !item.IsValid) == null);
        }

        public ZIPDLMSCalendar()
        {
            Seasons = new BindingList<SeasonProfile>();
            Weeks = new BindingList<WeekProfile>();
            Days = new BindingList<DayProfile>();
            CheckChanging(true);
            _changed = false;
        }

        internal void CheckChanging(bool on = true)
        {
            if (on)
            {
                Seasons.ListChanged += ListData_Changed;
                Weeks.ListChanged += ListData_Changed;
                Days.ListChanged += ListData_Changed;
            }
            else
            {
                Seasons.ListChanged -= ListData_Changed;
                Weeks.ListChanged -= ListData_Changed;
                Days.ListChanged -= ListData_Changed;
                _changed = false;
            }
        }

        public void CopyTo(ZIPDLMSCalendar target)
        {
            target.Name = Name;

            target.Seasons.Clear();
            foreach (var item in Seasons)
                target.Seasons.Add(item);

            target.Weeks.Clear();
            foreach (var item in Weeks)
                target.Weeks.Add(item);

            target.Days.Clear();
            foreach (var item in Days)
                target.Days.Add(item);
        }

        public void ToDlmsActive(GXDLMSActivityCalendar target)
        {
            target.CalendarNameActive = Name;

            target.DayProfileTableActive = Days.Select((item) =>
            {
                return new GXDLMSDayProfile(
                    item.DayId,
                    new GXDLMSDayProfileAction[] { });
            }).ToArray();

            target.WeekProfileTableActive = Weeks.Select((item) =>
            {
                return new GXDLMSWeekProfile(item.Name,
                    item.Monday.Value, item.Tuesday.Value, item.Wednesday.Value,
                    item.Thursday.Value, item.Friday.Value, item.Saturday.Value, item.Sunday.Value);
            }).ToArray();

            target.SeasonProfileActive = Seasons.Select((item) =>
            {
                return new GXDLMSSeasonProfile(item.Name, item.SeasonStart, item.WeekName);
            }).ToArray();

        }

        public void ToDlmsPassive(GXDLMSActivityCalendar target)
        {
            target.CalendarNamePassive = Name;

            target.DayProfileTablePassive = ToDlmsDayProfie();
            target.WeekProfileTablePassive = ToDlmsWeekProfie();
            target.SeasonProfilePassive = ToDlmsSeasonProfie();
        }

        internal GXDLMSDayProfile[] ToDlmsDayProfie()
        {
            return Days.Select((item) =>
            {
                return new GXDLMSDayProfile(
                    item.DayId,
                    item.Shedule.Select((it) =>
                    { return new GXDLMSDayProfileAction(it.StartTime, it.ScriptLN, (ushort)it.Selector); }).ToArray());
            }).ToArray();
        }

        internal GXDLMSWeekProfile[] ToDlmsWeekProfie()
        {
            return Weeks.Select((item) =>
            {
                return new GXDLMSWeekProfile(item.Name,
                    item.Monday.Value, item.Tuesday.Value, item.Wednesday.Value,
                    item.Thursday.Value, item.Friday.Value, item.Saturday.Value, item.Sunday.Value);
            }).ToArray();
        }

        internal GXDLMSSeasonProfile[] ToDlmsSeasonProfie()
        {
            return Seasons.Select((item) =>
            {
                GXDateTime sStart = new GXDateTime(item.SeasonStart);
                sStart.Skip |= DateTimeSkips.Year;
                return new GXDLMSSeasonProfile(item.Name, item.SeasonStart, item.WeekName);
            }).ToArray();
        }


        public void AssignDays(GXDLMSDayProfile[] dayProfile)
        {
            try
            {
                Days.Clear();
                if (dayProfile != null)
                    foreach (var item in dayProfile)
                    {
                        DayProfile zipDayProfile = new DayProfile()
                        {
                            DayId = item.DayId
                        };

                        foreach (var shedule in item.DaySchedules)
                        {
                            //var value = shedule.StartTime.Value;
                            //GXTime shedValue =  value.Hour, value.Minute, value.Second, value.Millisecond);
                            zipDayProfile.Shedule.Add(new DaySchedule()
                            {
                                //StartTimeHexStr = shedValue.ToHex(false, false),
                                StartTime = shedule.StartTime,
                                ScriptLN = shedule.ScriptLogicalName,
                                Selector = shedule.ScriptSelector,
                            }); ;
                        }
                        Days.Add(zipDayProfile);
                    }
            }
            finally
            {
                //ActiveCalendar.Days.ListChanged += ActiveCalendar.Days.ListData_Changed;
            }
        }

        public static bool IsDayPresent(BindingList<DayProfile> profile, int dayID)
        {
            bool retcode = (profile.FirstOrDefault(item => item.DayId == dayID) != null);
            return retcode;
        }

        public void AssignWeeks(GXDLMSWeekProfile[] weekProfile)
        {
            Weeks.Clear();
            if (weekProfile != null)
                foreach (var item in weekProfile) //dlmsCalendar.WeekProfileTableActive)
                {
                    Weeks.Add(new WeekProfile()
                    {
                        Name = ASCIIEncoding.ASCII.GetString(item.Name),
                        Monday = IsDayPresent(Days, item.Monday) ? item.Monday : (int?)null,
                        Tuesday = IsDayPresent(Days, item.Tuesday) ? item.Tuesday : (int?)null,
                        Wednesday = IsDayPresent(Days, item.Wednesday) ? item.Wednesday : (int?)null,
                        Thursday = IsDayPresent(Days, item.Thursday) ? item.Thursday : (int?)null,
                        Friday = IsDayPresent(Days, item.Friday) ? item.Friday : (int?)null,
                        Saturday = IsDayPresent(Days, item.Saturday) ? item.Saturday : (int?)null,
                        Sunday = IsDayPresent(Days, item.Sunday) ? item.Sunday : (int?)null
                    });
                }
        }

        public void AssignSeasons(GXDLMSSeasonProfile[] seasonProfile)
        {
            Seasons.Clear();
            if (seasonProfile != null)
                foreach (var item in seasonProfile)
                {
                    string weekName = ASCIIEncoding.ASCII.GetString(item.WeekName);
                    Seasons.Add(new SeasonProfile()
                    {
                        Name = ASCIIEncoding.ASCII.GetString(item.Name),
                        SeasonStart = item.Start,
                        WeekName = (Weeks.FirstOrDefault(wp => wp.Name.ToLower() == weekName.ToLower()) != null) ? weekName : null
                    });
                }
        }

        private void ListData_Changed(object sender, ListChangedEventArgs e)
        {
            if (
                (e.ListChangedType == ListChangedType.ItemChanged) ||
                (e.ListChangedType == ListChangedType.ItemAdded) ||
                (e.ListChangedType == ListChangedType.ItemDeleted)
               )
            {
                SetChanged();
            }
        }
    }

    public class SeasonProfile : INotifyPropertyChanged
    {
        private string weekName;
        private string seasonStartHexStr;
        private string name;

        /// <summary>
        /// Наименование сезонного профиля календаря 
        /// </summary>
        [JsonProperty("season_name")]
        [Description("Имя")]
        public string Name
        {
            get => name;
            set
            {
                name = value; OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// Дата и время начала указанного сезона. Год при передаче в DLMS не учитывается
        /// </summary>
        [JsonIgnore]
        [Description("Начало сезона")]
        public GXDateTime SeasonStart
        {
            get { return GXDateTime.ParseHex(SeasonStartHexStr); }
            set
            {
                //value.Skip |= DateTimeSkips.Year;
                SeasonStartHexStr = value.ToHex(false, false);
            }
        }

        /// <summary>
        /// Дата и время начала указанного сезона в виде шестнадцатиричной строки. 
        /// </summary>
        [JsonProperty("season_start")]
        public string SeasonStartHexStr
        {
            get => seasonStartHexStr;
            set
            {
                seasonStartHexStr = value; OnPropertyChanged("SeasonStart");
            }
        }

        /// <summary>
        /// Наименование недельного профиля
        /// </summary>
        [JsonProperty("week_name")]
        [Description("Неделя")]
        public string WeekName
        {
            get => weekName;
            set
            {
                weekName = value; OnPropertyChanged("WeekName");
            }
        }
        [JsonIgnore]
        public bool IsValid { get => CheckValid(); }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private bool CheckValid()
        {
            return
                !string.IsNullOrEmpty(Name) &&
                !string.IsNullOrEmpty(WeekName);
        }
    }

    public class WeekProfile : INotifyPropertyChanged
    {
        private string name;
        private int? monday;
        private int? tuesday;
        private int? wednesday;
        private int? thursday;
        private int? friday;
        private int? saturday;
        private int? sunday;

        /// <summary>
        /// Имя активного или пассивного недельного календаря в зависимости от 
        /// вкладки(Активный недельный профиль/Пассивный недельный профиль). 
        /// Имя, указанное в данном поле, используется в сезонном календаре
        /// </summary>
        [JsonProperty("week_name")]
        [Description("Имя")]
        public string Name
        {
            get => name;
            set
            {
                name = value; OnPropertyChanged("Name");
            }
        }
        /// <summary>
        /// далее все свойства класса указывают идентификатор соответствующего дня недели  
        /// суточного профиля используемого календаря (активного или пассивного) 
        /// </summary>
        [JsonProperty("monday")]
        [Description("Понедельник")]
        public int? Monday
        {
            get => monday;
            set
            {
                monday = value; OnPropertyChanged("Monday");
            }
        }
        [JsonProperty("tuesday")]
        [Description("Вторник")]
        public int? Tuesday
        {
            get => tuesday;
            set
            {
                tuesday = value; OnPropertyChanged("Tuesday");
            }
        }
        [JsonProperty("wednesday")]
        [Description("Среда")]
        public int? Wednesday
        {
            get => wednesday;
            set
            {
                wednesday = value; OnPropertyChanged("Wednesday");
            }
        }
        [JsonProperty("thursday")]
        [Description("Четверг")]
        public int? Thursday
        {
            get => thursday;
            set
            {
                thursday = value; OnPropertyChanged("Thursday");
            }
        }
        [JsonProperty("friday")]
        [Description("Пятница")]
        public int? Friday
        {
            get => friday;
            set
            {
                friday = value; OnPropertyChanged("Friday");
            }
        }
        [JsonProperty("saturday")]
        [Description("Суббота")]
        public int? Saturday
        {
            get => saturday;
            set
            {
                saturday = value; OnPropertyChanged("Saturday");
            }
        }
        [JsonProperty("sunday")]
        [Description("Воскресенье")]
        public int? Sunday
        {
            get => sunday;
            set
            {
                sunday = value; OnPropertyChanged("Sunday");
            }
        }

        public bool IsValid(DayProfile[] days) { return CheckValid(days); }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private bool CheckValid(DayProfile[] days)
        {
            return
                !string.IsNullOrEmpty(Name) &&
                Monday.HasValue &&
                Tuesday.HasValue &&
                Wednesday.HasValue &&
                Thursday.HasValue &&
                Friday.HasValue &&
                Saturday.HasValue &&
                Sunday.HasValue;
        }
    }

    public class DayProfile : INotifyPropertyChanged
    {
        private BindingList<DaySchedule> shedule;
        private int dayId;

        //private bool _isValid;

        [JsonProperty("day_id")]
        public int DayId
        {
            get => dayId;
            set
            {
                dayId = value; OnPropertyChanged("DayId");
            }
        }

        [JsonProperty("day_schedule")]
        public BindingList<DaySchedule> Shedule
        {
            get => shedule;
            set => shedule = value;
        }

        [JsonIgnore]
        public bool IsValid { get => Shedule.Count > 0; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public DayProfile()
        {
            Shedule = new BindingList<DaySchedule>();
        }
    }

    public class DaySchedule : INotifyPropertyChanged
    {
        const string csTarificationScriptTableLN = "0.0.10.0.100.255";
        private string hexStr;
        private string scriptLN;
        private int selector;

        //[JsonProperty("script_logical_name")]
        [JsonIgnore]
        public GXTime StartTime
        {
            get { return GXDateTime.ParseHex(HexStr) as GXTime; }
            set
            {
                HexStr = value?.ToHex(false, false);
            }
        }

        [JsonProperty("start_time")]
        public string HexStr
        {
            get => hexStr;
            set
            {
                hexStr = value; OnPropertyChanged("StartTime");

            }
        }
        [JsonProperty("script_logical_name")]
        [DefaultValue(csTarificationScriptTableLN)]
        public string ScriptLN
        {
            get => scriptLN;
            set { scriptLN = value; OnPropertyChanged("ScriptLN"); }
        }

        [JsonProperty("script_selector")]
        public int Selector
        {
            get => selector;
            set { selector = value; OnPropertyChanged("Selector"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public DaySchedule()
        {
            HexStr = "000000FF";
            //StartTime = new GXTime(0,0,0,0);
            ScriptLN = csTarificationScriptTableLN;
            Selector = 1;
        }
    }

    public class ActionSchedule
    {
        [JsonIgnore]
        public GXDateTime StartTime
        {
            get; set;
        }

        [JsonIgnore]
        public GXDateTime StartDate
        {
            get; set;
        }

        public GXDateTime Join()
        {
            return GXDateTime.ParseHex((new GXDate(StartDate)).DLMSValue + (new GXTime(StartTime)).DLMSValue + "800000");
        }
    }
}
