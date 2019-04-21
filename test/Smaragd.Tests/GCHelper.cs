using System;

namespace NKristek.Smaragd.Tests
{
    internal static class GCHelper
    {
        public static void TriggerGC()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }
}
