#region

//文件创建者：Egg
//创建时间：10-25 11:17

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace EggFramework.Util
{
    public static class DirectoryUtil
    {
        public static void MakeSureDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static bool CopyDir(string srcPath, string aimPath, string ignoreExt)
        {
            try
            {
                if (aimPath[^1] != Path.DirectorySeparatorChar)
                {
                    aimPath += Path.DirectorySeparatorChar;
                }

                MakeSureDirectory(aimPath);
                var fileList = Directory.GetFileSystemEntries(srcPath);
                foreach (var file in fileList)
                {
                    if (Directory.Exists(file))
                    {
                        CopyDir(file, aimPath + Path.GetFileName(file), ignoreExt);
                    }
                    else if (!file.EndsWith(ignoreExt))
                    {
                        File.Copy(file, aimPath + Path.GetFileName(file), true);
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static IEnumerable<string> GetFilePaths()
        {
            var dirs = Directory.GetDirectories("Assets/", "*", SearchOption.AllDirectories);
            var filterList = dirs.Where(file =>
                !file.Contains(".git") &&
                !file.Contains("Plugins") &&
                !file.Contains("Scenes")
            ).Select(file => file.Replace('\\', '/')).ToHashSet();
            return filterList;
        }

        public static string ExtractName(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        public static IEnumerable<string> GetFileEndWith(string end)
        {
            var files = Directory.GetFiles("Assets/", "*", SearchOption.AllDirectories);
            return files.Where(file => file.EndsWith(end));
        }

        public static string ExtractFolder(string path)
        {
            return Path.GetDirectoryName(path);
        }
    }
}