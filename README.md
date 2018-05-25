# Smaragd
This library contains base classes for implementing a C# .NET application using the MVVM architecture.

For an example project, please visit my other project [Stein](https://github.com/nkristek/Stein).

## Prerequisites

The nuget package and [DLL](https://github.com/nkristek/Smaragd/releases) are built using .NET 4.7, but you can compile the library yourself to fit your needs.

## Installation

The recommended way to use this library is via [Nuget](https://www.nuget.org/packages/NKristek.Smaragd/), but you also can either download the DLL from the latest [release](https://github.com/nkristek/Smaragd/releases) or compile it yourself.

## How to use

To get started, create a subclass of `ViewModel` like shown below.

```csharp
public class MyViewModel : ViewModel
{
    public MyViewModel()
    {
        DoStuffCommand = new DoStuffCommand(this);
    }

    private int _firstProperty;
    public int FirstProperty
    {
        get => _firstProperty;
        set
        {
            if (SetProperty(ref _firstProperty, value))
                SecondProperty = FirstProperty + 1;
        }
    }

    private int _secondProperty;
    public int SecondProperty
    {
        get => _secondProperty;
        set => SetProperty(ref _secondProperty, value);
    }

    [PropertySource(nameof(FirstProperty), nameof(SecondProperty))]
    public int ThirdProperty => FirstProperty + SecondProperty;

    [CommandCanExecuteSource(nameof(ThirdProperty))]
    public ViewModelCommand<MainWindowViewModel> DoStuffCommand { get; }
}
```

### SetProperty

There are a few things to notice here. Firstly, in setters of properties the use of `SetProperty()` is highly recommended. It will check if the new value is different from the existing value in the field and sets it if different. If the value changed it will return true and raise an event on the PropertyChanged event handler with the property name using `[CallerMemberName]`.

So, when `FirstProperty` is set to 1, `SecondProperty` will be set to 2. 

### RaisePropertyChanged

If you want to manually raise a `PropertyChanged` event, you can use `RaisePropertyChanged()` with the name of the property, but under normal conditions, this should not be necessary, since `SetProperty()` already does this.

### PropertySource

`ThirdProperty` uses the `PropertySource` attribute with the names of both `FirstProperty` and `SecondProperty`. The `ViewModel` will raise a `PropertyChanged` event for `ThirdProperty` when a `PropertyChanged` event is raised for either of these properties.

### CommandCanExecuteSource

The `DoStuffCommand` uses the `CommandCanExecuteSource` attribute, which indicates, that the `CanExecute()` method depends on the value of the properties named. Now, when a `PropertyChanged` event for `ThirdProperty` is invoked, a `CanExecuteChanged` event is raised on the command (when using custom command implementations, also implement the `IRaiseCanExecuteChanged` interface for this functionality to work).

### IsDirty

`ViewModel` implements an `IsDirty` property which is initially false and set to true, if an `PropertyChanged` event is raised for a property name which is not ignored (`IsDirty`, `Parent` and `IsReadOnly` are ignored by default, override `GetIsDirtyIgnoredPropertyNames()` to change this).

### Parent

The `Parent` property on `ViewModel` uses a `WeakReference` internally, a reference cycle is unlikely.

### IsReadOnly

The `IsReadOnly` property does what it implies, if set to true, `SetProperty` will now longer set any property or raise a `PropertyChanged` event except when for the `IsReadOnly` property itself.

### Nested ViewModels

If the property is a also a `ViewModel`, `SetProperty` will automatically call `UnregisterChildViewModel()` on the old value and `RegisterChildViewModel()` on the new value. This enables, that `PropertyChanged` events get raised for the child viewmodel property, when a `PropertyChanged` event is raised on the child viewmodel itself. It may make some bindings easier.

### ValidatingViewModel

Your custom viewmodel may also be of type `ValidatingViewModel` which also implements `IDataErrorInfo` and `INotifyDataErrorInfo`. It exposes `SetValidationError()` and `SetValidationErrors()` methods which could be used in the property setter of a property to validate. The `IsValid` property indicates, if validation errors are present.

**Please note**: Using this method, initially, there is no validation error, since the setter hasn't been called yet. You may call `Validate()` on the `ViewModel` after initializing.

### TreeViewModel

This `ViewModel` provides an `IsChecked` implementation to use in a TreeView. It will update its parent `TreeViewModel` and children `TreeViewModel` with appropriate states for `IsChecked`.

### DialogModel

There is also a `DialogModel` class, which inherits from `ValidatingViewModel` and implements a `Title` property to use in your dialog.

### Commands

`ViewModelCommand` and `AsyncViewModelCommand` provide base implementations for `ICommand` and `IRaiseCanExecuteChanged`.

## Overview

This library provides the following classes:

### ViewModels namespace

Attributes:
- PropertySource (usable through ComputedBindableBase)
- CommandCanExecuteSource (usable through ComputedBindableBase)
- InitiallyNotValid (usable through ValidatingViewModel)

Sorted by inheritance:
- BindableBase
- ComputedBindableBase
- ViewModel
- ValidatingViewModel
- DialogModel
- TreeViewModel

### Commands namespace

Interfaces:
- IRaiseCanExecuteChanged
- IAsyncCommand

Classes sorted and grouped by inheritance:

Non-async:
- Command
- RelayCommand

- BindableCommand
- ViewModelCommand

Async:
- AsyncCommand
- AsyncRelayCommand

- AsyncBindableCommand
- AsyncViewModelCommand

## Contribution

If you find a bug feel free to open an issue. Contributions are also appreciated.
