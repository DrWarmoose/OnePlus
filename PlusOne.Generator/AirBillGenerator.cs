#region Copyright © 2019, Warmoose Design Group
#endregion

namespace PlusOne.Generator
{
    public static class AirBillGenerator
    {
        private static long _airBillNumber = 0;
        
        public static string Create()
        {
            return $"SATX-{_airBillNumber++}";
        }
    }
}