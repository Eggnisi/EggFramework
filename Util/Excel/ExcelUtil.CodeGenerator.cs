#region

//文件创建者：Egg
//创建时间：03-22 06:00

#endregion

#if UNITY_EDITOR


using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EggFramework.Util.Excel
{
    public static partial class ExcelUtil
    {
        private static int GetFlattenColCount(List<ExcelColConfig> configs, List<string> colContents, string colPath)
        {
            var colCount = 0;
            var setting  = StorageUtil.LoadFromSettingFile(nameof(ExcelSetting), new ExcelSetting());
            foreach (var tableConfigColumn in configs)
            {
                if (TypeUtil.DefaultTypes.Contains(tableConfigColumn.ExcelColType) ||
                    TypeUtil.ResTypes.Contains(tableConfigColumn.ExcelColType) ||
                    TypeUtil.UnityStructTypes.Contains(tableConfigColumn.ExcelColType))
                {
                    colContents.Add(tableConfigColumn.IsList
                        ? $"{colPath}{TABLE_HEAD_SIGN}{tableConfigColumn.Name}(List<{tableConfigColumn.ExcelColType}>)"
                        : $"{colPath}{TABLE_HEAD_SIGN}{tableConfigColumn.Name}({tableConfigColumn.ExcelColType})");
                    colCount++;
                }
                else if (ExcelTypes.Contains(tableConfigColumn.ExcelColType))
                {
                    var structConfig = setting.Configs.Find(con => con.TypeName == tableConfigColumn.ExcelColType);
                    colCount += GetFlattenColCount(structConfig.Columns, colContents,
                        colPath + $"{TABLE_HEAD_SIGN}{tableConfigColumn.Name}({structConfig.TypeName})->");
                }
                else
                {
                    Debug.LogError("没有找到对应类型的ExcelCol成员");
                }
            }

            return colCount;
        }

        public static string GetTableHeader<T>() where T : IExcelEntity
        {
            var configs = StorageUtil.LoadFromSettingFile(nameof(ExcelTableConfig) + "s", new List<ExcelTableConfig>());
            foreach (var config in configs)
            {
                if (config.ConfigName == typeof(T).Name)
                {
                    return GetTableHeader(config);
                }
            }

            return string.Empty;
        }
        
        public static string GetTableHeader(ExcelTableConfig tableConfig)
        {
            var colNames = new List<string>();
            GetFlattenColCount(tableConfig.Columns, colNames, "");
            var builder        = new StringBuilder();
            var lastColContent = "";
            var depth          = colNames.Max(colName => colName.Split("->").Length);
            for (var i = 0; i < depth; i++)
            {
                for (var index = 0; index < colNames.Count; index++)
                {
                    var colName = colNames[index];
                    var colTree = colName.Split("->");

                    if (colTree.Length < depth - i ||
                        colTree[i - depth + colTree.Length] == lastColContent)
                    {
                        builder.Append(index == colNames.Count - 1 ? "" : ",");
                        continue;
                    }

                    var sampleContent = colTree[i - depth + colTree.Length];

                    lastColContent = sampleContent;
                    if (index == colNames.Count - 1)
                        builder.Append(sampleContent);
                    else builder.Append(sampleContent + ",");
                }

                builder.Append("\n");
            }

            return builder.ToString();
        }
    }
}
#endif