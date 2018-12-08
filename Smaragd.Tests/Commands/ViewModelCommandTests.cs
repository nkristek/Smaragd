using System;
using System.Linq;
using System.Runtime.CompilerServices;
using NKristek.Smaragd.Attributes;
using NKristek.Smaragd.Commands;
using NKristek.Smaragd.ViewModels;
using Xunit;

namespace NKristek.Smaragd.Tests.Commands
{
    public class ViewModelCommandTests
    {
        private class TestViewModel
            : ViewModel
        {
            private bool _testProperty;

            public bool TestProperty
            {
                get => _testProperty;
                set => SetProperty(ref _testProperty, value, out _);
            }
        }

        private class RelayViewModelCommand
            : ViewModelCommand<TestViewModel>
        {
            private readonly Action<TestViewModel, object> _execute;

            private readonly Func<TestViewModel, object, bool> _canExecute;

            public RelayViewModelCommand(TestViewModel parent, Action<TestViewModel, object> execute,
                Func<TestViewModel, object, bool> canExecute = null) : base(parent)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }
            
            protected override bool CanExecute(TestViewModel viewModel, object parameter)
            {
                return _canExecute?.Invoke(viewModel, parameter) ?? base.CanExecute(viewModel, parameter);
            }

            protected override void Execute(TestViewModel viewModel, object parameter)
            {
                _execute.Invoke(viewModel, parameter);
            }
        }

        private class DefaultViewModelCommand
            : ViewModelCommand<TestViewModel>
        {
            public DefaultViewModelCommand(TestViewModel parent) : base(parent) { }

            protected override void Execute(TestViewModel viewModel, object parameter)
            {
                if (viewModel == null)
                    throw new ArgumentNullException(nameof(viewModel));

                if (parameter == null)
                    throw new ArgumentNullException(nameof(parameter));

                if (viewModel != parameter)
                    throw new Exception("invalid parameter");
            }
        }

        private class CanExecuteSourceViewModelCommand
            : ViewModelCommand<TestViewModel>
        {
            public CanExecuteSourceViewModelCommand(TestViewModel parent) : base(parent) { }
            
            [CanExecuteSource(nameof(TestViewModel.TestProperty))]
            protected override bool CanExecute(TestViewModel viewModel, object parameter)
            {
                return viewModel.TestProperty;
            }

            protected override void Execute(TestViewModel viewModel, object parameter) { }
        }

