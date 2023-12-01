using Gurux.DLMS.Objects;

using System;
using System.Collections.Generic;

namespace Stend.Pruduction
{
    public class Spodes
    {
        public int ID { get; set; }
        public Nullable<int> ParentID { get; set; }
        public double OrderID { get; set; }
        public string OBIS { get; set; }
        public string DescRU { get; set; }
        public string DescEN { get; set; }
        public Nullable<int> IIC { get; set; }
        public List<int> Attr { get; set; }
        public Nullable<int> MeterClasses { get; set; }
        public Nullable<int> OKEI_ID { get; set; }
        public string Comment { get; set; }
        public Nullable<int> ObjectType { get; set; }
        public Nullable<int> Visibility { get; set; }
        public Nullable<bool> Оbligatory { get; set; }
        public string pivotTemplate { get; set; }
        private GXDLMSObject _object;
        public GXDLMSObject DevObject { get => _object; set => _object = value; }

        public static List<Spodes> spodes = new List<Spodes>();
    }
}
