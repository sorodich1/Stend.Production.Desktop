using System;

namespace Ankom.Common
{
    public class CRC16
    {
        const UInt16 PRESET_VALUE = 0xffff, POLYNOMIAL = 0x8408;
        const UInt16 CHECK_VALUE = 0xF0B8;

        // Рассчитывает контрольную сумму части байтового массива buffer длиной length
        public static UInt16 Calc(byte[] buffer, int length)
        {
            return (UInt16)~InternalCalc(buffer, length);
        }

        public static UInt16 Calc(byte[] buffer)
        {
            return (UInt16)~InternalCalc(buffer, buffer.Length);
        }

        /// <summary>
        ///  The standard 16-bit CRC polynomial specified in ISO/IEC 3309 is used .
        ///  Which is 0x8408: x^16  + x^12  + x^5 + 1 
        /// </summary>
        private static UInt16 InternalCalc(byte[] buffer, int bytesCount)
        {
            UInt16 i;
            UInt16 j;
            UInt16 current_crc_value = PRESET_VALUE;
            for (i = 0; i < bytesCount; i++)
            {
                current_crc_value = (UInt16)(current_crc_value ^ (buffer[i]));
                for (j = 0; j < 8; j++)
                {
                    if ((current_crc_value & 0x0001) != 0)
                    {
                        current_crc_value = (UInt16)((current_crc_value >> 1) ^ POLYNOMIAL);
                    }
                    else
                    {
                        current_crc_value = (UInt16)(current_crc_value >> 1);
                    }
                }
            }
            return current_crc_value;
        }

        /// <summary>
        /// Для  проверки принятых сообщений, включающих два байта CRC в хвосте пакета. 
        /// Если пакет корректен, рассчитанное для него значение контрольной суммы  CHECK_VALUE = 0xF0B8
        /// </summary>
        public static bool CheckPacket(byte[] buffer)
        {
            UInt16 current_crc_value = InternalCalc(buffer, buffer.Length);
            return current_crc_value == CRC16.CHECK_VALUE;
        }
    }
}
