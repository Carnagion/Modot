using System.Collections.Generic;
using System.Linq;

namespace Godot.Utility.Extensions
{
    public static class DirectoryExtensions
    {
        public static IEnumerable<string> GetFiles(this Directory directory, bool recursive = false)
        {
            return recursive
                ? directory.GetDirectories(true)
                    .SelectMany(path =>
                    {
                        Directory recursiveDirectory = new();
                        recursiveDirectory.Open(path);
                        return recursiveDirectory.GetElementsNonRecursive(true);
                    })
                    .Concat(directory.GetElementsNonRecursive(true))
                : directory.GetElementsNonRecursive(true);
        }

        public static IEnumerable<string> GetFiles(this Directory directory, string directoryPath, bool recursive = false)
        {
            directory.Open(directoryPath);
            return directory.GetFiles(recursive);
        }

        public static IEnumerable<string> GetDirectories(this Directory directory, bool recursive = false)
        {
            return recursive
                ? directory.GetElementsNonRecursive(false)
                    .SelectMany(path =>
                    {
                        Directory recursiveDirectory = new();
                        recursiveDirectory.Open(path);
                        return recursiveDirectory.GetDirectories(true).Prepend(path);
                    })
                : directory.GetElementsNonRecursive(false);
        }

        public static IEnumerable<string> GetDirectories(this Directory directory, string directoryPath, bool recursive = false)
        {
            directory.Open(directoryPath);
            return directory.GetDirectories(recursive);
        }

        private static IEnumerable<string> GetElementsNonRecursive(this Directory directory, bool trueIfFiles)
        {
            directory.ListDirBegin(true);
            while (true)
            {
                string next = directory.GetNext();
                if (next is "")
                {
                    yield break;
                }
                if (directory.CurrentIsDir() == trueIfFiles)
                {
                    continue;
                }
                string current = directory.GetCurrentDir();
                yield return current.EndsWith('/') ? $"{current}{next}" : $"{current}/{next}";
            }
        }
    }
}