# Smaragd

[![Build Status](https://dev.azure.com/nkristek/Smaragd/_apis/build/status/nkristek.Smaragd?branchName=master)](https://dev.azure.com/nkristek/Smaragd/_build/latest?definitionId=2&branchName=master)
[![Code coverage](https://img.shields.io/azure-devops/coverage/nkristek/Smaragd/2.svg)](https://dev.azure.com/nkristek/Smaragd/_build/latest?definitionId=2&branchName=master)
[![NuGet version](https://img.shields.io/nuget/v/NKristek.Smaragd.svg)](https://www.nuget.org/packages/NKristek.Smaragd/)
[![GitHub license](https://img.shields.io/github/license/nkristek/Smaragd.svg)](https://github.com/nkristek/Smaragd/blob/master/LICENSE)

This library contains base classes for implementing a C# .NET application using the MVVM architecture.

For an example project, please visit my other project [Stein](https://github.com/nkristek/Stein).

## Installation

The recommended way to use this library is via [Nuget](https://www.nuget.org/packages/NKristek.Smaragd/), but you also can either download the DLL from the latest [release](https://github.com/nkristek/Smaragd/releases/latest) or compile it yourself.

Currently supported frameworks:
- .NET Standard 2.0
- .NET Framework 4.5
- .NET Framework 4.5.1
- .NET Framework 4.5.2
- .NET Framework 4.6
- .NET Framework 4.6.1
- .NET Framework 4.6.2
- .NET Framework 4.7
- .NET Framework 4.7.1
- .NET Framework 4.7.2

## Getting started

To get started, create a subclass of `ViewModel` and optionally a command subclassing `ViewModelCommand<>`:

```csharp
public class MyViewModel : ViewModel
{
    public MyViewModel()
    {
        AddCommand(new TestCommand
        {
            Parent = this
        });
    }

    private int _firstProperty;

    public int FirstProperty
    {
        get => _firstProperty;
        set
        {
            if (SetProperty(ref _firstProperty, value, out _))
                SecondProperty = FirstProperty + 1;
        }
    }

    private int _secondProperty;

    public int SecondProperty
    {
        get => _secondProperty;
        set => SetProperty(ref _secondProperty, value, out _);
    }

    [PropertySource(nameof(FirstProperty), nameof(SecondProperty))]
    public int ThirdProperty => FirstProperty + SecondProperty;
}

public class TestCommand
    : ViewModelCommand<MyViewModel>
{
    [CanExecuteSource(nameof(MyViewModel.ThirdProperty))]
    protected override bool CanExecute(MyViewModel viewModel, object parameter)
    {
        return viewModel.ThirdProperty > 0;
    }

    protected override void Execute(MyViewModel viewModel, object parameter)
    {
        // execute...
    }
}
```

### SetProperty

There are a few things to notice here. Firstly, using `SetProperty()` in setters of properties is **highly** recommended. It will check, if the new value is different from the existing value in the field. If the value is different, it sets it, raise events on `INotifyPropertyChanging.PropertyChanging` and `INotifyPropertyChanged.PropertyChanged` and returns true.

Depending on the type of the `ViewModel` additional logic will be executed, like validation (`ValidatingViewModel`).

In this example, when `FirstProperty` is set to 1, `SecondProperty` will be set to 2. 

### RaisePropertyChanged

If you want to manually raise an event on `INotifyPropertyChanged.PropertyChanged`, you should use `RaisePropertyChanged()` with the name of the property.

Using `RaisePropertyChanged()` instead of directly raising events on `INotifyPropertyChanged.PropertyChanged` directly is **highly** recommended, otherwise `PropertySourceAttribute` and other functionality won't work correctly.

### PropertySourceAttribute

`ThirdProperty` uses `PropertySourceAttribute` with the names of both `FirstProperty` and `SecondProperty`. 

The `ViewModel` will **automatically** raise an event on `INotifyPropertyChanged.PropertyChanged` for `ThirdProperty` when `INotifyPropertyChanged.PropertyChanged` is raised for one of the two properties (in this case `FirstProperty` and `SecondProperty`). 

### CanExecuteSourceAttribute

`TestCommand` uses the `CanExecuteSourceAttribute`, which indicates, that `CanExecute()` depends on the value of the named properties. 

When `INotifyPropertyChanged.PropertyChanged` is raised for `ThirdProperty`, `CanExecuteChanged` will be raised also (when using custom command implementations implement the `IRaiseCanExecuteChanged` interface for this functionality to work).

Setting the `Parent` property of the command attaches the command to `INotifyPropertyChanged.PropertyChanged` and raises the events on `CanExecuteChanged` accordingly. **Please note**, that the `ViewModel` now holds a strong reference. Set the `Parent` property to null to detach the command from `INotifyPropertyChanged.PropertyChanged`.

### IsDirty

`ViewModel` implements an `IsDirty` property which is initially false.
If `RaisePropertyChanged()` is executed and `IsDirtyIgnoredAttribute` is not defined on the property (or on notified properties via `PropertySourceAttribute`), it will set `IsDirty` to `true`. 

For example:
```csharp
private bool _testProperty;

[IsDirtyIgnored]
public bool TestProperty
{
    get => _testProperty;
    set => SetProperty(ref _testProperty, value, out _);
}
```

Now, when `TestProperty` changes, `IsDirty` will **not** be automatically set to true.

Also, if the property implements `INotifyCollectionChanged` and no `IsDirtyIgnoredAttribute` exists, the `ViewModel` will set `IsDirty` when `INotifyCollectionChanged.CollectionChanged` is raised. When the collection property has a setter, please use `SetProperty()` to properly dettach and attach the event handler.

### Parent

The `Parent` property on `ViewModel` internally uses a `WeakReference`.

### IsReadOnly

The `IsReadOnly` property does what it implies, if set to true, `SetProperty()` will now longer set any property or raise events on `INotifyPropertyChanged.PropertyChanged` except for the `IsReadOnly` property itself.

### ValidatingViewModel

Your viewmodel may also inherit from `ValidatingViewModel` which implements `IDataErrorInfo` and `INotifyDataErrorInfo`. 

You may simply add Validations in the class constructor via 
```csharp
AddValidation(() => MyProperty, new PredicateValidation<int>(value => value >= 5, "Value has to be at least 5"));
```

This will execute this validation everytime `SetProperty()` changes this property.

Most of the time `PredicateValidation<T>` should suffice, but if you need something more advanced, you should inherit from `Validation<T>`.

If you want to perform batch operations and want to pause the validation, you can use `IsValidationSuspended`.

### TreeViewModel

This `ViewModel` provides an `IsChecked` implementation to use in a TreeView. It will update its parent `TreeViewModel` and children `TreeViewModel` with appropriate states for `IsChecked`.
If the `TreeViewModel` has children, `TreeChildren` should be overridden accordingly.

Example:
```csharp
private class FolderViewModel
    : TreeViewModel
{
    public ObservableCollection<FolderViewModel> Subfolders { get; } = new ObservableCollection<FolderViewModel>();

    protected override IEnumerable<TreeViewModel> TreeChildren => Subfolders;
}
```

**Please note:** The indeterminate state of `IsChecked` should only be set by updates from child ViewModel's. The `IsChecked` property will be set to `false` if trying to set it to `null`. If you want to set the `IsChecked` property to `null`, you have to use `TreeViewModel.SetIsChecked()`.

### DialogModel

There is also a `DialogModel` class, which inherits from `ValidatingViewModel` and implements a `Title` property to use in your dialog.

### Commands

`ViewModelCommand` and `AsyncViewModelCommand` provide base implementations for `ICommand`, `IAsyncCommand` and `IRaiseCanExecuteChanged`.

`ViewModel` has a `Commands` property and it is recommended to add all available commands to that dictionary using `AddCommand()`.

## Attributes when a property is overriden

When a property is overriden in a subclass, attributes are **not** inherited by default. As the overriden property most probably has different dependecies, you should redeclare the `PropertySourceAttribute` with the new dependencies. 

Example:
```csharp
class Base
{
    [IsDirtyIgnored]
    bool TestProperty { get; }

    [PropertySource(nameof(TestProperty))]
    virtual bool AnotherTestProperty => TestProperty;
}

class Derived : Base
{
    override bool AnotherTestProperty => true;
}
```

In that example, when using `Derived`, **only** the `IsDirtyIgnoredAttribute` from `TestProperty` is processed since this property is not overriden. 
No event on `PropertyChanged` will be raised for `AnotherTestProperty` when `TestProperty` changes.

If you access the base implementation in the override, you should define a `PropertySourceAttribute` with the `InheritAttributes` option set to true. In that case you can also define additional sources.

Example:
```csharp
class Base
{
    bool TestProperty { get; }

    [PropertySource(nameof(TestProperty))]
    virtual bool AnotherTestProperty => TestProperty;
}

class Derived : Base
{
    bool NewTestProperty { get; }

    [PropertySource(nameof(NewTestProperty), InheritAttributes = true)]
    override bool AnotherTestProperty => base.AnotherTestProperty && NewTestProperty;
}
```

## Overview

This library provides the following classes/interfaces:

### ViewModels

Interfaces:
- `IRaisePropertyChanging: INotifyPropertyChanging`
- `IRaisePropertyChanged: INotifyPropertyChanged`
- `IViewModel: IRaisePropertyChanging, IRaisePropertyChanged`
- `IRaiseErrorsChanged: INotifyDataErrorInfo`
- `IValidatingViewModel: IViewModel, IDataErrorInfo, IRaiseErrorsChanged`
- `IDialogModel: IViewModel`
- `ITreeViewModel: IViewModel`

Classes:
- `Bindable: IRaisePropertyChanging, IRaisePropertyChanged`
- `ComputedBindable: Bindable`
- `ViewModel: ComputedBindable, IViewModel`
- `ValidatingViewModel: ViewModel, IValidatingViewModel`
- `DialogModel: ValidatingViewModel, IDialogModel`
- `TreeViewModel: ViewModel, ITreeViewModel`

### Attributes:

Classes:
- `PropertySourceAttribute: Attribute`: usable on properties of classes inheriting from `ComputedBindable` (e.g. `ViewModel`).
- `CommandCanExecuteSourceAttribute: Attribute`: usable on any method called "CanExecute" in a class inheriting from either `ViewModelCommand<TViewModel>` or `AsyncViewModelCommand<TViewModel>`.
- `IsDirtyIgnoredAttribute: Attribute`: usable on properties of classes inheriting from `ViewModel`.

### Commands

Interfaces:
- `IRaiseCanExecuteChanged: ICommand`
- `IAsyncCommand: ICommand`
- `INamedCommand: ICommand`
- `IBindableCommand: ICommand, IRaisePropertyChanging, IRaisePropertyChanged`
- `IViewModelCommand: INamedCommand, IRaiseCanExecuteChanged, IBindableCommand`

Classes:
- `ViewModelCommand<TViewModel>: Bindable, IViewModelCommand`
- `AsyncViewModelCommand<TViewModel>: Bindable, IViewModelCommand, IAsyncCommand`

### Validation

Interfaces:
- `IValidation`

Classes:
- `Validation<T>: IValidation`
- `PredicateValidation<T>: Validation<T>`

## Contribution

If you find a bug feel free to open an issue. Contributions are also appreciated.
