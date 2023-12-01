//namespace Test
//{
//    public class Calibrators
//    {
//        public static double Calcs(double BaseValue, double Value, int calibCoef)
//        {
//            if (BaseValue == 0 || Value == 0)
//                return 0;
//            var calc = (Value - BaseValue) / BaseValue * 100;
//            int res = (int)((100 / (calc + 100) - 1) * calibCoef);
//            return res;
//        }
//    }
//}
namespace Test
{
    public class Calibrators
    {
        public static double Calcs(double BaseValue, double Value, int calibCoef)
         {
            if (BaseValue == 0 || Value == 0)
                return 0;
            var calc = (Value - BaseValue) / BaseValue* 100;
            double res = (int)((100 / (calc + 100) - 1) * calibCoef);
            return res;
         }
    }
}
