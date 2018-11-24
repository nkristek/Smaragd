using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Tests.ViewModels
{
    public class TreeViewModelTests
    {
        private class FolderViewModel
            : TreeViewModel
        {
            public ObservableCollection<FolderViewModel> Subfolders { get; } = new ObservableCollection<FolderViewModel>();

            protected override IEnumerable<TreeViewModel> TreeChildren => Subfolders;
        }

        [Fact]
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
            Assert.True(parent.IsChecked);
            Assert.True(firstChild.IsChecked);
            Assert.True(secondChild.IsChecked);

            firstChild.IsChecked = false;
            Assert.Null(parent.IsChecked);
            Assert.False(firstChild.IsChecked);
            Assert.True(secondChild.IsChecked);

            firstChild.IsChecked = true;
            Assert.True(parent.IsChecked);
            Assert.True(firstChild.IsChecked);
            Assert.True(secondChild.IsChecked);

            parent.IsChecked = null;
            Assert.False(parent.IsChecked);
            Assert.False(firstChild.IsChecked);
            Assert.False(secondChild.IsChecked);
        }

        [Fact]
        public void TestIsExpanded()
        {
            var treeViewModel = new FolderViewModel();
            Assert.False(treeViewModel.IsExpanded, "IsExpanded should be initially false.");

            treeViewModel.IsExpanded = true;
            Assert.True(treeViewModel.IsExpanded, "IsExpanded wasn't set to true.");
        }
    }
}
