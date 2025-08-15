#region

//文件创建者：Egg
//创建时间：05-15 12:24

#endregion

#if UNITY_EDITOR

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using EggFramework.Util;
using EggFramework.Util.EggCMD;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace EggFramework
{
    public static class PythonUtil
    {
        public static void ResizeTextures(string folderPath = "")
        {
            if (string.IsNullOrEmpty(folderPath)) folderPath = Application.dataPath;
            var scriptPath                                   = FindScript("texture_resizer");
            if (string.IsNullOrEmpty(scriptPath)) return;
            RunPythonScript(scriptPath, new List<string> { folderPath }, false, out var stdOut, out _);
            Debug.Log($"处理结果：{stdOut}");
        }

        public static string CountCodeLine()
        {
            var scriptPath = FindScript("code_counter");
            if (string.IsNullOrEmpty(scriptPath)) return string.Empty;
            var targetFolder = Path.Combine(Application.dataPath, "Scripts");
            RunPythonScript(scriptPath, new List<string> { targetFolder }, false, out var stdOut, out _);
            return stdOut;
        }

        public static void RefreshEncode()
        {
            var scriptPath = FindScript("gbk2utf8");
            if (string.IsNullOrEmpty(scriptPath)) return;
            var targetFolder = Path.Combine(Application.dataPath, "Excel/Text");
            RunPythonScript(scriptPath, new List<string> { targetFolder }, false, out var stdOut, out _);
            CommandLogger.Log(stdOut);
            AssetDatabase.Refresh();
        }
        
        public static void RefreshScriptsEncode()
        {
            var scriptPath = FindScript("gbk2utf8");
            if (string.IsNullOrEmpty(scriptPath)) return;
            var targetFolder = Path.Combine(Application.dataPath, "Scripts");
            RunPythonScript(scriptPath, new List<string> { targetFolder }, false, out var stdOut, out _);
            CommandLogger.Log(stdOut);
            AssetDatabase.Refresh();
        }

        public static void UpdateSheetAuthority(string appId, string appSecret, string sheetToken)
        {
            var scriptPath = FindScript("sheet_authority_updater");
            if (string.IsNullOrEmpty(scriptPath)) return;

            var requirementsPath = GetRequirementsPath(scriptPath);
            if (!InstallDependencies(requirementsPath)) return;

            var exitCode = RunPythonScript(scriptPath,
                new List<string> { appId, appSecret, @"Assets\Settings\EggFramework\SettingConfig.json", sheetToken },
                true,
                out _,
                out _);
            if (exitCode != 0) CommandLogger.LogError("权限更新失败");
            else CommandLogger.Log("权限更新完毕");
            AssetDatabase.Refresh();
        }

        public static void PullSheet(string appId, string appSecret, string sheetToken)
        {
            var scriptPath = FindScript("sheet_puller");
            if (string.IsNullOrEmpty(scriptPath)) return;

            var requirementsPath = GetRequirementsPath(scriptPath);
            if (!InstallDependencies(requirementsPath)) return;

            var exitCode = RunPythonScript(scriptPath,
                new List<string> { appId, appSecret, "Assets/Excel/Text", sheetToken },
                true,
                out _,
                out _);
            if (exitCode != 0) CommandLogger.LogError("下载失败，请重试");
            else CommandLogger.Log("下载完毕");
            AssetDatabase.Refresh();
        }

        public static void UpdateRemoteSheet(string appId, string appSecret, string branch)
        {
            var scriptPath = FindScript("sheet_creator");
            if (string.IsNullOrEmpty(scriptPath)) return;

            var requirementsPath = GetRequirementsPath(scriptPath);
            if (!InstallDependencies(requirementsPath)) return;
            var exitCode = RunPythonScript(scriptPath,
                new List<string>
                {
                    appId, appSecret,
                    @"Assets\Settings\EggFramework\SettingConfig.json", branch
                },
                true,
                out _,
                out _);
            if (exitCode != 0) CommandLogger.LogError("更新失败，请重试");
            else CommandLogger.Log("更新完毕");
            AssetDatabase.Refresh();
        }

        // 查找目标 Python 脚本
        private static string FindScript(string scriptName)
        {
            var files = DirectoryUtil.GetFileEndWith(".py");
            foreach (var file in files)
            {
                if (DirectoryUtil.ExtractName(file) == scriptName)
                {
                    return $"{Directory.GetCurrentDirectory()}/{file}";
                }
            }

            CommandLogger.LogError($"未找到 {scriptName} 脚本");
            return null;
        }

        // 获取依赖文件路径
        public static string GetRequirementsPath(string scriptPath)
        {
            var scriptDir = Path.GetDirectoryName(scriptPath);
            return Path.Combine(scriptDir, "requirements.txt");
        }

        // 安装 Python 依赖
        public static bool InstallDependencies(string requirementsPath)
        {
            if (!File.Exists(requirementsPath))
            {
                CommandLogger.Log("未找到依赖文件，跳过安装");
                return true;
            }

            CommandLogger.Log("正在检查依赖安装情况");
            var exitCode = RunCommand($"python -m pip install -r \"{requirementsPath}\"", true, out _, out _);

            if (exitCode != 0)
            {
                CommandLogger.LogError($"依赖安装失败（代码 {exitCode}）");
                return false;
            }

            CommandLogger.Log("依赖就绪");
            return true;
        }

        // 执行 Python 脚本
        private static int RunPythonScript(string scriptPath, List<string> paramList, bool showWindow,
            out string stdOut,
            out string stdErr)
        {
            var builder = new StringBuilder();
            builder.Append(scriptPath);
            paramList.ForEach(param => builder.Append($" {param}"));
            return RunCommand($"python {builder}", showWindow, out stdOut, out stdErr);
        }

        // 通用命令执行方法
        public static int RunCommand(string command, bool showWindow, out string stdOut, out string stdErr)
        {
            var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName               = "cmd.exe",
                Arguments              = $"/c {command}",
                WindowStyle            = showWindow ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden,
                UseShellExecute        = showWindow,
                CreateNoWindow         = !showWindow,
                RedirectStandardOutput = !showWindow,
                RedirectStandardError  = !showWindow,
            };
            if (!showWindow)
            {
                process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                process.StartInfo.StandardErrorEncoding  = Encoding.UTF8;
            }

            stdOut = string.Empty;
            stdErr = string.Empty;
            process.Start();
            if (!showWindow)
            {
                stdOut = process.StandardOutput.ReadToEnd();
                stdErr = process.StandardError.ReadToEnd();
            }

            process.WaitForExit();

            return process.ExitCode;
        }
    }
}
#endif