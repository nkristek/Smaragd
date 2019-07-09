# Smaragd

[![Build Status](https://dev.azure.com/nkristek/Smaragd/_apis/build/status/nkristek.Smaragd?branchName=master)](https://dev.azure.com/nkristek/Smaragd/_build/latest?definitionId=2&branchName=master)
[![Code coverage](https://img.shields.io/azure-devops/coverage/nkristek/Smaragd/2.svg)](https://dev.azure.com/nkristek/Smaragd/_build/latest?definitionId=2&branchName=master)
[![NuGet version](https://img.shields.io/nuget/v/NKristek.Smaragd.svg)](https://www.nuget.org/packages/NKristek.Smaragd/)
[![GitHub license](https://img.shields.io/github/license/nkristek/Smaragd.svg)](https://github.com/nkristek/Smaragd/blob/master/LICENSE)

This library contains base classes for implementing a C# .NET application using the MVVM architecture.

For an example project, please visit my other project [Stein](https://github.com/nkristek/Stein).

## Installation

The recommended way to use this library is via [Nuget](https://www.nuget.org/packages/NKristek.Smaragd/).

Currently supported frameworks:
- .NET Standard 2.0 or higher
- .NET Framework 4.5 or higher

## Getting started

To get started, create a subclass of `ViewModel`:

```csharp
public class MyViewModel : ViewModel
{
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

    [IsReadOnlyIgnored]
    public int SecondProperty
    {
        get => _secondProperty;
        set => SetProperty(ref _secondProperty, value);
    }

    [IsDirtyIgnored]
    [PropertySource(nameof(FirstProperty), nameof(SecondProperty))]
    public int ThirdProperty => FirstProperty + SecondProperty;
}
```

### SetProperty

There are a few things to notice here. Firstly, using `SetProperty()` in setters of properties is **highly** recommended. It will check, if the new value is different from the existing value in the field. If the value is different, it sets it, raises an event on `INotifyPropertyChanging.PropertyChanging` before the value will change and an event on `INotifyPropertyChanged.PropertyChanged` after the value will change and returns true.

In this example, when `FirstProperty` is set to 1, and since it changed, `SecondProperty` will be set to 2. 

### NotifyPropertyChanged / NotifyPropertyChanging

If you want to manually raise an event on `INotifyPropertyChanged.PropertyChanged` or `INotifyPropertyChanging.PropertyChanging`, you should use `NotifyPropertyChanged()` or `NotifyPropertyChanging()` respectively with the name of the property. Using these methods is **highly** recommended, otherwise functionality like the `PropertySourceAttribute` might not work as expected.

### PropertySourceAttribute

`ThirdProperty` uses `PropertySourceAttribute` with the names of both `FirstProperty` and `SecondProperty`. 

`NotifyPropertyChanging` and `NotifyPropertyChanged` will **automatically** raise an event on `INotifyPropertyChanging.PropertyChanging` and `INotifyPropertyChanged.PropertyChanged` respectively for `ThirdProperty` when an event is raised for one of the two properties (in this case `FirstProperty` and `SecondProperty`). 

### IsDirty

`ViewModel` implements an `IsDirty` property which is initially false.
If `INotifyPropertyChanged.PropertyChanged` is raised and the `IsDirtyIgnoredAttribute` is not defined on the property (or on notified properties via `PropertySourceAttribute`), `IsDirty` will be set to `true`. 

For example:
```csharp
private bool _testProperty;

[IsDirtyIgnored]
public bool TestProperty
{
    get => _testProperty;
    set => SetProperty(ref _testProperty, value);
}
```

Now, when `TestProperty` changes, `IsDirty` will **not** be automatically set to true.

Also, if the property implements `INotifyCollectionChanged` and no `IsDirtyIgnoredAttribute` exists, the `ViewModel` will set `IsDirty` when `INotifyCollectionChanged.CollectionChanged` is raised. When the collection property has a setter, please use `SetProperty()` to properly dettach and attach the event handler.

### Parent

The `Parent` property on `ViewModel` internally uses a `WeakReference`.

### IsReadOnly

The `IsReadOnly` property does what it implies, if set to true, `SetProperty()` will now longer set any property or raise events on `INotifyPropertyChanged.PropertyChanged` except for the `IsReadOnly` property itself or any property with the `IsReadOnlyIgnoredAttribute`.

### ValidatingViewModel

Your viewmodel may also inherit from `ValidatingViewModel` which implements `INotifyDataErrorInfo`. 

By using the `SetErrors()` method, the errors of the specified property can be set.

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

`ViewModelCommand<T>` and `AsyncViewModelCommand<T>` provide base implementations for `ICommand` and `IAsyncCommand`.

A basic example of a `ViewModelCommand<T>`

```csharp
public class TestCommand
    : ViewModelCommand<MyViewModel>
{
    protected override bool CanExecute(MyViewModel viewModel, object parameter)
    {
        return viewModel.ThirdProperty > 0;
    }

    protected override void OnParentPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e == null || String.IsNullOrEmpty(e.PropertyName) || e.PropertyName.Equals(nameof(MyViewModel.ThirdProperty)))
            NotifyCanExecuteChanged();
    }

    protected override void Execute(MyViewModel viewModel, object parameter)
    {
        // execute...
    }
}
```

## Attributes when a property is overriden

When a property is overriden in a subclass, attributes are **not** inherited by default. As the overriden property most probably has different dependecies, you should redeclare the `PropertySourceAttribute` with the new dependencies. 

Example:
```csharp
public class Base
{
    [IsDirtyIgnored]
    public bool TestProperty { get; }

    [PropertySource(nameof(TestProperty))]
    public virtual bool AnotherTestProperty => TestProperty;
}

public class Derived : Base
{
    public override bool AnotherTestProperty => true;
}
```

In that example, when using `Derived`, **only** the `IsDirtyIgnoredAttribute` from `TestProperty` is processed since this property is not overriden. 
No event on `PropertyChanged` will be raised for `AnotherTestProperty` when `TestProperty` changes.

If you access the base implementation in the override, you should define a `PropertySourceAttribute` with the `InheritAttributes` option set to true. In that case you can also define additional sources.

Example:
```csharp
public class Base
{
    public bool TestProperty { get; }

    [PropertySource(nameof(TestProperty))]
    public virtual bool AnotherTestProperty => TestProperty;
}

class Derived : Base
{
    public bool NewTestProperty { get; }

    [PropertySource(nameof(NewTestProperty), InheritAttributes = true)]
    public override bool AnotherTestProperty => base.AnotherTestProperty && NewTestProperty;
}
```

## Overview

This library provides the following classes/interfaces:

### ViewModels

Interfaces:
- `IBindable: INotifyPropertyChanging, INotifyPropertyChanged`
- `IViewModel: IBindable`
- `IValidatingViewModel: IViewModel, INotifyDataErrorInfo`
- `IDialogModel: IViewModel`
- `ITreeViewModel: IViewModel`

Classes:
- `Bindable: IBindable`
- `ViewModel: Bindable, IViewModel`
- `ValidatingViewModel: ViewModel, IValidatingViewModel`
- `DialogModel: ValidatingViewModel, IDialogModel`
- `TreeViewModel: ViewModel, ITreeViewModel`

### Attributes:

Attributes are usable on properties of classes inheriting from `ViewModel`:
- `PropertySourceAttribute: Attribute` 
- `IsDirtyIgnoredAttribute: Attribute`
- `IsReadOnlyIgnoredAttribute: Attribute`

### Commands

Interfaces:
- `IAsyncCommand: ICommand`
- `INamedCommand: ICommand`
- `IBindableCommand: ICommand, IBindable`
- `IViewModelCommand: INamedCommand, IBindableCommand`

Classes:
- `ViewModelCommand<TViewModel>: Bindable, IViewModelCommand`
- `AsyncViewModelCommand<TViewModel>: Bindable, IViewModelCommand, IAsyncCommand`

### Validation

Interfaces:
- `IValidation<TValue, TResult>`

Classes:
- `FuncValidation<TValue, TResult>: IValidation<TValue, TResult>`

### Helpers

- `WeakReferenceExtensions`:
    - `WeakReference<T>.TargetOrDefault<T>()`

## Contribution

If you find a bug feel free to open an issue. Contributions are also appreciated.
