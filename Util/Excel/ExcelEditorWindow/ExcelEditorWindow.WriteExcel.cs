#region

//文件创建者：Egg
//创建时间：10-30 10:45

#endregion

#if UNITY_EDITOR


using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EggFramework.Util.Excel
{
    public sealed partial class ExcelEditorWindowView
    {
        private TextAsset WriteExcel(ExcelTableConfig tableConfig)
        {
            var path = $"{Setting.ExcelDataPathRoot}/{tableConfig.ConfigName}/{tableConfig.ConfigName}.csv";
            DirectoryUtil.MakeSureDirectory($"{Setting.ExcelDataPathRoot}/{tableConfig.ConfigName}");
            if (File.Exists(path))
            {
                var newPath = path.Replace(".csv", "_") +
                              DateTime.Now.ToString(CultureInfo.InvariantCulture).Replace("/", "").Replace(" ", "")
                                  .Replace(":", "") +
                              ".csv";
                var currentDirectory = Directory.GetCurrentDirectory();
                File.Move(Path.Combine(currentDirectory, path), Path.Combine(currentDirectory, newPath));
            }

            File.WriteAllText(path, ExcelUtil.GetTableHeader(tableConfig));
            AssetDatabase.ImportAsset(path);
            AssetDatabase.Refresh();
            return AssetDatabase.LoadAssetAtPath<TextAsset>(path);
        }
    }
}
#endif