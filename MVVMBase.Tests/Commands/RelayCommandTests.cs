using Microsoft.VisualStudio.TestTools.UnitTesting;
using nkristek.MVVMBase.Commands;

namespace nkristek.MVVMBase.Tests.Commands
{
    /// <summary>
    /// Summary description for RelayCommandTests
    /// </summary>
    [TestClass]
    public class RelayCommandTests
    {
        [TestMethod]
        public void TestExecute()
        {
            var executeWasCalled = false;
            var command = new RelayCommand(o => executeWasCalled = true);
            command.Execute(null);
            Assert.IsTrue(executeWasCalled, "Execute was not called");
        }

        [TestMethod]
        public void TestCanExecute()
        {
            var command = new RelayCommand(o => { }, o => o != null);
            Assert.IsTrue(command.CanExecute(new object()), "CanExecute did not return true");
            Assert.IsFalse(command.CanExecute(null), "CanExecute did not return false");
        }

        [TestMethod]
        public void TestCanExecuteChanged()
        {
            var command = new RelayCommand(o => { });
            var canExecuteChangedInvokedCount = 0;
            command.CanExecuteChanged += (sender, e) => { canExecuteChangedInvokedCount++; };
            command.RaiseCanExecuteChanged();
            Assert.AreEqual(1, canExecuteChangedInvokedCount, "Invalid count of invocations of the CanExecuteChanged event");
        }
    }
}
