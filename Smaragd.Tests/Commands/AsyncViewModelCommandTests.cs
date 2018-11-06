using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NKristek.Smaragd.Commands;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Tests.Commands
{
    [TestClass]
    public class AsyncViewModelCommandTests
    {
        private class TestViewModel
            : ViewModel
        {

        }

        private class TestCommand
            : AsyncViewModelCommand<TestViewModel>
        {
            public TestCommand(TestViewModel parent) : base(parent) { }

            protected override async Task DoExecute(TestViewModel viewModel, object parameter)
            {
                await Task.Run(() =>
                {
                    if (viewModel == null)
                        throw new ArgumentNullException(nameof(viewModel));

                    if (parameter == null)
                        throw new ArgumentNullException(nameof(parameter));

                    if (viewModel != parameter)
                        throw new Exception("invalid parameter");
                });
            }
        }

        [TestMethod]
        public void TestParent()
        {
            var viewModel = new TestViewModel();
            var command = new TestCommand(viewModel);
            Assert.AreEqual(viewModel, command.Parent, "The command parent does not match");
        }

        [TestMethod]
        public void TestNoParentException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TestCommand(null), "AsyncViewModelCommand did not throw an exception when instancing with no parent");
        }

        [TestMethod]
        public void TestCanExecute()
        {
            Assert.IsTrue(new TestCommand(new TestViewModel()).CanExecute(null), "CanExecute() should return true by default.");
        }

        [TestMethod]
        public async Task TestDoExecute()
        {
            var viewModel = new TestViewModel();
            var command = new TestCommand(viewModel);
            await command.ExecuteAsync(viewModel);
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await command.ExecuteAsync(null), "DoExecute() should throw an exception when the parameter is null.");
        }
    }
}
