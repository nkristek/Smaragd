using Xunit;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Tests.ViewModels
{
    public class DialogModelTests
    {
        private class TestDialogModel
            : DialogModel
        {

        }

        [Fact]
        public void TestTitle()
        {
            const string title = "Test";
            var dialogModel = new TestDialogModel
            {
                Title = title
            };
            Assert.Equal(title, dialogModel.Title);
        }
    }
}
