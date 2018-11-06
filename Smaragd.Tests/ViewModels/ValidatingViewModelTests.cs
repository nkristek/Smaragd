using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NKristek.Smaragd.Validation;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Tests.ViewModels
{
    [TestClass]
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

        [TestMethod]
        public void TestSubscript()
        {
            var validatingModel = new TestValidatingModel();
            Assert.IsFalse(String.IsNullOrEmpty(validatingModel[null]), "Subscript should not fail and have an error");
            Assert.IsFalse(String.IsNullOrEmpty(validatingModel[nameof(TestValidatingModel.TestProperty)]), "Subscript of TestProperty should not fail and have an error");

            validatingModel.TestProperty = 5;
            Assert.IsTrue(String.IsNullOrEmpty(validatingModel[null]), "Subscript should have no error");
            Assert.IsTrue(String.IsNullOrEmpty(validatingModel[nameof(TestValidatingModel.TestProperty)]), "Subscript of TestProperty should not have an error");
        }

        [TestMethod]
        public void TestError()
        {
            var validatingModel = new TestValidatingModel();
            Assert.IsFalse(String.IsNullOrEmpty(validatingModel.Error), "Error should exist");
            
            validatingModel.TestProperty = 5;
            Assert.IsTrue(String.IsNullOrEmpty(validatingModel.Error), "Error should not exist");
        }

        [TestMethod]
        public void TestHasErrors()
        {
            var validatingModel = new TestValidatingModel();
            Assert.IsTrue(validatingModel.HasErrors, "ViewModel should have errors after initialization");
            
            validatingModel.TestProperty = 5;
            Assert.IsFalse(validatingModel.HasErrors, "ViewModel should now have no errors since TestProperty is at least 5");
        }

        [TestMethod]
        public void TestGetErrors()
        {
            var validatingModel = new TestValidatingModel();
            Assert.IsNotNull(validatingModel.GetErrors(null), "GetErrors() should not fail and have an error");
            Assert.IsFalse(IsNullOrEmpty(validatingModel.GetErrors(null)), "GetErrors() should not fail and have an error");
            Assert.IsNotNull(validatingModel.GetErrors(nameof(TestValidatingModel.TestProperty)), "GetErrors() of TestProperty should not fail and have an error");
            Assert.IsFalse(IsNullOrEmpty(validatingModel.GetErrors(nameof(TestValidatingModel.TestProperty))), "GetErrors() of TestProperty should not fail and have an error");

            validatingModel.TestProperty = 5;
            Assert.IsNotNull(validatingModel.GetErrors(null), "GetErrors() should have no error");
            Assert.IsTrue(IsNullOrEmpty(validatingModel.GetErrors(null)), "GetErrors() should have no error");
            Assert.IsNotNull(validatingModel.GetErrors(nameof(TestValidatingModel.TestProperty)), "GetErrors() of TestProperty should not have an error");
            Assert.IsTrue(IsNullOrEmpty(validatingModel.GetErrors(nameof(TestValidatingModel.TestProperty))), "GetErrors() of TestProperty should not have an error");
        }

        private static bool IsNullOrEmpty(IEnumerable enumerable)
        {
            if (enumerable == null)
                return true;

            foreach (var obj in enumerable)
                return false;

            return true;
        }

        [TestMethod]
        public void TestIsValid()
        {
            var validatingModel = new TestValidatingModel();
            Assert.IsFalse(validatingModel.IsValid, "ViewModel should not be valid after initialization");

            validatingModel.TestProperty = 5;
            Assert.IsTrue(validatingModel.IsValid, "ViewModel should be valid when the property has a valid value");

            validatingModel.TestProperty = 4;
            Assert.IsFalse(validatingModel.IsValid, "ViewModel should not be valid when the property has not a valid value");
        }

        [TestMethod]
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
            Assert.AreEqual(4, invokedPropertyChangedEvents.Count, "Invalid count of invocations of the PropertyChanged event. Invocations: " + String.Join(", ", invokedPropertyChangedEvents));
            Assert.IsTrue(invokedPropertyChangedEvents.Contains("HasErrors"), "The PropertyChanged event wasn't raised for the HasErrors property");
            Assert.IsTrue(invokedPropertyChangedEvents.Contains("IsValid"), "The PropertyChanged event wasn't raised for the IsValid property");
        }
        
        [TestMethod]
        public void TestValidate()
        {
            var validatingModel = new TestValidatingModel();
            Assert.IsFalse(validatingModel.IsValid, "ViewModel should not be valid after initialization");

            validatingModel.Validate();
            Assert.IsFalse(validatingModel.IsValid, "ViewModel should not be valid after calling Validate()");

            validatingModel.TestProperty = 5;
            validatingModel.Validate();
            Assert.IsTrue(validatingModel.IsValid, "ViewModel should be valid when the property has a valid value");
        }

        [TestMethod]
        public void TestAddValidation()
        {
            var test = new TestAddRemoveValidationsViewModel();
            Assert.IsFalse(test.Validations().Any(), "Validations are initially not empty.");

            test.AddValidations(new List<Validation<int>> { new PredicateValidation<int>(value => value >= 5, "Value has to be at least 5.") });
            Assert.IsTrue(test.Validations().Any(), "Validation was not added properly.");
        }

        [TestMethod]
        public void TestRemoveValidation()
        {
            var test = new TestAddRemoveValidationsViewModel();
            var validation = new PredicateValidation<int>(value => value >= 5, "Value has to be at least 5.");

            test.AddValidations(new List<Validation<int>> { validation });
            Assert.IsTrue(test.Validations().Any(), "Validation was not added properly.");

            test.RemoveValidations(new List<Validation<int>> { validation });
            Assert.IsFalse(test.Validations().Any(), "Validation was not removed properly.");
        }

        [TestMethod]
        public void TestRemoveValidations()
        {
            var test = new TestAddRemoveValidationsViewModel();

            test.AddValidations(new List<Validation<int>> { new PredicateValidation<int>(value => value >= 5, "Value has to be at least 5.") });
            Assert.IsTrue(test.Validations().Any(), "Validation was not added properly.");

            test.RemoveValidations();
            Assert.IsFalse(test.Validations().Any(), "Validation was not removed properly.");
        }

        [TestMethod]
        public void TestValidations()
        {
            var test = new TestAddRemoveValidationsViewModel();
            var validation = new PredicateValidation<int>(value => value >= 5, "Value has to be at least 5.");

            test.AddValidations(new List<Validation<int>> { validation });
            Assert.IsTrue(test.Validations().Any(kvp => kvp.Value.Contains(validation)), "Validation was not added properly.");
            Assert.IsTrue(test.Validations(() => test.TestProperty).Contains(validation), "Validation was not added properly.");

            test.RemoveValidations(new List<Validation<int>> { validation });
            Assert.IsFalse(test.Validations().Any(kvp => kvp.Value.Contains(validation)), "Validation was not removed properly.");
            Assert.IsFalse(test.Validations(() => test.TestProperty).Contains(validation), "Validation was not removed properly.");
        }

        [TestMethod]
        public void TestSuspendValidation()
        {
            var validatingModel = new TestValidatingModel();
            Assert.IsFalse(validatingModel.IsValid, "ViewModel should not be valid after initialization");

            using (validatingModel.SuspendValidation())
            {
                validatingModel.TestProperty = 5;
                Assert.IsFalse(validatingModel.IsValid, "ViewModel should be valid when validation is suspended");
            }

            Assert.IsTrue(validatingModel.IsValid, "ViewModel should be valid when the validations are not suspended anymore");
        }
    }
}
