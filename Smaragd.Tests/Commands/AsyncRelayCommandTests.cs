using Microsoft.VisualStudio.TestTools.UnitTesting;
using NKristek.Smaragd.Commands;

namespace NKristek.Smaragd.Tests.Commands
{
    /// <summary>
    /// Summary description for AsyncRelayCommandTests
    /// </summary>
    [TestClass]
    public class AsyncRelayCommandTests
    {
        [TestMethod]
        public void TestExecute()
        {
            var executeWasCalled = false;
            var command = new AsyncRelayCommand(o => executeWasCalled = true);
            command.ExecuteAsync(null).Wait();
            Assert.IsTrue(executeWasCalled, "Execute was not called");
        }

        [TestMethod]
        public void TestCanExecute()
        {
            var command = new AsyncRelayCommand(o => { }, o => o != null);
            Assert.IsTrue(command.CanExecute(new object()), "CanExecute did not return true");
            Assert.IsFalse(command.CanExecute(null), "CanExecute did not return false");
        }

        [TestMethod]
        public void TestCanExecuteChanged()
        {
            var command = new AsyncRelayCommand(o => { });
            var canExecuteChangedInvokedCount = 0;
            command.CanExecuteChanged += (sender, e) => { canExecuteChangedInvokedCount++; };
            command.RaiseCanExecuteChanged();
            Assert.AreEqual(1, canExecuteChangedInvokedCount, "Invalid count of invocations of the CanExecuteChanged event");
        }
    }
}
