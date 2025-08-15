#region

//文件创建者：Egg
//创建时间：07-29 08:09

#endregion

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace EggFramework
{
    [Serializable]
    public sealed class QuickShot
    {
        public int Id;
        
        [ShowInInspector] public Dictionary<ICanQuickShot, Dictionary<string, object>> QuickShotData = new();
    }
}