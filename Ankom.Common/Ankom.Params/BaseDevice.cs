using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Ankom.Params
{
    /// <summary>
    /// �������� ���������� ������������� ���������� ��� ����������� ��� ������ � �������������� ���������-�
    /// </summary>
    public class BaseDevice
    {
        /// <summary>
        /// ������� ����� ����������� � ���� ������ ������
        /// </summary>
        public int requestID = 0;

        /// <summary>
        /// ���������� (handle) ������ �����������. ���� ����� 0, ����������� ������� �����
        /// </summary>
        public int taskID = 0;

        /// <summary>
        /// ������� ������� � �������� ��������
        /// </summary>
        public AccessLevels accessLevel = AccessLevels.UnProtected;

        /// <summary>
        /// ���� � ����� � ����������� �������� � ������ ������������� ��������, 
        /// � ����� ��������������� ������� ��� �������������� ��������: ����, ���� ���������� ���������
        /// </summary>
        public String requestFile;

        /// <summary>
        /// K�� ������������� ��������, ��������: EGM ��� ��������� GAMMA
        /// </summary>
        public String manufacturerID;

        /// <summary>
        /// C������� ����� ��������, ��������: 1387034
        /// </summary>
        public int serialNumber;

        /// <summary>
        /// C������ ����� ��������. ��� ��������� GAMMA ����������� �� ���� ��� ��������� ������, ��27XX - �� �����?
        /// </summary>
        public int netAdr;

        /// <summary>
        /// ������ ��������: �� ��������� ������ ��� ������ ��������� � ��������� ��� ��������� ����������
        /// </summary>
        public UInt32 password;

        /// <summary>
        /// ���� ������ ������
        /// </summary>
        public UInt16 com_port = 0;

        private string _channelAddr;

        /// <summary>
        /// ����� ������ ������. � ����������� �� ���� ������ ����� ���� ������� �������� ����� ������� ������, 
        /// IP-������� ��� ������ �� TCP-IP ��� GPRS, ������� CSD-��������, ��� ����� ������������� 
        /// </summary>
        public string ChannelAddr
        {
            get
            {
                return _channelAddr;
            }
            set
            {
                _channelAddr = value;
            }
        }

        public bool valid;

        public String[] modification = { "" };
        public bool is3phase = false;

        /// <summary>
        /// ���� � ����� � ����������� ������ ��������, ������������� �������� ����������� �������
        /// </summary>
        public String name_tp;

        /// <summary>
        /// ���� � ����� ��������� ������ ��������
        /// </summary>
        public string protocol_log;

        /// <summary>
        /// ���� � ����� ��������� ���������� ���������� - ������������� � ������ ������ ���� 
        /// �������� ��������� ���������� � �������� ������ �� ������ ������ (������� � ������)
        /// </summary>
        public string debug_log;

        /// <summary>
        /// ������� ������������� ��������� (�������������) ���� � �������
        /// </summary>
        public bool timeset;

        /// <summary>
        /// ������� ������������� ���������� (true)/���������(false) ��������. 
        /// �������� �� ��������� null, ���� ��������������� �������� ������ �� ����� 
        /// </summary>
        public bool? relayOff = null;

        /// <summary>
        /// ������� ������������� ���������(true)/����������(false) �������� ���������� ���� ��  
        /// ���������� ������ ��������. �������� �� ��������� null, ���� ��������������� �������� 
        /// ������ �� ����� � ������������ �������� �������� � ��������
        /// </summary>
        public bool? relayPowerLimitOn = null;

        /// <summary>
        /// �������� ������ �������� (����). 
        /// �������� �� ��������� null, ���� ��������������� �������� � ���������� ������ 
        /// �� ������ � ������������ ��������, �������� � ��������  
        /// </summary>
        public int? relayPowerLimitValue = null;

        internal DeviceRequest requestParams = null;

        internal bool
            readNetQuality = false,
            readMeterLogs = false;

        public BaseDevice(String requestFileName, int meter)
        {
            valid = false;

            if (!File.Exists(requestFileName))
                return;

            requestFile = requestFileName;
            StringBuilder tmp = new StringBuilder(256, 256);

            com_port = (UInt16)CIniFile.GetPrivateProfileInt("MAIN_SECTION", "COM", 0, requestFile);

            if (com_port > 1000) // ����������� TCP/IP 
            {
                CIniFile.GetPrivateProfileString("MAIN_SECTION", "Ip", "127.0.0.1", tmp, 256, requestFile);
                _channelAddr = tmp.ToString();
                tmp.Length = 0;
            }
            else
                _channelAddr = "";

            // ������� ������ ������ ����������� ��������� ���������
            taskID = CIniFile.GetPrivateProfileInt("MAIN_SECTION", "TaskID", 0, requestFile);

            String meter_section = "Counter_" + meter.ToString();
            serialNumber = CIniFile.GetPrivateProfileInt(meter_section, "NCount", 0, requestFile);
            if (serialNumber == 0)
                return;

            netAdr = CIniFile.GetPrivateProfileInt(meter_section, "NetAdr", 0, requestFile);
            if (netAdr == 0)
                netAdr = serialNumber;

            // ������� ������ ���������� �������� ����
            int param = CIniFile.GetPrivateProfileInt(meter_section, "NetQuality", 0, requestFile);
            readNetQuality = param != 0;

            // ������� ������ �������� ��������
            param = CIniFile.GetPrivateProfileInt(meter_section, "MeterLogs", 0, requestFile);
            readMeterLogs = param != 0;

            manufacturerID = tmp.ToString();
            tmp.Length = 0;

            password = (UInt32)CIniFile.GetPrivateProfileInt(meter_section, "Passw", 0, requestFile);
            if (password != 0)
                accessLevel = AccessLevels.Protected;

            tmp.Length = 0;
            CIniFile.GetPrivateProfileString(meter_section, "Name_TP", "", tmp, 256, requestFile);
            name_tp = tmp.ToString();


            CIniFile.GetPrivateProfileString("MAIN_SECTION", "Protocol", @".\base\FlogModem.txt", tmp, 256, requestFile);
            protocol_log = tmp.ToString();

            // ������ (����������) �������� ������ ��������
            CIniFile.GetPrivateProfileString("MAIN_SECTION", "LogMTr", "", tmp, 256, requestFile);
            debug_log = tmp.ToString();

            // ��������� �������� "�������" (�� ������� � �������) ������������ ����
            int relayValue = CIniFile.GetPrivateProfileInt("RELE", "TurnOn", -1, requestFile);
            if (relayValue >= 0)
                // � ������ ����� ������� ���������� ����, ����� ��������� null
                relayOff = (relayValue == 3) ? true : false;

            // �������������� ���������� ���� �� ������ ��������
            int relayLimitOnEnabled = (int)CIniFile.GetPrivateProfileInt("Action", "Type", -1, requestFile);
            // ���� � ������ ����� ������� ���������� ���� �� ������ ��������, ������������� ��� ������� � �������� ������
            if (relayLimitOnEnabled >= 0)
            {
                relayPowerLimitOn = (relayLimitOnEnabled == 0) ? false : true;
                if (!relayPowerLimitOn.Value)
                    // ����������� ������ ���������� ���������� ���� �� ������ - ���� �������� ������ ��������� �������
                    relayPowerLimitValue = -1;
                else
                    relayPowerLimitValue = (int)CIniFile.GetPrivateProfileInt("Action", "Value", 0, requestFile);
            }

            // �������� �� ������������� ������������� ������� �������� � ��������
            timeset = (CIniFile.GetPrivateProfileInt(meter_section, "TimeSet", 0, requestFile) > 0);

            if (
                serialNumber != 0 && name_tp.Length != 0
                )
                valid = true;
            else
                valid = false;

        }

        public bool HasRele()
        {
            if ((modification == null) || (modification.Length == 0))
                return false;
            else
            {
                return false;
            }
        }

        internal static EnergyType SetPowerType(String str, byte[] pt)
        {
            EnergyType et = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == 'A')
                {
                    pt[0] = 1; et |= EnergyType.ActivePlus;
                }
                if (str[i] == 'B')
                {
                    pt[1] = 1; et |= EnergyType.ActiveMinus;
                }
                if (str[i] == 'C')
                {
                    pt[2] = 1; et |= EnergyType.ReactivePlus;
                }
                if (str[i] == 'D')
                {
                    pt[3] = 1; et |= EnergyType.ReactiveMinus;
                }
            }
            return et;
        }
    }
}
