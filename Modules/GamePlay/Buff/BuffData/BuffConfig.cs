#region

//文件创建者：Egg
//创建时间：02-21 11:24

#endregion


using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace EggFramework
{
    [Serializable]
    public sealed class BuffConfig
    {
        [LabelText("Buff引用文件路径")] public string       BuffRefFilePath      = "Assets/Data/Buff/BuffRef";
        [LabelText("Buff常量代码路径")] public string       BuffConstantCodePath = "Assets/Scripts/Generator/Buff";
        [LabelText("自定义钩子")]      public List<string> CustomTriggers       = new();
        [LabelText("自定义Tag")]     public List<string> CustomTags           = new();
    }
}