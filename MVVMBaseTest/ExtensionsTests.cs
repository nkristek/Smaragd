using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nkristek.MVVMBase.Extensions;

namespace nkristek.MVVMBaseTest
{
    [TestClass]
    public class ExtensionsTests
    {
        [TestMethod]
        public void TestIEnumerableExtensions()
        {
            var testInput = new[] { new object(), new object(), new object() };
            var result = 0;
            testInput.ForEach(i => result++);
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void TestStringExtensions()
        {
            var invalidPath = String.Concat(Path.GetInvalidPathChars());
            Assert.IsTrue(invalidPath.ContainsInvalidPathChars());
            Assert.IsFalse("test".ContainsInvalidPathChars());

            var validPath = invalidPath.ReplaceInvalidPathChars('a');
            Assert.IsFalse(validPath.ContainsInvalidPathChars());

            var invalidFileName = String.Concat(Path.GetInvalidFileNameChars());
            Assert.IsTrue(invalidFileName.ContainsInvalidFileNameChars());
            Assert.IsFalse("test".ContainsInvalidFileNameChars());

            var validFileName = invalidFileName.ReplaceInvalidFileNameChars('a');
            Assert.IsFalse(validFileName.ContainsInvalidFileNameChars());
        }
    }
}
