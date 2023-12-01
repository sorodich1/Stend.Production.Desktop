#if ZIP
using Ankom.Common;
using Gurux.DLMS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ZIP.DLMS
{
    public class Utils
    {
        /// <summary>
        /// Преобразование произвольного объекта в байтовый массив
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<byte> ObjectToByteArray(Object obj, bool reverse = false)
        {
            List<byte> ret = new List<byte>();
            //List<KeyValuePair<byte[], Type>> ret1 = new List<KeyValuePair<byte[], Type>>();
            if (obj == null)
            {
                ret.AddRange(new byte[0]);
                //ret1.Add(new KeyValuePair<byte[], Type>( null, typeof(Nullable)));
            }
            else
            if (obj is byte[])
                //ret1.Add(new KeyValuePair<byte[], Type>((byte[])obj, typeof(byte[])));
                ret.AddRange(reverse ? (byte[])obj : ((byte[])obj).Reverse());
            else
            if (obj is byte || obj is sbyte)
                //ret1.Add(new KeyValuePair<byte[], Type>(new byte[] { (byte)obj }, typeof(byte)));
                ret.AddRange(new byte[] { (byte)obj });
            else
            if (obj is UInt16)
                //ret1.Add(new KeyValuePair<byte[], Type>(FromUint16((UInt16)obj, reverse), typeof(UInt16)));
                ret.AddRange(HexUtils.FromUint16((UInt16)obj, reverse));
            else
            if (obj is Int16)
                //ret1.Add(new KeyValuePair<byte[], Type>(FromInt16((Int16)obj, reverse), typeof(Int16)));
                ret.AddRange(HexUtils.FromInt16((Int16)obj, reverse));
            else
            if (obj is UInt32)
                //ret1.Add(new KeyValuePair<byte[], Type>(FromUint32((UInt32)obj, reverse), typeof(UInt32)));
                ret.AddRange(HexUtils.FromUint32((UInt32)obj, reverse));
            else
            if (obj is Int32)
                //ret1.Add(new KeyValuePair<byte[], Type>(FromInt32((Int32)obj, reverse), typeof(Int32)));
                ret.AddRange(HexUtils.FromInt32((Int32)obj, reverse));
            else
            if (obj is UInt64)
                //ret1.Add(new KeyValuePair<byte[], Type>(FromUint64((UInt64)obj, reverse), typeof(UInt64)));
                ret.AddRange(HexUtils.FromUint64((UInt64)obj, reverse));
            else
            if (obj is Int64)
                //ret1.Add(new KeyValuePair<byte[], Type>(FromInt64((Int64)obj, reverse), typeof(Int64)));
                ret.AddRange(HexUtils.FromInt64((Int64)obj, reverse));
            else
            if (obj is string)
            {
                // для строк не используем параметр reverse!!!
                //ret1.Add(new KeyValuePair<byte[], Type>(Encoding.UTF8.GetBytes((string)obj), typeof(string)));
                ret.AddRange(Encoding.UTF8.GetBytes((string)obj));
            }
            else
            if (obj is GXDateTime)
            {
                // для строк не используем параметр reverse!!!
                //ret1.Add(new KeyValuePair<byte[], Type>(Encoding.UTF8.GetBytes((string)obj), typeof(string)));
                ret.AddRange(HexUtils.HexStrToBytes((obj as GXDateTime)?.DLMSValue));
            }
            else
            {
                // попытка преобразование GXStructure и других наследников List<object>
                List<object> objList = obj as List<object>;
                if (objList != null)
                {
                    foreach (object item in objList)
                    {
                        //ret1.AddRange(ObjectToByteArray(item, reverse));
                        ret.AddRange(ObjectToByteArray(item, reverse));
                    }
                }
                else
                {
                    // для объекта произвольного типа не используем параметр reverse!!!
                    //List<byte> byteArray = new List<byte>();
                    BinaryFormatter bf = new BinaryFormatter();
                    MemoryStream ms = new MemoryStream();
                    bf.Serialize(ms, obj);
                    //ret1.Add(new KeyValuePair<byte[], Type>(ms.ToArray(), typeof(byte[])));
                    ret.AddRange(ms.ToArray());
                }
            }
            //return ret1;
            return ret;
        }
    }
    /// <summary>
    /// Converts a binary value to and from a base 64 string value.
    /// </summary>
    public class DlmsBinaryConverter : JsonConverter
    {
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

            byte[] data = GetByteArray(value);

            writer.WriteValue(data);
        }

        private byte[] GetByteArray(object value)
        {
            if (value is byte[] || value is Byte[])
                return value as byte[];

            if (value is SqlBinary binary)
            {
                return binary.Value;
            }

            if (value is GXByteBuffer buffer)
            {
                return buffer.Data;
            }


            throw new JsonSerializationException($"Unexpected value type when writing binary: {value.GetType()}");
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

            byte[] data;

            if (reader.TokenType == JsonToken.StartArray)
            {
                data = ReadByteArray(reader);
            }
            else if (reader.TokenType == JsonToken.String)
            {
                // current token is already at base64 string
                // unable to call ReadAsBytes so do it the old fashion way
                string encodedData = reader.Value.ToString();
                data = Convert.FromBase64String(encodedData);
            }
            else
            {
                throw new JsonSerializationException($"Unexpected token parsing binary. Expected String or StartArray, got {reader.TokenType}.");
            }

            Type t = objectType;


            if (t == typeof(SqlBinary))
            {
                return new SqlBinary(data);
            }
            else
            if (t == typeof(GXByteBuffer))
            {
                return new GXByteBuffer(data);
            }
            else
                return data;

            //throw new JsonSerializationException($"Unexpected object type when writing binary: {objectType}");
        }

        private byte[] ReadByteArray(JsonReader reader)
        {
            List<byte> byteList = new List<byte>();

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.Integer:
                        byteList.Add(Convert.ToByte(reader.Value, CultureInfo.InvariantCulture));
                        break;
                    case JsonToken.EndArray:
                        return byteList.ToArray();
                    case JsonToken.Comment:
                        // skip
                        break;
                    default:
                        throw new JsonSerializationException($"Unexpected token when reading bytes: {reader.TokenType}");
                }
            }

            throw new JsonSerializationException("Unexpected end when reading bytes.");
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
                objectType == typeof(GXByteBuffer) ||
                objectType == typeof(SqlBinary) ||
                objectType == typeof(byte[]) ||
                objectType == typeof(Byte[]) ||
                objectType == typeof(SqlBinary?)
              )
            {
                return true;
            }
            return false;
        }
    }
}
#endif