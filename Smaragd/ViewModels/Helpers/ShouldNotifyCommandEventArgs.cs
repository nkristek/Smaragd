using System;

namespace NKristek.Smaragd.ViewModels.Helpers
{
    internal class ShouldNotifyCommandEventArgs
        : EventArgs
    {
        public string CommandNameToNotify { get; }

        internal ShouldNotifyCommandEventArgs(string commandNameToNotify)
        {
            CommandNameToNotify = commandNameToNotify;
        }
    }
}
