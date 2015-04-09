using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework;

//TODO: Add this later to save to xbox cards/HD
namespace Infantry.Helpers
{
    public static class FileHelper
    {
        /// <summary>
        /// Gets the text from a file
        /// </summary>
        /// <param name="fileName">File in question</param>
        /// <returns>Returns a string array if found, null if not</returns>
        static public string[] GetLines(string fileName)
        {
            string fullPath = fileName;
            string[] directories = Directory.GetDirectories(DirectoryHelper.RootDirectory);

            //Search current folders to find our file
            foreach (string path in directories)
            {
                if (File.Exists(Path.Combine(path, fileName)))
                {
                    fullPath = Path.Combine(path, fileName);
                    break;
                }
            }

            try
            {
                StreamReader reader = new StreamReader(new FileStream(fullPath, FileMode.Open, FileAccess.Read), System.Text.Encoding.UTF8);
                List<string> lines = new List<string>();
                do
                {
                    lines.Add(reader.ReadLine());
                } while (reader.Peek() > -1);
                reader.Close();

                return lines.ToArray();
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #region Create
        /// <summary>
        /// Creates or opens a game content file
        /// </summary>
        /// <param name="fileName">Content File name</param>
        /// <param name="create">Create?</param>
        /// <returns>Returns the opened file</returns>
        public static FileStream CreateContentFile(string fileName, bool create)
        {
            string fullPath = Path.Combine(DirectoryHelper.RootDirectory, fileName);
            return File.Open(fullPath, create ? FileMode.Create : FileMode.OpenOrCreate,
                FileAccess.Write, FileShare.ReadWrite);
        }
        #endregion

        #region Load
        /// <summary>
        /// Loads a game content file, null if not found
        /// </summary>
        /// <param name="fileName">Content File Name</param>
        /// <returns>Returns the opened file</returns>
        public static FileStream LoadContentFile(string fileName)
        {
            string fullPath;

            //First lets see if it exists either in our main or sub directories
            //It has, use this location
            if ((fullPath = DirectoryHelper.Find(fileName, DirectoryHelper.RootDirectory)) != null)
                return File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            //Nope, not found
            return null;
        }
        #endregion

        #region Save
        /// <summary>
        /// Saves a game content file
        /// </summary>
        /// <param name="fileName">Content File Name</param>
        /// <returns>Returns the opened save file</returns>
        public static FileStream SaveContentFile(string fileName)
        {
            string fullPath;

            //First lets see if its been moved
            //It has, use this location
            if ((fullPath = DirectoryHelper.Find(fileName, DirectoryHelper.RootDirectory)) != null)
                return File.Open(fullPath, FileMode.Create, FileAccess.Write);

            //Nope, create a new file
            fullPath = Path.Combine(DirectoryHelper.RootDirectory, fileName);
            return File.Open(fullPath, FileMode.Create, FileAccess.Write);
        }
        #endregion

        #region Find
        /// <summary>
        /// Finds a game content file
        /// </summary>
        /// <param name="fileName">Content File Name</param>
        /// <returns>Returns the path to the file, null if not found</returns>
        public static string FindContentFile(string fileName)
        {
            string fullPath = DirectoryHelper.Find(fileName, DirectoryHelper.RootDirectory);

            return (fullPath != null ? fullPath : null);
        }
        #endregion
    }
}
