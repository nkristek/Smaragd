using System;
using System.IO;
using System.Linq;

namespace nkristek.MVVMBase.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Tests if the given string contains characters which are invalid for paths
        /// </summary>
        /// <param name="path">A path</param>
        /// <returns>True if the given string contains characters which are invalid for paths</returns>
        public static bool ContainsInvalidPathChars(this string path)
        {
            return String.Concat(path.Split(Path.GetInvalidPathChars())).Count() != path.Count();
        }

        /// <summary>
        /// Replaces all characters in the given string which are invalid for paths
        /// </summary>
        /// <param name="path">A path</param>
        /// <param name="replacement">Replacement character</param>
        public static string ReplaceInvalidPathChars(this string path, char replacement)
        {
            foreach (var invalidChar in Path.GetInvalidPathChars())
                path = path.Replace(invalidChar, replacement);
            return path;
        }

        /// <summary>
        /// Tests if the given string contains characters which are invalid for file names
        /// </summary>
        /// <param name="fileName">A file name</param>
        /// <returns>True if the given string contains characters which are invalid for file names</returns>
        public static bool ContainsInvalidFileNameChars(this string fileName)
        {
            return String.Concat(fileName.Split(Path.GetInvalidFileNameChars())).Count() != fileName.Count();
        }

        /// <summary>
        /// Replaces all characters in the given string which are invalid for file names
        /// </summary>
        /// <param name="fileName">A file name</param>
        /// <param name="replacement">Replacement character</param>
        public static string ReplaceInvalidFileNameChars(this string fileName, char replacement)
        {
            foreach (var invalidChar in Path.GetInvalidFileNameChars())
                fileName = fileName.Replace(invalidChar, replacement);
            return fileName;
        }
    }
}
