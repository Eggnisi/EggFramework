#region

//文件创建者：Egg
//创建时间：03-22 03:19

#endregion

#if UNITY_EDITOR


using System.Collections.Generic;
using System.IO;
using System.Text;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace EggFramework.Util.Excel
{
    public abstract class ExcelDataView<TR, T> : ScriptableObject where TR : ExcelEntityRef<T> where T : IExcelEntity
    {
        [TableList(ShowIndexLabels = true)] public List<TR> DataViews = new();

        [Button(ButtonSizes.Large)]
        public void Save()
        {
            var setting = StorageUtil.LoadFromSettingFile(nameof(ExcelSetting), new ExcelSetting());
            var builder = new StringBuilder();
            var header  = ExcelUtil.GetTableHeader<T>();
            builder.Append(header);
            foreach (var data in DataViews)
            {
                foreach (var se in data.RawData)
                {
                    builder.Append(se);
                    builder.Append(',');
                }

                builder.Append('\n');
            }

            DirectoryUtil.MakeSureDirectory($"{setting.ExcelDataPathRoot}/{typeof(T).Name}");
            File.WriteAllText($"{setting.ExcelDataPathRoot}/{typeof(T).Name}/{typeof(T).Name}.csv", builder.ToString());
            AssetDatabase.Refresh();
        }
    }
}
#endif