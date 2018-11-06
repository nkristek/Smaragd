using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NKristek.Smaragd.Commands;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Tests.Commands
{
    [TestClass]
    public class ViewModelCommandTests
    {
        private class TestViewModel
            : ViewModel
        {

        }

        private class TestCommand
            : ViewModelCommand<TestViewModel>
        {
            public TestCommand(TestViewModel parent) : base(parent) { }

            protected override void DoExecute(TestViewModel viewModel, object parameter)
            {
                if (viewModel == null)
                    throw new ArgumentNullException(nameof(viewModel));

                if (parameter == null)
                    throw new ArgumentNullException(nameof(parameter));

                if (viewModel != parameter)
                    throw new Exception("invalid parameter");
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
            Assert.ThrowsException<ArgumentNullException>(() => new TestCommand(null), "ViewModelCommand did not throw an exception when instancing with no parent");
        }

        [TestMethod]
        public void TestCanExecute()
        {
            Assert.IsTrue(new TestCommand(new TestViewModel()).CanExecute(null), "CanExecute() should return true by default.");
        }

        [TestMethod]
        public void TestDoExecute()
        {
            var viewModel = new TestViewModel();
            var command = new TestCommand(viewModel);
            command.Execute(viewModel);
            Assert.ThrowsException<ArgumentNullException>(() => command.Execute(null), "DoExecute() should throw an exception when the parameter is null.");
        }
    }
}
