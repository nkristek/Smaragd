using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using NKristek.Smaragd.Attributes;
using NKristek.Smaragd.Commands;
using NKristek.Smaragd.ViewModels;
using Xunit;

namespace NKristek.Smaragd.Tests.Commands
{
    public class AsyncViewModelCommandTests
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

        private class AsyncRelayViewModelCommand
            : AsyncViewModelCommand<TestViewModel>
        {
            private readonly Func<TestViewModel, object, Task> _execute;

            private readonly Func<TestViewModel, object, bool> _canExecute;

            public AsyncRelayViewModelCommand(TestViewModel parent, Func<TestViewModel, object, Task> execute,
                Func<TestViewModel, object, bool> canExecute = null) : base(parent)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            protected override bool CanExecute(TestViewModel viewModel, object parameter)
            {
                return _canExecute?.Invoke(viewModel, parameter) ?? base.CanExecute(viewModel, parameter);
            }

            protected override async Task ExecuteAsync(TestViewModel viewModel, object parameter)
            {
                await _execute.Invoke(viewModel, parameter);
            }
        }

        private class DefaultAsyncViewModelCommand
            : AsyncViewModelCommand<TestViewModel>
        {
            public DefaultAsyncViewModelCommand(TestViewModel parent) : base(parent)
            {
            }

            protected override async Task ExecuteAsync(TestViewModel viewModel, object parameter)
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

        private class CanExecuteSourceAsyncViewModelCommand
            : AsyncViewModelCommand<TestViewModel>
        {
            public CanExecuteSourceAsyncViewModelCommand(TestViewModel parent) : base(parent)
            {
            }

            [CanExecuteSource(nameof(TestViewModel.TestProperty))]
            protected override bool CanExecute(TestViewModel viewModel, object parameter)
            {
                return viewModel.TestProperty;
            }

#pragma warning disable 1998
            protected override async Task ExecuteAsync(TestViewModel viewModel, object parameter)
            {
            }
#pragma warning restore 1998
        }

        [Fact]
        public void Constructor_ParentNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new DefaultAsyncViewModelCommand(null));
        }

        [Fact]
        public void Constructor_ParentNotNull_ThrowsNoException()
        {
            var viewModel = new TestViewModel();
            var command = new DefaultAsyncViewModelCommand(viewModel);
        }

        [Fact]
        public void Name_NotNullOrWhitespace()
        {
            var viewModel = new TestViewModel();
            var command = new DefaultAsyncViewModelCommand(viewModel);
            Assert.False(String.IsNullOrWhiteSpace(command.Name));
        }

        [Fact]
        public void Name_is_the_same_for_the_same_commandType()
        {
            var viewModel = new TestViewModel();
            var firstCommand = new DefaultAsyncViewModelCommand(viewModel);
            var secondCommand = new DefaultAsyncViewModelCommand(viewModel);
            Assert.Equal(firstCommand.Name, secondCommand.Name);
        }

        [Fact]
        public void Name_is_different_for_different_commandTypes()
        {
            var viewModel = new TestViewModel();
            var firstCommand = new DefaultAsyncViewModelCommand(viewModel);
            var secondCommand = new AsyncRelayViewModelCommand(viewModel, (vm, para) => Task.Run(() =>
            {
                if (vm == null)
                    throw new ArgumentNullException(nameof(vm));

                if (para == null)
                    throw new ArgumentNullException(nameof(para));

                if (vm != para)
                    throw new Exception("invalid parameter");
            }));
            Assert.NotEqual(firstCommand.Name, secondCommand.Name);
        }

        [Fact]
        public void Parent_was_set()
        {
            var viewModel = new TestViewModel();
            var command = new DefaultAsyncViewModelCommand(viewModel);
            Assert.Equal(viewModel, command.Parent);
        }

        [Fact]
        public void IsWorking_is_initially_false()
        {
            var viewModel = new TestViewModel();
            var command = new DefaultAsyncViewModelCommand(viewModel);
            Assert.False(command.IsWorking);
        }

        [Fact]
        public void CanExecute_returns_true_by_default()
        {
            var viewModel = new TestViewModel();
            var command = new DefaultAsyncViewModelCommand(viewModel);
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
        private static DefaultAsyncViewModelCommand CreateTestCommandWithDisposedParent()
        {
            var viewModel = new TestViewModel();
            return new DefaultAsyncViewModelCommand(viewModel);
        }

        [Fact]
        public async Task CanExecute_returns_false_if_command_is_working()
        {
            var lockObject = new object();
            var viewModel = new TestViewModel();
            var command = new AsyncRelayViewModelCommand(viewModel, async (vm, para) => await Task.Run(() =>
            {
                lock (lockObject)
                {
                }
            }));

            Task executeTask;
            lock (lockObject)
            {
                executeTask = command.ExecuteAsync(null);
                Assert.False(command.CanExecute(null));
            }

            await executeTask;
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, true)]
        public void CanExecute_With_ViewModel_Is_Evaluated(bool input, bool expectedResult)
        {
            var viewModel = new TestViewModel();
            var command = new AsyncRelayViewModelCommand(viewModel, async (vm, para) => await Task.Run(() => { }),
                (vm, para) => para is bool boolPara && boolPara);
            Assert.Equal(expectedResult, command.CanExecute(input));
        }

        [Fact]
        public void CanExecute_ViewModel_NotNull()
        {
            var canExecuteWasExecuted = false;
            var viewModel = new TestViewModel();
            var command = new AsyncRelayViewModelCommand(viewModel, async (vm, para) => await Task.Run(() => { }), (vm, para) =>
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
        public void ICommandExecute_executes_ExecuteAsync()
        {
            var lockObject = new object();
            var executeWasExecuted = false;
            var viewModel = new TestViewModel();
            var command = new AsyncRelayViewModelCommand(viewModel, async (vm, para) => await Task.Run(() =>
            {
                lock (lockObject)
                {
                    executeWasExecuted = true;
                }
            }));
            ((ICommand) command).Execute(null);

            lock (lockObject)
                Assert.True(executeWasExecuted);
        }

        [Fact]
        public async Task ExecuteAsync_executes_ExecuteAsync_with_ViewModel()
        {
            var executeWasExecuted = false;
            var viewModel = new TestViewModel();
            var command = new AsyncRelayViewModelCommand(viewModel, async (vm, para) => await Task.Run(() => executeWasExecuted = true));
            await command.ExecuteAsync(null);
            Assert.True(executeWasExecuted);
        }

        [Fact]
        public async Task ExecuteAsync_sets_IsWorking()
        {
            var lockObject = new object();
            var viewModel = new TestViewModel();
            var command = new AsyncRelayViewModelCommand(viewModel, async (vm, para) => await Task.Run(() =>
            {
                lock (lockObject)
                {
                }
            }));

            Task executeTask;
            lock (lockObject)
            {
                executeTask = command.ExecuteAsync(null);
                Assert.True(command.IsWorking);
            }

            await executeTask;
        }

        [Fact]
        public async Task ExecuteAsync_ViewModel_NotNull()
        {
            var viewModel = new TestViewModel();
            var command = new AsyncRelayViewModelCommand(viewModel, async (vm, para) =>
            {
                await Task.Run(() =>
                {
                    if (vm == null)
                        throw new ArgumentNullException(nameof(vm));
                    if (vm != viewModel)
                        throw new ArgumentException(nameof(vm));
                });
            });
            await command.ExecuteAsync(null);
        }

        [Fact]
        public async Task ExecuteAsync_Parameter_NotNull()
        {
            var viewModel = new TestViewModel();
            var command = new AsyncRelayViewModelCommand(viewModel, async (vm, para) =>
            {
                await Task.Run(() =>
                {
                    if (para == null)
                        throw new ArgumentNullException(nameof(para));
                    if (para != viewModel)
                        throw new ArgumentException(nameof(para));
                });
            });
            await command.ExecuteAsync(viewModel);
        }

        [Fact]
        public void RaiseCanExecuteChanged_raises_event_on_CanExecuteChanged()
        {
            var invokedCanExecuteChangedEvents = 0;
            var viewModel = new TestViewModel();
            var command = new DefaultAsyncViewModelCommand(viewModel);
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
            var command = new CanExecuteSourceAsyncViewModelCommand(viewModel);
            Assert.Equal(expectedResult, command.ShouldRaiseCanExecuteChanged(Enumerable.Repeat(input, 1)));
        }
    }
}