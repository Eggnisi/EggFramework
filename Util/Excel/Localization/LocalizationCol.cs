#region

//文件创建者：Egg
//创建时间：11-10 11:40

#endregion

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace EggFramework.Util.Localization
{
    [Serializable]
    public sealed partial class LocalizationCol
    {
        [LabelText("字段名")]
        public string ColName;
        [LabelText("默认语言数据")]
        public List<LocalizationItem> LocalizationItems = new();
    }
}