/*
 * (C) Copyright 20168 Kge
*/
using System;
using System.IO;
namespace Kge
{
    namespace Agent
    {
        namespace Library
        {
            public static class Os
            {
                /// <summary>
                /// Creates a directory on OS disk if doesn't exist
                /// </summary>
                /// <param name="rootdir">Root dir name</param>
                /// <param name="subdir">Subdirectrory name</param>
                /// <returns>Path of directory</returns>
                public static string CreateDirectoryOnOSDisk(string rootdir, string subdir)
                {
                    string root = Path.GetPathRoot(Environment.SystemDirectory);
                    string path = root + rootdir + "\\" + subdir;
                    if (!System.IO.Directory.Exists(path))
                    {
                        System.IO.Directory.CreateDirectory(path);
                    }
                    return path;
                }

            }
        }


    }
}
