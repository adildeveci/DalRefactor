using System;
using System.IO;
using System.Linq;

namespace DalRefactor
{
    public static class FileProcess
    {
        private static readonly string[] _ignoredFiles = { "bin", "obj", "Web References" };
        public static void StartScan(string rootPath)
        {
            // Get all subdirectories  
            string[] subdirectoryEntries = Directory.GetDirectories(rootPath);
            // Loop through them to see if they have any other subdirectories  
            foreach (string subdirectory in subdirectoryEntries)
            {
                if (!IsIgnoredPath(subdirectory))
                    LoadSubDirs(subdirectory);
            }

            GetFilesInDirectory(rootPath);
        } 
        private static void LoadSubDirs(string dir)
        {
            Console.WriteLine(dir);
            string[] subdirectoryEntries = Directory.GetDirectories(dir);
            foreach (string subdirectory in subdirectoryEntries)
            {
                LoadSubDirs(subdirectory);
            }
            GetFilesInDirectory(dir);
        }

        private static void GetFilesInDirectory(string dir)
        {
            //  string[] fileEntries = Directory.GetFiles(dir, "*.cs");
            var fileEntries = Directory
                .EnumerateFiles(dir, "*.cs", SearchOption.AllDirectories)
                .Select(Path.GetFileName); // <-- note you can shorten the lambda
            if (fileEntries.Any())
                Console.WriteLine(dir);

            foreach (string fileName in fileEntries)
            {
                Console.WriteLine(fileName);
                CodeEditor.EditFile(dir, fileName);
            }
        }


        private static bool IsIgnoredPath(string dir)
        {
            foreach (var ignoredFile in _ignoredFiles)
            {
                if (dir.EndsWith(ignoredFile))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
