#if ZIP
using Gurux.DLMS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ZIP.DLMS
{
    /// <summary>
    /// Converts a binary value to and from a base 64 string value.
    /// </summary>
    public class DlmsDateTimeArrayConverter : JsonConverter
    {
        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value != null)
            {
                if (value is GXDateTime[])
                {
                    writer.WriteToken(JsonToken.StartArray, null);
                    //List<string> data = new List<string>();
                    foreach (var dt in (value as GXDateTime[]))
                    {
                        writer.WriteValue(dt.DLMSValue);
                    }
                    writer.WriteToken(JsonToken.EndArray);
                }
            }
            else
                writer.WriteNull();
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

            GXDateTime[] data;

            if (reader.TokenType == JsonToken.StartArray)
            {
                data = ReadDateTimeArray(reader);
            }
            else
            {
                throw new JsonSerializationException($"Unexpected token parsing binary. Expected StartArray, got {reader.TokenType}.");
            }

            return data;
        }

        private GXDateTime[] ReadDateTimeArray(JsonReader reader)
        {
            List<GXDateTime> dateList = new List<GXDateTime>();

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.String:
                        dateList.Add(new GXDateTime((string)reader.Value));
                        break;
                    case JsonToken.EndArray:
                        return dateList.ToArray();
                    case JsonToken.Comment:
                        // skip
                        break;
                    default:
                        throw new JsonSerializationException($"Unexpected token when reading: {reader.TokenType}");
                }
            }

            throw new JsonSerializationException("Unexpected end when reading array.");
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            if (
                objectType == typeof(GXDateTime[])
              )
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Converts a binary value to and from a base 64 string value.
    /// </summary>
    public class DlmsCommunicationWindowConverter : JsonConverter
    {
        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value != null)
            {
                if (value is List<KeyValuePair<GXDateTime, GXDateTime>>)
                {
                    serializer.Serialize(writer, value);
                    //writer.WriteToken(JsonToken.StartArray, null);
                    ////List<string> data = new List<string>();
                    //foreach (var dt in (value as List<KeyValuePair<GXDateTime, GXDateTime>>))
                    //{
                    //    serializer.Serialize(writer, dt);
                    //    //writer.WriteValue(
                    //}
                    //writer.WriteToken(JsonToken.EndArray);
                }
            }
            else
                writer.WriteNull();
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

            List<KeyValuePair<GXDateTime, GXDateTime>> data;

            if (reader.TokenType == JsonToken.StartArray)
            {
                data = ReadKeyValuePairsArray(reader);
            }
            else
            {
                throw new JsonSerializationException($"Unexpected token parsing binary. Expected StartArray, got {reader.TokenType}.");
            }

            return data;
        }

        private List<KeyValuePair<GXDateTime, GXDateTime>> ReadKeyValuePairsArray(JsonReader reader)
        {
            List<KeyValuePair<GXDateTime, GXDateTime>> dateList = new List<KeyValuePair<GXDateTime, GXDateTime>>();
            GXDateTime key = null;
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.StartObject:
                    case JsonToken.EndObject:
                        break;
                    case JsonToken.PropertyName:
                        if ((string)reader.Value == "Key")
                        {
                            reader.Read();
                            key = new GXDateTime((string)reader.Value);
                        }
                        else
                        if ((string)reader.Value == "Value")
                        {
                            reader.Read();
                            GXDateTime value = new GXDateTime((string)reader.Value);
                            dateList.Add(new KeyValuePair<GXDateTime, GXDateTime>(key, value));
                        }
                        break;
                    case JsonToken.EndArray:
                        return dateList;
                    case JsonToken.Comment:
                        // skip
                        break;
                    default:
                        throw new JsonSerializationException($"Unexpected token when reading: {reader.TokenType}");
                }
            }

            throw new JsonSerializationException("Unexpected end when reading array.");
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            if (
                objectType == typeof(List<KeyValuePair<GXDateTime, GXDateTime>>)
              )
            {
                return true;
            }
            return false;
        }
    }
}
#endif