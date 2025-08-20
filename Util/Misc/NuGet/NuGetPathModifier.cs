#region

//文件创建者：Egg
//创建时间：08-15 07:47

#endregion

#if UNITY_EDITOR


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
            var rootPath = PathHelper.GetAbsoluteAssetsPath();
            if (File.Exists(Path.Combine(rootPath, "NuGet.config")))
            {
                File.Delete(Path.Combine(rootPath, "NuGet.config"));
                File.Delete(Path.Combine(rootPath, "NuGet.config") + ".meta");
            }

            if (File.Exists(Path.Combine(rootPath, "packages.config")))
            {
                File.Delete(Path.Combine(rootPath, "packages.config"));
                File.Delete(Path.Combine(rootPath, "packages.config") + ".meta");
            }
            
            if (Directory.Exists(Path.Combine(rootPath, "Packages")))
            {
                Directory.Delete(Path.Combine(rootPath, "Packages"));
                File.Delete(Path.Combine(rootPath, "Packages") + ".meta");
            }

            typeof(ConfigurationManager).GetProperty("NugetConfigFilePath",
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                ?.SetMethod
                .Invoke(null, new object[] { Path.Combine(PathHelper.Get3rdDirectoryPath(), "NuGet.config") });
        }
    }
}
#endif