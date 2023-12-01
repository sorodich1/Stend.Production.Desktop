//using Newtonsoft.Json;
//using Gurux.DLMS.Objects;

//using System.Collections.Generic;
//using System.Linq;
//using System;

//namespace Script.Test
//{
//    public class RootObject
//    {
//        [JsonProperty("Script")]
//        public Parametrisation Script { get; set; }
//    }
//    public class Parametrisation
//    {
//        List<BaseSteps> steps = null;
//        public List<BaseSteps> baseSteps { get => steps; set => steps = value; }
//        void LoadJson(string str) { }
//        public void Execute()
//        {
//            foreach(var item in steps)
//            {
//                item.ExecuteStep(item.Properties);
//            }
//        }
//    }
//    public class BaseSteps : IStep
//    {
//        public string StepName { get; set; }
//        internal List<object> _properties;
//        public List<object> Properties => _properties;

//        public virtual void ExecuteMetod(int metod, object[] param)
//        {
//            //throw new System.NotImplementedException();
//        }

//        public virtual void ExecuteStep(List<object> Properties)
//        {
//            //throw new System.NotImplementedException();
//        }
//    }
//    public class InicialisationStep : BaseSteps, IStep
//    {
//        private GXDLMSObjectCollection objects;
//        [JsonProperty("Objects")]
//        public GXDLMSObjectCollection Objects
//        {
//            get => objects;
//            set { objects = value; base._properties = objects?.ToList<object>(); }
//        }
//        public new List<object> Properties
//        {
//            get { return base.Properties; }
//        }
//        public new string StepName { get => "InicialisationStep"; }
//        public InicialisationStep()
//        {

//        }
//        public void Method1(object[] param) { }
//        public void Method2() { }

//        public override void ExecuteMetod(int metod, object[] param)
//        {
//            if(metod == 1)
//            {
//                Method1(new object[] { param[0], param[10] });
//            }
//            else
//            {
//                throw new Exception("Метод отсутствует");
//            }
//        }

//        public override void ExecuteStep(List<object> Properties)
//        {
//                if(Properties != null)
//            {
//                Console.WriteLine(StepName);
//            }
//        }
//    }
//    public class Calibration
//    {
//        private GXDLMSObject Object;
//        private double baseValue;
//        private string calibCoef;
//        [JsonProperty("Name")]
//        public string Name { get; set; }
//        [JsonProperty("CalibObject")]
//        public GXDLMSObject CalibObject { get => Object; set => Object = value; }
//        [JsonProperty("BaseValue")]
//        public double BaseValue { get => baseValue; set => baseValue = value; }
//        [JsonProperty("calibCoef")]
//        public string CalibCoef { get => calibCoef; set => calibCoef = value; }
//        [JsonProperty("Script")]
//        public string Script { get => calibCoef; set => calibCoef = value; }
//        public bool DoCalibrate()
//        {
//            return false;
//        }
//    }
//    public class CalibrationStep : BaseSteps, IStep
//    {
//        private List<Calibration> calibrations;
//        public List<Calibration> Calibrations
//        {
//            get { return calibrations; }
//            set { calibrations = value; base._properties = Calibrations?.ToList<object>(); }
//        }
//        [JsonIgnore]
//        public new List<object> Properties
//        {
//            get { return base.Properties; }
//        }
//        public new string StepName => "CalibrationStep";
//        public void Calibration(object[] param)
//        {
//            ((Calibration)param[0]).DoCalibrate();
//        }

//        public override void ExecuteMetod(int metod, object[] param)
//        {
                
//        }

//        public override void ExecuteStep(List<object> Properties)
//        {
                
//        }
//    }
//    public interface IStep
//    {
//        string StepName { get; }
//        List<object> Properties { get; }
//        void ExecuteMetod(int metod, object[] param);
//        void ExecuteStep(List<object> Properties);
//    }
//}
