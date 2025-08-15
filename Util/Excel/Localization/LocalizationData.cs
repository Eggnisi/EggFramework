#region

//文件创建者：Egg
//创建时间：11-10 11:33

#endregion

using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EggFramework.Util.Localization
{
    public sealed class LocalizationData : ScriptableObject
    {
        [LabelText("目标Excel表")]
        public string ExcelTable;

        [LabelText("语言包")] public List<LocalizationPackage> Packages = new();
    }
}