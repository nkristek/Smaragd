using NKristek.Smaragd.ViewModels;
using Xunit;

namespace NKristek.Smaragd.Tests.ViewModels
{
    public class DialogModelTests
    {
        private class TestDialogModel
            : DialogModel
        {
        }

        [Theory]
        [InlineData(null, "1")]
        [InlineData("1", "2")]
        public void Title_set(string initialValue, string valueToSet)
        {
            var viewModel = new TestDialogModel
            {
                Title = initialValue
            };
            Assert.Equal(initialValue, viewModel.Title);
            viewModel.Title = valueToSet;
            Assert.Equal(valueToSet, viewModel.Title);
        }
    }
}