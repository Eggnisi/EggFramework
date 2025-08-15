#region

//文件创建者：Egg
//创建时间：11-01 11:27

#endregion

using System;
using System.Collections.Generic;
using UnityEngine;

namespace EggFramework.Modules.UI
{
    [CreateAssetMenu(menuName = "Data/UIConfig")]
    public sealed class UIConfig : ScriptableObject
    {
        public List<UIGroupConfig> UIGroupConfigs = new();
    }
}