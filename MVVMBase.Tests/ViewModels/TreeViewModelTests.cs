using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                        foreach (var oldItem in args.OldItems)
                            OwnedElements.Remove((TreeViewModel)oldItem);

                    if (args.NewItems != null)
                        foreach (var newItem in args.NewItems)
                            OwnedElements.Add((TreeViewModel)newItem);
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

            parent.IsChecked = false;
            Assert.AreEqual(false, parent.IsChecked, "Parent is checked");
            Assert.AreEqual(false, firstChild.IsChecked, "First child is checked");
            Assert.AreEqual(false, secondChild.IsChecked, "Second child is checked");
        }
    }
}
