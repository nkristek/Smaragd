using System;
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
            public TestViewModel()
            {
                var testCommand = new DefaultAsyncViewModelCommand
                {
                    Parent = this
                };
                AddCommand(testCommand);
            }
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

            public AsyncRelayViewModelCommand(Func<TestViewModel, object, Task> execute,
                Func<TestViewModel, object, bool> canExecute = null)
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
        public void Name_NotNullOrWhitespace()
        {
            var command = new DefaultAsyncViewModelCommand();
            Assert.False(String.IsNullOrWhiteSpace(command.Name));
        }

        [Fact]
        public void Name_is_the_same_for_the_same_commandType()
        {
            var firstCommand = new DefaultAsyncViewModelCommand();
            var secondCommand = new DefaultAsyncViewModelCommand();
            Assert.Equal(firstCommand.Name, secondCommand.Name);
        }

        [Fact]
        public void Name_is_different_for_different_commandTypes()
        {
            var firstCommand = new DefaultAsyncViewModelCommand();
            var secondCommand = new AsyncRelayViewModelCommand((vm, para) => Task.Run(() =>
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
        public void IsWorking_is_initially_false()
        {
            var command = new DefaultAsyncViewModelCommand();
            Assert.False(command.IsWorking);
        }

        [Fact]
        public void CanExecute_returns_true_by_default()
        {
            var command = new DefaultAsyncViewModelCommand();
            Assert.True(command.CanExecute(null));
        }

        [Fact]
        public void Parent_disposed_is_null()
        {
            var command = CreateTestCommandWithDisposedParent();
            GCHelper.TriggerGC();
            Assert.Null(command.Parent);
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private static DefaultAsyncViewModelCommand CreateTestCommandWithDisposedParent()
        {
            var viewModel = new TestViewModel();
            return new DefaultAsyncViewModelCommand
            {
                Parent = viewModel
            };
        }

        [Fact]
        public async Task CanExecute_returns_false_if_command_is_working()
        {
            var lockObject = new object();
            var command = new AsyncRelayViewModelCommand(async (vm, para) => await Task.Run(() =>
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
            var command = new AsyncRelayViewModelCommand(async (vm, para) => await Task.Run(() => { }),
                (vm, para) => para is bool boolPara && boolPara);
            Assert.Equal(expectedResult, command.CanExecute(input));
        }

        [Fact]
        public void CanExecute_ViewModel_NotNull()
        {
            var canExecuteWasExecuted = false;
            var viewModel = new TestViewModel();
            var command = new AsyncRelayViewModelCommand(async (vm, para) => await Task.Run(() => { }), (vm, para) =>
            {
                if (vm == null)
                    throw new ArgumentNullException(nameof(vm));
                if (vm != viewModel)
                    throw new ArgumentException(nameof(vm));
                canExecuteWasExecuted = true;
                return true;
            })
            {
                Parent = viewModel
            };
            command.CanExecute(null);
            Assert.True(canExecuteWasExecuted);
        }

        [Fact]
        public void ICommandExecute_executes_ExecuteAsync()
        {
            var executeWasExecuted = false;
            var command = new AsyncRelayViewModelCommand(async (vm, para) => await Task.Run(() => executeWasExecuted = true));
            ((ICommand) command).Execute(null);
            while (command.IsWorking) ;
            Assert.True(executeWasExecuted);
        }

        [Fact]
        public async Task ExecuteAsync_executes_ExecuteAsync_with_ViewModel()
        {
            var executeWasExecuted = false;
            var command = new AsyncRelayViewModelCommand(async (vm, para) => await Task.Run(() => executeWasExecuted = true));
            await command.ExecuteAsync(null);
            Assert.True(executeWasExecuted);
        }

        [Fact]
        public async Task ExecuteAsync_sets_IsWorking()
        {
            var lockObject = new object();
            var command = new AsyncRelayViewModelCommand(async (vm, para) => await Task.Run(() =>
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
            var command = new AsyncRelayViewModelCommand(async (vm, para) =>
            {
                await Task.Run(() =>
                {
                    if (vm == null)
                        throw new ArgumentNullException(nameof(vm));
                    if (vm != viewModel)
                        throw new ArgumentException(nameof(vm));
                });
            })
            {
                Parent = viewModel
            };
            await command.ExecuteAsync(null);
        }

        [Fact]
        public async Task ExecuteAsync_Parameter_NotNull()
        {
            var viewModel = new TestViewModel();
            var command = new AsyncRelayViewModelCommand(async (vm, para) =>
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
            var command = new DefaultAsyncViewModelCommand();
            command.RaiseCanExecuteChanged();
            command.CanExecuteChanged += (sender, args) => invokedCanExecuteChangedEvents++;
            command.RaiseCanExecuteChanged();
            Assert.Equal(1, invokedCanExecuteChangedEvents);
        }

        [Fact]
        public void ParentPropertyChanged_raises_event_on_CanExecuteChanged()
        {
            var viewModel = new TestViewModel();
            var command = new CanExecuteSourceAsyncViewModelCommand
            {
                Parent = viewModel
            };

            var invokedCanExecuteChangedEvents = 0;
            command.CanExecuteChanged += (sender, args) => invokedCanExecuteChangedEvents++;
            viewModel.TestProperty = true;
            Assert.Equal(1, invokedCanExecuteChangedEvents);
        }

        [Fact]
        public void Parent_set()
        {
            var viewModel = new TestViewModel();
            var command = new DefaultAsyncViewModelCommand
            {
                Parent = viewModel
            };
            Assert.Equal(viewModel, command.Parent);
        }

        [Fact]
        public void Parent_set_twice()
        {
            var viewModel = new TestViewModel();
            var command = new DefaultAsyncViewModelCommand
            {
                Parent = viewModel
            };
            command.Parent = viewModel;
            Assert.Equal(viewModel, command.Parent);
        }

        [Fact]
        public void Parent_set_after_set()
        {
            var firstViewModel = new TestViewModel();
            var secondViewModel = new TestViewModel();
            var command = new DefaultAsyncViewModelCommand
            {
                Parent = firstViewModel
            };
            command.Parent = secondViewModel;
            Assert.Equal(secondViewModel, command.Parent);
        }

        [Fact]
        public void Parent_set_to_null()
        {
            var viewModel = new TestViewModel();
            var command = new DefaultAsyncViewModelCommand
            {
                Parent = viewModel
            };

            command.Parent = null;
            Assert.Null(command.Parent);
        }

        [Fact]
        public async Task ExecuteAsync_when_CanExecute_false()
        {
            var didExecute = false;
            var command = new AsyncRelayViewModelCommand(async (viewModel, parameter) => didExecute = true, (viewModel, parameter) => false);
            await command.ExecuteAsync(null);
            Assert.False(didExecute);
        }
    }
}