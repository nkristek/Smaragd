using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nkristek.MVVMBase.ViewModels;

namespace nkristek.MVVMBase.Tests.ViewModels
{
    /// <summary>
    /// Summary description for ValidatingViewModelTests
    /// </summary>
    [TestClass]
    public class TreeViewModelTests
    {
        private class FolderViewModel
            : TreeViewModel
        {
            public FolderViewModel()
            {
                Subfolders.CollectionChanged += (sender, args) =>
                {
                    if (args.OldItems != null)
                        foreach (var oldItem in args.OldItems.Cast<TreeViewModel>())
                            OwnedElements.Remove(oldItem);

                    if (args.NewItems != null)
                        foreach (var newItem in args.NewItems.Cast<TreeViewModel>())
                            OwnedElements.Add(newItem);
                };
            }

            public ObservableCollection<FolderViewModel> Subfolders { get; } = new ObservableCollection<FolderViewModel>();
        }

        [TestMethod]
        public void TestIsChecked()
        {
            var parent = new FolderViewModel();
            var firstChild = new FolderViewModel
            {
                Parent = parent
            };
            parent.Subfolders.Add(firstChild);
            var secondChild = new FolderViewModel
            {
                Parent = parent
            };
            parent.Subfolders.Add(secondChild);

            parent.IsChecked = true;
            Assert.AreEqual(true, parent.IsChecked, "Parent is not checked");
            Assert.AreEqual(true, firstChild.IsChecked, "First child is not checked");
            Assert.AreEqual(true, secondChild.IsChecked, "Second child is not checked");

            firstChild.IsChecked = false;
            Assert.AreEqual(null, parent.IsChecked, "Parent IsChecked is not null");
            Assert.AreEqual(false, firstChild.IsChecked, "First child is not checked");
            Assert.AreEqual(true, secondChild.IsChecked, "Second child is not checked");

            firstChild.IsChecked = true;
            Assert.AreEqual(true, parent.IsChecked, "Parent is not checked");
            Assert.AreEqual(true, firstChild.IsChecked, "First child is not checked");
            Assert.AreEqual(true, secondChild.IsChecked, "Second child is not checked");

            parent.IsChecked = null;
            Assert.AreEqual(false, parent.IsChecked, "Parent is checked");
            Assert.AreEqual(false, firstChild.IsChecked, "First child is checked");
            Assert.AreEqual(false, secondChild.IsChecked, "Second child is checked");
        }
    }
}
