using Ankom.Common;
using Gurux.DLMS;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using ZIP.DLMS;

namespace Production.Script
{
    public class WriteMasterKey : BaseStep
    {
        public WriteMasterKey() => StepName = "Запись мастер-прошивки";

        public string OBIS { get; set; } = "0.0.44.1.0.255";

        public override void ExecuteStep()
        {
            try
            {
                if (Owner != null && Owner.Device is GXDLMSDevice device)
                {
                    int num;
                    if(Owner != null)
                    {
                        device = Owner.Device;
                        num = device != null ? 1 : 0;
                    }
                    else
                    {
                        num = 0;
                    }
                    if(num == 0)
                    {
                        throw new Exception("Не определены настройки шага");
                    }
                    Status = EnumStepStatus.Run;
                    if(!(device.Objects.FindByLN(ObjectType.None, OBIS) is GXDLMSImageTransfer byLn))
                    {
                        throw new Exception("Не найден объект мастер прошивки");
                    }
                    FireEvent("генерация образа обновления мастер прошивки");

                    UpdateImage updateImage = new UpdateImage()
                    {
                        ImageIdentifier = Owner.Serial.Trim()
                    };

                    Guid guid = Guid.NewGuid();

                    //Генерация ключа
                    Owner.MasterKey = guid.ToByteArray();
                    string str = HexUtils.BytesToHexStr(Owner.MasterKey);

                    var u = Owner.StendSettings.FirmwareInfo.FirmwareKey;

                    byte[] byteKey = u.Concat(Owner.MasterKey).ToArray();


                    ////Добавление второй пары кюча
                    //List<byte> byteList = new List<byte>(Owner.StendSettings.FirmwareInfo.FirmwareKey);
                    //byteList.AddRange(Owner.MasterKey);

                    updateImage.Image = byteKey;
                    updateImage.ImageSize = updateImage.Image.Length;

                    //var index = Owner.StendSettings.Objects.FirstOrDefault(x => x.IsDuplicateSerialNumber == true);
                    //if(index != null)
                    //{
                        //Загрузка данных счётчика в БД
                        Helpers.LoadDB(Owner);
                   // }
                    //else
                    //{
                    //    Helpers.UpdateDB(Owner);
                    //}

                    Helpers.ReadAllAttributes(byLn, device);

                    if(!byLn.ImageTransferEnabled)
                    {
                        throw new Exception("Передача образа мастера - прошивки запрещена");
                    }

                    try
                    {
                        FireEvent("инициализация образа прошивки");
                        ImageTransferInitiate(device, byLn, updateImage);
                        FireEvent("Заливка и ожидание готовности к верификации");
                        ImageBlockTransfer(device, byLn, updateImage);
                        Thread.Sleep(2000);
                        FireEvent("Верификация образа прошивки");
                        ImageVerify(device, byLn, updateImage);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        internal static void ImageTransferInitiate(GXDLMSDevice dev, GXDLMSImageTransfer current, UpdateImage updateImage)
        {
            GXReplyData reply = new GXReplyData();

            if (dev == null || current == null)
                return;

            try
            {
                if (current.ImageTransferEnabled)
                {
                    byte[][] numArray = current.ImageTransferInitiate(dev.Comm.client, updateImage.ImageIdentifier, updateImage.ImageSize);
                    try
                    {
                        string logText = "[" + current.Description + "]: выполнение инициализации";
                        dev.Comm.ReadDataBlock(numArray[0], logText, reply);
                        Helpers.ReadAttributeValue(dev, current, 3);
                        Helpers.ReadAttributeValue(dev, current, 5);
                        Helpers.ReadAttributeValue(dev, current, 6);
                    }
                    catch(Exception ex)
                    {
                        if (dev.Comm.client.InterfaceType == InterfaceType.HDLC || dev.Comm.client.InterfaceType == InterfaceType.HdlcWithModeE || dev.Comm.client.InterfaceType == InterfaceType.PlcHdlc)
                        {
                            byte type;
                            GXDLMSClient.GetHdlcAddressInfo(new GXByteBuffer(numArray[0]), out int _, out int _, out type);
                            dev.Comm.client.HdlcSettings.SenderFrame = type;
                        }
                        throw ex;
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Ошибка инициализации мастер-прошивки: " + ex.Message);
            }
        }

        internal static void ImageVerify(GXDLMSDevice dev, GXDLMSImageTransfer current, object updateImage)
        {
            try
            {
                GXReplyData reply = new GXReplyData();
                byte[][] numArray = current.ImageVerify(dev.Comm.client);
                try
                {
                    string logText = "[" + current.Description + "]: выполнение верефикации";
                    dev.Comm.ReadDataBlock(numArray[0], logText, reply);
                    int num;
                    for(num = 0; num < 36; ++num)
                    {
                        Thread.Sleep(5000);
                        Helpers.ReadAttributeValue(dev, current, 6);

                        if(current.ImageTransferStatus == Gurux.DLMS.Objects.Enums.ImageTransferStatus.VerificationFailed)
                        {
                            throw new Exception("ошибка верефикации образа прошивки");
                        }

                        if(current.ImageTransferStatus == Gurux.DLMS.Objects.Enums.ImageTransferStatus.VerificationSuccessful)
                        {
                            break;
                        }

                        if (num >= 24)
                            throw new Exception("таймаут щжидания верефикации образа прошивки");
                    }
                }
                catch(Exception ex)
                {
                    if (dev.Comm.client.InterfaceType == InterfaceType.HDLC || dev.Comm.client.InterfaceType == InterfaceType.HdlcWithModeE || dev.Comm.client.InterfaceType == InterfaceType.PlcHdlc)
                    {
                        byte type;
                        GXDLMSClient.GetHdlcAddressInfo(new GXByteBuffer(numArray[0]), out int _, out int _, out type);
                        dev.Comm.client.HdlcSettings.SenderFrame = type;
                    }
                    throw ex;
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Ошибка верификации мастер-прошивки: " + ex.Message);
            }
        }

        internal static void ImageBlockTransfer(GXDLMSDevice dev, GXDLMSImageTransfer current, UpdateImage updateImage)
        {
            GXReplyData reply = new GXReplyData();
            try
            {
                if (updateImage.Image == null)
                    throw new Exception("Ошибка: образ прошивки не был загружен");

                int ImageBlockCount = 0;

                byte[][] data = current.ImageBlockTransfer(dev.Comm.client, updateImage.Image, out ImageBlockCount);

                try
                {
                    string logText = "[" + current.Description + "]: загрузка образа";

                    dev.Comm.ReadDataBlock(data, logText, reply);

                    int num;

                    for (num = 0; num < 60; ++num)
                    {
                        Thread.Sleep(5000);
                        Helpers.ReadAttributeValue(dev, current, 5);
                        if (current.ImageTransferEnabled)
                            break;
                    }
                    if (num >= 60)
                        throw new Exception("таймаут ожидания готовности к верификации образа прошивки");
                }
                catch(Exception ex)
                {
                    if (dev.Comm.client.InterfaceType == InterfaceType.HDLC || dev.Comm.client.InterfaceType == InterfaceType.HdlcWithModeE || dev.Comm.client.InterfaceType == InterfaceType.PlcHdlc)
                    {
                        byte type;
                        GXDLMSClient.GetHdlcAddressInfo(new GXByteBuffer(data[0]), out int _, out int _, out type);
                        dev.Comm.client.HdlcSettings.SenderFrame = type;
                    }
                    throw ex;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
     }
    internal class UpdateImage
    {
        internal bool reading;
        internal bool transfer;
        internal bool verefication;
        internal bool initialization;

        [DefaultValue("")]
        public string ImageIdentifier { get; set; }

        [DefaultValue(null)]
        public byte[] Image { get; set; }

        public long ImageSize { get; set; }
    }
}
