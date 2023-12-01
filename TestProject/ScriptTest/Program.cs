using NLog;
using System;
using Test;

namespace ScriptTest
{
    class Program
    {
        //public static RootObject productionScript = new RootObject()
        //{
        //    Script = new Parametrisation()
        //    {
        //        baseSteps = new System.Collections.Generic.List<BaseSteps>()
        //        {
        //            new InicialisationStep()
        //            {
        //                Objects = new Gurux.DLMS.Objects.GXDLMSObjectCollection()
        //                {
        //                    new Gurux.DLMS.Objects.GXDLMSScriptTable("0.0.10.164.1.255"),
        //                    new Gurux.DLMS.Objects.GXDLMSScriptTable("0.0.10.164.0.255")
        //                }
        //            },
        //            new CalibrationStep()
        //            {
        //                Calibrations = new System.Collections.Generic.List<Calibration>()
        //                {
        //                    new Calibration()
        //                    {
        //                        Name = "Калибровка счётчика",
        //                        BaseValue = 220,
        //                        CalibObject = new Gurux.DLMS.Objects.GXDLMSRegister("1.0.0.1.0.255"),
        //                        CalibCoef = "20345",
        //                        Script = "C# script калибровки напряжения"
        //                    },
        //                    new Calibration()
        //                    {
        //                        Name = "Калибровка тока",
        //                        BaseValue = 15.0,
        //                        CalibObject = new Gurux.DLMS.Objects.GXDLMSRegister("1.0.0.1.0.255"),
        //                        CalibCoef = "20345",
        //                        Script = "C# script калибровки тока"
        //                    }
        //                }
        //            }
        //        }
        //    }
        //};
        public static Logger Log = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            //Log.Info("Hello World!");
            //var jsonSerializerSettings = new JsonSerializerSettings()
            //{
            //    TypeNameHandling = TypeNameHandling.All,
            //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            //};
            //string json = File.ReadAllText("../../../testScript.json");
            //productionScript = JsonConvert.DeserializeObject<RootObject>(json, jsonSerializerSettings);
            //string jsonTest = JsonConvert.SerializeObject(productionScript, Formatting.Indented, jsonSerializerSettings);

            // Console.WriteLine(jsonTest);
            //Console.ReadKey();
            //string json = JsonConvert.SerializeObject(productionScript, Formatting.Indented, jsonSerializerSettings);
            //Console.WriteLine("Введите BaseValue");
            //string a = Console.ReadLine();
            //Console.WriteLine("Введите RealValue");
            //string b = Console.ReadLine();

            //var a1 = Convert.ToInt32(a);
            //var b1 = Convert.ToInt32(b);
            //Console.WriteLine("");
            //Console.WriteLine("");

            //  var str = Calibrators.Calcs(430, 209.82, 32767);
            int i = 1;
            string str = "123456";
            var index = Convert.ToInt32(str);

             Console.WriteLine(index);

            //Console.WriteLine(str);
            Console.ReadKey();
        }
    }
}
