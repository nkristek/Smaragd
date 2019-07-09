# Smaragd

[![Build Status](https://dev.azure.com/nkristek/Smaragd/_apis/build/status/nkristek.Smaragd?branchName=master)](https://dev.azure.com/nkristek/Smaragd/_build/latest?definitionId=2&branchName=master)
[![Code coverage](https://img.shields.io/azure-devops/coverage/nkristek/Smaragd/2.svg)](https://dev.azure.com/nkristek/Smaragd/_build/latest?definitionId=2&branchName=master)
[![NuGet version](https://img.shields.io/nuget/v/NKristek.Smaragd.svg)](https://www.nuget.org/packages/NKristek.Smaragd/)
[![GitHub license](https://img.shields.io/github/license/nkristek/Smaragd.svg)](https://github.com/nkristek/Smaragd/blob/master/LICENSE)

This is a very light weight library containing base classes for implementing a C# .NET application using the MVVM architecture.
It is fully unit tested and platform independent.

For an example project, please visit my other project [Stein](https://github.com/nkristek/Stein), where it is used in a WPF environment.

## Features

This library contains base implementations of:
- `INotifyPropertyChanging` and `INotifyPropertyChanged`
- `INotifyDataErrorInfo`
- `ICommand` and asynchronous `IAsyncCommand`

Additionally, there are other base classes that provide additional features, for example `TreeViewModel` implements an `IsChecked` property that automatically updates its parent and children.

For more information, please visit the [documentation](https://github.com/nkristek/Smaragd/wiki).

## Installation

The recommended way to use this library is via [Nuget](https://www.nuget.org/packages/NKristek.Smaragd/).

Currently supported frameworks:
- .NET Standard 2.0 or higher
- .NET Framework 4.5 or higher

## Contribution

If you find a bug feel free to open an issue. Contributions are also appreciated.
