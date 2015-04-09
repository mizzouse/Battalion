using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Infantry.Helpers
{
    public static class DirectoryHelper
    {
        private static string _rootDirectory = "";

        /// <summary>
        /// Gets the root directory path set at startup
        /// </summary>
        public static string RootDirectory
        {
            get { return _rootDirectory; }
            set
            {
                if (_rootDirectory == "")
                    _rootDirectory = value;
            }
        }

        #region Create Directory
        /// <summary>
        /// Creates a directory folder
        /// </summary>
        /// <param name="dirName">Directory name</param>
        public static void CreateDirectory(string dirName)
        {
            if (!Directory.Exists(Path.Combine(RootDirectory, dirName)))
                Directory.CreateDirectory(Path.Combine(RootDirectory, dirName));
        }
        #endregion

        #region Exists
        /// <summary>
        /// Does a file or directory exist?
        /// </summary>
        /// <param name="fileName">Name of file or directory</param>
        /// <param name="directory">Is this a directory name?</param>
        public static bool Exists(string fileName, bool directory)
        {
            string dir = Directory.GetCurrentDirectory();

            //Are we checking on an existing directory?
            if (directory)
            {
                if (Directory.Exists(Path.Combine(dir, fileName)))
                    return true;
                return (Find(fileName, dir) == null ? false : true);
            }
            else
            {
                //Just a filename
                if (File.Exists(Path.Combine(dir, fileName)))
                    return true;
                return (Find(fileName, dir) == null ? false : true);
            }
        }

        /// <summary>
        /// Finds a file or directory within our current directories.
        /// </summary>
        /// <param name="Name">Name of the file/directory</param>
        /// <param name="path">Current directory</param>
        /// <returns>Returns the path if found, null if not</returns>
        public static string Find(string Name, string path)
        {
            string file = Path.Combine(path, Name);

            //Does the file exist here?
            if (File.Exists(file) || Directory.Exists(file))
                return file;

            //Search inner directories
            foreach(string dir in Directory.GetDirectories(path))
            {
                file = Find(Name, dir);
                if (file != null)
                    return file;
            }

            return null;
        }
        #endregion
    }
}
