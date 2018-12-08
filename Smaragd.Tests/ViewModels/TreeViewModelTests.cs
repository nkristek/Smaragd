using System.Collections.Generic;
using System.Collections.ObjectModel;
using NKristek.Smaragd.ViewModels;
using Xunit;

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

        private class FileViewModel
            : TreeViewModel
        {
            public IEnumerable<TreeViewModel> TreeChildrenExternal => TreeChildren;
        }

        [Fact]
        public void IsChecked_DefaultValue()
        {
            var treeViewModel = new FolderViewModel();
            Assert.False(treeViewModel.IsChecked);
        }

        [Fact]
        public void IsChecked_True()
        {
            var treeViewModel = new FolderViewModel
            {
                IsChecked = true
            };
            Assert.True(treeViewModel.IsChecked);
        }

        [Fact]
        public void IsChecked_False()
        {
            var treeViewModel = new FolderViewModel
            {
                IsChecked = false
            };
            Assert.False(treeViewModel.IsChecked);
        }

        [Fact]
        public void IsChecked_Null()
        {
            var treeViewModel = new FolderViewModel
            {
                IsChecked = null
            };
            Assert.False(treeViewModel.IsChecked);
        }

        [Fact]
        public void IsChecked_ParentSetsChildren()
        {
            var parent = new FolderViewModel();
            var child = new FolderViewModel
            {
                Parent = parent
            };
            parent.Subfolders.Add(child);

            parent.IsChecked = true;
            Assert.True(child.IsChecked);
        }

        [Theory]
        [InlineData(false, false, false, false)]
        [InlineData(false, false, true, null)]
        [InlineData(false, true, false, null)]
        [InlineData(false, true, true, true)]
        [InlineData(true, false, false, false)]
        [InlineData(true, false, true, null)]
        [InlineData(true, true, false, null)]
        [InlineData(true, true, true, true)]
        public void IsChecked_ChildrenSetParent(bool? parentInitialValue, bool? firstChildValue, bool? secondChildValue, bool? expectedParentValue)
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

            parent.IsChecked = parentInitialValue;
            firstChild.IsChecked = firstChildValue;
            secondChild.IsChecked = secondChildValue;
            Assert.Equal(expectedParentValue, parent.IsChecked);
        }

        [Fact]
        public void IsExpanded_DefaultValue()
        {
            var treeViewModel = new FolderViewModel();
            Assert.False(treeViewModel.IsExpanded);
        }

        [Fact]
        public void SetIsExpanded()
        {
            var treeViewModel = new FolderViewModel
            {
                IsExpanded = true
            };
            Assert.True(treeViewModel.IsExpanded);
        }

        [Fact]
        public void TreeChildren_DefaultValue()
        {
            var fileViewModel = new FileViewModel();
            Assert.Null(fileViewModel.TreeChildrenExternal);
        }

        [Fact]
        public void ReevaluateIsChecked_TreeChildrenNotOverridden()
        {
            var parent = new FileViewModel();
            var child = new FileViewModel
            {
                Parent = parent,
                IsChecked = true
            };
            Assert.False(parent.IsChecked);
        }
    }
}
