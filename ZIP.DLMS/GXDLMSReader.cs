﻿using Gurux.Common;
using Gurux.DLMS;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;
using Gurux.DLMS.Secure;
using Gurux.Serial;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace ZIP.DLMS.Reader
{
    public class GXDLMSReader
    {
        Logger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Wait time in ms.
        /// </summary>
        public int WaitTime = 5000;
        /// <summary>
        /// Retry count.
        /// </summary>
        public int RetryCount = 3;
        IGXMedia Media;
        TraceLevel Trace;
        GXDLMSSecureClient Client;
        GXDLMSDevice Device;

        // Invocation counter (frame counter).
        string InvocationCounter = null;

        public Logger Log { get => _log; set => _log = value; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="client">DLMS Client</param>
        /// <param name="media">Media</param>
        /// <param name="trace">Trace level</param>
        /// <param name="invocationCounter">Logical name of invocation counter</param>
        public GXDLMSReader(GXDLMSSecureClient client, IGXMedia media, TraceLevel trace, string invocationCounter)
        {
            Trace = trace;
            Media = media;
            Client = client;
            InvocationCounter = invocationCounter;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="device">DLMS Client</param>
        /// <param name="media">Media</param>
        /// <param name="trace">Trace level</param>
        /// <param name="invocationCounter">Logical name of invocation counter</param>
        public GXDLMSReader(GXDLMSDevice device, TraceLevel traceLevel)
        {
            Device = device;
            Trace = traceLevel;
            Media = device.Media;
            Client = new GXDLMSSecureClient(
                device.UseLogicalNameReferencing,
                device.ClientAddress,
                device.PhysicalAddress,
                device.Authentication,
                device.Password,
                device.InterfaceType);
            InvocationCounter = device.InvocationCounter.ToString();// invocationCounter;
        }

        /// <summary>
        /// (Re)read all data from the meter.
        /// </summary>
        public void ReadAll(string outputFile)
        {
            try
            {
                InitializeConnection();
                if (GetAssociationView(outputFile))
                {
                    GetScalersAndUnits();
                    GetProfileGenericColumns();
                }
                GetReadOut();
                GetProfileGenerics();
                if (outputFile != null)
                {
                    try
                    {
                        Client.Objects.Save(outputFile, new GXXmlWriterSettings() { UseMeterTime = true, IgnoreDefaultValues = false });
                    }
                    catch (Exception)
                    {
                        //It's OK if this fails.
                    }
                }
            }
            finally
            {
                Close();
            }
        }

        /// <summary>
        /// Send SNRM Request to the meter.
        /// </summary>
        public void SNRMRequest()
        {
            GXReplyData reply = new GXReplyData();
            byte[] data;
            data = Client.SNRMRequest();
            if (data != null)
            {
                if (Trace > TraceLevel.Info)
                {
                    Log.Info("Отправка запроса SNRM: " + GXCommon.ToHex(data, true));
                }
                ReadDataBlock(data, reply);
                if (Trace == TraceLevel.Verbose)
                {
                    Log.Info("Разбор UA-ответа: " + reply.ToString());
                }
                //Has server accepted client.
                Client.ParseUAResponse(reply.Data);
                if (Trace > TraceLevel.Info)
                {
                    Log.Info("Разбор UA-ответа успешно завершен.");
                }
            }
        }

        /// <summary>
        /// Send AARQ Request to the meter.
        /// </summary>
        public void AarqRequest()
        {
            GXReplyData reply = new GXReplyData();
            //Generate AARQ request.
            //Split requests to multiple packets if needed.
            //If password is used all data might not fit to one packet.
            foreach (byte[] it in Client.AARQRequest())
            {
                if (Trace > TraceLevel.Info)
                {
                    Log.Info("Отправка запроса AARQ: ", GXCommon.ToHex(it, true));
                }
                reply.Clear();
                ReadDataBlock(it, reply);
            }
            if (Trace > TraceLevel.Info)
            {
                Log.Info("Разбор AARE-ответа: " + reply.ToString());
            }
            //Parse reply.
            Client.ParseAAREResponse(reply.Data);
            reply.Clear();
            //Get challenge Is HLS authentication is used.
            if (Client.Authentication > Authentication.Low)
            {
                foreach (byte[] it in Client.GetApplicationAssociationRequest())
                {
                    reply.Clear();
                    ReadDataBlock(it, reply);
                }
                Client.ParseApplicationAssociationResponse(reply.Data);
            }
            if (Trace > TraceLevel.Info)
            {
                Log.Info("Разбор AARE-ответа успешно завершен.");
            }
        }

        /// <summary>
        /// Read Invocation counter (frame counter) from the meter and update it.
        /// </summary>
        private void UpdateFrameCounter()
        {
            //Read frame counter if GeneralProtection is used.
            if (!string.IsNullOrEmpty(InvocationCounter) && Client.Ciphering != null && Client.Ciphering.Security != (byte)Security.None)
            {
                InitializeOpticalHead();
                byte[] data;
                GXReplyData reply = new GXReplyData();
                Client.ProposedConformance |= Conformance.GeneralProtection;
                int add = Client.ClientAddress;
                Authentication auth = Client.Authentication;
                Security security = Client.Ciphering.Security;
                byte[] challenge = Client.CtoSChallenge;
                try
                {
                    Client.ClientAddress = 16;
                    Client.Authentication = Authentication.None;
                    Client.Ciphering.Security = (byte)Security.None;
                    data = Client.SNRMRequest();
                    if (data != null)
                    {
                        if (Trace > TraceLevel.Info)
                        {
                            Log.Info("Отправка запроса SNRM: " + GXCommon.ToHex(data, true));
                        }
                        ReadDataBlock(data, reply);
                        if (Trace == TraceLevel.Verbose)
                        {
                            Log.Info("Разбор UA-ответа: " + reply.ToString());
                        }
                        //Has server accepted client.
                        Client.ParseUAResponse(reply.Data);
                        if (Trace > TraceLevel.Info)
                        {
                            Log.Info("Разбор UA-ответа успешно завершен.");
                        }
                    }
                    //Generate AARQ request.
                    //Split requests to multiple packets if needed.
                    //If password is used all data might not fit to one packet.
                    foreach (byte[] it in Client.AARQRequest())
                    {
                        if (Trace > TraceLevel.Info)
                        {
                            Log.Info("Отправка запроса AARQ: ", GXCommon.ToHex(it, true));
                        }
                        reply.Clear();
                        ReadDataBlock(it, reply);
                    }
                    if (Trace > TraceLevel.Info)
                    {
                        Log.Info("Разбор AARE-ответа: " + reply.ToString());
                    }
                    try
                    {
                        //Parse reply.
                        Client.ParseAAREResponse(reply.Data);
                        reply.Clear();
                        GXDLMSData d = new GXDLMSData(InvocationCounter);
                        Read(d, 2);
                        Client.Ciphering.InvocationCounter = 1 + Convert.ToUInt32(d.Value);
                        Log.Info("Счетчик вызовов: " + Convert.ToString(Client.Ciphering.InvocationCounter));
                        reply.Clear();
                        Disconnect();
                    }
                    catch (Exception Ex)
                    {
                        Disconnect();
                        throw Ex;
                    }
                }
                finally
                {
                    Client.ClientAddress = add;
                    Client.Authentication = auth;
                    Client.Ciphering.Security = security;
                    Client.CtoSChallenge = challenge;
                }
            }
        }

        /// <summary>
        /// Send IEC disconnect message.
        /// </summary>
        void DiscIEC()
        {
            ReceiveParameters<string> p = new ReceiveParameters<string>()
            {
                AllData = false,
                Eop = (byte)0x0A,
                WaitTime = WaitTime * 1000
            };
            string data = (char)0x01 + "B0" + (char)0x03 + "\r\n";
            Media.Send(data, null);
            p.Eop = "\n";
            p.AllData = true;
            p.Count = 1;

            Media.Receive(p);
        }

        /// <summary>
        /// Initialize optical head.
        /// </summary>
        void InitializeOpticalHead()
        {
            if (Client.InterfaceType != InterfaceType.HdlcWithModeE)
            {
                return;
            }
            GXSerial serial = Media as GXSerial;
            byte Terminator = 0x0A;
            Media.Open();
            //Some meters need a little break.
            Thread.Sleep(1000);
            //Query device information.
            string data = "/?!\r\n";
            if (Trace > TraceLevel.Info)
            {
                Log.Info("IEC Sending: " + data);
            }
            ReceiveParameters<string> p = new ReceiveParameters<string>()
            {
                AllData = false,
                Eop = Terminator,
                WaitTime = WaitTime * 1000
            };
            lock (Media.Synchronous)
            {
                Media.Send(data, null);
                if (!Media.Receive(p))
                {
                    //Try to move away from mode E.
                    try
                    {
                        Disconnect();
                    }
                    catch (Exception)
                    {
                    }
                    DiscIEC();
                    string str = "Не удалось получить ответ от устройства в заданное время.";
                    if (Trace > TraceLevel.Info)
                    {
                        Log.Info(str);
                    }
                    Media.Send(data, null);
                    if (!Media.Receive(p))
                    {
                        throw new Exception(str);
                    }
                }
                //If echo is used.
                if (p.Reply == data)
                {
                    p.Reply = null;
                    if (!Media.Receive(p))
                    {
                        //Try to move away from mode E.
                        GXReplyData reply = new GXReplyData();
                        Disconnect();
                        if (serial != null)
                        {
                            DiscIEC();
                            serial.DtrEnable = serial.RtsEnable = false;
                            serial.BaudRate = 9600;
                            serial.DtrEnable = serial.RtsEnable = true;
                            DiscIEC();
                        }
                        data = "Не удалось получить ответ от устройства в заданное время.";
                        if (Trace > TraceLevel.Info)
                        {
                            Log.Info(data);
                        }
                        throw new Exception(data);
                    }
                }
            }
            if (Trace > TraceLevel.Info)
            {
                Log.Info("IEC received: " + p.Reply);
            }
            int pos = 0;
            //With some meters there might be some extra invalid chars. Remove them.
            while (pos < p.Reply.Length && p.Reply[pos] != '/')
            {
                ++pos;
            }
            if (p.Reply[pos] != '/')
            {
                p.WaitTime = 100;
                Media.Receive(p);
                DiscIEC();
                throw new Exception("Invalid responce.");
            }
            string manufactureID = p.Reply.Substring(1 + pos, 3);
            char baudrate = p.Reply[4 + pos];
            int BaudRate = 0;
            switch (baudrate)
            {
                case '0':
                    BaudRate = 300;
                    break;
                case '1':
                    BaudRate = 600;
                    break;
                case '2':
                    BaudRate = 1200;
                    break;
                case '3':
                    BaudRate = 2400;
                    break;
                case '4':
                    BaudRate = 4800;
                    break;
                case '5':
                    BaudRate = 9600;
                    break;
                case '6':
                    BaudRate = 19200;
                    break;
                default:
                    throw new Exception("Unknown baud rate.");
            }
            if (Trace > TraceLevel.Info)
            {
                Log.Info("BaudRate is: " + BaudRate.ToString());
            }
            //Send ACK
            //Send Protocol control character
            // "2" HDLC protocol procedure (Mode E)
            byte controlCharacter = (byte)'2';
            //Send Baud rate character
            //Mode control character
            byte ModeControlCharacter = (byte)'2';
            //"2" //(HDLC protocol procedure) (Binary mode)
            //Set mode E.
            byte[] arr = new byte[] { 0x06, controlCharacter, (byte)baudrate, ModeControlCharacter, 13, 10 };
            if (Trace > TraceLevel.Info)
            {
                Log.Info("Moving to mode E.", arr);
            }
            lock (Media.Synchronous)
            {
                p.Reply = null;
                Media.Send(arr, null);
                //Some meters need this sleep. Do not remove.
                Thread.Sleep(200);
                p.WaitTime = 2000;
                //Note! All meters do not echo this.
                Media.Receive(p);
                if (p.Reply != null)
                {
                    if (Trace > TraceLevel.Info)
                    {
                        Log.Info("Received: " + p.Reply);
                    }
                }
                serial.BaudRate = BaudRate;
                serial.DataBits = 8;
                serial.Parity = Parity.None;
                serial.StopBits = StopBits.One;
                //Some meters need this sleep. Do not remove.
                Thread.Sleep(800);
            }
        }

        /// <summary>
        /// Initialize connection to the meter.
        /// </summary>
        public void InitializeConnection()
        {
            if (Device == null)
            {
                Log.Info("Standard: " + Client.Standard);
                if (Client.Ciphering.Security != (byte)Security.None)
                {
                    Log.Info("Security: " + Client.Ciphering.Security);
                    Log.Info("System title: " + GXCommon.ToHex(Client.Ciphering.SystemTitle, true));
                    Log.Info("Authentication key: " + GXCommon.ToHex(Client.Ciphering.AuthenticationKey, true));
                    Log.Info("Block cipher key: " + GXCommon.ToHex(Client.Ciphering.BlockCipherKey, true));
                    if (Client.Ciphering.DedicatedKey != null)
                    {
                        Log.Info("Dedicated key: " + GXCommon.ToHex(Client.Ciphering.DedicatedKey, true));
                    }
                }
                UpdateFrameCounter();
                InitializeOpticalHead();
                GXReplyData reply = new GXReplyData();
                SNRMRequest();
                //Generate AARQ request.
                //Split requests to multiple packets if needed.
                //If password is used all data might not fit to one packet.
                foreach (byte[] it in Client.AARQRequest())
                {
                    if (Trace > TraceLevel.Info)
                    {
                        Log.Info("Отправка запроса AARQ: ", GXCommon.ToHex(it, true));
                    }
                    reply.Clear();
                    ReadDataBlock(it, reply);
                }
                if (Trace > TraceLevel.Info)
                {
                    Log.Info("Разбор AARE-ответа: " + reply.ToString());
                }
                //Parse reply.
                Client.ParseAAREResponse(reply.Data);
                reply.Clear();
                //Get challenge Is HLS authentication is used.
                if (Client.IsAuthenticationRequired)
                {
                    foreach (byte[] it in Client.GetApplicationAssociationRequest())
                    {
                        reply.Clear();
                        ReadDataBlock(it, reply);
                    }
                    Client.ParseApplicationAssociationResponse(reply.Data);
                }
                if (Trace > TraceLevel.Info)
                {
                    Log.Info("Разбор AARE-ответа успешно завершен.");
                }
            }
            else
            {
                if (Device.Status <= DeviceState.Initialized)
                {
                    Device.InitializeConnection();
                }
            }
        }

        /// <summary>
        /// This method is used to update meter firmware.
        /// </summary>
        /// <param name="target"></param>
        public void ImageUpdate(GXDLMSImageTransfer target, byte[] identification, byte[] data)
        {
            //Check that image transfer ia enabled.
            GXReplyData reply = new GXReplyData();
            ReadDataBlock(Client.Read(target, 5), reply);
            Client.UpdateValue(target, 5, reply.Value);
            if (!target.ImageTransferEnabled)
            {
                throw new Exception("Передача образов не включена");
            }

            //Step 1: Read image block size.
            ReadDataBlock(Client.Read(target, 2), reply);
            Client.UpdateValue(target, 2, reply.Value);

            // Step 2: Initiate the Image transfer process.
            ReadDataBlock(target.ImageTransferInitiate(Client, identification, data.Length), reply);

            // Step 3: Transfers ImageBlocks.
            int imageBlockCount;
            ReadDataBlock(target.ImageBlockTransfer(Client, data, out imageBlockCount), reply);

            //Step 4: Check the completeness of the Image.
            ReadDataBlock(Client.Read(target, 3), reply);
            Client.UpdateValue(target, 3, reply.Value);

            // Step 5: The Image is verified;
            ReadDataBlock(target.ImageVerify(Client), reply);
            // Step 6: Before activation, the Image is checked;

            //Get list to images to activate.
            ReadDataBlock(Client.Read(target, 7), reply);
            Client.UpdateValue(target, 7, reply.Value);
            bool bFound = false;
            foreach (GXDLMSImageActivateInfo it in target.ImageActivateInfo)
            {
                if (GXCommon.EqualBytes(it.Identification, identification))
                {
                    bFound = true;
                    break;
                }
            }

            //Read image transfer status.
            ReadDataBlock(Client.Read(target, 6), reply);
            Client.UpdateValue(target, 6, reply.Value);
            if (target.ImageTransferStatus != Gurux.DLMS.Objects.Enums.ImageTransferStatus.VerificationSuccessful)
            {
                throw new Exception("Статус передача образа: " + target.ImageTransferStatus.ToString());
            }

            if (!bFound)
            {
                throw new Exception("Образ не найден.");
            }

            //Step 7: Activate image.
            ReadDataBlock(target.ImageActivate(Client), reply);
        }
        /// <summary>
        /// Read association view.
        /// </summary>
        public bool GetAssociationView(string outputFile)
        {
            if (outputFile != null)
            {
                //Save Association view to the cache so it is not needed to retrieve every time.
                if (File.Exists(outputFile))
                {
                    try
                    {
                        Client.Objects.Clear();
                        Client.Objects.AddRange(GXDLMSObjectCollection.Load(outputFile));
                        return false;
                    }
                    catch (Exception)
                    {
                        if (File.Exists(outputFile))
                        {
                            File.Delete(outputFile);
                        }
                    }
                }
            }
            GXReplyData reply = new GXReplyData();
            ReadDataBlock(Client.GetObjectsRequest(), reply);
            Client.ParseObjects(reply.Data, true);
            //Access rights must read differently when short Name referencing is used.
            if (!Client.UseLogicalNameReferencing)
            {
                GXDLMSAssociationShortName sn = (GXDLMSAssociationShortName)Client.Objects.FindBySN(0xFA00);
                if (sn != null && sn.Version > 0)
                {
                    Read(sn, 3);
                }
            }
            if (outputFile != null)
            {
                try
                {
                    Client.Objects.Save(outputFile, new GXXmlWriterSettings() { Values = false });
                }
                catch (Exception)
                {
                    //It's OK if this fails.
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Read scalers and units.
        /// </summary>
        public void GetScalersAndUnits()
        {
            GXDLMSObjectCollection objs = Client.Objects.GetObjects(new ObjectType[] { ObjectType.Register, ObjectType.ExtendedRegister, ObjectType.DemandRegister });
            //If trace is info.
            if (Trace > TraceLevel.Warning)
            {
                Log.Info("Read scalers and units from the device.");
            }
            if ((Client.NegotiatedConformance & Gurux.DLMS.Enums.Conformance.MultipleReferences) != 0)
            {
                List<KeyValuePair<GXDLMSObject, int>> list = new List<KeyValuePair<GXDLMSObject, int>>();
                foreach (GXDLMSObject it in objs)
                {
                    if ((it is GXDLMSRegister || it is GXDLMSExtendedRegister) && (it.GetAccess(3) & AccessMode.Read) != 0)
                    {
                        list.Add(new KeyValuePair<GXDLMSObject, int>(it, 3));
                    }
                    if (it is GXDLMSDemandRegister && (it.GetAccess(4) & AccessMode.Read) != 0)
                    {
                        list.Add(new KeyValuePair<GXDLMSObject, int>(it, 4));
                    }
                }
                if (list.Count != 0)
                {
                    try
                    {
                        ReadList(list);
                    }
                    catch (Exception)
                    {
                        Client.NegotiatedConformance &= ~Gurux.DLMS.Enums.Conformance.MultipleReferences;
                    }
                }
            }
            if ((Client.NegotiatedConformance & Gurux.DLMS.Enums.Conformance.MultipleReferences) == 0)
            {
                //Read values one by one.
                foreach (GXDLMSObject it in objs)
                {
                    try
                    {
                        if (it is GXDLMSRegister && (it.GetAccess(3) & AccessMode.Read) != 0)
                        {
                            Log.Info(it.Name);
                            Read(it, 3);
                        }
                        if (it is GXDLMSDemandRegister && (it.GetAccess(4) & AccessMode.Read) != 0)
                        {
                            Log.Info(it.Name);
                            Read(it, 4);
                        }
                    }
                    catch
                    {
                        //Actaric SL7000 can return error here. Continue reading.
                    }
                }
            }
        }

        /// <summary>
        /// Read profile generic columns.
        /// </summary>
        public void GetProfileGenericColumns()
        {
            //Read Profile Generic columns first.
            foreach (GXDLMSObject it in Client.Objects.GetObjects(ObjectType.ProfileGeneric))
            {
                try
                {
                    //If info.
                    if (Trace > TraceLevel.Warning)
                    {
                        Log.Info(it.LogicalName);
                    }
                    Read(it, 3);
                    //If info.
                    if (Trace > TraceLevel.Warning)
                    {
                        GXDLMSObject[] cols = (it as GXDLMSProfileGeneric).GetCaptureObject();
                        StringBuilder sb = new StringBuilder();
                        bool First = true;
                        foreach (GXDLMSObject col in cols)
                        {
                            if (!First)
                            {
                                sb.Append(" | ");
                            }
                            First = false;
                            sb.Append(col.Name);
                            sb.Append(" ");
                            sb.Append(col.Description);
                        }
                        Log.Info(sb.ToString());
                    }
                }
                catch (Exception)
                {
                    //Continue reading.
                }
            }
        }

        public void ShowValue(object val, int pos)
        {
            //If trace is info.
            if (Trace > TraceLevel.Warning)
            {
                //If data is array.
                if (val is byte[])
                {
                    val = GXCommon.ToHex((byte[])val, true);
                }
                else if (val is Array)
                {
                    string str = "";
                    for (int pos2 = 0; pos2 != (val as Array).Length; ++pos2)
                    {
                        if (str != "")
                        {
                            str += ", ";
                        }
                        if ((val as Array).GetValue(pos2) is byte[])
                        {
                            str += GXCommon.ToHex((byte[])(val as Array).GetValue(pos2), true);
                        }
                        else
                        {
                            str += (val as Array).GetValue(pos2).ToString();
                        }
                    }
                    val = str;
                }
                else if (val is System.Collections.IList)
                {
                    string str = "[";
                    bool empty = true;
                    foreach (object it2 in val as System.Collections.IList)
                    {
                        if (!empty)
                        {
                            str += ", ";
                        }
                        empty = false;
                        if (it2 is byte[])
                        {
                            str += GXCommon.ToHex((byte[])it2, true);
                        }
                        else
                        {
                            str += it2.ToString();
                        }
                    }
                    str += "]";
                    val = str;
                }
                Log.Info("Index: " + pos + " Value: " + val);
            }
        }

        public void GetProfileGenerics()
        {
            //Find profile generics register objects and read them.
            foreach (GXDLMSObject it in Client.Objects.GetObjects(ObjectType.ProfileGeneric))
            {
                //If trace is info.
                if (Trace > TraceLevel.Warning)
                {
                    Log.Info("-------- Reading " + it.GetType().Name + " " + it.Name + " " + it.Description);
                }
                long entriesInUse = -1;
                if ((it.GetAccess(7) & AccessMode.Read) != 0)
                {
                    entriesInUse = Convert.ToInt64(Read(it, 7));
                }
                long entries = -1;
                if ((it.GetAccess(8) & AccessMode.Read) != 0)
                {
                    entries = Convert.ToInt64(Read(it, 8));
                }
                //If trace is info.
                if (Trace > TraceLevel.Warning)
                {
                    Log.Info("Entries: " + entriesInUse + "/" + entries);
                }
                //If there are no columns or rows.
                if (entriesInUse == 0 || (it as GXDLMSProfileGeneric).CaptureObjects.Count == 0)
                {
                    continue;
                }
                //All meters are not supporting parameterized read.
                if ((Client.NegotiatedConformance & (Gurux.DLMS.Enums.Conformance.ParameterizedAccess | Gurux.DLMS.Enums.Conformance.SelectiveAccess)) != 0)
                {
                    try
                    {
                        //Read first row from Profile Generic.
                        object[] rows = ReadRowsByEntry(it as GXDLMSProfileGeneric, 1, 1);
                        //If trace is info.
                        if (Trace > TraceLevel.Warning)
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach (object[] row in rows)
                            {
                                foreach (object cell in row)
                                {
                                    if (cell is byte[])
                                    {
                                        sb.Append(GXCommon.ToHex((byte[])cell, true));
                                    }
                                    else
                                    {
                                        sb.Append(Convert.ToString(cell));
                                    }
                                    sb.Append(" | ");
                                }
                                sb.Append("\r\n");
                            }
                            Log.Info(sb.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error! Failed to read first row: " + ex.Message);
                        //Continue reading.
                    }
                }
                //All meters are not supporting parameterized read.
                if ((Client.NegotiatedConformance & (Conformance.ParameterizedAccess | Conformance.SelectiveAccess)) != 0)
                {
                    try
                    {
                        //Read last day from Profile Generic.

                        object[] rows = ReadRowsByRange(it as GXDLMSProfileGeneric, DateTime.Now.Date, DateTime.MaxValue);
                        //If trace is info.
                        if (Trace > TraceLevel.Warning)
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach (object[] row in rows)
                            {
                                foreach (object cell in row)
                                {
                                    if (cell is byte[])
                                    {
                                        sb.Append(GXCommon.ToHex((byte[])cell, true));
                                    }
                                    else
                                    {
                                        sb.Append(Convert.ToString(cell));
                                    }
                                    sb.Append(" | ");
                                }
                                sb.Append("\r\n");
                            }
                            Log.Info(sb.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error! Failed to read last day: " + ex.Message);
                        //Continue reading.
                    }
                }
            }
        }

        /// <summary>
        /// Read all objects from the meter.
        /// </summary>
        /// <remarks>
        /// It's not normal to read all data from the meter. This is just an example.
        /// </remarks>
        public void GetReadOut()
        {
            foreach (GXDLMSObject it in Client.Objects)
            {
                // Profile generics are read later because they are special cases.
                // (There might be so lots of data and we so not want waste time to read all the data.)
                if (it is GXDLMSProfileGeneric)
                {
                    continue;
                }
                if (!(it is IGXDLMSBase))
                {
                    //If interface is not implemented.
                    //Example manufacturer spesific interface.
                    if (Trace > TraceLevel.Error)
                    {
                        Log.Info("Unknown Interface: " + it.ObjectType.ToString());
                    }
                    continue;
                }
                if (Trace > TraceLevel.Warning)
                {
                    Log.Info("-------- Reading " + it.GetType().Name + " " + it.Name + " " + it.Description);
                }
                foreach (int pos in (it as IGXDLMSBase).GetAttributeIndexToRead(true))
                {
                    try
                    {
                        if ((it.GetAccess(pos) & AccessMode.Read) != 0)
                        {
                            object val = Read(it, pos);
                            ShowValue(val, pos);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error! " + it.GetType().Name + " " + it.Name + "Index: " + pos + " " + ex.Message);
                        Log.Error(ex.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Read DLMS Data from the device.
        /// </summary>
        /// <param name="data">Data to send.</param>
        /// <returns>Received data.</returns>
        public void ReadDLMSPacket(byte[] data, GXReplyData reply)
        {
            if (data == null && !reply.IsStreaming())
            {
                return;
            }
            GXReplyData notify = new GXReplyData();
            reply.Error = 0;
            object eop = (byte)0x7E;
            //In network connection terminator is not used.
            if (Client.InterfaceType != InterfaceType.HDLC &&
                Client.InterfaceType != InterfaceType.HdlcWithModeE)
            {
                eop = null;
            }
            int pos = 0;
            bool succeeded = false;
            GXByteBuffer rd = new GXByteBuffer();
            ReceiveParameters<byte[]> p = new ReceiveParameters<byte[]>()
            {
                Eop = eop,
                Count = Client.GetFrameSize(rd),
                AllData = true,
                WaitTime = WaitTime,
            };
            lock (Media.Synchronous)
            {
                /////////////////////////////////////////////
                while (!succeeded && pos != 3)
                {
                    if (!reply.IsStreaming())
                    {
                        WriteTrace("TX:\t" + DateTime.Now.ToLongTimeString() + "\t" + GXCommon.ToHex(data, true));
                        p.Reply = null;
                        Media.Send(data, null);
                    }
                    succeeded = Media.Receive(p);
                    if (!succeeded)
                    {
                        if (++pos >= RetryCount)
                        {
                            throw new Exception("Не удалось получить ответ от устройства в заданное время.");
                        }
                        //If Eop is not set read one byte at time.
                        if (p.Eop == null)
                        {
                            p.Count = 1;
                        }
                        //Try to read again...
                       Debug.WriteLine("Не удалось отправить данные. Повторная попытка " + pos.ToString() + "/3");
                    }
                }
                rd = new GXByteBuffer(p.Reply);
                try
                {
                    pos = 0;
                    //Loop until whole COSEM packet is received.
                    while (!Client.GetData(rd, reply, notify))
                    {
                        p.Reply = null;
                        if (notify.IsComplete && notify.Data.Data != null)
                        {
                            //Handle notify.
                            if (!notify.IsMoreData)
                            {
                                //Show received push message as XML.
                                string xml;
                                GXDLMSTranslator t = new GXDLMSTranslator(TranslatorOutputType.SimpleXml);
                                t.DataToXml(notify.Data, out xml);
                                Log.Info(xml);
                                notify.Clear();
                                continue;
                            }
                        }
                        if (p.Eop == null)
                        {
                            p.Count = Client.GetFrameSize(rd);
                        }
                        ///////////////////////////////////////
                        while (!Media.Receive(p))
                        {
                            if (++pos >= RetryCount)
                            {
                                throw new Exception("Не удалось получить ответ от устройства в заданное время.");
                            }
                            p.Reply = null;
                            Media.Send(data, null);
                            //Try to read again...
                           Debug.WriteLine("Не удалось отправить данные. Повторная попытка " + pos.ToString() + "/3");
                        }
                        rd.Set(p.Reply);
                    }
                }
                catch (Exception ex)
                {
                    WriteTrace("RX:\t" + DateTime.Now.ToLongTimeString() + "\t" + rd);
                    throw ex;
                }
            }

            WriteTrace("RX:\t" + DateTime.Now.ToLongTimeString() + "\t" + rd);
            if (reply.Error != 0)
            {
                if (reply.Error == (short)ErrorCode.Rejected)
                {
                    Thread.Sleep(1000);
                    ReadDLMSPacket(data, reply);
                }
                else
                {
                    throw new GXDLMSException(reply.Error);
                }
            }
        }

        /// <summary>
        /// Send data block(s) to the meter.
        /// </summary>
        /// <param name="data">Send data block(s).</param>
        /// <param name="reply">Received reply from the meter.</param>
        /// <returns>Return false if frame is rejected.</returns>
        public bool ReadDataBlock(byte[][] data, GXReplyData reply)
        {
            if (data == null)
            {
                return true;
            }
            foreach (byte[] it in data)
            {
                reply.Clear();
                ReadDataBlock(it, reply);
            }
            return reply.Error == 0;
        }

        /// <summary>
        /// Read data block from the device.
        /// </summary>
        /// <param name="data">data to send</param>
        /// <param name="text">Progress text.</param>
        /// <param name="multiplier"></param>
        /// <returns>Received data.</returns>
        public void ReadDataBlock(byte[] data, GXReplyData reply)
        {
            ReadDLMSPacket(data, reply);
            lock (Media.Synchronous)
            {
                while (reply.IsMoreData)
                {
                    if (reply.IsStreaming())
                    {
                        data = null;
                    }
                    else
                    {
                        data = Client.ReceiverReady(reply);
                    }
                    ReadDLMSPacket(data, reply);
                    if (Trace > TraceLevel.Info)
                    {
                        //If data block is read.
                        if ((reply.MoreData & RequestTypes.Frame) == 0)
                        {
                            Console.Write("+");
                        }
                        else
                        {
                            Console.Write("-");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Read attribute value.
        /// </summary>
        /// <param name="it">COSEM object to read.</param>
        /// <param name="attributeIndex">Attribute index.</param>
        /// <returns>Read value.</returns>
        public object Read(GXDLMSObject it, int attributeIndex)
        {
            GXReplyData reply = new GXReplyData();
            if (!ReadDataBlock(Client.Read(it, attributeIndex), reply))
            {
                if (reply.Error != (short)ErrorCode.Rejected)
                {
                    throw new GXDLMSException(reply.Error);
                }
                reply.Clear();
                Thread.Sleep(1000);
                if (!ReadDataBlock(Client.Read(it, attributeIndex), reply))
                {
                    throw new GXDLMSException(reply.Error);
                }
            }
            //Update data type.
            if (it.GetDataType(attributeIndex) == DataType.None)
            {
                it.SetDataType(attributeIndex, reply.DataType);
            }
            return Client.UpdateValue(it, attributeIndex, reply.Value);
        }


        /// <summary>
        /// Read list of attributes.
        /// </summary>
        public void ReadList(List<KeyValuePair<GXDLMSObject, int>> list)
        {
            byte[][] data = Client.ReadList(list);
            GXReplyData reply = new GXReplyData();
            List<object> values = new List<object>();
            foreach (byte[] it in data)
            {
                ReadDataBlock(it, reply);
                //Value is null if data is send in multiple frames.
                if (reply.Value is IEnumerable<object>)
                {
                    values.AddRange((IEnumerable<object>)reply.Value);
                }
                else if (reply.Value != null)
                {
                    values.Add(reply.Value);
                }
                reply.Clear();
            }
            if (values.Count != list.Count)
            {
                throw new Exception("Invalid reply. Read items count do not match.");
            }
            Client.UpdateValues(list, values);
        }

        /// <summary>
        /// Write list of attributes.
        /// </summary>
        public void WriteList(List<KeyValuePair<GXDLMSObject, int>> list)
        {
            byte[][] data = Client.WriteList(list);
            GXReplyData reply = new GXReplyData();
            foreach (byte[] it in data)
            {
                ReadDataBlock(it, reply);
                reply.Clear();
            }
        }

        /// <summary>
        /// Write attribute value.
        /// </summary>
        public void Write(GXDLMSObject it, int attributeIndex)
        {
            GXReplyData reply = new GXReplyData();
            ReadDataBlock(Client.Write(it, attributeIndex), reply);
        }

        /// <summary>
        /// Method attribute value.
        /// </summary>
        public void Method(GXDLMSObject it, int attributeIndex, object value, DataType type)
        {
            GXReplyData reply = new GXReplyData();
            ReadDataBlock(Client.Method(it, attributeIndex, value, type), reply);
        }

        /// <summary>
        /// Read Profile Generic Columns by entry.
        /// </summary>
        public object[] ReadRowsByEntry(GXDLMSProfileGeneric it, UInt32 index, UInt32 count)
        {
            GXReplyData reply = new GXReplyData();
            ReadDataBlock(Client.ReadRowsByEntry(it, index, count), reply);
            return (object[])Client.UpdateValue(it, 2, reply.Value);
        }

        /// <summary>
        /// Read Profile Generic Columns by range.
        /// </summary>
        public object[] ReadRowsByRange(GXDLMSProfileGeneric it, DateTime start, DateTime end)
        {
            GXReplyData reply = new GXReplyData();
            ReadDataBlock(Client.ReadRowsByRange(it, start, end), reply);
            return (object[])Client.UpdateValue(it, 2, reply.Value);
        }

        /// <summary>
        /// Disconnect.
        /// </summary>
        public void Disconnect()
        {
            if (Media != null && Client != null)
            {
                try
                {
                    if (Trace > TraceLevel.Info)
                    {
                        Log.Info("Disconnecting from the meter.");
                    }
                    GXReplyData reply = new GXReplyData();
                    ReadDLMSPacket(Client.DisconnectRequest(), reply);
                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// Release.
        /// </summary>
        public void Release()
        {
            if (Media != null && Client != null)
            {
                try
                {
                    if (Trace > TraceLevel.Info)
                    {
                        Log.Info("Release from the meter.");
                    }
                    GXReplyData reply = new GXReplyData();
                    ReadDataBlock(Client.ReleaseRequest(), reply);
                }
                catch (Exception ex)
                {
                    //All meters don't support Release.
                    Log.Error("Release failed. " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Close connection to the meter.
        /// </summary>
        public void Close()
        {
            if (Media != null && Client != null)
            {
                try
                {
                    if (Trace > TraceLevel.Info)
                    {
                        Log.Info("Disconnecting from the meter.");
                    }
                    GXReplyData reply = new GXReplyData();
                    try
                    {
                        //Release is call only for secured connections.
                        //All meters are not supporting Release and it's causing problems.
                        if (Client.InterfaceType == InterfaceType.WRAPPER ||
                            (Client.InterfaceType == InterfaceType.HDLC && Client.Ciphering.Security != (byte)Security.None))
                        {
                            ReadDataBlock(Client.ReleaseRequest(), reply);
                        }
                    }
                    catch (Exception ex)
                    {
                        //All meters don't support Release.
                        Log.Error("Release failed. " + ex.Message);
                    }
                    reply.Clear();
                    ReadDLMSPacket(Client.DisconnectRequest(), reply);
                    Media.Close();
                }
                catch
                {

                }
                Media = null;
                Client = null;
            }
        }

        /// <summary>
        /// Write trace.
        /// </summary>
        /// <param name="line"></param>
        void WriteTrace(string line)
        {
            if (Trace > TraceLevel.Info)
            {
                Log.Info(line);
            }
            using (FileStream fs = File.Open("trace.txt", FileMode.Append))
            {
                using (TextWriter writer = new StreamWriter(fs))
                {
                    writer.WriteLine(line);
                }
            }
        }
    }
}