        [Fact]
        public void Constructor_ParentNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new DefaultViewModelCommand(null));
        }

        [Fact]
        public void Constructor_ParentNotNull_ThrowsNoException()
        {
            var viewModel = new TestViewModel();
            var command = new DefaultViewModelCommand(viewModel);
        }

        [Fact]
        public void Name_NotNullOrWhitespace()
        {
            var viewModel = new TestViewModel();
            var command = new DefaultViewModelCommand(viewModel);
            Assert.False(String.IsNullOrWhiteSpace(command.Name));
        }

        [Fact]
        public void Name_is_the_same_for_the_same_commandType()
        {
            var viewModel = new TestViewModel();
            var firstCommand = new DefaultViewModelCommand(viewModel);
            var secondCommand = new DefaultViewModelCommand(viewModel);
            Assert.Equal(firstCommand.Name, secondCommand.Name);
        }

        [Fact]
        public void Name_is_different_for_different_commandTypes()
        {
            var viewModel = new TestViewModel();
            var firstCommand = new DefaultViewModelCommand(viewModel);
            var secondCommand = new RelayViewModelCommand(viewModel, (vm, para) =>
            {
                if (vm == null)
                    throw new ArgumentNullException(nameof(vm));

                if (para == null)
                    throw new ArgumentNullException(nameof(para));

                if (vm != para)
                    throw new Exception("invalid parameter");
            });
            Assert.NotEqual(firstCommand.Name, secondCommand.Name);
        }

        [Fact]
        public void Parent_was_set()
        {
            var viewModel = new TestViewModel();
            var command = new DefaultViewModelCommand(viewModel);
            Assert.Equal(viewModel, command.Parent);
        }

        [Fact]
        public void CanExecute_returns_true_by_default()
        {
            var viewModel = new TestViewModel();
            var command = new DefaultViewModelCommand(viewModel);
            Assert.True(command.CanExecute(null));
        }

        [Fact]
        public void Parent_disposed_is_null()
        {
            var command = CreateTestCommandWithDisposedParent();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Assert.Null(command.Parent);
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private static DefaultViewModelCommand CreateTestCommandWithDisposedParent()
        {
            var viewModel = new TestViewModel();
            return new DefaultViewModelCommand(viewModel);
        }
        
        [Theory]
        [InlineData(false, false)]
        [InlineData(true, true)]
        public void CanExecute_With_ViewModel_Is_Evaluated(bool input, bool expectedResult)
        {
            var viewModel = new TestViewModel();
            var command = new RelayViewModelCommand(viewModel, (vm, para) => { }, (vm, para) => para is bool boolPara && boolPara);
            Assert.Equal(expectedResult, command.CanExecute(input));
        }

        [Fact]
        public void CanExecute_ViewModel_NotNull()
        {
            var canExecuteWasExecuted = false;
            var viewModel = new TestViewModel();
            var command = new RelayViewModelCommand(viewModel, (vm, para) => { }, (vm, para) =>
            {
                if (vm == null)
                    throw new ArgumentNullException(nameof(vm));
                if (vm != viewModel)
                    throw new ArgumentException(nameof(vm));
                canExecuteWasExecuted = true;
                return true;
            });
            command.CanExecute(null);
            Assert.True(canExecuteWasExecuted);
        }

        [Fact]
        public void ICommandExecute_executes_Execute()
        {
            var executeWasExecuted = false;
            var viewModel = new TestViewModel();
            var command = new RelayViewModelCommand(viewModel, (vm, para) => executeWasExecuted = true);
            command.Execute(null);
            Assert.True(executeWasExecuted);
        }
        
        [Fact]
        public void Execute_ViewModel_NotNull()
        {
            var viewModel = new TestViewModel();
            var command = new RelayViewModelCommand(viewModel, (vm, para) =>
            {
                if (vm == null)
                    throw new ArgumentNullException(nameof(vm));
                if (vm != viewModel)
                    throw new ArgumentException(nameof(vm));
            });
            command.Execute(null);
        }
        
        [Fact]
        public void Execute_Parameter_NotNull()
        {
            var viewModel = new TestViewModel();
            var command = new RelayViewModelCommand(viewModel, (vm, para) =>
            {
                if (para == null)
                    throw new ArgumentNullException(nameof(para));
                if (para != viewModel)
                    throw new ArgumentException(nameof(para));
            });
            command.Execute(viewModel);
        }
        
        [Fact]
        public void RaiseCanExecuteChanged_raises_event_on_CanExecuteChanged()
        {
            var invokedCanExecuteChangedEvents = 0;
            var viewModel = new TestViewModel();
            var command = new DefaultViewModelCommand(viewModel);
            command.RaiseCanExecuteChanged();
            command.CanExecuteChanged += (sender, args) => invokedCanExecuteChangedEvents++;
            command.RaiseCanExecuteChanged();
            Assert.Equal(1, invokedCanExecuteChangedEvents);
        }
        
        [Theory]
        [InlineData("TestProperty", true)]
        [InlineData("NotTestProperty", false)]
        public void ShouldRaiseCanExecuteChanged(string input, bool expectedResult)
        {
            var viewModel = new TestViewModel();
            var command = new CanExecuteSourceViewModelCommand(viewModel);
            Assert.Equal(expectedResult, command.ShouldRaiseCanExecuteChanged(Enumerable.Repeat(input, 1)));
        }
    }
}
