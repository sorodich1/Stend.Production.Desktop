using Gurux.DLMS;
using Gurux.DLMS.Objects;

using System.ComponentModel;

namespace Stend.Production.Root
{
    public class CaptureGridObject
    {
        /// <summary>
        /// OBIS-код объекта захвата
        /// </summary>
        [DisplayName("OBIS-код")]
        public string OBIS { get; set; }
        /// <summary>
        /// Наименования объекта захвата
        /// </summary>
        [DisplayName("Наименование объекта")]
        public string Name { get; set; }
        /// <summary>
        /// Индекс элемента данных в списке объектов захвата
        /// </summary>
        [DisplayName("Индекс")]
        public int DataIndex { get; set; }
        /// <summary>
        /// Номер атрибута элемента данных в классе объекта
        /// </summary>
        [DisplayName("Атрибут")]
        public int AttributeInx { get; set; }
        /// <summary>
        /// комментарий объекта захвата
        /// </summary>
        [DisplayName("Комментарий")]
        public string Comment { get; set; }

        internal GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject> Column { get; private set; }

        public CaptureGridObject() { }

        public CaptureGridObject(GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject> keyValuePair)
        {
            UpdateColumn(keyValuePair);
        }

        internal void UpdateColumn(GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject> value)
        {
            Column = value;
            if (Column != null)
            {
                OBIS = Column.Key.LogicalName;
                DataIndex = Column.Value.DataIndex;
                AttributeInx = Column.Value.AttributeIndex;
                Name = Column.Key.Description;

                if (string.IsNullOrEmpty(Name))
                {
                    Name = "";
                    Column.Key.Description = Name;
                }
            }
        }
    }

    public class CaptureData : CaptureGridObject
    {
        [DisplayName("OBIS-код")]
        public new string OBIS { get => base.OBIS; }
        [DisplayName("Наименование объекта")]
        public new string Name { get => base.Name; }
        [DisplayName("Индекс")]
        public new int DataIndex { get => base.DataIndex; }
        [DisplayName("Аттрибут")]
        public new int AttributeInx { get => base.AttributeInx; }
        [DisplayName("Комментарий")]
        public new string Comment { get => base.Comment; }
        [DisplayName("Номер записи")]
        public int RecNo { get; set; }
        [DisplayName("Значение")]
        public string Value { get; set; }

        public CaptureData() : base() { }

        public CaptureData(GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject> value) : base(value) { }

        public CaptureData(CaptureGridObject gridObject) : base(gridObject?.Column)
        {
            if (gridObject != null)
                base.DataIndex = gridObject.DataIndex;
        }
    }
}
