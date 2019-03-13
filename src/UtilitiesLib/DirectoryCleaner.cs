using System;
using System.IO;

namespace gov.sandia.sld.common.utilities
{
    /// <summary>
    /// Used to clean files out of a folder. Construct with a path to the
    /// folder and a filename mask. Then call Clean with a CleanRule
    /// callback, and if the CleanRule callback returns true the file is deleted.
    /// </summary>
    public class DirectoryCleaner
    {
        public delegate bool CleanRule(FileInfo fi);

        public string rootFolder { get; set; }
        public string filenameMask { get; set; }

        public DirectoryCleaner(string root_folder, string filename_mask = "*.*")
        {
            rootFolder = root_folder;
            filenameMask = filename_mask;
        }

        public void Clean(CleanRule rule)
        {
            DirectoryTraverser dir = new DirectoryTraverser(rootFolder, filenameMask);

            foreach (FileInfo file in dir.files)
            {
                try
                {
                    if (rule(file))
                        file.Delete();
                }
                catch (Exception)
                {
                }
            }
        }
    }
}