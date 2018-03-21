using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        [TestMethod]
        public void TestMergeSequence()
        {
            var firstSequence = new List<string> { "1", "3" };
            var secondSequence = new List<string> { "1", "2", "3" };
            var mergedSequence = firstSequence.MergeSequence(secondSequence);
            Assert.IsTrue(mergedSequence.SequenceEqual(new List<string> { "1", "2", "3" }));

            firstSequence = new List<string> { "1", "2", "4" };
            secondSequence = new List<string> { "2", "3", "4", "5" };
            mergedSequence = firstSequence.MergeSequence(secondSequence);
            Assert.IsTrue(mergedSequence.SequenceEqual(new List<string> { "1", "2", "3", "4", "5" }));

            firstSequence = new List<string> { "1", "2", "3" };
            secondSequence = new List<string> { "4", "3", "2", "1" };
            mergedSequence = firstSequence.MergeSequence(secondSequence);
            Assert.IsTrue(mergedSequence.SequenceEqual(new List<string> { "1", "2", "4", "3" }));
        }
    }
}
