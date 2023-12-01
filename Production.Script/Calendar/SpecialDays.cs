﻿// Generated by Xamasoft JSON Class Generator
// http://www.xamasoft.com/json-class-generator

using Gurux.DLMS;
using Gurux.DLMS.Objects;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using ZIP.DLMS;

namespace Production.Script
{
    public class ZIPSpecialDays
    {
        GXDLMSSpecialDaysTable _specialDaysTable = new GXDLMSSpecialDaysTable();

        const string csSpecialDays = "0.0.11.0.0.255";
        //private bool _isChanged;
        BindingList<SpecialDay> _days = new BindingList<SpecialDay>();
        private bool _changed;

        public BindingList<SpecialDay> Days { get => _days; }
        public string LogicalName { get => _specialDaysTable.LogicalName; set => _specialDaysTable.LogicalName = value; }
        public GXDLMSSpecialDaysTable SpecialDaysTable { get => _specialDaysTable; }

        public ZIPSpecialDays(GXDLMSSpecialDaysTable table = null)
        {
            Assign(table?.Entries);
        }

        public void CopyTo(GXDLMSSpecialDaysTable daysTable)
        {
            if (daysTable != null)
                daysTable.Entries = _days.Select(item => new GXDLMSSpecialDay()
                {
                    Index = item.Index,
                    DayId = item.DayId,
                    Date = item.Date
                }).ToArray();
        }

        public void Assign(GXDLMSSpecialDaysTable daysTable)
        {
            if (daysTable != null)
                Assign(daysTable.Entries);
        }

        public void Assign(GXDLMSSpecialDay[] daysTable)
        {
            _days.ListChanged -= ListData_Changed;
            try
            {
                _days.Clear();
                if ((daysTable != null))
                    foreach (GXDLMSSpecialDay item in daysTable)
                        _days.Add(new SpecialDay() 
                        { 
                            Index = item.Index, 
                            DayId = item.DayId, 
                            Date = item.Date 
                        });
            }
            finally
            {
                _days.ListChanged += ListData_Changed;
                _changed = false;
            }
        }

        public event EventHandler<ListChangedEventArgs> OnChanged;

        private void SetChanged(ListChangedEventArgs e)
        {
            _changed = true;
            OnChanged?.Invoke(this, e);  //new EventArgs());
        }

        private void ListData_Changed(object sender, ListChangedEventArgs e)
        {
            if (
                (e.ListChangedType == ListChangedType.ItemChanged) ||
                (e.ListChangedType == ListChangedType.ItemAdded) ||
                (e.ListChangedType == ListChangedType.ItemDeleted)
               )
            {
                SetChanged(e);
            }
        }
    }

    public class SpecialDay : INotifyPropertyChanged
    {
        private GXDate date;
        private byte dayId;
        private ushort index;

        public UInt16 Index
        {
            get => index;
            set { index = value; OnPropertyChanged("Index"); }
        }

        //[JsonConverter(typeof(DLMSDateConverter))]
        [JsonIgnore]
        public GXDate Date
        {
            get => date;
            set { date = value; OnPropertyChanged("Date"); }
        }

        [XmlIgnore()]
        [JsonProperty("Date")]
        public string ActivateDateHS
        {
            get { return Date?.ToHex(false, false); }
            set { Date = GXDateTime.ParseHex(value) as GXDate; }
        }

        public byte DayId
        {
            get => dayId;
            set { dayId = value; OnPropertyChanged("DayId"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public override string ToString()
        {
            return Index.ToString() + " " + Date.ToString(true) + " " + DayId.ToString();
        }
    }
}