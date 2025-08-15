#region

//文件创建者：Egg
//创建时间：11-10 01:10

#endregion

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace EggFramework.Util.Localization
{
    [Serializable]
    public sealed class LocalizationConfig
    {
        [LabelText("已经生成本地化数据的表格")]
        [ReadOnly]
        public List<string> LocalizationTables = new();
    }
}