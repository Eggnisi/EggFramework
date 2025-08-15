#region

//文件创建者：Egg
//创建时间：11-01 11:28

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using EggFramework.Util;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EggFramework.Modules.UI
{
    [Serializable]
    public sealed class UIFormConfig
    {
        public string Name;
        public string AssetName;
        public int    Priority;
        [ValueDropdown("GetUIFormLogicNames")]
        public string UIFormLogic;

        private IEnumerable<string> GetUIFormLogicNames()
        {
            return TypeUtil.GetDerivedClasses(typeof(IUIFormLogic)).Select(type => type.Name);
        }
    }
}