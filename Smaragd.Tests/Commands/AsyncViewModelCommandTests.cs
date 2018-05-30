using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NKristek.Smaragd.Commands;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Tests.Commands
{
    /// <summary>
    /// Summary description for AsyncViewModelCommandTests
    /// </summary>
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
        public void TestViewModelCommand()
        {
            var viewModel = new TestViewModel();
            var command = new TestCommand(viewModel);
            command.ExecuteAsync(viewModel).Wait();
        }

        [TestMethod]
        public void TestParent()
        {
            var viewModel = new TestViewModel();
            var command = new TestCommand(viewModel);
            Assert.IsNotNull(command.Parent, "The command parent is null");
        }

        [TestMethod]
        public void TestNoParentException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TestCommand(null), "ViewModelCommand did not throw an exception when instancing with no parent");
        }
    }
}
