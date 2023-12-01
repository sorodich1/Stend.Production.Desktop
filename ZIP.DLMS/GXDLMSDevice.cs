//
// --------------------------------------------------------------------------
//  Gurux Ltd
//
//
//
// Version:         $Revision: 12250 $,
//                  $Date: 2020-12-16 12:25:15 +0200 (ke, 16 joulu 2020) $
//                  $Author: gurux01 $
//
// Copyright (c) Gurux Ltd
//
//---------------------------------------------------------------------------
//
//  DESCRIPTION
//
// This file is a part of Gurux Device Framework.
//
// Gurux Device Framework is Open Source software; you can redistribute it
// and/or modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; version 2 of the License.
// Gurux Device Framework is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCgHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License for more details.
//
// More information of Gurux DLMS/COSEM Director: https://www.gurux.org/GXDLMSDirector
//
// This code is licensed under the GNU General Public License v2.
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------

using Gurux.Common;
using Gurux.DLMS;
using Gurux.DLMS.Enums;
using Gurux.DLMS.ManufacturerSettings;
using Gurux.DLMS.Objects;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Xml.Serialization;

namespace ZIP.DLMS
{
    [Serializable]
    public class GXDLMSDevice : GXDLMSMeter
    {
        DeviceState m_Status;
        Logger _logger = LogManager.GetCurrentClassLogger();

        public Logger Logger { get => _logger; set { _logger = value; } }

        [DefaultValue(null)]
        public GXAsyncWork AsyncWork { get; set; }
        public bool AsyncWorkCanceled { get => AsyncWork != null && AsyncWork.IsCanceled; }

        internal void UpdateStatus(DeviceState state)
        {
            //Clear connecting.
            if ((state&DeviceState.Connected) > 0)
            {
                state &= ~DeviceState.Connecting;
                state |= DeviceState.Initialized;
            }
            m_Status = state;
            OnStatusChanged?.Invoke(this, m_Status);
        }

        [Browsable(false)]
        [XmlIgnore()]
        public DeviceState Status
        {
            get
            {
                return m_Status;
            }
        }

        [Browsable(false)]
        [XmlIgnore()]
        public double SystemTimeDelta = double.NaN;

        [Browsable(false)]
        [XmlIgnore()]
        public TimeZoneInfo TimeZoneInfo = null;

        [Browsable(false)]
        [XmlIgnore()]
        public ProgressEventHandler OnProgress;

        [Browsable(false)]
        [XmlIgnore()]
        public DisconnectEventHandler OnDisconnect;

        [Browsable(false)]
        [XmlIgnore()]
        public StatusEventHandler OnStatusChanged;

        [Browsable(false)]
        [XmlIgnore()]
        public MessageTraceEventHandler OnTrace;

        /// <summary>
        /// Meter sends event notification.
        /// </summary>
        [Browsable(false)]
        [XmlIgnore()]
        public ReceivedEventHandler OnEvent;


        public bool ReadyToExchange { get => Media != null && Media.IsOpen && (Status & DeviceState.Connected) > 0; }

        [Browsable(false)]
        [XmlIgnore()]
        System.Timers.Timer KeepAlive;

        [Browsable(false)]
        [XmlIgnore()]
        GXDLMSCommunicator communicator;

        public void KeepAliveStart(bool newCycle = false)
        {
            if (InactivityTimeout > 0)
            {
                if (Media.IsOpen && InactivityTimeout > 0)
                {
                    if (KeepAlive.Enabled || newCycle)
                    {
                        // выполняем внеочередную отправку запроса поддержки подключения 
                        KeepAliveRequest();
                        // останавливаем текущий цикл отправки запросов поддержки подключения 
                        KeepAlive.Enabled = false;
                    }
                    // запускаем новый цикл отправки запросов поддержки подключения 
                    KeepAlive.Interval = InactivityTimeout * 1000;
                    KeepAlive.Enabled = true;
                    GXLogWriter.WriteLog($"[{Name}]: Включен режим поддержки подключения. Таймаут {InactivityTimeout} сек.");
                }
            }
            else
                KeepAliveStop();
        }

        public void KeepAliveStop()
        {
            if (KeepAlive.Enabled)
            {
                KeepAlive.Enabled = false;
            }
        }


