#region

//文件创建者：Egg
//创建时间：11-10 11:33

#endregion

using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EggFramework.Util.Localization
{
    [Serializable]
    public sealed class LocalizationItem
    {
        [LabelText("唯一标识")]
        public string         EntryKey;
        [LabelText("值")]
        public string         Value;
    }
}