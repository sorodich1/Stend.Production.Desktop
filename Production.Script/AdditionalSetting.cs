using System;
using ZIP.DLMS;

namespace Production.Script
{
    public class AdditionalSetting : BaseStep
    {
        public AdditionalSetting() : base()
        {
            StepName = "Дополнительные настройки";
        }

        public string Value { get; set; }

        public override void ExecuteStep()
        {
            if(Owner != null)
            {
                GXDLMSDevice device = Owner.Device;

                if(device != null && InternalSteps != null)
                {
                    try
                    {
                        foreach(var value in InternalSteps)
                        {
                            if(value is BaseStep internalSteps)
                            {
                                internalSteps.Owner = Owner;
                                Value = Owner.StendSettings.InactivityTimeout;
                                internalSteps.ExecuteStep();
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        Status = EnumStepStatus.Fatal;
                        FireEvent(ex.Message);
                        throw new Exception(ex.Message);
                    }
                }
            }
        }
    }
}
