//
// --------------------------------------------------------------------------
//  ANKOM+ Ltd.
// 
//
//
// Filename:        $HeadURL$
//
// Version:         $Revision$,
//                  $Date$
//                  $Author$
//
// Copyright (c) ANKOM+ Ltd.
//
//---------------------------------------------------------------------------
//
//  DESCRIPTION
//
// This file is a part of Gurux Device Framework extension from ANKOM+ Ltd.
// Defines the basic functionality with JSON working
//
// More information of Gurux products: https://www.gurux.org
//
// This code is licensed under the GNU General Public License v2. 
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------
#if ZIP
using Gurux.DLMS;
using Gurux.DLMS.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZIP.DLMS
{
    public class DLMSTypesBinder : ISerializationBinder
    {
        public IList<Type> KnownTypes { get; set; }

        public Type BindToType(string assemblyName, string typeName)
        {
            return KnownTypes.SingleOrDefault(t => t.Name == typeName);
        }

        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = serializedType.Name;
        }

        public DLMSTypesBinder()
        {
            KnownTypes = new List<Type>(GXDLMSClient.GetObjectTypes());
            KnownTypes.Add(typeof(GXByteBuffer));
            //KnownTypes.Add(typeof(GXDLMSScript));
            //KnownTypes.Add(typeof(GXDLMSScriptAction));
            //KnownTypes.Add(typeof(GXDLMSObjectDefinition));
            //KnownTypes.Add(typeof(GXDLMSCaptureObject));
            //KnownTypes.Add(typeof(GXDLMSEmergencyProfile));
            //KnownTypes.Add(typeof(GXDLMSSeasonProfile));
            //KnownTypes.Add(typeof(GXDLMSWeekProfile));
            //KnownTypes.Add(typeof(GXDLMSDayProfile));
            //KnownTypes.Add(typeof(GXDLMSDayProfileAction));
            //KnownTypes.Add(typeof(GXDLMSActionItem));
            //KnownTypes.Add(typeof(GXDLMSSpecialDay));
        }
    }

    public class DLMSScriptConverter : CustomCreationConverter<GXDLMSScript>
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.GetElementType() == typeof(GXDLMSScript);
        }
        public override GXDLMSScript Create(Type objectType)
        {
            return Activator.CreateInstance(objectType) as GXDLMSScript;
        }
    }

    public class DLMSScriptActionConverter : CustomCreationConverter<GXDLMSScriptAction>
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.GetElementType() == typeof(GXDLMSScriptAction);
        }
        public override GXDLMSScriptAction Create(Type objectType)
        {
            return Activator.CreateInstance(objectType) as GXDLMSScriptAction;
        }
    }

    public class DLMSObjectDefinitionConverter : CustomCreationConverter<GXDLMSObjectDefinition>
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.GetElementType() == typeof(GXDLMSObjectDefinition);
        }
        public override GXDLMSObjectDefinition Create(Type objectType)
        {
            return Activator.CreateInstance(objectType) as GXDLMSObjectDefinition;
        }
    }

    public class DLMSCaptureObjectConverter : CustomCreationConverter<GXDLMSCaptureObject>
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.GetElementType() == typeof(GXDLMSCaptureObject);
        }
        public override GXDLMSCaptureObject Create(Type objectType)
        {
            return Activator.CreateInstance(objectType) as GXDLMSCaptureObject;
        }
    }

    public class DLMSEmergencyProfileConverter : CustomCreationConverter<GXDLMSEmergencyProfile>
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.GetElementType() == typeof(GXDLMSEmergencyProfile);
        }
        public override GXDLMSEmergencyProfile Create(Type objectType)
        {
            return Activator.CreateInstance(objectType) as GXDLMSEmergencyProfile;
        }
    }

    public class DLMSSeasonProfileConverter : CustomCreationConverter<GXDLMSSeasonProfile[]>
    {
        public override bool CanConvert(Type objectType)
        {
            return
                objectType.GetElementType() == typeof(GXDLMSSeasonProfile[]);
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            string data = JsonConvert.SerializeObject(value);

            writer.WriteValue(data);
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            return ReadSeasonProfileArray(reader);
        }

        private object ReadSeasonProfileArray(JsonReader reader)
        {
            List<GXDLMSSeasonProfile> dayList = new List<GXDLMSSeasonProfile>();
            GXDLMSSeasonProfile item = null;
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.StartObject:
                        item = new GXDLMSSeasonProfile();
                        dayList.Add(item);
                        break;
                    case JsonToken.PropertyName:
                        switch ((string)reader.Value)
                        {
                            case "Name":
                                reader.Read();
                                item.Name = UTF8Encoding.ASCII.GetBytes((string)reader.Value);
                                break;
                            case "WeekName":
                                reader.Read();
                                item.WeekName = UTF8Encoding.ASCII.GetBytes((string)reader.Value);
                                break;
                            case "Start":
                                reader.Read();
                                item.HexStr = reader.Value.ToString();
                                break;
                            default:
                                break;
                        }
                        break;
                    case JsonToken.EndArray:
                        return dayList.ToArray();
                    case JsonToken.Comment:
                        // skip
                        break;
                    default:
                        break;
                }
            }
            throw new JsonSerializationException($"Unexpected token parsing json.");
        }

        public override GXDLMSSeasonProfile[] Create(Type objectType)
        {
            return Activator.CreateInstance(objectType) as GXDLMSSeasonProfile[];
        }
    }

    public class DLMSWeekProfileConverter : CustomCreationConverter<GXDLMSWeekProfile[]>
    {
        public override bool CanConvert(Type objectType)
        {
            return
                objectType.GetElementType() == typeof(GXDLMSWeekProfile[]);
        }

        public override GXDLMSWeekProfile[] Create(Type objectType)
        {
            return Activator.CreateInstance(objectType) as GXDLMSWeekProfile[];
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            string data = JsonConvert.SerializeObject(value);

            writer.WriteValue(data);
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            return ReadWeekProfileArray(reader);
        }

        private object ReadWeekProfileArray(JsonReader reader)
        {
            List<GXDLMSWeekProfile> dayList = new List<GXDLMSWeekProfile>();
            GXDLMSWeekProfile item = null;
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.StartObject:
                        item = new GXDLMSWeekProfile();
                        dayList.Add(item);
                        break;
                    case JsonToken.PropertyName:
                        switch ((string)reader.Value)
                        {
                            case "Name":
                                reader.Read();
                                item.Name = UTF8Encoding.ASCII.GetBytes((string)reader.Value);
                                break;
                            case "Monday":
                                reader.Read();
                                item.Monday = reader.Value != null ? Convert.ToInt32(reader.Value.ToString()) : 0;
                                break;
                            case "Tuesday":
                                reader.Read();
                                item.Tuesday = reader.Value != null ? Convert.ToInt32(reader.Value.ToString()) : 0;
                                break;
                            case "Wednesday":
                                reader.Read();
                                item.Wednesday = reader.Value != null ? Convert.ToInt32(reader.Value.ToString()) : 0;
                                break;
                            case "Thursday":
                                reader.Read();
                                item.Thursday = reader.Value != null ? Convert.ToInt32(reader.Value.ToString()) : 0;
                                break;
                            case "Friday":
                                reader.Read();
                                item.Friday = reader.Value != null ? Convert.ToInt32(reader.Value.ToString()) : 0;
                                break;
                            case "Saturday":
                                reader.Read();
                                item.Saturday = reader.Value != null ? Convert.ToInt32(reader.Value.ToString()) : 0;
                                break;
                            case "Sunday":
                                reader.Read();
                                item.Sunday = reader.Value != null ? Convert.ToInt32(reader.Value.ToString()) : 0;
                                break;
                            default:
                                break;
                        }
                        break;
                    case JsonToken.EndArray:
                        return dayList.ToArray();
                    case JsonToken.Comment:
                        // skip
                        break;
                    default:
                        break;
                }
            }

            throw new JsonSerializationException($"Unexpected token parsing json.");
        }
    }

    public class DLMSDayProfileArrayConverter : CustomCreationConverter<GXDLMSDayProfile[]>
    {
        public override bool CanConvert(Type objectType)
        {
            return
                objectType.GetElementType() == typeof(GXDLMSDayProfile[]);
        }

        public override GXDLMSDayProfile[] Create(Type objectType)
        {
            return (new List<GXDLMSDayProfile>()).ToArray();
            //Activator.CreateInstance(objectType) as GXDLMSDayProfile[];
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            string data = JsonConvert.SerializeObject(value);

            writer.WriteValue(data);
        }
        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            return ReadDayProfileArray(reader, serializer);
        }

        private object ReadDayProfileArray(JsonReader reader, JsonSerializer serializer)
        {
            List<GXDLMSDayProfile> dayList = new List<GXDLMSDayProfile>();
            GXDLMSDayProfile item = null;
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.StartObject:
                        item = new GXDLMSDayProfile();
                        dayList.Add(item);
                        break;
                    case JsonToken.PropertyName:
                        switch ((string)reader.Value)
                        {
                            case "DaySchedules":
                                //reader.Read();
                                DLMSDayProfileActionConverter cnv = new DLMSDayProfileActionConverter();
                                item.DaySchedules = cnv.ReadJson(reader, typeof(GXDLMSDayProfileAction[]), null, serializer) as GXDLMSDayProfileAction[];
                                break;
                            case "DayId":
                                reader.Read();
                                item.DayId = reader.Value != null ? Convert.ToUInt16(reader.Value.ToString()) : 0;
                                break;
                            default:
                                break;
                        }
                        break;
                    case JsonToken.EndArray:
                        return dayList.ToArray();
                    case JsonToken.Comment:
                        // skip
                        break;
                    default:
                        break;
                }
            }
            throw new JsonSerializationException($"Unexpected token parsing json.");
        }
    }

    public class DLMSDayProfileActionConverter : CustomCreationConverter<GXDLMSDayProfileAction[]>
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.GetElementType() == typeof(GXDLMSDayProfileAction[]);
        }
        public override GXDLMSDayProfileAction[] Create(Type objectType)
        {
            return (new List<GXDLMSDayProfileAction>()).ToArray(); ;
        }
        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            string data = JsonConvert.SerializeObject(value);

            writer.WriteValue(data);
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            return ReadDayProfileActionArray(reader);
        }

        private object ReadDayProfileActionArray(JsonReader reader)
        {
            List<GXDLMSDayProfileAction> dayList = new List<GXDLMSDayProfileAction>();
            GXDLMSDayProfileAction item = null;
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.StartObject:
                        item = new GXDLMSDayProfileAction();
                        dayList.Add(item);
                        break;
                    case JsonToken.PropertyName:
                        switch ((string)reader.Value)
                        {
                            case "ScriptLogicalName":
                                reader.Read();
                                item.ScriptLogicalName = (string)reader.Value;
                                break;
                            case "ScriptSelector":
                                reader.Read();
                                item.ScriptSelector = reader.Value != null ? Convert.ToUInt16(reader.Value.ToString()) : (UInt16)0;
                                break;
                            case "StartTime":
                                reader.Read();
                                item.HexStr = reader.Value.ToString();
                                break;
                            default:
                                break;
                        }
                        break;
                    case JsonToken.EndArray:
                        return dayList.ToArray();
                    case JsonToken.Comment:
                        // skip
                        break;
                    default:
                        break;
                }
            }
            throw new JsonSerializationException($"Unexpected token parsing json.");
        }
    }

    public class DLMSActionItemConverter : CustomCreationConverter<GXDLMSActionItem>
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.GetElementType() == typeof(GXDLMSActionItem);
        }
        public override GXDLMSActionItem Create(Type objectType)
        {
            return Activator.CreateInstance(objectType) as GXDLMSActionItem;
        }
    }

    public class DLMSSpecialDayArrayConverter : CustomCreationConverter<GXDLMSSpecialDay[]>
    {
        public override bool CanConvert(Type objectType)
        {
            return
                objectType.GetElementType() == typeof(GXDLMSSpecialDay[]);
        }
        public override GXDLMSSpecialDay[] Create(Type objectType)
        {
            return Activator.CreateInstance(objectType) as GXDLMSSpecialDay[];
        }
        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            string data = JsonConvert.SerializeObject(value);

            writer.WriteValue(data);
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            if (reader.TokenType == JsonToken.StartArray)
                return ReadSpecialDayArray(reader);

            throw new JsonSerializationException($"Unexpected token parsing binary. Expected StartArray, got {reader.TokenType}.");
        }

        private object ReadSpecialDayArray(JsonReader reader)
        {
            List<GXDLMSSpecialDay> dayList = new List<GXDLMSSpecialDay>();
            GXDLMSSpecialDay item = null;
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.StartObject:
                        item = new GXDLMSSpecialDay();
                        dayList.Add(item);
                        break;
                    case JsonToken.PropertyName:
                        switch ((string)reader.Value)
                        {
                            case "Index":
                                reader.Read();
                                item.Index = reader.Value != null ? Convert.ToUInt16(reader.Value) : (UInt16)0;
                                break;
                            case "Date":
                                reader.Read();
                                item.Date = new GXDate((string)reader.Value);
                                break;
                            case "DayId":
                                reader.Read();
                                item.DayId = Convert.ToByte(reader.Value);
                                break;
                            default:
                                break;
                        }
                        break;
                    case JsonToken.EndArray:
                        return dayList.ToArray();
                    case JsonToken.Comment:
                        // skip
                        break;
                    default:
                        break;
                }
            }

            throw new JsonSerializationException("Unexpected end when reading json.");
        }
    }

    public class DLMSByteArrayConverter : CustomCreationConverter<Byte[]>
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsArray &&
                (
                   objectType.GetElementType() == typeof(Byte[]) ||
                   objectType.GetElementType() == typeof(byte[])
                );
        }
        public override Byte[] Create(Type objectType)
        {
            return Activator.CreateInstance(objectType) as Byte[];
        }
    }

    public class DLMSDateTimeConverter : CustomCreationConverter<GXDateTime>
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.GetElementType() == typeof(GXDateTime);
        }
        public override GXDateTime Create(Type objectType)
        {
            // TODO: организовать проверку наличия соответствующего типа в DLMS
            return Activator.CreateInstance(objectType) as GXDateTime;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return
                GXDateTime.ParseHex(reader.ReadAsString());
            //base.ReadJson(reader, objectType, existingValue, serializer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string text;

            if (value is GXDateTime dateTime)
            {
                text = dateTime.DLMSValue;
            }
            else
            {
                throw new JsonSerializationException("Unexpected value when converting GXDateTime.");
            }

            writer.WriteValue(text);
        }
    }
    public class DLMSTimeConverter : CustomCreationConverter<GXTime>
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.GetElementType() == typeof(GXTime);
        }
        public override GXTime Create(Type objectType)
        {
            // TODO: организовать проверку наличия соответствующего типа в DLMS
            return Activator.CreateInstance(objectType) as GXTime;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //return base.ReadJson(reader, objectType, existingValue, serializer);
            return
                new GXTime(GXDateTime.ParseHex(reader.ReadAsString()));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string text;

            if (value is GXTime dateTime)
            {
                text = dateTime.DLMSValue;
            }
            else
            {
                throw new JsonSerializationException("Unexpected value when converting GXTime.");
            }

            writer.WriteValue(text);
        }
    }
    public class DLMSDateConverter : CustomCreationConverter<GXDate>
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.GetElementType() == typeof(GXDate);
        }
        public override GXDate Create(Type objectType)
        {
            // TODO: организовать проверку наличия соответствующего типа в DLMS
            return Activator.CreateInstance(objectType) as GXDate;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return
                new GXDate(GXDateTime.ParseHex(reader.ReadAsString()));
            //return base.ReadJson(reader, objectType, existingValue, serializer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string text;

            if (value is GXDate dateTime)
            {
                text = dateTime.DLMSValue;
            }
            else
            {
                throw new JsonSerializationException("Unexpected value when converting GXDate.");
            }

            writer.WriteValue(text);
        }
    }
}
#endif