        /// <summary>
        /// Serial port connection needs keep alive messages every N second...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeepAlive_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            KeepAliveRequest();
        }

        public void KeepAliveRequest()
        {
            if (ReadyToExchange)
                try
                {
                    communicator.KeepAlive();
                }
                catch (Exception)
                {
                    this.Disconnect();
                }
        }

        /// <summary>
        /// Used logical client ID.
        /// </summary>
        /// <remarks>
        /// This is opsolite. Use ClientAddress.
        /// </remarks>
        [DefaultValue(null)]
        public object ClientID
        {
            get
            {
                return null;
            }
            set
            {
                ClientAddress = Convert.ToInt32(value);
            }
        }

        /// <summary>
        /// Is wrapper used.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(false)]
        public bool UseWrapper
        {
            get
            {
                return false;
            }
            set
            {
                if (value)
                {
                    InterfaceType = InterfaceType.WRAPPER;
                }
            }
        }

        void NotifyProgress(object sender, string description, int current, int maximium)
        {
            OnProgress?.Invoke(sender, description, current, maximium);
        }

        public void InitializeConnection()
        {
            try
            {
                UpdateStatus(DeviceState.Connecting);
                try
                {
                    communicator.InitializeConnection(true);
                    UpdateStatus(DeviceState.Connected);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка инициализации подключения: {ex.Message}");
                }
            }
            catch (Exception)
            {
                UpdateStatus(DeviceState.Initialized);
                if (Media != null)
                {
                    Media.Close();
                }
                throw;
            }
        }

        public GXDLMSCommunicator Comm
        {
            get
            {
                return communicator;
            }
        }

        [Browsable(false)]
        [XmlIgnore()]
        public Gurux.DLMS.ManufacturerSettings.GXObisCodeCollection ObisCodes
        {
            get
            {
                return communicator.client.CustomObisCodes;
            }
            set
            {
                communicator.client.CustomObisCodes = value;
            }
        }

        [Browsable(false)]
        [XmlIgnore()]
        public Gurux.Common.IGXMedia Media
        {
            get
            {
                return communicator.media;
            }
            set
            {
                communicator.media = value;
            }
        }

        [Browsable(false)]
        [XmlIgnore()]
        public GXManufacturerCollection Manufacturers
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSDevice(Gurux.Common.IGXMedia media) : base()
        {
            communicator = new GXDLMSCommunicator(this, media);
            Objects = communicator.client.Objects;
            Objects.Tag = this;
            communicator.OnProgress += new ProgressEventHandler(this.NotifyProgress);
            this.KeepAlive = new System.Timers.Timer
            {
                Enabled = false,
                Interval = 40000
            };
            this.KeepAlive.Elapsed += new System.Timers.ElapsedEventHandler(KeepAlive_Elapsed);
            m_Status = DeviceState.Initialized;
        }

        /// <summary>
        /// Constructor for serializing.
        /// </summary>
        public GXDLMSDevice() : this(null)
        {
        }

