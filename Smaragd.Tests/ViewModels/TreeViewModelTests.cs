using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Tests.ViewModels
{
    [TestClass]
    public class TreeViewModelTests
    {
        private class FolderViewModel
            : TreeViewModel
        {
            public ObservableCollection<FolderViewModel> Subfolders { get; } = new ObservableCollection<FolderViewModel>();

            protected override IEnumerable<TreeViewModel> TreeChildren => Subfolders;
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
            Assert.AreEqual(true, parent.IsChecked, "Parent is not checked.");
            Assert.AreEqual(true, firstChild.IsChecked, "First child is not checked.");
            Assert.AreEqual(true, secondChild.IsChecked, "Second child is not checked.");

            firstChild.IsChecked = false;
            Assert.AreEqual(null, parent.IsChecked, "Parent IsChecked is not null.");
            Assert.AreEqual(false, firstChild.IsChecked, "First child is not checked.");
            Assert.AreEqual(true, secondChild.IsChecked, "Second child is not checked.");

            firstChild.IsChecked = true;
            Assert.AreEqual(true, parent.IsChecked, "Parent is not checked.");
            Assert.AreEqual(true, firstChild.IsChecked, "First child is not checked.");
            Assert.AreEqual(true, secondChild.IsChecked, "Second child is not checked.");

            parent.IsChecked = null;
            Assert.AreEqual(false, parent.IsChecked, "Parent is checked, it should be false now (since setting IsChecked to null defaults to false).");
            Assert.AreEqual(false, firstChild.IsChecked, "First child is checked.");
            Assert.AreEqual(false, secondChild.IsChecked, "Second child is checked.");
        }

        [TestMethod]
        public void TestIsExpanded()
        {
            var treeViewModel = new FolderViewModel();
            Assert.IsFalse(treeViewModel.IsExpanded, "IsExpanded should be initially false.");

            treeViewModel.IsExpanded = true;
            Assert.IsTrue(treeViewModel.IsExpanded, "IsExpanded wasn't set to true.");
        }
    }
}
