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

        private string Title { get; }

        public DialogModelTests()
        {
            Title = "Test";
        }

        [Fact]
        public void TitleProperty()
        {
            var dialogModel = new TestDialogModel
            {
                Title = Title
            };
            Assert.Equal(Title, dialogModel.Title);
        }
    }
}