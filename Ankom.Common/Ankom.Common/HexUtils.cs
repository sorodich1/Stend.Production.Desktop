using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;

namespace Ankom.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class ByteArrayExt
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] SetBit(this byte[] self, int index, bool value)
        {
            int byteIndex = index / 8;
            int bitIndex = index % 8;
            byte mask = (byte)(1 << bitIndex);

            self[byteIndex] = (byte)(value ? (self[byteIndex] | mask) : (self[byteIndex] & ~mask));
            return self;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static byte[] ToggleBit(this byte[] self, int index)
        {
            int byteIndex = index / 8;
            int bitIndex = index % 8;
            byte mask = (byte)(1 << bitIndex);

            self[byteIndex] ^= mask;
            return self;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool GetBit(this byte[] self, int index, bool value)
        {
            int byteIndex = index / 8;
            int bitIndex = index % 8;
            byte mask = (byte)(1 << bitIndex);

            return (self[byteIndex] & mask) != 0;
        }
    }

    public static class StrUtils
    {
        public static string SurroundWithDoubleQuotes(this string text)
        {
            return SurroundWith(text, "\"");
        }

        public static string SurroundWith(this string text, string ends)
        {
            return ends + text + ends;
        }

        public static bool IsBase64String(string base64String)
        {
            //string pattern = @"^[a-zA-Z0-9\+/]*={0,3}$";
            //Regex rgx = new Regex(pattern);
            //return (
            //    !string.IsNullOrEmpty(base64String) &&
            //    !string.IsNullOrWhiteSpace(base64String) &&
            //    base64String.Length != 0 &&
            //    base64String.Length % 4 == 0 &&
            //    !base64String.Contains(" ") &&
            //    !base64String.Contains("\t") &&
            //    !base64String.Contains("\r") &&
            //    !base64String.Contains("\n")
            //    ) && (base64String.Length % 4 == 0 && rgx.Match(base64String, 0).Success);

            string pattern = @"^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)?$";
            Regex rgx = new Regex(pattern);
            return rgx.IsMatch(base64String);
        }

        public static byte[] ToHexArray(this string text, Encoding encoding = null)
        {
            if (encoding == null)
                return Encoding.Default.GetBytes(text);
            else return encoding.GetBytes(text);
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Раскодирует строку из кодировки Base64 в UTF-8
        /// </summary>
        /// <param name="base64EncodedData"></param>
        /// <returns></returns>
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }

    public static class HexUtils
    {

        /// <summary>
        /// Проверяет, установлен ли (=1) указанный бит байтового массива
        /// </summary>
        /// <param name="bitIndex">индекс бита в массиве</param>
        /// <param name="data">байтовый массив</param>
        /// <returns>true, если бит установлен</returns>
        public static bool IsBitOn(int bitIndex, byte[] data)
        {
            int byteIndex = bitIndex / 8;
            if (byteIndex >= data.Length)
                return false;
            int bitInByteIndex = bitIndex % 8;
            byte mask = (byte)(1 << bitInByteIndex);
            return
                (data[byteIndex] & mask) != 0;
        }

        /// <summary>
        /// Установка в 1 указанного бита байтового массива
        /// </summary>
        /// <param name="bitIndex">индекс бита в массиве</param>
        /// <param name="data">байтовый массив</param>
        /// <returns>true, если бит установлен</returns>
        public static bool SetBitOn(int bitIndex, byte[] data)
        {
            int byteIndex = bitIndex / 8;
            if (byteIndex >= data.Length)
                return false;
            int bitInByteIndex = bitIndex % 8;
            byte mask = (byte)(1 << bitInByteIndex);
            data[byteIndex] |= mask;
            return true;
        }

        /// <summary>
        /// Сброс (установка в 0) указанного бита байтового массива
        /// </summary>
        /// <param name="bitIndex">индекс бита в массиве</param>
        /// <param name="data">байтовый массив</param>
        /// <returns>true, если бит сброшен</returns>
        public static bool SetBitOff(int bitIndex, byte[] data)
        {
            int byteIndex = bitIndex / 8;
            if (byteIndex >= data.Length)
                return false;
            int bitInByteIndex = bitIndex % 8;
            byte mask = (byte)(1 << bitInByteIndex);
            data[byteIndex] &= (byte)~mask;
            return true;
        }

        /// <summary>
        /// Переключение указанного бита байтового массива
        /// </summary>
        /// <param name="bitIndex">индекс бита в массиве</param>
        /// <param name="data">байтовый массив</param>
        /// <returns>true, если бит переключен</returns>
        public static bool ToggleBit(int bitIndex, byte[] data)
        {
            int byteIndex = bitIndex / 8;
            if (byteIndex >= data.Length)
                return false;
            int bitInByteIndex = bitIndex % 8;
            byte mask = (byte)(1 << bitInByteIndex);
            data[byteIndex] ^= mask;
            return true;
        }

        internal static bool IsHexDigit(char c)
        {
            string hexChars = "ABCDEF";
            return IsDigit(c) || hexChars.IndexOf(c) >= 0;
        }

        internal static bool IsDigit(char c)
        {
            string hexChars = "0123456789";
            return hexChars.IndexOf(c) >= 0;
        }

        /// <summary>
        /// Проверяет, что указанная строка состоит только из шестнадцатиричных цифр
        /// </summary>
        /// <param name="input">проверяемая строка</param>
        /// <returns></returns>
        public static bool IsHexString(string input)
        {
            if (String.IsNullOrWhiteSpace(input))
                return false;
            var illegalCount = (from ch in input.ToUpperInvariant()
                                where !IsHexDigit(ch)
                                select ch).Count();
            return illegalCount == 0;
        }

        /// <summary>
        /// Проверяет, что указанная строка состоит только из цифр
        /// </summary>
        /// <param name="input">проверяемая строка</param>
        /// <returns></returns>
        public static bool IsDigitString(string input)
        {
            if (String.IsNullOrWhiteSpace(input))
                return false;
            var illegalCount = (from ch in input.ToUpperInvariant()
                                where !IsDigit(ch)
                                select ch).Count();
            return illegalCount == 0;
        }

        /// <summary>
        /// Преобразование числа в диапазоне [-127,127] из прямого кода в дополнительный. 
        /// Осуществляется по следующему алгоритму:
        /// Если число, записанное в прямом коде, положительное, то к нему дописывается старший(знаковый) разряд, равный 0, и на этом преобразование заканчивается;
        /// Если число, записанное в прямом коде, отрицательное, то все разряды числа инвертируются, а к результату прибавляется 1. К получившемуся числу дописывается старший(знаковый) разряд, равный 1. 
        /// </summary>
        /// <param name="value">число в диапазоне [-127,127]</param>
        /// <returns>дополнительный код заданного числа</returns>
        public static byte AdditionalCode(int value)
        {
            if (value >= 0)
                return (byte)(value & 0x7F);
            return (byte)(((~value) + 1) | 0x80);
        }

        public static int FromAdditionalCode(byte value)
        {
            if ((value & 0x80) != 0)
                return -(value & 0x7F);
            else
                return value;
        }

        public static int RestoreRightCode(byte value)
        {
            if ((value & 0x80) != 0)
                return value - 256;
            else
                return value;
        }

        public static string CheckHex(string input)
        {
            if (IsHexString(input))
            {
                if (input.Length % 2 != 0)
                    return '0' + input;
                else return input;
            }
            else
                throw new Exception("Входная строка не является 16-ричной");
        }

        public static byte HexStrToByte(string input)
        {
            return byte.Parse(input, NumberStyles.HexNumber);
        }

        public static string ByteToHexStr(byte input)
        {
            return CheckHex(input.ToString("X"));
        }

        public static long HexStrToLong(string input)
        {
            return long.Parse(input, NumberStyles.HexNumber);
        }

        public static string LongToHexStr(long input)
        {
            return CheckHex(input.ToString("X"));
        }

        public static ulong HexStrToULong(string input)
        {
            return ulong.Parse(input, NumberStyles.HexNumber);
        }

        public static string ULongToHexStr(ulong input)
        {
            return CheckHex(input.ToString("X"));
        }

        public static byte[] HexStrToBytes(string input)
        {
            if (String.IsNullOrEmpty(input))
                return null;
            string normalHex = CheckHex(input);
            byte[] result = new byte[normalHex.Length / 2];
            for (int i = 0, first = 0, second = 1; i < result.Length; ++i, first = first + 2, second = second + 2)
                result[i] = HexStrToByte(normalHex[first] + normalHex[second].ToString());
            return result;
        }

        public static string BytesToHexStr(byte[] input, bool reverse = false)
        {
            string result = string.Empty;
            if (input != null)
            {
                if (reverse)
                    input = input.Reverse().ToArray();
                foreach (byte _byte in input)
                    result += CheckHex(ByteToHexStr(_byte));
            }
            return result;
        }

        /// <summary>
        /// Копирует copyLength байтов из буфера source, начиная с индекса startIndex в новый буфер 
        /// При copyLength = -1 будут скопированы все байты исходного буфера, начиная с индекса startIndex
        /// </summary>
        /// <param name="source"></param>
        /// <param name="startIndex"></param>
        /// <param name="copyLength"></param>
        /// <param name="bufSize"></param>
        /// <param name="reverse"></param>
        /// <returns></returns>
        public static byte[] ExtractData(byte[] source, int startIndex, int copyLength = -1, bool reverse = false)
        {
            if (source == null)
                return null;
            copyLength = (copyLength < 0) ? source.Length - startIndex : Math.Min(source.Length - startIndex, copyLength);
            byte[] tmp = new byte[copyLength];
            Array.Copy(source, startIndex, tmp, 0, copyLength);
            if (reverse)
                Array.Reverse(tmp);
            return tmp;
        }

        public static byte[] FromUint16(UInt16 source, bool reverse = true)
        {
            if (reverse)
                return BitConverter.GetBytes(source).Reverse().ToArray();
            else
                return BitConverter.GetBytes(source);
        }
        public static byte[] FromInt16(Int16 source, bool reverse = true)
        {
            return FromUint16((UInt16)source, reverse);
        }
        public static byte[] FromUint32(UInt32 source, bool reverse = true)
        {
            if (reverse)
                return BitConverter.GetBytes(source).Reverse().ToArray();
            else
                return BitConverter.GetBytes(source);
        }
        public static byte[] FromInt32(Int32 source, bool reverse = true)
        {
            return FromUint32((UInt32)source, reverse);
        }
        public static byte[] FromUint64(UInt64 source, bool reverse = true)
        {
            if (reverse)
                return BitConverter.GetBytes(source).Reverse().ToArray();
            else
                return BitConverter.GetBytes(source);
        }
        public static byte[] FromInt64(Int64 source, bool reverse = true)
        {
            return FromUint64((UInt64)source, reverse);
        }

        public static UInt16 ToUInt16(byte[] source, int startIndex, int length = sizeof(UInt16), bool reverse = false)
        {
            UInt16 retCode = 0;
            byte[] tmp = ExtractData(source, startIndex, length, reverse);
            for (int i = 0; i < Math.Min(length, sizeof(UInt16)); i++)
            {
                retCode += (UInt16)(tmp[i] << (8 * i));
            }
            return retCode;
            //BitConverter.ToUInt16(ExtractData(source, startIndex, length, reverse), 0);
        }
        public static Int16 ToInt16(byte[] source, int startIndex, int length = sizeof(UInt16), bool reverse = false)
        {
            return (Int16)ToUInt16(source, startIndex, length, reverse);
        }
        public static UInt32 ToUInt32(byte[] source, int startIndex, int length = sizeof(UInt32), bool reverse = false)
        {
            UInt32 retCode = 0;
            byte[] tmp = ExtractData(source, startIndex, length, reverse);
            for (int i = 0; i < Math.Min(length, sizeof(UInt32)); i++)
            {
                retCode += (UInt32)(tmp[i] << (8 * i));
            }
            return retCode;
            //            return BitConverter.ToUInt32(ExtractData(source, startIndex, length, reverse), 0);
        }
        public static Int32 ToInt32(byte[] source, int startIndex, int length = sizeof(UInt32), bool reverse = false)
        {
            return (Int32)ToUInt32(source, startIndex, length, reverse);
        }
        public static UInt64 ToUInt64(byte[] source, int startIndex, int length = sizeof(UInt64), bool reverse = false)
        {
            UInt32 retCode = 0;
            byte[] tmp = ExtractData(source, startIndex, length, reverse);
            for (int i = 0; i < Math.Min(length, sizeof(UInt64)); i++)
            {
                retCode += (UInt32)(tmp[i] << (8 * i));
            }
            return retCode;
            //            return BitConverter.ToUInt32(ExtractData(source, startIndex, length, reverse), 0);
        }
        public static Int64 ToInt64(byte[] source, int startIndex, int length = sizeof(UInt64), bool reverse = false)
        {
            return (Int64)ToUInt64(source, startIndex, length, reverse);
        }

        #region функции работы с BCD
        public static UInt64 BCD2Long(byte[] source)
        {
            UInt64 result = 0;
            foreach (byte b in source)
            {
                UInt64 hi = (UInt64)(b >> 4);
                UInt64 lo = (UInt64)(b & 0x0F);
                result *= 10;
                result += hi;
                result *= 10;
                result += lo;
            }
            return result;
        }

        /// <summary>
        /// преобразует байт, упакованный в формат BCD, в соответствующее значение типа Int16
        /// Пример: 0x99 дает десятичное значение 99
        /// </summary>
        /// <param name="source">байт данных</param>
        /// <returns></returns>
        public static UInt16 BCD2Int16(byte source)
        {
            UInt16 result = 0;
            int hi = source >> 4;
            int lo = source & 0x0F;
            result += (UInt16)hi;
            result *= 10;
            result += (UInt16)lo;
            return result;
        }

        public static byte Byte2BCD(byte Value)
        {
            if (Value > 99)
                throw new Exception("Value exceed 99");
            return (byte)(((Value / 10) << 4) + (Value % 10));
        }

        /// <summary>
        /// преобразует числовую строку, состоящую из десятичных символов в массив байт формата BCD
        /// Пример: строка "111199999" дает массив  0x011119999
        /// </summary>
        /// <param name="Value">строка, состоящая из десятичных символов</param>
        /// <returns>массив байт формата BCD или null в случае ошибки</returns>
        public static byte[] DecStr2BCD(string Value)
        {
            List<byte> retCode = new List<byte>();
            try
            {
                if ((Value.Length % 2) == 1)
                    Value = Value.Insert(0, "0");
                for (int i = 0; i < Value.Length; i += 2)
                {

                    retCode.Add(
                        (byte)((Byte.Parse(Value[i].ToString(), NumberStyles.None) << 4) +
                        Byte.Parse(Value[i + 1].ToString()))
                    );
                }
                return retCode.ToArray();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// преобразует значение даты и времени типа DateTime в байтовый массив, упакованные в формат BCD. 
        /// байты в буфере будут представлены в следующем порядке следования (с младших разрядов): 
        ///    для includeSeconds = true  - секунды, минуты, часы, день месяца, месяц, год (короткий формат - последние 2 цифры) 
        ///    для includeSeconds = false - минуты, часы, день месяца, месяц, год (короткий формат - последние 2 цифры) 
        /// Пример: 31.12.2015 23:59:59 дает значение 0x...151231235959  
        /// </summary>
        /// <param name="dtValue">значение даты и времени для преобразования</param>
        /// <param name="includeSeconds">признак включения секунд в результирующий блок данных</param>
        /// <param name="includeMinutes">признак включения секунд в результирующий блок данных</param>
        /// <param name="reverse">признак следования данных в порядке: год, месяц...</param>
        /// <returns>упакованный в формат BCD байтовый массив значений даты и времени</returns>
        public static byte[] DateTime2BCD(DateTime dtValue, bool includeSeconds = true, bool includeMinutes = true, bool reverse = false)
        {
            List<byte> retCode = new List<byte>();
            try
            {
                if (includeMinutes)
                {
                    if (includeSeconds)
                    {
                        retCode.Add(Byte2BCD((byte)dtValue.Second));
                    }
                    retCode.Add(Byte2BCD((byte)dtValue.Minute));
                }
                retCode.Add(Byte2BCD((byte)dtValue.Hour));
                retCode.Add(Byte2BCD((byte)dtValue.Day));
                retCode.Add(Byte2BCD((byte)dtValue.Month));
                retCode.Add(Byte2BCD((byte)(dtValue.Year - 2000)));
                if (reverse)
                    retCode.Reverse();
                return retCode.ToArray();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// преобразует значение даты типа DateTime в байтовый массив, упакованные в формат BCD. 
        /// байты в буфере будут представлены в следующем порядке следования (с младших разрядов): 
        ///    день месяца (если включен флаг includeDay), месяц, год (короткий формат - последние 2 цифры) 
        /// Пример: 31.12.2015 23:59:59 дает значение 0x151231  
        /// </summary>
        /// <param name="dtValue">значение даты и времени для преобразования</param>
        /// <param name="reverse">признак следования данных в порядке: год, месяц...</param>
        /// <returns>упакованный в формат BCD байтовый массив значений даты и времени</returns>
        public static byte[] Date2BCD(DateTime dtValue, bool includeDay = true, bool reverse = false)
        {
            List<byte> retCode = new List<byte>();
            try
            {
                if (includeDay)
                    retCode.Add(Byte2BCD((byte)dtValue.Day));
                retCode.Add(Byte2BCD((byte)dtValue.Month));
                retCode.Add(Byte2BCD((byte)(dtValue.Year - 2000)));
                if (reverse)
                    retCode.Reverse();
                return retCode.ToArray();
            }
            catch
            {
                return null;
            }
        }

        public static byte[] DateTime2BCDNoSeconds(DateTime dtValue, bool reverse = false)
        {
            return DateTime2BCD(dtValue, false, true, reverse);
        }

        public static byte[] DateTime2BCDNoMinutes(DateTime dtValue, bool reverse = false)
        {
            return DateTime2BCD(dtValue, false, false, reverse);
        }

        /// <summary>
        /// преобразует данные буфера, упакованные в формат BCD, в соответствующее значение даты и времени типа DateTime
        /// байты в буфере должны быть представлены в следующем порядке следования (с младших разрядов): 
        ///  для includeSeconds = true  - секунды, минуты, часы, день месяца, месяц, год (короткий формат - последние 2 цифры) 
        ///  для includeSeconds = false - минуты, часы, день месяца, месяц, год (короткий формат - последние 2 цифры) 
        /// Пример: 0x...151231235959 для startIndex = 0 дает значение 31.12.2015 23:59:59
        /// </summary>
        /// <param name="buf">байтовый массив с упакованными данными даты и времени</param>
        /// <param name="startIndex">начальный индекс массива, откуда начинается блок данных</param>
        /// <param name="includeSeconds">признак наличия секунд в блоке данных</param>
        /// <param name="includeMinutes">признак наличия минут в блоке данных</param>
        /// <returns></returns>
        public static DateTime? BCD2DateTime(byte[] buf, int startIndex, bool includeSeconds, bool includeMinutes)
        {
            int ss = 0, mm = 0, hh, dd, MM, yy;
            try
            {
                if (includeMinutes)
                {
                    if (includeSeconds)
                    {
                        ss = BCD2Int16(buf[startIndex++]);
                    }
                    mm = BCD2Int16(buf[startIndex++]);
                }
                hh = BCD2Int16(buf[startIndex++]);
                dd = BCD2Int16(buf[startIndex++]);
                MM = BCD2Int16(buf[startIndex++]);
                yy = BCD2Int16(buf[startIndex++]);
                if ((dd == 0) || (MM == 0) || (yy == 00) || (yy > 99))
                    return null;
                else
                    return new DateTime(2000 + yy, MM, dd, hh, mm, ss);
            }
            catch
            {
                return null;
            }
        }

        public static DateTime? BCD2DateTimeNoSeconds(byte[] buf, int startIndex)
        {
            return BCD2DateTime(buf, startIndex, false, true);
        }

        public static DateTime? BCD2DateTimeNoMinutes(byte[] buf, int startIndex)
        {
            return BCD2DateTime(buf, startIndex, false, false);
        }

        /// <summary>
        /// преобразует данные буфера, упакованные в формат BCD, в соответствующее значение даты типа DateTime
        /// байты в буфере должны быть представлены в следующем порядке следования (с младших разрядов): 
        ///  день месяца, месяц, год (короткий формат - последние 2 цифры) 
        /// Пример: 0x...151201 для startIndex = 0 дает значение 01.12.2015
        ///         0x...151201 для startIndex = 0 и includeDay = false дает значение 01.01.2012
        /// </summary>
        /// <param name="buf">байтовый массив с упакованными данными даты</param>
        /// <param name="startIndex">начальный индекс массива, откуда начинается блок данных</param>
        /// <param name="includeDay">признак наличия дней в блоке данных</param>
        /// <returns></returns>
        public static DateTime? BCD2Date(byte[] buf, int startIndex, bool includeDay = true)
        {
            int dd, MM, yy;
            try
            {
                dd = (includeDay) ? BCD2Int16(buf[startIndex++]) : 1;
                MM = BCD2Int16(buf[startIndex++]);
                yy = BCD2Int16(buf[startIndex++]);
                if ((dd == 0) || (MM == 0) || (yy == 00) || (yy > 99))
                    return null;
                else
                    return new DateTime(2000 + yy, MM, dd);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// проверяет, является ли указанный диапазон данных байтового массива пустым или включает только байты 0x00  
        /// </summary>
        /// <param name="data">проверяемый двоичный (байтовый) массива</param>
        /// <param name="startIndex">начальный индекс, с которого начинается проверка</param>
        /// <param name="length">длина проверяемого диапазона данных</param>
        /// <returns>true, если указанный массив не определен, проверяемый диапазон имеет нулевую длину или включает только байты 0x00</returns>
        public static bool IsEmptyOrZero(byte[] data, int startIndex = 0, int length = -1)
        {
            if (data == null)
                return true;
            byte[] tmp = ExtractData(data, startIndex, length);
            return (tmp.Length == 0) || (tmp.Count(item => item != 0x00) == 0);
        }

        #endregion

    }
}