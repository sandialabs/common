using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace gov.sandia.sld.common.utilities
{
    /// <summary>
    /// Gets all of the files in a specified folder, and recursively all of
    /// that folder's folders.
    /// 
    /// Simply construct an instance with the root folder and the filename
    /// mask, and all of the files will appear in the files element.
    /// </summary>
    public class DirectoryTraverser
    {
        public string rootFolder { get; set; }
        public string filenameMask { get; set; }
        public List<FileInfo> files { get; private set; }
        public List<string> filenames { get { return files.ConvertAll<string>(f => f.FullName); } }

        public DirectoryTraverser(string root_folder, string filename_mask = "*.*")
        {
            rootFolder = root_folder;
            filenameMask = filename_mask;

            files = new List<FileInfo>();

            DirectoryInfo root = new DirectoryInfo(root_folder);
            Traverse(root);
        }

        private void Traverse(DirectoryInfo dir)
        {
            if (string.Compare(dir.Name, "__MACOSX", true) == 0)
                return;

            try
            {
                files.AddRange(dir.GetFiles(filenameMask));
            }
            catch (Exception)
            {
            }

            try
            {
                dir.GetDirectories().ToList().ForEach(d => Traverse(d));
            }
            catch (Exception)
            {
            }
        }
    }
}
