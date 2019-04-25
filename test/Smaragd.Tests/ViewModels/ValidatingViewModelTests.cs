using System;
using System.Collections.Generic;
using System.Linq;
using NKristek.Smaragd.Validation;
using NKristek.Smaragd.ViewModels;
using Xunit;

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
        }

        [Theory]
        [InlineData(null, "Value has to be at least 5.")]
        [InlineData("", "Value has to be at least 5.")]
        [InlineData(nameof(TestValidatingModel.TestProperty), "Value has to be at least 5.")]
        [InlineData("NotExistingProperty", null)]
        public void Subscript_with_invalid_data(string input, string expectedResult)
        {
            var validatingModel = new TestValidatingModel();
            Assert.Equal(expectedResult, validatingModel[input]);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(nameof(TestValidatingModel.TestProperty))]
        [InlineData("NotExistingProperty")]
        public void Subscript_with_valid_data(string input)
        {
            var validatingModel = new TestValidatingModel
            {
                TestProperty = 5
            };
            Assert.Null(validatingModel[input]);
        }

        [Fact]
        public void Error_with_invalid_data()
        {
            var validatingModel = new TestValidatingModel();
            Assert.Equal("Value has to be at least 5.", validatingModel.Error);
        }

        [Fact]
        public void Error_with_valid_data()
        {
            var validatingModel = new TestValidatingModel
            {
                TestProperty = 5
            };
            Assert.Null(validatingModel.Error);
        }

        [Fact]
        public void HasErrors_with_invalid_data()
        {
            var validatingModel = new TestValidatingModel();
            Assert.True(validatingModel.HasErrors);
        }

        [Fact]
        public void HasErrors_with_valid_data()
        {
            var validatingModel = new TestValidatingModel
            {
                TestProperty = 5
            };
            Assert.False(validatingModel.HasErrors);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(nameof(TestValidatingModel.TestProperty))]
        public void GetErrors_with_invalid_data(string input)
        {
            var validatingModel = new TestValidatingModel();
            Assert.Contains("Value has to be at least 5.", validatingModel.GetErrors(input).OfType<string>());
        }

        [Fact]
        public void GetErrors_with_invalid_data_and_not_existing_propertyName()
        {
            var validatingModel = new TestValidatingModel();
            Assert.Empty(validatingModel.GetErrors("NotExistingProperty"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(nameof(TestValidatingModel.TestProperty))]
        public void GetErrors_with_valid_data(string input)
        {
            var validatingModel = new TestValidatingModel
            {
                TestProperty = 5
            };
            Assert.Empty(validatingModel.GetErrors(input));
        }

        [Fact]
        public void ErrorsChanged()
        {
            var errorsChangedInvoked = 0;
            var validatingViewModel = new TestValidatingModel();
            validatingViewModel.ErrorsChanged += (sender, v) => errorsChangedInvoked++;
            validatingViewModel.TestProperty = 5;
            validatingViewModel.TestProperty = 5;
            Assert.Equal(1, errorsChangedInvoked);
        }

        [Fact]
        public void IsValid_with_invalid_data()
        {
            var validatingModel = new TestValidatingModel();
            Assert.False(validatingModel.IsValid);
        }

        [Fact]
        public void IsValid_with_valid_data()
        {
            var validatingModel = new TestValidatingModel
            {
                TestProperty = 5
            };
            Assert.True(validatingModel.IsValid);
        }

        [Fact]
        public void IsValid_PropertyChanged()
        {
            var invokedPropertyChangedEvents = new List<string>();

            var viewModel = new TestValidatingModel();
            viewModel.PropertyChanged += (sender, e) => { invokedPropertyChangedEvents.Add(e.PropertyName); };
            viewModel.TestProperty = 5;

            var expectedPropertyChangedEvents = new List<string>
            {
                nameof(TestValidatingModel.TestProperty),
                nameof(TestValidatingModel.IsDirty),
                nameof(TestValidatingModel.HasErrors),
                nameof(TestValidatingModel.IsValid)
            };
            Assert.Equal(expectedPropertyChangedEvents.OrderBy(n => n), invokedPropertyChangedEvents.OrderBy(n => n));
        }

        [Fact]
        public void Validations_empty()
        {
            var validatingViewModel = new TestAddRemoveValidationsViewModel();
            var validations = validatingViewModel.Validations().ToList();
            Assert.Empty(validations);
            Assert.Empty(validations.SelectMany(v => v.Value));
        }

        [Fact]
        public void Validations_of_property_empty()
        {
            var validatingViewModel = new TestAddRemoveValidationsViewModel();
            var validations = validatingViewModel.Validations(() => validatingViewModel.TestProperty);
            Assert.Empty(validations);
        }

        [Fact]
        public void Validations_not_empty()
        {
            var validatingViewModel = new TestAddRemoveValidationsViewModel();
            var validation = new PredicateValidation<int>(value => value >= 5, "Value has to be at least 5.");
            validatingViewModel.AddValidation(() => validatingViewModel.TestProperty, validation);
            var viewModelValidations = validatingViewModel.Validations().ToList();
            Assert.NotEmpty(viewModelValidations);
            Assert.NotEmpty(viewModelValidations.SelectMany(v => v.Value));
        }

        [Fact]
        public void Validations_of_property_not_empty()
        {
            var validatingViewModel = new TestAddRemoveValidationsViewModel();
            var validation = new PredicateValidation<int>(value => value >= 5, "Value has to be at least 5.");
            validatingViewModel.AddValidation(() => validatingViewModel.TestProperty, validation);
            var validationsOfTestProperty = validatingViewModel.Validations(() => validatingViewModel.TestProperty);
            Assert.NotEmpty(validationsOfTestProperty);
        }

        [Fact]
        public void Validations_of_property_invalid_expression_throws_ArgumentException()
        {
            var validatingViewModel = new TestAddRemoveValidationsViewModel();
            Assert.Throws<ArgumentException>(() => validatingViewModel.Validations(() => new object()));
        }

        [Fact]
        public void AddValidation()
        {
            var validatingViewModel = new TestAddRemoveValidationsViewModel();
            var validation = new PredicateValidation<int>(value => value >= 5, "Value has to be at least 5.");
            validatingViewModel.AddValidation(() => validatingViewModel.TestProperty, validation);
            Assert.Equal(Enumerable.Repeat(validation, 1), validatingViewModel.Validations().SelectMany(v => v.Value));
        }

        [Fact]
        public void AddValidation_changing_IsValid()
        {
            var validatingViewModel = new TestAddRemoveValidationsViewModel();
            Assert.True(validatingViewModel.IsValid);
            var validation = new PredicateValidation<int>(value => value >= 5, "Value has to be at least 5.");
            validatingViewModel.AddValidation(() => validatingViewModel.TestProperty, validation);
            Assert.False(validatingViewModel.IsValid);
            Assert.Equal(Enumerable.Repeat(validation, 1), validatingViewModel.Validations().SelectMany(v => v.Value));
        }

        [Fact]
        public void RemoveValidation_remove_not_all_validations()
        {
            var validatingViewModel = new TestAddRemoveValidationsViewModel();
            var firstValidation = new PredicateValidation<int>(value => value >= 5, "Value has to be at least 5.");
            var secondValidation = new PredicateValidation<int>(value => value <= 10, "Value has to be at most 10.");
            validatingViewModel.AddValidation(() => validatingViewModel.TestProperty, firstValidation);
            validatingViewModel.AddValidation(() => validatingViewModel.TestProperty, secondValidation);
            validatingViewModel.RemoveValidation(() => validatingViewModel.TestProperty, firstValidation);
            Assert.DoesNotContain(firstValidation, validatingViewModel.Validations().SelectMany(v => v.Value));
        }

        [Fact]
        public void RemoveValidation_remove_all_validations()
        {
            var validatingViewModel = new TestAddRemoveValidationsViewModel();
            var firstValidation = new PredicateValidation<int>(value => value >= 5, "Value has to be at least 5.");
            var secondValidation = new PredicateValidation<int>(value => value <= 10, "Value has to be at most 10.");
            validatingViewModel.AddValidation(() => validatingViewModel.TestProperty, firstValidation);
            validatingViewModel.AddValidation(() => validatingViewModel.TestProperty, secondValidation);
            validatingViewModel.RemoveValidation(() => validatingViewModel.TestProperty, firstValidation);
            validatingViewModel.RemoveValidation(() => validatingViewModel.TestProperty, secondValidation);
            Assert.Empty(validatingViewModel.Validations().SelectMany(v => v.Value));
        }

        [Fact]
        public void RemoveValidation_validation_already_removed()
        {
            var validatingViewModel = new TestAddRemoveValidationsViewModel();
            var validation = new PredicateValidation<int>(value => value >= 5, "Value has to be at least 5.");
            validatingViewModel.AddValidation(() => validatingViewModel.TestProperty, validation);
            validatingViewModel.RemoveValidation(() => validatingViewModel.TestProperty, validation);
            validatingViewModel.RemoveValidation(() => validatingViewModel.TestProperty, validation);
            Assert.Empty(validatingViewModel.Validations().SelectMany(v => v.Value));
        }

        [Fact]
        public void RemoveValidations()
        {
            var validatingViewModel = new TestAddRemoveValidationsViewModel();
            var firstValidation = new PredicateValidation<int>(value => value >= 5, "Value has to be at least 5.");
            var secondValidation = new PredicateValidation<int>(value => value <= 10, "Value has to be at most 10.");
            validatingViewModel.AddValidation(() => validatingViewModel.TestProperty, firstValidation);
            validatingViewModel.AddValidation(() => validatingViewModel.TestProperty, secondValidation);
            validatingViewModel.RemoveValidations(() => validatingViewModel.TestProperty);
            Assert.Empty(validatingViewModel.Validations().SelectMany(v => v.Value));
        }

        [Fact]
        public void IsValidationSuspended_initially_false()
        {
            var viewModel = new TestValidatingModel();
            Assert.False(viewModel.IsValidationSuspended);
        }

        [Fact]
        public void IsValidationSuspended_set()
        {
            var viewModel = new TestValidatingModel
            {
                IsValidationSuspended = true
            };
            Assert.True(viewModel.IsValidationSuspended);
        }

        [Theory]
        [InlineData(false, 1)]
        [InlineData(true, 0)]
        public void IsValidationSuspended_suspends_validation(bool validationsSuspended, int expectedExecutedValidation)
        {
            var validationsExecuted = 0;
            var validatingModel = new TestAddRemoveValidationsViewModel
            {
                IsValidationSuspended = validationsSuspended
            };
            var validation = new PredicateValidation<int>(value =>
            {
                validationsExecuted++;
                return value >= 5;
            }, "Value has to be at least 5.");
            validatingModel.AddValidation(() => validatingModel.TestProperty, validation);
            Assert.Equal(expectedExecutedValidation, validationsExecuted);
        }

        [Fact]
        public void SuspendValidation_invalid_to_valid()
        {
            var validatingModel = new TestValidatingModel
            {
                IsValidationSuspended = true,
                TestProperty = 5
            };
            Assert.False(validatingModel.IsValid);

            validatingModel.IsValidationSuspended = false;
            Assert.True(validatingModel.IsValid);
        }

        [Fact]
        public void SuspendValidation_valid_to_invalid()
        {
            var validatingModel = new TestValidatingModel
            {
                TestProperty = 5,
                IsValidationSuspended = true
            };
            validatingModel.TestProperty = 4;
            Assert.True(validatingModel.IsValid);

            validatingModel.IsValidationSuspended = false;
            Assert.False(validatingModel.IsValid);
        }

        [Fact]
        public void ValidateAll_property_does_not_exist()
        {
            var validationsExecuted = 0;
            var validatingModel = new TestAddRemoveValidationsViewModel
            {
                IsValidationSuspended = true
            };
            var validation = new PredicateValidation<int>(value =>
            {
                validationsExecuted++;
                return value >= 5;
            }, "Value has to be at least 5.");
            var notExistingPropertyOfViewModel = 0;
            validatingModel.AddValidation(() => notExistingPropertyOfViewModel, validation);

            validatingModel.IsValidationSuspended = false;
            Assert.Equal(0, validationsExecuted);
        }
    }
}