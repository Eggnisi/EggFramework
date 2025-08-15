#region

//文件创建者：Egg
//创建时间：11-20 10:50

#endregion

#if UNITY_EDITOR


using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EggFramework.Util.Excel
{
    public class TextWatcher : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            var data = ResUtil.GetAsset<ExcelRefData>();
            if (!data) return;
            var pairs = data.ExcelDataRefs
                .Select(dataRef => (AssetDatabase.GetAssetPath(dataRef.TextAsset), GetTypeByName(dataRef.Name)))
                .ToList();
            var flag = false;
            foreach (var asset in importedAssets)
            {
                var pair = pairs.Find(pair => pair.Item1 == asset);
                if (!string.IsNullOrEmpty(pair.Item1))
                {
                    flag = true;
                    ExcelUtil.ReadDataByExcelEntityType(pair.Item2);
                }
            }

            if (flag) PythonUtil.RefreshEncode();
        }

        static Type GetTypeByName(string name)
        {
            var types = TypeUtil.GetDerivedClasses(typeof(IExcelEntity));
            return types.Find(tp => tp.Name == name);
        }
    }
}
#endif