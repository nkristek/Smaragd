using Microsoft.VisualStudio.TestTools.UnitTesting;
using nkristek.MVVMBase.ViewModels;

namespace nkristek.MVVMBase.Tests.ViewModels
{
    /// <summary>
    /// Summary description for DialogModelTests
    /// </summary>
    [TestClass]
    public class DialogModelTests
    {
        private class TestDialogModel
            : DialogModel
        {

        }

        [TestMethod]
        public void TestTitle()
        {
            const string title = "Test";
            var dialogModel = new TestDialogModel
            {
                Title = title
            };
            Assert.AreEqual(title, dialogModel.Title, "Title property wasn't set");
        }
    }
}
