using System;

namespace NKristek.Smaragd.Tests
{
    internal static class GCHelper
    {
        internal static void TriggerGC()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }
}
