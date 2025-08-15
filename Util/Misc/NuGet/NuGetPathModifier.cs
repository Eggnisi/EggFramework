#region

//文件创建者：Egg
//创建时间：08-15 07:47

#endregion

using System.IO;
using System.Reflection;
using NugetForUnity.Configuration;
using UnityEditor;
using UnityEngine;

namespace EggFramework.Util
{
    public static class NuGetPathModifier
    {
        [InitializeOnLoadMethod]
        private static void InjectNuGetConfigPath()
        {
            Debug.Log($"自动设置NuGet包管理器配置文件路径:{PathHelper.BeautifyPath(PathHelper.Get3rdDirectoryPath())}");
            typeof(ConfigurationManager).GetProperty("NugetConfigFilePath",
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                ?.SetMethod
                .Invoke(null, new object[] { Path.Combine(PathHelper.Get3rdDirectoryPath(), "NuGet.config") });
        }
    }
}