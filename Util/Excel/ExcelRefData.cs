#region

//文件创建者：Egg
//创建时间：10-29 08:21

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace EggFramework.Util.Excel
{
    public sealed class ExcelRefData : ScriptableObject
    {
        [LabelText("Excel文本数据引用")] public List<ExcelDataRef> ExcelDataRefs = new();

#if UNITY_EDITOR
        public void Refresh()
        {
            ExcelDataRefs.Clear();
            var types = TypeUtil.GetDerivedClasses(typeof(IExcelEntity));
            foreach (var type in types)
            {
                var folderPath = $"Assets/Excel/Text/{type.Name}";
                var filePath   = $"{folderPath}/{type.Name}.csv";
                var text       = AssetDatabase.LoadAssetAtPath<TextAsset>(filePath);
                if (!text)
                {
                    DirectoryUtil.MakeSureDirectory(folderPath);
                    File.WriteAllText(filePath, string.Empty);
                    AssetDatabase.ImportAsset(filePath);
                    text = AssetDatabase.LoadAssetAtPath<TextAsset>(filePath);
                }

                ExcelDataRefs.Add(new ExcelDataRef
                {
                    Name      = type.Name,
                    TextAsset = text
                });
            }
        }
#endif
    }

    [Serializable]
    public struct ExcelDataRef
    {
        [LabelText("表名")]  public string    Name;
        [LabelText("表资产")] public TextAsset TextAsset;
    }
}