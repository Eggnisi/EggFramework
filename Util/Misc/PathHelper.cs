#region

//文件创建者：Egg
//创建时间：08-15 07:54

#endregion


using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EggFramework.Util
{
    public static class PathHelper
    {
        public static string GetAbsoluteAssetsPath() => Application.dataPath;
        public static string GetAbsoluteProjectPath() => GetParentDirectory(GetAbsoluteAssetsPath());
        public static string Get3rdDirectoryPath() => Path.Combine(GetFrameworkDirectoryPath(), "3rd");

        public static string GetFrameworkDirectoryPath()
        {
            var relativePath = FindAssemblyDefinition("EggFramework.asmdef");
            return GetParentDirectory(Path.Combine(GetAbsoluteProjectPath(), relativePath));
        }

        public static string BeautifyPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            // 特殊处理URL和网络路径
            if (path.StartsWith("http://") || path.StartsWith("https://"))
                return path.Replace('\\', '/');

            var  result         = new StringBuilder(path.Length);
            bool isNetworkPath  = path.StartsWith(@"\\");  // 检测网络路径
            bool isRooted       = Path.IsPathRooted(path); // 检测绝对路径
            char prevChar       = '\0';
            bool hasDriveLetter = false;

            // 处理Windows驱动器号 (如 C:)
            if (path.Length >= 2 && char.IsLetter(path[0]) && path[1] == ':')
            {
                result.Append(char.ToUpper(path[0])).Append(':');
                prevChar       = ':';
                hasDriveLetter = true;
            }
            else if (isNetworkPath)
            {
                // 保留网络路径前缀
                result.Append(@"\\");
                prevChar = '\\';
            }
            else if (isRooted)
            {
                // 处理Unix风格根路径
                result.Append('\\');
                prevChar = '\\';
            }

            // 遍历处理路径的每个字符
            for (int i = hasDriveLetter ? 2 : (isNetworkPath ? 2 : 0); i < path.Length; i++)
            {
                char current = path[i];

                // 跳过重复的分隔符
                if ((current == '/' || current == '\\') && (prevChar == '/' || prevChar == '\\'))
                {
                    continue;
                }

                // 统一替换斜杠为反斜杠
                if (current == '/')
                {
                    current = '\\';
                }

                // 跳过路径末尾的分隔符（除非是根路径）
                if (i == path.Length - 1 && (current == '\\' || current == '/') &&
                    !(isRooted && result.Length == 1))
                {
                    continue;
                }

                result.Append(current);
                prevChar = current;
            }

            // 处理特殊空路径情况
            if (result.Length == 0)
            {
                return "\\";
            }

            return result.ToString();
        }

        private static string FindAssemblyDefinition(string fileName)
        {
            var guids = AssetDatabase.FindAssets($"t:AssemblyDefinitionAsset");

            if (guids.Length > 0)
            {
                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    if (Path.GetFileName(path) == Path.GetFileName(fileName))
                    {
                        return path;
                    }
                }
            }

            Debug.LogWarning($"未找到文件: {fileName}");
            return null;
        }

        private static string GetParentDirectory(string path)
        {
            try
            {
                var directory = new DirectoryInfo(path);
                var parent    = directory.Parent;
                return parent?.FullName;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"获取上级目录失败: {e.Message}");
                return null;
            }
        }
    }
}