#region

//文件创建者：Egg
//创建时间：11-01 11:28

#endregion

using System;
using System.Collections.Generic;

namespace EggFramework.Modules.UI
{
    [Serializable]
    public sealed class UIGroupConfig
    {
        public string             Name;
        public int                Priority;
        public List<UIFormConfig> UIFormConfigs = new();
    }
}