        [Browsable(false)]
        public override string MediaType
        {
            get
            {
                if (communicator.media == null)
                {
                    return null;
                }
                return communicator.media.GetType().ToString();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    communicator.media = null;
                }
                else if (string.Compare(value, typeof(Gurux.Net.GXNet).FullName, true) == 0)
                {
                    communicator.media = new Gurux.Net.GXNet();
                }
                else if (string.Compare(value, typeof(Gurux.Serial.GXSerial).FullName, true) == 0)
                {
                    communicator.media = new Gurux.Serial.GXSerial();
                }
                else if (string.Compare(value, typeof(Gurux.Terminal.GXTerminal).FullName, true) == 0)
                {
                    communicator.media = new Gurux.Terminal.GXTerminal();
                }
                else
                {
                    Type type = Type.GetType(value);
                    if (type == null)
                    {
                        string ns = "";
                        int pos = value.LastIndexOf('.');
                        if (pos != -1)
                        {
                            ns = value.Substring(0, pos);
                        }
                        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                        {
                            if (assembly.GetName().Name == ns)
                            {
                                if (assembly.GetType(value, false, true) != null)
                                {
                                    type = assembly.GetType(value);
                                }
                            }
                        }
                    }
                    if (type == null)
                    {
                        throw new Exception("Недопустимый тип канала опроса: " + value);
                    }
                    communicator.media = (IGXMedia)Activator.CreateInstance(type);
                }
            }
        }

        public void Disconnect()
        {
            try
            {
                KeepAliveStop();
                if (Comm.media.IsOpen && m_Status != DeviceState.Disconnecting)
                {
                    if (Comm.media is IGXMedia2 media)
                    {
                        if (media.AsyncWaitHandle != null)
                        {
                            media.AsyncWaitHandle.Set();
                        }
                    }
                    UpdateStatus(DeviceState.Disconnecting);
                    communicator.Disconnect();
                }
                else
                {
                    Comm.media.Close();
                }

                GXLogWriter.WriteLog($"[{Name}]: Прибор учета отключен.", LogLevel.Info);
            }
            catch (Exception Ex)
            {
                //Do not show error occurred in disconnect. Write error only to the log file.
                GXLogWriter.WriteLog($"[{Name}]: {Ex.Message}", LogLevel.Error);
            }
            finally
            {
                UpdateStatus(DeviceState.Initialized);
                OnDisconnect?.Invoke(this);
            }
        }

        GXDLMSObject FindObject(ObjectType type, string logicalName)
        {
            foreach (GXDLMSObject it in Objects)
            {
                if (type == it.ObjectType && it.LogicalName == logicalName)
                {
                    return it;
                }
            }
            return null;
        }

        delegate void UpdateColumnsEventHandler(GXDLMSProfileGeneric item, GXManufacturer man);

        public void UpdateColumns(GXDLMSProfileGeneric item, GXManufacturer man)
        {
            if (Comm.parentForm != null)
                if (Comm.parentForm.InvokeRequired)
                {
                    try
                    {
                        Comm.parentForm.Invoke(new UpdateColumnsEventHandler(UpdateColumns), item, man);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.Message);
                    }
                    return;
                }
            try
            {
                item.Buffer.Clear();
                item.CaptureObjects.Clear();
#if !ZIP
                List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> cols = null;
                try
                {
                    Comm.GetProfileGenericColumns(item);
                    if (Standard == Gurux.DLMS.Enums.Standard.Italy && item.CaptureObjects.Count == 0)
                    {
                        cols = GetColumns(Comm.client.Objects, Comm.client.CustomObisCodes, item.LogicalName, 0);
                        GXDLMSConverter c = new GXDLMSConverter(Standard);
                        foreach (var it in cols)
                        {
                            c.UpdateOBISCodeInformation(it.Key);
                        }
                        item.CaptureObjects.AddRange(cols);
                    }
                }
                catch (GXDLMSException ex)
                {
                    if (Standard == Gurux.DLMS.Enums.Standard.Italy)
                    {
                        int type = 0;
                        if (Comm.client.Objects.FindByLN(ObjectType.Data, "0.0.96.1.3.255") is GXDLMSData obj)
                        {
                            if (obj.Value == null)
                            {
                                try
                                {
                                    Comm.ReadValue(obj, 2);
                                    type = Convert.ToInt16(obj.Value);
                                }
                                catch (Exception)
                                {
                                    type = 0;
                                }
                            }
                            else
                            {
                                type = Convert.ToInt16(obj.Value);
                            }
                        }
                        cols = GetColumns(Comm.client.Objects, Comm.client.CustomObisCodes, item.LogicalName, type);
                        item.CaptureObjects.Clear();
                        GXDLMSConverter c = new GXDLMSConverter(Standard);
                        foreach (var it in cols)
                        {
                            c.UpdateOBISCodeInformation(it.Key);
                        }
                        item.CaptureObjects.AddRange(cols);
                    }
                    if (cols == null || cols.Count == 0)
                    {
                        throw ex;
                    }
                    throw ex;
                }
#else
                Comm.GetProfileGenericColumns(item);
#endif
            }
            catch (Exception ex)
            {
                GXLogWriter.WriteLog($"[{Name}]: {ex.Message}", LogLevel.Error);
                throw;
            }
        }

        private static GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject> CreateColumn(GXDLMSObjectCollection objects, GXObisCodeCollection obisCodes, ObjectType ot, string ln, int index)
        {
            return CreateColumn(objects, obisCodes, ot, ln, index, DataType.None);
        }

        private static GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject> CreateColumn(GXDLMSObjectCollection objects, GXObisCodeCollection obisCodes, ObjectType ot, string ln, int index, DataType dt)
        {
            GXDLMSObject obj = objects.FindByLN(ot, ln);
            if (obj == null)
            {
                GXObisCode code = obisCodes.FindByLN(ot, ln, null);
                obj = GXDLMSClient.CreateObject(ot);
                obj.LogicalName = ln;
                if (code != null)
                {
                    GXDLMSAttributeSettings s = code.Attributes.Find(index);
                    if (s != null)
                    {
                        obj.SetDataType(index, s.Type);
                        obj.SetUIDataType(index, s.UIType);
                        obj.SetValues(index, s.Values);
                    }
                }
                else
                {
                    obj.SetUIDataType(index, dt);
                }
            }
            return new GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(obj, new GXDLMSCaptureObject(index, 0));
        }

        private static List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> GetColumns(GXDLMSObjectCollection objects, GXObisCodeCollection obisCodes, string ln, int type)
        {
            List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> list = new List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>();
            //Event Logbook
            if (ln == "7.0.99.98.0.255")
            {
                //If meter.
                if (type == 2)
                {
                    list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.1.1.0.255", 2));
                    list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.96.15.2.255", 2));
                    list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.96.11.2.255", 2));
                    list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "7.1.96.5.1.255", 2));
                    list.Add(CreateColumn(objects, obisCodes, ObjectType.AssociationLogicalName, "0.0.40.0.0.255", 11));
                }
                else
                {
                    //If concentrator.
                    list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.1.1.0.255", 2));
                    list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.96.15.2.255", 2));
                    list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.96.11.2.255", 2));
                    list.Add(CreateColumn(objects, obisCodes, ObjectType.AssociationLogicalName, "0.0.40.0.0.255", 11));
                    list.Add(CreateColumn(objects, obisCodes, ObjectType.GSMDiagnostic, "0.0.25.6.0.255", 5));
                    list.Add(CreateColumn(objects, obisCodes, ObjectType.GSMDiagnostic, "0.0.25.6.0.255", 6));
                }
            }
            else if (ln == "7.0.99.99.3.255")
            {
                //If meter.
                if (type == 2)
                {
                    list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.1.1.0.255", 2));
                    list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "7.1.96.5.1.255", 2));
                    list.Add(CreateColumn(objects, obisCodes, ObjectType.Register, "7.0.13.2.0.255", 2));
                    list.Add(CreateColumn(objects, obisCodes, ObjectType.Register, "7.0.12.2.0.255", 2));
                }
                else
                {
                    //If concentrator.
                    list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.1.1.0.255", 2));
                    list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "7.1.96.5.1.255", 2));
                }
            }
            else if (ln == "7.0.99.98.1.255")
            {
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.1.1.0.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.96.11.1.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.96.15.1.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.AssociationLogicalName, "0.0.40.0.0.255", 11));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Register, "7.0.13.2.0.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "7.1.96.5.1.255", 2));
            }
            else if (ln == "7.0.99.98.6.255")
            {
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.1.1.0.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.96.15.3.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "7.0.96.5.1.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.96.5.4.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.MacAddressSetup, "0.1.25.2.0.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.94.39.24.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.94.39.23.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.94.39.13.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.94.39.15.255", 2));
            }
            else if (ln == "7.0.99.16.0.255")
            {
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.96.15.1.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.ParameterMonitor, "0.0.16.2.0.255", 2));
            }
            else if (ln == "7.0.98.11.0.255")
            {
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.1.1.0.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "7.0.96.5.1.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "7.0.96.10.2.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "7.0.0.1.0.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Register, "7.0.13.2.0.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Register, "7.0.13.2.1.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Register, "7.0.13.2.2.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Register, "7.0.13.2.3.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Register, "7.0.12.2.0.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.ExtendedRegister, "7.0.43.45.0.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.ExtendedRegister, "7.0.43.45.0.255", 5));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.TariffPlan, "0.0.94.39.21.255", 2));
            }
            //Not Enrolled Detected List
            else if (ln == "0.0.21.0.1.255" ||
                //Enrolled Detected List
                ln == "0.1.21.0.1.255")
            {
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.1.1.0.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.94.39.36.255", 2, DataType.DateTime));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.MacAddressSetup, "0.1.25.2.0.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.94.39.35.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.94.39.23.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.94.39.34.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.94.39.38.255", 2));
            }
            // White list
            else if (ln == "0.0.21.0.2.255")
            {
                list.Add(CreateColumn(objects, obisCodes, ObjectType.MacAddressSetup, "0.1.25.2.0.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.94.39.13.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.94.39.15.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.94.39.39.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.1.1.0.255", 2));
            }
            // Bloack list
            else if (ln == "0.0.21.0.3.255")
            {
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.1.1.0.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.MacAddressSetup, "0.1.25.2.0.255", 2));
            }
            // Command Queue
            else if (ln == "0.0.21.0.10.255")
            {
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.1.1.0.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.MacAddressSetup, "0.1.25.2.0.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.94.39.45.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.94.39.27.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.94.39.50.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.94.39.28.255", 2));
            }
            // Directory event logbook
            else if (ln == "7.0.99.98.3.255")
            {
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.1.1.0.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.96.15.4.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.96.11.4.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.1.25.2.0.255", 2));
            }
            //Push Data Queue.
            else if (ln == "0.0.98.1.0.255")
            {
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.1.1.0.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.MacAddressSetup, "0.1.25.2.0.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.94.39.50.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.94.39.23.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.94.39.24.255", 2));
            }
            //Response Queue
            else if (ln == "0.0.21.0.11.255")
            {
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.1.1.0.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.MacAddressSetup, "0.1.25.2.0.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.94.39.27.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.94.39.29.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.94.39.50.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.94.39.23.255", 2));
                list.Add(CreateColumn(objects, obisCodes, ObjectType.Data, "0.0.94.39.24.255", 2));
            }
            return list;
        }

        private static void UpdateError(GXDLMSObject it, int attributeIndex, Exception ex)
        {
            if (ex is GXDLMSException t)
            {
                if (t.ErrorCode == 1 || t.ErrorCode == 3)
                {
                    it.SetAccess(attributeIndex, AccessMode.NoAccess);
                }
                else
                {
                    throw ex;
                }
            }
            else
            {
                throw ex;
            }
        }

        /// <summary>
        /// After UpdateObjects call objects can be read using Objects property.
        /// </summary>
        public void UpdateObjects()
        {
            try
            {
                // загружаем текущую ассоциацию ПУ (список объектов)
                GXDLMSObjectCollection objs = Comm.GetObjects();
                objs.Tag = this;
                int pos = 0;
                foreach (GXDLMSObject it in objs)
                {
                    ++pos;
                    //if (it.LogicalName == "1.0.0.8.4.255")
                    //    GXLogWriter.WriteLog("1.0.0.8.4.255 !!!");
                    NotifyProgress(this, "Создание объекта " + it.LogicalName, pos, objs.Count);
                    Objects.Add(it);
                }
                GXLogWriter.WriteLog($"[{Name}]: Всего создано объектов {Objects.Count}");

                //Read registers units and scalers.
                int cnt = Objects.Count;
#if !ZIP
                // доступ по короткому имени для счетчиков ZIP запрещен
                if (!UseLogicalNameReferencing)
                {
                    GXLogWriter.WriteLog("--- Чтение прав доступа. ---");
                    try
                    {
                        foreach (GXDLMSAssociationShortName sn in Objects.GetObjects(ObjectType.AssociationShortName))
                        {
                            if (sn.Version > 1)
                            {
                                Comm.ReadValue(sn, 3);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        GXLogWriter.WriteLog(ex.Message);
                    }
                    GXLogWriter.WriteLog("--- Чтение прав доступа завершено. ---");
                }
#endif

                GXLogWriter.WriteLog($"[{Name}]: Масштабные коэффициенты, единицы измерения и тип значений.");

                // множественный доступ к объектам разрешен?
                if ((Comm.client.NegotiatedConformance & Gurux.DLMS.Enums.Conformance.MultipleReferences) != 0)
                {
                    List<KeyValuePair<GXDLMSObject, int>> list = new List<KeyValuePair<GXDLMSObject, int>>();
                    foreach (GXDLMSObject it in Objects)
                    {
                        if ((it is GXDLMSRegister || it is GXDLMSExtendedRegister) && (it.GetAccess(3) & AccessMode.Read) != 0)
                        {
                            list.Add(new KeyValuePair<GXDLMSObject, int>(it, 3));
                        }
                        else
                        if (it is GXDLMSDemandRegister && (it.GetAccess(4) & AccessMode.Read) != 0)
                        {
                            list.Add(new KeyValuePair<GXDLMSObject, int>(it, 4));
                        }
                    }
                    if (list.Count != 0)
                    {
                        try
                        {
                            Comm.ReadList(list);
                        }
                        catch (Exception ex)
                        {
                            GXLogWriter.WriteLog($"[{Name}]: {ex.Message}", LogLevel.Error);
                        }
                    }

                }
                else // читаем по каждому объекту отдельно
                {
                    for (pos = 0; pos != cnt; ++pos)
                    {
                        GXDLMSObject it = Objects[pos];
                        if (
                            (it is GXDLMSData)
                            && (it.GetAccess(2) & AccessMode.Read) != 0
                        )
                        {
                            //Read value to get data type
                            try
                            {
                                if ((it.GetAccess(2) & AccessMode.Read) != 0)
                                {
                                    Comm.ReadValue(it, 2);
                                }
                            }
                            catch (Exception ex)
                            {
                                GXLogWriter.WriteLog($"[{Name}]: {ex.Message}", LogLevel.Error);
                                it.SetAccess(2, AccessMode.NoAccess);
                                if (ex is GXDLMSException)
                                {
                                    continue;
                                }
                                throw ex;
                            }
                        }
                        else
                        if (it is GXDLMSRegister || it is GXDLMSExtendedRegister)
                        {
                            //Read scaler first.
                            try
                            {
                                if ((it.GetAccess(3) & AccessMode.Read) != 0)
                                {
                                    Comm.ReadValue(it, 3);
                                }
                            }
                            catch (Exception ex)
                            {
                                GXLogWriter.WriteLog($"[{Name}]: {ex.Message}", LogLevel.Error);
                                it.SetAccess(3, AccessMode.NoAccess);
                                if (ex is GXDLMSException)
                                {
                                    continue;
                                }
                                throw ex;
                            }
                            //Read value to get data type
                            try
                            {
                                if ((it.GetAccess(2) & AccessMode.Read) != 0)
                                {
                                    Comm.ReadValue(it, 2);
                                }
                            }
                            catch (Exception ex)
                            {
                                GXLogWriter.WriteLog($"[{Name}]: {ex.Message}", LogLevel.Error);
                                it.SetAccess(2, AccessMode.NoAccess);
                                if (ex is GXDLMSException)
                                {
                                    continue;
                                }
                                throw ex;
                            }
                        }
                        else
                        if (it is GXDLMSDemandRegister)
                        {
                            //Read scaler first.
                            try
                            {
                                if ((it.GetAccess(4) & AccessMode.Read) != 0)
                                {
                                    Comm.ReadValue(it, 4);
                                }
                            }
                            catch (Exception ex)
                            {
                                GXLogWriter.WriteLog($"[{Name}]: {ex.Message}", LogLevel.Error);
                                UpdateError(it, 4, ex);

                                if (ex is GXDLMSException)
                                {
                                    continue;
                                }
                                throw ex;
                            }
                            //Read Current value to get data type
                            try
                            {
                                if ((it.GetAccess(2) & AccessMode.Read) != 0)
                                {
                                    Comm.ReadValue(it, 2);
                                }
                            }
                            catch (Exception ex)
                            {
                                GXLogWriter.WriteLog($"[{Name}]: {ex.Message}", LogLevel.Error);
                                it.SetAccess(2, AccessMode.NoAccess);
                                if (ex is GXDLMSException)
                                {
                                    continue;
                                }
                                throw ex;
                            }
                            //Read Period
                            try
                            {
                                Comm.ReadValue(it, 8);
                            }
                            catch (Exception ex)
                            {
                                GXLogWriter.WriteLog($"[{Name}]: {ex.Message}", LogLevel.Error);
                                UpdateError(it, 8, ex);
                                if (ex is GXDLMSException)
                                {
                                    continue;
                                }
                                throw ex;
                            }
                            //Read number of periods
                            try
                            {
                                Comm.ReadValue(it, 9);
                            }
                            catch (Exception ex)
                            {
                                GXLogWriter.WriteLog($"[{Name}]: {ex.Message}", LogLevel.Error);
                                UpdateError(it, 9, ex);
                                if (ex is GXDLMSException)
                                {
                                    continue;
                                }
                                throw ex;
                            }
                        }
                    }
                }
                GXLogWriter.WriteLog($"[{Name}]: Загрузка масштабных коэффициентов, единиц измерения и типа значений завершена.");
                GXLogWriter.WriteLog($"[{Name}]: Списки объектов захвата профилей.");
                //OnProgress?.Invoke(this, "Получение списка захватываемых объектов профиля.", cnt, cnt);
                foreach (GXDLMSProfileGeneric it in objs.GetObjects(ObjectType.ProfileGeneric))
                {
                    //++pos;
                    //Read Profile Generic Columns.
                    try
                    {
                        //NotifyProgress(this, "Получение объектов профиля", (2 * cnt) + pos, 3 * objs.Count);
                        UpdateColumns(it, Manufacturers.FindByIdentification(Manufacturer));
                        if (it.CaptureObjects == null || it.CaptureObjects.Count == 0)
                        {
                            continue;
                        }
                        else
                        {
                            InitCaptureObjects(it);
                        }
                    }
                    catch
                    {
                        GXLogWriter.WriteLog($"[{Name}]: Не удалось считать объекты захвата {it.LogicalName}.",  LogLevel.Error);
                        continue;
                    }
                }
            }
            finally
            {
                //NotifyProgress(this, "", 0, 0);
            }
        }
        /// <summary>
        /// Синхронизация типов данных объектов захвата указанного профиля 
        /// с типом данных соответствующего объекта ПУ при загрузке ассоциаций
        /// </summary>
        /// <param name="it">профиль, где требуется синхронизация типов данных объектов захвата</param>
        private void InitCaptureObjects(GXDLMSProfileGeneric it)
        {
            try
            {
                foreach (var obj in it.CaptureObjects)
                {
                    string obj_desc = $"{obj.Key}:{obj.Value.AttributeIndex}";
                    GXLogWriter.WriteLog($"[{Name}]: Синхронизация типа данных объекта захвата {obj_desc}.");
                    // для объекта захвата с неустановленным типом данных значения - попытка корректировки типа
                    try
                    {
                        GXDLMSObject deviceObject = Objects.FindByLN(obj.Key.ObjectType, obj.Key.LogicalName);
                        if (deviceObject != null)
                        {
                            DataType
                                //type = deviceObject.GetDataType(obj.Value.AttributeIndex),
                                coType = obj.Key.GetDataType(obj.Value.AttributeIndex);
                            // типы данных объекта ПУ и объекта захвата профиля не совпадают или не установлены
                            //if (type != coType || type == DataType.None)
                            {
                                // TODO: попытка прочитать значение из счетчика и откорректировать его тип в объекте захвата
                                Comm.ReadValue(deviceObject, obj.Value.AttributeIndex);
                                DataType type = deviceObject.GetDataType(obj.Value.AttributeIndex);
                                if (type != coType)
                                {
                                    GXLogWriter.WriteLog($"[{Name}]: Синхронизация типа данных {coType}->{type}.");
                                    obj.Key.SetDataType(obj.Value.AttributeIndex, type);
                                    //GXLogWriter.WriteLog($"[{Name}]:Синхронизация типов данных для {obj_desc} выполнена.");
                                }
                            }
                            //else //типы данных объекта ПУ и объекта захвата профиля синхронизированы
                        }
                        else
                            throw new Exception("объект не найден.");
                    }
                    catch (Exception ex)
                    {
                        GXLogWriter.WriteLog($"[{Name}]: Ошибка синхронизации типа данных {obj.Key}:{obj.Value.AttributeIndex}: {ex.Message}", LogLevel.Error);
                    }
                }
            }
            catch
            {
                // skip
            }
        }
    }


    /// <summary>
    /// List of devices.
    /// </summary>
    [Serializable]
    public class GXDLMSDeviceCollection : List<GXDLMSDevice>
    {
    }

    /// <summary>
    /// List of meters.
    /// </summary>
    [Serializable]
    public class GXDLMSMeterCollection : List<GXDLMSMeter>
    {
    }

}
