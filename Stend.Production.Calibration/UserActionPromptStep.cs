using Stend.Production.Root;
using System;
using System.Collections.Generic;
using System.Threading;
using ZIP.DLMS;

namespace Stend.Production.Calibration
{
    public class UserActionPromptStep : PlugIn, ISteps
    {
        private string _stepName = "Пауза для калибровки измерителей ПУ";
        private string _Description = "Выполняет остановку перед калибровкой нетрали ПУ";
        private bool _reconnectDevice = true;

        public string Name { get; } = null;

        public object Tag { get; set; }

        public bool ReconnectDevices { get => _reconnectDevice; set => _reconnectDevice = value; }

        public string StepName { get => _stepName; set => _stepName = value; }

        public string Version { get; }

        public string Description { get => _Description; set => _Description = value; }

        public List<PluginBase> InternalSteps { get; }

        public EnumStepStatus Status { get; set; } = EnumStepStatus.None;

        public void ExecuteStep(GXDLMSDevice device)
        {
            try
            {
                Status = EnumStepStatus.Run;

                if (ReconnectDevices)
                    device.Disconnect();

                int stepCount = 0;

                while (stepCount < 300)
                {
                    Thread.Sleep(1000);
                    stepCount++;

                    if (Tag is UserActionStatus status)
                    {
                        if (status == UserActionStatus.Ok)
                        {
                            break;
                        }
                        else
                        {
                            if (status == UserActionStatus.Abort)
                            {
                                throw new Exception("Операция прервана оператором");
                            }
                        }
                    }
                }
                if (stepCount >= 300)
                {
                    throw new Exception("Таймаут ожидания ответа оператора");
                }
                if (ReconnectDevices)
                    device.InitializeConnection();
                Status = EnumStepStatus.Succes;
            }
            catch (Exception ex)
            {
                Status = EnumStepStatus.Fatal;
                throw;
            }
        }

        public void Show()
        {
            throw new NotImplementedException();
        }
    }
}
