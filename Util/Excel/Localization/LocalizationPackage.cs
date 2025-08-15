#region

//文件创建者：Egg
//创建时间：11-10 12:38

#endregion

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EggFramework.Util.Localization
{
    [Serializable]
    public sealed class LocalizationPackage
    {
        [LabelText("语言")]
        public SystemLanguage Language;
        [LabelText("被本地化的字段")]
        public List<LocalizationCol> LocalizationCols = new();
    }
}