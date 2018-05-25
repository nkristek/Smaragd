# Smaragd
This library contains base classes for implementing a C# .NET application using the MVVM architecture.

For an example project, please visit my other project [Stein](https://github.com/nkristek/Stein).

## Prerequisites

The nuget package and [DLL](https://github.com/nkristek/Smaragd/releases) are built using .NET 4.7, but you can compile the library yourself to fit your needs.

## Installation

The recommended way to use this library is via [Nuget](https://www.nuget.org/packages/NKristek.Smaragd/), but you also can either download the DLL from the latest [release](https://github.com/nkristek/Smaragd/releases/latest) or compile it yourself.

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

`ThirdProperty` uses the `PropertySource` attribute with the names of both `FirstProperty` and `SecondProperty`. The `ViewModel` will raise a `PropertyChanged` event for `ThirdProperty` when a `PropertyChanged` event is raised for either of these properties. You can also define a single property name and one or multiple `NotifyCollectionChangedAction`. This will raise a `PropertyChanged` event for the property when the `CollectionChanged` event of the named collection occurs with one of the given `NotifyCollectionChangedAction`.

### CommandCanExecuteSource

The `DoStuffCommand` uses the `CommandCanExecuteSource` attribute, which indicates, that the `CanExecute()` method depends on the value of the properties named. When a `PropertyChanged` event for `ThirdProperty` is invoked, a `CanExecuteChanged` event is raised on the command (when using custom command implementations, also implement the `IRaiseCanExecuteChanged` interface for this functionality to work).

### IsDirty

`ViewModel` implements an `IsDirty` property which is initially false and set to true, if `SetProperty` changes a value, it will set `IsDirty` to true if no `IsDirtyIgnoredAttribute` is defined for this property. 

### Parent

The `Parent` property on `ViewModel` uses a `WeakReference` internally, a reference cycle is unlikely.

### IsReadOnly

The `IsReadOnly` property does what it implies, if set to true, `SetProperty` will now longer set any property or raise a `PropertyChanged` event except when for the `IsReadOnly` property itself.

### Nested ViewModels

If the property is a `ViewModel` you should call `RemoveChildViewModel()` on the old value and `AddChildViewModel()` on the new value if the property changed via SetProperty. This can be done like this:
```csharp
private ViewModel _child;
public ViewModel Child
{
    get => _child;
    set
    {
        if (SetProperty(ref _child, value, out var oldValue))
        {
            if (oldValue != null)
                RemoveChildViewModel(oldValue);
            if (value != null)
                AddChildViewModel(value);
        }
    }
}
```

`ViewModel` has a `Children` collection. To add viewmodel collections to this `ViewModelCollection`, you have to use `AddViewModelCollection()` on the collection. This `ViewModelCollection` is otherwise read only. The idea is, that there are one or multiple `ObservableCollection<TViewModel>` on the inheriting instance which get added to the `Children` collection by using the `CollectionChanged` event.

### ValidatingViewModel

Your custom viewmodel may also be of type `ValidatingViewModel` which implements `IDataErrorInfo` and `INotifyDataErrorInfo`. 

You may simply add Validations in the class constructor through either 
```csharp
AddValidation(() => MyProperty, new PredicateValidation<int>(value => value >= 5, "Value has to be at least 5"));
```
or
```csharp
AddValidation(nameof(MyProperty), new PredicateValidation<int>(value => value >= 5, "Value has to be at least 5"), MyProperty);
```

This will execute this validation everytime `SetProperty()` changes this property.
You can call `Validate()` to execute all validations again.

### TreeViewModel

This `ViewModel` provides an `IsChecked` implementation to use in a TreeView. It will update its parent `TreeViewModel` and children `TreeViewModel` with appropriate states for `IsChecked`.

**Please note:** The indeterminate state of `IsChecked` should only be set by updates from child viewModels. The `IsChecked` property will be set to `false` if trying to set it to `null`. If you want to set the `IsChecked` property to `null`, you have to use `SetIsChecked()`.

### DialogModel

There is also a `DialogModel` class, which inherits from `ValidatingViewModel` and implements a `Title` property to use in your dialog.

### Command

`ViewModelCommand` and `AsyncViewModelCommand` provide base implementations for `ICommand` and `IRaiseCanExecuteChanged`.

## Overview

This library provides the following classes:

### ViewModels namespace

Attributes:
- `PropertySource` (usable through `ComputedBindableBase`)
- `CommandCanExecuteSource` (usable through `ComputedBindableBase`)
- `IsDirtyIgnored` (usable through `ViewModel`)

`INotifyPropertyChanged`:
- `BindableBase`
- `ComputedBindableBase`
- `ViewModel`
- `ValidatingViewModel`
- `DialogModel`
- `TreeViewModel`

### Validation

Interfaces:
- `IValidation`

`IValidation`:
- `Validation<T>`
- `PredicateValidation<T>`

### Commands namespace

Interfaces:
- `IRaiseCanExecuteChanged`
- `IAsyncCommand`

`ICommand`:
- without `INotifyPropertyChanged`:
 - `Command`
 - `RelayCommand`
- with `INotifyPropertyChanged`:
 - `BindableCommand`
 - `ViewModelCommand`

`IAsyncCommand`:
- without `INotifyPropertyChanged`:
 - `AsyncCommand`
 - `AsyncRelayCommand`
- with `INotifyPropertyChanged`:
 - `AsyncBindableCommand`
 - `AsyncViewModelCommand`

## Contribution

If you find a bug feel free to open an issue. Contributions are also appreciated.
