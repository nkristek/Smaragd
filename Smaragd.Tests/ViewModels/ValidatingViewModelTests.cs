using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using NKristek.Smaragd.Validation;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Tests.ViewModels
{
    public class ValidatingViewModelTests
    {
        private class TestValidatingModel
            : ValidatingViewModel
        {
            public TestValidatingModel()
            {
                AddValidation(() => TestProperty, new PredicateValidation<int>(value => value >= 5, "Value has to be at least 5."));
                AddValidation(() => TestProperty, new PredicateValidation<int>(value => value >= 5, "Value really has to be at least 5."));
            }

            private int _testProperty;

            public int TestProperty
            {
                get => _testProperty;
                set => SetProperty(ref _testProperty, value, out _);
            }
        }

        private class TestAddRemoveValidationsViewModel
            : ValidatingViewModel
        {
            private int _testProperty;

            public int TestProperty
            {
                get => _testProperty;
                set => SetProperty(ref _testProperty, value, out _);
            }

            public void AddValidations(IEnumerable<Validation<int>> validations)
            {
                foreach (var validation in validations)
                    AddValidation(() => TestProperty, validation);
            }

            public void RemoveValidations(IEnumerable<Validation<int>> validations)
            {
                foreach (var validation in validations)
                    RemoveValidation(() => TestProperty, validation);
            }

            public void RemoveValidations()
            {
                RemoveValidations(() => TestProperty);
            }
        }

        [Fact]
        public void TestSubscript()
        {
            var validatingModel = new TestValidatingModel();
            Assert.False(String.IsNullOrEmpty(validatingModel[null]), "Subscript should not fail and have an error");
            Assert.False(String.IsNullOrEmpty(validatingModel[nameof(TestValidatingModel.TestProperty)]), "Subscript of TestProperty should not fail and have an error");

            validatingModel.TestProperty = 5;
            Assert.True(String.IsNullOrEmpty(validatingModel[null]), "Subscript should have no error");
            Assert.True(String.IsNullOrEmpty(validatingModel[nameof(TestValidatingModel.TestProperty)]), "Subscript of TestProperty should not have an error");
        }

        [Fact]
        public void TestError()
        {
            var validatingModel = new TestValidatingModel();
            Assert.False(String.IsNullOrEmpty(validatingModel.Error), "Error should exist");
            
            validatingModel.TestProperty = 5;
            Assert.True(String.IsNullOrEmpty(validatingModel.Error), "Error should not exist");
        }

        [Fact]
        public void TestHasErrors()
        {
            var validatingModel = new TestValidatingModel();
            Assert.True(validatingModel.HasErrors, "ViewModel should have errors after initialization");
            
            validatingModel.TestProperty = 5;
            Assert.False(validatingModel.HasErrors, "ViewModel should now have no errors since TestProperty is at least 5");
        }

        [Fact]
        public void TestGetErrors()
        {
            var validatingModel = new TestValidatingModel();
            Assert.NotNull(validatingModel.GetErrors(null));
            Assert.False(IsNullOrEmpty(validatingModel.GetErrors(null)), "GetErrors() should not fail and have an error");
            Assert.NotNull(validatingModel.GetErrors(nameof(TestValidatingModel.TestProperty)));
            Assert.False(IsNullOrEmpty(validatingModel.GetErrors(nameof(TestValidatingModel.TestProperty))), "GetErrors() of TestProperty should not fail and have an error");

            validatingModel.TestProperty = 5;
            Assert.NotNull(validatingModel.GetErrors(null));
            Assert.True(IsNullOrEmpty(validatingModel.GetErrors(null)), "GetErrors() should have no error");
            Assert.NotNull(validatingModel.GetErrors(nameof(TestValidatingModel.TestProperty)));
            Assert.True(IsNullOrEmpty(validatingModel.GetErrors(nameof(TestValidatingModel.TestProperty))), "GetErrors() of TestProperty should not have an error");
        }

        private static bool IsNullOrEmpty(IEnumerable enumerable)
        {
            if (enumerable == null)
                return true;

            foreach (var obj in enumerable)
                return false;

            return true;
        }

        [Fact]
        public void TestIsValid()
        {
            var validatingModel = new TestValidatingModel();
            Assert.False(validatingModel.IsValid, "ViewModel should not be valid after initialization");

            validatingModel.TestProperty = 5;
            Assert.True(validatingModel.IsValid, "ViewModel should be valid when the property has a valid value");

            validatingModel.TestProperty = 4;
            Assert.False(validatingModel.IsValid, "ViewModel should not be valid when the property has not a valid value");
        }

        [Fact]
        public void TestNotifyIsValid()
        {
            var invokedPropertyChangedEvents = new List<string>();

            var viewModel = new TestValidatingModel();
            viewModel.PropertyChanged += (sender, e) =>
            {
                invokedPropertyChangedEvents.Add(e.PropertyName);
            };

            viewModel.TestProperty = 5;

            // 4 PropertyChanged events should have happened, TestProperty, IsDirty (due to TestProperty), HasErrors and IsValid
            Assert.Equal(4, invokedPropertyChangedEvents.Count);
            Assert.True(invokedPropertyChangedEvents.Contains("HasErrors"), "The PropertyChanged event wasn't raised for the HasErrors property");
            Assert.True(invokedPropertyChangedEvents.Contains("IsValid"), "The PropertyChanged event wasn't raised for the IsValid property");
        }
        
        [Fact]
        public void TestValidate()
        {
            var validatingModel = new TestValidatingModel();
            Assert.False(validatingModel.IsValid, "ViewModel should not be valid after initialization");

            validatingModel.Validate();
            Assert.False(validatingModel.IsValid, "ViewModel should not be valid after calling Validate()");

            validatingModel.TestProperty = 5;
            validatingModel.Validate();
            Assert.True(validatingModel.IsValid, "ViewModel should be valid when the property has a valid value");
        }

        [Fact]
        public void TestAddValidation()
        {
            var test = new TestAddRemoveValidationsViewModel();
            Assert.False(test.Validations().Any(), "Validations are initially not empty.");

            test.AddValidations(new List<Validation<int>> { new PredicateValidation<int>(value => value >= 5, "Value has to be at least 5.") });
            Assert.True(test.Validations().Any(), "Validation was not added properly.");
        }

        [Fact]
        public void TestRemoveValidation()
        {
            var test = new TestAddRemoveValidationsViewModel();
            var validation = new PredicateValidation<int>(value => value >= 5, "Value has to be at least 5.");

            test.AddValidations(new List<Validation<int>> { validation });
            Assert.True(test.Validations().Any(), "Validation was not added properly.");

            test.RemoveValidations(new List<Validation<int>> { validation });
            Assert.False(test.Validations().Any(), "Validation was not removed properly.");
        }

        [Fact]
        public void TestRemoveValidations()
        {
            var test = new TestAddRemoveValidationsViewModel();

            test.AddValidations(new List<Validation<int>> { new PredicateValidation<int>(value => value >= 5, "Value has to be at least 5.") });
            Assert.True(test.Validations().Any(), "Validation was not added properly.");

            test.RemoveValidations();
            Assert.False(test.Validations().Any(), "Validation was not removed properly.");
        }

        [Fact]
        public void TestValidations()
        {
            var test = new TestAddRemoveValidationsViewModel();
            var validation = new PredicateValidation<int>(value => value >= 5, "Value has to be at least 5.");

            test.AddValidations(new List<Validation<int>> { validation });
            Assert.True(test.Validations().Any(kvp => kvp.Value.Contains(validation)), "Validation was not added properly.");
            Assert.True(test.Validations(() => test.TestProperty).Contains(validation), "Validation was not added properly.");

            test.RemoveValidations(new List<Validation<int>> { validation });
            Assert.False(test.Validations().Any(kvp => kvp.Value.Contains(validation)), "Validation was not removed properly.");
            Assert.False(test.Validations(() => test.TestProperty).Contains(validation), "Validation was not removed properly.");
        }

        [Fact]
        public void TestSuspendValidation()
        {
            var validatingModel = new TestValidatingModel();
            Assert.False(validatingModel.IsValid, "ViewModel should not be valid after initialization");

            using (validatingModel.SuspendValidation())
            {
                validatingModel.TestProperty = 5;
                Assert.False(validatingModel.IsValid, "ViewModel should be valid when validation is suspended");
            }

            Assert.True(validatingModel.IsValid, "ViewModel should be valid when the validations are not suspended anymore");
        }
    }
}
