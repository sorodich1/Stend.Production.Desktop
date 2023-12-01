using NLog;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace Ankom.Common
{
    public static class UniqueHID
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Вычисляет уникальную hash-сумму по информации об установленном на компьютере оборудовании
        /// </summary>
        /// <returns></returns>
        public static string HID()
        {
            StringBuilder sb = new StringBuilder();

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2",
               "SELECT * FROM Win32_Processor");

            foreach (ManagementObject queryObj in searcher.Get())
            {
                sb.Append(queryObj["NumberOfCores"]);
                sb.Append(queryObj["ProcessorId"]);
                sb.Append(queryObj["Name"]);
                sb.Append(queryObj["SocketDesignation"]);
            }

            searcher = new ManagementObjectSearcher("root\\CIMV2",
              "SELECT * FROM Win32_BIOS");

            foreach (ManagementObject queryObj in searcher.Get())
            {
                sb.Append(queryObj["Manufacturer"]);
                sb.Append(queryObj["Name"]);
                sb.Append(queryObj["Version"]);
            }

            searcher = new ManagementObjectSearcher("root\\CIMV2",
               "SELECT * FROM Win32_BaseBoard");

            foreach (ManagementObject queryObj in searcher.Get())
            {
                sb.Append(queryObj["Product"]);
            }

            var bytes = Encoding.ASCII.GetBytes(sb.ToString());
            SHA256Managed sha = new SHA256Managed();

            string hash = HexUtils.BytesToHexStr(sha.ComputeHash(bytes));

            return hash;
        }
    }
}
