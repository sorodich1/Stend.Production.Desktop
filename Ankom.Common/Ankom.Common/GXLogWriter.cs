using Ankom.Common.GxFileInfo;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Ankom.Common.GxLogWriter
{
    public class GXLogWriter
    {
        static bool isprogress = false;
        static string tmplog = "";
        static string tmpfilelog = "";

        static string m_LogPath = @".\FlogModem.txt";
        static string m_ByteLogPath = @".\ce272x_debug.txt";
        static System.Threading.Mutex DrMutex = new System.Threading.Mutex(false, "ASKUE_IO");

        public static string ByteLogPath
        {
            get
            {
                return m_ByteLogPath;
            }
            set
            {
                m_ByteLogPath = value;
            }
        }

        public static string LogPath
        {

            get
            {
                return m_LogPath;
            }
            set
            {
                m_LogPath = value;
            }
        }

        const UInt16 TitleDividerSize = 48;
        public static string GetTitleDivider(bool thin = false, int DividerSize = TitleDividerSize)
        {
            return new String(((thin) ? '-' : '='), DividerSize);
        }

        public static void WriteTitleDivider(bool thin = true, int DividerSize = TitleDividerSize)
        {
            WriteLog(GetTitleDivider(thin, DividerSize));
        }

        public static void WriteValue(string valueDescription, object Value, string Measurment = "", int descSize = 25)
        {
            WriteLog(valueDescription.PadRight(descSize) + ": " + Value.ToString() + ((Measurment != "") ? " " + Measurment : ""));
        }

        /// <summary>
        /// выводит сообщение, оформленное в виде заголовка
        /// </summary>
        /// <param name="message">текст сообщения</param>
        /// <param name="thin">толщина разделителя рамки</param>
        /// <param name="hasHeader">наличие верхней рамки</param>
        /// <param name="hasFooter">наличие нижней рамки</param>
        /// <param name="addDateTime">признак добавления временной метки перед сообщением</param>
        public static void WriteAsTitle(string message = "", bool thin = false, bool hasHeader = true, bool hasFooter = true, bool addDateTime = true)
        {
            if (hasHeader) WriteLog(GetTitleDivider(thin), addDateTime);
            if (message != "")
            {
                WriteLog(message, addDateTime);
                if (hasFooter) WriteLog(GetTitleDivider(thin), addDateTime);
            }
        }

        public static void WriteProgress(int openclose = 0)
        {
            if (!File.Exists(m_LogPath))
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(m_LogPath);
                file.Close();
            }

            if (File.Exists(m_LogPath))
            {
                DrMutex.WaitOne();

                using (StreamWriter file = new System.IO.StreamWriter(m_LogPath, true, System.Text.Encoding.GetEncoding(1251)))
                {
                    if (openclose < 0)
                    {
                        file.Write("[");
                        System.Console.Write(DateTime.Now.ToLongTimeString() + " [");
                        isprogress = true;
                    }

                    if (openclose == 0)
                    {
                        file.Write("=");
                        System.Console.Write("=");
                    }

                    if (openclose > 0)
                    {
                        file.Write("]");
                        System.Console.Write("]");
                        isprogress = false;

                    }

                    file.Flush();
                    file.Close();

                    //Sleep(100);
                }
                DrMutex.ReleaseMutex();
                if (openclose > 0)
                {
                    WriteLog("");
                }
            }
        }


        /// <summary>
        /// Append data to log file.
        /// </summary>
        /// 
        public static void WriteLog(string logMessage, bool outputDatetime = true)
        {
            if (logMessage == null)
            {
                return;
            }
            string log = "";
            if (logMessage != "")
                log = ((outputDatetime) ? DateTime.Now.ToLongTimeString() + " " : "") + logMessage.Replace("\r", "<CR>").Replace("\n", "<LF>");

            if (isprogress)
                tmplog += "\n" + log;
            else
            {
                if (tmplog.Length > 0)
                {
                    System.Console.WriteLine(tmplog);
                    tmplog = "";
                }
                System.Console.WriteLine(log);
            }

            System.Diagnostics.Trace.WriteLine(((outputDatetime) ? DateTime.Now.ToLongTimeString() + " " : "") + logMessage.Replace("\r", "<CR>").Replace("\n", "<LF>"));

            if (!File.Exists(m_LogPath))
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(m_LogPath);
                file.Close();
            }

            if (File.Exists(m_LogPath))
            {
                DrMutex.WaitOne();

                using (StreamWriter file = new System.IO.StreamWriter(m_LogPath, true, System.Text.Encoding.GetEncoding(1251)))
                {
                    if (isprogress)
                        tmpfilelog += "\n" + logMessage;
                    else
                    {
                        if (tmpfilelog.Length > 0)
                        {
                            file.WriteLine(tmpfilelog);
                            tmpfilelog = "";
                        }
                        file.WriteLine(logMessage);
                    }
                    file.Flush();
                    file.Close();

                    //System.Threading.Thread.Sleep(300);
                }
                DrMutex.ReleaseMutex();
            }

        }

        public static void WriteLog(string text, byte[] value, bool CRLF = true)
        {
            string str = DateTime.Now.ToLongTimeString() + " " + text;
            if (value != null)
            {
                int start = 0, data_length = value.Length;
                byte[] copy = new byte[data_length];
                value.CopyTo(copy, 0);
                for (int i = 0; i < copy.Length; i++)
                    if (copy[i] < 0x20)
                        copy[i] = (byte)'.';
                while (data_length > 0)
                {
                    int block_length = Math.Min(16, data_length);
                    str += (((CRLF) ? "\r\n" : "") + BitConverter.ToString(value, start, block_length).Replace('-', ' ')).PadRight(56) +
                        Encoding.ASCII.GetString(copy, start, block_length);
                    start += 16;
                    data_length -= 16;
                }
            }
            //System.Console.WriteLine(str);
            System.Diagnostics.Trace.WriteLine(str);
        }

        public static void TraceLog(string text, string value, bool CRLF = true)
        {
            string str = DateTime.Now.ToLongTimeString() + " " + text;
            if (value != null)
            {
                str += (((CRLF) ? "\r\n" : "") + value);
            }
            //System.Console.WriteLine(str);
            System.Diagnostics.Trace.WriteLine(str);
        }

        /// <summary>
        /// Clear log file.
        /// </summary>
        public static void ClearLog()
        {
            foreach (TraceListener it in System.Diagnostics.Trace.Listeners)
            {
                if (it is TextWriterTraceListener)
                {
                    //Flush and close the output.
                    Trace.Flush();
                    it.Flush();
                    if (((TextWriterTraceListener)it).Writer != null)
                    {
                        ((TextWriterTraceListener)it).Writer.Close();
                    }
                    ((TextWriterTraceListener)it).Writer = new StreamWriter(GXLogWriter.LogPath);
                    GXFileInfo.UpdateFileSecurity(GXLogWriter.LogPath);
                    break;
                }
            }
            Debug.WriteLine("Log created " + DateTime.Now.ToLongTimeString());
        }
    }

    public class CommandRunProgress
    {
        int value = 0;
        int max = 1;
        bool started = false;
        bool finished = true;

        public CommandRunProgress()
        {
            Start();
        }

        public void Finish()
        {
            if (!finished)
            {
                //OnProgress(null, "", max, max);
                for (int i = value; i < 20; i++)
                    GXLogWriter.WriteProgress();

                GXLogWriter.WriteProgress(1);
                finished = true;
            }
        }

        private void Start()
        {
            Finish();

            value = 0;
            max = 1;
            finished = true;
            started = false;
        }

        public void OnProgress(object sender, string description, int current, int maximium)
        {
            //            if (maximium < 10)
            //                return;
            if (maximium > max)
            {
                max = maximium;
            }
            if (current > max)
            {
                max = current;
            }

            if (!started)
            {
                GXLogWriter.WriteProgress(-1);
                started = true;
                finished = false;
            }

            if (current == 0) //start
            {
                value = 0;
            }

            while ((int)(current / (max / 20.0)) > value)
            {
                GXLogWriter.WriteProgress();
                value++;
            }

            if (current >= max) //finish
            {
                value = 0;
                Start();
            }

        }
    }

}
