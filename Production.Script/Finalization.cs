using Ankom.Common;
using Gurux.DLMS.ManufacturerSettings;
using System;
using System.Threading;
using ZIP.DLMS;

namespace Production.Script
{
    public class Finalization : BaseStep
    {
        public Finalization() : base()
        {
            StepName = "Сброс производственного режима";
        }

        /// <summary>
        /// Выполняет действия по калибровке прибора учета
        /// </summary>
        public override void ExecuteStep()
        {
            if (Owner != null)
            {
                GXDLMSDevice device = Owner.Device;


                if (device != null && InternalSteps != null)
                {
                    Status = EnumStepStatus.Run;
                    FireEvent("запущено на выполнение");
                    if (InternalSteps.Count > 0)
                        try
                        {
                            foreach (var value in InternalSteps)
                            {
                                if (value is BaseStep internalStep)
                                {
                                    internalStep.Owner = Owner;
                                    try
                                    {
                                        if(internalStep.Owner.StendSettings.IsStopFinalization == false)
                                            internalStep.ExecuteStep();
                                    }
                                    catch
                                    {
                                        internalStep.Status = EnumStepStatus.Fatal;
                                        throw;
                                    }
                                    internalStep.Status = EnumStepStatus.Succes;
                                }
                                else
                                    throw new Exception($"ожидается шаг задания типа BaseStep, получен {value.GetType().Name}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Status = EnumStepStatus.Fatal;
                            string err = $"{Status.ToName()} выполнения шага [{StepName}]: {ex.Message}";
                            FireEvent(err/*$"{ex.Message}"*/);
                            throw new Exception(err);
                        }
                    Status = EnumStepStatus.Succes;
                    FireEvent(Status.ToName());
                }
            }
            else
            {
                Status = EnumStepStatus.Fatal;
                string err = $"{Status.ToName()} выполнения шага [{StepName}]: отсутствуют настройки места прошивки";
                FireEvent(err);
                throw new Exception(err);
            }
        }
    }

    public class ResetDevices : BaseStep
    {
        public string AuthName { get; set; }

        public HDLCAddressType TypeHDLS { get; set; }

        public int PhAddress { get; set; }

        public int ClAddress { get; set; }

        public bool RebootNeeded { get; set; } = true;

        public ResetDevices() => this.StepName = "Перезагрузка устройства";

        public override void ExecuteStep()
        {
            try
            {
                if (Owner != null && Owner.Device is GXDLMSDevice device)
                {
                    int num;
                    device = Owner.Device;
                    if(device != null)
                    {
                        num = !this.Owner.StendSettings.IsStopFinalization ? 1 : 0;
                    }
                    device.Disconnect();
                    Status = EnumStepStatus.Run;
                    FireEvent("Перезагрузка в режиме 'Конфигуратор'");

                    if(RebootNeeded)
                    {
                        device.AuthenticationName = AuthName;
                        device.HDLCAddressing = TypeHDLS;
                        device.ClientAddress = ClAddress;
                        device.PhysicalAddress = int.Parse(Owner.Serial);
                        device.InitializeConnection();
                        Thread.Sleep(5000);
                        Status = EnumStepStatus.Succes;
                        FireEvent("Перезагрузка в режиме 'Конфигуратор'");
                        device.Disconnect();
                    }
                }
            }
            catch(Exception ex)
            {
                Status = EnumStepStatus.Fatal;
                FireEvent(ex.Message ?? "");
                throw;
            }
        }
    }
}