using Gurux.Common;
using Gurux.DLMS;
using Gurux.DLMS.Enums;
using NLog;
using System;

namespace ZIP.DLMS
{
    public class GXLogWriter
    {
        /// <summary>
        /// Received trace data.
        /// </summary>
        private static GXByteBuffer receivedTraceData = new GXByteBuffer();
        private static GXDLMSTranslator translator = new GXDLMSTranslator(TranslatorOutputType.SimpleXml);
        private static Logger Log = null;

        static GXLogWriter()
        {
            //Log = LogManager.GetCurrentClassLogger();
            //translator = new GXDLMSTranslator(TranslatorOutputType.SimpleXml);
            //receivedTraceData = new GXByteBuffer();
        }

        //static public string LogPath
        //{
        //    get
        //    {
        //        string path = string.Empty;
        //        if (Environment.OSVersion.Platform == PlatformID.Unix)
        //        {
        //            path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        //            path = System.IO.Path.Combine(path, ".Gurux");
        //        }
        //        else
        //        {
        //            //Vista: C:\ProgramData
        //            //XP: c:\Program Files\Common Files
        //            //XP = 5.1 & Vista = 6.0
        //            if (Environment.OSVersion.Version.Major >= 6)
        //            {
        //                path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        //            }
        //            else
        //            {
        //                path = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);
        //            }
        //            path = System.IO.Path.Combine(path, "Gurux");
        //        }
        //        path = System.IO.Path.Combine(path, "GXDLMSDirector");
        //        path = System.IO.Path.Combine(path, "GXDLMSDirector.log");
        //        return path;
        //    }
        //}

        /// <summary>
        /// Расширенный режим логгирования DLMS (bitwise)
        /// 0x00 - Logging off.
        /// 0x01 - Show data as hex.
        /// 0x02 - Show data as xml (включен XML-транслятор пакетов)
        /// </summary>
        public static int LogMode
        {
            get;
            set;
        } = 1;

        public static void SetLog(Logger logger)
        {
            if (logger == null)
                Log = LogManager.GetCurrentClassLogger();
            else
                Log = logger;
        }

        public static bool UseComments
        {
            get
            {
                return translator.Comments;
            }
            set
            {
                translator.Comments = value;
            }
        }

        private static void AddLog(Logger logger, string msg, LogLevel logLevel = null)
        {
            if (logger == null || logLevel == NLog.LogLevel.Off)
                return;
            if(logLevel == null || logLevel == NLog.LogLevel.Trace)
                logger.Trace(msg);
            else
                if (logLevel == LogLevel.Debug)
                logger.Debug(msg);
            else
                if (logLevel == LogLevel.Info)
                logger.Info(msg);
            else
                if (logLevel == LogLevel.Warn)
                logger.Warn(msg);
            else
                if (logLevel == LogLevel.Error)
                logger.Error(msg);
            else
                if (logLevel == LogLevel.Fatal)
                logger.Fatal(msg);
        }

        /// <summary>
        /// Append data to log file.
        /// </summary>
        public static void WriteLog(string data, LogLevel logLevel = null, Logger logger = null)
        {
            WriteLog(data, Properties.Settings.Default.LogTime, logLevel, logger);
        }

        public static void WriteLog(string text, byte[] value, LogLevel logLevel = null, Logger logger = null)
        {
            if (LogMode == 0)
                return;

            string str;
            if (Properties.Settings.Default.LogTime)
            {
                str = DateTime.Now.ToLongTimeString() + " " + text; 
            }
            else
            {
                str = text;
            }

            //Show data as hex.
            if ((LogMode & 1) != 0)
            {
                if (value != null)
                {
                    str += Environment.NewLine + GXCommon.ToHex(value, true);
                }
            }

            //Show data as xml.
            if (value != null && (LogMode & 2) != 0)
            {
                receivedTraceData.Set(value);
                try
                {
                    GXByteBuffer pdu = new GXByteBuffer();
                    InterfaceType type = GXDLMSTranslator.GetDlmsFraming(receivedTraceData);
                    while (translator.FindNextFrame(receivedTraceData, pdu, type))
                    {
                        str += (Environment.NewLine + translator.MessageToXml(receivedTraceData));
                        receivedTraceData.Trim();
                    }
                }
                catch (Exception)
                {
                    receivedTraceData.Clear();
                }
            }

            WriteLog(str, logLevel, logger);
        }

        /// <summary>
        /// Append data to log file.
        /// </summary>
        public static void WriteLog(string data, bool addTime, LogLevel logLevel = null, Logger logger = null)
        {
            if (data == null)
            {
                return;
            }
           // string msg = data.Replace("\r", Environment.NewLine).Replace("\n", Environment.NewLine);
            if (addTime)
            {
                if(logger != null)
                {
                    AddLog(logger, DateTime.Now.ToString("HH:mm:ss") + " " + data, logLevel);
                }
                AddLog(Log, DateTime.Now.ToString("HH:mm:ss") + " " + data, logLevel);
            }
            else
            {
                if (logger != null)
                {
                    AddLog(logger, data, logLevel);
                }
                AddLog(Log, data, logLevel);
            }
        }
    }
}
